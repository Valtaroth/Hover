using UnityEngine;

namespace Valtaroth.Core.Helpers
{
	/// <summary>
	/// Gizmo helper displaying a mesh's normals.
	/// </summary>
	[RequireComponent(typeof(MeshFilter))]
	public class NormalGizmo : MonoBehaviour
	{
		[SerializeField]
		private Color m_defaultColor = Color.grey;

		[SerializeField]
		private Color m_borderColor = Color.red;

		private MeshFilter m_meshFilter;
		
		private void OnDrawGizmosSelected()
		{
			if (m_meshFilter == null)
			{
				m_meshFilter = GetComponent<MeshFilter>();
			}
			if (m_meshFilter == null)
			{
				return;
			}

			Mesh mesh = Application.isPlaying ? m_meshFilter.mesh : m_meshFilter.sharedMesh;

			Color previous = Gizmos.color;
			Gizmos.color = m_defaultColor;

			int resolution = Mathf.RoundToInt(Mathf.Sqrt(mesh.vertices.Length));
			float scale = 1.0f / resolution;
			
			for (int v = 0; v < mesh.vertices.Length; v++)
			{
				bool isBorder = v < resolution || v > mesh.vertices.Length - resolution || v % resolution == 0 || v % resolution == resolution - 1;

				Gizmos.color = isBorder ? m_borderColor : m_defaultColor;
				Gizmos.DrawRay(transform.position + mesh.vertices[v], mesh.normals[v] * scale);
			}

			Gizmos.color = previous;
		}
	}
}