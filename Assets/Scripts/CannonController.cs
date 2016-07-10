using UnityEngine;
using System.Collections;
using System;

public class CannonController : MonoBehaviour, IDestroyableObject
{
	public Component Barrel;
	public float BarrelStartAngle;

	public Transform MissileSpawnPoint;
	public GameObject MissilePrefab;
	public float MissileForce;

	public Transform MuzzleFlashPoint;
	public GameObject MuzzleFlashPrefab;

	public PingPongAnimator TriggerAnimator;
	public PingPongAnimator BarrelAnimator;

	public AudioSource blastSource;
	public AudioClip blastSound;

	public GameObject CannonExplosionPrefab;

	public Action OnCannonFired;
	public Action OnCannonExploded;
	public Action OnMissileExploded;

	[HideInInspector]
	public bool FiringEnabled;

	void Awake()
	{
		Barrel.transform.localRotation = Quaternion.Euler(BarrelStartAngle, 0, 0);
	}

	public void Fire()
	{
		if (!FiringEnabled) return;
		if (TriggerAnimator.Animating || BarrelAnimator.Animating) return;

		OnCannonFired();

		// SFX
		blastSource.PlayOneShot(blastSound);
		Instantiate(MuzzleFlashPrefab, MuzzleFlashPoint.position, MuzzleFlashPoint.rotation);

		// Spawn the projectile
		SpawnMissile();

		// Animations
		TriggerAnimator.StartAnimation();
		BarrelAnimator.StartAnimation();

		// Vibrate controllers
		StartCoroutine(HapticUtils.LongVibrationBoth(1, FireHapticStrength));
	}

	static float FireHapticStrength(float t)
	{
		return Mathf.Exp(-10 * t);
	}

	void SpawnMissile()
	{
		GameObject missile = (GameObject)Instantiate(MissilePrefab, MissileSpawnPoint.position, MissileSpawnPoint.rotation);

		MissileController missileController = missile.GetComponent<MissileController>();
		missileController.OnMissileExploded = OnMissileExploded;

		Rigidbody rb = missile.GetComponent<Rigidbody>();
		rb.AddForce(MissileSpawnPoint.forward * MissileForce, ForceMode.Impulse);
	}

	public void OnDestroyObject()
	{
		Instantiate(CannonExplosionPrefab, transform.position, transform.rotation);
		OnCannonExploded();
		Destroy(gameObject);
	}
}
