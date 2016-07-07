using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour
{
	enum Action
	{
		NOTHING, NEXT, PREVIOUS
	}

	private const float DEADZONE = 0.1f;

	int currentIndex;

	Transform reference
	{
		get
		{
			var top = SteamVR_Render.Top();
			return (top != null) ? top.origin : null;
		}
	}

	void Start()
	{

	}

	private Action CheckInput()
	{
		var leftIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
		var rightIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);

		var index = -1;
		if (leftIndex != -1) index = leftIndex;
		else if (rightIndex != -1) index = rightIndex;

		if (index != -1)
		{
			if (SteamVR_Controller.Input(index).GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
			{
				// FIXME implement previous/next
				/*
				Vector2 touchLoc = SteamVR_Controller.Input(index).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);

				if (touchLoc.x <= -DEADZONE) return Action.PREVIOUS;
				else if (touchLoc.x >= DEADZONE) return Action.NEXT;
				*/

				return Action.NEXT;
			}

		}
		return Action.NOTHING;
	}

	void Update()
	{
		int index = -1;

		switch (CheckInput())
		{
			case Action.PREVIOUS:
				index = Mod(--currentIndex, transform.childCount);
				break;
			case Action.NEXT:
				index = Mod(++currentIndex, transform.childCount);
				break;
		}

		if (index != -1)
		{
			Transform position = transform.GetChild(index);

			var t = reference;
			if (t == null) return;

			Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.localPosition.x, 0.0f, SteamVR_Render.Top().head.localPosition.z);
			t.rotation = position.rotation;
			t.position = position.position - new Vector3(t.GetChild(0).localPosition.x, 0f, t.GetChild(0).localPosition.z) - headPosOnGround;
		}
	}

	private int Mod(int k, int n) { return ((k %= n) < 0) ? k + n : k; }
}
