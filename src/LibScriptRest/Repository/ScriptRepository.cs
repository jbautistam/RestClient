using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibRestClient.Messages;
using Bau.Libraries.LibScriptRest.Models.Sentences;

namespace Bau.Libraries.LibScriptRest.Repository;

/// <summary>
///		Repositorio para un archivo XML de instrucciones Rest
/// </summary>
internal class ScriptRepository
{	
	// Constantes privadas
	private const string TagRoot = "RestScript";
	private const string TagRequest = "Request";
	private const string TagMethod = "Method";
	private const string TagController = "Controller";
	private const string TagTarget = "Target";
	private const string TagQueryString = "QueryString";
	private const string TagHeader = "Header";
	private const string TagBody = "Body";
	private const string TagName = "Name";
	private const string TagValue = "Value";

	/// <summary>
	///		Interpreta las instrucciones de un texto
	/// </summary>
	internal RestProgram Parse(string xml)
	{
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().ParseText(xml);
		RestProgram program = new RestProgram();

			// Carga los datos
			foreach (MLNode rootML in fileML.Nodes)
				if (rootML.Name == TagRoot)
					foreach (MLNode nodeML in rootML.Nodes)
						switch (nodeML.Name)
						{
							case TagRequest:
									program.Instructions.Add(LoadInstruction(nodeML));
								break;
						}
			// Devuelve el programa
			return program;
	}

	/// <summary>
	///		Obtiene una instrucción
	/// </summary>
	private RestInstruction LoadInstruction(MLNode rootML)
	{
		return new RestInstruction(rootML.Attributes[TagTarget].Value.TrimIgnoreNull(), LoadRequest(rootML));
	}

	/// <summary>
	///		Carga los datos de una solicitud
	/// </summary>
	private RequestMessage LoadRequest(MLNode rootML)
	{
		RequestMessage request = new RequestMessage(rootML.Attributes[TagMethod].Value.GetEnum(RequestMessage.MethodType.Get),
													rootML.Attributes[TagController].Value.TrimIgnoreNull());

			// Asigna los datos
			LoadQueryStrings(rootML.Nodes, request.QueryStrings);
			LoadHeaders(rootML.Nodes, request.Headers);
			request.Content = rootML.Nodes[TagBody].Value.TrimIgnoreNull();
			// Devuelve la solicitud
			return request;
	}

	/// <summary>
	///		Carga los datos de query strings
	/// </summary>
	private void LoadQueryStrings(MLNodesCollection nodesML, Dictionary<string, object> queryStrings)
	{
		foreach (MLNode nodeML in nodesML)
			if (nodeML.Name == TagQueryString)
				queryStrings.Add(nodeML.Attributes[TagName].Value.TrimIgnoreNull(), nodeML.Attributes[TagValue].Value.TrimIgnoreNull());
	}

	/// <summary>
	///		Carga las cabeceras
	/// </summary>
	private void LoadHeaders(MLNodesCollection nodesML, Dictionary<string, string> headers)
	{
		foreach (MLNode nodeML in nodesML)
			if (nodeML.Name == TagHeader)
				headers.Add(nodeML.Attributes[TagName].Value.TrimIgnoreNull(), nodeML.Attributes[TagValue].Value.TrimIgnoreNull());
	}
}