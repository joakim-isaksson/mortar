using UnityEngine;
using System.Collections;

public class FireAction : MonoBehaviour
{
	public GameManager GameManager;

	public Transform MissileSpawnPoint;
	public GameObject MissilePrefab;
	public float MissileForce;

	public PingPongAnimator TriggerAnimator;
	public PingPongAnimator BarrelAnimator;

	public AudioSource blastSource;
	public AudioClip blastSound;

	public Light muzzleFlash;
	public float flashDuration;

	bool triggerCooldown;
	bool recoilCooldown;

	public void Fire()
	{
		// Check cooldowns
		if (TriggerAnimator.Animating || BarrelAnimator.Animating) return;

		// SFX
		blastSource.PlayOneShot(blastSound);
		StartCoroutine(MuzzleFlash());

		// Animations
		TriggerAnimator.StartAnimation();
		BarrelAnimator.StartAnimation();
		SpawnMissile();

		// Vibrate controllers
		StartCoroutine(HapticUtils.LongVibrationBoth(1, FireHapticStrength));
	}

	public void OnMissileExploded()
	{
		GameManager.OnTurnChange();
	}

	void Awake()
	{
		muzzleFlash.enabled = false;
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

	IEnumerator MuzzleFlash()
	{
		muzzleFlash.enabled = true;
		yield return new WaitForSeconds(flashDuration);
		muzzleFlash.enabled = false;
	}
}
