using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Security.Cryptography;

namespace Tests;

[TestClass]
public class OneTimePassword_Tests {

    #region New

    [TestMethod]
    public void OneTimePassword_Basic() {
        var o1 = new OneTimePassword();
        var o2 = new OneTimePassword();

        Assert.AreEqual(20, o1.GetSecret().Length);
        Assert.AreNotEqual(BitConverter.ToString(o1.GetSecret()), BitConverter.ToString(o2.GetSecret()));
    }

    [TestMethod]
    public void OneTimePassword_InvalidSecretEncoding() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var o = new OneTimePassword("@");
        });
    }

    [TestMethod]
    public void OneTimePassword_IgnoreSpacesInSecret() {
        var o = new OneTimePassword("MZx w6\tyT   bo I=");
        Assert.AreEqual("mzxw6ytboi", o.GetBase32Secret(SecretFormatFlags.None));
    }

    [TestMethod]
    public void OneTimePassword_NullSecret() {
        Assert.ThrowsException<ArgumentNullException>(() => {
            var o = new OneTimePassword(null as string);
        });
    }

    #endregion New


    #region Base32

    [TestMethod]
    public void OneTimePassword_Secret_Base32_Empty() {
        var o = new OneTimePassword("");

        Assert.AreEqual("", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("", o.GetBase32Secret());
        Assert.AreEqual("", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_SingleCharacter() {
        var o = new OneTimePassword("m");

        Assert.AreEqual("60", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("ma", o.GetBase32Secret());
        Assert.AreEqual("ma", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("ma", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("ma======", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("ma== ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_TwoCharacters() {
        var o = new OneTimePassword("my");

        Assert.AreEqual("66", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("my", o.GetBase32Secret());
        Assert.AreEqual("my", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("my", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("my======", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("my== ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_TwoCharacters_IgnoreCase() {
        var o = new OneTimePassword("MY======");

        Assert.AreEqual("66", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("my", o.GetBase32Secret());
        Assert.AreEqual("my", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("my", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("my======", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("my== ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_ThreeCharacters() {
        var o = new OneTimePassword("mzx");

        Assert.AreEqual("66-6E", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("mzxa", o.GetBase32Secret());
        Assert.AreEqual("mzxa", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("mzxa", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("mzxa====", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("mzxa ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_FourCharacters() {
        var o = new OneTimePassword("mzxq");

        Assert.AreEqual("66-6F", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("mzxq", o.GetBase32Secret());
        Assert.AreEqual("mzxq", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("mzxq", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("mzxq====", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("mzxq ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_FourCharacters_IgnoreCase() {
        var o = new OneTimePassword("MZXQ====");

        Assert.AreEqual("66-6F", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("mzxq", o.GetBase32Secret());
        Assert.AreEqual("mzxq", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("mzxq", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("mzxq====", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("mzxq ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_FiveCharacters() {
        var o = new OneTimePassword("mzxw6");

        Assert.AreEqual("66-6F-6F", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("mzxw 6", o.GetBase32Secret());
        Assert.AreEqual("mzxw6", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("mzxw 6", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("mzxw6===", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("mzxw 6===", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_FiveCharacters_IgnoreCase() {
        var o = new OneTimePassword("MZXW6===");

        Assert.AreEqual("66-6F-6F", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("mzxw 6", o.GetBase32Secret());
        Assert.AreEqual("mzxw6", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("mzxw 6", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("mzxw6===", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("mzxw 6===", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_SixCharacters() {
        var o = new OneTimePassword("mzxw6y");

        Assert.AreEqual("66-6F-6F-60", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("mzxw 6ya", o.GetBase32Secret());
        Assert.AreEqual("mzxw6ya", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("mzxw 6ya", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("mzxw6ya=", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("mzxw 6ya=", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_SevenCharacters() {
        var o = new OneTimePassword("mzxw6yq");

        Assert.AreEqual("66-6F-6F-62", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("mzxw 6yq", o.GetBase32Secret());
        Assert.AreEqual("mzxw6yq", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("mzxw 6yq", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("mzxw6yq=", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("mzxw 6yq=", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_SevenCharacters_IgnoreCase() {
        var o = new OneTimePassword("MZXW6YQ=");

        Assert.AreEqual("66-6F-6F-62", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("mzxw 6yq", o.GetBase32Secret());
        Assert.AreEqual("mzxw6yq", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("mzxw 6yq", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("mzxw6yq=", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("mzxw 6yq=", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_EightCharacters() {
        var o = new OneTimePassword("mzxw6ytb");

        Assert.AreEqual("66-6F-6F-62-61", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("mzxw 6ytb", o.GetBase32Secret());
        Assert.AreEqual("mzxw6ytb", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("mzxw 6ytb", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("mzxw6ytb", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("mzxw 6ytb", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_EightCharacters_IgnoreCase() {
        var o = new OneTimePassword("MZXW6YTB");

        Assert.AreEqual("66-6F-6F-62-61", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("mzxw 6ytb", o.GetBase32Secret());
        Assert.AreEqual("mzxw6ytb", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("mzxw 6ytb", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("mzxw6ytb", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("mzxw 6ytb", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_NineCharacters() {
        var o = new OneTimePassword("mzxw6ytbo");

        Assert.AreEqual("66-6F-6F-62-61-70", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("mzxw 6ytb oa", o.GetBase32Secret());
        Assert.AreEqual("mzxw6ytboa", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("mzxw 6ytb oa", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("mzxw6ytboa======", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("mzxw 6ytb oa== ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_TenCharacters() {
        var o = new OneTimePassword("mzxw6ytboi");

        Assert.AreEqual("66-6F-6F-62-61-72", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("mzxw 6ytb oi", o.GetBase32Secret());
        Assert.AreEqual("mzxw6ytboi", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("mzxw 6ytb oi", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("mzxw6ytboi======", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("mzxw 6ytb oi== ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_TenCharacters_IgnoreCase() {
        var o = new OneTimePassword("MZXW6YTBOI======");

        Assert.AreEqual("66-6F-6F-62-61-72", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("mzxw 6ytb oi", o.GetBase32Secret());
        Assert.AreEqual("mzxw6ytboi", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("mzxw 6ytb oi", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("mzxw6ytboi======", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("mzxw 6ytb oi== ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    [TestMethod]
    public void OneTimePassword_Base32_SixteenCharacters() {
        var o = new OneTimePassword("jbsw y3dp ehpk 3pxp");

        Assert.AreEqual("48-65-6C-6C-6F-21-DE-AD-BE-EF", BitConverter.ToString(o.GetSecret()));
        Assert.AreEqual("jbsw y3dp ehpk 3pxp", o.GetBase32Secret());
        Assert.AreEqual("jbswy3dpehpk3pxp", o.GetBase32Secret(SecretFormatFlags.None));
        Assert.AreEqual("jbsw y3dp ehpk 3pxp", o.GetBase32Secret(SecretFormatFlags.Spacing));
        Assert.AreEqual("jbswy3dpehpk3pxp", o.GetBase32Secret(SecretFormatFlags.Padding));
        Assert.AreEqual("jbsw y3dp ehpk 3pxp", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
    }

    #endregion


    #region HOTP

    [TestMethod]
    public void OneTimePassword_HOTP_Generate() {
        var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890")) { TimeStep = 0 };

        Assert.AreEqual(755224, o.GetCode());
        Assert.AreEqual(287082, o.GetCode());
        Assert.AreEqual(359152, o.GetCode());
        Assert.AreEqual(969429, o.GetCode());
        Assert.AreEqual(338314, o.GetCode());
        Assert.AreEqual(254676, o.GetCode());
        Assert.AreEqual(287922, o.GetCode());
        Assert.AreEqual(162583, o.GetCode());
        Assert.AreEqual(399871, o.GetCode());
        Assert.AreEqual(520489, o.GetCode());
    }

    [TestMethod]
    public void OneTimePassword_HOTP_Validate() {
        var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890")) { TimeStep = 0 };

        Assert.IsTrue(o.IsCodeValid(755224));
        Assert.IsTrue(o.IsCodeValid(287082));
        Assert.IsTrue(o.IsCodeValid(359152));
        Assert.IsTrue(o.IsCodeValid(969429));
        Assert.IsTrue(o.IsCodeValid(338314));
        Assert.IsTrue(o.IsCodeValid(254676));
        Assert.IsTrue(o.IsCodeValid(287922));
        Assert.IsTrue(o.IsCodeValid(162583));
        Assert.IsTrue(o.IsCodeValid(399871));
        Assert.IsTrue(o.IsCodeValid(520489));
    }

    [TestMethod]
    public void OneTimePassword_HOTP_Generate_SHA1() {
        var o = new OneTimePassword(Encoding.ASCII.GetBytes("12345678901234567890")) { Digits = 8, TimeStep = 0 };

        o.Counter = 0x0000000000000001;
        Assert.AreEqual(94287082, o.GetCode());

        o.Counter = 0x00000000023523EC;
        Assert.AreEqual(07081804, o.GetCode());

        o.Counter = 0x00000000023523ED;
        Assert.AreEqual(14050471, o.GetCode());

        o.Counter = 0x000000000273EF07;
        Assert.AreEqual(89005924, o.GetCode());

        o.Counter = 0x0000000003F940AA;
        Assert.AreEqual(69279037, o.GetCode());

        o.Counter = 0x0000000027BC86AA;
        Assert.AreEqual(65353130, o.GetCode());
    }

    [TestMethod]
    public void OneTimePassword_HOTP_Validate_SHA1() {
        var o = new OneTimePassword(Encoding.ASCII.GetBytes("12345678901234567890")) { Digits = 8, TimeStep = 0 };

        o.Counter = 0x0000000000000001;
        Assert.IsTrue(o.IsCodeValid(94287082));
        Assert.IsTrue(o.IsCodeValid(94287082));
        Assert.IsFalse(o.IsCodeValid(94287082));

        o.Counter = 0x00000000023523EC;
        Assert.IsTrue(o.IsCodeValid("0708 1804"));

        o.Counter = 0x00000000023523ED;
        Assert.IsTrue(o.IsCodeValid(14050471));

        o.Counter = 0x000000000273EF07;
        Assert.IsTrue(o.IsCodeValid(89005924));

        o.Counter = 0x0000000003F940AA;
        Assert.IsTrue(o.IsCodeValid(69279037));

        o.Counter = 0x0000000027BC86AA;
        Assert.IsTrue(o.IsCodeValid(65353130));
    }

    #endregion


    #region TOTP/8

    [TestMethod]
    public void OneTimePassword_TOTP_Generate_SHA1() {
        var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890")) {
            Digits = 8
        };

        Assert.AreEqual(94287082, o.GetCode(new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
        Assert.AreEqual(07081804, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
        Assert.AreEqual(14050471, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
        Assert.AreEqual(89005924, o.GetCode(new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
        Assert.AreEqual(69279037, o.GetCode(new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
        Assert.AreEqual(65353130, o.GetCode(new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

        Assert.AreEqual(94287082, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
        Assert.AreEqual(07081804, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
        Assert.AreEqual(14050471, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
        Assert.AreEqual(89005924, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
        Assert.AreEqual(69279037, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
        Assert.AreEqual(65353130, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

        Assert.AreEqual(94287082, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(07081804, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(14050471, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(89005924, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(69279037, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(65353130, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));

        Assert.AreEqual(o.GetCode(), o.GetCode(DateTime.UtcNow));
        Assert.AreEqual(o.GetCode(), o.GetCode(DateTime.Now));
    }

    [TestMethod]
    public void OneTimePassword_TOTP_Validate_SHA1() {
        var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890")) {
            Digits = 8
        };

        Assert.IsTrue(o.IsCodeValid(94287082, new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(07081804, new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(14050471, new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(89005924, new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(69279037, new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(65353130, new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

        Assert.IsTrue(o.IsCodeValid(94287082, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(07081804, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(14050471, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(89005924, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(69279037, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(65353130, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

        Assert.IsTrue(o.IsCodeValid(94287082, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(07081804, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(14050471, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(89005924, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(69279037, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(65353130, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));
    }


    [TestMethod]
    public void OneTimePassword_TOTP_Generate_SHA256() {
        var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890123456789012")) {
            Algorithm = OneTimePasswordAlgorithm.Sha256,
            Digits = 8
        };

        Assert.AreEqual(46119246, o.GetCode(new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
        Assert.AreEqual(68084774, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
        Assert.AreEqual(67062674, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
        Assert.AreEqual(91819424, o.GetCode(new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
        Assert.AreEqual(90698825, o.GetCode(new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
        Assert.AreEqual(77737706, o.GetCode(new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

        Assert.AreEqual(46119246, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
        Assert.AreEqual(68084774, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
        Assert.AreEqual(67062674, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
        Assert.AreEqual(91819424, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
        Assert.AreEqual(90698825, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
        Assert.AreEqual(77737706, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

        Assert.AreEqual(46119246, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(68084774, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(67062674, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(91819424, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(90698825, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(77737706, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));

        Assert.AreEqual(o.GetCode(), o.GetCode(DateTime.UtcNow));
        Assert.AreEqual(o.GetCode(), o.GetCode(DateTime.Now));
    }

    [TestMethod]
    public void OneTimePassword_TOTP_Validate_SHA256() {
        var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890123456789012")) {
            Algorithm = OneTimePasswordAlgorithm.Sha256,
            Digits = 8
        };

        Assert.IsTrue(o.IsCodeValid(46119246, new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(68084774, new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(67062674, new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(91819424, new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(90698825, new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(77737706, new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

        Assert.IsTrue(o.IsCodeValid(46119246, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(68084774, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(67062674, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(91819424, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(90698825, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(77737706, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

        Assert.IsTrue(o.IsCodeValid(46119246, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(68084774, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(67062674, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(91819424, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(90698825, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(77737706, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));
    }


    [TestMethod]
    public void OneTimePassword_TOTP_Generate_SHA512() {
        var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("1234567890123456789012345678901234567890123456789012345678901234")) {
            Algorithm = OneTimePasswordAlgorithm.Sha512,
            Digits = 8
        };

        Assert.AreEqual(90693936, o.GetCode(new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
        Assert.AreEqual(25091201, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
        Assert.AreEqual(99943326, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
        Assert.AreEqual(93441116, o.GetCode(new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
        Assert.AreEqual(38618901, o.GetCode(new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
        Assert.AreEqual(47863826, o.GetCode(new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

        Assert.AreEqual(90693936, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
        Assert.AreEqual(25091201, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
        Assert.AreEqual(99943326, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
        Assert.AreEqual(93441116, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
        Assert.AreEqual(38618901, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
        Assert.AreEqual(47863826, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

        Assert.AreEqual(90693936, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(25091201, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(99943326, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(93441116, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(38618901, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(47863826, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));

        Assert.AreEqual(o.GetCode(), o.GetCode(DateTime.UtcNow));
        Assert.AreEqual(o.GetCode(), o.GetCode(DateTime.Now));
    }

    [TestMethod]
    public void OneTimePassword_TOTP_Validate_SHA512() {
        var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("1234567890123456789012345678901234567890123456789012345678901234")) {
            Algorithm = OneTimePasswordAlgorithm.Sha512,
            Digits = 8
        };

        Assert.IsTrue(o.IsCodeValid(90693936, new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(25091201, new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(99943326, new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(93441116, new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(38618901, new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(47863826, new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

        Assert.IsTrue(o.IsCodeValid(90693936, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(25091201, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(99943326, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(93441116, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(38618901, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(47863826, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

        Assert.IsTrue(o.IsCodeValid(90693936, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(25091201, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(99943326, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(93441116, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(38618901, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(47863826, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));
    }

    #endregion


    #region TOTP/6

    [TestMethod]
    public void OneTimePassword_TOTP_Generate6_SHA1() {
        var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890")) {
            Digits = 6
        };

        Assert.AreEqual(287082, o.GetCode(new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
        Assert.AreEqual(081804, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
        Assert.AreEqual(050471, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
        Assert.AreEqual(005924, o.GetCode(new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
        Assert.AreEqual(279037, o.GetCode(new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
        Assert.AreEqual(353130, o.GetCode(new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

        Assert.AreEqual(287082, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
        Assert.AreEqual(081804, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
        Assert.AreEqual(050471, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
        Assert.AreEqual(005924, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
        Assert.AreEqual(279037, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
        Assert.AreEqual(353130, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

        Assert.AreEqual(287082, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(081804, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(050471, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(005924, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(279037, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(353130, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));

        Assert.AreEqual(o.GetCode(), o.GetCode(DateTime.UtcNow));
        Assert.AreEqual(o.GetCode(), o.GetCode(DateTime.Now));
    }

    [TestMethod]
    public void OneTimePassword_TOTP_Validate6_SHA1() {
        var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890")) {
            Digits = 6
        };

        Assert.IsTrue(o.IsCodeValid(287082, new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(081804, new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(050471, new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(005924, new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(279037, new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(353130, new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

        Assert.IsTrue(o.IsCodeValid(287082, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(081804, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(050471, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(005924, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(279037, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(353130, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

        Assert.IsTrue(o.IsCodeValid(287082, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(081804, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(050471, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(005924, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(279037, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(353130, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));
    }


    [TestMethod]
    public void OneTimePassword_TOTP_Generate6_SHA256() {
        var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890123456789012")) {
            Algorithm = OneTimePasswordAlgorithm.Sha256,
            Digits = 6
        };

        Assert.AreEqual(119246, o.GetCode(new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
        Assert.AreEqual(084774, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
        Assert.AreEqual(062674, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
        Assert.AreEqual(819424, o.GetCode(new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
        Assert.AreEqual(698825, o.GetCode(new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
        Assert.AreEqual(737706, o.GetCode(new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

        Assert.AreEqual(119246, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
        Assert.AreEqual(084774, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
        Assert.AreEqual(062674, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
        Assert.AreEqual(819424, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
        Assert.AreEqual(698825, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
        Assert.AreEqual(737706, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

        Assert.AreEqual(119246, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(084774, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(062674, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(819424, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(698825, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(737706, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));

        Assert.AreEqual(o.GetCode(), o.GetCode(DateTime.UtcNow));
        Assert.AreEqual(o.GetCode(), o.GetCode(DateTime.Now));
    }

    [TestMethod]
    public void OneTimePassword_TOTP_Validate6_SHA256() {
        var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890123456789012")) {
            Algorithm = OneTimePasswordAlgorithm.Sha256,
            Digits = 6
        };

        Assert.IsTrue(o.IsCodeValid(119246, new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(084774, new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(062674, new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(819424, new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(698825, new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(737706, new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

        Assert.IsTrue(o.IsCodeValid(119246, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(084774, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(062674, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(819424, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(698825, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(737706, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

        Assert.IsTrue(o.IsCodeValid(119246, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(084774, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(062674, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(819424, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(698825, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(737706, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));
    }


    [TestMethod]
    public void OneTimePassword_TOTP_Generate6_SHA512() {
        var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("1234567890123456789012345678901234567890123456789012345678901234")) {
            Algorithm = OneTimePasswordAlgorithm.Sha512,
            Digits = 6
        };

        Assert.AreEqual(693936, o.GetCode(new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
        Assert.AreEqual(091201, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
        Assert.AreEqual(943326, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
        Assert.AreEqual(441116, o.GetCode(new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
        Assert.AreEqual(618901, o.GetCode(new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
        Assert.AreEqual(863826, o.GetCode(new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

        Assert.AreEqual(693936, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
        Assert.AreEqual(091201, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
        Assert.AreEqual(943326, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
        Assert.AreEqual(441116, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
        Assert.AreEqual(618901, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
        Assert.AreEqual(863826, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

        Assert.AreEqual(693936, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(091201, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(943326, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(441116, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(618901, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        Assert.AreEqual(863826, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));

        Assert.AreEqual(o.GetCode(), o.GetCode(DateTime.UtcNow));
        Assert.AreEqual(o.GetCode(), o.GetCode(DateTime.Now));
    }

    [TestMethod]
    public void OneTimePassword_TOTP_Validate6_SHA512() {
        var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("1234567890123456789012345678901234567890123456789012345678901234")) {
            Algorithm = OneTimePasswordAlgorithm.Sha512,
            Digits = 6
        };

        Assert.IsTrue(o.IsCodeValid(693936, new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(091201, new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(943326, new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(441116, new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(618901, new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
        Assert.IsTrue(o.IsCodeValid(863826, new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

        Assert.IsTrue(o.IsCodeValid(693936, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(091201, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(943326, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(441116, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(618901, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
        Assert.IsTrue(o.IsCodeValid(863826, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

        Assert.IsTrue(o.IsCodeValid(693936, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(091201, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(943326, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(441116, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(618901, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        Assert.IsTrue(o.IsCodeValid(863826, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));
    }

    #endregion


    #region Parameters

    [TestMethod]
    public void OneTimePassword_Parameter_Digits() {
        var o = new OneTimePassword {
            Digits = 4
        };
        o.Digits = 9;
    }

    [TestMethod]
    public void OneTimePassword_Parameter_Digits_TooShort() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var o = new OneTimePassword {
                Digits = 3
            };
        });
    }

    [TestMethod]
    public void OneTimePassword_Parameter_Digits_TooLong() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var o = new OneTimePassword {
                Digits = 10
            };
        });
    }


    [TestMethod]
    public void OneTimePassword_Parameter_Counter_Change() {
        var o = new OneTimePassword() { TimeStep = 0 };
        o.Counter = 11;
    }

    [TestMethod]
    public void OneTimePassword_Parameter_Counter_WrongMode() {
        Assert.ThrowsException<NotSupportedException>(() => {
            var o = new OneTimePassword {
                Counter = 11
            };
        });
    }


    [TestMethod]
    public void OneTimePassword_Parameter_Algorithm() {
        var o = new OneTimePassword {
            Algorithm = OneTimePasswordAlgorithm.Sha1
        };
        o.Algorithm = OneTimePasswordAlgorithm.Sha256;
        o.Algorithm = OneTimePasswordAlgorithm.Sha512;
    }

    [TestMethod]
    public void OneTimePassword_Parameter_Algorithm_OutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var o = new OneTimePassword {
                Algorithm = (OneTimePasswordAlgorithm)3
            };
        });
    }


    [TestMethod]
    public void OneTimePassword_Parameter_GetCode() {
        var o = new OneTimePassword();
        o.GetCode();
    }

    #endregion

}
