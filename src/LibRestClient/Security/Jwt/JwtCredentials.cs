namespace Bau.Libraries.LibRestClient.Security.Jwt;

/// <summary>
///		Credenciales necesarias para comunicaciones utilizando Jwt
/// </summary>
public class JwtCredentials
{
	public JwtCredentials(string urlAuthority, string clientId, string clientSecret, string scopes)
	{
		UrlAuthority = urlAuthority;
		ClientId = clientId;
		ClientSecret = clientSecret;
		Scopes = scopes;
	}

	/// <summary>
	///		Url del servicio de autorización
	/// </summary>
	public string UrlAuthority { get; }
	
	/// <summary>
	///		Usuario del servicio de autorización
	/// </summary>
	public string ClientId { get; }
	
	/// <summary>
	///		Contraseña del servicio de autorización
	/// </summary>
	public string ClientSecret { get; }
	
	/// <summary>
	///		Ambitos
	/// </summary>
	public string Scopes { get; }
}
