/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-11-25: Refactored to use pattern matching
//2021-11-02: Properly disposing streams
//2021-11-01: Added PNG output (SaveAsPng method)
//2021-10-31: Refactored for .NET 5
//            Removed System.Drawing dependency
//2008-12-09: Changed Code 128 pattern 27 from 311212 to 312212
//2008-11-05: Refactoring (Microsoft.Maintainability : 'BarcodeImage.InitCode128()' has a maintainability index of 13)
//2008-04-11: Refactoring
//2008-04-05: New version

namespace Medo.Drawing {
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    /// <summary>
    /// Barcode drawing.
    /// </summary>
    /// <example>
    /// <code>
    /// var bp = BarcodePattern.GetNewCode128("1234");
    /// bp.SaveAsPng("myimage.png", Color.Blue, Color.Transparent);
    /// </code>
    /// </example>
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


        /// <summary>
        /// Writes PNG image to a file.
        /// </summary>
        /// <param name="fileName">File that will be written to.</param>
        /// <exception cref="ArgumentNullException">File name cannot be null.</exception>
        public void SaveAsPng(string fileName) {
            if (fileName == null) { throw new ArgumentNullException(nameof(fileName), "File name cannot be null."); }
            using var stream = File.OpenWrite(fileName);
            SaveAsPng(stream);
        }

        /// <summary>
        /// Writes PNG image to a stream.
        /// </summary>
        /// <param name="fileName">File that will be written to.</param>
        /// <param name="barColor">Bar color.</param>
        /// <param name="gapColor">Color of gaps and spaces.</param>
        /// <exception cref="ArgumentNullException">File name cannot be null.</exception>
        public void SaveAsPng(string fileName, Color barColor, Color gapColor) {
            if (fileName == null) { throw new ArgumentNullException(nameof(fileName), "File name cannot be null."); }
            using var stream = File.OpenWrite(fileName);
            SaveAsPng(stream, barColor, gapColor);
        }

        /// <summary>
        /// Writes PNG image to a file.
        /// </summary>
        /// <param name="fileName">File that will be written to.</param>
        /// <param name="barColor">Bar color.</param>
        /// <param name="gapColor">Color of gaps and spaces.</param>
        /// <param name="barWidth">Width of a single bar.</param>
        /// <param name="barHeight">Height of a single bar.</param>
        /// <param name="margin">Width of margin around barcode.</param>
        /// <exception cref="ArgumentNullException">File name cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Bar width must be between 1 and 100 pixels. -or- Bar height must be between 1 and 1000 pixels. -or- Margin must be between 1 and 1000 pixels.</exception>
        public void SaveAsPng(string fileName, Color barColor, Color gapColor, int barWidth, int barHeight, int margin) {
            if (fileName == null) { throw new ArgumentNullException(nameof(fileName), "File name cannot be null."); }
            using var stream = File.OpenWrite(fileName);
            SaveAsPng(stream, barColor, gapColor, barWidth, barHeight, margin);
        }

        /// <summary>
        /// Writes PNG image to a stream.
        /// </summary>
        /// <param name="stream">Stream that will be written to.</param>
        /// <exception cref="ArgumentNullException">Stream cannot be null.</exception>
        public void SaveAsPng(Stream stream) {
            SaveAsPng(stream, Color.Black, Color.White);
        }

        /// <summary>
        /// Writes PNG image to a stream.
        /// </summary>
        /// <param name="stream">Stream that will be written to.</param>
        /// <param name="barColor">Bar color.</param>
        /// <param name="gapColor">Color of gaps and spaces.</param>
        /// <exception cref="ArgumentNullException">Stream cannot be null.</exception>
        public void SaveAsPng(Stream stream, Color barColor, Color gapColor) {
            SaveAsPng(stream, barColor, gapColor, 1, 9, 3);
        }

        /// <summary>
        /// Writes PNG image to a stream.
        /// </summary>
        /// <param name="stream">Stream that will be written to.</param>
        /// <param name="barColor">Bar color.</param>
        /// <param name="gapColor">Color of gaps and spaces.</param>
        /// <param name="barWidth">Width of a single bar.</param>
        /// <param name="barHeight">Height of a single bar.</param>
        /// <param name="margin">Width of margin around barcode.</param>
        /// <exception cref="ArgumentNullException">Stream cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Bar width must be between 1 and 100 pixels. -or- Bar height must be between 1 and 1000 pixels. -or- Margin must be between 1 and 1000 pixels.</exception>
        public void SaveAsPng(Stream stream, Color barColor, Color gapColor, int barWidth, int barHeight, int margin) {
            if (stream == null) { throw new ArgumentNullException(nameof(stream), "Stream cannot be null."); }
            if (barWidth is <= 0 or > 100) { throw new ArgumentOutOfRangeException(nameof(barWidth), "Bar width must be between 1 and 100 pixels."); }
            if (barHeight is <= 0 or > 1000) { throw new ArgumentOutOfRangeException(nameof(barHeight), "Bar height must be between 1 and 1000 pixels."); }
            if (margin is < 0 or > 1000) { throw new ArgumentOutOfRangeException(nameof(margin), "Margin must be between 1 and 1000 pixels."); }

            var hasTransparency = (barColor.A < 255) || (gapColor.A < 255);
            var barBytes = hasTransparency ? new byte[] { barColor.R, barColor.G, barColor.B, barColor.A } : new byte[] { barColor.R, barColor.G, barColor.B };
            var gapBytes = hasTransparency ? new byte[] { gapColor.R, gapColor.G, gapColor.B, gapColor.A } : new byte[] { gapColor.R, gapColor.G, gapColor.B };

            var barcodePattern = GetBinaryPattern();
            var totalWidth = (uint)(margin + barcodePattern.Length + margin);
            var totalHeight = (uint)(margin + barHeight + margin);

            using var memStream = new MemoryStream();  // prepare data
            memStream.Write(new byte[] { 0x78, 0x9C });  // deflate header

            using (var deflateStream = new DeflateStream(memStream, CompressionLevel.Optimal)) {
                for (var y = 0; y < margin; y++) {  // top margin
                    deflateStream.Write(new byte[] { 0x00 }, 0, 1);  // start the line with no filter
                    for (var x = 0; x < totalWidth; x++) { deflateStream.Write(gapBytes); }
                }

                for (var y = 0; y < barHeight; y++) {  // barcode
                    deflateStream.Write(new byte[] { 0x00 }, 0, 1);  // start the line with no filter
                    for (var x = 0; x < margin; x++) { deflateStream.Write(gapBytes); }  // left margin
                    for (var x = 0; x < barcodePattern.Length; x++) {
                        deflateStream.Write(barcodePattern[x] ? barBytes : gapBytes);
                    }
                    for (var x = 0; x < margin; x++) { deflateStream.Write(gapBytes); }  // right margin
                }

                for (var y = 0; y < margin; y++) {  // bottom margin
                    deflateStream.Write(new byte[] { 0x00 }, 0, 1);  // start the line with no filter
                    for (var x = 0; x < totalWidth; x++) { deflateStream.Write(gapBytes); }
                }
            }

            stream.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, 0, 8);  // Header
            WritePngChunk(stream, Encoding.ASCII.GetBytes("IHDR"), GetBEBytes(totalWidth), GetBEBytes(totalHeight), new byte[] { 0x08, (byte)(hasTransparency ? 0x06 : 0x02), 0x00, 0x00, 0x00 });
            WritePngChunk(stream, Encoding.ASCII.GetBytes("IDAT"), memStream.ToArray());
            WritePngChunk(stream, Encoding.ASCII.GetBytes("IEND"));
            stream.Flush();
        }

        #region Png

        private static void WritePngChunk(Stream stream, byte[] chunkNameBytes, params byte[][] dataBytes) {
            var totalLength = 0U;
            foreach (var dataBuffer in dataBytes) {
                totalLength += (uint)dataBuffer.Length;
            }

            var chunkLenghtBytes = GetBEBytes(totalLength);
            stream.Write(chunkLenghtBytes, 0, chunkLenghtBytes.Length);

            stream.Write(chunkNameBytes, 0, chunkNameBytes.Length);
            var crc = CalculateCrc32(0xFFFFFFFF, chunkNameBytes);  // start CRC calculation with chunk name

            foreach (var dataBuffer in dataBytes) {
                stream.Write(dataBuffer, 0, dataBuffer.Length);
                crc = CalculateCrc32(crc, dataBuffer);  // add data bytes to CRC
            }

            var crcBytes = GetBEBytes(crc ^ 0xFFFFFFFF);  // final CRC xor and convert to bytes
            stream.Write(crcBytes, 0, crcBytes.Length);
        }

        private static byte[] GetBEBytes(uint number) {
            if (BitConverter.IsLittleEndian) {
                var buffer = BitConverter.GetBytes(number);
                return new byte[] { buffer[3], buffer[2], buffer[1], buffer[0] };
            } else {
                return BitConverter.GetBytes(number);
            }
        }

        private static readonly uint[] Crc32LookupTable = {
            0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419, 0x706AF48F, 0xE963A535, 0x9E6495A3,
            0x0EDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91,
            0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE, 0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7,
            0x136C9856, 0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9, 0xFA0F3D63, 0x8D080DF5,
            0x3B6E20C8, 0x4C69105E, 0xD56041E4, 0xA2677172, 0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B,
            0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3, 0x45DF5C75, 0xDCD60DCF, 0xABD13D59,
            0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599, 0xB8BDA50F,
            0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924, 0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D,
            0x76DC4190, 0x01DB7106, 0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F, 0x9FBFE4A5, 0xE8B8D433,
            0x7807C9A2, 0x0F00F934, 0x9609A88E, 0xE10E9818, 0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
            0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED, 0x1B01A57B, 0x8208F4C1, 0xF50FC457,
            0x65B0D9C6, 0x12B7E950, 0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65,
            0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2, 0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB,
            0x4369E96A, 0x346ED9FC, 0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9,
            0x5005713C, 0x270241AA, 0xBE0B1010, 0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
            0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17, 0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD,
            0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615, 0x73DC1683,
            0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8, 0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1,
            0xF00F9344, 0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB, 0x196C3671, 0x6E6B06E7,
            0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC, 0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
            0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1, 0xA6BC5767, 0x3FB506DD, 0x48B2364B,
            0xD80D2BDA, 0xAF0A1B4C, 0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF, 0x4669BE79,
            0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236, 0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F,
            0xC5BA3BBE, 0xB2BD0B28, 0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D,
            0x9B64C2B0, 0xEC63F226, 0x756AA39C, 0x026D930A, 0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713,
            0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B, 0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21,
            0x86D3D2D4, 0xF1D4E242, 0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1, 0x18B74777,
            0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C, 0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45,
            0xA00AE278, 0xD70DD2EE, 0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB,
            0xAED16A4A, 0xD9D65ADC, 0x40DF0B66, 0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
            0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605, 0xCDD70693, 0x54DE5729, 0x23D967BF,
            0xB3667A2E, 0xC4614AB8, 0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B, 0x2D02EF8D,
        };

        private static uint CalculateCrc32(uint initialCrc, byte[] buffer) {
            uint crc = initialCrc;
            foreach (var b in buffer) {
                crc = Crc32LookupTable[(crc ^ b) & 0xff] ^ (crc >> 8);
            }
            return crc;
        }

        #endregion Png

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
            if (startCharUpper is not 'A' and not 'B' and not 'C' and not 'D') { throw new ArgumentOutOfRangeException(nameof(startCharacter), "Invalid start character."); }
            if (endCharUpper is not 'A' and not 'B' and not 'C' and not 'D') { throw new ArgumentOutOfRangeException(nameof(endCharacter), "Invalid end character."); }
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
                return (value is >= 0 and <= 127);
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
