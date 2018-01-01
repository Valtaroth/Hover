using UnityEngine;

namespace Valtaroth.Core.Noise
{
	/// <summary>
	/// Class generating Perlin noise.
	/// </summary>
	public class PerlinNoiseProvider : INoiseProvider
	{
		public float GetValue(float x, float y)
		{
			return Mathf.PerlinNoise(x, y);
		}
	}
}