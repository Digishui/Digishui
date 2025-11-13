using Digishui;

namespace Digishui.Test
{
  [TestClass]
  public sealed class Base64GuidTests
  {
    const string SampleGuidString = "c9a646d3-9c61-4cb7-bfcd-ee2522c8f633";
    static readonly Guid SampleGuid = new(SampleGuidString);
    const string SampleBase64GuidString = "00amyWGct0y_ze4lIsj2Mw";

    const string OversizedBase64String = "YzlhNjQ2ZDMtOWM2MS00Y2I3LWJmY2QtZWUyNTIyYzhmNjMzIGFuZCBzb21lIGV4dHJhIGNoYXJzLg";
    const string InvalidBase64GuidString = "22CharacterStringIsBad";

    [TestMethod]
    public void DecodeSampleBase64GuidString()
    {
      Guid guid = Base64Guid.Decode(SampleBase64GuidString);

      Assert.AreEqual(SampleGuid, guid);
    }

    [TestMethod]
    public void DecodeOversizedBase64String()
    {
      Assert.Throws<ArgumentException>(() => Base64Guid.Decode(OversizedBase64String));
    }

    [TestMethod]
    public void DecodeInvalidBase64GuidString()
    {
      Assert.Throws<FormatException>(() => Base64Guid.Decode(InvalidBase64GuidString));
    }

    [TestMethod]
    public void EncodeSampleGuid()
    {
      string base64GuidString = Base64Guid.Encode(SampleGuid);

      Assert.AreEqual(SampleBase64GuidString, base64GuidString);
    }

    [TestMethod]
    public void EncodeSampleGuidString()
    {
      string base64GuidString = Base64Guid.Encode(SampleGuidString);

      Assert.AreEqual(SampleBase64GuidString, base64GuidString);
    }
  }
}
