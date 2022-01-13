using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using Medo.Security.Cryptography;

namespace Tests.Medo.Security.Cryptography;

public class RivestCipher4ManagedTests {

    public RivestCipher4ManagedTests(ITestOutputHelper output) => Output = output;
    private readonly ITestOutputHelper Output;


    [Theory(DisplayName = "RivestCipher4: Known Answers (Basic)")]
    [InlineData("Key", "Plaintext", "BBF316E8D940AF0AD3")]
    [InlineData("Wiki", "pedia", "1021BF0420")]
    [InlineData("Secret", "Attack at dawn", "45A01F645FC35B383552544B9BF5")]
    public void KnownAnswers_Basic(string keyText, string plainText, string cipherTextHex) {
        using var algorithm = new RivestCipher4Managed();

        var ct = Encrypt(algorithm,
            Encoding.ASCII.GetBytes(keyText),
            null,
            Encoding.ASCII.GetBytes(plainText));
        Assert.Equal(cipherTextHex, BitConverter.ToString(ct).Replace("-", ""));

        var pt = Decrypt(algorithm,
            Encoding.ASCII.GetBytes(keyText),
            null,
            ct);
        Assert.Equal(plainText, Encoding.ASCII.GetString(pt));
    }

    [Theory(DisplayName = "RivestCipher4: Known Answers (Basic with IV)")]
    [InlineData("Ke", "y", "Plaintext", "BBF316E8D940AF0AD3")]
    [InlineData("Wi", "ki", "pedia", "1021BF0420")]
    [InlineData("S", "ecret", "Attack at dawn", "45A01F645FC35B383552544B9BF5")]
    public void KnownAnswers_BasicIV(string keyText, string ivText, string plainText, string cipherTextHex) {
        using var algorithm = new RivestCipher4Managed();

        var ct = Encrypt(algorithm,
            Encoding.ASCII.GetBytes(keyText),
            Encoding.ASCII.GetBytes(ivText),
            Encoding.ASCII.GetBytes(plainText));
        Assert.Equal(cipherTextHex, BitConverter.ToString(ct).Replace("-", ""));

        var pt = Decrypt(algorithm,
            Encoding.ASCII.GetBytes(keyText),
            Encoding.ASCII.GetBytes(ivText),
            ct);
        Assert.Equal(plainText, Encoding.ASCII.GetString(pt));
    }


    [Theory(DisplayName = "RivestCipher4: Test Vectors")]
    [InlineData("Vector40A.txt")]
    [InlineData("Vector56A.txt")]
    [InlineData("Vector64A.txt")]
    [InlineData("Vector80A.txt")]
    [InlineData("Vector128A.txt")]
    [InlineData("Vector192A.txt")]
    [InlineData("Vector256A.txt")]
    [InlineData("Vector40B.txt")]
    [InlineData("Vector56B.txt")]
    [InlineData("Vector64B.txt")]
    [InlineData("Vector80B.txt")]
    [InlineData("Vector128B.txt")]
    [InlineData("Vector192B.txt")]
    [InlineData("Vector256B.txt")]
    public void Vectors(string fileName) {
        RetrieveVectors(fileName, out var key, out var dataQueue);

        using var ct = new MemoryStream();
        using var transform = new RivestCipher4Managed().CreateEncryptor(key, null);
        using var cs = new CryptoStream(ct, transform, CryptoStreamMode.Write);

        var n = 0;
        while (dataQueue.Count > 0) {
            var entry = dataQueue.Dequeue();
            var index = entry.Key;
            var expectedBytes = entry.Value;

            while (n <= index) {
                cs.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });  // hardcoded to 16-bytes
                n += 16;
            }
            cs.Flush();
            var cipherBytes = new byte[16];
            Array.Copy(ct.ToArray(), n - 16, cipherBytes, 0, 16);  // not efficient but good enough for test
            Assert.Equal(BitConverter.ToString(expectedBytes), BitConverter.ToString(cipherBytes));
        }
    }

    [Theory(DisplayName = "RivestCipher4Managed: Padding full blocks")]
    [InlineData(PaddingMode.None)]
    [InlineData(PaddingMode.PKCS7)]
    [InlineData(PaddingMode.Zeros)]
    [InlineData(PaddingMode.ANSIX923)]
    [InlineData(PaddingMode.ISO10126)]
    public void PaddingFull(PaddingMode padding) {
        var key = new byte[16]; RandomNumberGenerator.Fill(key);
        var iv = new byte[8]; RandomNumberGenerator.Fill(iv);
        var data = new byte[48]; RandomNumberGenerator.Fill(data);  // full blocks

        var algorithm = new RivestCipher4Managed() { Padding = padding, };

        var ct = Encrypt(algorithm, key, iv, data);
        var pt = Decrypt(algorithm, key, iv, ct);
        Assert.Equal(data.Length, pt.Length);
        Assert.Equal(BitConverter.ToString(data), BitConverter.ToString(pt));
    }

    [Theory(DisplayName = "RivestCipher4Managed: Padding partial blocks")]
    [InlineData(PaddingMode.PKCS7)]
    [InlineData(PaddingMode.Zeros)]
    [InlineData(PaddingMode.ANSIX923)]
    [InlineData(PaddingMode.ISO10126)]
    public void PaddingPartial(PaddingMode padding) {
        var key = new byte[16]; RandomNumberGenerator.Fill(key);
        var iv = new byte[8]; RandomNumberGenerator.Fill(iv);
        var data = new byte[42]; RandomNumberGenerator.Fill(data);

        var algorithm = new RivestCipher4Managed() { Padding = padding };

        var ct = Encrypt(algorithm, key, iv, data);
        var pt = Decrypt(algorithm, key, iv, ct);
        Assert.Equal(data.Length, pt.Length);
        Assert.Equal(BitConverter.ToString(data), BitConverter.ToString(pt));
    }

    [Theory(DisplayName = "RivestCipher4Managed: Only CBC supported")]
    [InlineData(CipherMode.ECB)]
    [InlineData(CipherMode.CFB)]
    [InlineData(CipherMode.CTS)]
    public void OnlyCbcSupported(CipherMode mode) {
        Assert.Throws<CryptographicException>(() => {
            var _ = new RivestCipher4Managed() { Mode = mode };
        });
    }

    [Theory(DisplayName = "RivestCipher4Managed: Large Final Block")]
    [InlineData(PaddingMode.None)]
    [InlineData(PaddingMode.PKCS7)]
    [InlineData(PaddingMode.Zeros)]
    [InlineData(PaddingMode.ANSIX923)]
    [InlineData(PaddingMode.ISO10126)]
    public void LargeFinalBlock(PaddingMode padding) {
        var crypto = new RivestCipher4Managed() { Padding = padding };
        crypto.GenerateKey();
        crypto.GenerateIV();
        var text = "This is a final block wider than block size.";  // more than 128 bits of data
        var bytes = Encoding.ASCII.GetBytes(text);

        using var encryptor = crypto.CreateEncryptor();
        var ct = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

        Assert.Equal(padding == PaddingMode.None ? bytes.Length : 48, ct.Length);

        using var decryptor = crypto.CreateDecryptor();
        var pt = decryptor.TransformFinalBlock(ct, 0, ct.Length);

        Assert.Equal(bytes.Length, pt.Length);
        Assert.Equal(text, Encoding.ASCII.GetString(pt));
    }

    [Theory(DisplayName = "RivestCipher4Managed: BlockSizeRounding")]
    [InlineData(PaddingMode.None)]
    [InlineData(PaddingMode.PKCS7)]
    [InlineData(PaddingMode.Zeros)]
    [InlineData(PaddingMode.ANSIX923)]
    [InlineData(PaddingMode.ISO10126)]
    public void BlockSizeRounding(PaddingMode padding) {
        var key = new byte[16]; RandomNumberGenerator.Fill(key);
        var iv = new byte[8]; RandomNumberGenerator.Fill(iv);

        for (int n = 0; n < 50; n++) {
            var data = new byte[n];
            RandomNumberGenerator.Fill(data);
            if ((padding == PaddingMode.Zeros) && (data.Length > 0)) { data[^1] = 1; }  // zero padding needs to have the last number non-zero

            var algorithm = new RivestCipher4Managed() { Padding = padding, };

            var expectedCryptLength = padding switch {
                PaddingMode.None => data.Length,
                PaddingMode.PKCS7 => ((data.Length / 16) + 1) * 16,
                PaddingMode.Zeros => (data.Length / 16 + (data.Length % 16 > 0 ? 1 : 0)) * 16,
                PaddingMode.ANSIX923 => ((data.Length / 16) + 1) * 16,
                PaddingMode.ISO10126 => ((data.Length / 16) + 1) * 16,
                _ => -1

            };
            var ct = Encrypt(algorithm, key, iv, data);
            Assert.Equal(expectedCryptLength, ct.Length);

            var pt = Decrypt(algorithm, key, iv, ct);
            Assert.Equal(data.Length, pt.Length);
            Assert.Equal(BitConverter.ToString(data), BitConverter.ToString(pt));
        }
    }

    [Theory(DisplayName = "RivestCipher4Managed: Random Testing")]
    [InlineData(PaddingMode.None)]
    [InlineData(PaddingMode.PKCS7)]
    [InlineData(PaddingMode.Zeros)]
    [InlineData(PaddingMode.ANSIX923)]
    [InlineData(PaddingMode.ISO10126)]
    public void Randomised(PaddingMode padding) {
        for (var n = 0; n < 1000; n++) {
            var crypto = new RivestCipher4Managed() { Padding = padding };
            crypto.GenerateKey();
            crypto.GenerateIV();
            var data = new byte[Random.Shared.Next(100)];
            RandomNumberGenerator.Fill(data);
            if ((padding == PaddingMode.Zeros) && (data.Length > 0)) { data[^1] = 1; }  // zero padding needs to have the last number non-zero

            var ct = Encrypt(crypto, crypto.Key, crypto.IV, data);
            if (padding is PaddingMode.None or PaddingMode.Zeros) {
                Assert.True(data.Length <= ct.Length);
            } else {
                Assert.True(data.Length < ct.Length);
            }

            var pt = Decrypt(crypto, crypto.Key, crypto.IV, ct);
            Assert.Equal(data.Length, pt.Length);
            Assert.Equal(BitConverter.ToString(data), BitConverter.ToString(pt));
        }
    }

    [Theory(DisplayName = "RivestCipher4Managed: Different block sizes")]
    [InlineData(PaddingMode.None)]
    [InlineData(PaddingMode.PKCS7)]
    [InlineData(PaddingMode.Zeros)]
    [InlineData(PaddingMode.ANSIX923)]
    [InlineData(PaddingMode.ISO10126)]
    public void DifferentBlockSizes(PaddingMode padding) {
        for (var blockSize = 8; blockSize <= 256; blockSize += 8) {
            var key = RandomNumberGenerator.GetBytes(16);
            var iv = default(byte[]);
            var data = new byte[Random.Shared.Next(100)];
            RandomNumberGenerator.Fill(data);
            if ((padding == PaddingMode.Zeros) && (data.Length > 0)) { data[^1] = 1; }  // zero padding needs to have the last number non-zero

            var crypto = new RivestCipher4Managed() { BlockSize = blockSize, Padding = padding };
            Assert.Equal(blockSize, crypto.BlockSize);

            var ct = Encrypt(crypto, key, iv, data);
            if (padding is PaddingMode.None or PaddingMode.Zeros) {
                Assert.True(data.Length <= ct.Length);
            } else {
                Assert.True(data.Length < ct.Length);
            }

            var pt = Decrypt(crypto, key, iv, ct);
            Assert.Equal(data.Length, pt.Length);
            Assert.Equal(BitConverter.ToString(data), BitConverter.ToString(pt));
        }
    }

    [Theory(DisplayName = "RivestCipher4Managed: Encrypt/Decrypt")]
    [InlineData(PaddingMode.None)]
    [InlineData(PaddingMode.PKCS7)]
    [InlineData(PaddingMode.Zeros)]
    [InlineData(PaddingMode.ANSIX923)]
    [InlineData(PaddingMode.ISO10126)]
    public void EncryptDecrypt(PaddingMode padding) {
        var crypto = new RivestCipher4Managed() { Padding = padding };
        crypto.GenerateKey();
        crypto.GenerateIV();
        var bytes = RandomNumberGenerator.GetBytes(1024);
        var bytesEnc = new byte[bytes.Length];
        var bytesDec = new byte[bytes.Length];

        var sw = Stopwatch.StartNew();
        using var encryptor = crypto.CreateEncryptor();
        using var decryptor = crypto.CreateDecryptor();
        for (var n = 0; n < 1024; n++) {
            encryptor.TransformBlock(bytes, 0, bytes.Length, bytesEnc, 0);
            decryptor.TransformBlock(bytesEnc, 0, bytesEnc.Length, bytesDec, 0);
        }

        var lastBytesEnc = encryptor.TransformFinalBlock(new byte[10], 0, 10);
        var lastBytesDec = decryptor.TransformFinalBlock(lastBytesEnc, 0, lastBytesEnc.Length);
        sw.Stop();

        Output.WriteLine($"Duration: {sw.ElapsedMilliseconds} ms");
    }


    #region Private helper

    private static byte[] Encrypt(SymmetricAlgorithm algorithm, byte[] key, byte[] iv, byte[] pt) {
        using var ms = new MemoryStream();
        using (var transform = algorithm.CreateEncryptor(key, iv)) {
            using var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
            cs.Write(pt, 0, pt.Length);
        }
        return ms.ToArray();
    }

    private static byte[] Decrypt(SymmetricAlgorithm algorithm, byte[] key, byte[] iv, byte[] ct) {
        using var ctStream = new MemoryStream(ct);
        using var transform = algorithm.CreateDecryptor(key, iv);
        using var cs = new CryptoStream(ctStream, transform, CryptoStreamMode.Read);
        using var ms = new MemoryStream();
        cs.CopyTo(ms);
        return ms.ToArray();
    }

    private static void RetrieveVectors(string fileName, out byte[] key, out Queue<KeyValuePair<int, byte[]>> data) {
        using var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Medo._Resources.Security.Cryptography.RivestCipher4." + fileName));

        key = GetBytes(reader.ReadLine().Trim(), out var headerText);
        if (!headerText.Equals("KEY", StringComparison.InvariantCultureIgnoreCase)) { throw new InvalidDataException(); }

        data = new Queue<KeyValuePair<int, byte[]>>();
        while (!reader.EndOfStream) {
            var line = reader.ReadLine();
            if (line.Length > 0) {
                var bytes = GetBytes(line, out var locationText);
                var location = int.Parse(locationText, NumberStyles.HexNumber);
                data.Enqueue(new KeyValuePair<int, byte[]>(location, bytes));
            }
        }
    }

    private static byte[] GetBytes(string lineText, out string headerText) {
        var parts = lineText.Split(":");
        if (parts.Length != 2) { throw new InvalidDataException(); }

        headerText = parts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries)[^1];
        var bytesText = parts[1].Trim().Replace(" ", "");
        if (bytesText.StartsWith("0x")) { bytesText = bytesText[2..]; }
        var data = new Queue<byte>();
        for (var i = 0; i < bytesText.Length; i += 2) {
            data.Enqueue(byte.Parse(bytesText.Substring(i, 2), NumberStyles.HexNumber));
        }
        return data.ToArray();
    }

    #endregion

}
