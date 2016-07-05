using UnityEngine;
using System.Collections;

public class ExplodeAtImpact : MonoBehaviour
{

	void OnTriggerEnter(Collider collider)
	{
		Debug.Log("Collide!");
		Destroy(gameObject);
	}
}
