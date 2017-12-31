using System;
using System.Collections.Generic;

namespace Valtaroth.Core.Infrastructure
{
	/// <summary>
	/// Class used to access instances used across the application.
	/// </summary>
	public class DIContainer
	{
		/// <summary>
		/// Dictionary of instances mapped to the type they are registered to.
		/// </summary>
		private Dictionary<Type, object> m_instances;

		/// <summary>
		/// Initializes an empty instance dictionary.
		/// </summary>
		public DIContainer()
		{
			m_instances = new Dictionary<Type, object>();
		}

		/// <summary>
		/// Binds an instance to the specified type.
		/// </summary>
		/// <typeparam name="TKey">The type to key the instance to.</typeparam>
		/// <param name="instance">The instance to register.</param>
		public void Bind<TKey>(TKey instance)
		{
			Type key = typeof(TKey);
			if (m_instances.ContainsKey(key))
			{
				throw new ArgumentException(string.Format("An instance of type {0} has already been registered!", key));
			}

			m_instances.Add(key, instance);
		}

		/// <summary>
		/// Unbinds an instance from its key.
		/// </summary>
		/// <typeparam name="TKey">The key to unbind the instance of.</typeparam>
		public void Unbind<TKey>()
		{
			Type key = typeof(TKey);
			if (!m_instances.ContainsKey(key))
			{
				return;
			}

			m_instances.Remove(key);
		}

		/// <summary>
		/// Gets an instance from the internal dictionary matching the specified key.
		/// </summary>
		/// <typeparam name="TKey">The key to get the instance for.</typeparam>
		/// <returns><c>null</c> if no instance is registered for the specified key, otherwise the registered instance.</returns>
		public TKey Get<TKey>()
		{
			Type key = typeof(TKey);
			if (!m_instances.ContainsKey(key))
			{
				return default(TKey);
			}

			return (TKey) m_instances[key];
		}
	}
}