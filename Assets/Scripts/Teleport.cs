using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	private Action checkInput()
	{
		var leftIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
		var rightIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);

		List<int> indices = new List<int>();
		if (leftIndex != -1) indices.Add(leftIndex);
		if (rightIndex != -1) indices.Add(rightIndex);

		foreach (int index in indices)
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

		switch (checkInput())
		{
			case Action.PREVIOUS:
				index = mod(--currentIndex, transform.childCount);
				break;
			case Action.NEXT:
				index = mod(++currentIndex, transform.childCount);
				break;
		}

		if (index != -1)
		{
			Transform location = transform.GetChild(index);
			TeleportLocation locationData = location.GetComponent<TeleportLocation>();

			var t = reference;
			if (t == null) return;

			//Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.localPosition.x, 0.0f, SteamVR_Render.Top().head.localPosition.z);
			//t.rotation = location.rotation;
			t.position = location.position;
			if (locationData.forceOrientation)
			{
				float eyeAngle = SteamVR_Render.Top().head.eulerAngles.y;
				float targetAngle = location.eulerAngles.y;
				t.Rotate(0, targetAngle - eyeAngle, 0);
			}
			else
			{
				t.eulerAngles = Vector3.zero;
			}
			//t.position = position.position - new Vector3(t.GetChild(0).localPosition.x, 0f, t.GetChild(0).localPosition.z) - headPosOnGround;
		}
	}

	private int mod(int k, int n) { return ((k %= n) < 0) ? k + n : k; }
}
