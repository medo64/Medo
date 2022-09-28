/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-09-27: Moved to Medo.IO.Hashing
//            Inheriting from NonCryptographicHashAlgorithm
//2021-11-25: Refactored to use pattern matching
//2021-10-31: Fixed reuse for new hash
//2021-03-04: Class is sealed
//2021-02-28: Refactored for .NET 5
//2017-08-26: Initial version

namespace Medo.IO.Hashing;

using System;
using System.IO.Hashing;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// Computes checksum using Damm's algorithm from numerical input.
/// This algorithm allows detection of all single-digit errors and all adjacent transposition errors.
/// Additionally, it will detect all phonetic errors associated with the English language (e.g. 13 to 30, 14 to 40, etc.)
/// Only numbers 0 to 9 are supported.
/// </summary>
/// <remarks>
/// https://en.wikipedia.org/wiki/Damm_algorithm
/// </remarks>
/// <example>
/// <code>
/// using var checksum = new Damm();
/// checksum.Append(Encoding.ASCII.GetBytes("42"));
/// var checksumChar = checksum.HashAsChar;
/// </code>
/// </example>
public sealed class Damm : NonCryptographicHashAlgorithm {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public Damm()
        : base(1) {
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

        var checksum = new Damm();
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
        var checksum = new Damm();
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

    private static readonly int[,] AntisymmetricQuasigroup = new int[10, 10] {
            {0, 3, 1, 7, 5, 9, 8, 6, 4, 2},
            {7, 0, 9, 2, 1, 5, 4, 8, 6, 3},
            {4, 2, 0, 6, 8, 7, 1, 3, 5, 9},
            {1, 7, 5, 0, 9, 8, 3, 4, 2, 6},
            {6, 1, 2, 3, 0, 4, 5, 9, 7, 8},
            {3, 6, 7, 4, 2, 0, 9, 5, 8, 1},
            {5, 8, 6, 9, 7, 2, 0, 1, 3, 4},
            {8, 9, 4, 5, 3, 6, 2, 0, 1, 7},
            {9, 4, 3, 8, 6, 1, 7, 2, 0, 5},
            {2, 5, 8, 1, 4, 3, 6, 7, 9, 0},
        };

    private void ProcessInitialization() {
        HashAsNumber = 0;
    }

    private void ProcessBytes(ReadOnlySpan<byte> source) {
        var savedHash = HashAsNumber;
        foreach (var b in source) {
            if (b is < 0x30 or > 0x39) {
                HashAsNumber = savedHash;  // restore old hash value
                throw new ArgumentOutOfRangeException(nameof(source), "Only numbers 0 to 9 are allowed.");
            }
            var row = HashAsNumber;
            var col = b - 0x30;
            HashAsNumber = AntisymmetricQuasigroup[row, col];
        }
    }

    /// <summary>
    /// Gets hash as number between 0 and 9.
    /// </summary>
    public int HashAsNumber { get; private set; }

    /// <summary>
    /// Gets hash as char.
    /// </summary>
    public char HashAsChar => (char)(0x30 + HashAsNumber);

    #endregion Algorithm

}
