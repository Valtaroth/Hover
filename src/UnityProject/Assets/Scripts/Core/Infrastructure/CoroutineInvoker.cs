using System;
using System.Collections;
using UnityEngine;

namespace Valtaroth.Core.Infrastructure
{
	public class CoroutineInvoker : ICoroutineInvoker
	{
		private MonoBehaviour m_behaviour;

		public CoroutineInvoker(MonoBehaviour behaviour)
		{
			m_behaviour = behaviour;
		}

		public Coroutine StartCoroutine(IEnumerator coroutine)
		{
			if (m_behaviour == null)
			{
				throw new NullReferenceException("Unable to start coroutines without a valid MonoBehaviour!");
			}

			return m_behaviour.StartCoroutine(coroutine);
		}

		public void StopCoroutine(Coroutine coroutine)
		{
			if (m_behaviour == null)
			{
				throw new NullReferenceException("Unable to stop coroutines without a valid MonoBehaviour!");
			}

			m_behaviour.StopCoroutine(coroutine);
		}
	}
}