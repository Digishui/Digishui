//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui
{
  //===========================================================================================================================
  public static class DateTimeUtil
  {
    //-------------------------------------------------------------------------------------------------------------------------
    public static System.DateTime CentralStandardNow
    {
      get
      {
        return TimeZoneInfo.ConvertTimeFromUtc(System.DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
      }
    }
  }
}
