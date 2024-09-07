namespace Bau.Libraries.LibRestClient.Security.Basic;

/// <summary>
///		Seguridad básica
/// </summary>
public class BasicSecurity : ISecurity
{
	public BasicSecurity(string user, string password)
	{
		User = user;
		Password = password;
	}

	/// <summary>
	///		Configura la seguridad básica
	/// </summary>
	public async Task ConfigureAsync(HttpClient httpClient, CancellationToken cancellationToken)
	{
		// Evita las advertencias
		await Task.Delay(1, cancellationToken);
		// Cambia las cabeceras de autorización
		httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", GetAuthorizationHeader());

		// Obtiene la cabecera de autentificación
		string GetAuthorizationHeader()
		{
			return Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{User}:{Password}"));
		}
	}

	/// <summary>
	///		Usuario
	/// </summary>
	public string User { get; }

	/// <summary>
	///		Contraseña
	/// </summary>
	public string Password { get; }
}