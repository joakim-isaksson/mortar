using UnityEngine;
using System.Collections;

public class FireAction : MonoBehaviour
{
	public Transform MissileSpawnPoint;
	public GameObject MissilePrefab;
	public float MissileForce;

	public Transform MuzzleFlashPoint;
	public GameObject MuzzleFlashPrefab;

	public PingPongAnimator TriggerAnimator;
	public PingPongAnimator BarrelAnimator;

	public AudioSource blastSource;
	public AudioClip blastSound;

	bool triggerCooldown;
	bool recoilCooldown;

	GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType<GameManager>();
	}

	public void Fire()
	{
		// Check cooldowns
		if (TriggerAnimator.Animating || BarrelAnimator.Animating) return;

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

	public void OnMissileExploded()
	{
		gameManager.OnTurnChange();
	}

	static float FireHapticStrength(float t)
	{
		return Mathf.Exp(-10 * t);
	}

	void SpawnMissile()
	{
		GameObject missile = (GameObject)Instantiate(MissilePrefab, MissileSpawnPoint.position, MissileSpawnPoint.rotation);

		MissileController missileController = missile.GetComponent<MissileController>();
		missileController.FireAction = this;

		Rigidbody rb = missile.GetComponent<Rigidbody>();
		rb.AddForce(MissileSpawnPoint.forward * MissileForce, ForceMode.Impulse);
	}
}
