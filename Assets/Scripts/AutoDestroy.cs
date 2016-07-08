using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour
{

	public float DestroyAfterSeconds;

	void Start()
	{
		StartCoroutine(WaitAndDestroy());
	}

	IEnumerator WaitAndDestroy()
	{
		yield return new WaitForSeconds(DestroyAfterSeconds);
		Destroy(gameObject);
	}
}
