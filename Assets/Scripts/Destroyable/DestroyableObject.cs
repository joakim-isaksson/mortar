using UnityEngine;
using System.Collections.Generic;

public class DestroyableObject : MonoBehaviour
{
	public static List<IDestroyableObject> DestroyableObjects = new List<IDestroyableObject>();

	void Start()
	{
		DestroyableObjects.Add(GetComponent<IDestroyableObject>());
	}
}
