using Newtonsoft.Json;

namespace Bau.Libraries.LibRestClient.Messages;

/// <summary>
///		Clase con los datos de un mensaje de respuesta
/// </summary>
public class ResponseMessage
{
	/// <summary>
	///		Obtiene la cadena de depuración con el contenido del mensaje
	/// </summary>
	public string GetDebugString()
	{
		System.Text.StringBuilder builder = new();

			// Asigna los datos principales
			builder.AppendLine($"Status code: {StatusCode.ToString()} - Reason: {ReasonPhrase ?? "NULL"}");
			builder.AppendLine();
			// Añade las cabeceras
			foreach (KeyValuePair<string, string> header in Headers)
				builder.AppendLine($"{header.Key}: {header.Value}");
			builder.AppendLine();
			// Añade el contenido
			builder.AppendLine(Content);
			// Devuelve la cadena
			return builder.ToString();
	}

    /// <summary>
    ///     Deserializa el cuerpo de un mensaje 
    /// </summary>
    public TypeData? GetContentToAsync<TypeData>()
    {
		if (string.IsNullOrWhiteSpace(Content))
			return default;
		else
			return JsonConvert.DeserializeObject<TypeData>(Content);
    }

	/// <summary>
	///		Código de estado
	/// </summary>
	public int StatusCode { get; set; }

	/// <summary>
	///		Texto de respuesta de estado del servidor
	/// </summary>
	public string? ReasonPhrase { get; set; }

	/// <summary>
	///		Cabeceras del mensaje
	/// </summary>
	public Dictionary<string, string> Headers { get; } = new(StringComparer.CurrentCultureIgnoreCase);

	/// <summary>
	///		Contenido
	/// </summary>
	public string Content { get; set; } = string.Empty;
}