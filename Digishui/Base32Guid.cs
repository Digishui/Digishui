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
  ///   Represents a 26-character Base32-encoded globally unique identifier (GUID)
  /// </summary>
  /// <remarks>
  ///   https://github.com/csharpvitamins/CSharpVitamins.ShortGuid
  /// </remarks>
  [DebuggerDisplay("{Value}")]
  public readonly struct Base32Guid
  {
    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// A read-only instance of the Base32Guid struct whose value is guaranteed to be all zeroes i.e. equivalent
    /// to <see cref="Guid.Empty"/>.
    /// </summary>
    public static readonly Base32Guid Empty = new(Guid.Empty);

    //-------------------------------------------------------------------------------------------------------------------------
    readonly Guid underlyingGuid;
    readonly string encodedString;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance with the given Base32 encoded string.
    /// <para>See also <seealso cref="Base32Guid.TryParse(string, out Base32Guid)"/> which will try to coerce the
    /// the value from Base32 or normal Guid string.</para>
    /// </summary>
    /// <param name="value">A 26 character Base32 encoded string to decode.</param>
    public Base32Guid(string value)
    {
      encodedString = value;
      underlyingGuid = Decode(value);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance with the given <see cref="System.Guid"/>.
    /// </summary>
    /// <param name="guid">The <see cref="System.Guid"/> to encode.</param>
    public Base32Guid(Guid guid)
    {
      encodedString = Encode(guid);
      underlyingGuid = guid;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the underlying <see cref="System.Guid"/> for the encoded Base32Guid.
    /// </summary>
    public Guid Guid => underlyingGuid;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the encoded string value of the <see cref="Guid"/> as a Base32 string.
    /// </summary>
    public string Value => encodedString;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the encoded Base32 string.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => encodedString;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns a value indicating whether this instance and a specified object represent the same type and value.
    /// <para>Compares for equality against other string, Guid and Base32Guid types.</para>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      if (obj is Base32Guid base32Guid)
      {
        return underlyingGuid.Equals(base32Guid.underlyingGuid);
      }

      if (obj is Guid guid)
      {
        return underlyingGuid.Equals(guid);
      }

      if (obj is string str)
      {
        // Try a Base32Guid string.
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
    /// Initialises a new instance of a Base32Guid using <see cref="Guid.NewGuid()"/>.
    /// <para>Equivalent of calling: <code>`new Base32Guid(Guid.NewGuid())`</code></para>
    /// </summary>
    /// <returns></returns>
    public static Base32Guid NewGuid() => new(Guid.NewGuid());

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Encodes the given value as an encoded Base32Guid string. The encoding is Base32, 
    /// resulting in a 26 character string.
    /// </summary>
    /// <param name="value">Any valid <see cref="System.Guid"/> string.</param>
    /// <returns>A 26 character Base32Guid Base32 string.</returns>
    public static string Encode(string value)
    {
      Guid guid = new(value);
      return Encode(guid);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Encodes the given <see cref="System.Guid"/> as an encoded Base32Guid string. The encoding is Base32, 
    /// resulting in a 26 character string.
    /// </summary>
    /// <param name="guid">The <see cref="System.Guid"/> to encode.</param>
    /// <returns>A 26 character Base32Guid Base32 string.</returns>
    public static string Encode(Guid guid)
    {
      string encoded = Base32Util.ToBase32String(guid.ToByteArray());





      return encoded[..26];
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Decodes the given value from a 26 character Base32 string to a <see cref="System.Guid"/>.
    /// <para>Supports: Base32Guid format only.</para>
    /// <para>See also <seealso cref="TryDecode(string, out Guid)"/> or <seealso cref="TryParse(string, out Guid)"/>.</para>
    /// </summary>
    /// <param name="value">A 26 character Base32 encoded string to decode.</param>
    /// <returns>A new <see cref="System.Guid"/> instance from the parsed string.</returns>
    /// <exception cref="FormatException">
    /// If <paramref name="value"/> is not a valid Base32 string (<seealso cref="Base32Util.FromBase32String(string)"/>)
    /// or if the decoded guid doesn't strictly match the input <paramref name="value"/>.
    /// </exception>
    public static Guid Decode(string value)
    {
      // avoid parsing larger strings/blobs
      if (value?.Length != 26)
      {
        throw new ArgumentException(
            $"A Base32Guid must be exactly 26 characters long. Received a {value?.Length ?? 0} character string.",
            paramName: nameof(value)
        );
      }





      Guid guid = new(Base32Util.FromBase32String(value));

      string sanityCheck = Encode(guid);

      if (sanityCheck != value) { throw new FormatException($"Invalid strict Base32Guid encoded string. The string '{value}' is valid Base32, but failed a round-trip test expecting '{sanityCheck}'."); }

      return guid;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// <para>Supports Base32Guid format only.</para>
    /// <para>Attempts to decode the given value from a 26 character Base32 string to 
    /// a <see cref="System.Guid"/>.</para>
    /// <para>The difference between TryParse and TryDecode:</para>
    /// <list type="number">
    ///     <item>
    ///         <term><see cref="TryParse(string, out Base32Guid)"/></term>
    ///         <description>Supports: Guid &amp; Base32Guid;</description>
    ///         <description>Tries to parse first as a <see cref="Base32Guid"/>, then as a
    ///         <see cref="System.Guid"/>, outputs the <see cref="Base32Guid"/> instance.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="TryParse(string, out Guid)"/></term>
    ///         <description>Supports: Guid &amp; Base32Guid;</description>
    ///         <description>Tries to parse first as a <see cref="Base32Guid"/>, then as a
    ///         <see cref="System.Guid"/>, outputs the underlying <see cref="System.Guid"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="TryDecode(string, out Guid)"/></term>
    ///         <description>Supports: Base32Guid;</description>
    ///         <description>Tries to decode a 26 character Base32 string as a
    ///         <see cref="Base32Guid"/> only, but outputs the result as a <see cref="System.Guid"/> - this method.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="value">The Base32Guid encoded string to decode.</param>
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
    /// <para>Supports Base32Guid &amp; Guid formats.</para>
    /// <para>Tries to parse the value from either a 26 character Base32 string or
    /// a <see cref="System.Guid"/> string, and outputs a <see cref="Base32Guid"/> instance.</para>
    /// <para>The difference between TryParse and TryDecode:</para>
    /// <list type="number">
    ///     <item>
    ///         <term><see cref="TryParse(string, out Base32Guid)"/></term>
    ///         <description>Supports: Guid &amp; Base32Guid; </description>
    ///         <description>Tries to parse first as a <see cref="Base32Guid"/>, then as a
    ///         <see cref="System.Guid"/>, outputs the <see cref="Base32Guid"/> instance - this method.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="TryParse(string, out Guid)"/></term>
    ///         <description>Supports: Guid &amp; Base32Guid;</description>
    ///         <description>Tries to parse first as a <see cref="Base32Guid"/>, then as a
    ///         <see cref="System.Guid"/>, outputs the <see cref="System.Guid"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="TryDecode(string, out Guid)"/></term>
    ///         <description>Supports: Base32Guid;</description>
    ///         <description>Tries to decode a 26 character Base32 string as a
    ///         <see cref="Base32Guid"/> only, but outputs the result as a <see cref="System.Guid"/>.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="value">The Base32Guid encoded string or string representation of a Guid.</param>
    /// <param name="Base32Guid">A new <see cref="Base32Guid"/> instance from the parsed string.</param>
    public static bool TryParse(string value, out Base32Guid base32Guid)
    {
      // Try a Base32Guid string.
      if (Base32Guid.TryDecode(value, out Guid guid))
      {
        base32Guid = guid;

        return true;
      }

      // Try a Guid string.
      if (Guid.TryParse(value, out guid))
      {
        base32Guid = guid;
        return true;
      }

      base32Guid = Base32Guid.Empty;
      return false;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// <para>Supports Base32Guid &amp; Guid formats.</para>
    /// <para>Tries to parse the value either a 26 character Base32 string or
    /// <see cref="System.Guid"/> string, and outputs the <see cref="Guid"/> value.</para>
    /// <para>The difference between TryParse and TryDecode:</para>
    /// <list type="number">
    ///     <item>
    ///         <term><see cref="TryParse(string, out Base32Guid)"/></term>
    ///         <description>Supports: Guid &amp; Base32Guid;</description>
    ///         <description>Tries to parse first as a <see cref="Base32Guid"/>, then as a
    ///         <see cref="System.Guid"/>, outputs the <see cref="Base32Guid"/> instance.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="TryParse(string, out Guid)"/></term>
    ///         <description>Supports: Guid &amp; Base32Guid;</description>
    ///         <description>Tries to parse first as a <see cref="Base32Guid"/>, then as a
    ///         <see cref="System.Guid"/>, outputs the <see cref="System.Guid"/> - this method.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="TryDecode(string, out Guid)"/></term>
    ///         <description>Supports: Base32Guid;</description>
    ///         <description>Tries to decode a 26 character Base32 string as a
    ///         <see cref="Base32Guid"/> only, outputting the result as a <see cref="System.Guid"/>.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="value">The Base32Guid encoded string or string representation of a Guid.</param>
    /// <param name="guid">A new <see cref="System.Guid"/> instance from the parsed string.</param>
    /// <returns>A boolean indicating if the parse was successful.</returns>
    public static bool TryParse(string value, out Guid guid)
    {
      // Try a Base32Guid string.
      if (Base32Guid.TryDecode(value, out guid))
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
    /// Determines if both Base32Guid instances have the same underlying <see cref="System.Guid"/> value.
    /// </summary>
    public static bool operator ==(Base32Guid x, Base32Guid y) => x.underlyingGuid == y.underlyingGuid;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines if both instances have the same underlying <see cref="System.Guid"/> value.
    /// </summary>
    public static bool operator ==(Base32Guid x, Guid y) => x.underlyingGuid == y;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines if both instances have the same underlying <see cref="System.Guid"/> value.
    /// </summary>
    public static bool operator ==(Guid x, Base32Guid y) => y == x; // NB: order of arguments

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines if both Base32Guid instances do not have the same underlying <see cref="System.Guid"/> value.
    /// </summary>
    public static bool operator !=(Base32Guid x, Base32Guid y) => !(x == y);

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines if both instances do not have the same underlying <see cref="System.Guid"/> value.
    /// </summary>
    public static bool operator !=(Base32Guid x, Guid y) => !(x == y);

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines if both instances do not have the same underlying <see cref="System.Guid"/> value.
    /// </summary>
    public static bool operator !=(Guid x, Base32Guid y) => !(x == y);

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Implicitly converts the Base32Guid to its string equivalent.
    /// </summary>
    public static implicit operator string(Base32Guid base32Guid) => base32Guid.encodedString;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Implicitly converts the Base32Guid to its <see cref="System.Guid"/> equivalent.
    /// </summary>
    public static implicit operator Guid(Base32Guid base32Guid) => base32Guid.underlyingGuid;

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Implicitly converts the string to a Base32Guid.
    /// </summary>
    public static implicit operator Base32Guid(string value)
    {
      if (string.IsNullOrEmpty(value)) { return Base32Guid.Empty; }

      if (TryParse(value, out Base32Guid base32Guid)) { return base32Guid; }

      throw new FormatException("Base32Guid should contain 26 Base32 characters or Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).");
    }

    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Implicitly converts the <see cref="System.Guid"/> to a Base32Guid.
    /// </summary>
    public static implicit operator Base32Guid(Guid guid)
    {
      if (guid == Guid.Empty) { return Base32Guid.Empty; }

      return new Base32Guid(guid);
    }

    #endregion
  }
}