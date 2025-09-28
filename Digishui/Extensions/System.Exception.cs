//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui.Extensions
{
  //===========================================================================================================================
  /// <summary>
  ///   System.Exception Extensions
  /// </summary>
  public static partial class Extensions
  {
    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string GetFullExceptionMessage(this Exception exception)
    {
      if (exception == null) { return "N/A"; }

      string exceptionMessage = "";

      if (exception.InnerException != null) { exceptionMessage = "\n\n" + GetFullExceptionMessage(exception.InnerException); }

      exceptionMessage = exception.Message + exceptionMessage;

      return exceptionMessage;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string GetFullStackTrace(this Exception exception)
    {
      if (exception == null) { return "N/A"; }

      string stackTrace = "";

      if (exception.InnerException != null) { stackTrace = "\n\n" + GetFullStackTrace(exception.InnerException); }

      stackTrace = exception.StackTrace + stackTrace;

      return stackTrace;
    }
  }
}