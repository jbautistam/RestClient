namespace Bau.Libraries.LibRestClient.Security;

/// <summary>
///		Interface para los servicios de seguridad
/// </summary>
public interface ISecurity
{
	/// <summary>
	///		Configura la seguridad de un cliente HTTP
	/// </summary>
	Task ConfigureAsync(HttpClient httpClient, CancellationToken cancellationToken);
}
