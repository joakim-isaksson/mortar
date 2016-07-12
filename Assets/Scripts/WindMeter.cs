using UnityEngine;
using System.Collections;

public class WindMeter : MonoBehaviour
{

	public float AngularVelocity;

	GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType<GameManager>();
	}

	void FixedUpdate()
	{
		float rotation = AngularVelocity * gameManager.Wind.magnitude;
		//Debug.Log("gameManager.Wind.sqrMagnitude: " + gameManager.Wind);
		transform.Rotate(transform.up, 5);
	}
}
