using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valtaroth.Core.Helpers;
using Valtaroth.Core.Infrastructure;
using Valtaroth.Hover.Map.Generation;

namespace Valtaroth.Hover.Map
{
	public class TerrainChunkController
	{
		private ICoroutineInvoker m_coroutineInvoker;

		private int m_viewRadius;
		private int m_cacheRadius;
		
		private TerrainChunkSettings m_chunkSettings;

		private Dictionary<TerrainChunkPosition, TerrainChunk> m_activeChunks;
		private Dictionary<TerrainChunkPosition, TerrainChunk> m_loadedChunks;
		
		public TerrainChunkController(ICoroutineInvoker coroutineInvoker, int viewRadius, int cacheRadius, TerrainChunkSettings chunkSettings)
		{
			m_coroutineInvoker = coroutineInvoker;
			
			m_cacheRadius = cacheRadius;
			m_viewRadius = Mathf.Min(viewRadius, m_cacheRadius);

			m_chunkSettings = chunkSettings;

			m_activeChunks = new Dictionary<TerrainChunkPosition, TerrainChunk>();
			m_loadedChunks = new Dictionary<TerrainChunkPosition, TerrainChunk>();
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
		}

		private void RemoveUnnecessaryChunks(List<TerrainChunkPosition> occupiedChunks)
		{
			List<TerrainChunkPosition> chunksToDestroy = new List<TerrainChunkPosition>();
			List<TerrainChunkPosition> chunksToDeactivate = new List<TerrainChunkPosition>();

			foreach (KeyValuePair<TerrainChunkPosition, TerrainChunk> entry in m_loadedChunks)
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
				DeactivateChunk(chunksToDeactivate[i]);
			}

			List<TerrainChunk> removedChunks = new List<TerrainChunk>();
			for (int i = 0; i < chunksToDestroy.Count; i++)
			{
				removedChunks.Add(RemoveChunk(chunksToDestroy[i]));
			}

			m_coroutineInvoker.StartCoroutine(DestroyChunksCoroutine(removedChunks));
		}

		private void ActivateRequiredChunks(List<TerrainChunkPosition> occupiedChunks)
		{
			for (int i = 0; i < occupiedChunks.Count; i++)
			{
				List<TerrainChunkPosition> chunksInRange = GetChunksInRange(occupiedChunks[i], m_viewRadius);
				for (int j = 0; j < chunksInRange.Count; j++)
				{
					ActivateChunk(chunksInRange[j]);
				}
			}
		}

		private IEnumerator DestroyChunksCoroutine(List<TerrainChunk> chunksToDestroy)
		{
			for (int i = 0; i < chunksToDestroy.Count; i++)
			{
				chunksToDestroy[i].Destroy();
				yield return null;
			}
		}

		private TerrainChunk CreateChunk(TerrainChunkPosition position)
		{
			Vector3 worldPosition = new Vector3(position.X * m_chunkSettings.Length, 0.0f, position.Z * m_chunkSettings.Length);
			
			Mesh mesh = TerrainMeshCreator.Create(worldPosition, m_chunkSettings.DetailResolution, new Gradient());

			GameObject obj = Object.Instantiate<GameObject>(m_chunkSettings.Prefab, worldPosition, Quaternion.identity, m_chunkSettings.Parent);
			obj.name = string.Format("Chunk [{0}/{1}]", position.X, position.Z);
			
			MeshFilter filter = obj.GetComponent<MeshFilter>();
			filter.mesh = mesh;

			MeshCollider collider = obj.GetComponent<MeshCollider>();
			collider.sharedMesh = mesh;

			return new TerrainChunk(position, obj);
		}

		private void ActivateChunk(TerrainChunkPosition position)
		{
			if (m_activeChunks.ContainsKey(position))
			{
				return;
			}

			if (m_loadedChunks.ContainsKey(position))
			{
				m_loadedChunks[position].Terrain.SetActive(true);
				m_activeChunks.Add(position, m_loadedChunks[position]);
				return;
			}

			TerrainChunk chunk = CreateChunk(position);
			m_loadedChunks.Add(chunk.Position, chunk);
			m_activeChunks.Add(chunk.Position, chunk);
		}

		private void DeactivateChunk(TerrainChunkPosition position)
		{
			if (m_activeChunks.ContainsKey(position))
			{
				m_activeChunks[position].Terrain.SetActive(false);
				if (!m_loadedChunks.ContainsKey(position))
				{
					m_loadedChunks.Add(position, m_activeChunks[position]);
				}

				m_activeChunks.Remove(position);
			}
			else if (m_loadedChunks.ContainsKey(position))
			{
				m_loadedChunks[position].Terrain.SetActive(false);
			}
		}

		private TerrainChunk RemoveChunk(TerrainChunkPosition position)
		{
			TerrainChunk chunk = null;
			if (m_activeChunks.ContainsKey(position))
			{
				chunk = m_activeChunks[position];
				m_activeChunks.Remove(position);
			}
			if (m_loadedChunks.ContainsKey(position))
			{
				chunk = m_loadedChunks[position];
				m_loadedChunks.Remove(position);
			}

			if (chunk == null)
			{
				return null;
			}

			chunk.Terrain.SetActive(false);
			return chunk;
		}
	}
}