/*
 * Derived from https://github.com/google/google-authenticator-android/blob/master/AuthenticatorApp/src/main/java/com/google/android/apps/authenticator/Base32String.java
 * 
 * Copyright (C) 2016 BravoTango86
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Text;

//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui
{
  //===========================================================================================================================
  // https://datatracker.ietf.org/doc/html/rfc4648
  public static class Base32Util
  {
    //-------------------------------------------------------------------------------------------------------------------------
    private static readonly char[] _digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray(); // RFC 4648 Base32
    private const int _mask = 31;
    private const int _shift = 5;  // 5 bits max value is 32, i.e. we need 32 chars to encode 5 bits

    //-------------------------------------------------------------------------------------------------------------------------
    private static int CharToInt(char c)
    {
      return c switch
      {
        'A' => 0,
        'B' => 1,
        'C' => 2,
        'D' => 3,
        'E' => 4,
        'F' => 5,
        'G' => 6,
        'H' => 7,
        'I' => 8,
        'J' => 9,
        'K' => 10,
        'L' => 11,
        'M' => 12,
        'N' => 13,
        'O' => 14,
        'P' => 15,
        'Q' => 16,
        'R' => 17,
        'S' => 18,
        'T' => 19,
        'U' => 20,
        'V' => 21,
        'W' => 22,
        'X' => 23,
        'Y' => 24,
        'Z' => 25,
        '2' => 26,
        '3' => 27,
        '4' => 28,
        '5' => 29,
        '6' => 30,
        '7' => 31,
        _ => -1,
      };
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static byte[] FromBase32String(string encoded)
    {
      ArgumentNullException.ThrowIfNull(encoded);

      // Remove whitespace and padding. Note: the padding is used as hint to determine how many bits to decode from the last
      // incomplete chunk. Also, canonicalize to all upper case.
      encoded = encoded.Trim().TrimEnd('=').ToUpper();

      if (encoded.Length == 0) { return []; }

      int outLength = encoded.Length * _shift / 8;
      byte[] result = new byte[outLength];
      int buffer = 0;
      int next = 0;
      int bitsLeft = 0;
      int charValue;

      foreach (char character in encoded)
      {
        charValue = CharToInt(character);

        if (charValue < 0) { throw new FormatException("Illegal character: `" + character + "`"); }

        buffer <<= _shift;
        buffer |= charValue & _mask;
        bitsLeft += _shift;

        if (bitsLeft >= 8)
        {
          result[next++] = (byte)(buffer >> (bitsLeft - 8));
          bitsLeft -= 8;
        }
      }

      return result;
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static string ToBase32String(byte[] data, bool padOutput = false)
    {
      return ToBase32String(data, 0, data.Length, padOutput);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static string ToBase32String(byte[] data, int offset, int length, bool padOutput = false)
    {
      ArgumentNullException.ThrowIfNull(data);

      ArgumentOutOfRangeException.ThrowIfNegative(offset);

      ArgumentOutOfRangeException.ThrowIfNegative(length);

      ArgumentOutOfRangeException.ThrowIfGreaterThan(data.Length, offset + length);

      if (length == 0) { return ""; }

      // SHIFT is the number of bits per output character, so the length of the output is the length of the input multiplied by
      // 8/SHIFT, rounded up. The computation below will fail, so don't do it.
      ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(length, 1 << 28);

      int outputLength = (length * 8 + _shift - 1) / _shift;
      StringBuilder result = new(outputLength);

      int last = offset + length;
      int buffer = data[offset++];
      int bitsLeft = 8;

      while (bitsLeft > 0 || offset < last)
      {
        if (bitsLeft < _shift)
        {
          if (offset < last)
          {
            buffer <<= 8;
            buffer |= (data[offset++] & 0xff);
            bitsLeft += 8;
          }
          else
          {
            int pad = _shift - bitsLeft;
            buffer <<= pad;
            bitsLeft += pad;
          }
        }

        int index = _mask & (buffer >> (bitsLeft - _shift));
        bitsLeft -= _shift;

        result.Append(_digits[index]);
      }

      if (padOutput)
      {
        int padding = 8 - (result.Length % 8);

        if (padding > 0) { result.Append('=', padding == 8 ? 0 : padding); }
      }

      return result.ToString();
    }
  }
}
