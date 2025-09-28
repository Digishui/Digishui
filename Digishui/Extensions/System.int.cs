//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui.Extensions
{
  //===========================================================================================================================
  /// <summary>
  ///   System.int Extensions
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
    public static int RoundUpToNearest(this int value, int boundary)
    {
      return (int)(Math.Ceiling((decimal)value / (decimal)boundary) * (decimal)boundary);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that returns a number with a 'th', 'rd' type suffix (1 = 1st, 3 = 3rd, etc).
    /// </summary>
    /// <param name="value">Value to suffix.</param>
    public static string ToSuffixedString(this int value)
    {
      string valueAsString = value.ToString();

      if (valueAsString.EndsWith("11")) { return valueAsString + "th"; }
      if (valueAsString.EndsWith("12")) { return valueAsString + "th"; }
      if (valueAsString.EndsWith("13")) { return valueAsString + "th"; }
      if (valueAsString.EndsWith('1')) { return valueAsString + "st"; } 
      if (valueAsString.EndsWith('2')) { return valueAsString + "nd"; }
      if (valueAsString.EndsWith('3')) { return valueAsString + "rd"; }
      return valueAsString + "th";
    }
  }
}