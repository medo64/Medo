/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-11-25: Refactored to use pattern matching
//2021-11-03: Initial version

namespace Medo.Drawing {
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    /// <summary>
    /// A simple PNG image reader/writer.
    /// Does not support reading interlaced files.
    /// </summary>
    /// <example>
    /// <code>
    /// var bmp = new SimplePngImage(2, 2);
    /// bmp.SetPixel(0, 0, Color.Red);
    /// bmp.SetPixel(0, 1, Color.Green);
    /// bmp.SetPixel(1, 0, Color.Blue);
    /// bmp.SetPixel(1, 1, Color.FromArgb(128, Color.Purple));
    /// bmp.Save("image.png");
    /// </code>
    /// </example>
    /// <remarks>https://www.w3.org/TR/2003/REC-PNG-20031110/</remarks>
    public sealed class SimplePngImage {

        /// <summary>
        /// Creates a new instance based on an image file.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <exception cref="ArgumentNullException">File name cannot be null.</exception>
        /// <exception cref="InvalidDataException">Invalid header.</exception>
        public SimplePngImage(string fileName) {
            if (fileName == null) { throw new ArgumentNullException(nameof(fileName), "File name cannot be null."); }
            using var stream = File.OpenRead(fileName);
            PixelBuffer = GetBufferFromStream(stream);
        }

        /// <summary>
        /// Creates a new instance based on image stream
        /// </summary>
        /// <param name="stream">Stream to read.</param>
        /// <exception cref="ArgumentNullException">Stream cannot be null.</exception>
        public SimplePngImage(Stream stream) {
            if (stream == null) { throw new ArgumentNullException(nameof(stream), "Stream cannot be null."); }
            PixelBuffer = GetBufferFromStream(stream);
        }


        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="width">Width of an image.</param>
        /// <param name="height">Height of an image.</param>
        /// <exception cref="ArgumentOutOfRangeException">Width must be a positive number. -or- Height must be a positive number.</exception>
        public SimplePngImage(int width, int height) {
            if (width < 1) { throw new ArgumentOutOfRangeException(nameof(width), "Width must be a positive number."); }
            if (height < 1) { throw new ArgumentOutOfRangeException(nameof(height), "Height must be a positive number."); }
            PixelBuffer = new Color[width, height];
        }

        #region State

        private readonly Color[,] PixelBuffer;

        private SimplePngImage(Color[,] pixelBuffer) {
            PixelBuffer = new Color[pixelBuffer.GetLength(0), pixelBuffer.GetLength(1)];
            Array.Copy(pixelBuffer, PixelBuffer, pixelBuffer.Length);
        }

        /// <summary>
        /// Creates a copy of the image.
        /// </summary>
        public SimplePngImage Clone() {
            return new SimplePngImage(PixelBuffer);
        }

        #endregion Clone

        #region Properties

        /// <summary>
        /// Gets image width.
        /// </summary>
        public int Width { get { return PixelBuffer.GetLength(0); } }

        /// <summary>
        /// Gets image height
        /// </summary>
        public int Height { get { return PixelBuffer.GetLength(1); } }

        #endregion Properties

        #region Pixel

        /// <summary>
        /// Returns color at a given pixel.
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <exception cref="ArgumentOutOfRangeException">X outside of bounds. -or- Y outside of bounds.</exception>
        public Color GetPixel(int x, int y) {
            if ((x < 0) || (x >= PixelBuffer.GetLength(0))) { throw new ArgumentOutOfRangeException(nameof(x), "X outside of bounds."); }
            if ((y < 0) || (y >= PixelBuffer.GetLength(1))) { throw new ArgumentOutOfRangeException(nameof(y), "Y outside of bounds."); }
            return PixelBuffer[x, y];
        }

        /// <summary>
        /// Sets color at a given pixel.
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <param name="color">Pixel color.</param>
        /// <exception cref="ArgumentOutOfRangeException">X outside of bounds. -or- Y outside of bounds.</exception>
        public void SetPixel(int x, int y, Color color) {
            if ((x < 0) || (x >= PixelBuffer.GetLength(0))) { throw new ArgumentOutOfRangeException(nameof(x), "X outside of bounds."); }
            if ((y < 0) || (y >= PixelBuffer.GetLength(1))) { throw new ArgumentOutOfRangeException(nameof(y), "Y outside of bounds."); }
            PixelBuffer[x, y] = color;
        }

        #endregion Pixel

        #region Save

        /// <summary>
        /// Writes PNG image to a file.
        /// </summary>
        /// <param name="fileName">File that will be written to.</param>
        /// <exception cref="ArgumentNullException">File name cannot be null.</exception>
        public void Save(string fileName) {
            if (fileName == null) { throw new ArgumentNullException(nameof(fileName), "File name cannot be null."); }
            using var stream = File.OpenWrite(fileName);
            Save(stream);
        }


        /// <summary>
        /// Writes PNG image to a stream.
        /// </summary>
        /// <param name="stream">Stream that will be written to.</param>
        /// <exception cref="ArgumentNullException">Stream cannot be null.</exception>
        public void Save(Stream stream) {
            if (stream == null) { throw new ArgumentNullException(nameof(stream), "Stream cannot be null."); }

            var hasAlpha = false;
            var hasColor = false;
            var width = Width;
            var height = Height;
            for (var x = 0; x < width; x++) {
                for (var y = 0; y < height; y++) {
                    if (PixelBuffer[x, y].A < 255) {
                        hasAlpha = true;
                        if (hasColor) { break; }  // finish early if color is also detected
                    }
                    if ((PixelBuffer[x, y].R != PixelBuffer[x, y].G) || (PixelBuffer[x, y].G != PixelBuffer[x, y].B)) {
                        hasColor = true;
                        if (hasAlpha) { break; }  // finish early if transparency is also detected
                    }
                }
            }
            var colorKind = hasColor && hasAlpha ? ColorKind.ColorAlpha : hasColor ? ColorKind.Color : hasAlpha ? ColorKind.MonoAlpha : ColorKind.Mono;

            using var memStream = new MemoryStream();  // prepare data
            memStream.Write(new byte[] { 0x78, 0x9C });  // deflate header

            using (var deflateStream = new DeflateStream(memStream, CompressionLevel.Optimal)) {
                for (var y = 0; y < Height; y++) {
                    deflateStream.Write(new byte[] { 0x00 }, 0, 1);  // start the line with no filter
                    for (var x = 0; x < Width; x++) {
                        var pixelColor = PixelBuffer[x, y];
                        var pixelBytes = colorKind switch {
                            ColorKind.ColorAlpha => new byte[] { pixelColor.R, pixelColor.G, pixelColor.B, pixelColor.A },
                            ColorKind.Color => new byte[] { pixelColor.R, pixelColor.G, pixelColor.B },
                            ColorKind.MonoAlpha => new byte[] { pixelColor.R, pixelColor.A },
                            ColorKind.Mono => new byte[] { pixelColor.R },
                            _ => Array.Empty<byte>()  // should not happen
                        };
                        deflateStream.Write(pixelBytes, 0, pixelBytes.Length);
                    }
                }
            }

            var colorType = colorKind switch {
                ColorKind.Color => 0x02,
                ColorKind.ColorAlpha => 0x06,
                ColorKind.Mono => 0x00,
                ColorKind.MonoAlpha => 0x04,
                _ => throw new InvalidOperationException("Cannot determine color type.")  // should not happen
            };

            stream.Write(PngHeaderBytes, 0, PngHeaderBytes.Length);  // header
            WritePngChunk(stream, PngChunkHeaderNameBytes, ToBytes((uint)Width), ToBytes((uint)Height), new byte[] { 0x08, (byte)colorType, 0x00, 0x00, 0x00 });  // header chunk
            WritePngChunk(stream, PngChunkDataNameBytes, memStream.ToArray());  // data chunk
            WritePngChunk(stream, PngChunkEndNameBytes);  // end chunk
            stream.Flush();
        }

        #endregion Save

        #region Load

        private enum ColorKind { Unknown, Color, ColorAlpha, Indexed, Mono, MonoAlpha }

        private static Color[,] GetBufferFromStream(Stream stream) {
            var headerBytes = new byte[8];
            stream.Read(headerBytes, 0, 8);

            if (!CheckIfEqual(headerBytes, PngHeaderBytes)) { throw new InvalidDataException("Invalid header."); }

            var width = 0;
            var height = 0;
            var bitDepth = 0;
            var colorStyle = ColorKind.Unknown;
            using var dataStream = new MemoryStream();

            var lengthBytes = new byte[4];
            var chunkNameBytes = new byte[4];
            var crcBytes = new byte[4];
            var palette = new List<Color>();
            while (true) {
                stream.Read(lengthBytes, 0, 4);
                stream.Read(chunkNameBytes, 0, 4);
                if (CheckIfEqual(chunkNameBytes, PngChunkEndNameBytes)) { break; }  // end chunk - don't even bother checking length

                var length = (int)FromBytes(lengthBytes);
                if (length < 0) { throw new InvalidDataException("Invalid chunk length."); }

                var dataBytes = new byte[length];
                stream.Read(dataBytes, 0, length);

                stream.Read(crcBytes, 0, 4);

                var crc = CalculateCrc32(0xFFFFFFFF, chunkNameBytes);  // start CRC calculation with chunk name
                crc = CalculateCrc32(crc, dataBytes);  // add data bytes to CRC
                var crcCalcBytes = ToBytes(crc ^ 0xFFFFFFFF);
                if (!CheckIfEqual(crcBytes, crcCalcBytes)) { throw new InvalidDataException("Invalid chunk CRC."); }

                if (CheckIfEqual(chunkNameBytes, PngChunkHeaderNameBytes)) {  // header chunk

                    width = (int)FromBytes(dataBytes, 0);
                    height = (int)FromBytes(dataBytes, 4);
                    bitDepth = dataBytes[8];
                    var colorType = dataBytes[9];
                    var compressionMethod = dataBytes[10];
                    var filterMethod = dataBytes[11];
                    var interlaceMethod = dataBytes[12];

                    switch (colorType) {
                        case 0:  // Greyscale
                            if (bitDepth is not 1 and not 2 and not 4 and not 8) { throw new InvalidDataException("Unsupported bit depth."); }
                            colorStyle = ColorKind.Mono;
                            break;

                        case 2:  // Truecolour
                            if (bitDepth != 8) { throw new InvalidDataException("Unsupported bit depth."); }
                            colorStyle = ColorKind.Color;
                            break;

                        case 3:  // Indexed-colour
                            if (bitDepth is not 1 and not 2 and not 4 and not 8) { throw new InvalidDataException("Unsupported bit depth."); }
                            colorStyle = ColorKind.Indexed;
                            break;

                        case 4:  // Greyscale with alpha
                            if (bitDepth != 8) { throw new InvalidDataException("Unsupported bit depth."); }
                            colorStyle = ColorKind.MonoAlpha;
                            break;

                        case 6:  // Truecolour with alpha
                            if (bitDepth != 8) { throw new InvalidDataException("Unsupported bit depth."); }
                            colorStyle = ColorKind.ColorAlpha;
                            break;

                        default: throw new InvalidDataException("Unsupported color type.");
                    }

                    if (compressionMethod != 0) { throw new InvalidDataException("Unsupported compression method."); }
                    if (filterMethod != 0) { throw new InvalidDataException("Unsupported filter method."); }
                    if (interlaceMethod != 0) { throw new InvalidDataException("Unsupported interlace method."); }

                } else if (CheckIfEqual(chunkNameBytes, PngChunkPaletteNameBytes)) {  // palette chunk
                    for (var i = 0; i < dataBytes.Length; i += 3) {
                        palette.Add(Color.FromArgb(dataBytes[i + 0], dataBytes[i + 1], dataBytes[i + 2]));
                    }
                } else if (CheckIfEqual(chunkNameBytes, PngChunkDataNameBytes)) {  // data chunk
                    dataStream.Write(dataBytes, 0, dataBytes.Length);
                }
            }

            if ((width <= 0) || (height <= 0)) { throw new InvalidDataException("Cannot determine image size."); }
            var pixelBuffer = new Color[width, height];

            dataStream.Position = 2;  // skip header
            using (var deflateStream = new DeflateStream(dataStream, CompressionMode.Decompress)) {
                byte[]? prevLineBytes = null;
                for (var y = 0; y < height; y++) {
                    var lineFilter = (byte)deflateStream.ReadByte();

                    var bitMultiplier = colorStyle switch {
                        ColorKind.Color => 3,
                        ColorKind.ColorAlpha => 4,
                        ColorKind.Indexed => 1,
                        ColorKind.Mono => 1,
                        ColorKind.MonoAlpha => 2,
                        _ => throw new InvalidDataException("Unsupported color type."),
                    };
                    var bitCount = width * bitDepth * bitMultiplier;
                    var byteCount = bitCount / 8 + (bitCount % 8 != 0 ? 1 : 0);
                    var lineBytes = new byte[byteCount];
                    deflateStream.Read(lineBytes, 0, lineBytes.Length);

                    switch (lineFilter) {
                        case 0:  // None
                            break;

                        case 1:  // Sub
                            for (var i = 0; i < lineBytes.Length; i++) {
                                var a = (i >= bitMultiplier) ? lineBytes[i - bitMultiplier] : (byte)0;
                                lineBytes[i] = (byte)(lineBytes[i] + a);
                            }
                            break;

                        case 2:  // Up
                            for (var i = 0; i < lineBytes.Length; i++) {
                                var b = (prevLineBytes != null) ? prevLineBytes[i] : (byte)0;
                                lineBytes[i] = (byte)(lineBytes[i] + b);
                            }
                            break;

                        case 3:  // Average
                            for (var i = 0; i < lineBytes.Length; i++) {
                                var a = (i >= bitMultiplier) ? lineBytes[i - bitMultiplier] : (byte)0;
                                var b = (prevLineBytes != null) ? prevLineBytes[i] : (byte)0;
                                lineBytes[i] = (byte)(lineBytes[i] + (byte)((a + b) / 2));
                            }
                            break;

                        case 4:  // Paeth
                            for (var i = 0; i < lineBytes.Length; i++) {
                                var a = (i >= bitMultiplier) ? lineBytes[i - bitMultiplier] : (byte)0;
                                var b = (prevLineBytes != null) ? prevLineBytes[i] : (byte)0;
                                var c = ((i >= bitMultiplier) && (prevLineBytes != null)) ? prevLineBytes[i - bitMultiplier] : (byte)0;
                                lineBytes[i] = (byte)(lineBytes[i] + PaethPredictor(a, b, c));
                            }
                            break;

                        default: throw new InvalidDataException("Unsupported scanline filter (" + lineFilter.ToString(CultureInfo.InvariantCulture) + ").");
                    }

                    switch (colorStyle) {
                        case ColorKind.Color:
                            for (var x = 0; x < width; x++) {
                                var offset = x * 3;
                                pixelBuffer[x, y] = Color.FromArgb(lineBytes[offset + 0], lineBytes[offset + 1], lineBytes[offset + 2]);
                            }
                            break;

                        case ColorKind.ColorAlpha:
                            for (var x = 0; x < width; x++) {
                                var offset = x * 4;
                                pixelBuffer[x, y] = Color.FromArgb(lineBytes[offset + 3], lineBytes[offset + 0], lineBytes[offset + 1], lineBytes[offset + 2]);
                            }
                            break;

                        case ColorKind.Indexed:
                        case ColorKind.Mono:  // mono is just indexed with preset palette for all practical purposes
                            for (var n = 0; n < bitCount; n += bitDepth) {
                                var i = n / 8;
                                var x = n / bitDepth;
                                var index = bitDepth switch {
                                    8 => lineBytes[i],
                                    4 => (x % 2 == 0) ? lineBytes[i] >> 4 : lineBytes[i] & 0x0F,
                                    2 => (x % 4 == 0) ? lineBytes[i] >> 6 : (x % 4 == 1) ? (lineBytes[i] >> 4) & 0x03 : (x % 4 == 2) ? (lineBytes[i] >> 2) & 0x03 : lineBytes[i] & 0x03,
                                    1 => (x % 8 == 0) ? lineBytes[i] >> 7 : (x % 8 == 1) ? (lineBytes[i] >> 6) & 0x01 : (x % 8 == 2) ? (lineBytes[i] >> 5) & 0x01 : (x % 8 == 3) ? (lineBytes[i] >> 4) & 0x01 : (x % 8 == 4) ? (lineBytes[i] >> 3) & 0x01 : (x % 8 == 5) ? (lineBytes[i] >> 2) & 0x01 : (x % 8 == 6) ? (lineBytes[i] >> 1) & 0x01 : lineBytes[i] & 0x01,
                                    _ => throw new InvalidDataException("Unsupported bits per pixel."),
                                };
                                if (colorStyle == ColorKind.Mono) {  // just setup grayscale
                                    var m = bitDepth switch {
                                        8 => index,
                                        4 => (index == 0) ? 0x00 : (index == 1) ? 0x11 : (index == 2) ? 0x22 : (index == 3) ? 0x33 : (index == 4) ? 0x44 : (index == 5) ? 0x55 : (index == 6) ? 0x66 : (index == 7) ? 0x77 : (index == 8) ? 0x88 : (index == 9) ? 0x99 : (index == 10) ? 0xAA : (index == 11) ? 0xBB : (index == 12) ? 0xCC : (index == 13) ? 0xDD : (index == 14) ? 0xEE : 0xFF,
                                        2 => (index == 0) ? 0x00 : (index == 1) ? 0x67 : (index == 2) ? 0xB6 : 0xFF,
                                        1 => index * 255,
                                        _ => throw new InvalidDataException("Unsupported bits per pixel."),
                                    };
                                    pixelBuffer[x, y] = Color.FromArgb(index, index, index);
                                } else if (index < palette.Count) {  // use the palette
                                    pixelBuffer[x, y] = palette[index];
                                }
                            }
                            break;

                        case ColorKind.MonoAlpha:
                            for (var x = 0; x < width; x++) {
                                var offset = x * 2;
                                var m = lineBytes[offset + 0];
                                var a = lineBytes[offset + 1];
                                pixelBuffer[x, y] = Color.FromArgb(a, m, m, m);
                            }
                            break;

                        default: throw new InvalidDataException("Unsupported color type.");
                    }

                    prevLineBytes = lineBytes;
                }
            }

            return pixelBuffer;
        }

        #endregion

        #region Png

        private readonly static byte[] PngHeaderBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        private readonly static byte[] PngChunkHeaderNameBytes = Encoding.ASCII.GetBytes("IHDR");
        private readonly static byte[] PngChunkPaletteNameBytes = Encoding.ASCII.GetBytes("PLTE");
        private readonly static byte[] PngChunkDataNameBytes = Encoding.ASCII.GetBytes("IDAT");
        private readonly static byte[] PngChunkEndNameBytes = Encoding.ASCII.GetBytes("IEND");

        private static void WritePngChunk(Stream stream, byte[] chunkNameBytes, params byte[][] dataBytes) {
            var totalLength = 0U;
            foreach (var dataBuffer in dataBytes) {
                totalLength += (uint)dataBuffer.Length;
            }

            var chunkLenghtBytes = ToBytes(totalLength);
            stream.Write(chunkLenghtBytes, 0, chunkLenghtBytes.Length);

            stream.Write(chunkNameBytes, 0, chunkNameBytes.Length);
            var crc = CalculateCrc32(0xFFFFFFFF, chunkNameBytes);  // start CRC calculation with chunk name

            foreach (var dataBuffer in dataBytes) {
                stream.Write(dataBuffer, 0, dataBuffer.Length);
                crc = CalculateCrc32(crc, dataBuffer);  // add data bytes to CRC
            }

            var crcBytes = ToBytes(crc ^ 0xFFFFFFFF);  // final CRC xor and convert to bytes
            stream.Write(crcBytes, 0, crcBytes.Length);
        }

        private static byte[] ToBytes(uint number) {  // always in big endian
            if (BitConverter.IsLittleEndian) {
                var buffer = BitConverter.GetBytes(number);
                return new byte[] { buffer[3], buffer[2], buffer[1], buffer[0] };
            } else {
                return BitConverter.GetBytes(number);
            }
        }

        private static uint FromBytes(byte[] bytes, int offset = 0) {  // always in big endian
            if (BitConverter.IsLittleEndian) {
                return (uint)((bytes[offset + 0] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | (bytes[offset + 3]));
            } else {
                return BitConverter.ToUInt32(bytes, offset);
            }
        }


        private static bool CheckIfEqual(byte[] bytes1, byte[] bytes2) {
            if (bytes1.Length != bytes2.Length) { return false; }
            for (var i = 0; i < bytes1.Length; i++) {
                if (bytes1[i] != bytes2[i]) { return false; }
            }
            return true;
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

        private static byte PaethPredictor(byte a, byte b, byte c) {  // a = left, b = above, c = upper left
            var p = a + b - c;  // initial estimate
            var pa = Math.Abs(p - a);  // distances to a, b, c
            var pb = Math.Abs(p - b);
            var pc = Math.Abs(p - c);

            // breaking ties in order a, b, c.
            if ((pa <= pb) && (pa <= pc)) {
                return a;
            } else if (pb <= pc) {
                return b;
            } else {
                return c;
            }
        }

        #endregion Png

    }
}
