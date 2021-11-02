using System;
using System.Text;
using Xunit;
using Medo.Security.Cryptography;

namespace Tests.Medo.Security.Cryptography {
    public class OneTimePasswordTests {

        #region New

        [Fact(DisplayName = "OneTimePassword: Basic")]
        public void Basic() {
            var o1 = new OneTimePassword();
            var o2 = new OneTimePassword();

            Assert.Equal(20, o1.GetSecret().Length);
            Assert.NotEqual(BitConverter.ToString(o1.GetSecret()), BitConverter.ToString(o2.GetSecret()));
        }

        [Fact(DisplayName = "OneTimePassword: Invalid encoding")]
        public void InvalidSecretEncoding() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var o = new OneTimePassword("@");
            });
        }

        [Fact(DisplayName = "OneTimePassword: Ignore Spaces")]
        public void IgnoreSpacesInSecret() {
            var o = new OneTimePassword("MZx w6\tyT   bo I=");
            Assert.Equal("mzxw6ytboi", o.GetBase32Secret(SecretFormatFlags.None));
        }

        [Fact(DisplayName = "OneTimePassword: Secret cannot be null")]
        public void NullSecret() {
            Assert.Throws<ArgumentNullException>(() => {
                var o = new OneTimePassword(null as string);
            });
        }

        #endregion New


        #region Base32

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (empty)")]
        public void Secret_Base32_Empty() {
            var o = new OneTimePassword("");

            Assert.Equal("", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("", o.GetBase32Secret());
            Assert.Equal("", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (single character)")]
        public void Base32_SingleCharacter() {
            var o = new OneTimePassword("m");

            Assert.Equal("60", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("ma", o.GetBase32Secret());
            Assert.Equal("ma", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("ma", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("ma======", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("ma== ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (two characters)")]
        public void Base32_TwoCharacters() {
            var o = new OneTimePassword("my");

            Assert.Equal("66", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("my", o.GetBase32Secret());
            Assert.Equal("my", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("my", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("my======", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("my== ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (two characters, ignore case)")]
        public void Base32_TwoCharacters_IgnoreCase() {
            var o = new OneTimePassword("MY======");

            Assert.Equal("66", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("my", o.GetBase32Secret());
            Assert.Equal("my", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("my", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("my======", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("my== ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (three characters)")]
        public void Base32_ThreeCharacters() {
            var o = new OneTimePassword("mzx");

            Assert.Equal("66-6E", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("mzxa", o.GetBase32Secret());
            Assert.Equal("mzxa", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("mzxa", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("mzxa====", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("mzxa ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (four characters)")]
        public void Base32_FourCharacters() {
            var o = new OneTimePassword("mzxq");

            Assert.Equal("66-6F", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("mzxq", o.GetBase32Secret());
            Assert.Equal("mzxq", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("mzxq", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("mzxq====", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("mzxq ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (four characters, ignore case)")]
        public void Base32_FourCharacters_IgnoreCase() {
            var o = new OneTimePassword("MZXQ====");

            Assert.Equal("66-6F", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("mzxq", o.GetBase32Secret());
            Assert.Equal("mzxq", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("mzxq", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("mzxq====", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("mzxq ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (five characters)")]
        public void Base32_FiveCharacters() {
            var o = new OneTimePassword("mzxw6");

            Assert.Equal("66-6F-6F", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("mzxw 6", o.GetBase32Secret());
            Assert.Equal("mzxw6", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("mzxw 6", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("mzxw6===", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("mzxw 6===", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (five characters, ignore case)")]
        public void Base32_FiveCharacters_IgnoreCase() {
            var o = new OneTimePassword("MZXW6===");

            Assert.Equal("66-6F-6F", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("mzxw 6", o.GetBase32Secret());
            Assert.Equal("mzxw6", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("mzxw 6", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("mzxw6===", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("mzxw 6===", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (six characters)")]
        public void Base32_SixCharacters() {
            var o = new OneTimePassword("mzxw6y");

            Assert.Equal("66-6F-6F-60", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("mzxw 6ya", o.GetBase32Secret());
            Assert.Equal("mzxw6ya", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("mzxw 6ya", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("mzxw6ya=", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("mzxw 6ya=", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (seven characters)")]
        public void Base32_SevenCharacters() {
            var o = new OneTimePassword("mzxw6yq");

            Assert.Equal("66-6F-6F-62", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("mzxw 6yq", o.GetBase32Secret());
            Assert.Equal("mzxw6yq", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("mzxw 6yq", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("mzxw6yq=", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("mzxw 6yq=", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (sever characters, ignore case)")]
        public void Base32_SevenCharacters_IgnoreCase() {
            var o = new OneTimePassword("MZXW6YQ=");

            Assert.Equal("66-6F-6F-62", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("mzxw 6yq", o.GetBase32Secret());
            Assert.Equal("mzxw6yq", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("mzxw 6yq", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("mzxw6yq=", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("mzxw 6yq=", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (eight characters)")]
        public void Base32_EightCharacters() {
            var o = new OneTimePassword("mzxw6ytb");

            Assert.Equal("66-6F-6F-62-61", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("mzxw 6ytb", o.GetBase32Secret());
            Assert.Equal("mzxw6ytb", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("mzxw 6ytb", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("mzxw6ytb", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("mzxw 6ytb", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (eight characters, ignore case)")]
        public void Base32_EightCharacters_IgnoreCase() {
            var o = new OneTimePassword("MZXW6YTB");

            Assert.Equal("66-6F-6F-62-61", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("mzxw 6ytb", o.GetBase32Secret());
            Assert.Equal("mzxw6ytb", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("mzxw 6ytb", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("mzxw6ytb", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("mzxw 6ytb", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (nine characters)")]
        public void Base32_NineCharacters() {
            var o = new OneTimePassword("mzxw6ytbo");

            Assert.Equal("66-6F-6F-62-61-70", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("mzxw 6ytb oa", o.GetBase32Secret());
            Assert.Equal("mzxw6ytboa", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("mzxw 6ytb oa", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("mzxw6ytboa======", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("mzxw 6ytb oa== ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (ten characters)")]
        public void Base32_TenCharacters() {
            var o = new OneTimePassword("mzxw6ytboi");

            Assert.Equal("66-6F-6F-62-61-72", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("mzxw 6ytb oi", o.GetBase32Secret());
            Assert.Equal("mzxw6ytboi", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("mzxw 6ytb oi", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("mzxw6ytboi======", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("mzxw 6ytb oi== ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (ten characters, ignore case)")]
        public void Base32_TenCharacters_IgnoreCase() {
            var o = new OneTimePassword("MZXW6YTBOI======");

            Assert.Equal("66-6F-6F-62-61-72", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("mzxw 6ytb oi", o.GetBase32Secret());
            Assert.Equal("mzxw6ytboi", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("mzxw 6ytb oi", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("mzxw6ytboi======", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("mzxw 6ytb oi== ====", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        [Fact(DisplayName = "OneTimePassword: Base32 encoding (sixteen characters)")]
        public void Base32_SixteenCharacters() {
            var o = new OneTimePassword("jbsw y3dp ehpk 3pxp");

            Assert.Equal("48-65-6C-6C-6F-21-DE-AD-BE-EF", BitConverter.ToString(o.GetSecret()));
            Assert.Equal("jbsw y3dp ehpk 3pxp", o.GetBase32Secret());
            Assert.Equal("jbswy3dpehpk3pxp", o.GetBase32Secret(SecretFormatFlags.None));
            Assert.Equal("jbsw y3dp ehpk 3pxp", o.GetBase32Secret(SecretFormatFlags.Spacing));
            Assert.Equal("jbswy3dpehpk3pxp", o.GetBase32Secret(SecretFormatFlags.Padding));
            Assert.Equal("jbsw y3dp ehpk 3pxp", o.GetBase32Secret(SecretFormatFlags.Spacing | SecretFormatFlags.Padding));
        }

        #endregion


        #region HOTP

        [Fact(DisplayName = "OneTimePassword: Generate HOTP")]
        public void HOTP_Generate() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890")) { TimeStep = 0 };

            Assert.Equal(755224, o.GetCode());
            Assert.Equal(287082, o.GetCode());
            Assert.Equal(359152, o.GetCode());
            Assert.Equal(969429, o.GetCode());
            Assert.Equal(338314, o.GetCode());
            Assert.Equal(254676, o.GetCode());
            Assert.Equal(287922, o.GetCode());
            Assert.Equal(162583, o.GetCode());
            Assert.Equal(399871, o.GetCode());
            Assert.Equal(520489, o.GetCode());
        }

        [Fact(DisplayName = "OneTimePassword: Validate HOTP")]
        public void HOTP_Validate() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890")) { TimeStep = 0 };

            Assert.True(o.IsCodeValid(755224));
            Assert.True(o.IsCodeValid(287082));
            Assert.True(o.IsCodeValid(359152));
            Assert.True(o.IsCodeValid(969429));
            Assert.True(o.IsCodeValid(338314));
            Assert.True(o.IsCodeValid(254676));
            Assert.True(o.IsCodeValid(287922));
            Assert.True(o.IsCodeValid(162583));
            Assert.True(o.IsCodeValid(399871));
            Assert.True(o.IsCodeValid(520489));
        }

        [Fact(DisplayName = "OneTimePassword: Generate HOTP (SHA-1)")]
        public void HOTP_Generate_SHA1() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890")) { Digits = 8, TimeStep = 0 };

            o.Counter = 0x0000000000000001;
            Assert.Equal(94287082, o.GetCode());

            o.Counter = 0x00000000023523EC;
            Assert.Equal(07081804, o.GetCode());

            o.Counter = 0x00000000023523ED;
            Assert.Equal(14050471, o.GetCode());

            o.Counter = 0x000000000273EF07;
            Assert.Equal(89005924, o.GetCode());

            o.Counter = 0x0000000003F940AA;
            Assert.Equal(69279037, o.GetCode());

            o.Counter = 0x0000000027BC86AA;
            Assert.Equal(65353130, o.GetCode());
        }

        [Fact(DisplayName = "OneTimePassword: Validate HOTP (SHA-1)")]
        public void HOTP_Validate_SHA1() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890")) { Digits = 8, TimeStep = 0 };

            o.Counter = 0x0000000000000001;
            Assert.True(o.IsCodeValid(94287082));
            Assert.True(o.IsCodeValid(94287082));
            Assert.False(o.IsCodeValid(94287082));

            o.Counter = 0x00000000023523EC;
            Assert.True(o.IsCodeValid("0708 1804"));

            o.Counter = 0x00000000023523ED;
            Assert.True(o.IsCodeValid(14050471));

            o.Counter = 0x000000000273EF07;
            Assert.True(o.IsCodeValid(89005924));

            o.Counter = 0x0000000003F940AA;
            Assert.True(o.IsCodeValid(69279037));

            o.Counter = 0x0000000027BC86AA;
            Assert.True(o.IsCodeValid(65353130));
        }

        #endregion


        #region TOTP/8

        [Fact(DisplayName = "OneTimePassword: Generate TOTP/8 (SHA-1)")]
        public void TOTP_Generate_SHA1() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890")) {
                Digits = 8
            };

            Assert.Equal(94287082, o.GetCode(new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
            Assert.Equal(07081804, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
            Assert.Equal(14050471, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
            Assert.Equal(89005924, o.GetCode(new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
            Assert.Equal(69279037, o.GetCode(new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
            Assert.Equal(65353130, o.GetCode(new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

            Assert.Equal(94287082, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
            Assert.Equal(07081804, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
            Assert.Equal(14050471, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
            Assert.Equal(89005924, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
            Assert.Equal(69279037, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
            Assert.Equal(65353130, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

            Assert.Equal(94287082, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(07081804, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(14050471, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(89005924, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(69279037, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(65353130, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));

            Assert.Equal(o.GetCode(), o.GetCode(DateTime.UtcNow));
            Assert.Equal(o.GetCode(), o.GetCode(DateTime.Now));
        }

        [Fact(DisplayName = "OneTimePassword: Validate TOTP/8 (SHA-1)")]
        public void TOTP_Validate_SHA1() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890")) {
                Digits = 8
            };

            Assert.True(o.IsCodeValid(94287082, new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(07081804, new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(14050471, new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(89005924, new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(69279037, new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(65353130, new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

            Assert.True(o.IsCodeValid(94287082, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(07081804, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(14050471, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(89005924, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(69279037, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(65353130, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

            Assert.True(o.IsCodeValid(94287082, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(07081804, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(14050471, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(89005924, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(69279037, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(65353130, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        }


        [Fact(DisplayName = "OneTimePassword: Generate TOTP/8 (SHA-256)")]
        public void TOTP_Generate_SHA256() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890123456789012")) {
                Algorithm = OneTimePasswordAlgorithm.Sha256,
                Digits = 8
            };

            Assert.Equal(46119246, o.GetCode(new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
            Assert.Equal(68084774, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
            Assert.Equal(67062674, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
            Assert.Equal(91819424, o.GetCode(new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
            Assert.Equal(90698825, o.GetCode(new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
            Assert.Equal(77737706, o.GetCode(new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

            Assert.Equal(46119246, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
            Assert.Equal(68084774, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
            Assert.Equal(67062674, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
            Assert.Equal(91819424, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
            Assert.Equal(90698825, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
            Assert.Equal(77737706, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

            Assert.Equal(46119246, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(68084774, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(67062674, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(91819424, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(90698825, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(77737706, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));

            Assert.Equal(o.GetCode(), o.GetCode(DateTime.UtcNow));
            Assert.Equal(o.GetCode(), o.GetCode(DateTime.Now));
        }

        [Fact(DisplayName = "OneTimePassword: Validate TOTP/8 (SHA-256)")]
        public void TOTP_Validate_SHA256() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890123456789012")) {
                Algorithm = OneTimePasswordAlgorithm.Sha256,
                Digits = 8
            };

            Assert.True(o.IsCodeValid(46119246, new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(68084774, new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(67062674, new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(91819424, new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(90698825, new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(77737706, new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

            Assert.True(o.IsCodeValid(46119246, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(68084774, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(67062674, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(91819424, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(90698825, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(77737706, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

            Assert.True(o.IsCodeValid(46119246, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(68084774, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(67062674, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(91819424, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(90698825, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(77737706, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        }


        [Fact(DisplayName = "OneTimePassword: Generate TOTP/8 (SHA-512)")]
        public void TOTP_Generate_SHA512() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("1234567890123456789012345678901234567890123456789012345678901234")) {
                Algorithm = OneTimePasswordAlgorithm.Sha512,
                Digits = 8
            };

            Assert.Equal(90693936, o.GetCode(new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
            Assert.Equal(25091201, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
            Assert.Equal(99943326, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
            Assert.Equal(93441116, o.GetCode(new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
            Assert.Equal(38618901, o.GetCode(new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
            Assert.Equal(47863826, o.GetCode(new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

            Assert.Equal(90693936, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
            Assert.Equal(25091201, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
            Assert.Equal(99943326, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
            Assert.Equal(93441116, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
            Assert.Equal(38618901, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
            Assert.Equal(47863826, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

            Assert.Equal(90693936, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(25091201, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(99943326, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(93441116, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(38618901, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(47863826, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));

            Assert.Equal(o.GetCode(), o.GetCode(DateTime.UtcNow));
            Assert.Equal(o.GetCode(), o.GetCode(DateTime.Now));
        }

        [Fact(DisplayName = "OneTimePassword: Validate TOTP/8 (SHA-512)")]
        public void TOTP_Validate_SHA512() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("1234567890123456789012345678901234567890123456789012345678901234")) {
                Algorithm = OneTimePasswordAlgorithm.Sha512,
                Digits = 8
            };

            Assert.True(o.IsCodeValid(90693936, new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(25091201, new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(99943326, new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(93441116, new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(38618901, new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(47863826, new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

            Assert.True(o.IsCodeValid(90693936, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(25091201, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(99943326, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(93441116, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(38618901, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(47863826, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

            Assert.True(o.IsCodeValid(90693936, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(25091201, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(99943326, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(93441116, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(38618901, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(47863826, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        }

        #endregion


        #region TOTP/6

        [Fact(DisplayName = "OneTimePassword: Generate TOTP/6 (SHA-1)")]
        public void TOTP_Generate6_SHA1() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890")) {
                Digits = 6
            };

            Assert.Equal(287082, o.GetCode(new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
            Assert.Equal(081804, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
            Assert.Equal(050471, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
            Assert.Equal(005924, o.GetCode(new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
            Assert.Equal(279037, o.GetCode(new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
            Assert.Equal(353130, o.GetCode(new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

            Assert.Equal(287082, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
            Assert.Equal(081804, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
            Assert.Equal(050471, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
            Assert.Equal(005924, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
            Assert.Equal(279037, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
            Assert.Equal(353130, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

            Assert.Equal(287082, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(081804, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(050471, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(005924, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(279037, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(353130, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));

            Assert.Equal(o.GetCode(), o.GetCode(DateTime.UtcNow));
            Assert.Equal(o.GetCode(), o.GetCode(DateTime.Now));
        }

        [Fact(DisplayName = "OneTimePassword: Validate TOTP/6 (SHA-1)")]
        public void TOTP_Validate6_SHA1() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890")) {
                Digits = 6
            };

            Assert.True(o.IsCodeValid(287082, new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(081804, new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(050471, new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(005924, new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(279037, new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(353130, new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

            Assert.True(o.IsCodeValid(287082, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(081804, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(050471, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(005924, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(279037, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(353130, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

            Assert.True(o.IsCodeValid(287082, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(081804, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(050471, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(005924, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(279037, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(353130, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        }


        [Fact(DisplayName = "OneTimePassword: Generate TOTP/6 (SHA-256)")]
        public void TOTP_Generate6_SHA256() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890123456789012")) {
                Algorithm = OneTimePasswordAlgorithm.Sha256,
                Digits = 6
            };

            Assert.Equal(119246, o.GetCode(new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
            Assert.Equal(084774, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
            Assert.Equal(062674, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
            Assert.Equal(819424, o.GetCode(new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
            Assert.Equal(698825, o.GetCode(new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
            Assert.Equal(737706, o.GetCode(new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

            Assert.Equal(119246, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
            Assert.Equal(084774, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
            Assert.Equal(062674, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
            Assert.Equal(819424, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
            Assert.Equal(698825, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
            Assert.Equal(737706, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

            Assert.Equal(119246, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(084774, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(062674, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(819424, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(698825, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(737706, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));

            Assert.Equal(o.GetCode(), o.GetCode(DateTime.UtcNow));
            Assert.Equal(o.GetCode(), o.GetCode(DateTime.Now));
        }

        [Fact(DisplayName = "OneTimePassword: Validate TOTP/6 (SHA-256)")]
        public void TOTP_Validate6_SHA256() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("12345678901234567890123456789012")) {
                Algorithm = OneTimePasswordAlgorithm.Sha256,
                Digits = 6
            };

            Assert.True(o.IsCodeValid(119246, new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(084774, new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(062674, new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(819424, new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(698825, new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(737706, new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

            Assert.True(o.IsCodeValid(119246, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(084774, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(062674, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(819424, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(698825, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(737706, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

            Assert.True(o.IsCodeValid(119246, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(084774, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(062674, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(819424, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(698825, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(737706, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        }


        [Fact(DisplayName = "OneTimePassword: Generate TOTP/6 (SHA-512)")]
        public void TOTP_Generate6_SHA512() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("1234567890123456789012345678901234567890123456789012345678901234")) {
                Algorithm = OneTimePasswordAlgorithm.Sha512,
                Digits = 6
            };

            Assert.Equal(693936, o.GetCode(new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
            Assert.Equal(091201, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
            Assert.Equal(943326, o.GetCode(new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
            Assert.Equal(441116, o.GetCode(new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
            Assert.Equal(618901, o.GetCode(new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
            Assert.Equal(863826, o.GetCode(new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

            Assert.Equal(693936, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
            Assert.Equal(091201, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
            Assert.Equal(943326, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
            Assert.Equal(441116, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
            Assert.Equal(618901, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
            Assert.Equal(863826, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

            Assert.Equal(693936, o.GetCode(new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(091201, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(943326, o.GetCode(new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(441116, o.GetCode(new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(618901, o.GetCode(new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
            Assert.Equal(863826, o.GetCode(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));

            Assert.Equal(o.GetCode(), o.GetCode(DateTime.UtcNow));
            Assert.Equal(o.GetCode(), o.GetCode(DateTime.Now));
        }

        [Fact(DisplayName = "OneTimePassword: Validate TOTP/6 (SHA-512)")]
        public void TOTP_Validate6_SHA512() {
            var o = new OneTimePassword(ASCIIEncoding.ASCII.GetBytes("1234567890123456789012345678901234567890123456789012345678901234")) {
                Algorithm = OneTimePasswordAlgorithm.Sha512,
                Digits = 6
            };

            Assert.True(o.IsCodeValid(693936, new DateTimeOffset(1970, 01, 01, 01, 00, 59, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(091201, new DateTimeOffset(2005, 03, 18, 02, 58, 29, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(943326, new DateTimeOffset(2005, 03, 18, 02, 58, 31, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(441116, new DateTimeOffset(2009, 02, 14, 00, 31, 30, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(618901, new DateTimeOffset(2033, 05, 18, 04, 33, 20, TimeSpan.FromHours(1))));
            Assert.True(o.IsCodeValid(863826, new DateTimeOffset(2603, 10, 11, 12, 33, 20, TimeSpan.FromHours(1))));

            Assert.True(o.IsCodeValid(693936, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(091201, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(943326, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(441116, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(618901, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc)));
            Assert.True(o.IsCodeValid(863826, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

            Assert.True(o.IsCodeValid(693936, new DateTime(1970, 01, 01, 00, 00, 59, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(091201, new DateTime(2005, 03, 18, 01, 58, 29, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(943326, new DateTime(2005, 03, 18, 01, 58, 31, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(441116, new DateTime(2009, 02, 13, 23, 31, 30, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(618901, new DateTime(2033, 05, 18, 03, 33, 20, DateTimeKind.Utc).ToLocalTime()));
            Assert.True(o.IsCodeValid(863826, new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc).ToLocalTime()));
        }

        #endregion


        #region Parameters

        [Fact(DisplayName = "OneTimePassword: Digits property change")]
        public void Parameter_Digits() {
            var o = new OneTimePassword {
                Digits = 4
            };
            o.Digits = 9;
        }

        [Fact(DisplayName = "OneTimePassword: Digits property change - too short")]
        public void Parameter_Digits_TooShort() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var o = new OneTimePassword {
                    Digits = 3
                };
            });
        }

        [Fact(DisplayName = "OneTimePassword: Digits property change - too short")]
        public void Parameter_Digits_TooLong() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var o = new OneTimePassword {
                    Digits = 10
                };
            });
        }


        [Fact(DisplayName = "OneTimePassword: Counter property change")]
        public void Parameter_Counter_Change() {
            var o = new OneTimePassword() { TimeStep = 0 };
            o.Counter = 11;
        }

        [Fact(DisplayName = "OneTimePassword: Counter property change - not in TOTP mode")]
        public void Parameter_Counter_WrongMode() {
            Assert.Throws<NotSupportedException>(() => {
                var o = new OneTimePassword {
                    Counter = 11
                };
            });
        }


        [Fact(DisplayName = "OneTimePassword: Algorithm property change")]
        public void Parameter_Algorithm() {
            var o = new OneTimePassword {
                Algorithm = OneTimePasswordAlgorithm.Sha1
            };
            o.Algorithm = OneTimePasswordAlgorithm.Sha256;
            o.Algorithm = OneTimePasswordAlgorithm.Sha512;
        }

        [Fact(DisplayName = "OneTimePassword: Algorithm property change - invalid enum")]
        public void Parameter_Algorithm_OutOfRange() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var o = new OneTimePassword {
                    Algorithm = (OneTimePasswordAlgorithm)3
                };
            });
        }


        [Fact(DisplayName = "OneTimePassword: GetCode")]
        public void Parameter_GetCode() {
            var o = new OneTimePassword();
            o.GetCode();
        }

        #endregion

    }
}
