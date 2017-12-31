using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valtaroth.Core.Infrastructure;
using Valtaroth.Core.Logging;
using Valtaroth.Core.UI;

namespace Hover.Infrastructure
{
	/// <summary>
	/// Class representing the main entry point of the application.
	/// </summary>
	public class Root : MonoBehaviour
	{
		/// <summary>
		/// The asset provider containing all assets not linked elsewhere.
		/// </summary>
		[SerializeField]
		private ScriptableObject m_assetProvider;

		/// <summary>
		/// The balancing files to load during the configuration step.
		/// </summary>
		[SerializeField]
		private List<Object> m_balancingFiles;

		/// <summary>
		/// Static accessor to locate any services used across the application.
		/// </summary>
		public static DIContainer Container { get; set; }

		/// <summary>
		/// Configures all services and loads the <c>Meta</c> scene and <c>MainMenu</c> user interface.
		/// </summary>
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
			RegisterInstances();

			SceneManager.LoadScene("Meta");

			IUIManager uiManager = Container.Get<IUIManager>();
			uiManager.Show("MainMenu");
		}

		/// <summary>
		/// Configures the necessary instances and registers them in the <see cref="Container"/>.
		/// </summary>
		private void RegisterInstances()
		{
			Container = new DIContainer();

			Container.Bind<Valtaroth.Core.Logging.ILogger>(new ConsoleLogger());
			Container.Bind<ICoroutineInvoker>(new CoroutineInvoker(this));

			Container.Bind<IUIManager>(new UIManager(Container));
		}
	}
}