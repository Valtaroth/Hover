using UnityEngine;

namespace Valtaroth.Hover.Map
{
	/// <summary>
	/// Class representing the position of a <see cref="TerrainChunk"/>.
	/// Custom hash functionality to allow easy referencing of chunks.
	/// </summary>
	public class TerrainChunkPosition
	{
		public int X { get; set; }
		public int Z { get; set; }

		public TerrainChunkPosition()
		{
			X = 0;
			Z = 0;
		}

		public TerrainChunkPosition(int x, int z)
		{
			X = x;
			Z = z;
		}

		public TerrainChunkPosition(TerrainChunkPosition position)
		{
			X = position.X;
			Z = position.Z;
		}
		
		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Z.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			TerrainChunkPosition other = obj as TerrainChunkPosition;
			if (other == null)
			{
				return false;
			}

			return X == other.X && Z == other.Z;
		}

		public override string ToString()
		{
			return string.Format("[{0}/{1}]", X, Z);
		}

		public int Distance(TerrainChunkPosition other)
		{
			return Distance(this, other);
		}

		public static int Distance(TerrainChunkPosition chunk1, TerrainChunkPosition chunk2)
		{
			return Mathf.Abs(chunk1.X - chunk2.X) + Mathf.Abs(chunk1.Z - chunk2.Z);
		}
	}
}