using PhoneNumbers;

//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui
{
  //===========================================================================================================================
  public class PstnUtil
  {
    //-------------------------------------------------------------------------------------------------------------------------
    private static PhoneNumberUtil PhoneNumberUtil { get; } = PhoneNumberUtil.GetInstance();

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Returns the supplied phone number formatted for display purposes.  Phone numbers in the +1 country code are returned
    ///   in AAA-PPP-NNNN format.  International phone numbers are returned in an industry-standard format including the
    ///   country code.
    /// </summary>
    /// <param name="pstn">Phone number to format for display purposes.</param>
    /// <returns>Phone number formatted for display purposes, or null if the phone number is invalid.</returns>
    public static string DisplayFormat(string pstn)
    {
      if (pstn == null) { return null; }

      pstn = pstn.Trim();

      if (PhoneNumberUtil.IsPossibleNumber(pstn, "US") == false) { return null; }

      PhoneNumber phoneNumber = PhoneNumberUtil.Parse(pstn, "US");

      if (phoneNumber.CountryCode == 1)
      {
        return PhoneNumberUtil.Format(PhoneNumberUtil.Parse(pstn, "US"), PhoneNumberFormat.NATIONAL).Replace("(", "").Replace(") ", "-");
      }
      else
      {
        return PhoneNumberUtil.Format(PhoneNumberUtil.Parse(pstn, "US"), PhoneNumberFormat.INTERNATIONAL);
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Returns the supplied phone number in E164 format for storage purposes.
    /// </summary>
    /// <param name="pstn">Phone number to format for storage purposes.</param>
    /// <returns>Phone number formatted for storage purposes, or null if the phone number is invalid.</returns>
    public static string StorageFormat(string pstn)
    {
      if (pstn == null) { return null; }

      pstn = pstn.Trim();

      if (PhoneNumberUtil.IsPossibleNumber(pstn, "US") == false) { return null; }

      return PhoneNumberUtil.Format(PhoneNumberUtil.Parse(pstn, "US"), PhoneNumberFormat.E164);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Returns the country code for the supplied pstn.
    /// </summary>
    /// <param name="pstn">PSTN for which a country code is sought.</param>
    /// <returns>Country code for the supplied PSTN.</returns>
    public static string GetCountryCode(string pstn)
    {
      pstn = StorageFormat(pstn);

      if (pstn == null) { return null; }

      PhoneNumber phoneNumber = PhoneNumberUtil.Parse(pstn, "");
      return phoneNumber.CountryCode.ToString();
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///   Returns the national number for the supplied pstn.
    /// </summary>
    /// <param name="pstn"></param>
    /// <returns></returns>
    public static string GetNationalNumber(string pstn)
    {
      pstn = StorageFormat(pstn);

      if (pstn == null) { return null; }

      return pstn.Replace($"+{GetCountryCode(pstn)}", "");
    }
  }
}
