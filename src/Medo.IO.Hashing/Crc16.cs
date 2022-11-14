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
//            Fixed CCITT and Z-modem calcultions
//            Added resources
//2007-10-31: New version

namespace Medo.IO.Hashing;

using System;
using System.Buffers.Binary;
using System.IO.Hashing;
using System.Runtime.CompilerServices;

/// <summary>
/// Computes hash using 16-bit CRC algorithm.
/// The following CRC-16 variants are supported: ACORN, ARC, AUG-CCITT,
/// AUTOSAR, BUYPASS, CDMA2000, CCITT, CCITT-FALSE, CCITT-TRUE, CMS, DARC,
/// DDS-110, DECT-R, DECT-X, DNP, EN-13757, EPC, EPC-C1G2, GENIBUS, GSM,
/// I-CODE, IBM-3740, ISO-HDLD, IBM-SDLC, IEC-61158-2, IEEE 802.3,
/// ISO-IEC-14443-3-A, ISO-IEC-14443-3-B, KERMIT, LHA, LJ1200, LTE, MAXIM,
/// MAXIM-DOW, MCRF4XX, MODBUS, NRSC-5, OPENSAFETY-A, OPENSAFETY-B,
/// PROFIBUS, RIELLO, SPI-FUJITSU, T10-DIF, TELEDISK, TMS37157, UMTS, USB,
/// V-41-LSB, V-41-MSB, VERIFONE, X-25, XMODEM, ZMODEM, and custom definitions.
/// </summary>
/// <example>
/// <code>
/// var crc = Crc16.GetArc();
/// crc.Append(Encoding.ASCII.GetBytes("Test"));
/// var hashValue = crc.HashAsUInt16;
/// </code>
/// </example>
public sealed class Crc16 : NonCryptographicHashAlgorithm {

    /// <summary>
    /// Creates a new instance using the CRC-16/ARC variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8005
    /// Initial value: 0x0000
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000
    /// </remarks>
    [Obsolete("Use GetCustom instead")]
    public Crc16()
        : this(0x8005, 0x0000, true, true, 0x0000) {
    }

    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="polynomial">Polynomial value.</param>
    /// <param name="initialValue">Starting digest.</param>
    /// <param name="reflectIn">If true, input byte is in reflected (LSB first) bit order.</param>
    /// <param name="reflectOut">If true, digest is in reflected (LSB first) bit order.</param>
    /// <param name="finalXorValue">Final XOR value.</param>
    private Crc16(ushort polynomial, ushort initialValue, bool reflectIn, bool reflectOut, ushort finalXorValue)
        : base(2) {
        _polynomial = polynomial;
        _initialValue = initialValue;
        _reverseIn = reflectIn ^ BitConverter.IsLittleEndian;
        _reverseOut = reflectOut ^ BitConverter.IsLittleEndian;
        _finalXorValue = finalXorValue;
        ProcessInitialization();
    }


    /// <summary>
    /// Returns a custom CRC-16 variant.
    /// </summary>
    /// <param name="polynomial">Polynomial value.</param>
    /// <param name="initialValue">Starting digest.</param>
    /// <param name="reflectIn">If true, input byte is in reflected (LSB first) bit order.</param>
    /// <param name="reflectOut">If true, digest is in reflected (LSB first) bit order.</param>
    /// <param name="finalXorValue">Final XOR value.</param>
    /// <remarks>
    /// Name                                            Poly    Init    Xor     Reflect
    /// -------------------------------------------------------------------------------
    /// ARC / IEEE 802.3 / LHA                          0x8005  0x0000  0x0000  In/Out
    /// CDMA2000                                        0xC867  0xFFFF  0x0000  -
    /// CMS                                             0x8005  0xFFFF  0x0000  -
    /// DDS-110                                         0x8005  0xB001  0x0000  -
    /// DECT-R                                          0x0589  0x0000  0x0001  -
    /// DECT-X                                          0x0589  0x0000  0x0000  -
    /// DNP                                             0x3D65  0x0000  0xFFFF  In/Out
    /// EN-13757                                        0x3D65  0x0000  0xFFFF  -
    /// GENIBUS / DARC / EPC / EPC-C1G2                 0x1021  0xFFFF  0xFFFF  -
    /// GSM                                             0x1021  0x0000  0xFFFF  -
    /// IBM-3740 / AUTOSAR / CCITT-FALSE                0x1021  0xFFFF  0x0000  -
    /// IBM-SDLC / ISO-HDLD / ISO-IEC-14443-3-B / X-25  0x1021  0xFFFF  0xFFFF  In/Out
    /// I-CODE                                          0x1021  0xFFFF  0x0000  -
    /// ISO-IEC-14443-3-A                               0x1021  0xC6C6  0x0000  In/Out
    /// KERMIT / CCITT / CCITT-TRUE / V-41-LSB          0x1021  0x0000  0x0000  In/Out
    /// LJ1200                                          0x6F63  0x0000  0x0000  -
    /// MAXIM-DOW / MAXIM                               0x8005  0x0000  0xFFFF  In/Out
    //  MCRF4XX                                         0x1021  0xFFFF  0x0000  In/Out
    /// MODBUS                                          0x8005  0xFFFF  0x0000  In/Out
    /// NRSC-5                                          0x800B  0xFFFF  0x0000  In/Out
    /// OPENSAFETY-A                                    0x5935  0x0000  0x0000  -
    /// OPENSAFETY-B                                    0x755B  0x0000  0x0000  -
    /// PROFIBUS / IEC-61158-2                          0x1DCF  0xFFFF  0xFFFF  -
    /// RIELLO                                          0x1021  0x554D  0x0000  In/Out
    /// SPI-FUJITSU / AUG-CCITT                         0x1021  0xF0B8  0x0000  -
    /// T10-DIF                                         0x8BB7  0x0000  0x0000  -
    /// TELEDISK                                        0xA097  0x0000  0x0000  -
    /// TMS37157                                        0x1021  0x3791  0x0000  In/Out
    /// UMTS / BUYPASS / VERIFONE                       0x8005  0x0000  0x0000  -
    /// USB                                             0x8005  0xFFFF  0xFFFF  In/Out
    /// XMODEM / ACORN / LTE / V-41-MSB / ZMODEM        0x1021  0x0000  0x0000  -
    ///
    /// See also:
    /// - https://reveng.sourceforge.io/crc-catalogue/16.htm
    /// - https://users.ece.cmu.edu/~koopman/crc/index.html
    /// </remarks>
    public static Crc16 GetCustom(ushort polynomial, ushort initialValue, bool reflectIn, bool reflectOut, ushort finalXorValue) {
        return new Crc16(polynomial, initialValue, reflectIn, reflectOut, finalXorValue);
    }


    /// <summary>
    /// Returns CRC-16/ARC variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8005
    /// Initial value: 0x0000
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetArc() {
        return new Crc16(0x8005, 0x0000, true, true, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/IEEE-802.3 variant.
    /// More widely known as CRC-16/ARC.
    /// </summary>
    public static Crc16 GetIeee8023() {
        return GetArc();
    }

    /// <summary>
    /// Returns CRC-16/LHA variant.
    /// More widely known as CRC-16/ARC.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8005
    /// Initial value: 0x0000
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetLha() {
        return GetArc();
    }

    /// <summary>
    /// Returns CRC-16/CDMA2000 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0xC867
    /// Initial value: 0xFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetCdma2000() {
        return new Crc16(0xC867, 0xFFFF, false, false, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/CMS variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8005
    /// Initial value: 0xFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetCms() {
        return new Crc16(0x8005, 0xFFFF, false, false, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/DDS-110 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8005
    /// Initial value: 0xB001
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetDds110() {
        return new Crc16(0x8005, 0xB001, false, false, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/DECT-R variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x0589
    /// Initial value: 0x800D
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0001
    /// </remarks>
    public static Crc16 GetDectR() {
        return new Crc16(0x0589, 0x0000, false, false, 0x0001);
    }

    /// <summary>
    /// Returns CRC-16/DECT-X variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x0589
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetDectX() {
        return new Crc16(0x0589, 0x0000, false, false, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/DNP variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x3D65
    /// Initial value: 0x0000
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetDnp() {
        return new Crc16(0x3D65, 0x0000, true, true, 0xFFFF);
    }

    /// <summary>
    /// Returns CRC-16/EN-13757 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x3D65
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetEn13757() {
        return new Crc16(0x3D65, 0x0000, false, false, 0xFFFF);
    }

    /// <summary>
    /// Returns CRC-16/GENIBUS variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetGenibus() {
        return new Crc16(0x1021, 0xFFFF, false, false, 0xFFFF);
    }

    /// <summary>
    /// Returns CRC-16/DARC variant.
    /// More widely known as CRC-16/GENIBUS.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetDarc() {
        return GetGenibus();
    }

    /// <summary>
    /// Returns CRC-16/EPC variant.
    /// More widely known as CRC-16/GENIBUS.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetEpc() {
        return GetGenibus();
    }

    /// <summary>
    /// Returns CRC-16/EPC-C1G2 variant.
    /// More widely known as CRC-16/GENIBUS.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetEpcC1G2() {
        return GetGenibus();
    }

    /// <summary>
    /// Returns CRC-16/I-CODE variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetICode() {
        return new Crc16(0x1021, 0xFFFF, false, false, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/GSM variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetGsm() {
        return new Crc16(0x1021, 0x0000, false, false, 0xFFFF);
    }

    /// <summary>
    /// Returns CRC-16/IBM-3740 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetIbm3740() {
        return new Crc16(0x1021, 0xFFFF, false, false, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/AUTOSAR variant.
    /// More widely known as CRC-16/IBM-3740.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetAutosar() {
        return GetIbm3740();
    }

    /// <summary>
    /// Returns CRC-16/CCITT-FALSE variant.
    /// More widely known as CRC-16/IBM-3740.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetCcittFalse() {
        return GetIbm3740();
    }

    /// <summary>
    /// Returns CRC-16/IBM-SDLC variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetIbmSdlc() {
        return new Crc16(0x1021, 0xFFFF, true, true, 0xFFFF);
    }

    /// <summary>
    /// Returns CRC-16/ISO-HDLD variant.
    /// More widely known as CRC-16/IBM-SDLC.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetIsoHdld() {
        return GetIbmSdlc();
    }

    /// <summary>
    /// Returns CRC-16/ISO-IEC-14443-3-B variant.
    /// More widely known as CRC-16/IBM-SDLC.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetIsoIec144433B() {
        return GetIbmSdlc();
    }

    /// <summary>
    /// Returns CRC-16/X-25 variant.
    /// More widely known as CRC-16/IBM-SDLC.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetX25() {
        return GetIbmSdlc();
    }

    /// <summary>
    /// Returns ISO-IEC-14443-3-A CRC-16 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xC6C6
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetIsoIec144433A() {
        return new Crc16(0x1021, 0xC6C6, true, true, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/KERMIT variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0x0000
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetKermit() {
        return new Crc16(0x1021, 0x0000, true, true, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/CCITT variant.
    /// More widely known as CRC-16/KERMIT.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0x0000
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetCcitt() {
        return GetKermit();
    }

    /// <summary>
    /// Returns CRC-16/CCITT-TRUE variant.
    /// More widely known as CRC-16/KERMIT.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0x0000
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetCcittTrue() {
        return GetKermit();
    }

    /// <summary>
    /// Returns CRC-16/V-41-LSB variant.
    /// More widely known as CRC-16/KERMIT.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xC6C6
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetV41Lsb() {
        return GetKermit();
    }

    /// <summary>
    /// Returns LJ1200 CRC-16 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x6F63
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetLj1200() {
        return new Crc16(0x6F63, 0x0000, false, false, 0x0000);
    }

    /// <summary>
    /// Returns Maxim CRC-16 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8005
    /// Initial value: 0x0000
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetMaximDow() {
        return new Crc16(0x8005, 0x0000, true, true, 0xFFFF);
    }

    /// <summary>
    /// Returns CRC-16/MAXIM variant.
    /// More widely known as CRC-16/MAXIM-DOW.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8005
    /// Initial value: 0x0000
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetMaxim() {
        return GetMaximDow();
    }

    /// <summary>
    /// Returns CRC-16/MCRF4XX variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetMcrf4xx() {
        return new Crc16(0x1021, 0xFFFF, true, true, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/MODBUS variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8005
    /// Initial value: 0xFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetModbus() {
        return new Crc16(0x8005, 0xFFFF, true, true, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/NRSC-5 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x800B
    /// Initial value: 0xFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetNrsc5() {
        return new Crc16(0x080B, 0xFFFF, true, true, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/OPENSAFETY-A variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x5935
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetOpenSafetyA() {
        return new Crc16(0x5935, 0x0000, false, false, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/OPENSAFETY-B variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x755B
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetOpenSafetyB() {
        return new Crc16(0x755B, 0x0000, false, false, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/PROFIBUS variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1DCF
    /// Initial value: 0xFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetProfibus() {
        return new Crc16(0x1DCF, 0xFFFF, false, false, 0xFFFF);
    }

    /// <summary>
    /// Returns CRC-16/IEC-61158-2 variant.
    /// More widely known as CRC-16/PROFIBUS.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1DCF
    /// Initial value: 0xFFFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetIec611582() {
        return GetProfibus();
    }

    /// <summary>
    /// Returns CRC-16/RIELLO variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0x554D
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetRiello() {
        return new Crc16(0x1021, 0x554D, true, true, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/SPI-FUJITSU variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0xF0B8
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetSpiFujitsu() {
        return new Crc16(0x1021, 0xF0B8, false, false, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/AUG-CCITT variant.
    /// More widely known as CRC-16/SPI-FUJITSU.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8005
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetAugCcitt() {
        return GetSpiFujitsu();
    }

    /// <summary>
    /// Returns CRC-16/T10-DIF variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8BB7
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetT10Dif() {
        return new Crc16(0x8BB7, 0x0000, false, false, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/TELEDISK variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetTeledisk() {
        return new Crc16(0xA097, 0x0000, false, false, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/TMS37157 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0x3791
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetTms37157() {
        return new Crc16(0x1021, 0x3791, true, true, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/UMTS variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8005
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetUmts() {
        return new Crc16(0x8005, 0x0000, false, false, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/BUYPASS variant.
    /// More widely known as CRC-16/UMTS.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8005
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetBuypass() {
        return GetUmts();
    }

    /// <summary>
    /// Returns CRC-16/VERIFONE variant.
    /// More widely known as CRC-16/UMTS.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8005
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetVerifone() {
        return GetUmts();
    }

    /// <summary>
    /// Returns CRC-16/USB variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x8005
    /// Initial value: 0xFFFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0xFFFF
    /// </remarks>
    public static Crc16 GetUsb() {
        return new Crc16(0x8005, 0xFFFF, true, true, 0xFFFF);
    }

    /// <summary>
    /// Returns CRC-16/XMODEM variant.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetXModem() {
        return new Crc16(0x1021, 0x0000, false, false, 0x0000);
    }

    /// <summary>
    /// Returns CRC-16/ACORN variant.
    /// More widely known as CRC-16/XMODEM.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetAcorn() {
        return GetXModem();
    }

    /// <summary>
    /// Returns CRC-16/LTE variant.
    /// More widely known as CRC-16/XMODEM.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetLte() {
        return GetXModem();
    }

    /// <summary>
    /// Returns CRC-16/V-41-MSB variant.
    /// More widely known as CRC-16/XMODEM.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetV41Msb() {
        return GetXModem();
    }

    /// <summary>
    /// Returns CRC-16/ZMODEM variant.
    /// More widely known as CRC-16/XMODEM.
    /// </summary>
    /// <remarks>
    /// Polynom: 0x1021
    /// Initial value: 0x0000
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x0000
    /// </remarks>
    public static Crc16 GetZModem() {
        return GetXModem();
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

    protected override void GetCurrentHashCore(Span<byte> destination) {
        if (BitConverter.IsLittleEndian ^ _reverseOut) {
            BinaryPrimitives.WriteUInt16LittleEndian(destination, HashAsUInt16);
        } else {
            BinaryPrimitives.WriteUInt16BigEndian(destination, HashAsUInt16);
        }
    }

    #endregion NonCryptographicHashAlgorithm


    #region Algorithm

    private readonly ushort _polynomial;
    private readonly ushort _initialValue;
    private readonly bool _reverseIn;
    private readonly bool _reverseOut;
    private readonly ushort _finalXorValue;

    private void ProcessInitialization() {
        _currDigest = _initialValue;
        var polynomialR = BitwiseReverse(_polynomial);

        for (var i = 0; i < 256; i++) {
            var crcValue = (ushort)i;

            for (var j = 1; j <= 8; j++) {
                if ((crcValue & 1) == 1) {
                    crcValue = (ushort)((crcValue >> 1) ^ polynomialR);
                } else {
                    crcValue >>= 1;
                }
            }

            _lookup[i] = crcValue;
        }
    }

    private readonly ushort[] _lookup = new ushort[256];
    private ushort _currDigest;

    private void ProcessBytes(ReadOnlySpan<byte> source) {
        foreach (var b in source) {
            if (_reverseIn) {
                _currDigest = (ushort)((_currDigest >> 8) ^ _lookup[(_currDigest & 0xff) ^ _lookupBitReverse0[b]]);
            } else {
                _currDigest = (ushort)((_currDigest >> 8) ^ _lookup[(_currDigest & 0xff) ^ b]);
            }
        }
    }

    private static readonly ushort[] _lookupBitReverse1 = { 0x0000, 0x8000, 0x4000, 0xC000, 0x2000, 0xA000, 0x6000, 0xE000, 0x1000, 0x9000, 0x5000, 0xD000, 0x3000, 0xB000, 0x7000, 0xF000, 0x0800, 0x8800, 0x4800, 0xC800, 0x2800, 0xA800, 0x6800, 0xE800, 0x1800, 0x9800, 0x5800, 0xD800, 0x3800, 0xB800, 0x7800, 0xF800, 0x0400, 0x8400, 0x4400, 0xC400, 0x2400, 0xA400, 0x6400, 0xE400, 0x1400, 0x9400, 0x5400, 0xD400, 0x3400, 0xB400, 0x7400, 0xF400, 0x0C00, 0x8C00, 0x4C00, 0xCC00, 0x2C00, 0xAC00, 0x6C00, 0xEC00, 0x1C00, 0x9C00, 0x5C00, 0xDC00, 0x3C00, 0xBC00, 0x7C00, 0xFC00, 0x0200, 0x8200, 0x4200, 0xC200, 0x2200, 0xA200, 0x6200, 0xE200, 0x1200, 0x9200, 0x5200, 0xD200, 0x3200, 0xB200, 0x7200, 0xF200, 0x0A00, 0x8A00, 0x4A00, 0xCA00, 0x2A00, 0xAA00, 0x6A00, 0xEA00, 0x1A00, 0x9A00, 0x5A00, 0xDA00, 0x3A00, 0xBA00, 0x7A00, 0xFA00, 0x0600, 0x8600, 0x4600, 0xC600, 0x2600, 0xA600, 0x6600, 0xE600, 0x1600, 0x9600, 0x5600, 0xD600, 0x3600, 0xB600, 0x7600, 0xF600, 0x0E00, 0x8E00, 0x4E00, 0xCE00, 0x2E00, 0xAE00, 0x6E00, 0xEE00, 0x1E00, 0x9E00, 0x5E00, 0xDE00, 0x3E00, 0xBE00, 0x7E00, 0xFE00, 0x0100, 0x8100, 0x4100, 0xC100, 0x2100, 0xA100, 0x6100, 0xE100, 0x1100, 0x9100, 0x5100, 0xD100, 0x3100, 0xB100, 0x7100, 0xF100, 0x0900, 0x8900, 0x4900, 0xC900, 0x2900, 0xA900, 0x6900, 0xE900, 0x1900, 0x9900, 0x5900, 0xD900, 0x3900, 0xB900, 0x7900, 0xF900, 0x0500, 0x8500, 0x4500, 0xC500, 0x2500, 0xA500, 0x6500, 0xE500, 0x1500, 0x9500, 0x5500, 0xD500, 0x3500, 0xB500, 0x7500, 0xF500, 0x0D00, 0x8D00, 0x4D00, 0xCD00, 0x2D00, 0xAD00, 0x6D00, 0xED00, 0x1D00, 0x9D00, 0x5D00, 0xDD00, 0x3D00, 0xBD00, 0x7D00, 0xFD00, 0x0300, 0x8300, 0x4300, 0xC300, 0x2300, 0xA300, 0x6300, 0xE300, 0x1300, 0x9300, 0x5300, 0xD300, 0x3300, 0xB300, 0x7300, 0xF300, 0x0B00, 0x8B00, 0x4B00, 0xCB00, 0x2B00, 0xAB00, 0x6B00, 0xEB00, 0x1B00, 0x9B00, 0x5B00, 0xDB00, 0x3B00, 0xBB00, 0x7B00, 0xFB00, 0x0700, 0x8700, 0x4700, 0xC700, 0x2700, 0xA700, 0x6700, 0xE700, 0x1700, 0x9700, 0x5700, 0xD700, 0x3700, 0xB700, 0x7700, 0xF700, 0x0F00, 0x8F00, 0x4F00, 0xCF00, 0x2F00, 0xAF00, 0x6F00, 0xEF00, 0x1F00, 0x9F00, 0x5F00, 0xDF00, 0x3F00, 0xBF00, 0x7F00, 0xFF00 };
    private static readonly ushort[] _lookupBitReverse0 = { 0x0000, 0x0080, 0x0040, 0x00C0, 0x0020, 0x00A0, 0x0060, 0x00E0, 0x0010, 0x0090, 0x0050, 0x00D0, 0x0030, 0x00B0, 0x0070, 0x00F0, 0x0008, 0x0088, 0x0048, 0x00C8, 0x0028, 0x00A8, 0x0068, 0x00E8, 0x0018, 0x0098, 0x0058, 0x00D8, 0x0038, 0x00B8, 0x0078, 0x00F8, 0x0004, 0x0084, 0x0044, 0x00C4, 0x0024, 0x00A4, 0x0064, 0x00E4, 0x0014, 0x0094, 0x0054, 0x00D4, 0x0034, 0x00B4, 0x0074, 0x00F4, 0x000C, 0x008C, 0x004C, 0x00CC, 0x002C, 0x00AC, 0x006C, 0x00EC, 0x001C, 0x009C, 0x005C, 0x00DC, 0x003C, 0x00BC, 0x007C, 0x00FC, 0x0002, 0x0082, 0x0042, 0x00C2, 0x0022, 0x00A2, 0x0062, 0x00E2, 0x0012, 0x0092, 0x0052, 0x00D2, 0x0032, 0x00B2, 0x0072, 0x00F2, 0x000A, 0x008A, 0x004A, 0x00CA, 0x002A, 0x00AA, 0x006A, 0x00EA, 0x001A, 0x009A, 0x005A, 0x00DA, 0x003A, 0x00BA, 0x007A, 0x00FA, 0x0006, 0x0086, 0x0046, 0x00C6, 0x0026, 0x00A6, 0x0066, 0x00E6, 0x0016, 0x0096, 0x0056, 0x00D6, 0x0036, 0x00B6, 0x0076, 0x00F6, 0x000E, 0x008E, 0x004E, 0x00CE, 0x002E, 0x00AE, 0x006E, 0x00EE, 0x001E, 0x009E, 0x005E, 0x00DE, 0x003E, 0x00BE, 0x007E, 0x00FE, 0x0001, 0x0081, 0x0041, 0x00C1, 0x0021, 0x00A1, 0x0061, 0x00E1, 0x0011, 0x0091, 0x0051, 0x00D1, 0x0031, 0x00B1, 0x0071, 0x00F1, 0x0009, 0x0089, 0x0049, 0x00C9, 0x0029, 0x00A9, 0x0069, 0x00E9, 0x0019, 0x0099, 0x0059, 0x00D9, 0x0039, 0x00B9, 0x0079, 0x00F9, 0x0005, 0x0085, 0x0045, 0x00C5, 0x0025, 0x00A5, 0x0065, 0x00E5, 0x0015, 0x0095, 0x0055, 0x00D5, 0x0035, 0x00B5, 0x0075, 0x00F5, 0x000D, 0x008D, 0x004D, 0x00CD, 0x002D, 0x00AD, 0x006D, 0x00ED, 0x001D, 0x009D, 0x005D, 0x00DD, 0x003D, 0x00BD, 0x007D, 0x00FD, 0x0003, 0x0083, 0x0043, 0x00C3, 0x0023, 0x00A3, 0x0063, 0x00E3, 0x0013, 0x0093, 0x0053, 0x00D3, 0x0033, 0x00B3, 0x0073, 0x00F3, 0x000B, 0x008B, 0x004B, 0x00CB, 0x002B, 0x00AB, 0x006B, 0x00EB, 0x001B, 0x009B, 0x005B, 0x00DB, 0x003B, 0x00BB, 0x007B, 0x00FB, 0x0007, 0x0087, 0x0047, 0x00C7, 0x0027, 0x00A7, 0x0067, 0x00E7, 0x0017, 0x0097, 0x0057, 0x00D7, 0x0037, 0x00B7, 0x0077, 0x00F7, 0x000F, 0x008F, 0x004F, 0x00CF, 0x002F, 0x00AF, 0x006F, 0x00EF, 0x001F, 0x009F, 0x005F, 0x00DF, 0x003F, 0x00BF, 0x007F, 0x00FF };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ushort BitwiseReverse(ushort value) {
        return (ushort)(_lookupBitReverse1[value & 0xFF] | _lookupBitReverse0[value >> 8]);
    }

    #endregion Algorithm


    /// <summary>
    /// Gets current digest.
    /// </summary>
    [Obsolete("Use HashAsUInt16 instead.")]
    public short HashAsInt16 {
        get { return (short)HashAsUInt16; }
    }

    /// <summary>
    /// Gets current digest.
    /// </summary>
    public ushort HashAsUInt16 {
        get {
            if (_reverseOut) {
                return (ushort)(BitwiseReverse(_currDigest) ^ _finalXorValue);
            } else {
                return (ushort)(_currDigest ^ _finalXorValue);
            }
        }
    }


    #region ReciprocalPolynomial

    /// <summary>
    /// Converts polynomial to its reversed reciprocal form.
    /// </summary>
    /// <param name="polynomial">Polynomial.</param>
    public static ushort ToReversedReciprocalPolynomial(ushort polynomial) {
        return unchecked((ushort)((polynomial >> 1) | 0x8000));
    }

    /// <summary>
    /// Converts polynomial from its reversed reciprocal to normal form.
    /// </summary>
    /// <param name="reversedReciprocalPolynomial">Reversed reciprocal polynomial.</param>
    public static ushort FromReversedReciprocalPolynomial(ushort reversedReciprocalPolynomial) {
        return unchecked((ushort)((reversedReciprocalPolynomial << 1) | 0x01));
    }

    #endregion ReciprocalPolynomial

}
