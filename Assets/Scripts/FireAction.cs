using UnityEngine;
using System.Collections;

public class FireAction : MonoBehaviour
{
	public ProjectileSpawner projectileSpawner;

	bool firing;

	void Awake()
	{

	}

	void Update()
	{
		if (firing)
		{
			projectileSpawner.Spawn();
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
}
