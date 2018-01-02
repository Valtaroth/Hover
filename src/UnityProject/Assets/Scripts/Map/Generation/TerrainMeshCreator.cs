using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Valtaroth.Hover.Map.Generation
{
	/// <summary>
	/// Utility class used to create planar meshes from noise functions.
	/// </summary>
	public static class TerrainMeshCreator
	{
		public static Mesh Create(Vector3 position, TerrainChunkSettings settings)
		{
			Mesh mesh = new Mesh();
			mesh.name = "Terrain";

			return Create(mesh, position, settings);
		}

		public static Mesh Create(Mesh mesh, Vector3 position, TerrainChunkSettings settings)
		{
			int overdrawResolution = settings.DetailResolution + 2;
			float stepSize = (float)settings.Length / settings.DetailResolution;
			float offset = 0.5f;

			Vector3[] vertices = new Vector3[(overdrawResolution + 1) * (overdrawResolution + 1)];
			Color[] colors = new Color[vertices.Length];
			Vector3[] normals = new Vector3[vertices.Length];
			Vector2[] uv = new Vector2[vertices.Length];

			Vector2 vertex = new Vector2();
			for (int v = 0, z = 0; z <= overdrawResolution; z++)
			{
				for (int x = 0; x <= overdrawResolution; x++, v++)
				{
					vertex.x = x * stepSize - stepSize;
					vertex.y = z * stepSize - stepSize;

					float noise = Mathf.PerlinNoise(vertex.x - offset + position.x, vertex.y - offset + position.z);

					vertices[v] = new Vector3(vertex.x - offset, noise * settings.Height, vertex.y - offset);
					colors[v] = settings.Coloring.Evaluate(noise);
					normals[v] = Vector3.up;
					uv[v] = new Vector2(vertex.x, vertex.y);
				}
			}

			mesh.vertices = vertices;
			mesh.colors = colors;
			mesh.normals = normals;
			mesh.uv = uv;

			mesh.triangles = CalculateTriangles(overdrawResolution);

			mesh.RecalculateNormals();
			mesh.RecalculateTangents();

			return RemoveBorder(mesh, mesh.vertices.ToList(), mesh.colors.ToList(), mesh.normals.ToList(), mesh.uv.ToList(), overdrawResolution + 1, settings.DetailResolution);
		}

		private static Mesh RemoveBorder(Mesh mesh, List<Vector3> vertices, List<Color> colors, List<Vector3> normals, List<Vector2> uv, int currentResolution, int newResolution)
		{
			mesh.triangles = new int[0];
			
			int length = vertices.Count;
			for (int v = length - 1; v >= 0; v--)
			{
				if (v < currentResolution || v > length - currentResolution || v % currentResolution == 0 || v % currentResolution == currentResolution - 1)
				{
					vertices.RemoveAt(v);
					colors.RemoveAt(v);
					normals.RemoveAt(v);
					uv.RemoveAt(v);
				}
			}
			
			mesh.vertices = vertices.ToArray();
			mesh.colors = colors.ToArray();
			mesh.normals = normals.ToArray();
			mesh.uv = uv.ToArray();
			
			mesh.RecalculateBounds();
			mesh.triangles = CalculateTriangles(newResolution);

			return mesh;
		}

		private static int[] CalculateTriangles(int resolution)
		{
			int[] triangles = new int[resolution * resolution * 6];
			for (int t = 0, v = 0, y = 0; y < resolution; y++, v++)
			{
				for (int x = 0; x < resolution; x++, v++, t += 6)
				{
					triangles[t] = v;
					triangles[t + 1] = v + resolution + 1;
					triangles[t + 2] = v + 1;
					triangles[t + 3] = v + 1;
					triangles[t + 4] = v + resolution + 1;
					triangles[t + 5] = v + resolution + 2;
				}
			}

			return triangles;
		}
	}
}