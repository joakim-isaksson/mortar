// Uncomment to test without VR controllers
//#define _DEBUG_CONTROLLER_

using UnityEngine;
using System.Collections;

public class CannonRotator : MonoBehaviour
{
	public Component Control;
	public Component Wheel;
	public Component Cannon;
	public Vector3 Axis;
	public float Multiplier;
	public float MaxAngularSpeed;
	public float MinAngle, MaxAngle;

	float angle;
	bool grabbed;
	SteamVR_TrackedObject controller;
	float nominalDist;
	int axisIndex;

	void Start()
	{
		Vector3 handleLocalPos = Control.transform.InverseTransformPoint(transform.position);
		nominalDist = new Vector2(handleLocalPos.x, handleLocalPos.y).magnitude;

		for (int i = 0; i < 3; i++)
		{
			if (Axis[i] != 0) axisIndex = i;
		}

		angle = Cannon.transform.localEulerAngles[axisIndex];

		if (MinAngle > MaxAngle)
		{
			float tmp = MinAngle;
			MinAngle = MaxAngle;
			MaxAngle = tmp;
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

			Vector3 controllerLocalPos = Control.transform.InverseTransformPoint(controller.transform.position);
			Vector2 toControllerDir = new Vector2(controllerLocalPos.x, controllerLocalPos.y);
			float controllerDist = toControllerDir.magnitude;
			toControllerDir /= controllerDist;

			float angleController = Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(new Vector2(0, 1), toControllerDir));
			float sign = Mathf.Sign(Vector3.Cross(new Vector3(0, 1, 0), new Vector3(toControllerDir.x, toControllerDir.y, 0)).z);

			float angleDiff = Mathf.DeltaAngle(Wheel.transform.eulerAngles.z, angleController * sign);

			float f = Mathf.Clamp(controllerDist / nominalDist, 0, 1);
			float maxAngSpeedPerFrame = MaxAngularSpeed * Time.deltaTime * f;
			angleDiff = Mathf.Clamp(angleDiff, -maxAngSpeedPerFrame, maxAngSpeedPerFrame);

			float nextAngle = angle + Axis[axisIndex] * angleDiff * Multiplier;
			if (nextAngle > MaxAngle) nextAngle = MaxAngle;
			else if (nextAngle < MinAngle) nextAngle = MinAngle;

			angleDiff = (nextAngle - angle) / (Axis[axisIndex] * Multiplier);

			angle = nextAngle;

			Wheel.transform.localEulerAngles = Wheel.transform.localEulerAngles + new Vector3(0, 0, angleDiff);

			Vector3 preRot = Cannon.transform.localEulerAngles;

			// Things might break here if axis == -1. If they do multiply the relevant angle by -1.
			Cannon.transform.localEulerAngles = Axis * angle;

			Debug.Log("pre: " + preRot.ToString("f4") + ", post: " + Cannon.transform.localEulerAngles.ToString("f4") + ", angle: " + angle);

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
