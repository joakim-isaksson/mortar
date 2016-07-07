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

	void Start()
	{

	}

	private Action checkInput()
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
				Vector2 touchLoc = SteamVR_Controller.Input(index).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);

				if (touchLoc.x <= -DEADZONE) return Action.PREVIOUS;
				else if (touchLoc.x >= DEADZONE) return Action.NEXT;
			}

		}
		return Action.NOTHING;
	}

	void Update()
	{
		GameObject camera = GameObject.FindWithTag("MainCamera");
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
			Debug.Log("teleporting to pos " + index);
			Transform position = transform.GetChild(index);


			Debug.Log("pre camera: " + camera.transform.position.ToString("f4"));
			camera.transform.position = position.position;
			Debug.Log("post camera: " + camera.transform.position.ToString("f4"));
		}
	}

	private int mod(int k, int n) { return ((k %= n) < 0) ? k + n : k; }
}
