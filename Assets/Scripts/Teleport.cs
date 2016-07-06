using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour
{

	void Start()
	{

	}

	private void checkInput()
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
				Debug.Log("press, loc: " + touchLoc.ToString("f4"));
			}

		}

	}

	void Update()
	{

		GameObject camera = GameObject.FindWithTag("MainCamera");

		checkInput();

		foreach (Transform loc in transform)
		{

		}
	}
}
