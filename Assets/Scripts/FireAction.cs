using UnityEngine;
using System.Collections;

public class FireAction : MonoBehaviour
{
	public Transform SpawnPoint;
	public GameObject MissilePrefab;
	public float FiringForce;
	public Vector3 TriggerPullPoint;
	public float TriggerPullTime;
	public float TriggerReturnTime;

	bool cooldown;

	public void OnTriggerStay(Collider other)
	{
		if (other.tag == "Controller")
		{
			SteamVR_TrackedObject controller = other.GetComponent<SteamVR_TrackedObject>();
			if (controller != null && controller.index != SteamVR_TrackedObject.EIndex.None)
			{
				if (SteamVR_Controller.Input((int)controller.index).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
				{
					TryToFire();
				}
			}
		}
	}

	void TryToFire()
	{
		Debug.Log("Trying to fire");

		// Check cooldown
		if (cooldown) return;
		cooldown = true;

		Debug.Log("Firing");

		StartCoroutine(AnimateTrigger());
		SpawnMissile();
	}

	void SpawnMissile()
	{
		GameObject missile = (GameObject)Instantiate(MissilePrefab, SpawnPoint.position, SpawnPoint.rotation);
		Rigidbody rb = missile.GetComponent<Rigidbody>();
		rb.AddForce(SpawnPoint.forward * FiringForce, ForceMode.Impulse);
	}

	IEnumerator AnimateTrigger()
	{
		Debug.Log("Starting animation");
		Vector3 startPosition = transform.position;
		yield return StartCoroutine(MoveObject(transform, TriggerPullPoint, TriggerPullTime));
		yield return StartCoroutine(MoveObject(transform, startPosition, TriggerReturnTime));
		Debug.Log("Animation ended");
		cooldown = false;
	}

	IEnumerator MoveObject(Transform transform, Vector3 endPos, float time)
	{
		Debug.Log("Animating...");
		Vector3 startPos = transform.position;
		float i = 0.0f;
		float rate = 1.0f / time;
		while (i < 1.0f)
		{
			i += Time.deltaTime * rate;
			transform.position = Vector3.Lerp(startPos, endPos, i);
			yield return null;
		}
	}
}
