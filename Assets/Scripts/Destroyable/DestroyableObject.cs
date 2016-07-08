using UnityEngine;
using System.Collections.Generic;

public class DestroyableObject : MonoBehaviour
{
	public static List<GameObject> DestroyableObjects = new List<GameObject>();

	void Start()
	{
		DestroyableObjects.Add(gameObject);
	}
}
