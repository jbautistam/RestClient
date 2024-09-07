namespace Bau.Libraries.LibRestClient.Security.None;

/// <summary>
///		Clase utilizada cuando no se establece ninguna seguridad
/// </summary>
public class NoneSecurity : ISecurity
{
	/// <summary>
	///		Configura la seguridad: en este caso, no hace nada
	/// </summary>
	public async Task ConfigureAsync(HttpClient httpClient, CancellationToken cancellationToken)
	{
		await Task.Delay(1, cancellationToken);
	}
}
