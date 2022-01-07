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

    #endregion

}
