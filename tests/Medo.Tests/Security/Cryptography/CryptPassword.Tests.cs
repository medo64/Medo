using System;
using Xunit;

namespace Medo.Tests.Security.Cryptography.CryptPassword {
    using Medo.Security.Cryptography;

    public class Tests {

        #region MD5

        [Fact(DisplayName = "CryptPassword: Create MD5")]
        public void Create_Md5() {
            var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.MD5);
            Assert.True(CryptPassword.Verify("Test", hash));
            Assert.False(CryptPassword.Verify("NotTest", hash));
            Assert.Equal(8, hash.Split('$')[2].Length);  // default salt length
        }

        [Fact(DisplayName = "CryptPassword: Create MD5 NoSalt")]
        public void Create_Md5_NoSalt() {
            var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.MD5, 0);
            Assert.Equal("$1$$smLce1bQjZePWXbJ5eh58/", hash);
        }

        [Fact(DisplayName = "CryptPassword: Create MD5 Long")]
        public void Create_Md5_Long() {
            var hash = CryptPassword.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ", CryptPasswordAlgorithm.MD5);
            Assert.True(CryptPassword.Verify("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ", hash));
        }

        [Fact(DisplayName = "CryptPassword: Create MD5 16b")]
        public void Create_Md5_16b() {
            var hash = CryptPassword.Create("1234567890123456", CryptPasswordAlgorithm.MD5, 0);
            Assert.Equal("$1$$RvxDspYl0hrlDuSmGR1fc/", hash);
        }

        [Fact(DisplayName = "CryptPassword: Create MD5 Verify")]
        public void Create_Md5_Verify1() {
            Assert.True(CryptPassword.Verify("Test", "$1$SALT$iYTuv61EcPDadxVotGguH0"));
            Assert.False(CryptPassword.Verify("Test", "$1$SALT$iYTuv61EcPDadxVotGguHF"));
        }

        [Fact(DisplayName = "CryptPassword: Create MD5 Verify2")]
        public void Create_Md5_Verify2() {
            Assert.True(CryptPassword.Verify("password", "$1$3azHgidD$SrJPt7B.9rekpmwJwtON31"));
            Assert.False(CryptPassword.Verify("password", "$1$3azHgidD$SrJPt7B.9rekpmwJwtON30"));
        }

        [Fact(DisplayName = "CryptPassword: Create MD5 VerifyNoSalt")]
        public void Create_Md5_Verify_NoSalt() {
            Assert.True(CryptPassword.Verify("Test", "$1$$smLce1bQjZePWXbJ5eh58/"));
            Assert.False(CryptPassword.Verify("Test", "$1$$smLce1bQjZePWXbJ5eh580"));
        }

        #endregion

        #region MD5-Apache

        [Fact(DisplayName = "CryptPassword: Create MD5Apache")]
        public void Create_Md5Apache() {
            var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.MD5Apache);
            Assert.True(CryptPassword.Verify("Test", hash));
            Assert.False(CryptPassword.Verify("NotTest", hash));
            Assert.Equal(8, hash.Split('$')[2].Length);  // default salt length
        }

        [Fact(DisplayName = "CryptPassword: Create MD5Apache NoSalt")]
        public void Create_Md5Apache_NoSalt() {
            var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.MD5Apache, 0);
            Assert.Equal("$apr1$$zccNMO7jOau6cLaAIpdIp1", hash);
        }

        [Fact(DisplayName = "CryptPassword: Create MD5Apache Verify1")]
        public void Create_Md5Apache_Verify1() {
            Assert.True(CryptPassword.Verify("Test", "$apr1$8fNPKrzo$LMfFH4wsbetnSxVk8zvnL/"));
            Assert.False(CryptPassword.Verify("Test", "$apr1$8fNPKrzo$LMfFH4wsbetnSxVk8zvnL0"));
        }

        [Fact(DisplayName = "CryptPassword: Create MD5Apache Verify2")]
        public void Create_Md5Apache_Verify2() {
            Assert.True(CryptPassword.Verify("myPassword", "$apr1$qHDFfhPC$nITSVHgYbDAK1Y0acGRnY0"));
        }

        [Fact(DisplayName = "CryptPassword: Create MD5Apache VerifyNoSalt")]
        public void Create_Md5Apache_Verify_NoSalt() {
            Assert.True(CryptPassword.Verify("Test", "$apr1$$zccNMO7jOau6cLaAIpdIp1"));
            Assert.False(CryptPassword.Verify("Test", "$apr1$$zccNMO7jOau6cLaAIpdIp0"));
        }

        #endregion

        #region SHA-256

        [Fact(DisplayName = "CryptPassword: Create SHA256")]
        public void Create_Sha256() {
            var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha256);
            Assert.True(CryptPassword.Verify("Test", hash));
            Assert.False(CryptPassword.Verify("NotTest", hash));
            Assert.Equal(16, hash.Split('$')[2].Length);  // default salt length
        }

        [Fact(DisplayName = "CryptPassword: Create SHA256")]
        public void Create_Sha256_NoSalt() {
            var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha256, 0);
            Assert.Equal("$5$$HznmDc1T0z.rHKK6lKLl06rT2QuK1hhSbA09Zur2KsD", hash);
        }

        [Fact(DisplayName = "CryptPassword: Create SHA256 32b")]
        public void Create_Sha256_32b() {
            var hash = CryptPassword.Create("12345678901234567890123456789012", CryptPasswordAlgorithm.Sha256, 0);
            Assert.Equal("$5$$aBwv.7LCzECcVRAUqSSEFrd.zN54eADoVnXZWC5res6", hash);
        }

        [Fact(DisplayName = "CryptPassword: Create SHA256 Long")]
        public void Create_Sha256_Long() {
            var hash = CryptPassword.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ", CryptPasswordAlgorithm.Sha256);
            Assert.True(CryptPassword.Verify("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ", hash));
        }

        [Fact(DisplayName = "CryptPassword: Create SHA256 ExplicitRounds")]
        public void Create_Sha256_ExplicitRounds() {
            var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha256, 0, 7777);
            Assert.Equal("$5$rounds=7777$$Z7sdS/EnisPsr1uK7pcVGQACIOOtoEREEqXJUHY.ja3", hash);
        }

        [Fact(DisplayName = "CryptPassword: Create SHA256 Verify")]
        public void Create_Sha256_Verify() {
            Assert.True(CryptPassword.Verify("Test", "$5$SALT$hRDk2PDSQpm22hspBC9DW3wmKv58ZcIPLrTAI/PyBc9"));
            Assert.False(CryptPassword.Verify("Test", "$5$SALT$hRDk2PDSQpm22hspBC9DW3wmKv58ZcIPLrTAI/PyBc0"));
        }

        [Fact(DisplayName = "CryptPassword: Create SHA256 Verify NoSalt")]
        public void Create_Sha256_Verify_NoSalt() {
            Assert.True(CryptPassword.Verify("Test", "$5$$HznmDc1T0z.rHKK6lKLl06rT2QuK1hhSbA09Zur2KsD"));
            Assert.False(CryptPassword.Verify("Test", "$5$$HznmDc1T0z.rHKK6lKLl06rT2QuK1hhSbA09Zur2Ks0"));
        }

        [Fact(DisplayName = "CryptPassword: Create SHA256 SaltTooLong")]
        public void Create_Sha256_SaltTooLong() {
            Assert.True(CryptPassword.Verify("Hello world!", "$5$rounds=10000$saltstringsaltst$3xv.VbSHBb41AL9AvLeujZkZRBAwqFMz2.opqey6IcA"));
        }

        [Fact(DisplayName = "CryptPassword: Create SHA256 ExtraLongPassword")]
        public void Create_Sha256_ExtraLongPassword() {
            Assert.True(CryptPassword.Verify("we have a short salt string but not a short password", "$5$rounds=77777$short$JiO1O3ZpDAxGJeaDIuqCoEFysAe1mZNJRs3pw0KQRd/"));
        }

        #endregion

        #region SHA-512

        [Fact(DisplayName = "CryptPassword: Create SHA512")]
        public void Create_Sha512() {
            var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha512);
            Assert.True(CryptPassword.Verify("Test", hash));
            Assert.False(CryptPassword.Verify("NotTest", hash));
            Assert.Equal(16, hash.Split('$')[2].Length);  // default salt length
        }

        [Fact(DisplayName = "CryptPassword: Create SHA512 NoSalt")]
        public void Create_Sha512_NoSalt() {
            var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha512, 0);
            Assert.Equal("$6$$A2vGKWUkCCh28GOsloAzFlH9OgSh8Kv37fsIgM/FmwIPpmZXE/Rx6h6Fdjw7bEasMtpE/e9QQL9Te0d1pUJk./", hash);
        }

        [Fact(DisplayName = "CryptPassword: Create SHA512 64b")]
        public void Create_Sha512_64b() {
            var hash = CryptPassword.Create("1234567890123456789012345678901234567890123456789012345678901234", CryptPasswordAlgorithm.Sha512, 0);
            Assert.Equal("$6$$WnKefX4kEZjuyvYWY6Bf5.Us3GWgJCcwj8faQRpFtCg9/aJOhojZ1vpchMG6CmNRYbn.y/Z.l6WotGTVuFSFW0", hash);
        }

        [Fact(DisplayName = "CryptPassword: Create SHA512 ExplicitRounds")]
        public void Create_Sha512_ExplicitRounds() {
            var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha512, 0, 7777);
            Assert.Equal("$6$rounds=7777$$UNBSSrJ9WQTbHqvso9.yDg0XdJAraq1dZir/V3SPvApoa.E0ilnLP.803MJqIHjOtTvuhxGv/cAXJ0ccTpYBP1", hash);
        }

        [Fact(DisplayName = "CryptPassword: Create SHA512 Verify")]
        public void Create_Sha512_Verify() {
            Assert.True(CryptPassword.Verify("Test", "$6$SALT$8GXK57PY.bq4j7Ng3f0LF6NcPXxUXqmmseKtw1ugn8uoKXiPJWG8Ub6bxJcHAPBL2y0ppLmQJcpR8mYJbdjVF1"));
            Assert.False(CryptPassword.Verify("Test", "$6$SALT$8GXK57PY.bq4j7Ng3f0LF6NcPXxUXqmmseKtw1ugn8uoKXiPJWG8Ub6bxJcHAPBL2y0ppLmQJcpR8mYJbdjVF0"));
        }

        [Fact(DisplayName = "CryptPassword: Create SHA512 Verify NoSalt")]
        public void Create_Sha512_Verify_NoSalt() {
            Assert.True(CryptPassword.Verify("Test", "$6$$A2vGKWUkCCh28GOsloAzFlH9OgSh8Kv37fsIgM/FmwIPpmZXE/Rx6h6Fdjw7bEasMtpE/e9QQL9Te0d1pUJk./"));
            Assert.False(CryptPassword.Verify("Test", "$6$$A2vGKWUkCCh28GOsloAzFlH9OgSh8Kv37fsIgM/FmwIPpmZXE/Rx6h6Fdjw7bEasMtpE/e9QQL9Te0d1pUJk.0"));
        }

        [Fact(DisplayName = "CryptPassword: Create SHA512 Verify SaltTooLong")]
        public void Create_Sha512_Verify_SaltTooLong() {
            Assert.True(CryptPassword.Verify("Hello world!", "$6$rounds=10000$saltstringsaltst$OW1/O6BYHV6BcXZu8QVeXbDWra3Oeqh0sbHbbMCVNSnCM/UrjmM0Dp8vOuZeHBy/YTBmSK6H9qs/y3RnOaw5v."));
        }

        [Fact(DisplayName = "CryptPassword: Create SHA512 Verify ExtraLongPassword")]
        public void Create_Sha512_Verify_ExtraLongPassword() {
            Assert.True(CryptPassword.Verify("a very much longer text to encrypt.  This one even stretches over morethan one line.", "$6$rounds=1400$anotherlongsalts$POfYwTEok97VWcjxIiSOjiykti.o/pQs.wPvMxQ6Fm7I6IoYN3CmLs66x9t0oSwbtEW7o7UmJEiDwGqd8p4ur1"));
        }

        #endregion

        #region Default

        [Fact(DisplayName = "CryptPassword: Create Default")]
        public void Create_Default() {
            var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha512);
            Assert.StartsWith("$6$", hash, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(106, hash.Length);  // total length
            Assert.Equal(16, hash.Split('$')[2].Length);  // default salt length
            Assert.Equal(86, hash.Split('$')[3].Length);  // hash length
        }

        #endregion

        #region Errors

        [Fact(DisplayName = "CryptPassword: Password(string) ArgumentNullException")]
        public void Validate_PasswordString_ArgumentNullException() {
            Assert.Throws<ArgumentNullException>(() => {
                CryptPassword.Create(default(string));
            });
        }

        [Fact(DisplayName = "CryptPassword: Password(byte[]) ArgumentNullException")]
        public void Validate_PasswordBytes_ArgumentNullException() {
            Assert.Throws<ArgumentNullException>(() => {
                CryptPassword.Create(default(byte[]));
            });
            Assert.Throws<ArgumentNullException>(() => {
                CryptPassword.Create(default(byte[]));
            });
        }

        [Fact(DisplayName = "CryptPassword: Algorithm ArgumentOutOfRangeException")]
        public void Validate_Algorithm_ArgumentOutOfRangeException() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                CryptPassword.Create(Array.Empty<byte>(), 0);
            });
        }

        [Fact(DisplayName = "CryptPassword: Salt ArgumentNullException")]
        public void Validate_Salt_ArgumentNullException() {
            Assert.Throws<ArgumentNullException>(() => {
                CryptPassword.Create(Array.Empty<byte>(), CryptPasswordAlgorithm.Sha256, null);
            });
        }

        [Fact(DisplayName = "CryptPassword: SaltSize ArgumentOutOfRangeException")]
        public void Validate_SaltSize_ArgumentOutOfRangeException() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                CryptPassword.Create(Array.Empty<byte>(), CryptPasswordAlgorithm.Sha512, -2);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                CryptPassword.Create(Array.Empty<byte>(), CryptPasswordAlgorithm.Sha512, 17);
            });
        }

        [Fact(DisplayName = "CryptPassword: IterationCount ArgumentOutOfRangeException")]
        public void Validate_IterationCount_ArgumentOutOfRangeException() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                CryptPassword.Create(Array.Empty<byte>(), CryptPasswordAlgorithm.MD5, CryptPassword.DefaultSaltSize, 999);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                CryptPassword.Create(Array.Empty<byte>(), CryptPasswordAlgorithm.MD5, CryptPassword.DefaultSaltSize, 1000000000);
            });
        }


        [Fact(DisplayName = "CryptPassword: Input UnknownAlgorithm")]
        public void Input_UnknownAlgorithm() {
            Assert.False(CryptPassword.Verify("Test", "$XXX$SALT$8GXK57PY.bq4j7Ng3f0LF6NcPXxUXqmmseKtw1ugn8uoKXiPJWG8Ub6bxJcHAPBL2y0ppLmQJcpR8mYJbdjVF1"));
        }

        [Fact(DisplayName = "CryptPassword: Input NullHash")]
        public void Input_NullHash() {
            Assert.False(CryptPassword.Verify("Test", null));
        }

        [Fact(DisplayName = "CryptPassword: Input EmptyHash")]
        public void Input_EmptyHash() {
            Assert.False(CryptPassword.Verify("Test", ""));
        }

        #endregion

    }
}
