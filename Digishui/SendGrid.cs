//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui
{
  //===========================================================================================================================
  public class SendGrid
  {
    internal static string ApiKey { get; private set; } = null;
    internal static string DefaultFromAddress { get; private set; } = null;
    internal static string DefaultFromName { get; private set; } = null;
    internal static string DefaultBcc { get; private set; } = null;

    //-------------------------------------------------------------------------------------------------------------------------
    public static void Init(string apiKey, string defaultFromAddress, string defaultFromName, string defaultBcc)
    {
      ApiKey = apiKey;
      DefaultFromAddress = defaultFromAddress;
      DefaultFromName = defaultFromName;
      DefaultBcc = defaultBcc;
    }
  }
}