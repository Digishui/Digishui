using System.Globalization;

//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui.Extensions
{
  //===========================================================================================================================
  /// <summary>
  ///   System.object Extensions
  /// </summary>
  public static partial class Extensions
  {
    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that converts the supplied object to a string, or if the object is null either the optional supplied 
    ///   string or a null value.
    /// </summary>
    /// <param name="value">Object to convert to a string.</param>
    /// <param name="nullString">
    ///   Optional string to return if the object is null.  If this value is not supplied, this method will return a null
    ///   value if the object is null.
    /// </param>
    /// <returns>
    ///   String representation of the object, or if the object is null either the optional supplied string or a null value.
    /// </returns>
    /// <remarks>
    ///   Consider using null-coalescing operator instead.
    /// </remarks>
    public static string ToStringOrNull(this object value, string nullString = null)
    {
      if (value == null) { return nullString; }

      return value.ToString();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Determines if the supplied object represents a datetime.
    /// </summary>
    /// <param name="value">The value to inspect.</param>
    /// <returns></returns>
    public static bool IsDateTime(this object value)
    {
      if (value != null)
      {
        if (value is DateTime)
        {
          return true;
        }
        else if (value is string stringValue)
        {
          if ((stringValue.Trim().Length == 8) && (stringValue.IsNumeric() == true))
          {
            return DateTime.TryParseExact(stringValue, "yyyyMMdd", null, DateTimeStyles.None, out _);
          }
          else
          {
            return DateTime.TryParse(stringValue, out _);
          }
        }
      }

      return false;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that determines if the supplied object represents a number.
    /// </summary>
    /// <param name="value">String value to evaluate.</param>
    /// <returns>Boolean value indicating whether the supplied string is numeric.</returns>
    public static bool IsNumeric(this object value)
    {
      if (value == null) { return false; }

      if (value is byte) { return true; }
      if (value is sbyte) { return true; }
      if (value is short) { return true; }
      if (value is ushort) { return true; }
      if (value is int) { return true; }
      if (value is uint) { return true; }
      if (value is long) { return true; }
      if (value is ulong) { return true; }
      if (value is float) { return true; }
      if (value is double) { return true; }
      if (value is decimal) { return true; }

      if (value is string)
      {
        string stringValue = value.ToString();

        if (stringValue.IsEmpty() == true) return false;

        stringValue = stringValue.Replace(" ", "");
        stringValue = stringValue.Replace(",", "");
        stringValue = stringValue.Replace("$", "");


        return decimal.TryParse(stringValue, out _);
      }

      return false;
    }
  }
}