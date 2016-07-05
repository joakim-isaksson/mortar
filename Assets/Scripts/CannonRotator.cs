//#define _DEBUG_CONTROLLER_

using UnityEngine;
using System.Collections;

public class CannonRotator : MonoBehaviour
{
	float angle;
	bool grabbed;
	SteamVR_TrackedObject controller;
	float nominalDist;

	public Component control;
	public Component wheel;
	public Component cannon;
	public Vector3 axis;
	public float multiplier;
	public float maxAngularSpeed;

	// Use this for initialization
	void Start()
	{
		Vector3 handleLocalPos = control.transform.InverseTransformPoint(transform.position);
		nominalDist = new Vector2(handleLocalPos.x, handleLocalPos.y).magnitude;
	}

	// Update is called once per frame
	void Update()
	{
		if (grabbed)
		{

#if (!_DEBUG_CONTROLLER_)
			if (controller.index == SteamVR_TrackedObject.EIndex.None)
			{
				grabbed = false;
				controller = null;
				return;
			}

			int index = (int)controller.index;
			if (SteamVR_Controller.Input(index).GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
			{
				grabbed = false;
				controller = null;
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

			float normAngDiff = angleDiff;
			//float normAngDiff = Mathf.Max(angleDiff, (angleDiff * controllerDist) / nominalDist);

			float f = Mathf.Clamp(controllerDist / nominalDist, 0, 1);
			float maxAngSpeedPerFrame = maxAngularSpeed * Time.deltaTime * f;
			float limitedNormAngDiff = Mathf.Clamp(normAngDiff, -maxAngSpeedPerFrame, maxAngSpeedPerFrame);

			angleDiff = limitedNormAngDiff;

			Debug.Log("speed: " + normAngDiff + ", limited: " + limitedNormAngDiff);

			wheel.transform.eulerAngles = new Vector3(wheel.transform.eulerAngles.x, wheel.transform.eulerAngles.y, wheel.transform.eulerAngles.z + angleDiff);

			//wheel.transform.eulerAngles = new Vector3(wheel.transform.eulerAngles.x, wheel.transform.eulerAngles.y, angleController * sign);

			//Debug.Log("angle: " + angle + ", sign: " + sign + ", euler: " + wheel.transform.eulerAngles.ToString("f4") + ", diff: " + angleDiff);

			cannon.transform.Rotate(axis * angleDiff * multiplier);
		}
		//if (colliding) transform.Rotate(new Vector3(0, 0, Time.deltaTime * 90));
	}

	/*
	public void OnTriggerExit(Collider other)
	{
		Debug.Log("OnTriggerExit: " + other.tag);
		if (other.tag == "Controller")
		{
			grabbed = false;
			controller = null;
		}
	}
	*/

	public void OnTriggerStay(Collider other)
	{
		Debug.Log("OnTriggerEnter: " + other.tag);
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
					grabbed = true;
					this.controller = controller;
				}
			}

		}
	}
}
