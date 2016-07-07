using UnityEngine;
using System.Collections;

public class FireAction : MonoBehaviour
{
	public Transform MissileSpawnPoint;
	public GameObject MissilePrefab;
	public float MissileForce;

	public PingPongAnimator TriggerAnimator;
	public PingPongAnimator BarrelAnimator;

	public AudioSource blastSource;
	public AudioClip blastSound;

	bool triggerCooldown;
	bool recoilCooldown;

	private static float FireHapticStrength(float t)
	{
		return Mathf.Exp(-10 * t);
	}

	public void Fire()
	{
		// Check cooldowns
		if (TriggerAnimator.Animating || BarrelAnimator.Animating) return;

		blastSource.PlayOneShot(blastSound);
		TriggerAnimator.StartAnimation();
		BarrelAnimator.StartAnimation();
		SpawnMissile();

		// Vibrate controllers
		StartCoroutine(HapticUtils.LongVibrationBoth(1, FireHapticStrength));
	}

	void SpawnMissile()
	{
		GameObject missile = (GameObject)Instantiate(MissilePrefab, MissileSpawnPoint.position, MissileSpawnPoint.rotation);
		Rigidbody rb = missile.GetComponent<Rigidbody>();
		rb.AddForce(MissileSpawnPoint.forward * MissileForce, ForceMode.Impulse);
	}
}
