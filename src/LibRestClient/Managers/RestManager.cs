using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibRestClient.Messages;

namespace Bau.Libraries.LibRestClient.Managers;

/// <summary>
///     Agente para ejecución de peticiones Rest
/// </summary>
internal class RestManager
{
    internal RestManager(RestConnection connection)
    {
        Connection = connection;
    }

	/// <summary>
	///     Realiza una llamada Rest
	/// </summary>
	internal async Task<ResponseMessage> CallAsync(RequestMessage requestMessage, CancellationToken cancellationToken)
    {
        using (HttpClient httpClient = await GetHttpClientAsync(requestMessage.Timeout, cancellationToken))
        {
            return await GetResponseAsync(await SendAsync(httpClient, requestMessage, cancellationToken), cancellationToken);
        }
    }

    /// <summary>
    ///     Convierte la respuesta
    /// </summary>
	private async Task<ResponseMessage> GetResponseAsync(HttpResponseMessage httpResponse, CancellationToken cancellationToken)
	{
        ResponseMessage response = new();

            // Convierte los datos de la respuesta
            response.StatusCode = (int) httpResponse.StatusCode;
            response.ReasonPhrase = httpResponse.ReasonPhrase;
            // Añade las cabeceras
            foreach (KeyValuePair<string, IEnumerable<string>> header in httpResponse.Headers)
                if (header.Value != null)
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
	///     Obtiene un cliente para HTTP
	/// </summary>
	private async Task<HttpClient> GetHttpClientAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        HttpClient httpClient = new HttpClient
                                    {
                                        BaseAddress = Connection.Url,
                                        Timeout = timeout
                                    };

            // Configura la seguridad
            await Connection.Security.ConfigureAsync(httpClient, cancellationToken);
            // Devuelve el cliente
            return httpClient;
    }

    /// <summary>
    ///     Envía el mensaje
    /// </summary>
    private async Task<HttpResponseMessage> SendAsync(HttpClient httpClient, RequestMessage request, CancellationToken cancellationToken)
    {
        string fullUrl = GetFullUrl(request.EndPoint, request.QueryStrings);

            // Llama al método
            return request.Method switch
                {
                    RequestMessage.MethodType.Get => await httpClient.GetAsync(fullUrl, cancellationToken),
                    RequestMessage.MethodType.Post => await httpClient.PostAsync(fullUrl, request.GetHttpContent(), cancellationToken),
                    RequestMessage.MethodType.Put => await httpClient.PutAsync(fullUrl, request.GetHttpContent(), cancellationToken),
                    RequestMessage.MethodType.Delete => await httpClient.DeleteAsync(fullUrl, cancellationToken),
                    _ => throw new ArgumentException($"Unknown method: {request.Method.ToString()}")
                };
    }

    /// <summary>
    ///     Obtiene la cadena completa incluyendo los argumentos de la query string
    /// </summary>
    private string GetFullUrl(string method, Dictionary<string, object> queryString)
    {
        // Añade los parámetros de la queryString
        foreach (KeyValuePair<string, object> query in queryString)
        {
            // Añade el separador
            if (method.Contains('?'))
                method += "&";
            else
                method += "?";
            // Añade el queryString
            method += $"{query.Key}={query.Value?.ToString()}";
        }
        // Devuelve la cadena completa
        return method;
    }

    /// <summary>
    ///     Conexión
    /// </summary>
    internal RestConnection Connection { get; }
}
