using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Security.Cryptography;

namespace Tests;

[TestClass]
public class RivestCipher4Managed_Tests {

    [DataTestMethod]
    [DataRow("Key", "Plaintext", "BBF316E8D940AF0AD3")]
    [DataRow("Wiki", "pedia", "1021BF0420")]
    [DataRow("Secret", "Attack at dawn", "45A01F645FC35B383552544B9BF5")]
    public void RivestCipher4Managed_KnownAnswers_Basic(string keyText, string plainText, string cipherTextHex) {
        using var algorithm = new RivestCipher4Managed();

        var ct = Encrypt(algorithm,
            Encoding.ASCII.GetBytes(keyText),
            null,
            Encoding.ASCII.GetBytes(plainText));
        Assert.AreEqual(cipherTextHex, BitConverter.ToString(ct).Replace("-", ""));

        var pt = Decrypt(algorithm,
            Encoding.ASCII.GetBytes(keyText),
            null,
            ct);
        Assert.AreEqual(plainText, Encoding.ASCII.GetString(pt));
    }

    [DataTestMethod]
    [DataRow("Ke", "y", "Plaintext", "BBF316E8D940AF0AD3")]
    [DataRow("Wi", "ki", "pedia", "1021BF0420")]
    [DataRow("S", "ecret", "Attack at dawn", "45A01F645FC35B383552544B9BF5")]
    public void RivestCipher4Managed_KnownAnswers_BasicIV(string keyText, string ivText, string plainText, string cipherTextHex) {
        using var algorithm = new RivestCipher4Managed();

        var ct = Encrypt(algorithm,
            Encoding.ASCII.GetBytes(keyText),
            Encoding.ASCII.GetBytes(ivText),
            Encoding.ASCII.GetBytes(plainText));
        Assert.AreEqual(cipherTextHex, BitConverter.ToString(ct).Replace("-", ""));

        var pt = Decrypt(algorithm,
            Encoding.ASCII.GetBytes(keyText),
            Encoding.ASCII.GetBytes(ivText),
            ct);
        Assert.AreEqual(plainText, Encoding.ASCII.GetString(pt));
    }


    [DataTestMethod]
    [DataRow("Vector40A.txt")]
    [DataRow("Vector56A.txt")]
    [DataRow("Vector64A.txt")]
    [DataRow("Vector80A.txt")]
    [DataRow("Vector128A.txt")]
    [DataRow("Vector192A.txt")]
    [DataRow("Vector256A.txt")]
    [DataRow("Vector40B.txt")]
    [DataRow("Vector56B.txt")]
    [DataRow("Vector64B.txt")]
    [DataRow("Vector80B.txt")]
    [DataRow("Vector128B.txt")]
    [DataRow("Vector192B.txt")]
    [DataRow("Vector256B.txt")]
    public void RivestCipher4Managed_Vectors(string fileName) {
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
            Assert.AreEqual(BitConverter.ToString(expectedBytes), BitConverter.ToString(cipherBytes));
        }
    }

    [DataTestMethod]
    [DataRow(PaddingMode.None)]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void RivestCipher4Managed_PaddingFull(PaddingMode padding) {
        var key = new byte[16]; RandomNumberGenerator.Fill(key);
        var iv = new byte[8]; RandomNumberGenerator.Fill(iv);
        var data = new byte[48]; RandomNumberGenerator.Fill(data);  // full blocks

        var algorithm = new RivestCipher4Managed() { Padding = padding, };

        var ct = Encrypt(algorithm, key, iv, data);
        var pt = Decrypt(algorithm, key, iv, ct);
        Assert.AreEqual(data.Length, pt.Length);
        Assert.AreEqual(BitConverter.ToString(data), BitConverter.ToString(pt));
    }

    [DataTestMethod]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void RivestCipher4Managed_PaddingPartial(PaddingMode padding) {
        var key = new byte[16]; RandomNumberGenerator.Fill(key);
        var iv = new byte[8]; RandomNumberGenerator.Fill(iv);
        var data = new byte[42]; RandomNumberGenerator.Fill(data);

        var algorithm = new RivestCipher4Managed() { Padding = padding };

        var ct = Encrypt(algorithm, key, iv, data);
        var pt = Decrypt(algorithm, key, iv, ct);
        Assert.AreEqual(data.Length, pt.Length);
        Assert.AreEqual(BitConverter.ToString(data), BitConverter.ToString(pt));
    }

    [DataTestMethod]
    [DataRow(CipherMode.ECB)]
    [DataRow(CipherMode.CFB)]
    [DataRow(CipherMode.CTS)]
    public void RivestCipher4Managed_OnlyCbcSupported(CipherMode mode) {
        Assert.ThrowsException<CryptographicException>(() => {
            var _ = new RivestCipher4Managed() { Mode = mode };
        });
    }

    [DataTestMethod]
    [DataRow(PaddingMode.None)]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void RivestCipher4Managed_LargeFinalBlock(PaddingMode padding) {
        var crypto = new RivestCipher4Managed() { Padding = padding };
        crypto.GenerateKey();
        crypto.GenerateIV();
        var text = "This is a final block wider than block size.";  // more than 128 bits of data
        var bytes = Encoding.ASCII.GetBytes(text);

        using var encryptor = crypto.CreateEncryptor();
        var ct = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

        Assert.AreEqual(padding == PaddingMode.None ? bytes.Length : 48, ct.Length);

        using var decryptor = crypto.CreateDecryptor();
        var pt = decryptor.TransformFinalBlock(ct, 0, ct.Length);

        Assert.AreEqual(bytes.Length, pt.Length);
        Assert.AreEqual(text, Encoding.ASCII.GetString(pt));
    }

    [DataTestMethod]
    [DataRow(PaddingMode.None)]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void RivestCipher4Managed_BlockSizeRounding(PaddingMode padding) {
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
            Assert.AreEqual(expectedCryptLength, ct.Length);

            var pt = Decrypt(algorithm, key, iv, ct);
            Assert.AreEqual(data.Length, pt.Length);
            Assert.AreEqual(BitConverter.ToString(data), BitConverter.ToString(pt));
        }
    }

    [DataTestMethod]
    [DataRow(PaddingMode.None)]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void RivestCipher4Managed_Randomised(PaddingMode padding) {
        for (var n = 0; n < 1000; n++) {
            var crypto = new RivestCipher4Managed() { Padding = padding };
            crypto.GenerateKey();
            crypto.GenerateIV();
            var data = new byte[Random.Shared.Next(100)];
            RandomNumberGenerator.Fill(data);
            if ((padding == PaddingMode.Zeros) && (data.Length > 0)) { data[^1] = 1; }  // zero padding needs to have the last number non-zero

            var ct = Encrypt(crypto, crypto.Key, crypto.IV, data);
            if (padding is PaddingMode.None or PaddingMode.Zeros) {
                Assert.IsTrue(data.Length <= ct.Length);
            } else {
                Assert.IsTrue(data.Length < ct.Length);
            }

            var pt = Decrypt(crypto, crypto.Key, crypto.IV, ct);
            Assert.AreEqual(data.Length, pt.Length);
            Assert.AreEqual(BitConverter.ToString(data), BitConverter.ToString(pt));
        }
    }

    [DataTestMethod]
    [DataRow(PaddingMode.None)]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void RivestCipher4Managed_DifferentBlockSizes(PaddingMode padding) {
        for (var blockSize = 8; blockSize <= 256; blockSize += 8) {
            var key = RandomNumberGenerator.GetBytes(16);
            var iv = default(byte[]);
            var data = new byte[Random.Shared.Next(100)];
            RandomNumberGenerator.Fill(data);
            if ((padding == PaddingMode.Zeros) && (data.Length > 0)) { data[^1] = 1; }  // zero padding needs to have the last number non-zero

            var crypto = new RivestCipher4Managed() { BlockSize = blockSize, Padding = padding };
            Assert.AreEqual(blockSize, crypto.BlockSize);

            var ct = Encrypt(crypto, key, iv, data);
            if (padding is PaddingMode.None or PaddingMode.Zeros) {
                Assert.IsTrue(data.Length <= ct.Length);
            } else {
                Assert.IsTrue(data.Length < ct.Length);
            }

            var pt = Decrypt(crypto, key, iv, ct);
            Assert.AreEqual(data.Length, pt.Length);
            Assert.AreEqual(BitConverter.ToString(data), BitConverter.ToString(pt));
        }
    }

    [DataTestMethod]
    [DataRow(PaddingMode.None)]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void RivestCipher4Managed_EncryptDecrypt(PaddingMode padding) {
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

        Debug.WriteLine($"Duration: {sw.ElapsedMilliseconds} ms");
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
        using var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests._Resources.Security.Cryptography.RivestCipher4." + fileName));

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
