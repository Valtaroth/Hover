using System.Collections;
using UnityEngine;

namespace Valtaroth.Core.Infrastructure
{
	public interface ICoroutineInvoker
	{
		Coroutine StartCoroutine(IEnumerator coroutine);
		void StopCoroutine(Coroutine coroutine);
	}
}