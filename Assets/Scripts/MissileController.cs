using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour
{
	public AudioClip ExplosionNear;
	public AudioClip ExplosionFar;
	public float FarDistance;

	Rigidbody rigidBody;
	AudioSource audioSource;
	Transform playerPosition;

	void Awake()
	{
		rigidBody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		playerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
	}

	void FixedUpdate()
	{
		rigidBody.MoveRotation(Quaternion.LookRotation(rigidBody.velocity));
	}

	void OnTriggerEnter(Collider collider)
	{
		Explode();
	}

	void Explode()
	{
		float distanceToPlayer = Vector3.Distance(transform.position, playerPosition.position);

		if (distanceToPlayer < FarDistance) audioSource.PlayOneShot(ExplosionFar);
		else audioSource.PlayOneShot(ExplosionFar);

		Destroy(gameObject);
	}
}
