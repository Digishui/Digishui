using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui
{
  //===========================================================================================================================
  public class DigishuiController : Controller
  {
    //-------------------------------------------------------------------------------------------------------------------------
    protected static AjaxErrorsResult AjaxErrors(Dictionary<string, string> errors, int statusCode = StatusCodes.Status400BadRequest)
    {
      return new AjaxErrorsResult(errors, statusCode);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    protected static AjaxErrorsResult AjaxError(string key, string value, int statusCode = StatusCodes.Status400BadRequest)
    {
      return new AjaxErrorsResult(new Dictionary<string, string> { { key, value } }, statusCode);
    }
  }

  //===========================================================================================================================
  public class AjaxErrorsResult(Dictionary<string, string> errors, int statusCode = StatusCodes.Status400BadRequest) : ActionResult
  {
    //-------------------------------------------------------------------------------------------------------------------------
    public Dictionary<string, string> Errors { get; private set; } = errors;
    public int StatusCode { get; private set; } = statusCode;

    //-------------------------------------------------------------------------------------------------------------------------
    public override void ExecuteResult(ActionContext context)
    {
      ArgumentNullException.ThrowIfNull(context);

      HttpContext httpContext = context.HttpContext;
      HttpResponse httpResponse = context.HttpContext.Response;

      var statusCodePagesFeature = httpContext.Features.Get<IStatusCodePagesFeature>();
      if (statusCodePagesFeature is not null) { statusCodePagesFeature.Enabled = false; }

      httpResponse.StatusCode = StatusCode;

      if ((Errors?.Count ?? 0) != 0)
      {
        httpResponse.ContentType = "application/json";

        IHttpBodyControlFeature httpBodyControlFeature = httpContext.Features.Get<IHttpBodyControlFeature>();
        if (httpBodyControlFeature != null) { httpBodyControlFeature.AllowSynchronousIO = true; }

        byte[] responseData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Errors));
        httpResponse.Body.Write(responseData, 0, responseData.Length);
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public override async Task ExecuteResultAsync(ActionContext context)
    {
      ArgumentNullException.ThrowIfNull(context);

      HttpContext httpContext = context.HttpContext;
      HttpResponse httpResponse = context.HttpContext.Response;

      var statusCodePagesFeature = httpContext.Features.Get<IStatusCodePagesFeature>();
      if (statusCodePagesFeature is not null) { statusCodePagesFeature.Enabled = false; }

      httpResponse.StatusCode = StatusCode;

      if ((Errors?.Count ?? 0) != 0)
      {
        await httpResponse.WriteAsJsonAsync(Errors);
      }
    }
  }
}