using System.Collections.Specialized;
using System.Net;

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
    private static HttpWebRequest CreateRequest(Uri uri,
                                                CookieContainer cookieContainer,
                                                Dictionary<string, string> requestHeaders,
                                                Uri refererUri = null,
                                                bool ignoreCertificateValidationErrors = false)
    {
      HttpWebRequest httpWebRequest = WebRequest.CreateHttp(uri);

      if (ignoreCertificateValidationErrors == true) { httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; }; }

      httpWebRequest.CookieContainer = cookieContainer;

      httpWebRequest.AllowAutoRedirect = true;

      httpWebRequest.AutomaticDecompression = DecompressionMethods.All;

      httpWebRequest.Referer = refererUri?.ToString() ?? "https://www.google.com";

      if (requestHeaders.ContainsKey("User-Agent") == false)
      {
        httpWebRequest.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:136.0) Gecko/20100101 Firefox/136.0";
      }
      else
      {
        httpWebRequest.UserAgent = requestHeaders["User-Agent"];
        requestHeaders.Remove("User-Agent");
      }

      if (requestHeaders.ContainsKey("DNT") == false) { requestHeaders.Add("DNT", "1"); }
      if (requestHeaders.ContainsKey("Cache-Control") == false) { requestHeaders.Add("Cache-Control", "no-cache"); }
      if (requestHeaders.ContainsKey("Connection") == false) { requestHeaders.Add("Connection", "keep-alive"); }
      if (requestHeaders.ContainsKey("Accept") == false) { requestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"); }

      foreach (KeyValuePair<string, string> header in requestHeaders)
      {
        httpWebRequest.Headers[header.Key] = header.Value;
      }

      httpWebRequest.ReadWriteTimeout = 60000;
      httpWebRequest.Timeout = 60000;

      return httpWebRequest;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static async Task<HttpWebResponse> WebRequestOptionsAsync(this Uri uri,
                                                                     CookieContainer cookieContainer,
                                                                     Uri refererUri = null)
    {
      return await WebRequestOptionsAsync
      (
        uri,
        cookieContainer,
        [],
        refererUri
      );
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static async Task<HttpWebResponse> WebRequestOptionsAsync(this Uri uri,
                                                                     CookieContainer cookieContainer,
                                                                     Dictionary<string, string> requestHeaders,
                                                                     Uri refererUri = null)
    {
      HttpWebRequest httpWebRequest = CreateRequest(uri, cookieContainer, requestHeaders, refererUri);
      HttpWebResponse httpWebResponse = await httpWebRequest.OptionsAsync();
      return httpWebResponse;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static async Task<HttpWebResponse> WebRequestGetAsync(this Uri uri,
                                                                 CookieContainer cookieContainer,
                                                                 Uri refererUri = null)
    {
      return await WebRequestGetAsync
      (
        uri,
        cookieContainer,
        [],
        refererUri
      );
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static async Task<HttpWebResponse> WebRequestGetAsync(this Uri uri,
                                                                 CookieContainer cookieContainer,
                                                                 Dictionary<string, string> requestHeaders,
                                                                 Uri refererUri = null)
    {
      HttpWebRequest httpWebRequest = CreateRequest(uri, cookieContainer, requestHeaders, refererUri);
      HttpWebResponse httpWebResponse = await httpWebRequest.GetAsync();
      return httpWebResponse;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static async Task<HttpWebResponse> WebRequestPostAsync(this Uri uri,
                                                                  CookieContainer cookieContainer,
                                                                  NameValueCollection formData,
                                                                  Uri refererUri = null)
    {
      return await WebRequestPostAsync
      (
        uri,
        cookieContainer,
        [],
        formData,
        refererUri
      );
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static async Task<HttpWebResponse> WebRequestPostAsync(this Uri uri,
                                                                  CookieContainer cookieContainer,
                                                                  Dictionary<string, string> requestHeaders,
                                                                  NameValueCollection formData,
                                                                  Uri refererUri = null)
    {
      HttpWebRequest httpWebRequest = CreateRequest(uri, cookieContainer, requestHeaders, refererUri);
      HttpWebResponse httpWebResponse = await httpWebRequest.PostAsync(formData);
      return httpWebResponse;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static async Task<HttpWebResponse> WebRequestPostAsync(this Uri uri,
                                                                  CookieContainer cookieContainer,
                                                                  NameValueCollection formData,
                                                                  List<FormFile> formFiles,
                                                                  Uri refererUri = null)
    {
      return await WebRequestPostAsync
      (
        uri,
        cookieContainer,
        [],
        formData,
        formFiles,
        refererUri
      );
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <remarks>
    ///   https://stackoverflow.com/questions/566462/upload-files-with-httpwebrequest-multipart-form-data
    /// </remarks>
    public static async Task<HttpWebResponse> WebRequestPostAsync(this Uri uri,
                                                                  CookieContainer cookieContainer,
                                                                  Dictionary<string, string> requestHeaders,
                                                                  NameValueCollection formData,
                                                                  List<FormFile> formFiles,
                                                                  Uri refererUri = null)
    {
      string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

      HttpWebRequest httpWebRequest = CreateRequest(uri, cookieContainer, requestHeaders, refererUri);
      httpWebRequest.ContentType = $"multipart/form-data; boundary={boundary}";
      httpWebRequest.Method = "POST";
      httpWebRequest.KeepAlive = true;

      Stream requestMemoryStream = await httpWebRequest.GetRequestStreamAsync();

      if (formData != null)
      {
        foreach (string key in formData.Keys)
        {
          //If the key contains "[]//", we're dealing with the desire to submit an unindexed array based on the order
          //that fields bearing the same key preceding the "[]//" we added to the formData NameValueCollection. This is
          //handled just by submitting the same form field over and over again, and the server parses it based on order
          //of inclusion. So, the part following "[]//" is a descriptor to differentiate values in the collection, and
          //it's not sent in the POST payload. Technically, NameValueCollection halfway handles this scenario, in that
          //if you try to add entries with the same key multiple times it accepts tham and comma separates them, but this
          //becomes problematic if one of the values contains commas (it doesn't quote comma-containing values, so you
          //can't figure out if it's a new field with the same name or a value containing a comma). This is a hack around
          //the implementation of the NameValueCollection as it pertains to duplication submission of unindexed formField
          //arrays.
          string formFieldName = key;
          if (formFieldName.Contains("[]//") == true) { formFieldName = formFieldName[..(key.IndexOf("[]//") + 2)]; }

          string formItem = $"\r\n--{boundary}\r\nContent-Disposition: form-data; name=\"{formFieldName}\";\r\n\r\n{formData[key]}";
          byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(formItem);
          await requestMemoryStream.WriteAsync(formItemBytes);
        }
      }

      foreach (FormFile formFile in formFiles)
      {
        string header = $"\r\n--{boundary}\r\nContent-Disposition: form-data; name=\"{formFile.FormFieldName}\"; filename=\"{formFile.FileName}\"\r\nContent-Type: {formFile.ContentType ?? "application/octet-stream"}\r\n\r\n";
        byte[] headerBytes = System.Text.Encoding.UTF8.GetBytes(header);
        await requestMemoryStream.WriteAsync(headerBytes);
        await formFile.Stream.CopyToAsync(requestMemoryStream, true);
      }

      byte[] endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes($"\r\n--{boundary}--");
      await requestMemoryStream.WriteAsync(endBoundaryBytes);

      return (HttpWebResponse)(await httpWebRequest.GetResponseAsync());
    }
  }
}
