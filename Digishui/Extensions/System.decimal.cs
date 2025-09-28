//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui.Extensions
{
  //===========================================================================================================================
  /// <summary>
  ///   System.decimal Extensions
  /// </summary>
  public static partial class Extensions
  {
    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Rounds supplied value up to the nearest multiple of the supplied boundary.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="boundary"></param>
    /// <returns></returns>
    public static decimal RoundUpToNearest(this decimal value, decimal boundary)
    {
      return (Math.Ceiling(value / boundary) * boundary);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that converts the supplied decimal to an integer.
    /// </summary>
    /// <param name="value">String value to convert.</param>
    /// <returns>Integer that the string represents.</returns>
    public static int ToInt(this decimal value)
    {
      return (int)Math.Round(value);
    }
  }
}