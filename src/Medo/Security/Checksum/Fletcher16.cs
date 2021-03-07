/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Security.Checksum {
    using System.Security.Cryptography;

    /// <summary>
    /// Computes checksum using Fletcher-16 algorithm.
    /// </summary>
    public sealed class Fletcher16 : HashAlgorithm {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Fletcher16() { }


        /// <summary>
        /// Gets hash as 16-bit integer.
        /// </summary>
        public short HashAsInt16 => (short)((_sum2 << 8) | _sum1);


        #region HashAlgorithm

        /// <summary>
        /// Gets the size, in bits, of the computed hash code.
        /// </summary>
        public override int HashSize => 16;


        private int _sum1 = 0;
        private int _sum2 = 0;
        private const int _maximumRunningSum = int.MaxValue / 2; //to avoid modulus every run
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
                _sum1 = 0;
                _sum2 = 0;
                _initializationPending = false;
            }

            for (var i = ibStart; i < (ibStart + cbSize); i++) {
                _sum1 += array[i];
                _sum2 += _sum1;
                if (_sum2 > _maximumRunningSum) {
                    _sum1 %= 255;
                    _sum2 %= 255;
                }
            }

            _sum1 %= 255;
            _sum2 %= 255;
        }

        /// <summary>
        /// Finalizes the hash computation.
        /// </summary>
        /// <returns></returns>
        protected override byte[] HashFinal() {
            return new byte[] { (byte)_sum2, (byte)_sum1 };
        }

        #endregion

    }
}
