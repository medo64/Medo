using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Security.Cryptography;

namespace Tests;

[TestClass]
public class CryptPassword_Tests {

    #region MD5

    [TestMethod]
    public void CryptPassword_Create_Md5() {
        var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.MD5);
        Assert.IsTrue(CryptPassword.Verify("Test", hash));
        Assert.IsFalse(CryptPassword.Verify("NotTest", hash));
        Assert.AreEqual(8, hash.Split('$')[2].Length);  // default salt length
    }

    [TestMethod]
    public void CryptPassword_Create_Md5_NoSalt() {
        var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.MD5, 0);
        Assert.AreEqual("$1$$smLce1bQjZePWXbJ5eh58/", hash);
    }

    [TestMethod]
    public void CryptPassword_Create_Md5_Long() {
        var hash = CryptPassword.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ", CryptPasswordAlgorithm.MD5);
        Assert.IsTrue(CryptPassword.Verify("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ", hash));
    }

    [TestMethod]
    public void CryptPassword_Create_Md5_16b() {
        var hash = CryptPassword.Create("1234567890123456", CryptPasswordAlgorithm.MD5, 0);
        Assert.AreEqual("$1$$RvxDspYl0hrlDuSmGR1fc/", hash);
    }

    [TestMethod]
    public void CryptPassword_Create_Md5_Verify1() {
        Assert.IsTrue(CryptPassword.Verify("Test", "$1$SALT$iYTuv61EcPDadxVotGguH0"));
        Assert.IsFalse(CryptPassword.Verify("Test", "$1$SALT$iYTuv61EcPDadxVotGguHF"));
    }

    [TestMethod]
    public void CryptPassword_Create_Md5_Verify2() {
        Assert.IsTrue(CryptPassword.Verify("password", "$1$3azHgidD$SrJPt7B.9rekpmwJwtON31"));
        Assert.IsFalse(CryptPassword.Verify("password", "$1$3azHgidD$SrJPt7B.9rekpmwJwtON30"));
    }

    [TestMethod]
    public void CryptPassword_Create_Md5_Verify_NoSalt() {
        Assert.IsTrue(CryptPassword.Verify("Test", "$1$$smLce1bQjZePWXbJ5eh58/"));
        Assert.IsFalse(CryptPassword.Verify("Test", "$1$$smLce1bQjZePWXbJ5eh580"));
    }

    #endregion

    #region MD5-Apache

    [TestMethod]
    public void CryptPassword_Create_Md5Apache() {
        var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.MD5Apache);
        Assert.IsTrue(CryptPassword.Verify("Test", hash));
        Assert.IsFalse(CryptPassword.Verify("NotTest", hash));
        Assert.AreEqual(8, hash.Split('$')[2].Length);  // default salt length
    }

    [TestMethod]
    public void CryptPassword_Create_Md5Apache_NoSalt() {
        var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.MD5Apache, 0);
        Assert.AreEqual("$apr1$$zccNMO7jOau6cLaAIpdIp1", hash);
    }

    [TestMethod]
    public void CryptPassword_Create_Md5Apache_Verify1() {
        Assert.IsTrue(CryptPassword.Verify("Test", "$apr1$8fNPKrzo$LMfFH4wsbetnSxVk8zvnL/"));
        Assert.IsFalse(CryptPassword.Verify("Test", "$apr1$8fNPKrzo$LMfFH4wsbetnSxVk8zvnL0"));
    }

    [TestMethod]
    public void CryptPassword_Create_Md5Apache_Verify2() {
        Assert.IsTrue(CryptPassword.Verify("myPassword", "$apr1$qHDFfhPC$nITSVHgYbDAK1Y0acGRnY0"));
    }

    [TestMethod]
    public void CryptPassword_Create_Md5Apache_Verify_NoSalt() {
        Assert.IsTrue(CryptPassword.Verify("Test", "$apr1$$zccNMO7jOau6cLaAIpdIp1"));
        Assert.IsFalse(CryptPassword.Verify("Test", "$apr1$$zccNMO7jOau6cLaAIpdIp0"));
    }

    #endregion

    #region SHA-256

    [TestMethod]
    public void CryptPassword_Create_Sha256() {
        var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha256);
        Assert.IsTrue(CryptPassword.Verify("Test", hash));
        Assert.IsFalse(CryptPassword.Verify("NotTest", hash));
        Assert.AreEqual(16, hash.Split('$')[2].Length);  // default salt length
    }

    [TestMethod]
    public void CryptPassword_Create_Sha256_NoSalt() {
        var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha256, 0);
        Assert.AreEqual("$5$$HznmDc1T0z.rHKK6lKLl06rT2QuK1hhSbA09Zur2KsD", hash);
    }

    [TestMethod]
    public void CryptPassword_Create_Sha256_32b() {
        var hash = CryptPassword.Create("12345678901234567890123456789012", CryptPasswordAlgorithm.Sha256, 0);
        Assert.AreEqual("$5$$aBwv.7LCzECcVRAUqSSEFrd.zN54eADoVnXZWC5res6", hash);
    }

    [TestMethod]
    public void CryptPassword_Create_Sha256_Long() {
        var hash = CryptPassword.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ", CryptPasswordAlgorithm.Sha256);
        Assert.IsTrue(CryptPassword.Verify("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ", hash));
    }

    [TestMethod]
    public void CryptPassword_Create_Sha256_ExplicitRounds() {
        var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha256, 0, 7777);
        Assert.AreEqual("$5$rounds=7777$$Z7sdS/EnisPsr1uK7pcVGQACIOOtoEREEqXJUHY.ja3", hash);
    }

    [TestMethod]
    public void CryptPassword_Create_Sha256_Verify() {
        Assert.IsTrue(CryptPassword.Verify("Test", "$5$SALT$hRDk2PDSQpm22hspBC9DW3wmKv58ZcIPLrTAI/PyBc9"));
        Assert.IsFalse(CryptPassword.Verify("Test", "$5$SALT$hRDk2PDSQpm22hspBC9DW3wmKv58ZcIPLrTAI/PyBc0"));
    }

    [TestMethod]
    public void CryptPassword_Create_Sha256_Verify_NoSalt() {
        Assert.IsTrue(CryptPassword.Verify("Test", "$5$$HznmDc1T0z.rHKK6lKLl06rT2QuK1hhSbA09Zur2KsD"));
        Assert.IsFalse(CryptPassword.Verify("Test", "$5$$HznmDc1T0z.rHKK6lKLl06rT2QuK1hhSbA09Zur2Ks0"));
    }

    [TestMethod]
    public void CryptPassword_Create_Sha256_SaltTooLong() {
        Assert.IsTrue(CryptPassword.Verify("Hello world!", "$5$rounds=10000$saltstringsaltst$3xv.VbSHBb41AL9AvLeujZkZRBAwqFMz2.opqey6IcA"));
    }

    [TestMethod]
    public void CryptPassword_Create_Sha256_ExtraLongPassword() {
        Assert.IsTrue(CryptPassword.Verify("we have a short salt string but not a short password", "$5$rounds=77777$short$JiO1O3ZpDAxGJeaDIuqCoEFysAe1mZNJRs3pw0KQRd/"));
    }

    #endregion

    #region SHA-512

    [TestMethod]
    public void CryptPassword_Create_Sha512() {
        var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha512);
        Assert.IsTrue(CryptPassword.Verify("Test", hash));
        Assert.IsFalse(CryptPassword.Verify("NotTest", hash));
        Assert.AreEqual(16, hash.Split('$')[2].Length);  // default salt length
    }

    [TestMethod]
    public void CryptPassword_Create_Sha512_NoSalt() {
        var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha512, 0);
        Assert.AreEqual("$6$$A2vGKWUkCCh28GOsloAzFlH9OgSh8Kv37fsIgM/FmwIPpmZXE/Rx6h6Fdjw7bEasMtpE/e9QQL9Te0d1pUJk./", hash);
    }

    [TestMethod]
    public void CryptPassword_Create_Sha512_64b() {
        var hash = CryptPassword.Create("1234567890123456789012345678901234567890123456789012345678901234", CryptPasswordAlgorithm.Sha512, 0);
        Assert.AreEqual("$6$$WnKefX4kEZjuyvYWY6Bf5.Us3GWgJCcwj8faQRpFtCg9/aJOhojZ1vpchMG6CmNRYbn.y/Z.l6WotGTVuFSFW0", hash);
    }

    [TestMethod]
    public void CryptPassword_Create_Sha512_ExplicitRounds() {
        var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha512, 0, 7777);
        Assert.AreEqual("$6$rounds=7777$$UNBSSrJ9WQTbHqvso9.yDg0XdJAraq1dZir/V3SPvApoa.E0ilnLP.803MJqIHjOtTvuhxGv/cAXJ0ccTpYBP1", hash);
    }

    [TestMethod]
    public void CryptPassword_Create_Sha512_Verify() {
        Assert.IsTrue(CryptPassword.Verify("Test", "$6$SALT$8GXK57PY.bq4j7Ng3f0LF6NcPXxUXqmmseKtw1ugn8uoKXiPJWG8Ub6bxJcHAPBL2y0ppLmQJcpR8mYJbdjVF1"));
        Assert.IsFalse(CryptPassword.Verify("Test", "$6$SALT$8GXK57PY.bq4j7Ng3f0LF6NcPXxUXqmmseKtw1ugn8uoKXiPJWG8Ub6bxJcHAPBL2y0ppLmQJcpR8mYJbdjVF0"));
    }

    [TestMethod]
    public void CryptPassword_Create_Sha512_Verify_NoSalt() {
        Assert.IsTrue(CryptPassword.Verify("Test", "$6$$A2vGKWUkCCh28GOsloAzFlH9OgSh8Kv37fsIgM/FmwIPpmZXE/Rx6h6Fdjw7bEasMtpE/e9QQL9Te0d1pUJk./"));
        Assert.IsFalse(CryptPassword.Verify("Test", "$6$$A2vGKWUkCCh28GOsloAzFlH9OgSh8Kv37fsIgM/FmwIPpmZXE/Rx6h6Fdjw7bEasMtpE/e9QQL9Te0d1pUJk.0"));
    }

    [TestMethod]
    public void CryptPassword_Create_Sha512_Verify_SaltTooLong() {
        Assert.IsTrue(CryptPassword.Verify("Hello world!", "$6$rounds=10000$saltstringsaltst$OW1/O6BYHV6BcXZu8QVeXbDWra3Oeqh0sbHbbMCVNSnCM/UrjmM0Dp8vOuZeHBy/YTBmSK6H9qs/y3RnOaw5v."));
    }

    [TestMethod]
    public void CryptPassword_Create_Sha512_Verify_ExtraLongPassword() {
        Assert.IsTrue(CryptPassword.Verify("a very much longer text to encrypt.  This one even stretches over morethan one line.", "$6$rounds=1400$anotherlongsalts$POfYwTEok97VWcjxIiSOjiykti.o/pQs.wPvMxQ6Fm7I6IoYN3CmLs66x9t0oSwbtEW7o7UmJEiDwGqd8p4ur1"));
    }

    #endregion

    #region Default

    [TestMethod]
    public void CryptPassword_Create_Default() {
        var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha512);
        Assert.IsTrue(hash.StartsWith("$6$", StringComparison.OrdinalIgnoreCase));
        Assert.AreEqual(106, hash.Length);  // total length
        Assert.AreEqual(16, hash.Split('$')[2].Length);  // default salt length
        Assert.AreEqual(86, hash.Split('$')[3].Length);  // hash length
    }

    #endregion

    #region Errors

    [TestMethod]
    public void CryptPassword_Validate_PasswordString_ArgumentNullException() {
        Assert.ThrowsException<ArgumentNullException>(() => {
            CryptPassword.Create(default(string));
        });
    }

    [TestMethod]
    public void CryptPassword_Validate_PasswordBytes_ArgumentNullException() {
        Assert.ThrowsException<ArgumentNullException>(() => {
            CryptPassword.Create(default(byte[]));
        });
        Assert.ThrowsException<ArgumentNullException>(() => {
            CryptPassword.Create(default(byte[]));
        });
    }

    [TestMethod]
    public void CryptPassword_Validate_Algorithm_ArgumentOutOfRangeException() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            CryptPassword.Create(Array.Empty<byte>(), 0);
        });
    }

    [TestMethod]
    public void CryptPassword_Validate_Salt_ArgumentNullException() {
        Assert.ThrowsException<ArgumentNullException>(() => {
            CryptPassword.Create(Array.Empty<byte>(), CryptPasswordAlgorithm.Sha256, null);
        });
    }

    [TestMethod]
    public void CryptPassword_Validate_SaltSize_ArgumentOutOfRangeException() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            CryptPassword.Create(Array.Empty<byte>(), CryptPasswordAlgorithm.Sha512, -2);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            CryptPassword.Create(Array.Empty<byte>(), CryptPasswordAlgorithm.Sha512, 17);
        });
    }

    [TestMethod]
    public void CryptPassword_Validate_IterationCount_ArgumentOutOfRangeException() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            CryptPassword.Create(Array.Empty<byte>(), CryptPasswordAlgorithm.MD5, CryptPassword.DefaultSaltSize, 999);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            CryptPassword.Create(Array.Empty<byte>(), CryptPasswordAlgorithm.MD5, CryptPassword.DefaultSaltSize, 1000000000);
        });
    }


    [TestMethod]
    public void CryptPassword_Input_UnknownAlgorithm() {
        Assert.IsFalse(CryptPassword.Verify("Test", "$XXX$SALT$8GXK57PY.bq4j7Ng3f0LF6NcPXxUXqmmseKtw1ugn8uoKXiPJWG8Ub6bxJcHAPBL2y0ppLmQJcpR8mYJbdjVF1"));
    }

    [TestMethod]
    public void CryptPassword_Input_NullHash() {
        Assert.IsFalse(CryptPassword.Verify("Test", null));
    }

    [TestMethod]
    public void CryptPassword_Input_EmptyHash() {
        Assert.IsFalse(CryptPassword.Verify("Test", ""));
    }

    #endregion

}
