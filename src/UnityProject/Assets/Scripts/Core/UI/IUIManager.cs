namespace Valtaroth.Core.UI
{
	/// <summary>
	/// Interface describing necessarz functionality of a manager for user interfaces.
	/// </summary>
	public interface IUIManager
	{
		/// <summary>
		/// Shows the specified user interface, loading it if necessary.
		/// </summary>
		/// <param name="sceneName">The name of the interface's scene.</param>
		void Show(string sceneName);

		/// <summary>
		/// Shows the specified user interface, loading it if necessary, after hiding all other.
		/// </summary>
		/// <param name="sceneName">The name of the interface's scene.</param>
		void SwitchTo(string sceneName);

		/// <summary>
		/// Hides the specified user interface.
		/// </summary>
		/// <param name="sceneName">The name of the interface's scene.</param>
		void Hide(string sceneName);

		/// <summary>
		/// Hides all currently shown user interfaces.
		/// </summary>
		void HideAll();

		/// <summary>
		/// Unloads the specified user interface, removing it from memory.
		/// </summary>
		/// <param name="sceneName">The name of the interface's scene.</param>
		void Unload(string sceneName);

		/// <summary>
		/// Unloads all currently loaded user interfaces.
		/// </summary>
		void UnloadAll();

		/// <summary>
		/// Gets the root of the scene with the specified name in case it is loaded.
		/// </summary>
		/// <param name="sceneName">The name of the interface's scene.</param>
		/// <returns><c>null</c> if the scene is not loaded, otherwise the scene's root <see cref="IUIScene"/>.</returns>
		IUIScene GetScene(string sceneName);
	}
}