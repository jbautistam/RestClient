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
		EndPoint = endPoint;
		Method = method;
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
			builder.AppendLine($"{Method.ToString()} {EndPoint}");
			builder.AppendLine();
			// Añade las queryString
			if (QueryStrings is null || QueryStrings.Count == 0)
				builder.AppendLine("No query string");
			else
				foreach (KeyValuePair<string, object> queryString in QueryStrings)
					builder.AppendLine($"{queryString.Key}: {queryString.Value?.ToString() ?? "Null value"}");
			builder.AppendLine();
			// Añade las cabeceras
			if (Headers is null || Headers.Count == 0)
				builder.AppendLine("No headers");
			else
				foreach (KeyValuePair<string, string> header in Headers)
					builder.AppendLine($"{header.Key}: {header.Value}");
			builder.AppendLine();
			// Añade el contenido
			if (Content is null)
				builder.AppendLine("No content");
			else
				builder.AppendLine(Content?.ToString() ?? "Null content");
			// Devuelve la cadena
			return builder.ToString();
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
	public Dictionary<string, string> Headers { get; } = new(StringComparer.CurrentCultureIgnoreCase);

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