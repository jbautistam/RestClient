using System.Net;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibRestClient.Messages;

namespace Bau.Libraries.LibRestClient.Connections;

/// <summary>
///		Clase con los datos de una conexión Rest
/// </summary>
public class RestConnection : IDisposable
{
	// Variables privadas
	private HttpClient? _httpClient;

	public RestConnection(Uri url, TimeSpan? timeout = null, Security.ISecurity? security = null)
	{
		Url = url;
		Timeout = timeout ?? TimeSpan.FromMinutes(2);
		Security = security ?? new Security.None.NoneSecurity();
	}

	/// <summary>
	///     Obtiene un cliente para HTTP
	/// </summary>
	private async Task<HttpClient> GetHttpClientAsync(CancellationToken cancellationToken)
    {
		if (_httpClient is null)
		{
            HttpClientHandler handler = new()
                                            {
                                                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.All
                                            };

			    // Crea un nuevo cliente con el handler para descompresión automática
			    _httpClient = new HttpClient(handler)
                                        {
                                            BaseAddress = Url,
                                            Timeout = Timeout
                                        };
                // Configura la seguridad
                await Security.ConfigureAsync(_httpClient, cancellationToken);
		}
        // Devuelve el cliente
        return _httpClient;
    }

	/// <summary>
	///		Envía un mensaje a la Web
	/// </summary>
	public async Task<ResponseMessage> SendAsync(RequestMessage requestMessage, CancellationToken cancellationToken)
	{
        return await ConvertResponseAsync(await SendAsync(await GetHttpClientAsync(cancellationToken), requestMessage, cancellationToken), 
                                          cancellationToken);
    }

    /// <summary>
    ///     Convierte la respuesta
    /// </summary>
	private async Task<ResponseMessage> ConvertResponseAsync(HttpResponseMessage httpResponse, CancellationToken cancellationToken)
	{
        ResponseMessage response = new();

            // Convierte los datos de la respuesta
            response.StatusCode = (int) httpResponse.StatusCode;
            response.ReasonPhrase = httpResponse.ReasonPhrase;
            // Añade las cabeceras
            foreach (KeyValuePair<string, IEnumerable<string>> header in httpResponse.Headers)
                if (header.Value is not null)
                {
                    string headerValue = string.Empty;

                        // Añade el valor de la cabecera
                        foreach (string part in header.Value)
                            headerValue = headerValue.AddWithSeparator(part, ";");
                        // Añade la cabecera a la respuesta
                        response.Headers.Add(header.Key, headerValue);
                }
                else
                    response.Headers.Add(header.Key, string.Empty);
            // Obtiene el contenido
            response.Content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            // Devuelve la respuesta
            return response;
	}

    /// <summary>
    ///     Envía el mensaje
    /// </summary>
    private async Task<HttpResponseMessage> SendAsync(HttpClient httpClient, RequestMessage request, CancellationToken cancellationToken)
    {
        Uri fullUrl = GetFullUrl(Url, request.EndPoint, request.QueryStrings);
        HttpRequestMessage httpRequest = new()
                                            {
                                                RequestUri = fullUrl,
                                                Method = ConvertMethod(request.Method)
                                            };

            // Añade las cabeceras a la solicitud
            foreach (KeyValuePair<string, string?> keyValue in request.Headers)
                httpRequest.Headers.TryAddWithoutValidation(keyValue.Key, keyValue.Value);
            // Convierte el contenido
            if (request.Content is not null)
                httpRequest.Content = new StringContent(request.Content?.ToString() ?? string.Empty, System.Text.Encoding.UTF8, "application/json");
            // Envía la solicitud y obtiene la respuesta
            return await httpClient.SendAsync(httpRequest, cancellationToken);

        // Convierte el método
        HttpMethod ConvertMethod(RequestMessage.MethodType method)
        {
            return method switch
                {
                    RequestMessage.MethodType.Post => HttpMethod.Post,
                    RequestMessage.MethodType.Put => HttpMethod.Put,
                    RequestMessage.MethodType.Delete => HttpMethod.Delete,
                    _ => HttpMethod.Get
                };
        }
    }

    /// <summary>
    ///     Obtiene la cadena completa incluyendo los argumentos de la query string
    /// </summary>
    private Uri GetFullUrl(Uri urlBase, string endPoint, Dictionary<string, object> queryString)
    {
        string url = urlBase.ToString();

            // Quita la / final
            if (url.EndsWith('/'))
                url = url[..^1];
            // Añade los parámetros de la queryString
            foreach (KeyValuePair<string, object> query in queryString)
            {
                // Añade el separador
                if (endPoint.Contains('?'))
                    endPoint += "&";
                else
                    endPoint += "?";
                // Añade el queryString
                endPoint += $"{query.Key}={query.Value?.ToString()}";
            }
            // Quita la / inicial
            if (endPoint.StartsWith('/'))
                endPoint = endPoint[1..];
            // Crea la url
            url = $"{url}/{endPoint}";
            // Devuelve la cadena completa
            return new Uri(url);
    }

	/// <summary>
	///		Libera la memoria
	/// </summary>
	protected virtual void Dispose(bool disposing)
	{
		if (!Disposed)
		{
			// Libera los recursos administrados
			if (disposing)
			{
                if (_httpClient is not null)
				    _httpClient = null;
			}
			// En su caso, aquí se deberían liberar los recursos no administrados
			// Indica que se ha liberado la memoria
			Disposed = true;
		}
	}

	/// <summary>
	///		Libera la memoria
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	///		Url base
	/// </summary>
	public Uri Url { get; }

	/// <summary>
	///		Tiempo de espera
	/// </summary>
	public TimeSpan Timeout { get; }

	/// <summary>
	///		Servicio de seguridad
	/// </summary>
	public Security.ISecurity Security { get; }

	/// <summary>
	///		Indica si se ha liberado la memoria de este objeto
	/// </summary>
	public bool Disposed { get; private set; }
}