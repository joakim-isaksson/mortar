using UnityEngine;
using System.Collections;

public class TerrainUtils
{
	public static Bounds GetTerrainBounds(Terrain terrain)
	{
		Vector3 pos = terrain.GetPosition();
		Vector3 size = terrain.terrainData.size;

		Vector3 center = pos + size * 0.5f;
		center.y = 0;

		return new Bounds(center, size);
	}
}
