using System.Linq;
using UnityEngine;
using Valtaroth.Core.Infrastructure;

namespace Valtaroth.Hover.Map
{
	public class ChunkTest : MonoBehaviour, ICoroutineInvoker
    {
		[SerializeField]
		private int m_viewRadius;

		[SerializeField]
		private int m_cacheRadius;

		[SerializeField]
		private GameObject m_chunkPrefab;

		[Space]
		[SerializeField]
		private Transform[] m_targets;

		private TerrainChunkController m_chunkController;
		
		private void Update()
		{
			if (m_chunkController == null)
			{
				m_chunkController = new TerrainChunkController(this, m_viewRadius, m_cacheRadius, new TerrainChunkSettings(16, 1, 1, m_chunkPrefab, new GameObject("Terrain").transform));
			}

			m_chunkController.Update(m_targets.Select(t => t.transform.position).ToArray());
		}
	}
}