using IdentityModel.Client;

namespace Bau.Libraries.LibRestClient.Security.Jwt;

/// <summary>
///		Clase para la seguridad utilizando JWT
/// </summary>
public class JwtSecurity : ISecurity
{
	public JwtSecurity(JwtCredentials credentials)
	{
		Credentials = credentials;
	}

	/// <summary>
	///		Configura la seguridad de un cliente HTTP
	/// </summary>
	public async Task ConfigureAsync(HttpClient httpClient, CancellationToken cancellationToken)
	{
        // Obtiene el token si es necesario
        if (Token is null || TokenExpiration < DateTime.UtcNow)
        {
            Token = await GetTokenAsync(httpClient, cancellationToken);
            TokenExpiration = DateTime.UtcNow.AddSeconds(Token?.ExpiresIn ?? 0);
        }
        // Añade el token de verificación
        if (Token is null || Token.IsError)
            throw new Exception($"Can't get a valid token. {Token?.Error}");
        else
            httpClient.SetBearerToken(Token?.AccessToken);
	}

    /// <summary>
    ///     Obtiene el token
    /// </summary>
    private async Task<TokenResponse?> GetTokenAsync(HttpClient httpClient, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(Credentials.UrlAuthority))
        {
            DiscoveryDocumentResponse discoveryDocument = await httpClient.GetDiscoveryDocumentAsync(Credentials.UrlAuthority, cancellationToken);

                // Comprueba si hay algún error antes de continuar
                if (discoveryDocument.IsError) 
                    throw new Exception(discoveryDocument.Error);
                else
                    return await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                                                                                        {
                                                                                            Address = discoveryDocument.TokenEndpoint,
                                                                                            ClientId = Credentials.ClientId,
                                                                                            ClientSecret = Credentials.ClientSecret,
                                                                                            Scope = Credentials.Scopes
                                                                                        },
                                                                                    cancellationToken);
        }
        else
            return null;
    }

	/// <summary>
	///		Credenciales
	/// </summary>
	public JwtCredentials Credentials { get; }

    /// <summary>
    ///     Token
    /// </summary>
    private TokenResponse? Token { get; set; }

    /// <summary>
    ///     Hora de expiración del token
    /// </summary>
    private DateTime TokenExpiration { get; set; }
}
