using System.Text;

namespace Digishui
{
  //===========================================================================================================================
  /// <summary>
  ///   Simple class for logging to console with timestamps for each new line. Contents also exposed as string.
  /// </summary>
  public static class ConsoleLog
  {
    //-------------------------------------------------------------------------------------------------------------------------
    private static bool NewLine { get; set; } = true;

    //-------------------------------------------------------------------------------------------------------------------------
    private static StringBuilder Archive { get; } = new StringBuilder();

    //-------------------------------------------------------------------------------------------------------------------------
    public static string Contents
    {
      get
      {
        return Archive.ToString();
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Writes to the log without terminating the line, allowing for additional content to be written to the line.
    /// </summary>
    /// <param name="value">Value to be written to the log.</param>
    /// <param name="forceNewLineFirst">Forces termination of current unterminated line if true.</param>
    public static void Write(string value = "", bool forceNewLineFirst = false)
    {
      if ((forceNewLineFirst == true) && (NewLine == false))
      {
        Console.WriteLine();
        Archive.AppendLine();
        NewLine = true;
      }

      string logEntry = (NewLine == true) ? $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fffffff}] " : "";
      logEntry += value;

      Console.Write(logEntry);
      Archive.Append(logEntry);

      NewLine = false;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Writes to the log and terminates the line.
    /// </summary>
    /// <param name="value">Value to be written to the log.</param>
    /// <param name="forceNewLineFirst">Forces termination of current unterminated line if true.</param>
    public static void WriteLine(string value = "", bool forceNewLineFirst = false)
    {
      if ((forceNewLineFirst == true) && (NewLine == false))
      {
        Console.WriteLine();
        Archive.AppendLine();
        NewLine = true;
      }

      string logEntry = (NewLine == true) ? $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fffffff}] " : "";
      logEntry += value;

      Console.WriteLine(logEntry);
      Archive.AppendLine(logEntry);

      NewLine = true;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static void Clear()
    {
      Console.Clear();
      Archive.Clear();
    }
  }
}