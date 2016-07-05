using UnityEngine;
using System.Collections;

public class FireAction : MonoBehaviour
{
	public Transform spawnPoint;
	public GameObject missilePrefab;
	public float force;

	bool firing;

	void Update()
	{
		if (firing)
		{
			SpawnMissile();
			firing = false;
		}
	}

	public void OnTriggerStay(Collider other)
	{
		if (firing) return;

		if (other.tag == "Controller")
		{
			SteamVR_TrackedObject controller = other.GetComponent<SteamVR_TrackedObject>();
			if (controller != null && controller.index != SteamVR_TrackedObject.EIndex.None)
			{
				if (SteamVR_Controller.Input((int)controller.index).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
				{
					firing = true;
				}
			}

		}
	}

	void SpawnMissile()
	{
		GameObject missile = (GameObject)Instantiate(missilePrefab, spawnPoint.position, spawnPoint.rotation);
		Rigidbody rb = missile.GetComponent<Rigidbody>();
		rb.AddForce(spawnPoint.forward * force, ForceMode.Impulse);
	}
}
