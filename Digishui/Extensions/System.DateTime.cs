//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui.Extensions
{
  //===========================================================================================================================
  /// <summary>
  ///   System.DateTime Extensions
  /// </summary>
  public static partial class Extensions
  {
    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Returns the supplied DateTime value changed to include the supplied year.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    public static DateTime ChangeYear(this DateTime value, int year)
    {
      return new DateTime(year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Returns the supplied DateTime value changed to include the supplied month.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="Year"></param>
    /// <returns></returns>
    public static DateTime ChangeMonth(this DateTime value, int month)
    {
      return new DateTime(value.Year, month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Returns the supplied DateTime value changed to include the supplied day.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="day"></param>
    /// <returns></returns>
    public static DateTime ChangeDay(this DateTime value, int day)
    {
      return new DateTime(value.Year, value.Month, day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Converts the supplied UTC DateTime value to a Eastern Standard DateTime.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTime ToEasternStandard(this DateTime value)
    {
      return TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Converts the supplied Eastern Standard DateTime value to a UTC DateTime.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTime FromEasternStandard(this DateTime value)
    {
      return TimeZoneInfo.ConvertTimeToUtc(value, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Converts the supplied UTC DateTime value to a Central Standard DateTime.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTime ToCentralStandard(this DateTime value)
    {
      return TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Converts the supplied Central Standard DateTime value to a UTC DateTime.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTime FromCentralStandard(this DateTime value)
    {
      return TimeZoneInfo.ConvertTimeToUtc(value, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Converts the supplied UTC DateTime value to a Mountain Standard DateTime.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTime ToMountainStandard(this DateTime value)
    {
      return TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time"));
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Converts the supplied Mountain Standard DateTime value to a UTC DateTime.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTime FromMountainStandard(this DateTime value)
    {
      return TimeZoneInfo.ConvertTimeToUtc(value, TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time"));
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Converts the supplied UTC DateTime value to a Pacific Standard DateTime.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTime ToPacificStandard(this DateTime value)
    {
      return TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Converts the supplied Pacific Standard DateTime value to a UTC DateTime.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTime FromPacificStandard(this DateTime value)
    {
      return TimeZoneInfo.ConvertTimeToUtc(value, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static bool IsHoliday(this DateTime dateTime)
    {
      if (dateTime.DayOfYear == 1) { return true; } //New Year's Day
      else if (dateTime.IsEasterSunday() == true) { return true; } //Easter Sunday
      else if (dateTime.IsMemorialDay() == true) { return true; } //Memorial Day
      else if ((dateTime.Month == 7) && (dateTime.Day == 4)) { return true; } //Independence Day
      else if (dateTime.IsLaborDay() == true) { return true; } //Labor Day
      else if (dateTime.IsThanksgiving() == true) { return true; } //Thanksgiving
      else if (dateTime.IsBlackFriday() == true) { return true; } //Black Friday
      else if ((dateTime.Month == 12) && (dateTime.Day == 24)) { return true; } //Christmas Eve
      else if ((dateTime.Month == 12) && (dateTime.Day == 25)) { return true; } //Christmas
      else return false;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <remarks>
    ///   https://social.msdn.microsoft.com/Forums/vstudio/en-US/36d8ad39-647f-40b0-b745-36d3b639a576/c-monthcalendar-and-easter?forum=csharpgeneral
    /// </remarks>
    private static bool IsEasterSunday(this DateTime dateTime)
    {
      int year = dateTime.Year;

      int g = year % 19;
      int c = year / 100;
      int h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25) + 19 * g + 15) % 30;
      int i = h - (int)(h / 28) * (1 - (int)(h / 28) * (int)(29 / (h + 1)) * (int)((21 - g) / 11));

      int day = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
      int month = 3;

      if (day > 31)
      {
        month++;
        day -= 31;
      }

      return (dateTime.Date == (new DateTime(year, month, day)));
    }

    //-------------------------------------------------------------------------------------------------------------------------
    private static bool IsMemorialDay(this DateTime dateTime)
    {
      DateTime memorialDay = new(dateTime.Year, 5, 31);

      while (memorialDay.DayOfWeek != DayOfWeek.Monday)
      {
        memorialDay = memorialDay.AddDays(-1);
      }

      return (dateTime.Date == memorialDay);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    private static bool IsLaborDay(this DateTime dateTime)
    {
      DateTime laborDay = new(dateTime.Year, 9, 1);

      while (laborDay.DayOfWeek != DayOfWeek.Monday)
      {
        laborDay = laborDay.AddDays(1);
      }

      return (dateTime.Date == laborDay);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    private static bool IsThanksgiving(this DateTime dateTime)
    {
      DateTime thanksgiving = new(dateTime.Year, 11, 1);

      while (thanksgiving.DayOfWeek != DayOfWeek.Thursday)
      {
        thanksgiving = thanksgiving.AddDays(1);
      }

      thanksgiving = thanksgiving.AddDays(21);

      return (dateTime.Date == thanksgiving);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    private static bool IsBlackFriday(this DateTime dateTime)
    {
      DateTime thanksgiving = new(dateTime.Year, 11, 1);

      while (thanksgiving.DayOfWeek != DayOfWeek.Thursday)
      {
        thanksgiving = thanksgiving.AddDays(1);
      }

      thanksgiving = thanksgiving.AddDays(21);

      DateTime blackFriday = thanksgiving.AddDays(1);

      return (dateTime.Date == blackFriday);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    private static DateTime Round(this DateTime dateTime, TimeSpan timeSpan, bool roundUp)
    {
      var remainder = dateTime.Ticks % timeSpan.Ticks;

      if (remainder == 0)
      {
        return dateTime;
      }

      long delta;

      if (roundUp)
      {
        delta = timeSpan.Ticks - remainder;
      }
      else
      {
        delta = -remainder;
      }

      return dateTime.AddTicks(delta);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static DateTime RoundDown(this DateTime dateTime, TimeSpan timeSpan)
    {
      return dateTime.Round(timeSpan, false);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static DateTime RoundUp(this DateTime dateTime, TimeSpan timeSpan)
    {
      return dateTime.Round(timeSpan, true);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Returns the start of the week for the supplied datetime.
    /// </summary>
    /// <remarks>
    ///   https://stackoverflow.com/questions/38039/how-can-i-get-the-datetime-for-the-start-of-the-week
    /// </remarks>
    /// <param name="dateTime">Datetime for which the start of the week is desired.</param>
    /// <param name="startOfWeek">Starting day of the week. Default = Sunday.</param>
    /// <returns>Start of the week for the supplied datetime.</returns>
    public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Sunday)
    {
      int diff = dateTime.DayOfWeek - startOfWeek;

      if (diff < 0) { diff += 7; }

      return dateTime.AddDays(-1 * diff).Date;
    }
  }
}