using UnityEngine;
using System.Collections;

public class ProjectileSpawner : MonoBehaviour
{
	public GameObject prefab;
	public float force;

	public void Spawn()
	{
		GameObject missile = (GameObject)Instantiate(prefab, transform.position, transform.rotation);
		Rigidbody rb = missile.GetComponent<Rigidbody>();
		rb.AddForce(transform.forward * force, ForceMode.Impulse);
	}
}
