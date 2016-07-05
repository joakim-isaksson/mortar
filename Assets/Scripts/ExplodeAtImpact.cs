using UnityEngine;
using System.Collections;

public class ExplodeAtImpact : MonoBehaviour
{

	void OnTriggerEnter(Collider collider)
	{
		Destroy(gameObject);
	}
}
