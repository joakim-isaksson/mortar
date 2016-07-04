using UnityEngine;
using System.Collections;

public class RotateToTrajectory : MonoBehaviour
{
	Rigidbody rigidBody;

	void Awake()
	{
		rigidBody = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		rigidBody.MoveRotation(Quaternion.LookRotation(rigidBody.velocity));
	}
}
