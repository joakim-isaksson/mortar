using UnityEngine;
using System.Collections;

public class StopRotating : MonoBehaviour
{
	public CannonRotator rotator;

	public void OnTriggerExit(Collider other)
	{
		if (other.tag == "Controller") rotator.StopRotating();
	}
}
