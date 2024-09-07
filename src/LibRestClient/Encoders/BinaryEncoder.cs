using System.Text;

namespace Bau.Libraries.LibRestClient.Encoders;

/// <summary>
///		Codificador / decodificador binario
/// </summary>
public class BinayEncoder : IEncoder
{
	/// <summary>
	///		Codifica una cadena (a base 64)
	/// </summary>
	public string Encode(string source, bool subject)
	{
		return Encode(Encoding.ASCII.GetBytes(source), subject);
	}

	/// <summary>
	///		Codifica un array de bytes
	/// </summary>
	public string Encode(byte[] source, bool subject)
	{
		byte[] asciiBytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, source);
		char[] asciiChars = new char[Encoding.ASCII.GetCharCount(asciiBytes, 0, asciiBytes.Length)];

			// Obtiene los caracteres de la codificación
			Encoding.ASCII.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
			// Devuelve la cadena a partir del array de caracteres
			return new string(asciiChars);
	}

	/// <summary>
	///		Decodifica una cadena desde Base64 a otra cadena
	/// </summary>
	public string Decode(string source, bool subject)
	{
		return Encoding.UTF8.GetString(Encoding.ASCII.GetBytes(source));
	}

	/// <summary>
	///		Decodificar una cadena desde un array de bytes
	/// </summary>
	public string Decode(byte[] source, bool subject)
	{
		return Encoding.UTF8.GetString(source);
	}

	/// <summary>
	///		Decodifica una cadena en base 64 a un array de bytes
	/// </summary>
	public byte[] DecodeToBytes(string source, bool subject)
	{
		return DecodeToBytes(Encoding.ASCII.GetBytes(source), subject);
	}

	/// <summary>
	///		Decodifica un array de bytes en un array de bytes
	/// </summary>
	public byte[] DecodeToBytes(byte[] source, bool subject)
	{
		return Encoding.Convert(Encoding.UTF8, Encoding.UTF8, source);
	}
}