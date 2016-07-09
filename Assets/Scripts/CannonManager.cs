using UnityEngine;

public class CannonManager : MonoBehaviour, IDestroyableObject
{
	public Component Barrel;
	public float BarrelStartAngle;

	public GameObject CannonExplosionPrefab;

	GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType<GameManager>();
		Barrel.transform.localRotation = Quaternion.Euler(BarrelStartAngle, 0, 0);
	}

	public void OnDestroyObject()
	{
		Instantiate(CannonExplosionPrefab, transform.position, transform.rotation);
		gameManager.OnCannonDestroyed(gameObject);
		Destroy(gameObject);
	}
}
