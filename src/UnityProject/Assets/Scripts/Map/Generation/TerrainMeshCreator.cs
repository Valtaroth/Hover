﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hover.Map.Generation
{
	public static class TerrainMeshCreator
	{
		public static Mesh Create(Vector3 position, int resolution, Gradient coloring)
		{
			Mesh mesh = new Mesh();
			mesh.name = "Terrain";

			int overdrawResolution = resolution + 2;
			float stepSize = 1f / resolution;

			Vector3[] vertices = new Vector3[(overdrawResolution + 1) * (overdrawResolution + 1)];
			Color[] colors = new Color[vertices.Length];
			Vector3[] normals = new Vector3[vertices.Length];
			Vector2[] uv = new Vector2[vertices.Length];
			
			for (int v = 0, z = 0; z <= overdrawResolution; z++)
			{
				for (int x = 0; x <= overdrawResolution; x++, v++)
				{
					float noise = Mathf.PerlinNoise(x * stepSize - stepSize - 0.5f + position.x, z * stepSize - stepSize - 0.5f + position.z);

					vertices[v] = new Vector3(x * stepSize - stepSize - 0.5f, noise, z * stepSize - stepSize - 0.5f);
					colors[v] = coloring.Evaluate(noise);
					normals[v] = Vector3.up;
					uv[v] = new Vector2(x * stepSize - stepSize, z * stepSize - stepSize);
				}
			}

			mesh.vertices = vertices;
			mesh.colors = colors;
			mesh.normals = normals;
			mesh.uv = uv;

			mesh.triangles = CalculateTriangles(overdrawResolution);
			
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();
			
			return RemoveBorder(mesh, overdrawResolution + 1, resolution);
		}

		private static Mesh RemoveBorder(Mesh mesh, int currentResolution, int newResolution)
		{
			mesh.triangles = new int[0];

			List<Vector3> vertices = new List<Vector3>();
			List<Color> colors = new List<Color>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uv = new List<Vector2>();
			
			for (int v = mesh.vertices.Length - 1; v >= 0; v--)
			{
				if (v < currentResolution || v > mesh.vertices.Length - currentResolution || v % currentResolution == 0 || v % currentResolution == currentResolution - 1)
				{
					continue;
				}

				vertices.Insert(0, mesh.vertices[v]);
				colors.Insert(0, mesh.colors[v]);
				normals.Insert(0, mesh.normals[v]);
				uv.Insert(0, mesh.uv[v]);
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