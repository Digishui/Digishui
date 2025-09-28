using System.Text;

//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui.Extensions
{
  //===========================================================================================================================
  /// <summary>
  ///   System.Text.StringBuilder Extensions
  /// </summary>
  public static partial class Extensions
  {
    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that writes the supplied StringBuilder content the the supplied output stream.
    /// </summary>
    /// <param name="value">StringBuilder content to write to the supplied output stream.</param>
    /// <param name="outputStream">Output stream to which the StringBuilder should be written.</param>
    public static void WriteToStream(this StringBuilder value, Stream outputStream)
    {
      value.ToString().WriteToStream(outputStream);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that writes the supplied StringBuilder content the the supplied output stream.
    /// </summary>
    /// <param name="value">StringBuilder content to write to the supplied output stream.</param>
    /// <param name="outputStream">Output stream to which the StringBuilder should be written.</param>
    public static async Task WriteToStreamAsync(this StringBuilder value, Stream outputStream)
    {
      await value.ToString().WriteToStreamAsync(outputStream);
    }
  }
}
