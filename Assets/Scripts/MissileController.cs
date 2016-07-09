using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour
{
	public AudioSource ExplosionNear;
	public AudioSource ExplosionFar;
	public float FarDistance;

	public GameObject ExplosionPrefab;
	public float AutoExplodeAtAltitude;
	public float BlastRadius;

	[HideInInspector]
	public FireAction FireAction;

	Rigidbody rigidBody;
	Transform playerPosition;
	MeshRenderer meshRenderer;
	bool exploding;

	void Awake()
	{
		rigidBody = GetComponent<Rigidbody>();
		meshRenderer = GetComponent<MeshRenderer>();
		playerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
	}

	void FixedUpdate()
	{
		// Update missile flying angle
		rigidBody.MoveRotation(Quaternion.LookRotation(rigidBody.velocity));

		// Check for automatic explosion
		if (transform.position.y < AutoExplodeAtAltitude) Explode();
	}

	void OnTriggerEnter(Collider collider)
	{
		Explode();
	}

	void Explode()
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

		// Notify the firing unit of the explosion
		FireAction.OnMissileExploded();

		// Find and destroy all destroyable objects in the blast radius
		foreach (GameObject obj in DestroyableObject.DestroyableObjects)
		{
			if (Vector3.Distance(transform.position, obj.transform.position) < BlastRadius)
			{
				obj.GetComponent<IDestroyableObject>().OnDestroyObject();
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
