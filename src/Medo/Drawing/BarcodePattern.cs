/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-10-31: Refactored for .NET 5
//            Removed System.Drawing dependency
//2008-12-09: Changed Code 128 pattern 27 from 311212 to 312212
//2008-11-05: Refactoring (Microsoft.Maintainability : 'BarcodeImage.InitCode128()' has a maintainability index of 13)
//2008-04-11: Refactoring
//2008-04-05: New version

namespace Medo.Drawing {
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Barcode drawing.
    /// </summary>
    public sealed class BarcodePattern {

        private BarcodePattern(IBarcodeImpl barcodeImpl, string text) {
            BarcodeImpl = barcodeImpl;
            Text = text;
        }

        private readonly IBarcodeImpl BarcodeImpl;
        private int[]? CachedPattern;


        /// <summary>
        /// Returns interleaved pattern for barcode.
        /// It starts with black field and alternates bars and gaps/spaces.
        /// Each number signifies how wide the field is (e.g. 4 means 4-bar/space width).
        /// Number 0 signifies space (usually equal to 1 gap).
        /// </summary>
        public int[] GetInterleavedPattern() {
            var pattern = CachedPattern ?? BarcodeImpl.GetPattern(Text);

            var output = new int[pattern.Length];
            Array.Copy(pattern, output, output.Length);
            return output;
        }

        /// <summary>
        /// Returns binary pattern for barcode.
        /// Bars will have value true, gaps/spaces will have value false.
        /// </summary>
        public bool[] GetBinaryPattern() {
            var output = new List<bool>();
            var pattern = GetInterleavedPattern();
            for (var i = 0; i < pattern.Length; i++) {
                var element = pattern[i];
                if (i % 2 == 0) {  // bar
                    for (var j = 0; j < element; j++) {
                        output.Add(true);
                    }
                } else if (element != 0) {  // gap
                    for (var j = 0; j < element; j++) {
                        output.Add(false);
                    }
                } else {  // space
                    output.Add(false);
                }
            }
            return output.ToArray();
        }

        /// <summary>
        /// Gets width of barcode.
        /// </summary>
        public int GetPatternWidth() {
            var pattern = GetInterleavedPattern();
            var totalWidth = 0;
            for (var i = 0; i < pattern.Length; i++) {
                var element = pattern[i];
                if (i % 2 == 0) {  // bar
                    totalWidth += element;
                } else if (pattern[i] != 0) {  // gap
                    totalWidth += element;
                } else {  // space
                    totalWidth += 1;
                }
            }
            return totalWidth;
        }

        private string _text = String.Empty;  // will be overwritten in constructor with real value
        /// <summary>
        /// Gets/sets text.
        /// </summary>
        /// <exception cref="ArgumentNullException">Text cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Character not supported.</exception>
        public string Text {
            get { return _text; }
            set {
                if (value == null) { throw new ArgumentNullException(nameof(value), "Text cannot be null."); }
                for (var i = 0; i < value.Length; ++i) {
                    if (!BarcodeImpl.IsCharacterSupported(value[i])) { throw new ArgumentOutOfRangeException(nameof(value), string.Format(CultureInfo.InvariantCulture, "Character '{0}' is not supported.", value[i])); }
                }
                _text = value;
                CachedPattern = null;  // clear cache
            }
        }


        #region Static

        /// <summary>
        /// Returns implementation of "Code 128" barcode drawing.
        /// Supported characters are all from 7-bit ASCII.
        /// No start/end characters are supported.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <exception cref="ArgumentNullException">Text cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Character not supported.</exception>
        public static BarcodePattern GetNewCode128(string text) {
            var barcode = new BarcodePattern(new Code128Impl(), text);
            return barcode;
        }

        /// <summary>
        /// Returns implementation of "Codabar" barcode drawing with 'A' as both start and end character.
        /// Also known as "NW-7", "USD-4" and "Code 2 of 7".
        /// Supported characters are: 0-9 - $ : / . +.
        /// Supported start/end characters are: A-D.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <exception cref="ArgumentNullException">Text cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Character not supported.</exception>
        public static BarcodePattern GetNewCodabar(string text) {
            return GetNewCodabar(text, 'A', 'A');
        }

        /// <summary>
        /// Returns implementation of "Codabar" barcode drawing.
        /// Also known as "NW-7", "USD-4" and "Code 2 of 7".
        /// Supported characters are: 0-9 - $ : / . +.
        /// Supported start/end characters are: A-D.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="startCharacter">Starting character.</param>
        /// <param name="endCharacter">Ending character.</param>
        /// <exception cref="ArgumentNullException">Text cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Character not supported. -or- Invalid start character. -or- Invalid end character.</exception>
        public static BarcodePattern GetNewCodabar(string text, char startCharacter, char endCharacter) {
            var startCharUpper = char.ToUpperInvariant(startCharacter);
            var endCharUpper = char.ToUpperInvariant(endCharacter);
            if ((startCharUpper != 'A') && (startCharUpper != 'B') && (startCharUpper != 'C') && (startCharUpper != 'D')) { throw new ArgumentOutOfRangeException(nameof(startCharacter), "Invalid start character."); }
            if ((endCharUpper != 'A') && (endCharUpper != 'B') && (endCharUpper != 'C') && (endCharUpper != 'D')) { throw new ArgumentOutOfRangeException(nameof(endCharacter), "Invalid end character."); }
            return new BarcodePattern(new CodabarImpl(startCharUpper, endCharUpper), text);
        }

        #endregion Static


        #region Barcode implementations

        private interface IBarcodeImpl {
            bool IsCharacterSupported(char character);
            int[] GetPattern(string text);
        }

        #region Codabar

        private class CodabarImpl : IBarcodeImpl {
            internal CodabarImpl(char startCharacter, char endCharacter) {
                StartCharacter = startCharacter;
                EndCharacter = endCharacter;
            }

            private readonly char StartCharacter;
            private readonly char EndCharacter;
            private static readonly Dictionary<char, int[]> StartStopEncoding = new() {
                {
                    'A',
                    new int[] { 1, 1, 2, 2, 1, 2, 1 }
                },
                { 'B', new int[] { 1, 1, 1, 2, 1, 2, 2 } },
                { 'C', new int[] { 1, 2, 1, 2, 1, 1, 2 } },
                { 'D', new int[] { 1, 1, 1, 2, 2, 2, 1 } }
            };
            private static readonly Dictionary<char, int[]> TextEncoding = new() {
                { '0', new int[] { 1, 1, 1, 1, 1, 2, 2 } },
                { '1', new int[] { 1, 1, 1, 1, 2, 2, 1 } },
                { '2', new int[] { 1, 1, 1, 2, 1, 1, 2 } },
                { '3', new int[] { 2, 2, 1, 1, 1, 1, 1 } },
                { '4', new int[] { 1, 1, 2, 1, 1, 2, 1 } },
                { '5', new int[] { 2, 1, 1, 1, 1, 2, 1 } },
                { '6', new int[] { 1, 2, 1, 1, 1, 1, 2 } },
                { '7', new int[] { 1, 2, 1, 1, 2, 1, 1 } },
                { '8', new int[] { 1, 2, 2, 1, 1, 1, 1 } },
                { '9', new int[] { 2, 1, 1, 2, 1, 1, 1 } },
                { '-', new int[] { 1, 1, 1, 2, 2, 1, 1 } },
                { '$', new int[] { 1, 1, 2, 2, 1, 1, 1 } },
                { ':', new int[] { 2, 1, 1, 1, 2, 1, 2 } },
                { '/', new int[] { 2, 1, 2, 1, 1, 1, 2 } },
                { '.', new int[] { 2, 1, 2, 1, 2, 1, 1 } },
                { '+', new int[] { 1, 1, 2, 1, 2, 1, 2 } }
            };

            bool IBarcodeImpl.IsCharacterSupported(char character) {
                return TextEncoding.ContainsKey(character);
            }

            int[] IBarcodeImpl.GetPattern(string text) {
                var ev = new List<int>();
                ev.AddRange(StartStopEncoding[StartCharacter]);
                ev.Add(0);
                foreach (var ch in text) {
                    ev.AddRange(TextEncoding[ch]);
                    ev.Add(0);
                }
                ev.AddRange(StartStopEncoding[EndCharacter]);
                return ev.ToArray();
            }
        }

        #endregion Codabar

        #region Code 128

        private class Code128Impl : IBarcodeImpl {
            internal Code128Impl() { }

            //private const byte A_FNC3 = 96;
            //private const byte A_FNC2 = 97;
            //private const byte A_SHIFT = 98;
            //private const byte A_CODEC = 99;
            //private const byte A_CODEB = 100;
            //private const byte A_FNC4 = 101;
            //private const byte A_FNC1 = 102;
            private const byte A_STARTA = 103;
            //private const byte A_STARTB = 104;
            //private const byte A_STARTC = 105;
            private const byte A_STOP = 106;
            //private const byte B_FNC3 = 96;
            //private const byte B_FNC2 = 97;
            //private const byte B_SHIFT = 98;
            //private const byte B_CODEC = 99;
            //private const byte B_FNC4 = 100;
            //private const byte B_CODE1 = 101;
            //private const byte B_FNC1 = 102;
            //private const byte B_STARTA = 103;
            //private const byte B_STARTB = 104;
            //private const byte B_STARTC = 105;
            //private const byte B_STOP = 106;
            //private const byte C_CODEB = 100;
            //private const byte C_CODEA = 101;
            //private const byte C_FNC1 = 102;
            //private const byte C_STARTA = 103;
            //private const byte C_STARTB = 104;
            //private const byte C_STARTC = 105;
            //private const byte C_STOP = 106;

            private static readonly Dictionary<int, int[]> EncodingLookup = new() {
                { 0, new int[] { 2, 1, 2, 2, 2, 2 } },
                { 1, new int[] { 2, 2, 2, 1, 2, 2 } },
                { 2, new int[] { 2, 2, 2, 2, 2, 1 } },
                { 3, new int[] { 1, 2, 1, 2, 2, 3 } },
                { 4, new int[] { 1, 2, 1, 3, 2, 2 } },
                { 5, new int[] { 1, 3, 1, 2, 2, 2 } },
                { 6, new int[] { 1, 2, 2, 2, 1, 3 } },
                { 7, new int[] { 1, 2, 2, 3, 1, 2 } },
                { 8, new int[] { 1, 3, 2, 2, 1, 2 } },
                { 9, new int[] { 2, 2, 1, 2, 1, 3 } },
                { 10, new int[] { 2, 2, 1, 3, 1, 2 } },
                { 11, new int[] { 2, 3, 1, 2, 1, 2 } },
                { 12, new int[] { 1, 1, 2, 2, 3, 2 } },
                { 13, new int[] { 1, 2, 2, 1, 3, 2 } },
                { 14, new int[] { 1, 2, 2, 2, 3, 1 } },
                { 15, new int[] { 1, 1, 3, 2, 2, 2 } },
                { 16, new int[] { 1, 2, 3, 1, 2, 2 } },
                { 17, new int[] { 1, 2, 3, 2, 2, 1 } },
                { 18, new int[] { 2, 2, 3, 2, 1, 1 } },
                { 19, new int[] { 2, 2, 1, 1, 3, 2 } },
                { 20, new int[] { 2, 2, 1, 2, 3, 1 } },
                { 21, new int[] { 2, 1, 3, 2, 1, 2 } },
                { 22, new int[] { 2, 2, 3, 1, 1, 2 } },
                { 23, new int[] { 3, 1, 2, 1, 3, 1 } },
                { 24, new int[] { 3, 1, 1, 2, 2, 2 } },
                { 25, new int[] { 3, 2, 1, 1, 2, 2 } },
                { 26, new int[] { 3, 2, 1, 2, 2, 1 } },
                { 27, new int[] { 3, 1, 2, 2, 1, 2 } },
                { 28, new int[] { 3, 2, 2, 1, 1, 2 } },
                { 29, new int[] { 3, 2, 2, 2, 1, 1 } },
                { 30, new int[] { 2, 1, 2, 1, 2, 3 } },
                { 31, new int[] { 2, 1, 2, 3, 2, 1 } },
                { 32, new int[] { 2, 3, 2, 1, 2, 1 } },
                { 33, new int[] { 1, 1, 1, 3, 2, 3 } },
                { 34, new int[] { 1, 3, 1, 1, 2, 3 } },
                { 35, new int[] { 1, 3, 1, 3, 2, 1 } },
                { 36, new int[] { 1, 1, 2, 3, 1, 3 } },
                { 37, new int[] { 1, 3, 2, 1, 1, 3 } },
                { 38, new int[] { 1, 3, 2, 3, 1, 1 } },
                { 39, new int[] { 2, 1, 1, 3, 1, 3 } },
                { 40, new int[] { 2, 3, 1, 1, 1, 3 } },
                { 41, new int[] { 2, 3, 1, 3, 1, 1 } },
                { 42, new int[] { 1, 1, 2, 1, 3, 3 } },
                { 43, new int[] { 1, 1, 2, 3, 3, 1 } },
                { 44, new int[] { 1, 3, 2, 1, 3, 1 } },
                { 45, new int[] { 1, 1, 3, 1, 2, 3 } },
                { 46, new int[] { 1, 1, 3, 3, 2, 1 } },
                { 47, new int[] { 1, 3, 3, 1, 2, 1 } },
                { 48, new int[] { 3, 1, 3, 1, 2, 1 } },
                { 49, new int[] { 2, 1, 1, 3, 3, 1 } },
                { 50, new int[] { 2, 3, 1, 1, 3, 1 } },
                { 51, new int[] { 2, 1, 3, 1, 1, 3 } },
                { 52, new int[] { 2, 1, 3, 3, 1, 1 } },
                { 53, new int[] { 2, 1, 3, 1, 3, 1 } },
                { 54, new int[] { 3, 1, 1, 1, 2, 3 } },
                { 55, new int[] { 3, 1, 1, 3, 2, 1 } },
                { 56, new int[] { 3, 3, 1, 1, 2, 1 } },
                { 57, new int[] { 3, 1, 2, 1, 1, 3 } },
                { 58, new int[] { 3, 1, 2, 3, 1, 1 } },
                { 59, new int[] { 3, 3, 2, 1, 1, 1 } },
                { 60, new int[] { 3, 1, 4, 1, 1, 1 } },
                { 61, new int[] { 2, 2, 1, 4, 1, 1 } },
                { 62, new int[] { 4, 3, 1, 1, 1, 1 } },
                { 63, new int[] { 1, 1, 1, 2, 2, 4 } },
                { 64, new int[] { 1, 1, 1, 4, 2, 2 } },
                { 65, new int[] { 1, 2, 1, 1, 2, 4 } },
                { 66, new int[] { 1, 2, 1, 4, 2, 1 } },
                { 67, new int[] { 1, 4, 1, 1, 2, 2 } },
                { 68, new int[] { 1, 4, 1, 2, 2, 1 } },
                { 69, new int[] { 1, 1, 2, 2, 1, 4 } },
                { 70, new int[] { 1, 1, 2, 4, 1, 2 } },
                { 71, new int[] { 1, 2, 2, 1, 1, 4 } },
                { 72, new int[] { 1, 2, 2, 4, 1, 1 } },
                { 73, new int[] { 1, 4, 2, 1, 1, 2 } },
                { 74, new int[] { 1, 4, 2, 2, 1, 1 } },
                { 75, new int[] { 2, 4, 1, 2, 1, 1 } },
                { 76, new int[] { 2, 2, 1, 1, 1, 4 } },
                { 77, new int[] { 4, 1, 3, 1, 1, 1 } },
                { 78, new int[] { 2, 4, 1, 1, 1, 2 } },
                { 79, new int[] { 1, 3, 4, 1, 1, 1 } },
                { 80, new int[] { 1, 1, 1, 2, 4, 2 } },
                { 81, new int[] { 1, 2, 1, 1, 4, 2 } },
                { 82, new int[] { 1, 2, 1, 2, 4, 1 } },
                { 83, new int[] { 1, 1, 4, 2, 1, 2 } },
                { 84, new int[] { 1, 2, 4, 1, 1, 2 } },
                { 85, new int[] { 1, 2, 4, 2, 1, 1 } },
                { 86, new int[] { 4, 1, 1, 2, 1, 2 } },
                { 87, new int[] { 4, 2, 1, 1, 1, 2 } },
                { 88, new int[] { 4, 2, 1, 2, 1, 1 } },
                { 89, new int[] { 2, 1, 2, 1, 4, 1 } },
                { 90, new int[] { 2, 1, 4, 1, 2, 1 } },
                { 91, new int[] { 4, 1, 2, 1, 2, 1 } },
                { 92, new int[] { 1, 1, 1, 1, 4, 3 } },
                { 93, new int[] { 1, 1, 1, 3, 4, 1 } },
                { 94, new int[] { 1, 3, 1, 1, 4, 1 } },
                { 95, new int[] { 1, 1, 4, 1, 1, 3 } },
                { 96, new int[] { 1, 1, 4, 3, 1, 1 } },
                { 97, new int[] { 4, 1, 1, 1, 1, 3 } },
                { 98, new int[] { 4, 1, 1, 3, 1, 1 } },
                { 99, new int[] { 1, 1, 3, 1, 4, 1 } },
                { 100, new int[] { 1, 1, 4, 1, 3, 1 } },
                { 101, new int[] { 3, 1, 1, 1, 4, 1 } },
                { 102, new int[] { 4, 1, 1, 1, 3, 1 } },
                { 103, new int[] { 2, 1, 1, 4, 1, 2 } },
                { 104, new int[] { 2, 1, 1, 2, 1, 4 } },
                { 105, new int[] { 2, 1, 1, 2, 3, 2 } },
                { 106, new int[] { 2, 3, 3, 1, 1, 1, 2 } }
            };
            private static readonly Dictionary<int, string>[] MappingLookup = new[] {
                new Dictionary<int, string>() {  // Code Set 0
                    { 0, " " },
                    { 1, "!" },
                    { 2, "\"" },
                    { 3, "#" },
                    { 4, "$" },
                    { 5, "%" },
                    { 6, "&" },
                    { 7, "'" },
                    { 8, "(" },
                    { 9, ")" },
                    { 10, "*" },
                    { 11, "+" },
                    { 12, "," },
                    { 13, "-" },
                    { 14, "." },
                    { 15, "/" },
                    { 16, "0" },
                    { 17, "1" },
                    { 18, "2" },
                    { 19, "3" },
                    { 20, "4" },
                    { 21, "5" },
                    { 22, "6" },
                    { 23, "7" },
                    { 24, "8" },
                    { 25, "9" },
                    { 26, ":" },
                    { 27, ";" },
                    { 28, "<" },
                    { 29, "=" },
                    { 30, ">" },
                    { 31, "?" },
                    { 32, "@" },
                    { 33, "A" },
                    { 34, "B" },
                    { 35, "C" },
                    { 36, "D" },
                    { 37, "E" },
                    { 38, "F" },
                    { 39, "G" },
                    { 40, "H" },
                    { 41, "I" },
                    { 42, "J" },
                    { 43, "K" },
                    { 44, "L" },
                    { 45, "M" },
                    { 46, "N" },
                    { 47, "O" },
                    { 48, "P" },
                    { 49, "Q" },
                    { 50, "R" },
                    { 51, "S" },
                    { 52, "T" },
                    { 53, "U" },
                    { 54, "V" },
                    { 55, "W" },
                    { 56, "X" },
                    { 57, "Y" },
                    { 58, "Z" },
                    { 59, "[" },
                    { 60, "\\" },
                    { 61, "]" },
                    { 62, "^" },
                    { 63, "_" },
                    { 64, Convert.ToChar(0).ToString() },
                    { 65, Convert.ToChar(1).ToString() },
                    { 66, Convert.ToChar(2).ToString() },
                    { 67, Convert.ToChar(3).ToString() },
                    { 68, Convert.ToChar(4).ToString() },
                    { 69, Convert.ToChar(5).ToString() },
                    { 70, Convert.ToChar(6).ToString() },
                    { 71, Convert.ToChar(7).ToString() },
                    { 72, Convert.ToChar(8).ToString() },
                    { 73, Convert.ToChar(9).ToString() },
                    { 74, Convert.ToChar(10).ToString() },
                    { 75, Convert.ToChar(11).ToString() },
                    { 76, Convert.ToChar(12).ToString() },
                    { 77, Convert.ToChar(13).ToString() },
                    { 78, Convert.ToChar(14).ToString() },
                    { 79, Convert.ToChar(15).ToString() },
                    { 80, Convert.ToChar(16).ToString() },
                    { 81, Convert.ToChar(17).ToString() },
                    { 82, Convert.ToChar(18).ToString() },
                    { 83, Convert.ToChar(19).ToString() },
                    { 84, Convert.ToChar(20).ToString() },
                    { 85, Convert.ToChar(21).ToString() },
                    { 86, Convert.ToChar(22).ToString() },
                    { 87, Convert.ToChar(23).ToString() },
                    { 88, Convert.ToChar(24).ToString() },
                    { 89, Convert.ToChar(25).ToString() },
                    { 90, Convert.ToChar(26).ToString() },
                    { 91, Convert.ToChar(27).ToString() },
                    { 92, Convert.ToChar(28).ToString() },
                    { 93, Convert.ToChar(29).ToString() },
                    { 94, Convert.ToChar(30).ToString() },
                    { 95, Convert.ToChar(31).ToString() },
                },
                new Dictionary<int, string>() {  // Code Set 1
                    { 0, " " },
                    { 1, "!" },
                    { 2, "\"" },
                    { 3, "#" },
                    { 4, "$" },
                    { 5, "%" },
                    { 6, "&" },
                    { 7, "'" },
                    { 8, "(" },
                    { 9, ")" },
                    { 10, "*" },
                    { 11, "+" },
                    { 12, "," },
                    { 13, "-" },
                    { 14, "." },
                    { 15, "/" },
                    { 16, "0" },
                    { 17, "1" },
                    { 18, "2" },
                    { 19, "3" },
                    { 20, "4" },
                    { 21, "5" },
                    { 22, "6" },
                    { 23, "7" },
                    { 24, "8" },
                    { 25, "9" },
                    { 26, ":" },
                    { 27, ";" },
                    { 28, "<" },
                    { 29, "=" },
                    { 30, ">" },
                    { 31, "?" },
                    { 32, "@" },
                    { 33, "A" },
                    { 34, "B" },
                    { 35, "C" },
                    { 36, "D" },
                    { 37, "E" },
                    { 38, "F" },
                    { 39, "G" },
                    { 40, "H" },
                    { 41, "I" },
                    { 42, "J" },
                    { 43, "K" },
                    { 44, "L" },
                    { 45, "M" },
                    { 46, "N" },
                    { 47, "O" },
                    { 48, "P" },
                    { 49, "Q" },
                    { 50, "R" },
                    { 51, "S" },
                    { 52, "T" },
                    { 53, "U" },
                    { 54, "V" },
                    { 55, "W" },
                    { 56, "X" },
                    { 57, "Y" },
                    { 58, "Z" },
                    { 59, "[" },
                    { 60, "\\" },
                    { 61, "]" },
                    { 62, "^" },
                    { 63, "_" },
                    { 64, "`" },
                    { 65, "a" },
                    { 66, "b" },
                    { 67, "c" },
                    { 68, "d" },
                    { 69, "e" },
                    { 70, "f" },
                    { 71, "g" },
                    { 72, "h" },
                    { 73, "i" },
                    { 74, "j" },
                    { 75, "k" },
                    { 76, "l" },
                    { 77, "m" },
                    { 78, "n" },
                    { 79, "o" },
                    { 80, "p" },
                    { 81, "q" },
                    { 82, "r" },
                    { 83, "s" },
                    { 84, "t" },
                    { 85, "u" },
                    { 86, "v" },
                    { 87, "w" },
                    { 88, "x" },
                    { 89, "y" },
                    { 90, "z" },
                    { 91, "{" },
                    { 92, "|" },
                    { 93, "}" },
                    { 94, "~" },
                    { 95, Convert.ToChar(127).ToString() },
                },
                new Dictionary<int, string>() {  // Code Set 2
                    { 0, "00" },
                    { 1, "01" },
                    { 2, "02" },
                    { 3, "03" },
                    { 4, "04" },
                    { 5, "05" },
                    { 6, "06" },
                    { 7, "07" },
                    { 8, "08" },
                    { 9, "09" },
                    { 10, "10" },
                    { 11, "11" },
                    { 12, "12" },
                    { 13, "13" },
                    { 14, "14" },
                    { 15, "15" },
                    { 16, "16" },
                    { 17, "17" },
                    { 18, "18" },
                    { 19, "19" },
                    { 20, "20" },
                    { 21, "21" },
                    { 22, "22" },
                    { 23, "23" },
                    { 24, "24" },
                    { 25, "25" },
                    { 26, "26" },
                    { 27, "27" },
                    { 28, "28" },
                    { 29, "29" },
                    { 30, "30" },
                    { 31, "31" },
                    { 32, "32" },
                    { 33, "33" },
                    { 34, "34" },
                    { 35, "35" },
                    { 36, "36" },
                    { 37, "37" },
                    { 38, "38" },
                    { 39, "39" },
                    { 40, "40" },
                    { 41, "41" },
                    { 42, "42" },
                    { 43, "43" },
                    { 44, "44" },
                    { 45, "45" },
                    { 46, "46" },
                    { 47, "47" },
                    { 48, "48" },
                    { 49, "49" },
                    { 50, "50" },
                    { 51, "51" },
                    { 52, "52" },
                    { 53, "53" },
                    { 54, "54" },
                    { 55, "55" },
                    { 56, "56" },
                    { 57, "57" },
                    { 58, "58" },
                    { 59, "59" },
                    { 60, "60" },
                    { 61, "61" },
                    { 62, "62" },
                    { 63, "63" },
                    { 64, "64" },
                    { 65, "65" },
                    { 66, "66" },
                    { 67, "67" },
                    { 68, "68" },
                    { 69, "69" },
                    { 70, "70" },
                    { 71, "71" },
                    { 72, "72" },
                    { 73, "73" },
                    { 74, "74" },
                    { 75, "75" },
                    { 76, "76" },
                    { 77, "77" },
                    { 78, "78" },
                    { 79, "79" },
                    { 80, "80" },
                    { 81, "81" },
                    { 82, "82" },
                    { 83, "83" },
                    { 84, "84" },
                    { 85, "85" },
                    { 86, "86" },
                    { 87, "87" },
                    { 88, "88" },
                    { 89, "89" },
                    { 90, "90" },
                    { 91, "91" },
                    { 92, "92" },
                    { 93, "93" },
                    { 94, "94" },
                    { 95, "95" },
                    { 96, "96" },
                    { 97, "97" },
                    { 98, "98" },
                    { 99, "99" },
                }
            };

            bool IBarcodeImpl.IsCharacterSupported(char character) {
                var value = (int)character;
                return (value >= 0) && (value <= 127);
            }

            int[] IBarcodeImpl.GetPattern(string text) {
                var ev = new List<int>();

                var startCodeSet = GetCode128BestCodeSet(text, -1);
                var checksum = A_STARTA + startCodeSet;

                ev.AddRange(EncodingLookup[(byte)(A_STARTA + startCodeSet)]);

                var codeSet = startCodeSet;
                var j = 1;
                for (var i = 0; i < text.Length; i++) {
                    int tmpCharIndex;

                    var tmpOldCodeSet = codeSet;
                    codeSet = GetCode128BestCodeSet(text[i..text.Length], tmpOldCodeSet);
                    if (codeSet == 2) {
                        tmpCharIndex = GetCode128CharPos(text.Substring(i, 2), codeSet);
                    } else {
                        tmpCharIndex = GetCode128CharPos(text.Substring(i, 1), codeSet);
                    }

                    if (codeSet != tmpOldCodeSet) {
                        ev.AddRange(EncodingLookup[101 - codeSet]);
                        checksum += j * (101 - codeSet);
                        j += 1;
                    }

                    ev.AddRange(EncodingLookup[tmpCharIndex]);
                    checksum += j * tmpCharIndex;

                    if (codeSet == 2) { i++; }  // extra character used with code set 2
                    j += 1;
                }

                ev.AddRange(EncodingLookup[checksum % 103]);
                ev.AddRange(EncodingLookup[A_STOP]);

                return ev.ToArray();
            }

            private static int GetCode128BestCodeSet(string text, int currentCodeSetIndex) {
                int[] tmpCount = { 0, 0, 0 };

                int i;
                for (i = 0; i < text.Length; i++) {
                    if (GetCode128CharPos(text.Substring(i, 1), 0) == -1) {
                        break;
                    }
                }
                tmpCount[0] = i;

                for (i = 0; i < text.Length; i++) {
                    if (GetCode128CharPos(text.Substring(i, 1), 1) == -1) {
                        break;
                    }
                }
                tmpCount[1] = i;

                for (i = 0; i < text.Length - 1; i++) {
                    if (GetCode128CharPos(text.Substring(i, 2), 2) == -1) {
                        break;
                    }
                }
                tmpCount[2] = i * 2;

                switch (currentCodeSetIndex) {
                    case 0:
                        if (tmpCount[0] > 0) { tmpCount[0] += 1; }
                        break;
                    case 1:
                        if (tmpCount[1] > 0) { tmpCount[1] += 1; }
                        break;
                    case 2:
                        if (tmpCount[2] > 0) { tmpCount[2] += 2; }
                        break;
                }

                var tmpCountMax = tmpCount[0];
                var tmpCountMaxIndex = 0;
                if (tmpCount[1] > tmpCountMax) { tmpCountMaxIndex = 1; tmpCountMax = tmpCount[1]; }
                if (tmpCount[2] > tmpCountMax) { tmpCountMaxIndex = 2; }

                return tmpCountMaxIndex;
            }

            private static int GetCode128CharPos(string text, int codeTableIndex) {
                var iMapping = MappingLookup[codeTableIndex].GetEnumerator();
                while (iMapping.MoveNext()) {
                    if (iMapping.Current.Value == text) {
                        return iMapping.Current.Key;
                    }
                }
                return -1;
            }

        }

        #endregion Code 128

        #endregion Barcode implementations

    }
}
