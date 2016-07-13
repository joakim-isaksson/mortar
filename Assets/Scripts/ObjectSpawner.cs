using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
	public struct ObjectLocation
	{
		public Vector2 position;
		public float radius;

		public ObjectLocation(Vector2 position, float radius)
		{
			this.position = position;
			this.radius = radius;
		}
	}

	[System.Serializable]
	public class Spawnable
	{
		public GameObject prefab;
		public float radius;
		public float weight;
	}

	public int ObjectCount = 100;
	public Spawnable[] Spawnables;

	private WeightedList<Spawnable> weightedSpawnables;

	void Awake()
	{
		weightedSpawnables = new WeightedList<Spawnable>();

		foreach (Spawnable s in Spawnables)
		{
			weightedSpawnables.Add(s, s.weight);
		}
	}

	public void SpawnObjects(GameObject worldContainer, Bounds bounds, List<ObjectLocation> existingObjects)
	{
		List<GameObject> objects = new List<GameObject>();
		List<ObjectLocation> positions = new List<ObjectLocation>();

		if (existingObjects != null) positions.AddRange(existingObjects);

		for (int i = 0; i < ObjectCount; i++)
		{
			Spawnable objectToSpawn = weightedSpawnables.Get(Random.value);

			Vector2 pos;
			int iteration = 0;
			bool valid;
			do
			{
				valid = true;

				float x = Random.Range(bounds.min.x, bounds.max.x);
				float z = Random.Range(bounds.min.z, bounds.max.z);

				pos = new Vector2(x, z);

				foreach (ObjectLocation l in positions)
				{
					float minSqDist = Mathf.Max(l.radius, objectToSpawn.radius);
					minSqDist *= minSqDist;

					if ((l.position - pos).sqrMagnitude < minSqDist)
					{
						valid = false;
						break;
					}
				}
				if (++iteration > 10000) throw new System.SystemException("failed to randomize placements for objects, " + objects.Count + " created so far");
			} while (!valid);

			if (iteration > 10) Debug.LogWarning("ObjectSpawner iteration count " + iteration + ", objects: " + objects.Count);

			positions.Add(new ObjectLocation(pos, objectToSpawn.radius));

			GameObject spawnedObject = (GameObject)Instantiate(objectToSpawn.prefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
			spawnedObject.transform.parent = worldContainer.transform;

			objects.Add(spawnedObject);
		}
	}
}
