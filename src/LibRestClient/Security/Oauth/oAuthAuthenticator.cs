using System;
using System.Security.Cryptography;
using System.Text;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibRest.Encoders;
using Bau.Libraries.LibRest.Messages;
using Bau.Libraries.LibRest.Messages.Parameters;

namespace Bau.Libraries.LibRest.Authentication.Oauth
{
	/// <summary>
	///		Rutinas de autentificación para sistemas oAuth
	/// </summary>
	public class oAuthAuthenticator : IAuthenticator
	{ 
		// Constantes privadas
		private const string Version = "1.0";
		private const string cnstStrSignatureMethod = "HMAC-SHA1";

		public oAuthAuthenticator(string consumerKey = null, string consumerSecret = null, string accessToken = null, string accessTokenSecret = null)
		{
			ConsumerKey = consumerKey;
			ConsumerSecret = consumerSecret;
			AccessToken = accessToken;
			AccessTokenSecret = accessTokenSecret;
		}

		/// <summary>
		///		Procesa la autentificación
		/// </summary>
		public void Process(RequestMessage request)
		{
			string nonce, timestamp, signature;

				// Inicializa los valores aleatorios
				nonce = new Random().Next(int.MaxValue).ToString("X"); // 123400, 9999999
				timestamp = ((int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
				// Genera la firma
				signature = GetSignature(GetDataToSign(request.MethodDescription, request.URL, request.QueryParameters, nonce, timestamp));
				// Añade la cabecera de autorización
				request.Headers.Add("Authorization", GetAuthorizationData(signature, nonce, timestamp));
		}

		/// <summary>
		///		Obtiene la firma a partir de los datos
		/// </summary>
		private string GetSignature(string dataToSign)
		{
			HMACSHA1 sha1 = new HMACSHA1(Encoding.ASCII.GetBytes(string.Format("{0}&{1}", ConsumerSecret.UrlEncode(), AccessTokenSecret.UrlEncode())));

				// Devuelve la firma
				return Convert.ToBase64String(sha1.ComputeHash(Encoding.ASCII.GetBytes(dataToSign.ToString())));
		}

		/// <summary>
		///		Obtiene los datos a firmar
		/// </summary>
		private string GetDataToSign(string methodDescription, string url, ParameterDataCollection queryStrings, string nonce, string timeStamp)
		{
			ParameterDataCollection parameters = queryStrings.Clone();
			string dataToSign = "";

				// Añade los valores oAuth a los parámetros
				parameters.Add("oauth_version", Version);
				parameters.Add("oauth_consumer_key", ConsumerKey);
				parameters.Add("oauth_nonce", nonce);
				parameters.Add("oauth_signature_method", cnstStrSignatureMethod);
				parameters.Add("oauth_timestamp", timeStamp);
				parameters.Add("oauth_token", AccessToken);
				// Ordena los parámetros
				parameters.SortByKey();
				// Añade los parámetros a la cadena
				foreach (ParameterData parameter in parameters)
					dataToSign = dataToSign.AddWithSeparator(parameter.Key + "=" + parameter.ValueEncoded, "&", false);
				// Codifica los parámetros y lo añade a los datos a firmar
				dataToSign = methodDescription + "&" + NormalizeUrl(url).UrlEncode() + "&" + dataToSign.UrlEncodeRFC3986();
				// Devuelve los datos a firmar
				return dataToSign;
		}

		/// <summary>
		///		Obtiene los datos de autorización
		/// </summary>
		private string GetAuthorizationData(string signature, string nonce, string timestamp)
		{
			string authorizationData = "OAuth ";

				// Añade los datos de autorización
				authorizationData += GetAuthorizationParameter("oauth_nonce", nonce) + ",";
				authorizationData += GetAuthorizationParameter("oauth_signature_method", cnstStrSignatureMethod) + ",";
				authorizationData += GetAuthorizationParameter("oauth_timestamp", timestamp) + ",";
				authorizationData += GetAuthorizationParameter("oauth_consumer_key", ConsumerKey) + ",";
				authorizationData += GetAuthorizationParameter("oauth_token", AccessToken) + ",";
				authorizationData += GetAuthorizationParameter("oauth_signature", signature) + ",";
				authorizationData += GetAuthorizationParameter("oauth_version", Version);
				// Devuelve la cadena
				return authorizationData;
		}

		/// <summary>
		///		Devuelve un parámetro de la cadena de autorización formateado
		/// </summary>
		private string GetAuthorizationParameter(string key, string value)
		{
			return $"{key}=\"{value.UrlEncode()}\"";
		}

		/// <summary>
		///		Normaliza una URL
		/// </summary>
		private string NormalizeUrl(string url)
		{
			Uri uri = new Uri(url);
			string port = string.Empty;

				// Obtiene el puerto si no es el predeterminado
				if (uri.Scheme.EqualsIgnoreCase("http") && uri.Port != 80 ||
						uri.Scheme.EqualsIgnoreCase("https") && uri.Port != 443 ||
						uri.Scheme.EqualsIgnoreCase("ftp") && uri.Port != 20)
					port = $":{uri.Port}";
				// Devuelve la URL normalizada
				return $"{uri.Scheme}://{uri.Host}{port}{uri.AbsolutePath}";
		}

		/// <summary>
		///		Obtiene los tokens de autorización a Twitter para que la aplicación pueda validar un usuario
		/// </summary>
		public bool GetAuthorizationTokens(string urlRequestToken, string urlCallBack, out string oAuthToken, out string oAuthTokenSecret)
		{
			RequestMessage request = new RequestMessage(RequestMessage.MethodType.Post, urlRequestToken);
			ResponseMessage response;

				// Añade los parámetros
				request.QueryParameters.Add("oaut_callback", urlCallBack);
				// Envía el mensaje al servidor
				response = GetResponseOAuth(request);
				// Si todo ha ido bien, el servidor oAuth nos responde con una cadena del tipo: oauth_token=xxx&oauth_token_secret=xxx&oauth_callback_confirmed=true
				ExtractTokensAccess(response, out oAuthToken, out oAuthTokenSecret);
				// Devuelve la cadena
				return !oAuthToken.IsEmpty() && !oAuthTokenSecret.IsEmpty();
		}

		/// <summary>
		///		Obtiene el token de acceso a partir de un PIN
		/// </summary>
		public bool GetAccessToken(string urlRequestAccessToken, string pin, out string oAuthToken, out string oAuthTokenSecret)
		{
			RequestMessage request = new RequestMessage(RequestMessage.MethodType.Post, urlRequestAccessToken);
			ResponseMessage response;

				// Añade los parámetros
				request.QueryParameters.Add("oauth_verifier", pin);
				// Envía el mensaje al servidor
				response = GetResponseOAuth(request);
				// Obtiene los tokens de la respuesta
				ExtractTokensAccess(response, out oAuthToken, out oAuthTokenSecret);
				// Devuelve el valor que indica si todo ha ido correcto
				return !oAuthToken.IsEmpty() && !oAuthTokenSecret.IsEmpty();
		}

		/// <summary>
		///		Envía una solicitud Web utilizando oAuth
		/// </summary>
		private ResponseMessage GetResponseOAuth(RequestMessage request)
		{
			RestController objRestController = new RestController("BauRest", 20000, this);

				// Devuelve la respuesta del servidor
				return objRestController.Send(request);
		}

		/// <summary>
		///		Extrae los tokens de acceso de una respuesta
		/// </summary>
		private void ExtractTokensAccess(ResponseMessage response, out string oAuthToken, out string oAuthTokenSecret)
		{ 
			// Inicializa los valores de salida
			oAuthToken = null;
			oAuthTokenSecret = null;
			// Obtiene los tokens de la respuesta
			if (!response.IsError && !response.Body.IsEmpty())
			{
				string[] bodyParts = response.Body.Split('&');

					// Recorre el array buscando las cadenas
					if (bodyParts != null && bodyParts.Length > 0)
						foreach (string body in bodyParts)
						{
							string[] queryParts = body.Split('=');

								if (queryParts != null && queryParts.Length == 2)
								{
									if (queryParts[0].EqualsIgnoreCase("oauth_token"))
										oAuthToken = queryParts[1];
									else if (queryParts[0].EqualsIgnoreCase("oauth_token_secret"))
										oAuthTokenSecret = queryParts[1];
								}
						}
			}
		}

		/// <summary>
		///		Clave de aplicación
		/// </summary>
		public string ConsumerKey { get; set; }

		/// <summary>
		///		Clave secreta de aplicación
		/// </summary>
		public string ConsumerSecret { get; set; }

		/// <summary>
		///		Token de acceso
		/// </summary>
		public string AccessToken { get; set; }

		/// <summary>
		///		Token de acceso secreto
		/// </summary>
		public string AccessTokenSecret { get; set; }
	}
}