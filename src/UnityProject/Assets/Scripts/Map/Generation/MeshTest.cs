using System.Diagnostics;
using UnityEngine;
using Valtaroth.Core.Helpers;
using Debug = UnityEngine.Debug;

namespace Valtaroth.Hover.Map.Generation
{
	public class MeshTest : MonoBehaviour
	{
		[SerializeField]
		private Material m_material;

		[SerializeField]
		private int m_resolution;

		[SerializeField]
		private Gradient m_coloring;

		[SerializeField]
		private int m_size;
		
		[ContextMenu("Test")]
		private void Test()
		{
			Stopwatch watchTotal = new Stopwatch();

			GameObject parent = new GameObject("Terrain");
			parent.transform.position = transform.position;

			for (int z = 0; z < m_size; z++)
			{
				for (int x = 0; x < m_size; x++)
				{
					Vector3 position = transform.position + new Vector3(x, 0.0f, z);

					Stopwatch watch = new Stopwatch();

					Mesh mesh = TerrainMeshCreator.Create(position, m_resolution, m_coloring);

					watch.Stop();
					Debug.LogFormat("Created mesh with resolution {0}: {1}ms/{2}ticks", m_resolution, watch.ElapsedMilliseconds, watch.ElapsedTicks);
					watch.Reset();

					GameObject obj = new GameObject(string.Format("Chunk [{0}/{1}]", x, z));
					obj.transform.SetParent(parent.transform);
					obj.transform.position = position;

					MeshFilter filter = obj.AddComponent<MeshFilter>();
					filter.mesh = mesh;

					MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
					renderer.material = m_material;

					obj.AddComponent<MeshCollider>();
					obj.AddComponent<NormalGizmo>();

					watch.Stop();
					Debug.LogFormat("Created game object: {0}ms/{1}ticks", watch.ElapsedMilliseconds, watch.ElapsedTicks);
				}
			}

			watchTotal.Stop();
			Debug.LogFormat("Created terrain of size {0}: {1}ms/{2}ticks", m_size, watchTotal.ElapsedMilliseconds, watchTotal.ElapsedTicks);
		}
	}
}