using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMap : MonoBehaviour
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
		public Texture2D placementMap;
	}

	public int ObjectCount = 100;
	public Spawnable[] Spawnables;

	[HideInInspector]
	public GameObject WorldContainer;
	[HideInInspector]
	public Terrain Terrain;
	public Texture2D CannonPlacementMap;

	WeightedList<Spawnable> weightedSpawnables;

	Bounds bounds;

	List<ObjectLocation> positions;

	void Awake()
	{
		weightedSpawnables = new WeightedList<Spawnable>();

		foreach (Spawnable s in Spawnables)
		{
			weightedSpawnables.Add(s, s.weight);
		}

		WorldContainer = new GameObject("WorldContainer");
		WorldContainer.transform.parent = transform;
		Terrain = GetComponentInChildren<Terrain>();
		bounds = TerrainUtils.GetTerrainBounds(Terrain);

		positions = new List<ObjectLocation>();
	}

	public Vector3 GetSpawnPosition(float radius, float? minDistanceToAll, float? maxDistanceToAll, Texture2D placementMap)
	{
		Vector2 pos;
		int iteration = 0;
		bool valid;
		do
		{
			valid = true;

			if (++iteration > 10000) throw new System.SystemException("failed to randomize placements for objects");

			float x = Random.Range(bounds.min.x, bounds.max.x);
			float z = Random.Range(bounds.min.z, bounds.max.z);

			pos = new Vector2(x, z);

			float rx = (x - bounds.min.x) / (bounds.max.x - bounds.min.x);
			float rz = (z - bounds.min.z) / (bounds.max.z - bounds.min.z);

			int pixelY = placementMap.height - (int)(rz * placementMap.height);
			Color color = placementMap.GetPixel((int)(rx * placementMap.width), pixelY);
			//Debug.Log("x: " + ((int)(rx * placementMap.width)) + ", y: " + ((int)(rz * placementMap.height)) + ", color: " + color);
			if (color.r < 0.1f)
			{
				valid = false;
				continue;
			}

			foreach (ObjectLocation l in positions)
			{
				float minSqDist = l.radius + radius;
				minSqDist *= minSqDist;

				float sqDist = (l.position - pos).sqrMagnitude;
				if (sqDist < minSqDist || (minDistanceToAll != null && sqDist < minDistanceToAll * minDistanceToAll) || (maxDistanceToAll != null && sqDist >= maxDistanceToAll * maxDistanceToAll))
				{
					valid = false;
					break;
				}
			}
		} while (!valid);

		positions.Add(new ObjectLocation(pos, radius));
		return new Vector3(pos.x, Terrain.SampleHeight(new Vector3(pos.x, 0, pos.y)), pos.y);
	}

	public void SpawnSceneryObjects()
	{
		List<GameObject> objects = new List<GameObject>();

		for (int i = 0; i < ObjectCount; i++)
		{
			Spawnable objectToSpawn = weightedSpawnables.Get(Random.value);

			Vector3 pos = GetSpawnPosition(objectToSpawn.radius, null, null, objectToSpawn.placementMap);

			positions.Add(new ObjectLocation(pos, objectToSpawn.radius));
			float dir = Random.Range(0, 360);
			GameObject spawnedObject = (GameObject)Instantiate(objectToSpawn.prefab, pos, Quaternion.Euler(0, dir, 0));
			spawnedObject.transform.parent = WorldContainer.transform;

			objects.Add(spawnedObject);
		}
	}
}
