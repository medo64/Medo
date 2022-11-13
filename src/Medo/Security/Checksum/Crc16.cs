/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-09-27: Moved to Medo.IO.Hashing
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

namespace Medo.Security.Checksum;

using System;
using System.Security.Cryptography;

/// <summary>
/// Computes hash using 16-bit CRC algorithm.
/// The following CRC-16 variants are supported: ACORN, ARC, AUG-CCITT,
/// AUTOSAR, BUYPASS, CDMA2000, CCITT, CCITT-FALSE, CCITT-TRUE, CMS, DARC,
/// DDS-110, DECT-R, DECT-X, DNP, EN-13757, EPC, EPC-C1G2, GENIBUS, GSM,
/// I-CODE, IBM-3740, ISO-HDLD, IBM-SDLC, IEC-61158-2, IEEE 802.3,
/// ISO-IEC-14443-3-A, ISO-IEC-14443-3-B, KERMIT, LHA, LJ1200, LTE, MAXIM,
/// MAXIM-DOW, MCRF4XX, MODBUS, NRSC-5, OPENSAFETY-A, OPENSAFETY-B,
/// PROFIBUS, RIELLO, SPI-FUJITSU, T10-DIF, TELEDISK, TMS37157, UMTS, USB,
/// V-41-LSB, V-41-MSB, VERIFONE, X-25, XMODEM, and ZMODEM.
/// </summary>
/// <example>
/// <code>
/// var crc = Crc16.GetArc();
/// crc.ComputeHash(Encoding.ASCII.GetBytes("Test"));
/// var hashValue = crc.HashAsByte;
/// </code>
/// </example>
#if NET7_0_OR_GREATER
[Obsolete("Use Medo.IO.Hashing.Crc16 instead")]
#endif
public sealed class Crc16 : HashAlgorithm {

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
    [Obsolete("Use GetCustom instead")]
    public Crc16(short polynomial, short initialValue, bool reflectIn, bool reflectOut, short finalXorValue)
        : this(unchecked((ushort)polynomial), unchecked((ushort)initialValue), reflectIn, reflectOut, unchecked((ushort)finalXorValue)) {
    }

    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="polynomial">Polynomial value.</param>
    /// <param name="initialValue">Starting digest.</param>
    /// <param name="reflectIn">If true, input byte is in reflected (LSB first) bit order.</param>
    /// <param name="reflectOut">If true, digest is in reflected (LSB first) bit order.</param>
    /// <param name="finalXorValue">Final XOR value.</param>
    private Crc16(ushort polynomial, ushort initialValue, bool reflectIn, bool reflectOut, ushort finalXorValue) {
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
    /// </remarks>
    public static Crc16 GetCustom(short polynomial, short initialValue, bool reflectIn, bool reflectOut, short finalXorValue) {
        return new Crc16((ushort)polynomial, (ushort)initialValue, reflectIn, reflectOut, (ushort)finalXorValue);
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
        return new Crc16((ushort)0x8005, 0x0000, true, true, 0x0000);
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
        return new Crc16((ushort)0xC867, 0xFFFF, false, false, 0x0000);
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
        return new Crc16((ushort)0x8005, 0xFFFF, false, false, 0x0000);
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
        return new Crc16((ushort)0x8005, 0xB001, false, false, 0x0000);
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
        return new Crc16((ushort)0x0589, 0x0000, false, false, 0x0001);
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
        return new Crc16((ushort)0x0589, 0x0000, false, false, 0x0000);
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
        return new Crc16((ushort)0x3D65, 0x0000, true, true, 0xFFFF);
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
        return new Crc16((ushort)0x3D65, 0x0000, false, false, 0xFFFF);
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
        return new Crc16((ushort)0x1021, 0xFFFF, false, false, 0xFFFF);
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
        return new Crc16((ushort)0x1021, 0xFFFF, false, false, 0x0000);
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
        return new Crc16((ushort)0x1021, 0x0000, false, false, 0xFFFF);
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
        return new Crc16((ushort)0x1021, 0xFFFF, false, false, 0x0000);
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
        return new Crc16((ushort)0x1021, 0xFFFF, true, true, 0xFFFF);
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
        return new Crc16((ushort)0x1021, 0xC6C6, true, true, 0x0000);
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
        return new Crc16((ushort)0x1021, 0x0000, true, true, 0x0000);
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
        return new Crc16((ushort)0x6F63, 0x0000, false, false, 0x0000);
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
        return new Crc16((ushort)0x8005, 0x0000, true, true, 0xFFFF);
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
        return new Crc16((ushort)0x1021, 0xFFFF, true, true, 0x0000);
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
        return new Crc16((ushort)0x8005, 0xFFFF, true, true, 0x0000);
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
        return new Crc16((ushort)0x080B, 0xFFFF, true, true, 0x0000);
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
        return new Crc16((ushort)0x5935, 0x0000, false, false, 0x0000);
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
        return new Crc16((ushort)0x755B, 0x0000, false, false, 0x0000);
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
        return new Crc16((ushort)0x1DCF, 0xFFFF, false, false, 0xFFFF);
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
        return new Crc16((ushort)0x1021, 0x554D, true, true, 0x0000);
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
        return new Crc16((ushort)0x1021, 0xF0B8, false, false, 0x0000);
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
        return new Crc16((ushort)0x8BB7, 0x0000, false, false, 0x0000);
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
        return new Crc16((ushort)0xA097, 0x0000, false, false, 0x0000);
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
        return new Crc16((ushort)0x1021, 0x3791, true, true, 0x0000);
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
        return new Crc16((ushort)0x8005, 0x0000, false, false, 0x0000);
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
        return new Crc16((ushort)0x8005, 0xFFFF, true, true, 0xFFFF);
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
        return new Crc16((ushort)0x1021, 0x0000, false, false, 0x0000);
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


    #region HashAlgorithm

    /// <summary>
    /// Gets the size, in bits, of the computed hash code.
    /// </summary>
    public override int HashSize => 16;

    private bool _initializationPending;

    /// <summary>
    /// Initializes an instance.
    /// </summary>
    public override void Initialize() {
        _initializationPending = true; //to avoid base class' HashFinal call after ComputeHash clear HashAsInt16.
    }

    /// <summary>
    /// Computes the hash over the data.
    /// </summary>
    /// <param name="array">The input data.</param>
    /// <param name="ibStart">The offset into the byte array from which to begin using data.</param>
    /// <param name="cbSize">The number of bytes in the array to use as data.</param>
    protected override void HashCore(byte[] array, int ibStart, int cbSize) {
        if (_initializationPending) {
            ProcessInitialization();
            _initializationPending = false;
        }

        ProcessBytes(array, ibStart, cbSize);
    }

    /// <summary>
    /// Finalizes the hash computation.
    /// </summary>
    protected override byte[] HashFinal() {
        var digestBytes = BitConverter.GetBytes(HashAsInt16);
        if (BitConverter.IsLittleEndian) { Array.Reverse(digestBytes); }
        return digestBytes;
    }

    #endregion HashAlgorithm


    #region Algorithm

    private readonly ushort _polynomial;
    private readonly ushort _initialValue;
    private readonly ushort _finalXorValue;
    private readonly bool _reverseIn;
    private readonly bool _reverseOut;

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

    private void ProcessBytes(byte[] bytes, int index, int count) {
        for (var i = index; i < (index + count); i++) {
            if (_reverseIn) {
                _currDigest = (ushort)((_currDigest >> 8) ^ _lookup[(_currDigest & 0xff) ^ _lookupBitReverse[bytes[i]]]);
            } else {
                _currDigest = (ushort)((_currDigest >> 8) ^ _lookup[(_currDigest & 0xff) ^ bytes[i]]);
            }
        }
    }

    private static readonly byte[] _lookupBitReverse = { 0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0, 0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8, 0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4, 0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4, 0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC, 0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2, 0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2, 0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA, 0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6, 0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE, 0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE, 0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1, 0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9, 0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5, 0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD, 0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3, 0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3, 0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB, 0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7, 0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF };

    internal static ushort BitwiseReverse(ushort value) {
        return (ushort)((_lookupBitReverse[value & 0xff] << 8) | (_lookupBitReverse[value >> 8]));
    }

    #endregion Algorithm


    /// <summary>
    /// Gets current digest.
    /// </summary>
    public short HashAsInt16 {
        get {
            if (_reverseOut) {
                return (short)(BitwiseReverse(_currDigest) ^ _finalXorValue);
            } else {
                return (short)(_currDigest ^ _finalXorValue);
            }
        }
    }


    #region ReciprocalPolynomial

    /// <summary>
    /// Converts polynomial to its reversed reciprocal form.
    /// </summary>
    /// <param name="polynomial">Polynomial.</param>
    public static short ToReversedReciprocalPolynomial(short polynomial) {
        return unchecked((short)((polynomial >> 1) | 0x8000));
    }

    /// <summary>
    /// Converts polynomial to its reversed reciprocal form.
    /// </summary>
    /// <param name="polynomial">Polynomial.</param>
    /// <exception cref="ArgumentOutOfRangeException">Value must be between 0x0000 and 0xFFFF.</exception>
    public static short ToReversedReciprocalPolynomial(int polynomial) {
        if (polynomial is < 0 or > ushort.MaxValue) { throw new ArgumentOutOfRangeException(nameof(polynomial), "Value must be between 0x0000 and 0xFFFF."); }
        return unchecked(ToReversedReciprocalPolynomial((short)polynomial));
    }

    /// <summary>
    /// Converts polynomial from its reversed reciprocal to normal form.
    /// </summary>
    /// <param name="reversedReciprocalPolynomial">Reversed reciprocal polynomial.</param>
    public static short FromReversedReciprocalPolynomial(short reversedReciprocalPolynomial) {
        return unchecked((short)((reversedReciprocalPolynomial << 1) | 0x01));
    }

    /// <summary>
    /// Converts polynomial from its reversed reciprocal to normal form.
    /// </summary>
    /// <param name="reversedReciprocalPolynomial">Reversed reciprocal polynomial.</param>
    /// <exception cref="ArgumentOutOfRangeException">Value must be between 0x0000 and 0xFFFF.</exception>
    public static short FromReversedReciprocalPolynomial(int reversedReciprocalPolynomial) {
        if (reversedReciprocalPolynomial is < 0 or > ushort.MaxValue) { throw new ArgumentOutOfRangeException(nameof(reversedReciprocalPolynomial), "Value must be between 0x0000 and 0xFFFF."); }
        return unchecked(FromReversedReciprocalPolynomial((short)reversedReciprocalPolynomial));
    }

    #endregion ReciprocalPolynomial

}
