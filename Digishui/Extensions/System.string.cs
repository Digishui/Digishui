using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui.Extensions
{
  //===========================================================================================================================
  /// <summary>
  ///   System.string Extensions
  /// </summary>
  public static partial class Extensions
  {
    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that collapses all white space in the string.
    /// </summary>
    /// <param name="originalString"></param>
    /// <returns></returns>
    public static string CollapseWhitespace(this string originalString)
    {
      //If the string is null or empty, there's no work to do.
      if (string.IsNullOrEmpty(originalString)) { return originalString; }

      StringBuilder stringBuilder = new();

      bool newWhitespaceRegionDetected = false;
      bool firstNonWhitespaceRegionDetected = false;

      foreach (char character in originalString)
      {
        if (char.IsWhiteSpace(character))
        {
          newWhitespaceRegionDetected = true;
        }
        else
        {
          //Add space to the output string if we're in a new whitespace region, unless we have not yet detected a
          //non-whitespace region.  This ensures that all whitespace is condensed down to single spaces, and that there is no
          //leading whitespace.
          if (newWhitespaceRegionDetected && firstNonWhitespaceRegionDetected) { stringBuilder.Append(' '); }

          //Add the current character to the output string.
          stringBuilder.Append(character);

          newWhitespaceRegionDetected = false;
          firstNonWhitespaceRegionDetected = true;

        }
      }

      return stringBuilder.ToString();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Generates an MD5 hash of the supplied string.
    /// </summary>
    /// <param name="value">Value to hash</param>
    /// <returns>MD5 hash of the supplied input</returns>
    public static string GetMD5Hash(this string value)
    {
      return value.ToStream().GetMD5Hash();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Replaces the last instance of the specified string with the 
    /// </summary>
    /// <param name="sourceString"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    /// <returns></returns>
    public static string ReplaceLast(this string sourceString, string oldValue, string newValue)
    {
      int oldValueStartIndex = sourceString.LastIndexOf(oldValue);

      if (oldValueStartIndex == -1) { return sourceString; }

      return sourceString.Remove(oldValueStartIndex, oldValue.Length).Insert(oldValueStartIndex, newValue);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Returns the leftmost specified characters from the supplied string.
    /// </summary>
    /// <param name="value">String to parse.</param>
    /// <param name="maxLength">Leftmost number of characters to return.</param>
    /// <returns></returns>
    public static string Left(this string value, int maxLength)
    {
      //Check if the value is valid
      if (value.IsEmpty() == true)
      {
        //Set valid empty string as string could be null
        value = string.Empty;
      }
      else if (value.Length > maxLength)
      {
        //Make the string no longer than the max length
        value = value[..maxLength];
      }

      //Return the string
      return value;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Returns the rightmost specified characters from the supplied string.
    /// </summary>
    /// <param name="value">String to parse.</param>
    /// <param name="maxLength">Rightmost number of characters to return.</param>
    /// <returns></returns>
    public static string Right(this string value, int maxLength)
    {
      //Check if the value is valid
      if (value.IsEmpty() == true)
      {
        //Set valid empty string as string could be null
        value = string.Empty;
      }
      else if (value.Length > maxLength)
      {
        //Make the string no longer than the max length
        value = value.Substring(value.Length - maxLength, maxLength);
      }

      //Return the string
      return value;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that determines if the supplied string value is a valid representation of a GUID.
    /// </summary>
    /// <param name="value">String value to evaluate.</param>
    /// <returns>Boolean value indicating whether the supplied string is a valid representation of a GUID.</returns>
    public static bool IsGuid(this string value)
    {
      if (value.IsEmpty() == true) return false;

      return Guid.TryParse(value, out _);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that determines if the supplied string value is a valid representation of a bool.
    /// </summary>
    /// <param name="value">String value to evaluate.</param>
    /// <returns>Boolean value indicating whether the supplied string is a valid representation of a bool.</returns>
    public static bool IsBool(this string value)
    {
      if (value.IsEmpty() == true) { return false; }

      return bool.TryParse(value, out _);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that determines if the supplied string value is a valid representation of a DateTime.
    /// </summary>
    /// <param name="value">String value to evaluate.</param>
    /// <returns>Boolean value indicating whether the supplied string is a valid representation of a DateTime.</returns>
    public static bool IsDateTime(this string value)
    {
      if (value.IsEmpty() == true) { return false; }

      value = value.Trim().ToLower();

      if ((value.IsInt() == true) && (value.ToInt() >= -657435) && (value.ToInt() <= 99999)) { value += ".0"; }

      if ((value.IsNumeric() == true) && (value.IsInt() == false) && (value.ToDecimal() >= -657435.0m) && (value.ToDecimal() <= 2958465.99999999m))
      {
        //OLE Automation Dates.
        return true;
      }
      else if ((value.IsNumeric() == true) && (value.IsInt() == true) && (value.Length == 8))
      {
        //yyyyMMdd.
        return DateTime.TryParseExact(value, "yyyyMMdd", null, DateTimeStyles.None, out _);
      }
      else if ((value.Length >= 23) && ((value.EndsWith(" am") == true) || (value.EndsWith(" pm") == true)))
      {
        //Format encountered in real world.
        return DateTime.TryParseExact(value, "dd-MMM-yy h.mm.sss.fff tt", null, DateTimeStyles.None, out _);
      }
      else
      {
        return DateTime.TryParse(value, out _);
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that determines if the supplied string value is numeric.
    /// </summary>
    /// <param name="value">String value to evaluate.</param>
    /// <returns>Boolean value indicating whether the supplied string is numeric.</returns>
    public static bool IsNumeric(this string value)
    {
      if (value.IsEmpty() == true) { return false; }

      value = value.Replace(" ", "");
      value = value.Replace(",", "");
      value = value.Replace("$", "");

      return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that determines if the supplied string value is an integer.
    /// </summary>
    /// <param name="value">String value to evaluate.</param>
    /// <returns>Boolean value indicating whether the supplied string is an integer.</returns>
    public static bool IsInt(this string value)
    {
      if (value == null) { return false; }

      value = value.Replace(" ", "");
      value = value.Replace(",", "");
      value = value.Replace("$", "");

      return int.TryParse(value, out _);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Returns boolean value indicating if the supplied value is an email address.
    /// </summary>
    /// <remarks>
    ///   https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
    /// </remarks>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsEmail(this string value)
    {
      //If the string is null or empty, it is not an email address.
      if (String.IsNullOrEmpty(value)) { return false; }

      //If the string matches this regex, we consider it to be an email address.
      return Regex.IsMatch(value,
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that determines if the supplied string value is empty.  If the string is null, has no contents, or
    ///   contains only whitespace, it is considered empty.
    /// </summary>
    /// <param name="value">String value to evaluate.</param>
    /// <returns>Boolean value indicating whether the supplied string is empty.</returns>
    public static bool IsEmpty(this string value)
    {
      if (value != null) { value = value.Trim(); }

      return string.IsNullOrEmpty(value);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Returns a reversed copy of the supplied string.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static string Reverse(this string value)
    {
      char[] charArray = value.ToCharArray();
      Array.Reverse(charArray);
      return new string(charArray);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that returns the supplied string as a stream.
    /// </summary>
    /// <param name="value">String value to convert.</param>
    /// <returns>Stream representation of the string.</returns>
    public static Stream ToStream(this string value)
    {
      //We intentionally do not create MyMemoryStream with a using block here because MyMemoryStream needs to survive in an 
      //open state outside the scope of this function.  MemoryStream objects automatically dispose when they completely fall 
      //out of scope, so there is no concern of leaks.
      MemoryStream memoryStream = new();

      //Write the supplied string to our stream.
      WriteToStream(value, memoryStream);

      //Rewind the stream so it can be used without additional required steps.
      memoryStream.Position = 0;

      //Return the stream.
      return memoryStream;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that writes the supplied string the the supplied output stream.
    /// </summary>
    /// <param name="value">String value to write to the supplied stream.</param>
    /// <param name="outputStream">Output stream to which the string should be written.</param>
    public static void WriteToStream(this string value, Stream outputStream)
    {
      using StreamWriter streamWriter = new(outputStream, Encoding.Default, 1024, true);
      streamWriter.Write(value);
      streamWriter.Flush();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that writes the supplied string the the supplied output stream.
    /// </summary>
    /// <param name="value">String value to write to the supplied stream.</param>
    /// <param name="outputStream">Output stream to which the string should be written.</param>
    public static async Task WriteToStreamAsync(this string value, Stream outputStream)
    {
      using StreamWriter streamWriter = new(outputStream, Encoding.Default, 1024, true);
      await streamWriter.WriteAsync(value);
      await streamWriter.FlushAsync();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that converts the supplied string to a datetime.
    /// </summary>
    /// <param name="value">String value to convert.</param>
    /// <returns>DateTime that the string represents.</returns>
    public static DateTime ToDateTime(this string value)
    {
      if (value.IsDateTime() == false) { throw new ArgumentException("The string cannot be converted to a DateTime because it does not represent a DateTime value."); }

      value = value.Trim().ToLower();
      int valueLength = value.Length;
      bool valueIsInt = value.IsInt();

      if (((valueIsInt == true) && (value.ToInt() >= -657435) && (value.ToInt() <= 99999)) || ((value.IsNumeric() == true) && (value.ToDecimal() >= -657435.0m) && (value.ToDecimal() <= 2958465.99999999m)))
      {
        //OLE Automation Dates.
        return DateTime.FromOADate(Convert.ToDouble(value));
      }
      else if ((valueIsInt == true) && (valueLength == 8))
      {
        //yyyyMMdd.
        return DateTime.ParseExact(value, "yyyyMMdd", null, DateTimeStyles.None);
      }
      else if ((valueLength >= 23) && ((value.EndsWith(" am") == true) || (value.EndsWith(" pm") == true)))
      {
        //Format encountered in real world.
        return DateTime.ParseExact(value, "dd-MMM-yy h.mm.sss.fff tt", null, DateTimeStyles.None);
      }
      else
      {
        return DateTime.Parse(value);
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that converts the supplied string to a datetime or returns null.
    /// </summary>
    /// <param name="value">String value to convert.</param>
    /// <returns>DateTime that the string represents or null.</returns>
    public static DateTime? ToDateTimeOrNull(this string value)
    {
      if (value.IsDateTime() == false) { return null; }

      return ToDateTime(value);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that converts the supplied string to a decimal.
    /// </summary>
    /// <param name="value">String value to convert.</param>
    /// <returns>Decimal that the string represents.</returns>
    public static decimal ToDecimal(this string value)
    {
      if (value.IsNumeric() == false) { throw new ArgumentException("The string cannot be converted to a decimal because it does not represent a numeric value."); }

      value = value.Replace(" ", "");
      value = value.Replace(",", "");
      value = value.Replace("$", "");

      return decimal.Parse(value, NumberStyles.Any);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that converts the supplied string to a decimal.
    /// </summary>
    /// <param name="value">String value to convert.</param>
    /// <returns>Decimal that the string represents.</returns>
    public static decimal? ToDecimalOrNull(this string value)
    {
      if (value.IsNumeric() == false) { return null; }

      value = value.Replace(" ", "");
      value = value.Replace(",", "");
      value = value.Replace("$", "");

      return decimal.Parse(value, NumberStyles.Any);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that converts the supplied string to an integer.
    /// </summary>
    /// <param name="value">String value to convert.</param>
    /// <returns>Integer that the string represents.</returns>
    public static int ToInt(this string value)
    {
      if (value.IsInt() == false) { throw new ArgumentException("The string cannot be converted to an integer because it does not represent an integer value."); }

      value = value.Replace(" ", "");
      value = value.Replace(",", "");
      value = value.Replace("$", "");

      return int.Parse(value);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that converts the supplied string to an integer.
    /// </summary>
    /// <param name="value">String value to convert.</param>
    /// <returns>Integer that the string represents.</returns>
    public static int? ToIntOrNull(this string value)
    {
      if (value.IsInt() == false) { return null; }

      value = value.Replace(" ", "");
      value = value.Replace(",", "");
      value = value.Replace("$", "");

      return int.Parse(value);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that converts the supplied string to a Guid.
    /// </summary>
    /// <param name="value">String value to convert.</param>
    /// <returns>Guid that the string represents.</returns>
    public static Guid ToGuid(this string value)
    {
      if (value.IsGuid() == false) { throw new ArgumentException("The string cannot be converted to a Guid because it does not represent an Guid value."); }

      return Guid.Parse(value);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that converts the supplied string to a Guid.
    /// </summary>
    /// <param name="value">String value to convert.</param>
    /// <returns>Guid that the string represents.</returns>
    public static Guid? ToGuidOrNull(this string value)
    {
      if (value.IsGuid() == false) { return null; }

      return Guid.Parse(value);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that returns the number of times the specified value occurs in the extended string.
    /// </summary>
    /// <param name="extendedString">String in which we're counting the occurences of the value.</param>
    /// <param name="value">String value to count.</param>
    /// <returns>Number of times the value occurs in the string.</returns>
    public static int Count(this string extendedString, string value)
    {
      return ((extendedString.Length - extendedString.Replace(value, "").Length) / value.Length);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Extension method that returs a masked string, keeping the specified number of characters from the beginning and
    ///   end of the string, if applicable.
    /// </summary>
    /// <param name="value">String to mask.</param>
    /// <param name="keepFirst">Number of characters to keep from the beginning of the string.</param>
    /// <param name="keepLast">Number of characters to keep from the end of the string.</param>
    /// <param name="maskCharacter">Character to use to mask the string.</param>
    /// <returns></returns>
    public static string Mask(this string value, int keepFirst = 0, int keepLast = 4, char maskCharacter = 'X')
    {
      if (value.IsEmpty() == true) { return value; }

      if (value.Trim().Length <= (keepFirst + keepLast)) { return value; }

      string mask = new(maskCharacter, value.Trim().Length - keepFirst - keepLast);

      return value.Left(keepFirst) + mask + value.Right(keepLast);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Determines if the string is a publicly switched telephone number.
    /// </summary>
    /// <param name="value">String to evaluate.</param>
    /// <returns>Boolean indicating if the evaluated string is a publicly switched telephone number.</returns>
    public static bool IsPstn(this string value)
    {
      if (value.IsEmpty() == true) { return false; }

      return (PstnUtil.StorageFormat(value) != null);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Determines if the string is a time zone.
    /// </summary>
    /// <param name="value">String to evaluate.</param>
    /// <returns>Boolean indicating if the evaluated string is a time zone.</returns>
    public static bool IsTimeZone(this string value)
    {
      List<string> timeZoneList = [.. TimeZoneInfo.GetSystemTimeZones().Select(s => s.Id)];

      return timeZoneList.Contains(value);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Returns only the numbers in the supplied string.
    /// </summary>
    /// <remarks>
    ///   https://stackoverflow.com/questions/3977497/stripping-out-non-numeric-characters-in-string
    /// </remarks>
    /// <param name="value">String to evaluate.</param>
    /// <returns>String containing only the numbers that were in the supplied string.</returns>
    public static string GetNumbers(this string value)
    {
      return new string([.. value.Where(w => char.IsDigit(w))]);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Determines if the string is an ABA Routing Number
    /// </summary>
    /// <remarks>
    ///   http://www.brainjar.com/js/validation/
    /// </remarks>
    /// <param name="value">String to evaluate.</param>
    /// <returns>Boolean indicating if the evaluated string is an ABA Routing Number.</returns>
    public static bool IsAbaRoutingNumber(this string value)
    {
      if (value == null) { return false; }
      
      value = value.GetNumbers();

      if (value.Length != 9) { return false; }

      int checksum = 0;

      for (int index = 0; index < 9; index += 3)
      {
        checksum += (value.Substring(index, 1).ToInt() * 3)
                 + (value.Substring(index + 1, 1).ToInt() * 7)
                 + value.Substring(index + 2, 1).ToInt();
      }

      return ((checksum != 0) && ((checksum % 10) == 0));
    }
  }
}