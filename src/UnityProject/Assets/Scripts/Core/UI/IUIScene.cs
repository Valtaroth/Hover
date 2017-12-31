using UnityEngine;

namespace Valtaroth.Core.UI
{
	/// <summary>
	/// Interface used to identify user interface components.
	/// </summary>
	public interface IUIScene
	{
		/// <summary>
		/// The scene's root <see cref="GameObject"/>.
		/// Purposefully overlaying Unity's <see cref="Component.gameObject"/> property to avoid redundant implementation.
		/// </summary>
		GameObject gameObject { get; }

		/// <summary>
		/// Called by the <see cref="IUIManager"/> after the scene was loaded.
		/// Executes one time preparations.
		/// </summary>
		void OnLoad();

		/// <summary>
		/// Called by the <see cref="IUIManager"/> after the scene was shown.
		/// Execute show preparations like list population.
		/// </summary>
		void OnShow();
	}
}