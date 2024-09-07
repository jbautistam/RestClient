namespace Bau.Libraries.LibRestClient;

/// <summary>
///		Clase con los datos de una conexión Rest
/// </summary>
public class RestConnection : IDisposable
{
	// Variables privadas
	private Managers.RestManager? _restManager;

	public RestConnection(Uri url, Security.ISecurity? security = null)
	{
		Url = url;
		Security = security ?? new Security.None.NoneSecurity();
	}

	/// <summary>
	///		Envía un mensaje a la Web
	/// </summary>
	public async Task<Messages.ResponseMessage> SendAsync(Messages.RequestMessage requestMessage, CancellationToken cancellationToken)
	{
		return await RestManager.CallAsync(requestMessage, cancellationToken);
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
				_restManager = null;
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
	///		Servicio de seguridad
	/// </summary>
	public Security.ISecurity Security { get; }

	/// <summary>
	///		Manager de REST
	/// </summary>
	private Managers.RestManager RestManager
	{
		get
		{
			// Crea el manager de REST si no existe en memoria
			_restManager ??= new Managers.RestManager(this);
			// Devuelve el manager de rest
			return _restManager;
		}
	}

	/// <summary>
	///		Indica si se ha liberado la memoria de este objeto
	/// </summary>
	public bool Disposed { get; private set; }
}