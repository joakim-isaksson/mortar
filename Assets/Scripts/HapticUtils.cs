using UnityEngine;
using System.Collections;

public class HapticUtils
{
	// length is how long the vibration should go for
	// strength is vibration strength from 0-1
	public static IEnumerator LongVibrationBoth(float length, float strength)
	{
		int left = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
		int right = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);

		for (float i = 0; i < length; i += Time.deltaTime)
		{
			if (left != -1) SteamVR_Controller.Input(left).TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
			if (right != -1 && left != right) SteamVR_Controller.Input(right).TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
			yield return null;
		}
	}
}
