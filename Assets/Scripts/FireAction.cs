using UnityEngine;
using System.Collections;

public class FireAction : MonoBehaviour
{
	public Transform MissileSpawnPoint;
	public GameObject MissilePrefab;
	public float MissileForce;

	public PingPongAnimator TriggerAnimator;
	public PingPongAnimator BarrelAnimator;

	bool triggerCooldown;
	bool recoilCooldown;

	public void Fire()
	{
		// Check cooldowns
		if (TriggerAnimator.Animating || BarrelAnimator.Animating) return;

		TriggerAnimator.StartAnimation();
		BarrelAnimator.StartAnimation();
		SpawnMissile();
	}

	void SpawnMissile()
	{
		GameObject missile = (GameObject)Instantiate(MissilePrefab, MissileSpawnPoint.position, MissileSpawnPoint.rotation);
		Rigidbody rb = missile.GetComponent<Rigidbody>();
		rb.AddForce(MissileSpawnPoint.forward * MissileForce, ForceMode.Impulse);
	}
}
