using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Reflection;
using System.Text;

//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui.Extensions
{
  //===========================================================================================================================
  /// <summary>
  ///   SendGrid.SendGridMessage Extensions
  /// </summary>
  public static partial class Extensions
  {
    //-------------------------------------------------------------------------------------------------------------------------
    private static SendGridClient SendGridClient
    {
      get
      {
        return new SendGridClient(SendGrid.ApiKey);
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    private static WebClient SendGridWebClient
    {
      get
      {
        WebClient webClient = new()
        {
          BaseAddress = "https://api.sendgrid.com/v3/",
          UseDefaultCredentials = false
        };

        webClient.Headers.Add("Authorization", $"Bearer {SendGrid.ApiKey}");
        webClient.Headers.Add("Content-Type", "application/json");
        webClient.Headers.Add("User-Agent", $"digishui/{Assembly.GetAssembly(typeof(Extensions)).GetName().Version}");
        webClient.Headers.Add("Accept", "application/json");

        return webClient;
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Prepares SendGrid message for sending.
    /// </summary>
    /// <param name="sendGridMessage">The SendGrid message to prepare.</param>
    /// <param name="addDefaultBcc">If true, adds the default bcc address from application configuration setting (Digishui.SendGridDefaultBCC)</param>
    /// <param name="renderPlainTextOnlyAlsoAsPreformattedHtml">If true and message only has plain text content, adds html content equal to plain text content, wrapped in pre tags to preserve fixed-width font rendering in most mail clients.</param>
    private static void Prepare(this SendGridMessage sendGridMessage, bool addDefaultBcc = false, bool renderPlainTextOnlyAlsoAsPreformattedHtml = true)
    {
      if (sendGridMessage.From == null)
      {
        sendGridMessage.From = new EmailAddress(SendGrid.DefaultFromAddress, SendGrid.DefaultFromName);
      }

      if (addDefaultBcc == true)
      {
        sendGridMessage.AddBcc(SendGrid.DefaultBcc);
      }

      //Sendgrid does not like non-ascii characters in plain text. This isn't an exhaustive solution to the issue but covers some common errors.
      //sendGridMessage.PlainTextContent = sendGridMessage.PlainTextContent
      //  .Replace("‘", "'")
      //  .Replace("’", "'")
      //  .Replace("“", "\"")
      //  .Replace("”", "\"");

      //If plain text content is supplied but html content is not supplied, set htmlcontent to same as plain text content
      //wrapped in <pre> tags. This ensures fixed width font rendering across most mail clients, which is preferable since
      //plain text content is composed with that assumption. Also, if only plain text content is supplied, disable click
      //tracking and open tracking because these features mangle the plain text composition, and SendGrid doesn't apply
      //the same mangling to the html composition when wrapped in <pre> tags.
      if ((sendGridMessage.PlainTextContent != null) && (sendGridMessage.HtmlContent == null) && (renderPlainTextOnlyAlsoAsPreformattedHtml == true))
      {
        sendGridMessage.HtmlContent = $"<pre style=\"white-space: pre-wrap; white-space: -moz-pre-wrap; white-space: -pre-wrap; white-space: -o-pre-wrap; white-space: break-word; font-size: 10pt\">{WebUtility.HtmlEncode(sendGridMessage.PlainTextContent)}</pre>";

        sendGridMessage.SetClickTracking(false, false);
        sendGridMessage.SetOpenTracking(false, null);
      }

      //Sending a message through the SendGrid API fails if you have the same email address listed as a recipient more than
      //once across the To, CC, and BCC fields. The following code deduplicates addresses, with the most consequential instance
      //of the address being preserved.

      List<string> recipients = [];

      for (int i = (sendGridMessage.Personalizations[0]?.Tos?.Count ?? 0) - 1; i >= 0; i--)
      {
        EmailAddress emailAddress = sendGridMessage.Personalizations[0].Tos[i];

        if (recipients.Contains(emailAddress.Email.Trim().ToLower()) == false)
        {
          recipients.Add(emailAddress.Email.Trim().ToLower());
        }
        else
        {
          sendGridMessage.Personalizations[0].Tos.RemoveAt(i);
        }
      }

      for (int i = (sendGridMessage.Personalizations[0]?.Ccs?.Count ?? 0) - 1; i >= 0; i--)
      {
        EmailAddress emailAddress = sendGridMessage.Personalizations[0].Ccs[i];

        if (recipients.Contains(emailAddress.Email.Trim().ToLower()) == false)
        {
          recipients.Add(emailAddress.Email.Trim().ToLower());
        }
        else
        {
          sendGridMessage.Personalizations[0].Ccs.RemoveAt(i);
        }
      }

      if ((sendGridMessage.Personalizations[0]?.Ccs?.Count ?? 0) == 0) { sendGridMessage.Personalizations[0].Ccs = null; }

      for (int i = (sendGridMessage.Personalizations[0]?.Bccs?.Count ?? 0) - 1; i >= 0; i--)
      {
        EmailAddress emailAddress = sendGridMessage.Personalizations[0].Bccs[i];

        if (recipients.Contains(emailAddress.Email.Trim().ToLower()) == false)
        {
          recipients.Add(emailAddress.Email.Trim().ToLower());
        }
        else
        {
          sendGridMessage.Personalizations[0].Bccs.RemoveAt(i);
        }
      }

      if ((sendGridMessage.Personalizations[0]?.Bccs?.Count ?? 0) == 0) { sendGridMessage.Personalizations[0].Bccs = null; }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    private static void EmbedImages(this SendGridMessage sendGridMessage, string templatePath, string imageExtension, string imageMimeType)
    {
      string[] imageFiles = Directory.GetFiles(templatePath, $"*.{imageExtension}");

      foreach (string imageFile in imageFiles)
      {
        if (Path.GetFileName(imageFile).StartsWith('.') == false)
        {
          if (sendGridMessage.HtmlContent.Contains(Path.GetFileName(imageFile)) == true)
          {
            byte[] fileBytes = File.ReadAllBytes(imageFile);
            string base64FileContent = Convert.ToBase64String(fileBytes);
            sendGridMessage.AddAttachment(Path.GetFileName(imageFile), base64FileContent, imageMimeType, "inline", Path.GetFileName(imageFile));
          }
        }
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static void LoadTemplate(this SendGridMessage sendGridMessage, string templateName)
    {
      string templatePath = $"{System.AppDomain.CurrentDomain.BaseDirectory}\\EmailTemplates\\{templateName}";

      if (File.Exists(Path.Combine(templatePath, "body.html")) == true)
      {
        sendGridMessage.HtmlContent = File.ReadAllText(Path.Combine(templatePath, "body.html"));
        sendGridMessage.HtmlContent = sendGridMessage.HtmlContent.Replace("<img src=\"", "<img src=\"cid:");

        sendGridMessage.EmbedImages(templatePath, "gif", "image/gif");
        sendGridMessage.EmbedImages(templatePath, "jpg", "image/jpeg");
        sendGridMessage.EmbedImages(templatePath, "png", "image/png");
      }

      if (File.Exists(Path.Combine(templatePath, "body.txt")) == true)
      {
        sendGridMessage.PlainTextContent = File.ReadAllText(Path.Combine(templatePath, "body.txt")); ;
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static void AddAttachment(this SendGridMessage sendGridMessage, string content, string filename, string mimeType)
    {
      string base64FileContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));

      sendGridMessage.AddAttachment(Path.GetFileName(filename), base64FileContent, mimeType);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static void AddAttachment(this SendGridMessage sendGridMessage, Stream contentStream, string filename, string mimeType)
    {
      byte[] fileBytes = new byte[contentStream.Length];
      contentStream.ReadExactly(fileBytes);

      string base64FileContent = Convert.ToBase64String(fileBytes);

      sendGridMessage.AddAttachment(Path.GetFileName(filename), base64FileContent, mimeType);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static void SetTemplateVariable(this SendGridMessage sendGridMessage, string variableName, string value)
    {
      if (sendGridMessage.HtmlContent != null)
      {
        sendGridMessage.HtmlContent = sendGridMessage.HtmlContent.Replace($"{{[{variableName}]}}", value);
      }

      if (sendGridMessage.PlainTextContent != null)
      {
        sendGridMessage.PlainTextContent = sendGridMessage.PlainTextContent.Replace($"{{[{variableName}]}}", value);
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Sends SendGrid message asynchronously.
    /// </summary>
    /// <param name="sendGridMessage">The SendGrid message to send.</param>
    /// <param name="addDefaultBcc">If true, adds the default bcc address from application configuration setting (Digishui.SendGridDefaultBCC)</param>
    /// <param name="renderPlainTextOnlyAlsoAsPreformattedHtml">If true and message only has plain text content, adds html content equal to plain text content, wrapped in pre tags to preserve fixed-width font rendering in most mail clients.</param>
    /// <returns></returns>
    public static async Task<bool> SendAsync(this SendGridMessage sendGridMessage, bool addDefaultBcc = false, bool renderPlainTextOnlyAlsoAsPreformattedHtml = true)
    {
      sendGridMessage.Prepare(addDefaultBcc, renderPlainTextOnlyAlsoAsPreformattedHtml);

      try
      {
        Response response = await SendGridClient.SendEmailAsync(sendGridMessage);
      }
      catch (Exception exception)
      {
        System.Diagnostics.Debug.Print(exception.Message);

        return false;
      }

      return true;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Sends SendGrid message synchronously.
    /// </summary>
    /// <param name="sendGridMessage">The SendGrid message to send.</param>
    /// <param name="addDefaultBcc">If true, adds the default bcc address from application configuration setting (Digishui.SendGridDefaultBCC)</param>
    /// <param name="renderPlainTextOnlyAlsoAsPreformattedHtml">If true and message only has plain text content, adds html content equal to plain text content, wrapped in pre tags to preserve fixed-width font rendering in most mail clients.</param>
    /// <returns></returns>
    public static bool Send(this SendGridMessage sendGridMessage, bool addDefaultBcc = false, bool renderPlainTextOnlyAlsoAsPreformattedHtml = true)
    {
      sendGridMessage.Prepare(addDefaultBcc, renderPlainTextOnlyAlsoAsPreformattedHtml);

      try
      {
        System.Diagnostics.Debug.Print(SendGridWebClient.UploadString("mail/send", sendGridMessage.Serialize()));
      }
      catch (Exception exception)
      {
        System.Diagnostics.Debug.Print(exception.Message);

        return false;
      }

      return true;
    }
  }
}
