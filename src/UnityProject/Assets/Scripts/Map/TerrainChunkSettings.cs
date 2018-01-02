using UnityEngine;

namespace Valtaroth.Hover.Map
{
	public class TerrainChunkSettings
    {
		public int DetailResolution { get; set; }
		public int Length { get; set; }
		public int Height { get; set; }

		public GameObject Prefab { get; set; }
		public Transform Parent { get; set; }

		public TerrainChunkSettings(int detailResolution, int length, int height, GameObject prefab, Transform parent)
		{
			DetailResolution = detailResolution;
			Length = length;
			Height = height;

			Prefab = prefab;
			Parent = parent;
		}
	}
}