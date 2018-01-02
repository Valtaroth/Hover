using System.Linq;
using UnityEngine;
using Valtaroth.Core.Infrastructure;
using Valtaroth.Core.Noise;

namespace Valtaroth.Hover.Map
{
	public class ChunkTest : MonoBehaviour, ICoroutineInvoker
    {
		[Header("Generation")]
		[SerializeField]
		private int m_detailResolution;

		[SerializeField]
		private int m_length;

		[SerializeField]
		private int m_height;

		[SerializeField]
		private GameObject m_chunkPrefab;

		[SerializeField]
		private Gradient m_coloring;

		[Header("Tiling")]
		[SerializeField]
		private int m_viewRadius;

		[SerializeField]
		private int m_cacheRadius;
		
		[SerializeField]
		private Transform[] m_targets;

		private TerrainChunkController m_chunkController;
		
		private void FixedUpdate()
		{
			if (m_chunkController == null)
			{
				m_chunkController = new TerrainChunkController(
					this, 
					m_viewRadius, 
					m_cacheRadius, 
					new TerrainChunkSettings
					{
						DetailResolution = m_detailResolution,
						Length = m_length,
						Height = m_height,
						NoiseProvider = new PerlinNoiseProvider(),
						Coloring = m_coloring,
						Prefab = m_chunkPrefab,
						Parent = new GameObject("Terrain").transform
					}
				);
			}

			m_chunkController.Update(m_targets.Select(t => t.transform.position).ToArray());
		}
	}
}