using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valtaroth.Core.Infrastructure;
using ILogger = Valtaroth.Core.Logging.ILogger;

namespace Valtaroth.Core.UI
{
	/// <summary>
	/// Manager used to show and hide user interfaces.
	/// </summary>
	public class UIManager : IUIManager
	{
		/// <summary>
		/// Instance used to execute <see cref="Coroutine"/>s.
		/// </summary>
		private ICoroutineInvoker m_coroutineInvoker;

		/// <summary>
		/// Instance used to execute <see cref="Coroutine"/>s.
		/// </summary>
		private ILogger m_logger;

		/// <summary>
		/// The root of all loaded user interfaces.
		/// </summary>
		private Transform m_sceneRoot;

		/// <summary>
		/// Dictionary of loaded user interfaces for quick access.
		/// </summary>
		private Dictionary<string, IUIScene> m_loadedScenes;

		/// <summary>
		/// Initializes a new instance, creating a scene root object.
		/// </summary>
		/// <param name="container">The container used to fetch shared instances.</param>
		public UIManager(DIContainer container)
		{
			m_coroutineInvoker = container.Get<ICoroutineInvoker>();
			m_logger = container.Get<ILogger>();
			m_loadedScenes = new Dictionary<string, IUIScene>();

			m_sceneRoot = new GameObject("UIRoot").transform;
			Object.DontDestroyOnLoad(m_sceneRoot.gameObject);
		}

		/// <summary>
		/// Shows the specified user interface, loading it if necessary.
		/// </summary>
		/// <param name="sceneName">The name of the interface's scene.</param>
		public void Show(string sceneName)
		{
			if (m_loadedScenes.ContainsKey(sceneName))
			{
				m_loadedScenes[sceneName].gameObject.SetActive(true);
				m_loadedScenes[sceneName].OnShow();
				return;
			}

			m_coroutineInvoker.StartCoroutine(LoadScene(sceneName));
		}

		/// <summary>
		/// Loads the user interface with the specified name and caches it for later reference.
		/// </summary>
		/// <param name="sceneName">The name of the interface's scene.</param>
		private IEnumerator LoadScene(string sceneName)
		{
			AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			if (operation == null)
			{
				m_logger.Error("Unable to load scene '{0}'!", sceneName);
				yield break;
			}

			while(!operation.isDone)
			{
				yield return null;
			}

			Scene newScene = SceneManager.GetSceneByName(sceneName);
			GameObject[] roots = newScene.GetRootGameObjects();
			for (int i = 0; i < roots.Length; i++)
			{
				if (RegisterScene(sceneName, roots[i]))
				{
					break;
				}
			}

			if (!m_loadedScenes.ContainsKey(sceneName))
			{
				m_logger.Error("Unable to locate 'IUIScene' component on root of scene '{0}'!", sceneName);
			}
			else
			{
				m_loadedScenes[sceneName].OnLoad();
				m_loadedScenes[sceneName].OnShow();
			}

			operation = SceneManager.UnloadSceneAsync(newScene);
			if (operation == null)
			{
				yield break;
			}

			while (!operation.isDone)
			{
				yield return null;
			}
		}

		/// <summary>
		/// Registers a scene by parenting it to <see cref="m_sceneRoot"/> and caching its <see cref="IUIScene"/> for later reference.
		/// </summary>
		/// <param name="sceneName">The name of the scene to register.</param>
		/// <param name="root">The root of the scene to register.</param>
		/// <returns><c>false</c> if registration failed, otherwise <c>true</c>.</returns>
		private bool RegisterScene(string sceneName, GameObject root)
		{
			if (!root.name.Equals(sceneName))
			{
				return false;
			}

			IUIScene scene = (IUIScene) root.GetComponent(typeof(IUIScene));
			if (scene == null)
			{
				return false;
			}

			SceneManager.MoveGameObjectToScene(root, SceneManager.GetActiveScene());
			root.transform.SetParent(m_sceneRoot);
			m_loadedScenes.Add(sceneName, scene);

			return true;
		}

		/// <summary>
		/// Shows the specified user interface, loading it if necessary, after hiding all other.
		/// </summary>
		/// <param name="sceneName">The name of the interface's scene.</param>
		public void SwitchTo(string sceneName)
		{
			HideAll();
			Show(sceneName);
		}

		/// <summary>
		/// Hides the specified user interface.
		/// </summary>
		/// <param name="sceneName">The name of the interface's scene.</param>
		public void Hide(string sceneName)
		{
			if (!m_loadedScenes.ContainsKey(sceneName))
			{
				return;
			}

			m_loadedScenes[sceneName].gameObject.SetActive(false);
		}

		/// <summary>
		/// Hides all currently shown user interfaces.
		/// </summary>
		public void HideAll()
		{
			foreach (KeyValuePair<string, IUIScene> entry in m_loadedScenes)
			{
				entry.Value.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Unloads the specified user interface, removing it from memory.
		/// </summary>
		/// <param name="sceneName">The name of the interface's scene.</param>
		public void Unload(string sceneName)
		{
			if (!m_loadedScenes.ContainsKey(sceneName))
			{
				return;
			}
			
			Object.Destroy(m_loadedScenes[sceneName].gameObject);
			m_loadedScenes.Remove(sceneName);
		}

		/// <summary>
		/// Unloads all currently loaded user interfaces.
		/// </summary>
		public void UnloadAll()
		{
			foreach(KeyValuePair<string, IUIScene> entry in m_loadedScenes)
			{
				Object.Destroy(entry.Value.gameObject);
			}

			m_loadedScenes.Clear();
		}

		/// <summary>
		/// Gets the root of the scene with the specified name in case it is loaded.
		/// </summary>
		/// <param name="sceneName">The name of the interface's scene.</param>
		/// <returns><c>null</c> if the scene is not loaded, otherwise the scene's root <see cref="IUIScene"/>.</returns>
		public IUIScene GetScene(string sceneName)
		{
			if (!m_loadedScenes.ContainsKey(sceneName))
			{
				m_logger.Error("No UI scene with name '{0}' is registered!", sceneName);
				return null;
			}

			return m_loadedScenes[sceneName];
		}
	}
}