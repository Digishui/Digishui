using Digishui;

namespace Digishui.Test
{
  [TestClass]
  public sealed class Base32GuidTests
  {
    const string SampleGuidString = "05951ef7-5244-4a35-9fc5-f8a564d9eaa5";
    static readonly Guid SampleGuid = new(SampleGuidString);
    const string SampleBase32GuidString = "64PJKBKEKI2UVH6F7CSWJWPKUU";

    const string OversizedBase32String = "64PJKBKEKI2UVH6F7CSWJWPKUU2NDKNSLBTS3UZP6N5YSSFSHWGM";
    const string InvalidBase32GuidString = "THIS26CHARACTERSTRINGISBAD";

    [TestMethod]
    public void DecodeSampleBase32GuidString()
    {
      Guid guid = Base32Guid.Decode(SampleBase32GuidString);

      Assert.AreEqual(SampleGuid, guid);
    }

    [TestMethod]
    public void DecodeOversizedBase32String()
    {
      Assert.ThrowsException<ArgumentException>(() => Base32Guid.Decode(OversizedBase32String));
    }

    [TestMethod]
    public void DecodeInvalidBase32GuidString()
    {
      Assert.ThrowsException<FormatException>(() => Base32Guid.Decode(InvalidBase32GuidString));
    }

    [TestMethod]
    public void EncodeSampleGuid()
    {
      string base32GuidString = Base32Guid.Encode(SampleGuid);

      Assert.AreEqual(SampleBase32GuidString, base32GuidString);
    }

    [TestMethod]
    public void EncodeSampleGuidString()
    {
      string base32GuidString = Base32Guid.Encode(SampleGuidString);

      Assert.AreEqual(SampleBase32GuidString, base32GuidString);
    }
  }
}
