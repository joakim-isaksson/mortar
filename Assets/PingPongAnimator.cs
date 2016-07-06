using UnityEngine;
using System.Collections;

public class PingPongAnimator : MonoBehaviour
{
	public Transform PointA;
	public Transform PointB;
	public float TimeFromAToB;
	public float TimeFromBToA;

	[HideInInspector]
	public bool Animating
	{
		get { return animating; }
	}
	bool animating;

	public void StartAnimation()
	{
		if (animating) return;
		StartCoroutine(PingPongAnimation());
	}

	IEnumerator PingPongAnimation()
	{
		animating = true;
		yield return StartCoroutine(Tween(PointA, PointB, TimeFromAToB));
		yield return StartCoroutine(Tween(PointB, PointA, TimeFromBToA));
		animating = false;
	}

	IEnumerator Tween(Transform from, Transform to, float duration)
	{
		float passed = 0;
		float time = 0;
		while (time < 1.0f)
		{
			passed += Time.deltaTime;
			time = passed / duration;
			transform.position = Vector3.Lerp(from.position, to.position, time);
			yield return null;
		}
	}
}
