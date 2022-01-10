using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Xunit;
using Medo.Security.Cryptography;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;

namespace Tests.Medo.Security.Cryptography;

public class RivestCipher4ManagedTests {

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


    [Theory(DisplayName = "RivestCipher4Managed: Only CBC supported")]
    [InlineData(CipherMode.ECB)]
    [InlineData(CipherMode.CFB)]
    [InlineData(CipherMode.CTS)]
    public void OnlyCbcSupported(CipherMode mode) {
        Assert.Throws<CryptographicException>(() => {
            var _ = new RivestCipher4Managed() { Mode = mode };
        });
    }

    [Theory(DisplayName = "RivestCipher4Managed: No padding supported")]
    [InlineData(PaddingMode.PKCS7)]
    [InlineData(PaddingMode.Zeros)]
    [InlineData(PaddingMode.ANSIX923)]
    [InlineData(PaddingMode.ISO10126)]
    public void NoPaddingSupported(PaddingMode padding) {
        Assert.Throws<CryptographicException>(() => {
            var _ = new RivestCipher4Managed() { Padding = padding };
        });
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
