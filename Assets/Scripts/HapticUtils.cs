using UnityEngine;
using System.Collections;

public class HapticUtils
{
	public delegate float StrengthFunction(float t);

	public static float MaxStrength(float t) { return 1; }

	public static IEnumerator LongVibrationBoth(float length, StrengthFunction f = null)
	{
		// Default to constant function
		if (f == null) f = MaxStrength;

		int left = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
		int right = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);

		for (float i = 0; i < length; i += Time.deltaTime)
		{
			float strength = f(i / length);

			if (left != -1) SteamVR_Controller.Input(left).TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
			if (right != -1 && left != right) SteamVR_Controller.Input(right).TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
			yield return null;
		}
	}
}
