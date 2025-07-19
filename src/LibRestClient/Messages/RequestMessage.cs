using System.Text.Json;

namespace Bau.Libraries.LibRestClient.Messages;

/// <summary>
///		Clase con los datos de un mensaje
/// </summary>
public class RequestMessage
{
	/// <summary>
	///		Tipo de método
	/// </summary>
	public enum MethodType
	{
		/// <summary>Método GET</summary>
		Get,
		/// <summary>Método PUT</summary>
		Put,
		/// <summary>Método PATCH</summary>
		Patch,
		/// <summary>Método POST</summary>
		Post,
		/// <summary>Método DELETE</summary>
		Delete,
		/// <summary>Método HEAD</summary>
		Head,
		/// <summary>Método OPTIONS</summary>
		Options
	}

	public RequestMessage(MethodType method, string endPoint, TimeSpan? timeout = null)
	{
		Method = method;
		EndPoint = endPoint;
		Timeout = timeout ?? TimeSpan.FromMinutes(2);
	}

	/// <summary>
	///		Obtiene el contenido HTTP
	/// </summary>
	internal HttpContent? GetHttpContent()
	{
		if (Content is not null)
		{
			if (Content is string json)
				return new StringContent(json, System.Text.Encoding.UTF8, "application/json");
			else
				return new StringContent(JsonSerializer.Serialize(Content), System.Text.Encoding.UTF8, "application/json");
		}
		else
			return null;
	}

	/// <summary>
	///		Obtiene la cadena con los datos para depuración
	/// </summary>
	public string GetDebugString()
	{
		System.Text.StringBuilder builder = new();

			// Asigna los datos principales
			builder.AppendLine($"{Method.ToString().ToUpper()} {GetUrlCall(EndPoint, QueryStrings)}");
			// Añade las cabeceras
			if (Headers is not null)
				foreach (KeyValuePair<string, string?> header in Headers)
					builder.AppendLine($"{header.Key}: {header.Value}");
			builder.AppendLine();
			// Añade el contenido
			if (Content is not null)
				builder.AppendLine(Content?.ToString());
			// Devuelve la cadena
			return builder.ToString();
	}

	/// <summary>
	///		Obtiene la URL de la llamada con sus query strings
	/// </summary>
	private string GetUrlCall(string endPoint, Dictionary<string, object> queryStrings)
	{
		string result = endPoint;
		bool first = true;

			// Añade las queryString
			foreach (KeyValuePair<string, object> queryString in queryStrings)
			{
				// Añade el separador
				if (first)
					result += "?";
				else
					result += "&";
				// Añade el query string
				result += $"{queryString.Key}: {queryString.Value?.ToString()}";
			}
			// Devuelve la cadena
			return result;
	}

	/// <summary>
	///		Punto de entrada
	/// </summary>
	public string EndPoint { get; }

	/// <summary>
	///		Método
	/// </summary>
	public MethodType Method { get; }

	/// <summary>
	///		Cabeceras del mensaje
	/// </summary>
	public Dictionary<string, string?> Headers { get; } = new(StringComparer.CurrentCultureIgnoreCase);

	/// <summary>
	///		Cadenas añadidas al queryString
	/// </summary>
	public Dictionary<string, object> QueryStrings { get; } = new(StringComparer.CurrentCultureIgnoreCase);

	/// <summary>
	///		Contenido
	/// </summary>
	public object? Content { get; set; }

	/// <summary>
	///		Tiempo máximo de espera
	/// </summary>
	public TimeSpan Timeout { get; }
}