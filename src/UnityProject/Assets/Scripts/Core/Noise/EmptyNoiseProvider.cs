namespace Valtaroth.Core.Noise
{
	/// <summary>
	/// Empty class always providing <c>0.0f</c>.
	/// </summary>
	public class EmptyNoiseProvider : INoiseProvider
	{
		public float GetValue(float x, float y)
		{
			return 0.0f;
		}
	}
}