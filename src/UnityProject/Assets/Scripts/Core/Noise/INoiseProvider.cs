namespace Valtaroth.Core.Noise
{
	/// <summary>
	/// Interface for any class providing noise generation functionality.
	/// </summary>
	public interface INoiseProvider
	{
		float GetValue(float x, float y);
	}
}