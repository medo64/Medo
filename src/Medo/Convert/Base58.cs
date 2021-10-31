/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2019-10-04: Refactored for .NET 5
//2019-03-09: Initial version

namespace Medo.Convert {
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Text;

    /// <summary>
    /// Base58 encoder/decoder with leading-zero preservation.
    /// </summary>
    public static class Base58 {

        private static readonly char[] Map = new char[] {
            '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',
            'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W',
            'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

        private static readonly Encoding Utf8 = new UTF8Encoding(false);


        #region ToString

        /// <summary>
        /// Returns Base58 string.
        /// </summary>
        /// <param name="bytes">Bytes to convert.</param>
        /// <exception cref="NullReferenceException">Bytes cannot be null.</exception>
        public static string ToString(byte[] bytes) {
            if (bytes == null) { throw new ArgumentNullException(nameof(bytes), "Bytes cannot be null."); }
            if (TryToString(bytes, out var result)) { return result; }
            throw new FormatException("Cannot convert.");
        }

        /// <summary>
        /// Returns Base58 string.
        /// </summary>
        /// <param name="text">UTF-8 text to convert.</param>
        /// <exception cref="NullReferenceException">Text cannot be null.</exception>
        public static string ToString(string text) {
            if (text == null) { throw new ArgumentNullException(nameof(text), "Text cannot be null."); }
            return ToString(Utf8.GetBytes(text));
        }


        /// <summary>
        /// Returns true of conversion succeeded.
        /// </summary>
        /// <param name="bytes">Bytes to convert.</param>
        /// <param name="result">Converstion result as Base58 string.</param>
        /// <exception cref="NullReferenceException">Bytes cannot be null.</exception>
        public static bool TryToString(byte[] bytes, out string result) {
            if (bytes == null) { throw new ArgumentNullException(nameof(bytes), "Bytes cannot be null."); }

            if (bytes.Length == 0) { //don't bother if there is nothing in array
                result = "";
                return true;
            }

            BigInteger input;
            var buffer = new byte[bytes.Length + 1]; //extra byte is leading 0
            try {
                Buffer.BlockCopy(bytes, 0, buffer, 1, bytes.Length);
                if (BitConverter.IsLittleEndian) { Array.Reverse(buffer); }
                input = new BigInteger(buffer);
            } finally {
                if (buffer != null) { Array.Clear(buffer, 0, buffer.Length); }
            }

            var remainders = new List<int>();
            while (input > 0) {
                var remainder = input % 58;
                remainders.Add((int)remainder);
                input /= 58;
            }

            //preserver leading zeros
            foreach (var b in bytes) {
                if (b == 0) { remainders.Add(0); } else { break; }
            }

            remainders.Reverse();

            var sbOutput = new StringBuilder();
            foreach (var remainder in remainders) {
                sbOutput.Append(Map[remainder]);
            }
            result = sbOutput.ToString();

            return true;
        }

        /// <summary>
        /// Returns true of conversion succeeded.
        /// </summary>
        /// <param name="text">Text to convert.</param>
        /// <param name="result">Converstion result as Base58 string.</param>
        /// <exception cref="NullReferenceException">Text cannot be null.</exception>
        public static bool TryToString(string text, out string result) {
            if (text == null) { throw new ArgumentNullException(nameof(text), "Text cannot be null."); }
            return TryToString(Utf8.GetBytes(text), out result);
        }

        #endregion ToString


        #region AsBytes

        /// <summary>
        /// Returns bytes based on their Base58 encoding.
        /// </summary>
        /// <param name="base58">Base58 encoded string.</param>
        /// <exception cref="NullReferenceException">Base58 string cannot be null.</exception>
        /// <exception cref="FormatException">Unknown character.</exception>
        public static byte[] AsBytes(string base58) {
            if (base58 == null) { throw new ArgumentNullException(nameof(base58), "Base58 string cannot be null."); }
            if (TryAsBytes(base58, out var result)) { return result; }
            throw new FormatException("Cannot convert.");
        }

        /// <summary>
        /// Returns true of conversion succeeded.
        /// </summary>
        /// <param name="base58">Base58 encoded string.</param>
        /// <param name="result">Conversion result as bytes.</param>
        /// <exception cref="NullReferenceException">Base58 string cannot be null.</exception>
        /// <exception cref="FormatException">Unknown character.</exception>
        public static bool TryAsBytes(string base58, out byte[] result) {
            if (base58 == null) { throw new ArgumentNullException(nameof(base58), "Base58 string cannot be null."); }

            if (string.IsNullOrEmpty(base58)) {
                result = Array.Empty<byte>();
                return true;
            }

            var inStarting = true;
            var startingZeros = 0;
            var indices = new List<int>();
            foreach (var c in base58) {
                var index = Array.IndexOf(Map, c);
                if (index >= 0) {
                    if (inStarting && (index == 0)) {
                        startingZeros += 1;
                    } else {
                        inStarting = false;
                    }
                    indices.Add(index);
                } else {
                    throw new FormatException("Unknown character.");
                }
            }

            var output = new BigInteger();
            foreach (var index in indices) {
                output *= 58;
                output += index;
            }
            var outputBytes = output.ToByteArray();

            if (BitConverter.IsLittleEndian) { Array.Reverse(outputBytes); }

            try {
                var extraZeros = (outputBytes[0] == 0x00) ? startingZeros - 1 : startingZeros;
                var buffer = new byte[outputBytes.Length + extraZeros];
                if (extraZeros >= 0) {
                    Buffer.BlockCopy(outputBytes, 0, buffer, extraZeros, outputBytes.Length);
                } else {
                    Buffer.BlockCopy(outputBytes, -extraZeros, buffer, 0, buffer.Length);
                }
                result = buffer;
                return true;
            } finally {
                Array.Clear(outputBytes, 0, outputBytes.Length);
            }
        }

        #endregion AsBytes

        #region AsString

        /// <summary>
        /// Returns UTF-8 string based on it's Base58 encoding.
        /// </summary>
        /// <param name="base58">Base58 encoded string.</param>
        /// <exception cref="NullReferenceException">Base58 string cannot be null.</exception>
        /// <exception cref="FormatException">Unknown character.</exception>
        public static string? AsString(string base58) {
            if (base58 == null) { throw new ArgumentNullException(nameof(base58), "Base58 string cannot be null."); }
            if (TryAsString(base58, out var result)) { return result; }
            throw new FormatException("Cannot convert.");
        }

        /// <summary>
        /// Returns true of conversion succeeded.
        /// </summary>
        /// <param name="base58">Base58 encoded string.</param>
        /// <param name="result">Conversion result as UTF-8 string.</param>
        /// <exception cref="NullReferenceException">Base58 string cannot be null.</exception>
        /// <exception cref="FormatException">Unknown character.</exception>
        public static bool TryAsString(string base58, out string? result) {
            if (base58 == null) { throw new ArgumentNullException(nameof(base58), "Base58 string cannot be null."); }
            if (TryAsBytes(base58, out var bytes)) {
                result = Utf8.GetString(bytes);
                return true;
            } else {
                result = null;
                return false;
            }
        }

        #endregion AsString

    }
}
