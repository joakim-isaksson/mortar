using UnityEngine;
using System.Collections;

public class CannonManager : MonoBehaviour
{
	public Component Barrel;
	public float BarrelStartAngle;

	GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType<GameManager>();
		Barrel.transform.rotation = Quaternion.Euler(BarrelStartAngle, 0, 0);
	}

	public void OnDestroy()
	{
		gameManager.OnCannonDestroyed(gameObject);
		Destroy(gameObject);
	}
}
