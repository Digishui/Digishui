using System.Diagnostics;
using System.Text;

//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui
{
  //===========================================================================================================================
  /// <summary>
  ///   Allows for easy writing of stream data to debug output.
  /// </summary>
  /// <remarks>
  ///   https://stackoverflow.com/questions/2779746/is-there-a-textwriter-interface-to-the-system-diagnostics-debug-class
  /// </remarks>
  public class DebugTextWriter : StreamWriter
  {
    public DebugTextWriter()
        : base(new DebugOutStream(), Encoding.Unicode, 1024)
    {
      this.AutoFlush = true;
    }

    class DebugOutStream : Stream
    {
      public override void Write(byte[] buffer, int offset, int count)
      {
        Debug.Write(Encoding.Unicode.GetString(buffer, offset, count));
      }

      public override bool CanRead { get { return false; } }
      public override bool CanSeek { get { return false; } }
      public override bool CanWrite { get { return true; } }
      public override void Flush() { Debug.Flush(); }
      public override long Length { get { throw new InvalidOperationException(); } }
      public override int Read(byte[] buffer, int offset, int count) { throw new InvalidOperationException(); }
      public override long Seek(long offset, SeekOrigin origin) { throw new InvalidOperationException(); }
      public override void SetLength(long value) { throw new InvalidOperationException(); }
      public override long Position
      {
        get { throw new InvalidOperationException(); }
        set { throw new InvalidOperationException(); }
      }
    };
  }
}