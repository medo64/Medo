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
public class RabbitManaged_Tests {

    [DataTestMethod]
    [DataRow("VectorA1.txt")]
    [DataRow("VectorA2.txt")]
    [DataRow("VectorA3.txt")]
    [DataRow("VectorA4.txt")]
    [DataRow("VectorA5.txt")]
    [DataRow("VectorA6.txt")]
    [DataRow("VectorB1.txt")]
    [DataRow("VectorB2.txt")]
    [DataRow("VectorB3.txt")]
    public void RabbitManaged_Vectors(string fileName) {
        RetrieveVectors(fileName, out var key, out var iv, out var dataQueue);

        using var ct = new MemoryStream();
        using var transform = new RabbitManaged().CreateEncryptor(key, iv);
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
    [DataRow("23C2731E8B5469FD8DABB5BC592A0F3A",
             "712906405EF03201",
             "1AE2D4EDCF9B6063B00FD6FDA0B223ADED157E77031CF0440B",
             "Rabbit stream cipher test")]
    [DataRow("0EA30464E88F321047ACCCFED2AC18F0",
             "CCEBA07AE8B1FBE6",
             "747ED2055DA707D9A04F717F28F8A010DD8A84F31EE3262745",
             "Rabbit stream cipher test")]
    [DataRow("BD9E9C4E91C9B2858741C46AA91A11DE",
             "2374EC9C1026B41C",
             "81FDD1CEF6549AA55032B45197B22F0A4A043B59BE7084CA02",
             "Rabbit stream cipher test")]
    public void RabbitManaged_Examples(string keyHex, string ivHex, string cipherHex, string plainText) {
        var key = GetBytes(keyHex);
        var iv = GetBytes(ivHex);
        var cipherBytes = GetBytes(cipherHex);

        var ct = Encrypt(new RabbitManaged(), key, iv, Encoding.ASCII.GetBytes(plainText));
        Assert.AreEqual(BitConverter.ToString(cipherBytes), BitConverter.ToString(ct));

        var pt = Decrypt(new RabbitManaged(), key, iv, cipherBytes);
        Assert.AreEqual(plainText, Encoding.ASCII.GetString(pt));
    }


    [DataTestMethod]
    [DataRow(PaddingMode.None)]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void RabbitManaged_PaddingFull(PaddingMode padding) {
        var key = new byte[16]; RandomNumberGenerator.Fill(key);
        var iv = new byte[8]; RandomNumberGenerator.Fill(iv);
        var data = new byte[48]; RandomNumberGenerator.Fill(data);  // full blocks

        var algorithm = new RabbitManaged() { Padding = padding, };

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
    public void RabbitManaged_PaddingPartial(PaddingMode padding) {
        var key = new byte[16]; RandomNumberGenerator.Fill(key);
        var iv = new byte[8]; RandomNumberGenerator.Fill(iv);
        var data = new byte[42]; RandomNumberGenerator.Fill(data);

        var algorithm = new RabbitManaged() { Padding = padding };

        var ct = Encrypt(algorithm, key, iv, data);
        var pt = Decrypt(algorithm, key, iv, ct);
        Assert.AreEqual(data.Length, pt.Length);
        Assert.AreEqual(BitConverter.ToString(data), BitConverter.ToString(pt));
    }


    [DataTestMethod]
    [DataRow(CipherMode.ECB)]
    [DataRow(CipherMode.CFB)]
    [DataRow(CipherMode.CTS)]
    public void RabbitManaged_OnlyCbcSupported(CipherMode mode) {
        Assert.ThrowsException<CryptographicException>(() => {
            var _ = new RabbitManaged() { Mode = mode };
        });
    }

    [DataTestMethod]
    [DataRow(PaddingMode.None)]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void RabbitManaged_LargeFinalBlock(PaddingMode padding) {
        var crypto = new RabbitManaged() { Padding = padding };
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
    public void RabbitManaged_BlockSizeRounding(PaddingMode padding) {
        var key = new byte[16]; RandomNumberGenerator.Fill(key);
        var iv = new byte[8]; RandomNumberGenerator.Fill(iv);

        for (int n = 0; n < 50; n++) {
            var data = new byte[n];
            RandomNumberGenerator.Fill(data);
            if ((padding == PaddingMode.Zeros) && (data.Length > 0)) { data[^1] = 1; }  // zero padding needs to have the last number non-zero

            var algorithm = new RabbitManaged() { Padding = padding, };

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
    public void RabbitManaged_Randomised(PaddingMode padding) {
        for (var n = 0; n < 1000; n++) {
            var crypto = new RabbitManaged() { Padding = padding };
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
    public void RabbitManaged_EncryptDecrypt(PaddingMode padding) {
        var crypto = new RabbitManaged() { Padding = padding };
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

    private static void RetrieveVectors(string fileName, out byte[] key, out byte[] iv, out Queue<KeyValuePair<int, byte[]>> data) {
        using var reader = new StreamReader(Helper.GetResourceStream("Security.Cryptography.Rabbit." + fileName));

        key = GetLineBytes(reader.ReadLine().Trim(), out var headerText);
        if (!headerText.Equals("KEY", StringComparison.InvariantCultureIgnoreCase)) { throw new InvalidDataException(); }

        data = new Queue<KeyValuePair<int, byte[]>>();

        iv = GetLineBytes(reader.ReadLine().Trim(), out var ivHeader);
        if (!ivHeader.Equals("IV", StringComparison.InvariantCultureIgnoreCase)) {  // it's not IV
            var location = int.Parse(ivHeader, NumberStyles.HexNumber);
            data.Enqueue(new KeyValuePair<int, byte[]>(location, iv));
            iv = null;
        }

        while (!reader.EndOfStream) {
            var line = reader.ReadLine();
            if (line.Length > 0) {
                var bytes = GetLineBytes(line, out var locationText);
                var location = int.Parse(locationText, NumberStyles.HexNumber);
                data.Enqueue(new KeyValuePair<int, byte[]>(location, bytes));
            }
        }
    }

    private static byte[] GetLineBytes(string lineText, out string headerText) {
        var parts = lineText.Split(":");
        if (parts.Length != 2) { throw new InvalidDataException(); }

        headerText = parts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries)[^1];
        var bytesText = parts[1].Trim().Replace(" ", "");
        if (bytesText.StartsWith("0x")) { bytesText = bytesText[2..]; }
        return GetBytes(bytesText);
    }

    private static byte[] GetBytes(string bytesText) {
        var data = new Queue<byte>();
        for (var i = 0; i < bytesText.Length; i += 2) {
            data.Enqueue(byte.Parse(bytesText.Substring(i, 2), NumberStyles.HexNumber));
        }
        return data.ToArray();
    }

    #endregion

}
