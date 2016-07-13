using UnityEngine;
using System.Collections;
using System;

public class ExplodeWhenDestroyed : MonoBehaviour, IDestroyableObject
{
	public GameObject ExplosionPrefab;

	public void OnDestroyObject()
	{
		Instantiate(ExplosionPrefab, transform.position, transform.rotation);
		Destroy(gameObject);
	}
}
