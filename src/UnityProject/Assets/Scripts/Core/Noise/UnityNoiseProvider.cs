using UnityEngine;

namespace Valtaroth.Core.Noise
{
	/// <summary>
	/// Class generating Perlin noise using Unity's <see cref="Mathf.PerlinNoise(float, float)"/>.
	/// </summary>
	public class UnityNoiseProvider : INoiseProvider
	{
		private int m_seed;

		private float m_scale;

		public UnityNoiseProvider() : this(0, 1.0f)
		{
		}

		public UnityNoiseProvider(int seed) : this(seed, 1.0f)
		{
		}

		public UnityNoiseProvider(int seed, float scale)
		{
			m_seed = seed;
			m_scale = scale;
		}

		public float GetValue(float x, float y)
		{
			return Mathf.PerlinNoise((x + m_seed) * m_scale, (y + m_seed) * m_scale);
		}
	}
}