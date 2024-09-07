using System.Net.Http.Headers;

namespace Bau.Libraries.LibRestClient.Security.ApiKey;

/// <summary>
///		Seguridad con ApiKey (o bearer)
/// </summary>
public class ApiKeySecurity : ISecurity
{
	public ApiKeySecurity(string apiKey)
	{
		ApiKey = apiKey;
	}

	/// <summary>
	///		Configura la seguridad básica
	/// </summary>
	public async Task ConfigureAsync(HttpClient httpClient, CancellationToken cancellationToken)
	{
		// Evita las advertencias
		await Task.Delay(1, cancellationToken);
		// Cambia las cabeceras de autorización
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
	}

	/// <summary>
	///		Clave de API
	/// </summary>
	public string ApiKey { get; }
}