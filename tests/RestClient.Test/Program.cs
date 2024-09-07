using Bau.Libraries.LibRestClient;
using Bau.Libraries.LibRestClient.Messages;

RestConnection connection = new(new Uri("https://catfact.ninja/"));

// Solicita los personajes a la API
await RequestAsync(connection, RequestMessage.MethodType.Get, "fact");

// Solicitud al servicio
async Task RequestAsync(RestConnection connection, RequestMessage.MethodType method, string controller)
{
	try
	{
		await TreatRequestAsync(connection, new RequestMessage(method, controller, TimeSpan.FromMinutes(2)));
	}
	catch (Exception exception)
	{
		Console.WriteLine($"Error when call {method.ToString()} : {connection.Url.ToString()} : {controller}", exception.Message);
	}
}


// Muestra la información de una solicitud / respuesta
async Task TreatRequestAsync(RestConnection connection, RequestMessage request)
{
	ResponseMessage response = await connection.SendAsync(request, CancellationToken.None);

		// Muestra la información de la conexión
		Console.WriteLine($"Connection url: {connection.Url.ToString()}");
		// Muestra la información de la solicitud
		Console.WriteLine(request.GetDebugString());
		// Separador
		WriteLogSeparator('-');
		// Muestra la información de la respuesta
		if (response is null)
			Console.WriteLine("No response");
		else
			Console.WriteLine(response.GetDebugString());
		// Separador
		WriteLogSeparator('=');
}

// Escribe un separador en la consola
void WriteLogSeparator(char separator)
{
	Console.WriteLine(new string(separator, 80));
	Console.WriteLine();
}
