/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-09-27: Moved to Medo.IO.Hashing
//            Inheriting from NonCryptographicHashAlgorithm
//2021-11-25: Refactored to use pattern matching
//2021-10-31: Refactored for .NET 5
//2009-01-05: Initial version.

namespace Medo.IO.Hashing;

using System;
using System.IO.Hashing;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// Computes hash using ISO 7064 algorithm from numerical input.
/// </summary>
/// <example>
/// <code>
/// var checksum = new Iso7064();
/// checksum.Append(Encoding.ASCII.GetBytes("42"));
/// var checkDigit = crc.HashAsChar;
/// </code>
/// </example>
public sealed class Iso7064 : NonCryptographicHashAlgorithm {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public Iso7064()
        :base(1) {
        ProcessInitialization();
    }


    #region Helpers

    /// <summary>
    /// Returns hash characters based on digits passed to it.
    /// </summary>
    /// <param name="digits">Digits to checksum.</param>
    /// <exception cref="ArgumentNullException">Digits cannot be null.</exception>
    public static string ComputeHash(string digits) {
        return ComputeHash(digits, returnAllDigits: false);
    }

    /// <summary>
    /// Returns hash characters based on digits passed to it.
    /// </summary>
    /// <param name="digits">Digits to checksum.</param>
    /// <param name="returnAllDigits">If true, all digits are to be returned instead of only hash.</param>
    /// <exception cref="ArgumentNullException">Digits cannot be null.</exception>
    public static string ComputeHash(string digits, bool returnAllDigits) {
        if (digits == null) { throw new ArgumentNullException(nameof(digits), "Digits cannot be null."); }
        digits = digits.Replace(" ", "").Replace("-", ""); //ignore dashes and spaces

        var checksum = new Iso7064();
        checksum.Append(Encoding.ASCII.GetBytes(digits));
        if (returnAllDigits) {
            return digits + checksum.HashAsChar.ToString();
        } else {
            return checksum.HashAsChar.ToString();
        }
    }


    /// <summary>
    /// Returns if hash matches digits.
    /// </summary>
    /// <param name="digitsWithHash">Digits with appended hash.</param>
    /// <exception cref="ArgumentNullException">Digits cannot be null.</exception>
    public static bool ValidateHash(string digitsWithHash) {
        if (digitsWithHash == null) { throw new ArgumentNullException(nameof(digitsWithHash), "Digits cannot be null."); }
        digitsWithHash = digitsWithHash.Replace(" ", "").Replace("-", ""); //ignore dashes and spaces
        if (digitsWithHash.Length < 1) { throw new ArgumentOutOfRangeException(nameof(digitsWithHash), "Digits cannot be less than one character long."); }

        var digits = digitsWithHash[0..^1];
        var hash = digitsWithHash[^1];
        var checksum = new Iso7064();
        checksum.Append(Encoding.ASCII.GetBytes(digits));
        return (hash == checksum.HashAsChar);
    }

    #endregion


    #region NonCryptographicHashAlgorithm

    /// <inheritdoc/>
    public override void Append(ReadOnlySpan<byte> source) {
        ProcessBytes(source);
    }

    /// <inheritdoc/>
    public override void Reset() {
        ProcessInitialization();
    }

    protected override void GetCurrentHashCore(Span<byte> destination) {
        var hash = (byte)(0x30 + HashAsNumber);
        MemoryMarshal.Write(destination, ref hash);
    }

    #endregion NonCryptographicHashAlgorithm


    #region Algorithm

    private int DigestSum;

    private void ProcessInitialization() {
        DigestSum = 10;
    }

    private void ProcessBytes(ReadOnlySpan<byte> source) {
        int oldDigest = DigestSum;
        foreach (var b in source) {
            if (b is >= (byte)'0' and <= (byte)'9') {
                DigestSum += (b - '0');
                if (DigestSum > 10) { DigestSum -= 10; }
                DigestSum *= 2;
                if (DigestSum >= 11) { DigestSum -= 11; }
            } else {
                DigestSum = oldDigest;
                throw new ArgumentOutOfRangeException(nameof(source), "Only numeric data is supported.");
            }
        }
    }

    /// <summary>
    /// Gets hash as number between 0 and 9.
    /// </summary>
    public int HashAsNumber {
        get {
            int tmp = 11 - DigestSum;
            if (tmp == 10) {
                return 0;
            } else {
                return tmp;
            }
        }
    }

    /// <summary>
    /// Gets hash as byte.
    /// </summary>
    public byte HashAsByte => (byte)(0x30 + HashAsNumber);

    /// <summary>
    /// Gets hash as char.
    /// </summary>
    public char HashAsChar => (char)(0x30 + HashAsNumber);

    #endregion Algorithm

}
