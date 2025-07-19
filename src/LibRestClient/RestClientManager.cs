using Bau.Libraries.LibRestClient.Connections;

namespace Bau.Libraries.LibRestClient;

/// <summary>
///		Manager de envío de mensajes a conexiones Rest
/// </summary>
public class RestClientManager : IDisposable
{
	/// <summary>
	///		Añade una conexión
	/// </summary>
	public void AddConnection(Uri url, TimeSpan timeout, Security.ISecurity? security = null)
	{	
		RestConnection connection = new(url, timeout, security ?? new Security.None.NoneSecurity());

			// Añade / modifica la conexión
			if (RestConnections.ContainsKey(url))
				RestConnections[url] = connection;
			else
				RestConnections.Add(connection.Url, connection);
	}

	/// <summary>
	///		Envía un mensaje a la Web
	/// </summary>
	public async Task<Messages.ResponseMessage> SendAsync(Uri url, Messages.RequestMessage requestMessage, CancellationToken cancellationToken)
	{
		RestConnection? connection = GetConnection(url);

			if (connection is null)
				throw new ArgumentException($"Can't find a connection for url {url.ToString()}");
			else
				return await connection.SendAsync(requestMessage, cancellationToken);
	}

	/// <summary>
	///		Obtiene una conexión
	/// </summary>
	private RestConnection? GetConnection(Uri url)
	{
		if (RestConnections.TryGetValue(url, out RestConnection? connection))
			return connection;
		else
			return null;
	}

	/// <summary>
	///		Limpia las conexiones
	/// </summary>
	public void Clear()
	{
		// Libera las conexiones
		foreach (KeyValuePair<Uri, RestConnection> keyValue in RestConnections)
			keyValue.Value.Dispose();
		// Limpia la lista de conexiones
		RestConnections.Clear();
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
				Clear();
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
	///		Indica si se ha liberado la memoria de este objeto
	/// </summary>
	public bool Disposed { get; private set; }

	/// <summary>
	///		Conexiones
	/// </summary>
	private Dictionary<Uri, RestConnection> RestConnections { get; } = [];
}