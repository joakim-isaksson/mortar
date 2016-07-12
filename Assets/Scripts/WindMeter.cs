using UnityEngine;
using System.Collections;

public class WindMeter : MonoBehaviour
{

	public float MaximumAngularVelocity;

	GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType<GameManager>();
	}

	void FixedUpdate()
	{
		float rotation = MaximumAngularVelocity * gameManager.Wind.magnitude * Time.deltaTime;
		transform.Rotate(transform.up, rotation);
	}
}
