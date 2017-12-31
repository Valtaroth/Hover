namespace Valtaroth.Core.Logging
{
	public class ConsoleLogger : ILogger
	{
		public void Trace(string message, params string[] arguments)
		{
			UnityEngine.Debug.LogFormat(message, arguments);
		}

		public void Debug(string message, params string[] arguments)
		{
			UnityEngine.Debug.LogFormat(message, arguments);
		}

		public void Info(string message, params string[] arguments)
		{
			UnityEngine.Debug.LogFormat(message, arguments);
		}

		public void Warning(string message, params string[] arguments)
		{
			UnityEngine.Debug.LogWarningFormat(message, arguments);
		}

		public void Error(string message, params string[] arguments)
		{
			UnityEngine.Debug.LogErrorFormat(message, arguments);
		}

		public void Fatal(string message, params string[] arguments)
		{
			UnityEngine.Debug.LogErrorFormat(message, arguments);
		}
	}
}