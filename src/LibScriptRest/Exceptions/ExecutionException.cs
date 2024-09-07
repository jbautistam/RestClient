namespace Bau.Libraries.LibScriptRest.Exceptions;

/// <summary>
///		Excepción en la ejecución del programa
/// </summary>
public class ExecutionException : Exception
{
	public ExecutionException() {}

	public ExecutionException(string? message) : base(message) {}

	public ExecutionException(string? message, Exception? innerException) : base(message, innerException) {}
}