using System.Text;

//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui.Extensions
{
  //===========================================================================================================================
  /// <summary>
  ///   System.IO.Stream Extensions
  /// </summary>
  public static partial class Extensions
  {
    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Generates an MD5 hash of the supplied input.
    /// </summary>
    /// <param name="value">Value to hash</param>
    /// <returns>MD5 hash of the supplied input</returns>
    public static string GetMD5Hash(this Stream value)
    {
      //Start from the beginning of the stream.
      value.Position = 0;

      // Use input string to calculate MD5 hash
      System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
      byte[] hashBytes = md5.ComputeHash(value);

      // Convert the byte array to lowercase hexadecimal string
      StringBuilder stringBuilder = new();
      for (int i = 0; i < hashBytes.Length; i++)
      {
        stringBuilder.Append(hashBytes[i].ToString("x2"));
      }

      //Return the MDS hash.
      return stringBuilder.ToString();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that rewinds the supplied InputStream and copies it to the supplied OutputStream.
    /// </summary>
    /// <param name="inputStream">Stream to rewind and copy to the OutputStream.</param>
    /// <param name="outputStream">Stream to which the rewinded InputStream should be copied.</param>
    public static void CopyTo(this Stream inputStream, Stream outputStream, bool rewindFirst = false)
    {
      if (rewindFirst == true) { inputStream.Position = 0; }

      inputStream.CopyTo(outputStream);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that rewinds the supplied InputStream and copies it to the supplied OutputStream.
    /// </summary>
    /// <param name="inputStream">Stream to rewind and copy to the OutputStream.</param>
    /// <param name="outputStream">Stream to which the rewinded InputStream should be copied.</param>
    public static async Task CopyToAsync(this Stream inputStream, Stream outputStream, bool rewindFirst = false)
    {
      if (rewindFirst == true) { inputStream.Position = 0; }

      await inputStream.CopyToAsync(outputStream);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that returns a string representation of the supplied stream using the supplied or system default 
    ///   text encoding.
    /// </summary>
    /// <param name="value">Stream for which a string representation will be returned.</param>
    /// <param name="encoding">Text encoding for supplied stream.</param>
    /// <returns>String representation of the supplied stream using the supplied or system default text encoding.</returns>
    public static string GetString(this Stream value, Encoding encoding = null)
    {
      encoding ??= Encoding.Default;

      using MemoryStream memoryStream = new();

      value.CopyTo(memoryStream, true);

      return encoding.GetString(memoryStream.ToArray());
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that returns a string representation of the supplied stream using the supplied or system default 
    ///   text encoding.
    /// </summary>
    /// <param name="value">Stream for which a string representation will be returned.</param>
    /// <param name="encoding">Text encoding for supplied stream.</param>
    /// <returns>String representation of the supplied stream using the supplied or system default text encoding.</returns>
    public static async Task<string> GetStringAsync(this Stream value, Encoding encoding = null)
    {
      encoding ??= Encoding.Default;

      using MemoryStream memoryStream = new();

      await value.CopyToAsync(memoryStream, true);

      return encoding.GetString(memoryStream.ToArray());
    }
  }
}
