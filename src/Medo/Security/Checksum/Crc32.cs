/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

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

namespace Medo.Security.Checksum {
    using System;
    using System.Security.Cryptography;

    /// <summary>
    /// Computes hash using 32-bit CRC algorithm.
    /// </summary>
    public sealed class Crc32 : HashAlgorithm {

        /// <summary>
        /// Creates new instance using standard IEEE 802.3 implementation.
        /// </summary>
        public Crc32()
            : this(0x04C11DB7, unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF), true, true) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="polynomial">Polynomial value.</param>
        /// <param name="initialValue">Starting digest.</param>
        /// <param name="finalXorValue">Final XOR value.</param>
        /// <param name="reflectIn">If true, input byte is in reflected (LSB first) bit order.</param>
        /// <param name="reflectOut">If true, digest is in reflected (LSB first) bit order.</param>
        /// <remarks>
        /// Name        Poly        Init        XorOut      RefIn  RefOut
        /// -------------------------------------------------------------
        /// Castagnoli  0x1EDC6F41  0xFFFFFFFF  0xFFFFFFFF  true   true
        /// IEEE 802.3  0x04C11DB7  0xFFFFFFFF  0xFFFFFFFF  true   true
        /// JamCRC      0x04C11DB7  0xFFFFFFFF  0x00000000  true   true
        /// POSIX       0x04C11DB7  0x00000000  0xFFFFFFFF  false  false
        /// XFER        0x000000AF  0x00000000  0x00000000  false  false
        /// </remarks>
        public Crc32(int polynomial, int initialValue, int finalXorValue, bool reflectIn, bool reflectOut) {
            _polynomial = (uint)polynomial;
            _initialValue = (uint)initialValue;
            _finalXorValue = (uint)finalXorValue;
            _reverseIn = !reflectIn;
            _reverseOut = !reflectOut;
            ProcessInitialization();
        }


        /// <summary>
        /// Returns IEEE 802.3 implementation.
        /// </summary>
        public static Crc32 GetCastagnoli() {  // 0xE3069283
            return new Crc32(0x1EDC6F41, unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF), true, true);
        }

        /// <summary>
        /// Returns IEEE 802.3 implementation.
        /// </summary>
        public static Crc32 GetIeee() {  // 0xCBF43926
            return new Crc32(0x04C11DB7, unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF), true, true);
        }

        /// <summary>
        /// Returns JamCRC implementation.
        /// </summary>
        public static Crc32 GetJam() {  // 0x340BC6D9
            return new Crc32(0x04C11DB7, unchecked((int)0xFFFFFFFF), 0x00000000, true, true);
        }

        /// <summary>
        /// Returns POSIX implementation.
        /// </summary>
        public static Crc32 GetPosix() {  // 0x765E7680
            return new Crc32(0x04C11DB7, 0x00000000, unchecked((int)0xFFFFFFFF), false, false);
        }

        /// <summary>
        /// Returns XFER implementation.
        /// </summary>
        public static Crc32 GetXfer() {  // 0xBD0BE338
            return new Crc32(0x000000AF, 0x00000000, 0x00000000, false, false);
        }


        #region HashAlgorithm

        /// <summary>
        /// Gets the size, in bits, of the computed hash code.
        /// </summary>
        public override int HashSize => 32;

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
            var digestBytes = BitConverter.GetBytes(HashAsInt32);
            if (BitConverter.IsLittleEndian) { Array.Reverse(digestBytes); }
            return digestBytes;
        }

        #endregion HashAlgorithm


        #region Algorithm

        private readonly uint _polynomial;
        private readonly uint _initialValue;
        private readonly uint _finalXorValue;
        private readonly bool _reverseIn;
        private readonly bool _reverseOut;

        private void ProcessInitialization() {
            _currDigest = _initialValue;
            var polynomialR = BitwiseReverse(_polynomial);
            for (uint i = 0; i < 256; i++) {
                var crcValue = i;

                for (int j = 1; j <= 8; j++) {
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

        private void ProcessBytes(byte[] bytes, int index, int count) {
            for (var i = index; i < (index + count); i++) {
                if (_reverseIn) {
                    _currDigest = (_currDigest >> 8) ^ _lookup[(int)((_currDigest & 0xff) ^ _lookupBitReverse[bytes[i]])];
                } else {
                    _currDigest = (_currDigest >> 8) ^ _lookup[(int)((_currDigest & 0xff) ^ (bytes[i]))];
                }
            }
        }

        private static readonly byte[] _lookupBitReverse = { 0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0, 0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8, 0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4, 0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4, 0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC, 0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2, 0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2, 0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA, 0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6, 0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE, 0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE, 0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1, 0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9, 0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5, 0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD, 0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3, 0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3, 0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB, 0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7, 0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF };

        internal static uint BitwiseReverse(uint value) {
            return (uint)((_lookupBitReverse[value & 0xff] << 24) | (_lookupBitReverse[(value >> 8) & 0xff] << 16) | (_lookupBitReverse[(value >> 16) & 0xff] << 8) | (_lookupBitReverse[(value >> 24) & 0xff]));
        }

        #endregion Algorithm


        /// <summary>
        /// Gets current digest.
        /// </summary>
        public int HashAsInt32 {
            get {
                if (_reverseOut) {
                    return (int)(BitwiseReverse(_currDigest) ^ _finalXorValue);
                } else {
                    return (int)(_currDigest ^ _finalXorValue);
                }
            }
        }

    }
}
