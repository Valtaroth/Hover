﻿using UnityEngine;

namespace Valtaroth.Hover.Map
{
	/// <summary>
	/// Class representing a square chunk of terrain.
	/// </summary>
	public class TerrainChunk
	{
		public TerrainChunkPosition Position { get; private set; }
		public GameObject Terrain { get; private set; }

		public TerrainChunk(TerrainChunkPosition position, GameObject terrain)
		{
			Position = position;
			Terrain = terrain;
		}

		public void Destroy()
		{
			if (Terrain != null)
			{
				Object.Destroy(Terrain);
			}
		}
	}
}