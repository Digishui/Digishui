/*
 * Derived from https://github.com/csharpvitamins/CSharpVitamins.ShortGuid
 *
 * MIT License
 *
 * Copyright (c) 2007 Dave Transom
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
 * Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
 * WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Diagnostics;

//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui
{
  //===========================================================================================================================
  /// <summary>
  ///   Represents a 22-character URL-safe Base64-encoded globally unique identifier (GUID)
  /// </summary>
  /// <remarks>
  ///   https://github.com/csharpvitamins/CSharpVitamins.ShortGuid
  ///   https://datatracker.ietf.org/doc/html/rfc4648
  /// </remarks>
  [DebuggerDisplay("{Value}")]
  public readonly struct Base64Guid
  {
    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// A read-only instance of the Base64Guid struct whose value is guaranteed to be all zeroes i.e. equivalent
    /// to <see cref="Guid.Empty"/>.
    /// </summary>
    public static readonly Base64Guid Empty = new(Guid.Empty);

    //-------------------------------------------------------------------------------------------------------------------------
    readonly Guid underlyingGuid;
    readonly string encodedString;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance with the given URL-safe Base64 encoded string.
    /// <para>See also <seealso cref="Base64Guid.TryParse(string, out Base64Guid)"/> which will try to coerce the
    /// the value from URL-safe Base64 or normal Guid string.</para>
    /// </summary>
    /// <param name="value">A 22 character URL-safe Base64 encoded string to decode.</param>
    public Base64Guid(string value)
    {
      encodedString = value;
      underlyingGuid = Decode(value);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance with the given <see cref="System.Guid"/>.
    /// </summary>
    /// <param name="guid">The <see cref="System.Guid"/> to encode.</param>
    public Base64Guid(Guid guid)
    {
      encodedString = Encode(guid);
      underlyingGuid = guid;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the underlying <see cref="System.Guid"/> for the encoded Base64Guid.
    /// </summary>
    public Guid Guid => underlyingGuid;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the encoded string value of the <see cref="Guid"/> as a URL-safe Base64 string.
    /// </summary>
    public string Value => encodedString;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the encoded URL-safe Base64 string.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => encodedString;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns a value indicating whether this instance and a specified object represent the same type and value.
    /// <para>Compares for equality against other string, Guid and Base64Guid types.</para>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      if (obj is Base64Guid base64Guid)
      {
        return underlyingGuid.Equals(base64Guid.underlyingGuid);
      }

      if (obj is Guid guid)
      {
        return underlyingGuid.Equals(guid);
      }

      if (obj is string str)
      {
        // Try a Base64Guid string.
        if (TryDecode(str, out guid))
          return underlyingGuid.Equals(guid);

        // Try a guid string.
        if (Guid.TryParse(str, out guid))
          return underlyingGuid.Equals(guid);
      }

      return false;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the hash code for the underlying <see cref="System.Guid"/>.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => underlyingGuid.GetHashCode();

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Initialises a new instance of a Base64Guid using <see cref="Guid.NewGuid()"/>.
    /// <para>Equivalent of calling: <code>`new Base64Guid(Guid.NewGuid())`</code></para>
    /// </summary>
    /// <returns></returns>
    public static Base64Guid NewGuid() => new(Guid.NewGuid());

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Encodes the given value as an encoded Base64Guid string. The encoding is similar to Base64, with
    /// some non-URL safe characters replaced, and padding removed, resulting in a 22 character string.
    /// </summary>
    /// <param name="value">Any valid <see cref="System.Guid"/> string.</param>
    /// <returns>A 22 character Base64Guid URL-safe Base64 string.</returns>
    public static string Encode(string value)
    {
      Guid guid = new(value);
      return Encode(guid);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Encodes the given <see cref="System.Guid"/> as an encoded Base64Guid string. The encoding is similar to Base64, with 
    /// some non-URL safe characters replaced, and padding removed, resulting in a 22 character string.
    /// </summary>
    /// <param name="guid">The <see cref="System.Guid"/> to encode.</param>
    /// <returns>A 22 character Base64Guid URL-safe Base64 string.</returns>
    public static string Encode(Guid guid)
    {
      string encoded = Convert.ToBase64String(guid.ToByteArray());

      encoded = encoded
        .Replace("/", "_")
        .Replace("+", "-");

      return encoded[..22];
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Decodes the given value from a 22 character URL-safe Base64 string to a <see cref="System.Guid"/>.
    /// <para>Supports: Base64Guid format only.</para>
    /// <para>See also <seealso cref="TryDecode(string, out Guid)"/> or <seealso cref="TryParse(string, out Guid)"/>.</para>
    /// </summary>
    /// <param name="value">A 22 character URL-safe Base64 encoded string to decode.</param>
    /// <returns>A new <see cref="System.Guid"/> instance from the parsed string.</returns>
    /// <exception cref="FormatException">
    /// If <paramref name="value"/> is not a valid Base64 string (<seealso cref="Convert.FromBase64String(string)"/>)
    /// or if the decoded guid doesn't strictly match the input <paramref name="value"/>.
    /// </exception>
    public static Guid Decode(string value)
    {
      // avoid parsing larger strings/blobs
      if (value?.Length != 22)
      {
        throw new ArgumentException(
            $"A Base64Guid must be exactly 22 characters long. Received a {value?.Length ?? 0} character string.",
            paramName: nameof(value)
        );
      }

      string base64value = value
        .Replace("_", "/")
        .Replace("-", "+") + "==";

      Guid guid = new(Convert.FromBase64String(base64value));

      string sanityCheck = Encode(guid);

      if (sanityCheck != value) { throw new FormatException($"Invalid strict Base64Guid encoded string. The string '{value}' is valid URL-safe Base64, but failed a round-trip test expecting '{sanityCheck}'."); }

      return guid;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// <para>Supports Base64Guid format only.</para>
    /// <para>Attempts to decode the given value from a 22 character URL-safe Base64 string to
    /// a <see cref="System.Guid"/>.</para>
    /// <para>The difference between TryParse and TryDecode:</para>
    /// <list type="number">
    ///     <item>
    ///         <term><see cref="TryParse(string, out Base64Guid)"/></term>
    ///         <description>Supports: Guid &amp; Base64Guid;</description>
    ///         <description>Tries to parse first as a <see cref="Base64Guid"/>, then as a
    ///         <see cref="System.Guid"/>, outputs the <see cref="Base64Guid"/> instance.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="TryParse(string, out Guid)"/></term>
    ///         <description>Supports: Guid &amp; Base64Guid;</description>
    ///         <description>Tries to parse first as a <see cref="Base64Guid"/>, then as a
    ///         <see cref="System.Guid"/>, outputs the underlying <see cref="System.Guid"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="TryDecode(string, out Guid)"/></term>
    ///         <description>Supports: Base64Guid;</description>
    ///         <description>Tries to decode a 22 character URL-safe Base64 string as a
    ///         <see cref="Base64Guid"/> only, but outputs the result as a <see cref="System.Guid"/> - this method.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="value">The Base64Guid encoded string to decode.</param>
    /// <param name="guid">A new <see cref="System.Guid"/> instance from the parsed string.</param>
    /// <returns>A boolean indicating if the decode was successful.</returns>
    public static bool TryDecode(string value, out Guid guid)
    {
      try
      {
        guid = Decode(value);
        return true;
      }
      catch
      {
        guid = Guid.Empty;
        return false;
      }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// <para>Supports Base64Guid &amp; Guid formats.</para>
    /// <para>Tries to parse the value from either a 22 character URL-safe Base64 string or
    /// a <see cref="System.Guid"/> string, and outputs a <see cref="Base64Guid"/> instance.</para>
    /// <para>The difference between TryParse and TryDecode:</para>
    /// <list type="number">
    ///     <item>
    ///         <term><see cref="TryParse(string, out Base64Guid)"/></term>
    ///         <description>Supports: Guid &amp; Base64Guid; </description>
    ///         <description>Tries to parse first as a <see cref="Base64Guid"/>, then as a
    ///         <see cref="System.Guid"/>, outputs the <see cref="Base64Guid"/> instance - this method.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="TryParse(string, out Guid)"/></term>
    ///         <description>Supports: Guid &amp; Base64Guid;</description>
    ///         <description>Tries to parse first as a <see cref="Base64Guid"/>, then as a
    ///         <see cref="System.Guid"/>, outputs the <see cref="System.Guid"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="TryDecode(string, out Guid)"/></term>
    ///         <description>Supports: Base64Guid;</description>
    ///         <description>Tries to decode a 22 character URL-safe Base64 string as a
    ///         <see cref="Base64Guid"/> only, but outputs the result as a <see cref="System.Guid"/>.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="value">The Base64Guid encoded string or string representation of a Guid.</param>
    /// <param name="Base64Guid">A new <see cref="Base64Guid"/> instance from the parsed string.</param>
    public static bool TryParse(string value, out Base64Guid base64Guid)
    {
      // Try a Base64Guid string.
      if (Base64Guid.TryDecode(value, out Guid guid))
      {
        base64Guid = guid;

        return true;
      }

      // Try a Guid string.
      if (Guid.TryParse(value, out guid))
      {
        base64Guid = guid;
        return true;
      }

      base64Guid = Base64Guid.Empty;
      return false;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// <para>Supports Base64Guid &amp; Guid formats.</para>
    /// <para>Tries to parse the value either a 22 character URL-safe Base64 string or
    /// <see cref="System.Guid"/> string, and outputs the <see cref="Guid"/> value.</para>
    /// <para>The difference between TryParse and TryDecode:</para>
    /// <list type="number">
    ///     <item>
    ///         <term><see cref="TryParse(string, out Base64Guid)"/></term>
    ///         <description>Supports: Guid &amp; Base64Guid;</description>
    ///         <description>Tries to parse first as a <see cref="Base64Guid"/>, then as a
    ///         <see cref="System.Guid"/>, outputs the <see cref="Base64Guid"/> instance.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="TryParse(string, out Guid)"/></term>
    ///         <description>Supports: Guid &amp; Base64Guid;</description>
    ///         <description>Tries to parse first as a <see cref="Base64Guid"/>, then as a
    ///         <see cref="System.Guid"/>, outputs the <see cref="System.Guid"/> - this method.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="TryDecode(string, out Guid)"/></term>
    ///         <description>Supports: Base64Guid;</description>
    ///         <description>Tries to decode a 22 character URL-safe Base64 string as a
    ///         <see cref="Base64Guid"/> only, outputting the result as a <see cref="System.Guid"/>.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="value">The Base64Guid encoded string or string representation of a Guid.</param>
    /// <param name="guid">A new <see cref="System.Guid"/> instance from the parsed string.</param>
    /// <returns>A boolean indicating if the parse was successful.</returns>
    public static bool TryParse(string value, out Guid guid)
    {
      // Try a Base64Guid string.
      if (Base64Guid.TryDecode(value, out guid))
        return true;

      // Try a Guid string.
      if (Guid.TryParse(value, out guid))
        return true;

      guid = Guid.Empty;
      return false;
    }

    #region Operators

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines if both Base64Guid instances have the same underlying <see cref="System.Guid"/> value.
    /// </summary>
    public static bool operator ==(Base64Guid x, Base64Guid y) => x.underlyingGuid == y.underlyingGuid;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines if both instances have the same underlying <see cref="System.Guid"/> value.
    /// </summary>
    public static bool operator ==(Base64Guid x, Guid y) => x.underlyingGuid == y;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines if both instances have the same underlying <see cref="System.Guid"/> value.
    /// </summary>
    public static bool operator ==(Guid x, Base64Guid y) => y == x; // NB: order of arguments

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines if both Base64Guid instances do not have the same underlying <see cref="System.Guid"/> value.
    /// </summary>
    public static bool operator !=(Base64Guid x, Base64Guid y) => !(x == y);

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines if both instances do not have the same underlying <see cref="System.Guid"/> value.
    /// </summary>
    public static bool operator !=(Base64Guid x, Guid y) => !(x == y);

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines if both instances do not have the same underlying <see cref="System.Guid"/> value.
    /// </summary>
    public static bool operator !=(Guid x, Base64Guid y) => !(x == y);

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Implicitly converts the Base64Guid to its string equivalent.
    /// </summary>
    public static implicit operator string(Base64Guid base64Guid) => base64Guid.encodedString;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Implicitly converts the Base64Guid to its <see cref="System.Guid"/> equivalent.
    /// </summary>
    public static implicit operator Guid(Base64Guid base64Guid) => base64Guid.underlyingGuid;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Implicitly converts the string to a Base64Guid.
    /// </summary>
    public static implicit operator Base64Guid(string value)
    {
      if (string.IsNullOrEmpty(value)) { return Base64Guid.Empty; }

      if (TryParse(value, out Base64Guid base64Guid)) { return base64Guid; }

      throw new FormatException("Base64Guid should contain 22 Base64 characters or Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).");
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Implicitly converts the <see cref="System.Guid"/> to a Base64Guid.
    /// </summary>
    public static implicit operator Base64Guid(Guid guid)
    {
      if (guid == Guid.Empty) { return Base64Guid.Empty; }

      return new Base64Guid(guid);
    }

    #endregion
  }
}
