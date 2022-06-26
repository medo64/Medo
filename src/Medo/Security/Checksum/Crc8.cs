/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-06-24: Removed default constructor in favor of GetCustom method
//            Added more variants
//2022-01-05: Added more variants
//            Fixed big-endian system operation
//2021-03-06: Refactored for .NET 5
//2008-06-07: Replaced ShiftRight function with right shift (>>) operator
//            Implemented bit reversal via lookup table (http://graphics.stanford.edu/~seander/bithacks.html)
//            Append is not longer returning intermediate digest (performance reasons)
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2
//2008-01-05: Added resources
//2007-10-31: New version

namespace Medo.Security.Checksum;

using System;
using System.Security.Cryptography;

/// <summary>
/// Computes hash using the 8-bit CRC algorithm.
/// The following CRC-8 variants are supported: AUTOSAR, BLUETOOTH, CCITT,
/// CDMA2000, DALLAS, DARC, DVB-S2, GSM-A, GSM-B, HITAG, I-432-1, I-CODE,
/// ITU, LTE, MAXIM, MAXIM-DOW, MIFARE, MIFARE-MAD, NRSC-5, OpenSAFETY,
/// ROHC, SAE-J1850, SMBUS, TECH-3250, and WCDMA2000.
/// </summary>
/// <example>
/// <code>
/// var crc = Crc8.GetOpenSafety();
/// crc.ComputeHash(Encoding.ASCII.GetBytes("Test"));
/// var hashValue = crc.HashAsByte;
/// </code>
/// </example>
public sealed class Crc8 : HashAlgorithm {

    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="polynomial">The polynomial value.</param>
    /// <param name="initialValue">The starting digest value.</param>
    /// <param name="reflectIn">If true, input byte is in the reflected (LSB first) bit order.</param>
    /// <param name="reflectOut">If true, digest is in the reflected (LSB first) bit order.</param>
    /// <param name="finalXorValue">The final XOR value.</param>
    private Crc8(byte polynomial, byte initialValue, bool reflectIn, bool reflectOut, byte finalXorValue) {
        _polynomial = polynomial;
        _initialValue = initialValue;
        _reverseIn = reflectIn ^ BitConverter.IsLittleEndian;
        _reverseOut = reflectOut ^ BitConverter.IsLittleEndian;
        _finalXorValue = finalXorValue;
        ProcessInitialization();
    }


    /// <summary>
    /// Returns a custom CRC-8 variant.
    /// </summary>
    /// <param name="polynomial">The polynomial value.</param>
    /// <param name="initialValue">The starting digest value.</param>
    /// <param name="reflectIn">If true, input byte is in the reflected (LSB first) bit order.</param>
    /// <param name="reflectOut">If true, digest is in the reflected (LSB first) bit order.</param>
    /// <param name="finalXorValue">The final XOR value.</param>
    /// <remarks>
    /// Name                         Polynomial                         Init  Xor   Reflect
    /// ----------------------------------------------------------------------------------------
    /// AUTOSAR                      0x2F (x⁸ + x⁵ + x³ + x² + x + 1)   0xFF  0xFF  -
    /// BLUETOOTH                    0xA7 (x⁸ + x⁷ + x⁵ + x² + x + 1)   0x00  0x00  In/Out
    /// CDMA2000                     0x9B (x⁸ + x⁷ +  x⁴ + x³ + x + 1)  0xFF  0x00  -
    /// DARC                         0x39 (x⁸ + x⁵ + x⁴ + x³ + 1)       0x00  0x00  In/Out
    /// DVB-S2                       0xD5 (x⁸ + x⁷ + x⁶ + x⁴ + x² + 1)  0x00  0x00  -
    /// GSM-A                        0x1D (x⁸ + x⁴ + x³ + 1)            0x00  0x00  -
    /// GSM-B                        0x49 (x⁸ + x⁶ + x³ + 1)            0x00  0xFF  -
    /// HITAG                        0x1D (x⁸ + x⁴ + x³ + 1)            0xFF  0x00  -
    /// I-432-1 / ITU / CCITT / ATM  0x07 (x⁸ + x² + x + 1)             0x00  0x55  -
    /// I-CODE                       0x1D (x⁸ + x⁴ + x³ + 1)            0xBF  0x00  -
    /// LTE                          0x9B (x⁸ + x⁷ +  x⁴ + x³ + x + 1)  0x00  0x00  -
    /// MAXIM-DOW / MAXIM            0x31 (x⁸ + x⁵ + x⁴ + 1)            0x00  0x00  In/Out
    /// MIFARE-MAD / MIFARE          0x1D (x⁸ + x⁴ + x³ + 1)            0xE3  0x00  -
    /// NRSC-5                       0x31 (x⁸ + x⁵ + x⁴ + 1)            0xFF  0x00  -
    /// OPENSAFETY / C2              0x2F (x⁸ + x⁵ + x³ + x² + x + 1)   0x00  0x00  -
    /// ROHC                         0x07 (x⁸ + x² + x + 1)             0xFF  0x00  In/Out
    /// SAE-J1850                    0x1D (x⁸ + x⁴ + x³ + 1)            0xFF  0xFF  -
    /// SMBUS                        0x07 (x⁸ + x² + x + 1)             0x00  0x00  -
    /// TECH-3250                    0x1D (x⁸ + x⁴ + x³ + 1)            0xFF  0x00  In/Out
    /// WCDMA2000                    0x9B (x⁸ + x⁷ +  x⁴ + x³ + x + 1)  0x00  0x00  In/Out
    /// 
    /// See also:
    /// - https://reveng.sourceforge.io/crc-catalogue/1-15.htm
    /// - https://users.ece.cmu.edu/~koopman/crc/index.html
    /// </remarks>
    public static Crc8 GetCustom(byte polynomial, byte initialValue, bool reflectIn, bool reflectOut, byte finalXorValue) {
        return new Crc8(polynomial, initialValue, reflectIn, reflectOut, finalXorValue);
    }


    /// <summary>
    /// Returns CRC-8/AUTOSAR variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁵ + x³ + x² + x + 1 (0x2F)
    /// Initial value: 0xFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFF
    /// </remarks>
    public static Crc8 GetAutosar() {
        return new Crc8(0x2F, 0xFF, false, false, 0xFF);
    }

    /// <summary>
    /// Returns CRC-8/BLUETOOTH variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁷ + x⁵ + x² + x + 1 (0xA7)
    /// Initial value: 0x00
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetBluetooth() {
        return new Crc8(0xA7, 0x00, true, true, 0x00);
    }

    /// <summary>
    /// Returns CRC-8/CDMA2000 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁷ +  x⁴ + x³ + x + 1 (0x9B)
    /// Initial value: 0xFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetCdma2000() {
        return new Crc8(0x9B, 0xFF, false, false, 0x00);
    }

    /// <summary>
    /// Returns CRC-8/DARC variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁵ + x⁴ + x³ + 1 (0x39)
    /// Initial value: 0x00
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetDarc() {
        return new Crc8(0x39, 0x00, true, true, 0x00);
    }

    /// <summary>
    /// Returns CRC-8/DVB-S2 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁷ + x⁶ + x⁴ + x² + 1 (0xD5)
    /// Initial value: 0x00
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetDvbS2() {
        return new Crc8(0xD5, 0x00, false, false, 0x00);
    }

    /// <summary>
    /// Returns CRC-8/GSM-A variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁴ + x³ + 1 (0x1D)
    /// Initial value: 0x00
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetGsmA() {
        return new Crc8(0x1D, 0x00, false, false, 0x00);
    }

    /// <summary>
    /// Returns CRC-8/GSM-B variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁶ + x³ + 1 (0x49)
    /// Initial value: 0x00
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFF
    /// </remarks>
    public static Crc8 GetGsmB() {
        return new Crc8(0x49, 0x00, false, false, 0xFF);
    }

    /// <summary>
    /// Returns CRC-8/HITAG variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁴ + x³ + 1 (0x1D)
    /// Initial value: 0xFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetHitag() {
        return new Crc8(0x1D, 0xFF, false, false, 0x00);
    }

    /// <summary>
    /// Returns CRC-8/I-432-1 / CRC-8/ITU / CRC-8/CCITT / CRC-8/ATM variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x² + x + 1 (0x07)
    /// Initial value: 0x00
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x55
    /// </remarks>
    public static Crc8 GetI4321() {
        return new Crc8(0x07, 0x00, false, false, 0x55);
    }

    /// <summary>
    /// Returns CRC-8/CCITT variant.
    /// More widely known as CRC-8/I-432-1.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x² + x + 1 (0x07)
    /// Initial value: 0x00
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x55
    /// </remarks>
    public static Crc8 GetCcitt() {
        return GetI4321();
    }

    /// <summary>
    /// Returns CRC-8/ITU variant.
    /// More widely known as CRC-8/I-432-1.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x² + x + 1 (0x07)
    /// Initial value: 0x00
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x55
    /// </remarks>
    public static Crc8 GetItu() {
        return GetI4321();
    }

    /// <summary>
    /// Returns CRC-8/ATM variant.
    /// More widely known as CRC-8/I-432-1.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x² + x + 1 (0x07)
    /// Initial value: 0x00
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x55
    /// </remarks>
    public static Crc8 GetAtm() {
        return GetI4321();
    }

    /// <summary>
    /// Returns CRC-8/I-CODE variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁴ + x³ + 1 (0x1D)
    /// Initial value: 0xFD
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetICode() {
        return new Crc8(0x1D, 0xBF, false, false, 0x00);
    }

    /// <summary>
    /// Returns the CRC-8/LTE variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁷ +  x⁴ + x³ + x + 1 (0x9B)
    /// Initial value: 0x00
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetLte() {
        return new Crc8(0x9B, 0x00, false, false, 0x00);
    }

    /// <summary>
    /// Returns CRC-8/MAXIM-DOW / CRC-8/MAXIM variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁵ + x⁴ + 1 (0x31)
    /// Initial value: 0x00
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetMaximDow() {
        return new Crc8(0x31, 0x00, true, true, 0x00);
    }

    /// <summary>
    /// Returns CRC-8/DALLAS variant.
    /// More widely known as CRC-8/MAXIM-DOW.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁵ + x⁴ + 1 (0x31)
    /// Initial value: 0x00
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetDallas() {
        return GetMaximDow();
    }

    /// <summary>
    /// Returns CRC-8/MAXIM variant.
    /// More widely known as CRC-8/MAXIM-DOW.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁵ + x⁴ + 1 (0x31)
    /// Initial value: 0x00
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetMaxim() {
        return GetMaximDow();
    }

    /// <summary>
    /// Returns the CRC-8/MIFARE-MAD variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁴ + x³ + 1 (0x1D)
    /// Initial value: 0xE3
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetMifareMad() {
        return new Crc8(0x1D, 0xE3, false, false, 0x00);
    }

    /// <summary>
    /// Returns the CRC-8/MIFARE variant.
    /// More widely known as CRC-8/MIFARE-MAD.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁴ + x³ + 1 (0x1D)
    /// Initial value: 0xE3
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetMifare() {
        return GetMifareMad();
    }

    /// <summary>
    /// Returns the CRC-8/NRSC-5 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁵ + x⁴ + 1 (0x31)
    /// Initial value: 0xFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetNrsc5() {
        return new Crc8(0x31, 0xFF, false, false, 0x00);
    }

    /// <summary>
    /// Returns the CRC-8/OPENSAFETY CRC-8 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁵ + x³ + x² + x + 1 (0x2F)
    /// Initial value: 0x00
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00
    ///
    /// On microcontrollers it could be implemented as:
    /// <code>
    /// uint8_t crc8(uint8_t* data, uint8_t length) {
    ///     uint8_t crc = 0;
    ///     while (length--) {
    ///         crc ^= *data++;
    ///         for (uint8_t i = 0; i < 8; i++) {
    ///             if (crc & 0x80) {
    ///                 crc = (uint8_t)(crc << 1u) ^ 0x2F;
    ///             } else {
    ///                 crc <<= 1u;
    ///             }
    ///         }
    ///     }
    ///     return crc;
    /// }
    /// </code>
    /// </remarks>
    public static Crc8 GetOpenSafety() {
        return new Crc8(0x2F, 0x00, false, false, 0x00);
    }

    /// <summary>
    /// Returns the CRC-8/C2 CRC-8 variant.
    /// More widely known as CRC-8/OpenSAFETY.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁵ + x³ + x² + x + 1 (0x2F)
    /// Initial value: 0x00
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetC2() {
        return GetOpenSafety();
    }

    /// <summary>
    /// Returns CRC-8/ROHC variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x² + x + 1 (0x07)
    /// Initial value: 0xFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetRohc() {
        return new Crc8(0x07, 0xFF, true, true, 0x00);
    }

    /// <summary>
    /// Returns CRC-8/SAE-J1850 CRC-8 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁴ + x³ + 1 (0x1D)
    /// Initial value: 0xFF
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0xFF
    /// </remarks>
    public static Crc8 GetSaeJ1850() {
        return new Crc8(0x1D, 0xFF, false, false, 0xFF);
    }

    /// <summary>
    /// Returns CRC-8/SMBUS variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x² + x + 1 (0x07)
    /// Initial value: 0x00
    /// Reflect In: No
    /// Reflect Out: No
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetSMBus() {
        return new Crc8(0x07, 0x00, false, false, 0x00);
    }

    /// <summary>
    /// Returns CRC-8/TECH-3250 variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁴ + x³ + 1 (0x1D)
    /// Initial value: 0xFF
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetTech3250() {
        return new Crc8(0x1D, 0xFF, true, true, 0x00);
    }

    /// <summary>
    /// Returns CRC-8/WCDMA variant.
    /// </summary>
    /// <remarks>
    /// Polynom: x⁸ + x⁷ +  x⁴ + x³ + x + 1 (0x9B)
    /// Initial value: 0x00
    /// Reflect In: Yes
    /// Reflect Out: Yes
    /// Output XOR: 0x00
    /// </remarks>
    public static Crc8 GetWcdma2000() {
        return new Crc8(0x9B, 0x00, true, true, 0x00);
    }


    #region HashAlgorithm

    /// <summary>
    /// Gets the size, in bits, of the computed hash code.
    /// </summary>
    public override int HashSize => 8;

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
    /// <returns></returns>
    protected override byte[] HashFinal() {
        return new byte[] { HashAsByte };
    }

    #endregion HashAlgorithm


    #region Algorithm

    private readonly byte _polynomial;
    private readonly byte _initialValue;
    private readonly byte _finalXorValue;
    private readonly bool _reverseIn;
    private readonly bool _reverseOut;

    private void ProcessInitialization() {
        _currDigest = _initialValue;
        var polynomialR = BitwiseReverse(_polynomial);

        for (int i = 0; i < 256; i++) {
            byte crcValue = (byte)i;

            for (int j = 1; j <= 8; j++) {
                if ((crcValue & 1) == 1) {
                    crcValue = (byte)((crcValue >> 1) ^ polynomialR);
                } else {
                    crcValue >>= 1;
                }
            }

            _lookup[i] = crcValue;
        }
    }

    private readonly byte[] _lookup = new byte[256];
    private byte _currDigest;

    private void ProcessBytes(byte[] bytes, int index, int count) {
        for (var i = index; i < (index + count); i++) {
            if (_reverseIn) {
                _currDigest = _lookup[_currDigest ^ BitwiseReverse(bytes[i])];
            } else {
                _currDigest = _lookup[_currDigest ^ bytes[i]];
            }
        }
    }

    private static readonly byte[] _lookupBitReverse = { 0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0, 0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8, 0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4, 0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4, 0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC, 0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2, 0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2, 0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA, 0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6, 0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE, 0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE, 0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1, 0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9, 0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5, 0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD, 0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3, 0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3, 0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB, 0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7, 0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF };

    internal static byte BitwiseReverse(byte value) {
        return _lookupBitReverse[value];
    }

    #endregion Algorithm


    /// <summary>
    /// Gets current digest.
    /// </summary>
    public byte HashAsByte {
        get {
            if (_reverseOut) {
                return (byte)(BitwiseReverse(_currDigest) ^ _finalXorValue);
            } else {
                return (byte)(_currDigest ^ _finalXorValue);
            }
        }
    }


    #region ReciprocalPolynomial

    /// <summary>
    /// Converts polynomial to its reversed reciprocal form.
    /// </summary>
    /// <param name="polynomial">Polynomial.</param>
    /// <returns></returns>
    public static byte ToReversedReciprocalPolynomial(byte polynomial) {
        return (byte)((polynomial >> 1) | 0x80);
    }

    /// <summary>
    /// Converts polynomial from its reversed reciprocal to normal form.
    /// </summary>
    /// <param name="reversedReciprocalPolynomial">Reversed reciprocal polynomial.</param>
    /// <returns></returns>
    public static byte FromReversedReciprocalPolynomial(byte reversedReciprocalPolynomial) {
        return (byte)((reversedReciprocalPolynomial << 1) | 0x01);
    }


    #endregion ReciprocalPolynomial

}
