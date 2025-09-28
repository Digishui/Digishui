using System.Net;
using System.Net.Mail;

//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui.Extensions
{
  //===========================================================================================================================
  /// <summary>
  ///   System.Net.Mail.MailMessage Extensions for use with SendGrid via SMTP.
  /// </summary>
  public static partial class Extensions
  {
    //-------------------------------------------------------------------------------------------------------------------------
    private static SmtpClient SmtpClient
    {
      get
      {
        SmtpClient smtpClient = new("smtp.sendgrid.net", 465)
        {
          EnableSsl = true,
          UseDefaultCredentials = false,
          Credentials = new NetworkCredential("apikey", SendGrid.ApiKey)
        };

        return smtpClient;
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    private static void Prepare(this MailMessage mailMessage, bool supportBcc = false)
    {
      mailMessage.From ??= new MailAddress(SendGrid.DefaultFromAddress, SendGrid.DefaultFromName);

      if (supportBcc == true)
      {
        mailMessage.Bcc.Add(SendGrid.DefaultBcc);
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static void AddAttachment(this MailMessage mailMessage, string content, string fileName, string mimeType)
    {
      mailMessage.Attachments.Add(new Attachment(content.ToStream(), fileName, mimeType));
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static void AddAttachment(this MailMessage mailMessage, Stream contentStream, string fileName, string mimeType)
    {
      mailMessage.Attachments.Add(new Attachment(contentStream, fileName, mimeType));
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static async Task<bool> SendAsync(this MailMessage mailMessage, bool supportBcc = false)
    {
      mailMessage.Prepare(supportBcc);

      try
      {
        await SmtpClient.SendMailAsync(mailMessage);
      }
      catch
      {
        return false;
      }

      return true;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static bool Send(this MailMessage mailMessage, bool supportBcc = false)
    {
      mailMessage.Prepare(supportBcc);

      try
      {
        SmtpClient.Send(mailMessage);
      }
      catch
      {
        return false;
      }

      return true;
    }
  }
}
