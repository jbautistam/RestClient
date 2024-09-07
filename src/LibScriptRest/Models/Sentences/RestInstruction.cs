using Bau.Libraries.LibRestClient.Messages;

namespace Bau.Libraries.LibScriptRest.Models.Sentences;

/// <summary>
///		Instrucción para envío de mensajes
/// </summary>
public class RestInstruction
{
    public RestInstruction(string target, RequestMessage request)
    {
        Target = target;
        Request = request;
    }

    /// <summary>
    ///		Conexión sobre la que se ejecuta la solicitud
    /// </summary>
    public string Target { get; }

    /// <summary>
    ///		Solicitud
    /// </summary>
    public RequestMessage Request { get; }
}
