namespace Valtaroth.Core.Logging
{
	public interface ILogger
	{
		void Trace(string message, params string[] arguments);
		
		void Debug(string message, params string[] arguments);
		
		void Info(string message, params string[] arguments);

		void Warning(string message, params string[] arguments);

		void Error(string message, params string[] arguments);
		
		void Fatal(string message, params string[] arguments);
	}
}