using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Valtaroth.Core.Infrastructure;
using Valtaroth.Hover.Map.Generation;

namespace Valtaroth.Hover.Map
{
	/// <summary>
	/// Class holding all laoded terrain chunks and executing spawning and removal.
	/// </summary>
    public class TerrainChunkCache
    {
		private const int ChunkCreationLimitPerFrame = 1;

		private ICoroutineInvoker m_coroutineInvoker;
		public TerrainChunkController m_chunkController;
		private TerrainChunkSettings m_chunkSettings;

		public Dictionary<TerrainChunkPosition, TerrainChunk> ActiveChunks { get; set; }
		public Dictionary<TerrainChunkPosition, TerrainChunk> LoadedChunks { get; set; }

		private List<TerrainChunkPosition> m_requestedChunks;
		private List<TerrainChunkPosition> m_chunksInCreation;

		public TerrainChunkCache(TerrainChunkController controller, ICoroutineInvoker coroutineInvoker, TerrainChunkSettings chunkSettings)
		{
			m_coroutineInvoker = coroutineInvoker;
			m_chunkSettings = chunkSettings;

			ActiveChunks = new Dictionary<TerrainChunkPosition, TerrainChunk>();
			LoadedChunks = new Dictionary<TerrainChunkPosition, TerrainChunk>();

			m_requestedChunks = new List<TerrainChunkPosition>();
			m_chunksInCreation = new List<TerrainChunkPosition>();
		}
		
		public void Update()
		{
			if (m_requestedChunks.Count < 1)
			{
				return;
			}

			m_chunksInCreation.AddRange(m_requestedChunks);
			
			m_coroutineInvoker.StartCoroutine(CreateRequestedChunks(m_requestedChunks.ToArray()));
			m_requestedChunks.Clear();
		}

		private IEnumerator CreateRequestedChunks(TerrainChunkPosition[] requestedChunks)
		{
			for (int i = 0; i < requestedChunks.Length; i++)
			{
				CreateChunk(requestedChunks[i]);
				m_chunksInCreation.Remove(requestedChunks[i]);

				if (i % ChunkCreationLimitPerFrame == 0)
				{
					yield return null;
				}
			}
		}

		private void CreateChunk(TerrainChunkPosition position)
		{
			Vector3 worldPosition = new Vector3(position.X * m_chunkSettings.Length, 0.0f, position.Z * m_chunkSettings.Length);

			GameObject obj = Object.Instantiate(m_chunkSettings.Prefab, worldPosition, Quaternion.identity, m_chunkSettings.Parent);
			obj.name = string.Format("Chunk [{0}/{1}]", position.X, position.Z);

			Mesh mesh = TerrainMeshCreator.Create(worldPosition, m_chunkSettings);

			MeshFilter filter = obj.GetComponent<MeshFilter>();
			filter.mesh = mesh;

			MeshCollider collider = obj.GetComponent<MeshCollider>();
			collider.sharedMesh = mesh;

			TerrainChunk chunk = new TerrainChunk(position, obj);
			LoadedChunks.Add(position, chunk);
			ActiveChunks.Add(position, chunk);
		}

		public void RequestChunk(TerrainChunkPosition position)
		{
			if (m_requestedChunks.Contains(position) || m_chunksInCreation.Contains(position))
			{
				return;
			}

			m_requestedChunks.Add(position);
		}
		
		public void ActivateChunk(TerrainChunkPosition position)
		{
			if (ActiveChunks.ContainsKey(position))
			{
				return;
			}

			if (LoadedChunks.ContainsKey(position))
			{
				LoadedChunks[position].Terrain.SetActive(true);
				ActiveChunks.Add(position, LoadedChunks[position]);
				return;
			}

			RequestChunk(position);
		}

		public void DeactivateChunk(TerrainChunkPosition position)
		{
			if (ActiveChunks.ContainsKey(position))
			{
				ActiveChunks[position].Terrain.SetActive(false);
				if (!LoadedChunks.ContainsKey(position))
				{
					LoadedChunks.Add(position, ActiveChunks[position]);
				}

				ActiveChunks.Remove(position);
			}
			else if (LoadedChunks.ContainsKey(position))
			{
				LoadedChunks[position].Terrain.SetActive(false);
			}
		}

		public TerrainChunk RemoveChunk(TerrainChunkPosition position)
		{
			TerrainChunk chunk = null;
			if (ActiveChunks.ContainsKey(position))
			{
				chunk = ActiveChunks[position];
				ActiveChunks.Remove(position);
			}
			if (LoadedChunks.ContainsKey(position))
			{
				chunk = LoadedChunks[position];
				LoadedChunks.Remove(position);
			}

			if (chunk == null)
			{
				return null;
			}

			chunk.Terrain.SetActive(false);
			return chunk;
		}

		public void DestroyChunks(List<TerrainChunk> chunksToDestroy)
		{
			m_coroutineInvoker.StartCoroutine(DestroyChunksCoroutine(chunksToDestroy));
		}

		private IEnumerator DestroyChunksCoroutine(List<TerrainChunk> chunksToDestroy)
		{
			for (int i = 0; i < chunksToDestroy.Count; i++)
			{
				chunksToDestroy[i].Destroy();
				yield return null;
			}
		}
	}
}