using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour
{
	public AudioSource ExplosionNear;
	public AudioSource ExplosionFar;
	public float FarDistance;

	Rigidbody rigidBody;
	Transform playerPosition;

	void Awake()
	{
		rigidBody = GetComponent<Rigidbody>();
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
		if (distanceToPlayer < FarDistance) PlayAndDestroy(ExplosionNear);
		else PlayAndDestroy(ExplosionNear);
	}

	IEnumerator PlayAndDestroy(AudioSource source)
	{
		source.Play();
		yield return new WaitForSeconds(source.clip.length);
		Destroy(gameObject);
	}
}
