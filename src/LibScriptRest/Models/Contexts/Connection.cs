namespace Bau.Libraries.LibScriptRest.Models.Contexts;

/// <summary>
///		Datos de una conexión
/// </summary>
internal class Connection
{
	internal Connection(Uri url)
	{
		Url = url;
	}

	/// <summary>
	///		Url de la conexión
	/// </summary>
	internal Uri Url { get; }
}
