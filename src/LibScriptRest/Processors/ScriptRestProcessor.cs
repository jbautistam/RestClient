using Bau.Libraries.LibScriptRest.Models.Sentences;

namespace Bau.Libraries.LibScriptRest.Processors;

/// <summary>
///		Procesador para un script de Rest
/// </summary>
internal class ScriptRestProcessor
{
	/// <summary>
	///		Ejecuta un programa
	/// </summary>
	internal Task ExecuteAsync(RestProgram restProgram)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	///		Diccionario de conexiones
	/// </summary>
	internal Dictionary<string, Models.Contexts.Connection> Connections { get; } = new(StringComparer.CurrentCultureIgnoreCase);
}
