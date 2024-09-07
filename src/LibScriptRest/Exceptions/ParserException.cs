namespace Bau.Libraries.LibScriptRest.Exceptions;

/// <summary>
///		Excepción en la interpretación del programa
/// </summary>
public class ParserException : Exception
{
	public ParserException() {}

	public ParserException(string? message) : base(message) {}

	public ParserException(string? message, Exception? innerException) : base(message, innerException) {}
}
