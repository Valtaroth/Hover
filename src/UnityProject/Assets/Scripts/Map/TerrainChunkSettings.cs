using UnityEngine;
using Valtaroth.Core.Noise;

namespace Valtaroth.Hover.Map
{
	/// <summary>
	/// Settings required to create a chunk of terrain.
	/// </summary>
	public class TerrainChunkSettings
    {
		public int DetailResolution { get; set; }
		public int Length { get; set; }
		public int Height { get; set; }

		public INoiseProvider NoiseProvider { get; set; }
		public Gradient Coloring { get; set; }

		public GameObject Prefab { get; set; }
		public Transform Parent { get; set; }
	}
}