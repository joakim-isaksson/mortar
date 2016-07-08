using UnityEngine;
using System.Collections.Generic;

public class Destroyable : MonoBehaviour
{
	public static List<GameObject> Destroyables = new List<GameObject>();

	void Start()
	{
		Destroyables.Add(gameObject);
	}
}
