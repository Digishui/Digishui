using Digishui.Extensions;
using HtmlAgilityPack;
using System.Collections.Specialized;
using System.Net;
using System.Net.Mime;
using System.Text;

//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui
{
  //===========================================================================================================================
  public class Trimaxion
  {
    //-------------------------------------------------------------------------------------------------------------------------
    public Uri CurrentUri { get; private set; } = new Uri("https://www.google.com", UriKind.Absolute);
    public MemoryStream ResponseStream { get; private set; } = null;

    //-------------------------------------------------------------------------------------------------------------------------
    private CookieContainer CookieContainer { get; set; }
    private HttpWebResponse HttpWebResponse { get; set; } = null;
    private Encoding ResponseEncoding { get; set; } = null;
    private string ResponseContentCache { get; set; } = string.Empty;

    //-------------------------------------------------------------------------------------------------------------------------
    public Trimaxion() 
    {
      CookieContainer = new CookieContainer();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public Trimaxion(CookieContainer cookieContainer, Uri referer)
    {
      CookieContainer = cookieContainer;
      CurrentUri = referer;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public async Task OptionsAsync(Uri uri)
    {
      HttpWebResponse = await uri.WebRequestOptionsAsync(CookieContainer, CurrentUri);
      await ProcessResponse();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public async Task OptionsAsync(Uri uri, Dictionary<string, string> requestHeaders)
    {
      ProcessRequestHeaders(uri, requestHeaders);

      HttpWebResponse = await uri.WebRequestOptionsAsync(CookieContainer, requestHeaders, CurrentUri);
      await ProcessResponse();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public async Task OptionsAsync(Uri uri, Dictionary<string, string> requestHeaders, CookieContainer cookieContainer)
    {
      ProcessRequestHeaders(uri, requestHeaders, cookieContainer);

      HttpWebResponse = await uri.WebRequestOptionsAsync(CookieContainer, requestHeaders, CurrentUri);
      await ProcessResponse();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public async Task GetAsync(Uri uri)
    {
      HttpWebResponse = await uri.WebRequestGetAsync(CookieContainer, CurrentUri);
      await ProcessResponse();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public async Task GetAsync(Uri uri, Dictionary<string, string> requestHeaders)
    {
      ProcessRequestHeaders(uri, requestHeaders);

      HttpWebResponse = await uri.WebRequestGetAsync(CookieContainer, requestHeaders, CurrentUri);
      await ProcessResponse();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public async Task GetAsync(Uri uri, Dictionary<string, string> requestHeaders, CookieContainer cookieContainer)
    {
      ProcessRequestHeaders(uri, requestHeaders, cookieContainer);

      HttpWebResponse = await uri.WebRequestGetAsync(CookieContainer, requestHeaders, CurrentUri);
      await ProcessResponse();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public async Task PostAsync(Uri uri, NameValueCollection formData)
    {
      HttpWebResponse = await uri.WebRequestPostAsync(CookieContainer, formData, CurrentUri);
      await ProcessResponse();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public async Task PostAsync(Uri uri, Dictionary<string, string> requestHeaders, NameValueCollection formData)
    {
      ProcessRequestHeaders(uri, requestHeaders);

      HttpWebResponse = await uri.WebRequestPostAsync(CookieContainer, requestHeaders, formData, CurrentUri);
      await ProcessResponse();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public async Task PostAsync(Uri uri, Dictionary<string, string> requestHeaders, CookieContainer cookieContainer, NameValueCollection formData)
    {
      ProcessRequestHeaders(uri, requestHeaders, cookieContainer);

      HttpWebResponse = await uri.WebRequestPostAsync(CookieContainer, requestHeaders, formData, CurrentUri);
      await ProcessResponse();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public async Task PostAsync(Uri uri, NameValueCollection formData, List<FormFile> formFiles)
    {
      HttpWebResponse = await uri.WebRequestPostAsync(CookieContainer, formData, formFiles, CurrentUri);
      await ProcessResponse();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public async Task PostAsync(Uri uri, Dictionary<string, string> requestHeaders, NameValueCollection formData, List<FormFile> formFiles)
    {
      ProcessRequestHeaders(uri, requestHeaders);

      HttpWebResponse = await uri.WebRequestPostAsync(CookieContainer, requestHeaders, formData, formFiles, CurrentUri);
      await ProcessResponse();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public async Task PostAsync(Uri uri, Dictionary<string, string> requestHeaders, CookieContainer cookieContainer, NameValueCollection formData, List<FormFile> formFiles)
    {
      ProcessRequestHeaders(uri, requestHeaders, cookieContainer);

      HttpWebResponse = await uri.WebRequestPostAsync(CookieContainer, requestHeaders, formData, formFiles, CurrentUri);
      await ProcessResponse();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    private void ProcessRequestHeaders(Uri uri, Dictionary<string, string> requestHeaders, CookieContainer cookieContainer = null)
    {
      if (cookieContainer != null) 
      {
        CookieContainer = cookieContainer;

        if (requestHeaders.ContainsKey("Cookie") == true) 
        {
          requestHeaders.Remove("Cookie"); 
        }
      }
      else if (requestHeaders.ContainsKey("Cookie") == true)
      {
        CookieContainer = new CookieContainer();

        string cookieDomain = uri.Host;
        string cookiePath = "/";

        string cookieHeader = requestHeaders["Cookie"];
        string[] cookies = cookieHeader.Split(';');
        foreach (string cookie in cookies)
        {
          string[] cookieParts = cookie.Split('=');
          CookieContainer.Add(new Cookie(cookieParts[0].Trim(), cookieParts[1].Trim(), cookiePath, cookieDomain));
        }

        requestHeaders.Remove("Cookie");
      }

      if (requestHeaders.ContainsKey("Referer") == true) 
      { 
        CurrentUri = new Uri(requestHeaders["Referer"], UriKind.Absolute);

        requestHeaders.Remove("Referer");
      }

      if (requestHeaders.ContainsKey("Content-Length") == true)
      {
        requestHeaders.Remove("Content-Length");
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    private async Task ProcessResponse()
    {
      CurrentUri = HttpWebResponse.ResponseUri;

      ResponseEncoding = (HttpWebResponse.CharacterSet.IsEmpty() == true) ? Encoding.UTF8 : Encoding.GetEncoding(HttpWebResponse.CharacterSet);

      ResponseStream?.Dispose();
      if (ResponseContentCache != String.Empty) { ResponseContentCache = String.Empty; }

      ResponseStream = new MemoryStream();
      await HttpWebResponse.GetResponseStream().CopyToAsync(ResponseStream);
      ResponseStream.Position = 0;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public string ResponseContent
    {
      get
      {
        if (ResponseContentCache == String.Empty)
        {
          if ((ResponseStream?.Length ?? 0) <= 0)
          {
            ResponseContentCache = String.Empty;
          }
          else
          {
            StreamReader streamReader = new(ResponseStream, ResponseEncoding);
            ResponseContentCache = streamReader.ReadToEnd();
            ResponseStream.Position = 0;
          }
        }

        return ResponseContentCache;
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public HttpStatusCode ResponseStatusCode
    {
      get
      {
        return HttpWebResponse.StatusCode;
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public string ResponseContentType
    {
      get
      {
        return HttpWebResponse.ContentType;
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public WebHeaderCollection ResponseHeaders
    {
      get
      {
        return HttpWebResponse.Headers;
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public Dictionary<string, object> ResponseHeadersDictionary
    {
      get
      {
        Dictionary<string, object> headersDictionary = [];

        foreach(var key in ResponseHeaders.AllKeys)
        {
          object value = ResponseHeaders[key];

          if (key == "Set-Cookie")
          {
            string cookieData = value.ToString();
            cookieData = cookieData.Replace("=Sun, ", "=Sun,\a");
            cookieData = cookieData.Replace("=Mon, ", "=Mon,\a");
            cookieData = cookieData.Replace("=Tue, ", "=Tue,\a");
            cookieData = cookieData.Replace("=Wed, ", "=Wed,\a");
            cookieData = cookieData.Replace("=Thu, ", "=Thu,\a");
            cookieData = cookieData.Replace("=Fri, ", "=Fri,\a");
            cookieData = cookieData.Replace("=Sat, ", "=Sat,\a");
            cookieData = cookieData.Replace(", ", "\t");

            string[] cookieArray = cookieData.Split('\t');

            ICollection<string> cookies = [];

            foreach (string cookie in cookieArray)
            {
              cookies.Add(cookie.Replace("\a", " "));
            }

            if (cookies.Count > 1)
            {
              value = cookies;
            }
          }

          headersDictionary.Add(key, value);
        }

        return headersDictionary;
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public ContentDisposition ResponseContentDisposition
    {
      get
      {
        if (HttpWebResponse.Headers["Content-Disposition"] != null) { return new ContentDisposition(HttpWebResponse.Headers["Content-Disposition"]); }
        else { return null; }
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public HtmlNode GetFormNodeById(string formId = null)
    {
      HtmlNode.ElementsFlags.Remove("form");
      HtmlDocument htmlDocument = new();
      htmlDocument.LoadHtml(ResponseContent);

      HtmlNode formNode = null;

      if (formId == null)
      {
        formNode = htmlDocument.DocumentNode.SelectNodes("//form")[0];
      }
      else
      {
        foreach (HtmlNode htmlNode in htmlDocument.DocumentNode.SelectNodes("//form"))
        {
          if (htmlNode.Attributes["id"]?.Value == formId)
          {
            formNode = htmlNode;
          }
        }
      }

      return formNode;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public string GetHrefStartingWith(string hrefStart)
    {
      HtmlDocument htmlDocument = new();
      htmlDocument.LoadHtml(ResponseContent);

      foreach (HtmlNode htmlNode in htmlDocument.DocumentNode.SelectNodes("//a[@href]"))
      {
        string href = htmlNode.Attributes["href"].Value;

        if (href.StartsWith(hrefStart) == true) { return href; }
      }

      return null;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public Uri GetFormActionUri(string baseUrl = null, string formName = null)
    {
      HtmlNode formNode = GetFormNodeById(formName);

      if (baseUrl == null)
      {
        return new Uri(formNode.Attributes["action"]?.Value, UriKind.Absolute);
      }
      else
      {
        return new Uri(new Uri(baseUrl, UriKind.Absolute), formNode.Attributes["action"]?.Value);
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public NameValueCollection GetFormData(string formName = null)
    {
      NameValueCollection formData = [];

      HtmlNode formNode = GetFormNodeById(formName);

      foreach (HtmlNode inputNode in formNode.Descendants("input"))
      {
        string elementType = inputNode.Attributes["type"]?.Value;

        if (elementType == "hidden")
        {
          string elementName = inputNode.Attributes["name"]?.Value;
          string elementValue = (inputNode.Attributes["value"] != null) ? inputNode.Attributes["value"].Value : "";

          if (elementName != null)
          {
            formData.Add(elementName, elementValue);
          }
        }
      }

      return formData;
    }
  }
}
