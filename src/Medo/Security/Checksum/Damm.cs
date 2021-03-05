/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Security.Checksum {

    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Computes checksum using Damm's algorithm.
    /// This algorithm allows detection of all single-digit errors and all adjancent transposition errors.
    /// Additionally, it will detect all phonetic errors associated with the English language (e.g. 13 to 30, 14 to 40, etc.)
    /// Only number 0 to 9 are supported.
    /// </summary>
    /// <remarks>
    /// https://en.wikipedia.org/wiki/Damm_algorithm
    /// </remarks>
    public sealed class Damm : HashAlgorithm {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Damm() { }


        #region Helpers

        /// <summary>
        /// Gets hash as number between 0 and 9.
        /// </summary>
        public int HashAsNumber { get; private set; } = 0;

        /// <summary>
        /// Gets hash as char.
        /// </summary>
        public char HashAsChar => (char)(0x30 + HashAsNumber);


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

            using var checksum = new Damm();
            checksum.ComputeHash(Encoding.ASCII.GetBytes(digits));
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
            using var checksum = new Damm();
            checksum.ComputeHash(Encoding.ASCII.GetBytes(digits));
            return (hash == checksum.HashAsChar);
        }

        #endregion


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


        #region HashAlgorithm

        /// <summary>
        /// Gets the size, in bits, of the computed hash code.
        /// </summary>
        public override int HashSize => 8;


        /// <summary>
        /// Initializes an instance.
        /// </summary>
        public override void Initialize() {
        }

        /// <summary>
        /// Computes the hash over the data.
        /// </summary>
        /// <param name="array">The input data.</param>
        /// <param name="ibStart">The offset into the byte array from which to begin using data.</param>
        /// <param name="cbSize">The number of bytes in the array to use as data.</param>
        /// <exception cref="ArgumentOutOfRangeException">Only numbers 0 to 9 are allowed.</exception>
        protected override void HashCore(byte[] array, int ibStart, int cbSize) {
            for (var i = ibStart; i < (ibStart + cbSize); i++) {
                var b = array[i];
                if ((b < 0x30) || (b > 0x39)) { throw new ArgumentOutOfRangeException(nameof(array), "Only numbers 0 to 9 are allowed."); }
                var row = HashAsNumber;
                var col = b - 0x30;
                HashAsNumber = AntisymmetricQuasigroup[row, col];
            }
        }

        /// <summary>
        /// Finalizes the hash computation.
        /// </summary>
        /// <returns></returns>
        protected override byte[] HashFinal() {
            return new byte[] { (byte)(0x30 + HashAsNumber) };
        }

        #endregion

    }
}
