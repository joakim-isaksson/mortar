using UnityEngine;
using System.Collections;
using System;

public class CannonManager : MonoBehaviour, IDestroyableObject
{
	public Component Barrel;
	public float BarrelStartAngle;

	GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType<GameManager>();
		Barrel.transform.rotation = Quaternion.Euler(BarrelStartAngle, 0, 0);
	}

	public void OnDestroyObject()
	{
		gameManager.OnCannonDestroyed(gameObject);
		Destroy(gameObject);
	}
}
