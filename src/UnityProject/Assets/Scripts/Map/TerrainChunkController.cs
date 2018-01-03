using System.Collections.Generic;
using UnityEngine;
using Valtaroth.Core.Infrastructure;

namespace Valtaroth.Hover.Map
{
	/// <summary>
	/// Class controlling addition and removal of terrain chunks.
	/// </summary>
	public class TerrainChunkController
	{
		private int m_viewRadius;
		private int m_cacheRadius;
		
		private TerrainChunkSettings m_chunkSettings;
		private TerrainChunkCache m_chunkCache;
		
		public TerrainChunkController(ICoroutineInvoker coroutineInvoker, int viewRadius, int cacheRadius, TerrainChunkSettings chunkSettings)
		{
			m_cacheRadius = cacheRadius;
			m_viewRadius = Mathf.Min(viewRadius, m_cacheRadius);

			m_chunkSettings = chunkSettings;
			m_chunkCache = new TerrainChunkCache(this, coroutineInvoker, chunkSettings);
		}

		public TerrainChunkPosition GetChunkPosition(Vector3 worldPosition)
		{
			var x = (int) Mathf.Floor(worldPosition.x / m_chunkSettings.Length);
			var z = (int) Mathf.Floor(worldPosition.z / m_chunkSettings.Length);

			return new TerrainChunkPosition(x, z);
		}

		public List<TerrainChunkPosition> GetChunksInRange(TerrainChunkPosition position, int radius)
		{
			List<TerrainChunkPosition> result = new List<TerrainChunkPosition>();
			for (int z = -radius; z <= radius; z++)
			{
				for (int x = -radius; x <= radius; x++)
				{
					if (x * x + z * z < radius * radius)
					{
						result.Add(new TerrainChunkPosition(position.X + x, position.Z + z));
					}
				}
			}

			return result;
		}

		public bool IsChunkInRange(TerrainChunkPosition chunk, int radius, List<TerrainChunkPosition> occupiedChunks)
		{
			for (int i = 0; i < occupiedChunks.Count; i++)
			{
				if (occupiedChunks[i].Distance(chunk) <= radius)
				{
					return true;
				}
			}

			return false;
		}

		public void Update(params Vector3[] positions)
		{
			List<TerrainChunkPosition> occupiedChunks = new List<TerrainChunkPosition>();
			for (int i = 0; i < positions.Length; i++)
			{
				TerrainChunkPosition chunk = GetChunkPosition(positions[i]);
				if (!occupiedChunks.Contains(chunk))
				{
					occupiedChunks.Add(chunk);
				}
			}

			RemoveUnnecessaryChunks(occupiedChunks);
			ActivateRequiredChunks(occupiedChunks);

			m_chunkCache.Update();
		}

		public void RemoveUnnecessaryChunks(List<TerrainChunkPosition> occupiedChunks)
		{
			List<TerrainChunkPosition> chunksToDestroy = new List<TerrainChunkPosition>();
			List<TerrainChunkPosition> chunksToDeactivate = new List<TerrainChunkPosition>();

			foreach (KeyValuePair<TerrainChunkPosition, TerrainChunk> entry in m_chunkCache.LoadedChunks)
			{
				TerrainChunkPosition chunk = entry.Key;

				if (IsChunkInRange(chunk, m_viewRadius, occupiedChunks))
				{
					continue;
				}

				if (IsChunkInRange(chunk, m_cacheRadius, occupiedChunks))
				{
					chunksToDeactivate.Add(chunk);
					continue;
				}

				chunksToDestroy.Add(chunk);
			}

			for (int i = 0; i < chunksToDeactivate.Count; i++)
			{
				m_chunkCache.DeactivateChunk(chunksToDeactivate[i]);
			}

			List<TerrainChunk> removedChunks = new List<TerrainChunk>();
			for (int i = 0; i < chunksToDestroy.Count; i++)
			{
				removedChunks.Add(m_chunkCache.RemoveChunk(chunksToDestroy[i]));
			}

			m_chunkCache.DestroyChunks(removedChunks);
		}

		public void ActivateRequiredChunks(List<TerrainChunkPosition> occupiedChunks)
		{
			for (int i = 0; i < occupiedChunks.Count; i++)
			{
				List<TerrainChunkPosition> chunksInRange = GetChunksInRange(occupiedChunks[i], m_viewRadius);
				for (int j = 0; j < chunksInRange.Count; j++)
				{
					m_chunkCache.ActivateChunk(chunksInRange[j]);
				}
			}
		}
	}
}