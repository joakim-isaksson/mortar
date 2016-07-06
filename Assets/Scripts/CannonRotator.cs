// Uncomment to test without VR controllers
//#define _DEBUG_CONTROLLER_

using UnityEngine;
using System.Collections;

public class CannonRotator : MonoBehaviour
{
	public Component control;
	public Component wheel;
	public Component cannon;
	public Vector3 axis;
	public float multiplier;
	public float maxAngularSpeed;
	public float minAngle, maxAngle;

	float angle;
	bool grabbed;
	SteamVR_TrackedObject controller;
	float nominalDist;
	int axisIndex;

	void Start()
	{
		Vector3 handleLocalPos = control.transform.InverseTransformPoint(transform.position);
		nominalDist = new Vector2(handleLocalPos.x, handleLocalPos.y).magnitude;

		for (int i = 0; i < 3; i++)
		{
			if (axis[i] != 0) axisIndex = i;
		}
	}

	void Update()
	{
		if (grabbed)
		{
#if (!_DEBUG_CONTROLLER_)
			if (controller.index == SteamVR_TrackedObject.EIndex.None)
			{
				stopRotating();
				return;
			}

			int index = (int)controller.index;
			if (SteamVR_Controller.Input(index).GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
			{
				stopRotating();
				return;
			}
#endif

			Vector3 controllerLocalPos = control.transform.InverseTransformPoint(controller.transform.position);
			Vector2 toControllerDir = new Vector2(controllerLocalPos.x, controllerLocalPos.y);
			float controllerDist = toControllerDir.magnitude;
			toControllerDir /= controllerDist;

			float angleController = Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(new Vector2(0, 1), toControllerDir));
			float sign = Mathf.Sign(Vector3.Cross(new Vector3(0, 1, 0), new Vector3(toControllerDir.x, toControllerDir.y, 0)).z);

			float angleDiff = Mathf.DeltaAngle(wheel.transform.eulerAngles.z, angleController * sign);

			float f = Mathf.Clamp(controllerDist / nominalDist, 0, 1);
			float maxAngSpeedPerFrame = maxAngularSpeed * Time.deltaTime * f;
			angleDiff = Mathf.Clamp(angleDiff, -maxAngSpeedPerFrame, maxAngSpeedPerFrame);

			float curAngle = cannon.transform.eulerAngles[axisIndex];

			float nextAngle = curAngle + axis[axisIndex] * angleDiff * multiplier;
			if (nextAngle > maxAngle) nextAngle = maxAngle;
			else if (nextAngle < minAngle) nextAngle = minAngle;

			angleDiff = (nextAngle - curAngle) / (axis[axisIndex] * multiplier);

			wheel.transform.eulerAngles = new Vector3(wheel.transform.eulerAngles.x, wheel.transform.eulerAngles.y, wheel.transform.eulerAngles.z + angleDiff);
			cannon.transform.Rotate(axis * angleDiff * multiplier);
		}
	}

	public void OnTriggerStay(Collider other)
	{
		if (other.tag == "Controller")
		{
			SteamVR_TrackedObject controller = other.GetComponent<SteamVR_TrackedObject>();
			if (controller != null)
			{
#if (!_DEBUG_CONTROLLER_)
				if (controller.index == SteamVR_TrackedObject.EIndex.None) return;
				if (SteamVR_Controller.Input((int)controller.index).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
#endif
				{
					if (!grabbed)
					{
						grabbed = true;
						this.controller = controller;
#if (!_DEBUG_CONTROLLER_)
						SteamVR_Controller.Input((int)controller.index).TriggerHapticPulse(3999);
#endif
					}
				}
			}
		}
	}

	public void stopRotating()
	{
#if (!_DEBUG_CONTROLLER_)
		if (controller != null && controller.index != SteamVR_TrackedObject.EIndex.None)
		{
			SteamVR_Controller.Input((int)controller.index).TriggerHapticPulse(3999);
		}
#endif

		grabbed = false;
		controller = null;
	}
}
