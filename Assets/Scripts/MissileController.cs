using UnityEngine;
using System.Collections;
using System;

public class MissileController : MonoBehaviour
{
	public AudioSource ExplosionNear;
	public AudioSource ExplosionFar;
	public float FarDistance;

	public GameObject ExplosionPrefab;
	public float AutoExplodeAtAltitude;
	public float BlastRadius;

	public Action OnMissileExploded;

	GameManager gameManager;
	Rigidbody rigidBody;
	Transform playerPosition;
	MeshRenderer meshRenderer;
	bool exploding;

	void Awake()
	{
		gameManager = FindObjectOfType<GameManager>();
		rigidBody = GetComponent<Rigidbody>();
		meshRenderer = GetComponent<MeshRenderer>();
		playerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
	}

	void FixedUpdate()
	{
		// Update missile flying angle
		rigidBody.MoveRotation(Quaternion.LookRotation(rigidBody.velocity));

		// Add wind force
		rigidBody.AddForce(gameManager.Wind);

		// Check for automatic explosion
		if (transform.position.y < AutoExplodeAtAltitude) Explode(null);
	}

	void OnTriggerEnter(Collider collider)
	{
		IDestroyableObject destroyable = collider.gameObject.GetComponent<IDestroyableObject>();
		Explode(destroyable);
	}

	void Explode(IDestroyableObject target)
	{
		// Explode only once
		if (exploding) return;
		exploding = true;

		// Hide the missile and show the explosion
		meshRenderer.enabled = false;
		Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);

		// Play explosion sound depending on the distance to the player
		float distanceToPlayer = Vector3.Distance(transform.position, playerPosition.position);
		AudioSource source = ExplosionFar;
		if (distanceToPlayer < FarDistance) source = ExplosionNear;
		source.Play();

		// Notify missile listener of the explosion
		OnMissileExploded();

		if (target != null) target.OnDestroyObject();

		// Find and destroy all destroyable objects in the blast radius
		foreach (GameObject obj in DestroyableObject.DestroyableObjects)
		{
			if ((transform.position - obj.transform.position).sqrMagnitude < BlastRadius * BlastRadius)
			{
				IDestroyableObject destroyable = obj.GetComponent<IDestroyableObject>();
				if (destroyable != target) destroyable.OnDestroyObject();
			}
		}

		// Destroy this missile object after the explosion sound has ended
		StartCoroutine(WaitAndDestroy(source.clip.length));
	}

	IEnumerator WaitAndDestroy(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Destroy(gameObject);
	}
}
