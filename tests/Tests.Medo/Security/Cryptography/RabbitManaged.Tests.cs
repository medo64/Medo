using System;
using System.IO;
using System.Security.Cryptography;
using Xunit;
using Medo.Security.Cryptography;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Tests.Medo.Security.Cryptography;

public class RabbitManagedTests {

    [Theory(DisplayName = "RabbitManaged: Test Vectors")]
    [InlineData("VectorA1.txt")]
    [InlineData("VectorA2.txt")]
    [InlineData("VectorA3.txt")]
    [InlineData("VectorA4.txt")]
    [InlineData("VectorA5.txt")]
    [InlineData("VectorA6.txt")]
    [InlineData("VectorB1.txt")]
    [InlineData("VectorB2.txt")]
    [InlineData("VectorB3.txt")]
    public void Vectors(string fileName) {
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
            Assert.Equal(BitConverter.ToString(expectedBytes), BitConverter.ToString(cipherBytes));
        }
    }

    [Theory(DisplayName = "RabbitManaged: Text")]
    [InlineData("23C2731E8B5469FD8DABB5BC592A0F3A",
        "712906405EF03201",
        "1AE2D4EDCF9B6063B00FD6FDA0B223ADED157E77031CF0440B",
        "Rabbit stream cipher test")]
    [InlineData("0EA30464E88F321047ACCCFED2AC18F0",
        "CCEBA07AE8B1FBE6",
        "747ED2055DA707D9A04F717F28F8A010DD8A84F31EE3262745",
        "Rabbit stream cipher test")]
    [InlineData("BD9E9C4E91C9B2858741C46AA91A11DE",
        "2374EC9C1026B41C",
        "81FDD1CEF6549AA55032B45197B22F0A4A043B59BE7084CA02",
        "Rabbit stream cipher test")]
    public void Examples(string keyHex, string ivHex, string cipherHex, string plainText) {
        var key = GetBytes(keyHex);
        var iv = GetBytes(ivHex);
        var cipherBytes = GetBytes(cipherHex);

        var ct = Encrypt(new RabbitManaged(), key, iv, Encoding.ASCII.GetBytes(plainText));
        Assert.Equal(BitConverter.ToString(cipherBytes), BitConverter.ToString(ct));

        var pt = Decrypt(new RabbitManaged(), key, iv, cipherBytes);
        Assert.Equal(plainText, Encoding.ASCII.GetString(pt));
    }


    [Theory(DisplayName = "RabbitManaged: Padding full blocks")]
    [InlineData(PaddingMode.None)]
    [InlineData(PaddingMode.PKCS7)]
    [InlineData(PaddingMode.Zeros)]
    [InlineData(PaddingMode.ANSIX923)]
    [InlineData(PaddingMode.ISO10126)]
    public void PaddingFull(PaddingMode padding) {
        var key = new byte[16]; RandomNumberGenerator.Fill(key);
        var iv = new byte[8]; RandomNumberGenerator.Fill(iv);
        var data = new byte[48]; RandomNumberGenerator.Fill(data);  // full blocks

        var algorithm = new RabbitManaged() { Padding = padding, };

        var ct = Encrypt(algorithm, key, iv, data);
        var pt = Decrypt(algorithm, key, iv, ct);
        Assert.Equal(data.Length, pt.Length);
        Assert.Equal(BitConverter.ToString(data), BitConverter.ToString(pt));
    }

    [Theory(DisplayName = "RabbitManaged: Padding partial blocks")]
    [InlineData(PaddingMode.PKCS7)]
    [InlineData(PaddingMode.Zeros)]
    [InlineData(PaddingMode.ANSIX923)]
    [InlineData(PaddingMode.ISO10126)]
    public void PaddingPartial(PaddingMode padding) {
        var key = new byte[16]; RandomNumberGenerator.Fill(key);
        var iv = new byte[8]; RandomNumberGenerator.Fill(iv);
        var data = new byte[42]; RandomNumberGenerator.Fill(data);

        var algorithm = new RabbitManaged() { Padding = padding };

        var ct = Encrypt(algorithm, key, iv, data);
        var pt = Decrypt(algorithm, key, iv, ct);
        Assert.Equal(data.Length, pt.Length);
        Assert.Equal(BitConverter.ToString(data), BitConverter.ToString(pt));
    }


    [Theory(DisplayName = "RabbitManaged: Only CBC supported")]
    [InlineData(CipherMode.ECB)]
    [InlineData(CipherMode.CFB)]
    [InlineData(CipherMode.CTS)]
    public void OnlyCbcSupported(CipherMode mode) {
        Assert.Throws<CryptographicException>(() => {
            var _ = new RabbitManaged() { Mode = mode };
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

    private static void RetrieveVectors(string fileName, out byte[] key, out byte[] iv, out Queue<KeyValuePair<int, byte[]>> data) {
        using var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Medo._Resources.Security.Cryptography.Rabbit." + fileName));

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
