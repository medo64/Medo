/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-11-13: Using unsigned integers for both checksum input and output
//            Adjusted output endianness to also depend on output reflection
//2022-11-11: Using machine-endianness when bytes are returned
//2022-09-27: Moved to Medo.IO.Hashing
//            Inheriting from NonCryptographicHashAlgorithm
//2022-06-24: Obsoleted default constructor in favor of GetCustom method
//2022-01-05: Added more variants
//            Fixed big-endian system operation
//2021-03-06: Refactored for .NET 5
//            Adjusted to work on big-endian platform too
//2008-06-07: Replaced ShiftRight function with right shift (>>) operator
//            Implemented bit reversal via lookup table (http://graphics.stanford.edu/~seander/bithacks.html) and inlined byte bit reversal
//            Append is not longer returning intermediate digest (performance reasons)
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2
//2008-01-05: Changed class in order to be CLS compliant
//            Fixed Posix and X-fer calcultions
//            Added resources
//2007-10-31: Moved to common
//            Added internal Bitwise class
//            Removed obsoleted constructors

namespace Medo.IO.Hashing;

using System;
using System.Buffers.Binary;
using System.IO.Hashing;
using System.Runtime.CompilerServices;

/// <summary>
/// Computes hash using 32-bit CRC algorithm.
/// The following CRC-32 variants are supported: AAL5, ADCCP, AIXM, AUTOSAR,
/// BASE91-C, BASE91-D, BZIP2, CASTAGNOLI, CD-ROM-EDC, CKSUM, DECT-B,
/// IEEE-802.3, INTERLAKEN, ISCSI, ISO-HDLC, JAMCRC, MPEG-2, PKZIP, POSIX,
/// V-42, XFER, XZ, and custom definitions.
/// </summary>
/// <example>
/// <code>
/// var crc = Crc32.GetIsoHdlc();
/// crc.Append(Encoding.ASCII.GetBytes("Test"));
/// var hashValue = crc.HashAsUInt32;
/// </code>
/// </example>
public sealed class Crc32 : NonCryptographicHashAlgorithm {

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
    public Crc32()
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
    private Crc32(uint polynomial, uint initialValue, bool reflectIn, bool reflectOut, uint finalXorValue)
        : base(4) {
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
    /// Name                                               Polynomial  Init        Xor         Reflect
    /// ----------------------------------------------------------------------------------------------
    /// AIXM                                               0x814141AB  0x00000000  0x00000000  -
    /// AUTOSAR                                            0xF4ACFB13  0xFFFFFFFF  0xFFFFFFFF  In/Out
    /// BASE91-D                                           0xA833982B  0xFFFFFFFF  0xFFFFFFFF  In/Out
    /// BZIP2 / AAL5 / DECT-B                              0x04C11DB7  0xFFFFFFFF  0xFFFFFFFF  -
    /// CD-ROM-EDC                                         0x8001801B  0x00000000  0x00000000  In/Out
    /// CKSUM / POSIX                                      0x04C11DB7  0x00000000  0xFFFFFFFF  -
    /// ISCSI / BASE91-C / CASTAGNOLI / INTERLAKEN         0x1EDC6F41  0xFFFFFFFF  0xFFFFFFFF  In/Out
    /// ISO-HDLC / ADCCP / IEEE-802.3 / V-42 / XZ / PKZIP  0x04C11DB7  0xFFFFFFFF  0xFFFFFFFF  In/Out
    /// JAMCRC                                             0x04C11DB7  0xFFFFFFFF  0x00000000  In/Out
    /// MPEG-2                                             0x04C11DB7  0xFFFFFFFF  0x00000000  -
    /// XFER                                               0x000000AF  0x00000000  0x00000000  -
    ///
    /// See also:
    /// - https://reveng.sourceforge.io/crc-catalogue/17plus.htm
    /// - https://users.ece.cmu.edu/~koopman/crc/index.html
    /// </remarks>
    public static Crc32 GetCustom(uint polynomial, uint initialValue, bool reflectIn, bool reflectOut, uint finalXorValue) {
        return new Crc32(polynomial, initialValue, reflectIn, reflectOut, finalXorValue);
    }

    /// <summary>
    /// Returns CRC-32/ISO-HDLC variant.
    /// Compatible with System.IO.Hashing.Crc32.
    /// </summary>
    public static Crc32 GetDefault() {
        return Crc32.GetIsoHdlc();
    }

    /// <summary>
    /// Returns CRC-32/AIXM variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x814141AB
    /// Initial value: 0x00000000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00000000
    /// </remarks>
    public static Crc32 GetAixm() {
        return new Crc32(0x814141AB, 0x00000000, false, false, 0x00000000);
    }

    /// <summary>
    /// Returns CRC-32/AUTOSAR variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0xF4ACFB13
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetAutosar() {
        return new Crc32(0xF4ACFB13, 0xFFFFFFFF, true, true, 0xFFFFFFFF);
    }

    /// <summary>
    /// Returns CRC-32/BASE91-D variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0xA833982B
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetBase91D() {
        return new Crc32(0xA833982B, 0xFFFFFFFF, true, true, 0xFFFFFFFF);
    }

    /// <summary>
    /// Returns CRC-32/BZIP2 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetBZip2() {
        return new Crc32(0x04C11DB7, 0xFFFFFFFF, false, false, 0xFFFFFFFF);
    }

    /// <summary>
    /// Returns CRC-32/AAL5 variant.
    /// More widely known as CRC-32/BZIP2.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetAal5() {
        return GetBZip2();
    }

    /// <summary>
    /// Returns CRC-32/DECT-B variant.
    /// More widely known as CRC-32/BZIP2.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetDectB() {
        return GetBZip2();
    }

    /// <summary>
    /// Returns CRC-32/CD-ROM-EDC variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8001801B
    /// Initial value: 0x00000000
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x00000000
    /// </remarks>
    public static Crc32 GetCdromEdc() {
        return new Crc32(0x8001801B, 0x00000000, true, true, 0x00000000);
    }

    /// <summary>
    /// Returns CRC-32/CKSUM variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0x00000000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetCksum() {
        return new Crc32(0x04C11DB7, 0x00000000, false, false, 0xFFFFFFFF);
    }

    /// <summary>
    /// Returns CRC-32/POSIX variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0x00000000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetPosix() {
        return GetCksum();
    }

    /// <summary>
    /// Returns CRC-32/ISCSI variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1EDC6F41
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetIScsi() {
        return new Crc32(0x1EDC6F41, 0xFFFFFFFF, true, true, 0xFFFFFFFF);
    }

    /// <summary>
    /// Returns CRC-32/BASE91-C variant.
    /// More widely known as CRC-32/ISCSI.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1EDC6F41
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetBase91C() {
        return GetIScsi();
    }

    /// <summary>
    /// Returns CRC-32/CASTAGNOLI variant.
    /// More widely known as CRC-32/ISCSI.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1EDC6F41
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetCastagnoli() {
        return GetIScsi();
    }

    /// <summary>
    /// Returns CRC-32/INTERLAKEN variant.
    /// More widely known as CRC-32/ISCSI.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1EDC6F41
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetInterlaken() {
        return GetIScsi();
    }

    /// <summary>
    /// Returns CRC-32/ISO-HDLC variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetIsoHdlc() {
        return new Crc32(0x04C11DB7, 0xFFFFFFFF, true, true, 0xFFFFFFFF);
    }

    /// <summary>
    /// Returns CRC-32/ADCCP variant.
    /// More widely known as CRC-32/ISO-HDLC.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetAdccp() {
        return GetIsoHdlc();
    }

    /// <summary>
    /// Returns CRC-32/IEEE-802.3 variant.
    /// More widely known as CRC-32/ISO-HDLC.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetIeee() {
        return GetIsoHdlc();
    }

    /// <summary>
    /// Returns CRC-32/IEEE-802.3 variant.
    /// More widely known as CRC-32/ISO-HDLC.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetIeee8023() {
        return GetIsoHdlc();
    }

    /// <summary>
    /// Returns CRC-32/V-42 variant.
    /// More widely known as CRC-32/ISO-HDLC.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetV42() {
        return GetIsoHdlc();
    }

    /// <summary>
    /// Returns CRC-32/XZ variant.
    /// More widely known as CRC-32/ISO-HDLC.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetXZ() {
        return GetIsoHdlc();
    }

    /// <summary>
    /// Returns CRC-32/PKZIP variant.
    /// More widely known as CRC-32/ISO-HDLC.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetPkZip() {
        return GetIsoHdlc();
    }

    /// <summary>
    /// Returns CRC-32/JAMCRC variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x00000000
    /// </remarks>
    public static Crc32 GetJamCrc() {
        return new Crc32(0x04C11DB7, 0xFFFFFFFF, true, true, 0x00000000);
    }

    /// <summary>
    /// Returns CRC-32/JAMCRC variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFFFFFF
    /// </remarks>
    public static Crc32 GetJam() {
        return GetJamCrc();
    }

    /// <summary>
    /// Returns CRC-32/MPEG-2 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x04C11DB7
    /// Initial value: 0xFFFFFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00000000
    /// </remarks>
    public static Crc32 GetMpeg2() {
        return new Crc32(0x04C11DB7, 0xFFFFFFFF, false, false, 0x00000000);
    }

    /// <summary>
    /// Returns CRC-32/XFER variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x000000AF
    /// Initial value: 0x00000000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00000000
    /// </remarks>
    public static Crc32 GetXfer() {
        return new Crc32(0x000000AF, 0x00000000, false, false, 0x00000000);
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
            BinaryPrimitives.WriteUInt32LittleEndian(destination, HashAsUInt32);
        } else {
            BinaryPrimitives.WriteUInt32BigEndian(destination, HashAsUInt32);
        }
    }

    #endregion NonCryptographicHashAlgorithm


    #region Algorithm

    private readonly uint _polynomial;
    private readonly uint _initialValue;
    private readonly bool _reverseIn;
    private readonly bool _reverseOut;
    private readonly uint _finalXorValue;

    private void ProcessInitialization() {
        _currDigest = _initialValue;
        var polynomialR = BitwiseReverse(_polynomial);
        for (var i = 0; i < 256; i++) {
            var crcValue = (uint)i;

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

    private readonly uint[] _lookup = new uint[256];
    private uint _currDigest;

    private void ProcessBytes(ReadOnlySpan<byte> source) {
        foreach (var b in source) {
            if (_reverseIn) {
                _currDigest = (_currDigest >> 8) ^ _lookup[((_currDigest & 0xff) ^ _lookupBitReverse0[b])];
            } else {
                _currDigest = (_currDigest >> 8) ^ _lookup[((_currDigest & 0xff) ^ (b))];
            }
        }
    }

    private static readonly uint[] _lookupBitReverse3 = { 0x00000000, 0x80000000, 0x40000000, 0xC0000000, 0x20000000, 0xA0000000, 0x60000000, 0xE0000000, 0x10000000, 0x90000000, 0x50000000, 0xD0000000, 0x30000000, 0xB0000000, 0x70000000, 0xF0000000, 0x08000000, 0x88000000, 0x48000000, 0xC8000000, 0x28000000, 0xA8000000, 0x68000000, 0xE8000000, 0x18000000, 0x98000000, 0x58000000, 0xD8000000, 0x38000000, 0xB8000000, 0x78000000, 0xF8000000, 0x04000000, 0x84000000, 0x44000000, 0xC4000000, 0x24000000, 0xA4000000, 0x64000000, 0xE4000000, 0x14000000, 0x94000000, 0x54000000, 0xD4000000, 0x34000000, 0xB4000000, 0x74000000, 0xF4000000, 0x0C000000, 0x8C000000, 0x4C000000, 0xCC000000, 0x2C000000, 0xAC000000, 0x6C000000, 0xEC000000, 0x1C000000, 0x9C000000, 0x5C000000, 0xDC000000, 0x3C000000, 0xBC000000, 0x7C000000, 0xFC000000, 0x02000000, 0x82000000, 0x42000000, 0xC2000000, 0x22000000, 0xA2000000, 0x62000000, 0xE2000000, 0x12000000, 0x92000000, 0x52000000, 0xD2000000, 0x32000000, 0xB2000000, 0x72000000, 0xF2000000, 0x0A000000, 0x8A000000, 0x4A000000, 0xCA000000, 0x2A000000, 0xAA000000, 0x6A000000, 0xEA000000, 0x1A000000, 0x9A000000, 0x5A000000, 0xDA000000, 0x3A000000, 0xBA000000, 0x7A000000, 0xFA000000, 0x06000000, 0x86000000, 0x46000000, 0xC6000000, 0x26000000, 0xA6000000, 0x66000000, 0xE6000000, 0x16000000, 0x96000000, 0x56000000, 0xD6000000, 0x36000000, 0xB6000000, 0x76000000, 0xF6000000, 0x0E000000, 0x8E000000, 0x4E000000, 0xCE000000, 0x2E000000, 0xAE000000, 0x6E000000, 0xEE000000, 0x1E000000, 0x9E000000, 0x5E000000, 0xDE000000, 0x3E000000, 0xBE000000, 0x7E000000, 0xFE000000, 0x01000000, 0x81000000, 0x41000000, 0xC1000000, 0x21000000, 0xA1000000, 0x61000000, 0xE1000000, 0x11000000, 0x91000000, 0x51000000, 0xD1000000, 0x31000000, 0xB1000000, 0x71000000, 0xF1000000, 0x09000000, 0x89000000, 0x49000000, 0xC9000000, 0x29000000, 0xA9000000, 0x69000000, 0xE9000000, 0x19000000, 0x99000000, 0x59000000, 0xD9000000, 0x39000000, 0xB9000000, 0x79000000, 0xF9000000, 0x05000000, 0x85000000, 0x45000000, 0xC5000000, 0x25000000, 0xA5000000, 0x65000000, 0xE5000000, 0x15000000, 0x95000000, 0x55000000, 0xD5000000, 0x35000000, 0xB5000000, 0x75000000, 0xF5000000, 0x0D000000, 0x8D000000, 0x4D000000, 0xCD000000, 0x2D000000, 0xAD000000, 0x6D000000, 0xED000000, 0x1D000000, 0x9D000000, 0x5D000000, 0xDD000000, 0x3D000000, 0xBD000000, 0x7D000000, 0xFD000000, 0x03000000, 0x83000000, 0x43000000, 0xC3000000, 0x23000000, 0xA3000000, 0x63000000, 0xE3000000, 0x13000000, 0x93000000, 0x53000000, 0xD3000000, 0x33000000, 0xB3000000, 0x73000000, 0xF3000000, 0x0B000000, 0x8B000000, 0x4B000000, 0xCB000000, 0x2B000000, 0xAB000000, 0x6B000000, 0xEB000000, 0x1B000000, 0x9B000000, 0x5B000000, 0xDB000000, 0x3B000000, 0xBB000000, 0x7B000000, 0xFB000000, 0x07000000, 0x87000000, 0x47000000, 0xC7000000, 0x27000000, 0xA7000000, 0x67000000, 0xE7000000, 0x17000000, 0x97000000, 0x57000000, 0xD7000000, 0x37000000, 0xB7000000, 0x77000000, 0xF7000000, 0x0F000000, 0x8F000000, 0x4F000000, 0xCF000000, 0x2F000000, 0xAF000000, 0x6F000000, 0xEF000000, 0x1F000000, 0x9F000000, 0x5F000000, 0xDF000000, 0x3F000000, 0xBF000000, 0x7F000000, 0xFF000000 };
    private static readonly uint[] _lookupBitReverse2 = { 0x00000000, 0x00800000, 0x00400000, 0x00C00000, 0x00200000, 0x00A00000, 0x00600000, 0x00E00000, 0x00100000, 0x00900000, 0x00500000, 0x00D00000, 0x00300000, 0x00B00000, 0x00700000, 0x00F00000, 0x00080000, 0x00880000, 0x00480000, 0x00C80000, 0x00280000, 0x00A80000, 0x00680000, 0x00E80000, 0x00180000, 0x00980000, 0x00580000, 0x00D80000, 0x00380000, 0x00B80000, 0x00780000, 0x00F80000, 0x00040000, 0x00840000, 0x00440000, 0x00C40000, 0x00240000, 0x00A40000, 0x00640000, 0x00E40000, 0x00140000, 0x00940000, 0x00540000, 0x00D40000, 0x00340000, 0x00B40000, 0x00740000, 0x00F40000, 0x000C0000, 0x008C0000, 0x004C0000, 0x00CC0000, 0x002C0000, 0x00AC0000, 0x006C0000, 0x00EC0000, 0x001C0000, 0x009C0000, 0x005C0000, 0x00DC0000, 0x003C0000, 0x00BC0000, 0x007C0000, 0x00FC0000, 0x00020000, 0x00820000, 0x00420000, 0x00C20000, 0x00220000, 0x00A20000, 0x00620000, 0x00E20000, 0x00120000, 0x00920000, 0x00520000, 0x00D20000, 0x00320000, 0x00B20000, 0x00720000, 0x00F20000, 0x000A0000, 0x008A0000, 0x004A0000, 0x00CA0000, 0x002A0000, 0x00AA0000, 0x006A0000, 0x00EA0000, 0x001A0000, 0x009A0000, 0x005A0000, 0x00DA0000, 0x003A0000, 0x00BA0000, 0x007A0000, 0x00FA0000, 0x00060000, 0x00860000, 0x00460000, 0x00C60000, 0x00260000, 0x00A60000, 0x00660000, 0x00E60000, 0x00160000, 0x00960000, 0x00560000, 0x00D60000, 0x00360000, 0x00B60000, 0x00760000, 0x00F60000, 0x000E0000, 0x008E0000, 0x004E0000, 0x00CE0000, 0x002E0000, 0x00AE0000, 0x006E0000, 0x00EE0000, 0x001E0000, 0x009E0000, 0x005E0000, 0x00DE0000, 0x003E0000, 0x00BE0000, 0x007E0000, 0x00FE0000, 0x00010000, 0x00810000, 0x00410000, 0x00C10000, 0x00210000, 0x00A10000, 0x00610000, 0x00E10000, 0x00110000, 0x00910000, 0x00510000, 0x00D10000, 0x00310000, 0x00B10000, 0x00710000, 0x00F10000, 0x00090000, 0x00890000, 0x00490000, 0x00C90000, 0x00290000, 0x00A90000, 0x00690000, 0x00E90000, 0x00190000, 0x00990000, 0x00590000, 0x00D90000, 0x00390000, 0x00B90000, 0x00790000, 0x00F90000, 0x00050000, 0x00850000, 0x00450000, 0x00C50000, 0x00250000, 0x00A50000, 0x00650000, 0x00E50000, 0x00150000, 0x00950000, 0x00550000, 0x00D50000, 0x00350000, 0x00B50000, 0x00750000, 0x00F50000, 0x000D0000, 0x008D0000, 0x004D0000, 0x00CD0000, 0x002D0000, 0x00AD0000, 0x006D0000, 0x00ED0000, 0x001D0000, 0x009D0000, 0x005D0000, 0x00DD0000, 0x003D0000, 0x00BD0000, 0x007D0000, 0x00FD0000, 0x00030000, 0x00830000, 0x00430000, 0x00C30000, 0x00230000, 0x00A30000, 0x00630000, 0x00E30000, 0x00130000, 0x00930000, 0x00530000, 0x00D30000, 0x00330000, 0x00B30000, 0x00730000, 0x00F30000, 0x000B0000, 0x008B0000, 0x004B0000, 0x00CB0000, 0x002B0000, 0x00AB0000, 0x006B0000, 0x00EB0000, 0x001B0000, 0x009B0000, 0x005B0000, 0x00DB0000, 0x003B0000, 0x00BB0000, 0x007B0000, 0x00FB0000, 0x00070000, 0x00870000, 0x00470000, 0x00C70000, 0x00270000, 0x00A70000, 0x00670000, 0x00E70000, 0x00170000, 0x00970000, 0x00570000, 0x00D70000, 0x00370000, 0x00B70000, 0x00770000, 0x00F70000, 0x000F0000, 0x008F0000, 0x004F0000, 0x00CF0000, 0x002F0000, 0x00AF0000, 0x006F0000, 0x00EF0000, 0x001F0000, 0x009F0000, 0x005F0000, 0x00DF0000, 0x003F0000, 0x00BF0000, 0x007F0000, 0x00FF0000 };
    private static readonly uint[] _lookupBitReverse1 = { 0x00000000, 0x00008000, 0x00004000, 0x0000C000, 0x00002000, 0x0000A000, 0x00006000, 0x0000E000, 0x00001000, 0x00009000, 0x00005000, 0x0000D000, 0x00003000, 0x0000B000, 0x00007000, 0x0000F000, 0x00000800, 0x00008800, 0x00004800, 0x0000C800, 0x00002800, 0x0000A800, 0x00006800, 0x0000E800, 0x00001800, 0x00009800, 0x00005800, 0x0000D800, 0x00003800, 0x0000B800, 0x00007800, 0x0000F800, 0x00000400, 0x00008400, 0x00004400, 0x0000C400, 0x00002400, 0x0000A400, 0x00006400, 0x0000E400, 0x00001400, 0x00009400, 0x00005400, 0x0000D400, 0x00003400, 0x0000B400, 0x00007400, 0x0000F400, 0x00000C00, 0x00008C00, 0x00004C00, 0x0000CC00, 0x00002C00, 0x0000AC00, 0x00006C00, 0x0000EC00, 0x00001C00, 0x00009C00, 0x00005C00, 0x0000DC00, 0x00003C00, 0x0000BC00, 0x00007C00, 0x0000FC00, 0x00000200, 0x00008200, 0x00004200, 0x0000C200, 0x00002200, 0x0000A200, 0x00006200, 0x0000E200, 0x00001200, 0x00009200, 0x00005200, 0x0000D200, 0x00003200, 0x0000B200, 0x00007200, 0x0000F200, 0x00000A00, 0x00008A00, 0x00004A00, 0x0000CA00, 0x00002A00, 0x0000AA00, 0x00006A00, 0x0000EA00, 0x00001A00, 0x00009A00, 0x00005A00, 0x0000DA00, 0x00003A00, 0x0000BA00, 0x00007A00, 0x0000FA00, 0x00000600, 0x00008600, 0x00004600, 0x0000C600, 0x00002600, 0x0000A600, 0x00006600, 0x0000E600, 0x00001600, 0x00009600, 0x00005600, 0x0000D600, 0x00003600, 0x0000B600, 0x00007600, 0x0000F600, 0x00000E00, 0x00008E00, 0x00004E00, 0x0000CE00, 0x00002E00, 0x0000AE00, 0x00006E00, 0x0000EE00, 0x00001E00, 0x00009E00, 0x00005E00, 0x0000DE00, 0x00003E00, 0x0000BE00, 0x00007E00, 0x0000FE00, 0x00000100, 0x00008100, 0x00004100, 0x0000C100, 0x00002100, 0x0000A100, 0x00006100, 0x0000E100, 0x00001100, 0x00009100, 0x00005100, 0x0000D100, 0x00003100, 0x0000B100, 0x00007100, 0x0000F100, 0x00000900, 0x00008900, 0x00004900, 0x0000C900, 0x00002900, 0x0000A900, 0x00006900, 0x0000E900, 0x00001900, 0x00009900, 0x00005900, 0x0000D900, 0x00003900, 0x0000B900, 0x00007900, 0x0000F900, 0x00000500, 0x00008500, 0x00004500, 0x0000C500, 0x00002500, 0x0000A500, 0x00006500, 0x0000E500, 0x00001500, 0x00009500, 0x00005500, 0x0000D500, 0x00003500, 0x0000B500, 0x00007500, 0x0000F500, 0x00000D00, 0x00008D00, 0x00004D00, 0x0000CD00, 0x00002D00, 0x0000AD00, 0x00006D00, 0x0000ED00, 0x00001D00, 0x00009D00, 0x00005D00, 0x0000DD00, 0x00003D00, 0x0000BD00, 0x00007D00, 0x0000FD00, 0x00000300, 0x00008300, 0x00004300, 0x0000C300, 0x00002300, 0x0000A300, 0x00006300, 0x0000E300, 0x00001300, 0x00009300, 0x00005300, 0x0000D300, 0x00003300, 0x0000B300, 0x00007300, 0x0000F300, 0x00000B00, 0x00008B00, 0x00004B00, 0x0000CB00, 0x00002B00, 0x0000AB00, 0x00006B00, 0x0000EB00, 0x00001B00, 0x00009B00, 0x00005B00, 0x0000DB00, 0x00003B00, 0x0000BB00, 0x00007B00, 0x0000FB00, 0x00000700, 0x00008700, 0x00004700, 0x0000C700, 0x00002700, 0x0000A700, 0x00006700, 0x0000E700, 0x00001700, 0x00009700, 0x00005700, 0x0000D700, 0x00003700, 0x0000B700, 0x00007700, 0x0000F700, 0x00000F00, 0x00008F00, 0x00004F00, 0x0000CF00, 0x00002F00, 0x0000AF00, 0x00006F00, 0x0000EF00, 0x00001F00, 0x00009F00, 0x00005F00, 0x0000DF00, 0x00003F00, 0x0000BF00, 0x00007F00, 0x0000FF00 };
    private static readonly uint[] _lookupBitReverse0 = { 0x00000000, 0x00000080, 0x00000040, 0x000000C0, 0x00000020, 0x000000A0, 0x00000060, 0x000000E0, 0x00000010, 0x00000090, 0x00000050, 0x000000D0, 0x00000030, 0x000000B0, 0x00000070, 0x000000F0, 0x00000008, 0x00000088, 0x00000048, 0x000000C8, 0x00000028, 0x000000A8, 0x00000068, 0x000000E8, 0x00000018, 0x00000098, 0x00000058, 0x000000D8, 0x00000038, 0x000000B8, 0x00000078, 0x000000F8, 0x00000004, 0x00000084, 0x00000044, 0x000000C4, 0x00000024, 0x000000A4, 0x00000064, 0x000000E4, 0x00000014, 0x00000094, 0x00000054, 0x000000D4, 0x00000034, 0x000000B4, 0x00000074, 0x000000F4, 0x0000000C, 0x0000008C, 0x0000004C, 0x000000CC, 0x0000002C, 0x000000AC, 0x0000006C, 0x000000EC, 0x0000001C, 0x0000009C, 0x0000005C, 0x000000DC, 0x0000003C, 0x000000BC, 0x0000007C, 0x000000FC, 0x00000002, 0x00000082, 0x00000042, 0x000000C2, 0x00000022, 0x000000A2, 0x00000062, 0x000000E2, 0x00000012, 0x00000092, 0x00000052, 0x000000D2, 0x00000032, 0x000000B2, 0x00000072, 0x000000F2, 0x0000000A, 0x0000008A, 0x0000004A, 0x000000CA, 0x0000002A, 0x000000AA, 0x0000006A, 0x000000EA, 0x0000001A, 0x0000009A, 0x0000005A, 0x000000DA, 0x0000003A, 0x000000BA, 0x0000007A, 0x000000FA, 0x00000006, 0x00000086, 0x00000046, 0x000000C6, 0x00000026, 0x000000A6, 0x00000066, 0x000000E6, 0x00000016, 0x00000096, 0x00000056, 0x000000D6, 0x00000036, 0x000000B6, 0x00000076, 0x000000F6, 0x0000000E, 0x0000008E, 0x0000004E, 0x000000CE, 0x0000002E, 0x000000AE, 0x0000006E, 0x000000EE, 0x0000001E, 0x0000009E, 0x0000005E, 0x000000DE, 0x0000003E, 0x000000BE, 0x0000007E, 0x000000FE, 0x00000001, 0x00000081, 0x00000041, 0x000000C1, 0x00000021, 0x000000A1, 0x00000061, 0x000000E1, 0x00000011, 0x00000091, 0x00000051, 0x000000D1, 0x00000031, 0x000000B1, 0x00000071, 0x000000F1, 0x00000009, 0x00000089, 0x00000049, 0x000000C9, 0x00000029, 0x000000A9, 0x00000069, 0x000000E9, 0x00000019, 0x00000099, 0x00000059, 0x000000D9, 0x00000039, 0x000000B9, 0x00000079, 0x000000F9, 0x00000005, 0x00000085, 0x00000045, 0x000000C5, 0x00000025, 0x000000A5, 0x00000065, 0x000000E5, 0x00000015, 0x00000095, 0x00000055, 0x000000D5, 0x00000035, 0x000000B5, 0x00000075, 0x000000F5, 0x0000000D, 0x0000008D, 0x0000004D, 0x000000CD, 0x0000002D, 0x000000AD, 0x0000006D, 0x000000ED, 0x0000001D, 0x0000009D, 0x0000005D, 0x000000DD, 0x0000003D, 0x000000BD, 0x0000007D, 0x000000FD, 0x00000003, 0x00000083, 0x00000043, 0x000000C3, 0x00000023, 0x000000A3, 0x00000063, 0x000000E3, 0x00000013, 0x00000093, 0x00000053, 0x000000D3, 0x00000033, 0x000000B3, 0x00000073, 0x000000F3, 0x0000000B, 0x0000008B, 0x0000004B, 0x000000CB, 0x0000002B, 0x000000AB, 0x0000006B, 0x000000EB, 0x0000001B, 0x0000009B, 0x0000005B, 0x000000DB, 0x0000003B, 0x000000BB, 0x0000007B, 0x000000FB, 0x00000007, 0x00000087, 0x00000047, 0x000000C7, 0x00000027, 0x000000A7, 0x00000067, 0x000000E7, 0x00000017, 0x00000097, 0x00000057, 0x000000D7, 0x00000037, 0x000000B7, 0x00000077, 0x000000F7, 0x0000000F, 0x0000008F, 0x0000004F, 0x000000CF, 0x0000002F, 0x000000AF, 0x0000006F, 0x000000EF, 0x0000001F, 0x0000009F, 0x0000005F, 0x000000DF, 0x0000003F, 0x000000BF, 0x0000007F, 0x000000FF };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint BitwiseReverse(uint value) {
        return _lookupBitReverse3[value & 0xFF] | _lookupBitReverse2[(value >> 8) & 0xFF] | _lookupBitReverse1[(value >> 16) & 0xFF] | _lookupBitReverse0[value >> 24];
    }

    #endregion Algorithm


    /// <summary>
    /// Gets current digest.
    /// </summary>
    [Obsolete("Use HashAsUInt32 instead.")]
    public int HashAsInt32 {
        get { return (int)HashAsUInt32; }
    }

    /// <summary>
    /// Gets current digest.
    /// </summary>
    public uint HashAsUInt32 {
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
    public static uint ToReversedReciprocalPolynomial(uint polynomial) {
        return (polynomial >> 1) | 0x80000000;
    }

    /// <summary>
    /// Converts polynomial from its reversed reciprocal to normal form.
    /// </summary>
    /// <param name="reversedReciprocalPolynomial">Reversed reciprocal polynomial.</param>
    public static uint FromReversedReciprocalPolynomial(uint reversedReciprocalPolynomial) {
        return (reversedReciprocalPolynomial << 1) | 0x01;
    }

    #endregion ReciprocalPolynomial

}
