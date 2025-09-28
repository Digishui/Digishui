//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui
{
  public class FormFile
  {
    public string FormFieldName { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; } = null;
    public Stream Stream { get; set; }
  }
}