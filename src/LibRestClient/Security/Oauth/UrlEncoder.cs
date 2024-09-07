using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.LibRest.Encoders
{
	/// <summary>
	///		Codificador para las Url
	/// </summary>
	internal static class UrlEncoder
	{ 
		// Constante con los caracteres que no entran dentro del conjunto de caracteres reservados
		private const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

		/// <summary>
		///		Codifica utilizando el sistema RFC 3986
		/// </summary>
		internal static string UrlEncode(this string value, Messages.Parameters.ParameterData.UrlEncoderType encoderType)
		{
			switch (encoderType)
			{
				case Messages.Parameters.ParameterData.UrlEncoderType.Rfc3986:
					return UrlEncodeRFC3986(value);
				default:
					return UrlEncode(value);
			}
		}

		/// <summary>
		///		Codifica utilizando el sistema RFC 3986
		/// </summary>
		internal static string UrlEncodeRFC3986(this string value)
		{
			if (value.IsEmpty())
				return "";
			else
			{
				string encoded = Uri.EscapeDataString(value);

					// Reemplaza los valores codificados por los valores de RFC 3986
					return Regex.Replace(encoded, "(%[0-9a-f][0-9a-f])", chrChar => chrChar.Value.ToUpper())
									.Replace("(", "%28")
									.Replace(")", "%29")
									.Replace("$", "%24")
									.Replace("!", "%21")
									.Replace("*", "%2A")
									.Replace("'", "%27")
									.Replace("%7E", "~");
			}
		}

		/// <summary>
		///		Codifica una URL. Es diferente a la implementación predeterminada de .NET porque envía los caracteres
		///	hexadecimales en mayúsculas como especifica OAuth
		/// </summary>
		internal static string UrlEncode(this string value)
		{
			if (value.IsEmpty())
				return "";
			else
			{
				StringBuilder result = new StringBuilder();

					// Recorre los caracteres codificándolos
					foreach (char symbol in value)
						if (UnreservedChars.IndexOf(symbol) != -1)
							result.Append(symbol);
						else if (string.Format("{0:X2}", (int) symbol).Length > 3) // algunos caracteres producen cadenas con más de dos caracteres, se utiliza el urlencoder predeterminado para obtener el valor correcto
							result.Append(HttpUtility.UrlEncode(value.Substring(value.IndexOf(symbol), 1)).ToUpper());
						else
							result.Append('%' + string.Format("{0:X2}", (int) symbol));
					// Devuelve la cadena codificada
					return result.ToString();
			}
		}
	}
}
