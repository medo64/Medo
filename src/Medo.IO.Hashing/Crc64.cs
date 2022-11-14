/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-11-13: Initial version

namespace Medo.IO.Hashing;

using System;
using System.Buffers.Binary;
using System.IO.Hashing;
using System.Runtime.CompilerServices;

/// <summary>
/// Computes hash using 64-bit CRC algorithm.
/// The following CRC-64 variants are supported: ECMA-182, GO-ISO, GO-ECMA,
/// MS, REDIS, XZ, and custom definitions.
/// </summary>
/// <example>
/// <code>
/// var crc = Crc64.GetEcma182();
/// crc.Append(Encoding.ASCII.GetBytes("Test"));
/// var hashValue = crc.HashAsUInt64;
/// </code>
/// </example>
public sealed class Crc64 : NonCryptographicHashAlgorithm {

    /// <summary>
    /// Creates a new instance using the CRC-32/ISO-HDLC variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    [Obsolete("Use GetCustom instead")]
    public Crc64()
        : this(0x04C11DB7, 0xFFFFFFFF, true, true, 0xFFFFFFFF) {
    }

    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="polynomial">Polynomial value.</param>
    /// <param name="initialValue">Starting digest.</param>
    /// <param name="reflectIn">If true, input byte is in reflected (LSB first) bit order.</param>
    /// <param name="reflectOut">If true, digest is in reflected (LSB first) bit order.</param>
    /// <param name="finalXorValue">Final XOR value.</param>
    private Crc64(ulong polynomial, ulong initialValue, bool reflectIn, bool reflectOut, ulong finalXorValue)
        : base(8) {
        _polynomial = polynomial;
        _initialValue = initialValue;
        _reverseIn = reflectIn ^ BitConverter.IsLittleEndian;
        _reverseOut = reflectOut ^ BitConverter.IsLittleEndian;
        _finalXorValue = finalXorValue;
        ProcessInitialization();
    }


    /// <summary>
    /// Returns a custom CRC-32 variant.
    /// </summary>
    /// <param name="polynomial">Polynomial value.</param>
    /// <param name="initialValue">Starting digest.</param>
    /// <param name="reflectIn">If true, input byte is in reflected (LSB first) bit order.</param>
    /// <param name="reflectOut">If true, digest is in reflected (LSB first) bit order.</param>
    /// <param name="finalXorValue">Final XOR value.</param>
    /// <remarks>
    /// Name                                               Polynomial          Init                XorOut              Reflect
    /// ----------------------------------------------------------------------------------------------------------------------
    /// ECMA-182                                           0x42F0E1EBA9EA3693  0x0000000000000000  0x0000000000000000  -
    /// GO-ISO                                             0x000000000000001B  0xFFFFFFFFFFFFFFFF  0xFFFFFFFFFFFFFFFF  In/Out
    /// MS                                                 0x259C84CBA6426349  0xFFFFFFFFFFFFFFFF  0x0000000000000000  In/Out
    /// REDIS                                              0xAD93D23594C935A9  0x0000000000000000  0x0000000000000000  In/Out
    /// WE                                                 0x42F0E1EBA9EA3693  0xFFFFFFFFFFFFFFFF  0xFFFFFFFFFFFFFFFF  -
    /// XZ / GO-ECMA                                       0x42F0E1EBA9EA3693  0xFFFFFFFFFFFFFFFF  0xFFFFFFFFFFFFFFFF  In/Out
    ///
    /// See also:
    /// - https://reveng.sourceforge.io/crc-catalogue/17plus.htm
    /// - https://users.ece.cmu.edu/~koopman/crc/index.html
    /// </remarks>
    public static Crc64 GetCustom(ulong polynomial, ulong initialValue, bool reflectIn, bool reflectOut, ulong finalXorValue) {
        return new Crc64(polynomial, initialValue, reflectIn, reflectOut, finalXorValue);
    }

    /// <summary>
    /// Returns CRC-64/ECMA-182 variant.
    /// Compatible with System.IO.Hashing.Crc64.
    /// </summary>
    /// <remarks>
    public static Crc64 GetDefault() {
        return GetEcma182();
    }

    /// <summary>
    /// Returns CRC-64/ECMA-182 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x42F0E1EBA9EA3693
    /// Initial value: 0x0000000000000000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000000000000000
    /// </remarks>
    public static Crc64 GetEcma182() {
        return new Crc64(0x42F0E1EBA9EA3693, 0x0000000000000000, false, false, 0x0000000000000000);
    }

    /// <summary>
    /// Returns CRC-64/GO-ISO variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x000000000000001B
    /// Initial value: 0xFFFFFFFFFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFFFFFFFFFF
    /// </remarks>
    public static Crc64 GetGoIso() {
        return new Crc64(0x000000000000001B, 0xFFFFFFFFFFFFFFFF, true, true, 0xFFFFFFFFFFFFFFFF);
    }

    /// <summary>
    /// Returns CRC-64/MS variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x259C84CBA6426349
    /// Initial value: 0xFFFFFFFFFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000000000000000
    /// </remarks>
    public static Crc64 GetMs() {
        return new Crc64(0x259C84CBA6426349, 0xFFFFFFFFFFFFFFFF, true, true, 0x0000000000000000);
    }

    /// <summary>
    /// Returns CRC-64/REDIS variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0xAD93D23594C935A9
    /// Initial value: 0x0000000000000000
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000000000000000
    /// </remarks>
    public static Crc64 GetRedis() {
        return new Crc64(0xAD93D23594C935A9, 0x0000000000000000, true, true, 0x0000000000000000);
    }

    /// <summary>
    /// Returns CRC-64/WE variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x42F0E1EBA9EA3693
    /// Initial value: 0xFFFFFFFFFFFFFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFFFFFFFFFFFFFF
    /// </remarks>
    public static Crc64 GetWe() {
        return new Crc64(0x42F0E1EBA9EA3693, 0xFFFFFFFFFFFFFFFF, false, false, 0xFFFFFFFFFFFFFFFF);
    }

    /// <summary>
    /// Returns CRC-64/XZ variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x42F0E1EBA9EA3693
    /// Initial value: 0xFFFFFFFFFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFFFFFFFFFF
    /// </remarks>
    public static Crc64 GetXz() {
        return new Crc64(0x42F0E1EBA9EA3693, 0xFFFFFFFFFFFFFFFF, true, true, 0xFFFFFFFFFFFFFFFF);
    }

    /// <summary>
    /// Returns CRC-64/GO-ECMA variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x42F0E1EBA9EA3693
    /// Initial value: 0xFFFFFFFFFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFFFFFFFFFF
    /// </remarks>
    public static Crc64 GetGoEcma() {
        return GetXz();
    }


    #region NonCryptographicHashAlgorithm

    /// <inheritdoc/>
    public override void Append(ReadOnlySpan<byte> source) {
        ProcessBytes(source);
    }

    /// <inheritdoc/>
    public override void Reset() {
        ProcessInitialization();
    }

    /// <summary>
    /// It will return result as machine endian.
    /// </summary>
    /// <param name="destination"></param>
    protected override void GetCurrentHashCore(Span<byte> destination) {
        if (BitConverter.IsLittleEndian ^ _reverseOut) {
            BinaryPrimitives.WriteUInt64LittleEndian(destination, HashAsUInt64);
        } else {
            BinaryPrimitives.WriteUInt64BigEndian(destination, HashAsUInt64);
        }
    }

    #endregion NonCryptographicHashAlgorithm


    #region Algorithm

    private readonly ulong _polynomial;
    private readonly ulong _initialValue;
    private readonly bool _reverseIn;
    private readonly bool _reverseOut;
    private readonly ulong _finalXorValue;

    private void ProcessInitialization() {
        _currDigest = _initialValue;
        var polynomialR = BitwiseReverse(_polynomial);
        for (var i = 0; i < 256; i++) {
            var crcValue = (ulong)i;

            for (var j = 1; j <= 8; j++) {
                if ((crcValue & 1) == 1) {
                    crcValue = (crcValue >> 1) ^ polynomialR;
                } else {
                    crcValue >>= 1;
                }
            }

            _lookup[i] = crcValue;
        }
    }

    private readonly ulong[] _lookup = new ulong[256];
    private ulong _currDigest;

    private void ProcessBytes(ReadOnlySpan<byte> source) {
        foreach (var b in source) {
            if (_reverseIn) {
                _currDigest = (_currDigest >> 8) ^ _lookup[((_currDigest & 0xFF) ^ _lookupBitReverse[b])];
            } else {
                _currDigest = (_currDigest >> 8) ^ _lookup[((_currDigest & 0xFF) ^ (b))];
            }
        }
    }

    private static readonly ulong[] _lookupBitReverse = { 0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0, 0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8, 0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4, 0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4, 0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC, 0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2, 0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2, 0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA, 0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6, 0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE, 0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE, 0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1, 0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9, 0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5, 0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD, 0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3, 0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3, 0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB, 0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7, 0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong BitwiseReverse(ulong value) {
        return (_lookupBitReverse[value & 0xFF] << 56) | (_lookupBitReverse[(value >> 8) & 0xFF] << 48) | (_lookupBitReverse[(value >> 16) & 0xFF] << 40) | (_lookupBitReverse[(value >> 24) & 0xFF] << 32) | (_lookupBitReverse[(value >> 32) & 0xFF] << 24) | (_lookupBitReverse[(value >> 40) & 0xFF] << 16) | (_lookupBitReverse[(value >> 48) & 0xFF] << 8) | (_lookupBitReverse[(value >> 56) & 0xFF]);
    }

    #endregion Algorithm


    /// <summary>
    /// Gets current digest.
    /// </summary>
    public ulong HashAsUInt64 {
        get {
            if (_reverseOut) {
                return (BitwiseReverse(_currDigest) ^ _finalXorValue);
            } else {
                return (_currDigest ^ _finalXorValue);
            }
        }
    }


    #region ReciprocalPolynomial

    /// <summary>
    /// Converts polynomial to its reversed reciprocal form.
    /// </summary>
    /// <param name="polynomial">Polynomial.</param>
    public static ulong ToReversedReciprocalPolynomial(ulong polynomial) {
        return (polynomial >> 1) | 0x8000000000000000;
    }

    /// <summary>
    /// Converts polynomial from its reversed reciprocal to normal form.
    /// </summary>
    /// <param name="reversedReciprocalPolynomial">Reversed reciprocal polynomial.</param>
    public static ulong FromReversedReciprocalPolynomial(ulong reversedReciprocalPolynomial) {
        return (reversedReciprocalPolynomial << 1) | 0x01;
    }

    #endregion ReciprocalPolynomial

}
