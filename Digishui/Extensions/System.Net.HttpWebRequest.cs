using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui.Extensions
{
  //===========================================================================================================================
  /// <summary>
  ///   System.Uri Extensions
  /// </summary>
  public static partial class Extensions
  {
    //-------------------------------------------------------------------------------------------------------------------------
    public static async Task<HttpWebResponse> OptionsAsync(this HttpWebRequest httpWebRequest)
    {
      httpWebRequest.Method = "OPTIONS";

      return await httpWebRequest.GetResponseAsyncNoException();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static async Task<HttpWebResponse> GetAsync(this HttpWebRequest httpWebRequest)
    {
      httpWebRequest.Method = "GET";

      return await httpWebRequest.GetResponseAsyncNoException();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static async Task<HttpWebResponse> PostAsync(this HttpWebRequest httpWebRequest,
                                                        NameValueCollection formData)
    {
      httpWebRequest.Method = "POST";
      httpWebRequest.ContentType = "application/x-www-form-urlencoded";
      httpWebRequest.AllowWriteStreamBuffering = true;

      var postDataStringBuilder = new StringBuilder();

      foreach (string name in formData)
      {
        foreach (string value in formData.GetValues(name))
        {
          if (postDataStringBuilder.Length > 0) { postDataStringBuilder.Append('&'); }

          postDataStringBuilder.Append(WebUtility.UrlEncode(name));

          postDataStringBuilder.Append('=');

          postDataStringBuilder.Append(WebUtility.UrlEncode(value));
        }
      }

      using (Stream requestStream = await httpWebRequest.GetRequestStreamAsync())
      {
        ASCIIEncoding asciiEncoding = new();
        byte[] postBytes = asciiEncoding.GetBytes(postDataStringBuilder.ToString());
        await requestStream.WriteAsync(postBytes);
      }

      return (HttpWebResponse)(await httpWebRequest.GetResponseAsyncNoException());
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <see cref="https://stackoverflow.com/questions/10081726/why-does-httpwebrequest-throw-an-exception-instead-returning-httpstatuscode-notf"/>
    /// <param name="httpWebRequest"></param>
    /// <returns></returns>
    public static async Task<HttpWebResponse> GetResponseAsyncNoException(this HttpWebRequest httpWebRequest)
    {
      try
      {
        return (HttpWebResponse)(await httpWebRequest.GetResponseAsync());
      }
      catch (WebException webException)
      {
        if (webException.Response is not HttpWebResponse webResponse) { throw; }

        return webResponse;
      }
    }
  }
}
