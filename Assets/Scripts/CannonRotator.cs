using UnityEngine;
using System.Collections;

public class CannonRotator : MonoBehaviour
{
	float angle;
	bool grabbed;
	SteamVR_TrackedObject controller;

	public Component control;
	public Component wheel;
	public Component cannon;
	public Vector3 axis;
	public float multiplier;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (grabbed)
		{
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

			Vector3 controllerLocalPos = control.transform.InverseTransformPoint(controller.transform.position);
			Vector2 toControllerDir = new Vector2(controllerLocalPos.x, controllerLocalPos.y).normalized;

			float angle = Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(new Vector2(0, 1), toControllerDir));
			float sign = Mathf.Sign(Vector3.Cross(new Vector3(0, 1, 0), new Vector3(toControllerDir.x, toControllerDir.y, 0)).z);

			float angleDiff = Mathf.DeltaAngle(wheel.transform.eulerAngles.z, angle * sign);

			wheel.transform.eulerAngles = new Vector3(wheel.transform.eulerAngles.x, wheel.transform.eulerAngles.y, angle * sign);

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

	public void OnTriggerEnter(Collider other)
	{
		Debug.Log("OnTriggerEnter: " + other.tag);
		if (other.tag == "Controller")
		{
			SteamVR_TrackedObject controller = other.GetComponent<SteamVR_TrackedObject>();
			if (controller != null)
			{
				Debug.Log("has TrackedObject");
				if (controller.index == SteamVR_TrackedObject.EIndex.None) return;
				int index = (int)controller.index;

				if (SteamVR_Controller.Input(index).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
				{
					Debug.Log("is pressedDown");
					grabbed = true;
					this.controller = controller;
				}
			}

		}
	}
}
