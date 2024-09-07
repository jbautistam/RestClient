namespace Bau.Libraries.LibScriptRest;

/// <summary>
///		Procesador para scripts de Rest
/// </summary>
public class ScriptRestManager
{
	/// <summary>
	///		Añade una conexión
	/// </summary>
	public void AddConnection(string key, Uri url)
	{
		Processor.Connections.Add(key, new Models.Contexts.Connection(url));
	}

	/// <summary>
	///		Ejecuta las instrucciones de un script Rest
	/// </summary>
	public async Task ExecuteAsync(string script)
	{
		await Processor.ExecuteAsync(new Repository.ScriptRepository().Parse(script));
	}

	/// <summary>
	///		Procesador
	/// </summary>
	private Processors.ScriptRestProcessor Processor { get; } = new();
}
