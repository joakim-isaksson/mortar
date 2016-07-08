using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour
{
	public AudioSource ExplosionNear;
	public AudioSource ExplosionFar;
	public float FarDistance;
	public float AutoExplodeAtAltitude;
	public GameObject ExplosionPrefab;

	[HideInInspector]
	public FireAction FireAction;

	Rigidbody rigidBody;
	Transform playerPosition;
	MeshRenderer renderer;
	bool exploding;

	void Awake()
	{
		rigidBody = GetComponent<Rigidbody>();
		renderer = GetComponent<MeshRenderer>();
		playerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
	}

	void FixedUpdate()
	{
		// Update missile flying angle
		rigidBody.MoveRotation(Quaternion.LookRotation(rigidBody.velocity));

		// Check for automatic explosion
		if (transform.position.y < AutoExplodeAtAltitude) Explode(transform.position);
	}

	void OnTriggerEnter(Collider collider)
	{
		Explode(collider.transform.position);
	}

	void Explode(Vector3 explosionPoint)
	{
		// Explode only once
		if (exploding) return;
		exploding = true;

		// Hide the missile and show an explosion
		renderer.enabled = false;
		Instantiate(ExplosionPrefab, explosionPoint, Quaternion.identity);

		// Play explosion sound depending on the distance from player
		float distanceToPlayer = Vector3.Distance(transform.position, playerPosition.position);
		if (distanceToPlayer < FarDistance) StartCoroutine(PlayAndDestroy(ExplosionNear));
		else StartCoroutine(PlayAndDestroy(ExplosionFar));
	}

	IEnumerator PlayAndDestroy(AudioSource source)
	{
		source.Play();
		yield return new WaitForSeconds(source.clip.length);

		fireAction.OnMissileExploded();

		Destroy(gameObject);
	}
}
