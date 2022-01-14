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

namespace Tests.Medo.Security.Cryptography {
    public class TwofishManagedTests {

        private readonly ITestOutputHelper Output;

        public TwofishManagedTests(ITestOutputHelper output) => Output = output;


        [Fact(DisplayName = "TwoFish: Known Answers (ECB)")]
        public void KnownAnswers_ECB() {
            var tests = GetTestBlocks(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Medo._Resources.Security.Cryptography.Twofish.ECB_TBL.TXT"));
            foreach (var test in tests) {
                using var algorithm = new TwofishManaged() { KeySize = test.KeySize, Mode = CipherMode.ECB, Padding = PaddingMode.None };
                var ct = Encrypt(algorithm, test.Key, null, test.PlainText);
                Assert.Equal(BitConverter.ToString(test.CipherText), BitConverter.ToString(ct));

                var pt = Decrypt(algorithm, test.Key, null, test.CipherText);
                Assert.Equal(BitConverter.ToString(test.PlainText), BitConverter.ToString(pt));
            }
        }

        //[Fact(DisplayName = "TwoFish: Monte Carlo (ECB) Encrypt")]
        private void MonteCarlo_ECB_Encrypt() { //takes ages
            var tests = GetTestBlocks(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Medo._Resources.Security.Cryptography.Twofish.ECB_E_M.TXT"));
            var sw = Stopwatch.StartNew();
            foreach (var test in tests) {
                MonteCarlo_ECB_E(test);
            }
            sw.Stop();
            Output.WriteLine("Duration: " + sw.ElapsedMilliseconds.ToString() + " ms");
        }

        [Fact(DisplayName = "TwoFish: Monte Carlo (ECB) Encrypt single")]
        public void MonteCarlo_ECB_Encrypt_One() {
            var tests = GetTestBlocks(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Medo._Resources.Security.Cryptography.Twofish.ECB_E_M.TXT"));
            var test = tests[Rnd.Next(tests.Count)];
            MonteCarlo_ECB_E(test);
        }


        //[Fact(DisplayName = "TwoFish: Monte Carlo (ECB) Decrypt")]
        private void MonteCarlo_ECB_Decrypt() { //takes ages
            var tests = GetTestBlocks(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Medo._Resources.Security.Cryptography.Twofish.ECB_D_M.TXT"));
            var sw = Stopwatch.StartNew();
            foreach (var test in tests) {
                MonteCarlo_ECB_D(test);
            }
            sw.Stop();
            Output.WriteLine("Duration: " + sw.ElapsedMilliseconds.ToString() + " ms");
        }

        [Fact(DisplayName = "TwoFish: Monte Carlo (ECB) Decrypt single")]
        public void MonteCarlo_ECB_Decrypt_One() {
            var tests = GetTestBlocks(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Medo._Resources.Security.Cryptography.Twofish.ECB_D_M.TXT"));
            var test = tests[Rnd.Next(tests.Count)];
            MonteCarlo_ECB_D(test);
        }


        //[Fact(DisplayName = "TwoFish: Monte Carlo (CBC) Encrypt")]
        private void MonteCarlo_CBC_Encrypt() { //takes ages
            var tests = GetTestBlocks(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Medo._Resources.Security.Cryptography.Twofish.CBC_E_M.TXT"));
            var sw = Stopwatch.StartNew();
            foreach (var test in tests) {
                MonteCarlo_CBC_E(test);
            }
            sw.Stop();
            Output.WriteLine("Duration: " + sw.ElapsedMilliseconds.ToString() + " ms");
        }

        [Fact(DisplayName = "TwoFish: Monte Carlo (CBC) Encrypt single")]
        public void MonteCarlo_CBC_Encrypt_One() {
            var tests = GetTestBlocks(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Medo._Resources.Security.Cryptography.Twofish.CBC_E_M.TXT"));
            var test = tests[Rnd.Next(tests.Count)];
            MonteCarlo_CBC_E(test);
        }


        //[Fact(DisplayName = "TwoFish: Monte Carlo (CBC) Decrypt")]
        private void MonteCarlo_CBC_Decrypt() { //takes ages
            var tests = GetTestBlocks(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Medo._Resources.Security.Cryptography.Twofish.CBC_D_M.TXT"));
            var sw = Stopwatch.StartNew();
            foreach (var test in tests) {
                MonteCarlo_CBC_D(test);
            }
            sw.Stop();
            Output.WriteLine("Duration: " + sw.ElapsedMilliseconds.ToString() + " ms");
        }

        [Fact(DisplayName = "TwoFish: Monte Carlo (CBC) Decrypt single")]
        public void MonteCarlo_CBC_Decrypt_One() {
            var tests = GetTestBlocks(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Medo._Resources.Security.Cryptography.Twofish.CBC_D_M.TXT"));
            var test = tests[Rnd.Next(tests.Count)];
            MonteCarlo_CBC_D(test);
        }


        [Theory(DisplayName = "TwoFish: Padding full blocks")]
        [InlineData(PaddingMode.None)]
        [InlineData(PaddingMode.PKCS7)]
        [InlineData(PaddingMode.Zeros)]
        [InlineData(PaddingMode.ANSIX923)]
        [InlineData(PaddingMode.ISO10126)]
        public void PaddingFull(PaddingMode padding) {
            var key = new byte[32]; RandomNumberGenerator.Fill(key);
            var iv = new byte[16]; RandomNumberGenerator.Fill(iv);
            var data = new byte[48]; RandomNumberGenerator.Fill(data);  // full blocks

            var algorithm = new TwofishManaged() { Padding = padding, };

            var ct = Encrypt(algorithm, key, iv, data);
            var pt = Decrypt(algorithm, key, iv, ct);
            Assert.Equal(data.Length, pt.Length);
            Assert.Equal(BitConverter.ToString(data), BitConverter.ToString(pt));
        }

        [Theory(DisplayName = "TwoFish: Padding partial blocks")]
        [InlineData(PaddingMode.PKCS7)]
        [InlineData(PaddingMode.Zeros)]
        [InlineData(PaddingMode.ANSIX923)]
        [InlineData(PaddingMode.ISO10126)]
        public void PaddingPartial(PaddingMode padding) {
            var key = new byte[32]; RandomNumberGenerator.Fill(key);
            var iv = new byte[16]; RandomNumberGenerator.Fill(iv);
            var data = new byte[42]; RandomNumberGenerator.Fill(data);

            var algorithm = new TwofishManaged() { Padding = padding };

            var ct = Encrypt(algorithm, key, iv, data);
            var pt = Decrypt(algorithm, key, iv, ct);
            Assert.Equal(data.Length, pt.Length);
            Assert.Equal(BitConverter.ToString(data), BitConverter.ToString(pt));
        }

        [Theory(DisplayName = "TwoFish: Only CBC supported")]
        [InlineData(CipherMode.CFB)]
        [InlineData(CipherMode.CTS)]
        public void OnlyCbcAndEbcSupported(CipherMode mode) {
            Assert.Throws<CryptographicException>(() => {
                var _ = new TwofishManaged() { Mode = mode };
            });
        }

        [Theory(DisplayName = "TwoFish: Large Final Block")]
        [InlineData(PaddingMode.None)]
        [InlineData(PaddingMode.PKCS7)]
        [InlineData(PaddingMode.Zeros)]
        [InlineData(PaddingMode.ANSIX923)]
        [InlineData(PaddingMode.ISO10126)]
        public void LargeFinalBlock(PaddingMode padding) {
            var crypto = new TwofishManaged() { Padding = padding };
            crypto.GenerateKey();
            crypto.GenerateIV();
            var text = "This is a final block wider than block size.";  // more than 128 bits of data
            if (padding is PaddingMode.None) { text += "1234"; }  // must have a full block if no padding
            var bytes = Encoding.ASCII.GetBytes(text);

            using var encryptor = crypto.CreateEncryptor();
            var ct = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

            Assert.Equal(padding == PaddingMode.None ? bytes.Length : 48, ct.Length);

            using var decryptor = crypto.CreateDecryptor();
            var pt = decryptor.TransformFinalBlock(ct, 0, ct.Length);

            Assert.Equal(bytes.Length, pt.Length);
            Assert.Equal(text, Encoding.ASCII.GetString(pt));
        }

        [Theory(DisplayName = "TwoFish: BlockSizeRounding")]
        [InlineData(PaddingMode.None)]
        [InlineData(PaddingMode.PKCS7)]
        [InlineData(PaddingMode.Zeros)]
        [InlineData(PaddingMode.ANSIX923)]
        [InlineData(PaddingMode.ISO10126)]
        public void BlockSizeRounding(PaddingMode padding) {
            var key = new byte[32]; RandomNumberGenerator.Fill(key);
            var iv = new byte[16]; RandomNumberGenerator.Fill(iv);

            for (int n = 0; n < 50; n++) {
                if ((n % 16 != 0) && (padding is PaddingMode.None)) { continue; }  // padding None works only on full blocks

                var data = new byte[n];
                RandomNumberGenerator.Fill(data);
                if ((padding == PaddingMode.Zeros) && (data.Length > 0)) { data[^1] = 1; }  // zero padding needs to have the last number non-zero

                var algorithm = new TwofishManaged() { Padding = padding, };

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

        [Theory(DisplayName = "TwoFish: Random Testing")]
        [InlineData(PaddingMode.None)]
        [InlineData(PaddingMode.PKCS7)]
        [InlineData(PaddingMode.Zeros)]
        [InlineData(PaddingMode.ANSIX923)]
        [InlineData(PaddingMode.ISO10126)]
        public void Randomised(PaddingMode padding) {
            for (var n = 0; n < 1000; n++) {
                var crypto = new TwofishManaged() { Padding = padding };
                crypto.GenerateKey();
                crypto.GenerateIV();
                var data = new byte[Random.Shared.Next(100)];
                if (padding is PaddingMode.None) { data = new byte[data.Length / 16 * 16]; }  // make it rounded number if no padding
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

        [Theory(DisplayName = "TwoFish: Encrypt/Decrypt")]
        [InlineData(PaddingMode.None)]
        [InlineData(PaddingMode.PKCS7)]
        [InlineData(PaddingMode.Zeros)]
        [InlineData(PaddingMode.ANSIX923)]
        [InlineData(PaddingMode.ISO10126)]
        public void EncryptDecrypt(PaddingMode padding) {
            var crypto = new TwofishManaged() { Padding = padding };
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

            if (padding is PaddingMode.None) {  // has to be a full block if no padding
                var lastBytesEnc = encryptor.TransformFinalBlock(new byte[16], 0, 16);
                var lastBytesDec = decryptor.TransformFinalBlock(lastBytesEnc, 0, lastBytesEnc.Length);
            } else {
                var lastBytesEnc = encryptor.TransformFinalBlock(new byte[10], 0, 10);
                var lastBytesDec = decryptor.TransformFinalBlock(lastBytesEnc, 0, lastBytesEnc.Length);
            }
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

        #endregion

        #region Private: Monte carlo
        // http://www.ntua.gr/cryptix/old/cryptix/aes/docs/katmct.html

        private static void MonteCarlo_ECB_E(TestBlock test) {
            using var algorithm = new TwofishManaged() { KeySize = test.KeySize, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            var key = test.Key;
            var pt = test.PlainText;
            byte[] ct = null;
            for (var j = 0; j < 10000; j++) {
                ct = Encrypt(algorithm, key, null, pt);
                pt = ct;
            }
            Assert.Equal(BitConverter.ToString(test.CipherText), BitConverter.ToString(ct));
        }

        private static void MonteCarlo_ECB_D(TestBlock test) {
            using var algorithm = new TwofishManaged() { KeySize = test.KeySize, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            var key = test.Key;
            var ct = test.CipherText;
            byte[] pt = null;
            for (var j = 0; j < 10000; j++) {
                pt = Decrypt(algorithm, key, null, ct);
                ct = pt;
            }
            Assert.Equal(BitConverter.ToString(test.PlainText), BitConverter.ToString(pt));
        }


        private static void MonteCarlo_CBC_E(TestBlock test) {
            using var algorithm = new TwofishManaged() { KeySize = test.KeySize, Mode = CipherMode.CBC, Padding = PaddingMode.None };
            var key = test.Key;
            var cv = test.IV;
            var pt = test.PlainText;
            byte[] ct = null;
            for (var j = 0; j < 10000; j++) {
                var ob = Encrypt(algorithm, key, cv, pt);
                pt = (j == 0) ? cv : ct;
                ct = ob;
                cv = ct;
            }
            Assert.Equal(BitConverter.ToString(test.CipherText), BitConverter.ToString(ct));
        }

        private static void MonteCarlo_CBC_D(TestBlock test) {
            using var algorithm = new TwofishManaged() { KeySize = test.KeySize, Mode = CipherMode.CBC, Padding = PaddingMode.None };
            var key = test.Key;
            var cv = test.IV;
            var ct = test.CipherText;
            byte[] pt = null;
            for (var j = 0; j < 10000; j++) {
                pt = Decrypt(algorithm, key, cv, ct);
                cv = ct;
                ct = pt;
            }
            Assert.Equal(BitConverter.ToString(test.PlainText), BitConverter.ToString(pt));
        }

        #endregion


        #region Multiblock

        [Fact(DisplayName = "TwoFish: Multiblock (ECB/128) Encrypt")]
        public void MultiBlock_ECB_128_Encrypt() {
            var key = ParseBytes("00000000000000000000000000000000");
            var pt = ParseBytes("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            var ct = Encrypt(algorithm, key, null, pt);
            Assert.Equal("9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Multiblock (ECB/128) Decrypt")]
        public void MultiBlock_ECB_128_Decrypt() {
            var key = ParseBytes("00000000000000000000000000000000");
            var ct = ParseBytes("9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            var pt = Decrypt(algorithm, key, null, ct);
            Assert.Equal("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", BitConverter.ToString(pt).Replace("-", ""));
        }


        [Fact(DisplayName = "TwoFish: Multiblock (CBC/128) Encrypt")]
        public void MultiBlock_CBC_128_Encrypt() {
            var key = ParseBytes("00000000000000000000000000000000");
            var iv = ParseBytes("00000000000000000000000000000000");
            var pt = ParseBytes("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.CBC, Padding = PaddingMode.None };
            var ct = Encrypt(algorithm, key, iv, pt);
            Assert.Equal("9F589F5CF6122C32B6BFEC2F2AE8C35AD491DB16E7B1C39E86CB086B789F541905EF8C61A811582634BA5CB7106AA641", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Multiblock (CBC/128) Decrypt")]
        public void MultiBlock_CBC_128_Decrypt() {
            var key = ParseBytes("00000000000000000000000000000000");
            var iv = ParseBytes("00000000000000000000000000000000");
            var ct = ParseBytes("9F589F5CF6122C32B6BFEC2F2AE8C35AD491DB16E7B1C39E86CB086B789F541905EF8C61A811582634BA5CB7106AA641");
            var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.CBC, Padding = PaddingMode.None };
            var pt = Decrypt(algorithm, key, iv, ct);
            Assert.Equal("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", BitConverter.ToString(pt).Replace("-", ""));
        }


        [Fact(DisplayName = "TwoFish: Multiblock (ECB/192) Encrypt")]
        public void MultiBlock_ECB_192_Encrypt() {
            var key = ParseBytes("000000000000000000000000000000000000000000000000");
            var pt = ParseBytes("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            var algorithm = new TwofishManaged() { KeySize = 192, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            var ct = Encrypt(algorithm, key, null, pt);
            Assert.Equal("EFA71F788965BD4453F860178FC19101EFA71F788965BD4453F860178FC19101EFA71F788965BD4453F860178FC19101", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Multiblock (ECB/192) Decrypt")]
        public void MultiBlock_ECB_192_Decrypt() {
            var key = ParseBytes("000000000000000000000000000000000000000000000000");
            var ct = ParseBytes("EFA71F788965BD4453F860178FC19101EFA71F788965BD4453F860178FC19101EFA71F788965BD4453F860178FC19101");
            var algorithm = new TwofishManaged() { KeySize = 192, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            var pt = Decrypt(algorithm, key, null, ct);
            Assert.Equal("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", BitConverter.ToString(pt).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Multiblock (CBC/192) Encrypt")]
        public void MultiBlock_CBC_192_Encrypt() {
            var key = ParseBytes("000000000000000000000000000000000000000000000000");
            var iv = ParseBytes("00000000000000000000000000000000");
            var pt = ParseBytes("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            var algorithm = new TwofishManaged() { KeySize = 192, Mode = CipherMode.CBC, Padding = PaddingMode.None };
            var ct = Encrypt(algorithm, key, iv, pt);
            Assert.Equal("EFA71F788965BD4453F860178FC1910188B2B2706B105E36B446BB6D731A1E88F2DD994D2C4E64517CC9DB9AED2D5909", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Multiblock (CBC/192) Decrypt")]
        public void MultiBlock_CBC_192_Decrypt() {
            var key = ParseBytes("000000000000000000000000000000000000000000000000");
            var iv = ParseBytes("00000000000000000000000000000000");
            var ct = ParseBytes("EFA71F788965BD4453F860178FC1910188B2B2706B105E36B446BB6D731A1E88F2DD994D2C4E64517CC9DB9AED2D5909");
            var algorithm = new TwofishManaged() { KeySize = 192, Mode = CipherMode.CBC, Padding = PaddingMode.None };
            var pt = Decrypt(algorithm, key, iv, ct);
            Assert.Equal("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", BitConverter.ToString(pt).Replace("-", ""));
        }


        [Fact(DisplayName = "TwoFish: Multiblock (ECB/256) Encrypt")]
        public void MultiBlock_ECB_256_Encrypt() {
            var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
            var pt = ParseBytes("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            var algorithm = new TwofishManaged() { KeySize = 256, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            var ct = Encrypt(algorithm, key, null, pt);
            Assert.Equal("57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Multiblock (ECB/256) Decrypt")]
        public void MultiBlock_ECB_256_Decrypt() {
            var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
            var ct = ParseBytes("57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F");
            var algorithm = new TwofishManaged() { KeySize = 256, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            var pt = Decrypt(algorithm, key, null, ct);
            Assert.Equal("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", BitConverter.ToString(pt).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Multiblock (CBC/256) Encrypt")]
        public void MultiBlock_CBC_256_Encrypt() {
            var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
            var iv = ParseBytes("00000000000000000000000000000000");
            var pt = ParseBytes("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            var algorithm = new TwofishManaged() { KeySize = 256, Mode = CipherMode.CBC, Padding = PaddingMode.None };
            var ct = Encrypt(algorithm, key, iv, pt);
            Assert.Equal("57FF739D4DC92C1BD7FC01700CC8216FD43BB7556EA32E46F2A282B7D45B4E0D2804E32925D62BAE74487A06B3CD2D46", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Multiblock (CBC/256) Decrypt")]
        public void MultiBlock_CBC_256_Decrypt() {
            var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
            var iv = ParseBytes("00000000000000000000000000000000");
            var ct = ParseBytes("57FF739D4DC92C1BD7FC01700CC8216FD43BB7556EA32E46F2A282B7D45B4E0D2804E32925D62BAE74487A06B3CD2D46");
            var algorithm = new TwofishManaged() { KeySize = 256, Mode = CipherMode.CBC, Padding = PaddingMode.None };
            var pt = Decrypt(algorithm, key, iv, ct);
            Assert.Equal("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", BitConverter.ToString(pt).Replace("-", ""));
        }


        [Fact(DisplayName = "TwoFish: Multiblock 2 (ECB/256) Encrypt")]
        public void MultiBlockNonFinal_ECB_256_Encrypt() {
            var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
            var pt = ParseBytes("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            using var algorithm = new TwofishManaged() { KeySize = 256, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            algorithm.Key = key;
            var ct = new byte[pt.Length];
            using (var transform = algorithm.CreateEncryptor()) {
                transform.TransformBlock(pt, 0, pt.Length, ct, 0);
                transform.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            }
            Assert.Equal("57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Multiblock 2 (ECB/256) Decrypt")]
        public void MultiBlockNotFinal_ECB_256_Decrypt() {
            var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
            var ct = ParseBytes("57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F");
            using var algorithm = new TwofishManaged() { KeySize = 256, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            algorithm.Key = key;
            var pt = new byte[ct.Length];
            using (var transform = algorithm.CreateDecryptor()) {
                transform.TransformBlock(ct, 0, ct.Length, pt, 0);
                transform.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            }
            Assert.Equal("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", BitConverter.ToString(pt).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Multiblock 3 (ECB/256) Encrypt")]
        public void MultiBlockFinal_ECB_256_Encrypt() {
            var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
            var pt = ParseBytes("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            using var algorithm = new TwofishManaged() { KeySize = 256, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            algorithm.Key = key;
            var ct = algorithm.CreateEncryptor().TransformFinalBlock(pt, 0, pt.Length);
            Assert.Equal("57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Multiblock 3 (ECB/256) Decrypt")]
        public void MultiBlockFinal_ECB_256_Decrypt() {
            var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
            var ct = ParseBytes("57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F");
            using var algorithm = new TwofishManaged() { KeySize = 256, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            algorithm.Key = key;
            var pt = algorithm.CreateDecryptor().TransformFinalBlock(ct, 0, ct.Length);
            Assert.Equal("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", BitConverter.ToString(pt).Replace("-", ""));
        }


        [Fact(DisplayName = "TwoFish: Multiblock 2 (CBC/256) Encrypt")]
        public void MultiBlockNonFinal_CBC_256_Encrypt() {
            var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
            var iv = ParseBytes("00000000000000000000000000000000");
            var pt = ParseBytes("9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A");
            using var algorithm = new TwofishManaged() { KeySize = 256, Mode = CipherMode.CBC, Padding = PaddingMode.None };
            algorithm.Key = key;
            algorithm.IV = iv;
            var ct = new byte[pt.Length];
            using (var transform = algorithm.CreateEncryptor()) {
                transform.TransformBlock(pt, 0, pt.Length, ct, 0);
                transform.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            }
            Assert.Equal("61B5BC459C4E9491DD9E6ACB7478813047BE7250D34F792C17F0C23583C0B040B95C9FAE11107EE9BAC3D79BBFE019EE", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Multiblock 2 (CBC/256) Decrypt")]
        public void MultiBlockNonFinal_CBC_256_Decrypt() {
            var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
            var iv = ParseBytes("00000000000000000000000000000000");
            var ct = ParseBytes("61B5BC459C4E9491DD9E6ACB7478813047BE7250D34F792C17F0C23583C0B040B95C9FAE11107EE9BAC3D79BBFE019EE");
            using var algorithm = new TwofishManaged() { KeySize = 256, Mode = CipherMode.CBC, Padding = PaddingMode.None };
            algorithm.Key = key;
            algorithm.IV = iv;
            var pt = new byte[ct.Length]; pt[ct.Length - 1] = 0xFF;
            using (var transform = algorithm.CreateDecryptor()) {
                transform.TransformBlock(ct, 0, ct.Length, pt, 0);
                transform.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            }
            Assert.Equal("9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A", BitConverter.ToString(pt).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Multiblock 3 (CBC/256) Encrypt")]
        public void MultiBlockFinal_CBC_256_Encrypt() {
            var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
            var iv = ParseBytes("00000000000000000000000000000000");
            var pt = ParseBytes("9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A");
            using var algorithm = new TwofishManaged() { KeySize = 256, Mode = CipherMode.CBC, Padding = PaddingMode.None };
            algorithm.Key = key;
            algorithm.IV = iv;
            var ct = algorithm.CreateEncryptor().TransformFinalBlock(pt, 0, pt.Length);
            Assert.Equal("61B5BC459C4E9491DD9E6ACB7478813047BE7250D34F792C17F0C23583C0B040B95C9FAE11107EE9BAC3D79BBFE019EE", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Multiblock 3 (CBC/256) Decrypt")]
        public void MultiBlockFinal_CBC_256_Decrypt() {
            var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
            var iv = ParseBytes("00000000000000000000000000000000");
            var ct = ParseBytes("61B5BC459C4E9491DD9E6ACB7478813047BE7250D34F792C17F0C23583C0B040B95C9FAE11107EE9BAC3D79BBFE019EE");
            using var algorithm = new TwofishManaged() { KeySize = 256, Mode = CipherMode.CBC, Padding = PaddingMode.None };
            algorithm.Key = key;
            algorithm.IV = iv;
            var pt = algorithm.CreateDecryptor().TransformFinalBlock(ct, 0, ct.Length);
            Assert.Equal("9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A", BitConverter.ToString(pt).Replace("-", ""));
        }

        #endregion


        #region Padding

        [Fact(DisplayName = "TwoFish: Padding Zeros (ECB/128) Encrypt")]
        public void Padding_Zeros_ECB_128_Encrypt() {
            var key = ParseBytes("00000000000000000000000000000000");
            var pt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.Zeros };
            var ct = Encrypt(algorithm, key, null, pt);
            Assert.Equal("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB33C25C273BF09B94A31DE3C27C28DFB5C", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Padding Zeros (ECB/128) Decrypt")]
        public void Padding_Zeros_ECB_128_Decrypt() {
            var key = ParseBytes("00000000000000000000000000000000");
            var ct = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB33C25C273BF09B94A31DE3C27C28DFB5C");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.Zeros };
            var pt = Decrypt(algorithm, key, null, ct);
            Assert.Equal("The quick brown fox jumps over the lazy dog", Encoding.UTF8.GetString(pt));
        }

        [Fact(DisplayName = "TwoFish: Padding None at boundary (ECB/128) Encrypt")]
        public void Padding_None_ECB_128_Encrypt_16() {
            var key = ParseBytes("00000000000000000000000000000000");
            var pt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog once");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            var ct = Encrypt(algorithm, key, null, pt);
            Assert.Equal("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB59", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Padding None at boundary (ECB/128) Decrypt")]
        public void Padding_None_ECB_128_Decrypt_16() {
            var key = ParseBytes("00000000000000000000000000000000");
            var ct = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB59");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            var pt = Decrypt(algorithm, key, null, ct);
            Assert.Equal("The quick brown fox jumps over the lazy dog once", Encoding.UTF8.GetString(pt));
        }


        [Fact(DisplayName = "TwoFish: Padding Zeros at boundary (ECB/128) Encrypt")]
        public void Padding_Zeros_ECB_128_Encrypt_16() {
            var key = ParseBytes("00000000000000000000000000000000");
            var pt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog once");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.Zeros };
            var ct = Encrypt(algorithm, key, null, pt);
            Assert.Equal("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB59", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Padding Zeros at boundary (ECB/128) Decrypt")]
        public void Padding_Zeros_ECB_128_Decrypt_16() {
            var key = ParseBytes("00000000000000000000000000000000");
            var ct = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB59");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.Zeros };
            var pt = Decrypt(algorithm, key, null, ct);
            Assert.Equal("The quick brown fox jumps over the lazy dog once", Encoding.UTF8.GetString(pt));
        }


        [Fact(DisplayName = "TwoFish: Padding PKCS#7 (ECB/128) Encrypt")]
        public void Padding_Pkcs7_ECB_128_Encrypt() {
            var key = ParseBytes("00000000000000000000000000000000");
            var pt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
            var ct = Encrypt(algorithm, key, null, pt);
            Assert.Equal("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB3235D2E6063F32DE35B8A62A384FC587E", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Padding PKCS#7 (ECB/128) Decrypt")]
        public void Padding_Pkcs7_ECB_128_Decrypt() {
            var key = ParseBytes("00000000000000000000000000000000");
            var ct = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB3235D2E6063F32DE35B8A62A384FC587E");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
            var pt = Decrypt(algorithm, key, null, ct);
            Assert.Equal("The quick brown fox jumps over the lazy dog", Encoding.UTF8.GetString(pt));
        }

        [Fact(DisplayName = "TwoFish: Padding PKCS#7 at boundary (ECB/128) Encrypt")]
        public void Padding_Pkcs7_ECB_128_Encrypt_16() {
            var key = ParseBytes("00000000000000000000000000000000");
            var pt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog once");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
            var ct = Encrypt(algorithm, key, null, pt);
            Assert.Equal("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB59771D591428AF301D69FA1E227D083527", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Padding PKCS#7 at boundary (ECB/128) Decrypt")]
        public void Padding_Pkcs7_ECB_128_Decrypt_16() {
            var key = ParseBytes("00000000000000000000000000000000");
            var ct = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB59771D591428AF301D69FA1E227D083527");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
            var pt = Decrypt(algorithm, key, null, ct);
            Assert.Equal("The quick brown fox jumps over the lazy dog once", Encoding.UTF8.GetString(pt));
        }


        [Fact(DisplayName = "TwoFish: Padding ANSI X923 (ECB/128) Encrypt")]
        public void Padding_AnsiX923_ECB_128_Encrypt() {
            var key = ParseBytes("00000000000000000000000000000000");
            var pt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ANSIX923 };
            var ct = Encrypt(algorithm, key, null, pt);
            Assert.Equal("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB3B696D40A5E12225D3E05E8A466F078C2", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Padding ANSI X923 (ECB/128) Decrypt")]
        public void Padding_AnsiX923_ECB_128_Decrypt() {
            var key = ParseBytes("00000000000000000000000000000000");
            var ct = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB3B696D40A5E12225D3E05E8A466F078C2");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ANSIX923 };
            var pt = Decrypt(algorithm, key, null, ct);
            Assert.Equal("The quick brown fox jumps over the lazy dog", Encoding.UTF8.GetString(pt));
        }

        [Fact(DisplayName = "TwoFish: Padding ANSI X923 at boundary (ECB/128) Encrypt")]
        public void Padding_AnsiX923_ECB_128_Encrypt_16() {
            var key = ParseBytes("00000000000000000000000000000000");
            var pt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog once");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ANSIX923 };
            var ct = Encrypt(algorithm, key, null, pt);
            Assert.Equal("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB5958A06DC5AD2D7C0550771D6E9D59D58B", BitConverter.ToString(ct).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Padding ANSI X923 at boundary (ECB/128) Decrypt")]
        public void Padding_AnsiX923_ECB_128_Decrypt_16() {
            var key = ParseBytes("00000000000000000000000000000000");
            var ct = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB5958A06DC5AD2D7C0550771D6E9D59D58B");
            using var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ANSIX923 };
            var pt = Decrypt(algorithm, key, null, ct);
            Assert.Equal("The quick brown fox jumps over the lazy dog once", Encoding.UTF8.GetString(pt));
        }


        [Fact(DisplayName = "TwoFish: Padding ISO 10126 (ECB/128) Encrypt/Decrypt")]
        public void Padding_Iso10126_ECB_128_DecryptAndEncrypt() {
            var key = ParseBytes("00000000000000000000000000000000");
            var pt = "The quick brown fox jumps over the lazy dog";

            var ctA = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB3B696D40A5E12225D3E05E8A466F078C2");
            using (var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ISO10126 }) {
                var ptA = Decrypt(algorithm, key, null, ctA);
                Assert.Equal(pt, Encoding.UTF8.GetString(ptA));
            }

            var ptB = Encoding.UTF8.GetBytes(pt);
            using (var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ISO10126 }) {
                var ctB = Encrypt(algorithm, key, null, ptB);
                var ptC = Decrypt(algorithm, key, null, ctB);
                Assert.Equal(pt, Encoding.UTF8.GetString(ptC));
                Assert.NotEqual(BitConverter.ToString(ctA).Replace("-", ""), BitConverter.ToString(ctB).Replace("-", "")); //chances are good padding will be different (due to randomness involved)
            }
        }

        [Fact(DisplayName = "TwoFish: Padding ISO 10126 at boundary (ECB/128) Encrypt/Decrypt")]
        public void Padding_Iso10126_ECB_128_DecryptAndEncrypt_16() {
            var key = ParseBytes("00000000000000000000000000000000");
            var pt = "The quick brown fox jumps over the lazy dog once";

            var ctA = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB5958A06DC5AD2D7C0550771D6E9D59D58B");
            using (var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ISO10126 }) {
                var ptA = Decrypt(algorithm, key, null, ctA);
                Assert.Equal(pt, Encoding.UTF8.GetString(ptA));
            }

            var ptB = Encoding.UTF8.GetBytes(pt);
            using (var algorithm = new TwofishManaged() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ISO10126 }) {
                var ctB = Encrypt(algorithm, key, null, ptB);
                var ptC = Decrypt(algorithm, key, null, ctB);
                Assert.Equal(pt, Encoding.UTF8.GetString(ptC));
                Assert.NotEqual(BitConverter.ToString(ctA).Replace("-", ""), BitConverter.ToString(ctB).Replace("-", "")); //chances are good padding will be different (due to randomness involved)
            }
        }

        #endregion


        #region Other

        [Fact(DisplayName = "TwoFish: Transform block (same array) Encrypt")]
        public void TransformBlock_Encrypt_UseSameArray() {
            var key = ParseBytes("00000000000000000000000000000000");
            var iv = ParseBytes("00000000000000000000000000000000");
            var ctpt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog once");
            using (var twofish = new TwofishManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None, KeySize = 128, Key = key, IV = iv }) {
                using var transform = twofish.CreateEncryptor();
                transform.TransformBlock(ctpt, 0, 48, ctpt, 0);
            }
            Assert.Equal("B0DD30E9AB1F1329C1BEE154DDBE88AF8C47A4FE24D56DC027ED503652C9D164CE26E0C6E32BCA8756482B99988E8C79", BitConverter.ToString(ctpt).Replace("-", ""));
        }

        [Fact(DisplayName = "TwoFish: Transform block (same array) Decrypt")]
        public void TransformBlock_Decrypt_UseSameArray() {
            var key = ParseBytes("00000000000000000000000000000000");
            var iv = ParseBytes("00000000000000000000000000000000");
            var ctpt = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF8C47A4FE24D56DC027ED503652C9D164CE26E0C6E32BCA8756482B99988E8C79");
            using (var twofish = new TwofishManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None, KeySize = 128, Key = key, IV = iv }) {
                using var transform = twofish.CreateDecryptor();
                transform.TransformBlock(ctpt, 0, 48, ctpt, 0); //no caching last block if Padding is none
            }
            Assert.Equal("The quick brown fox jumps over the lazy dog once", Encoding.UTF8.GetString(ctpt));
        }

        #endregion


        #region Private setup

        private static readonly Random Rnd = new();

        private static List<TestBlock> GetTestBlocks(Stream fileStream) {
            var result = new List<TestBlock>();

            using (var s = new StreamReader(fileStream)) {
                int? keySize = null, i = null;
                byte[] key = null, iv = null, ct = null, pt = null;

                while (!s.EndOfStream) {
                    var line = s.ReadLine();
                    if (line.StartsWith("KEYSIZE=", StringComparison.Ordinal)) {
                        keySize = int.Parse(line[8..], CultureInfo.InvariantCulture);
                        i = null;
                    } else if (line.StartsWith("I=", StringComparison.Ordinal)) {
                        if (keySize == null) { continue; }
                        i = int.Parse(line[2..], CultureInfo.InvariantCulture);
                    } else if (line.StartsWith("KEY=", StringComparison.Ordinal)) {
                        key = ParseBytes(line[4..]);
                    } else if (line.StartsWith("IV=", StringComparison.Ordinal)) {
                        iv = ParseBytes(line[3..]);
                    } else if (line.StartsWith("PT=", StringComparison.Ordinal)) {
                        pt = ParseBytes(line[3..]);
                    } else if (line.StartsWith("CT=", StringComparison.Ordinal)) {
                        ct = ParseBytes(line[3..]);
                    } else if (line.Equals("", StringComparison.Ordinal)) {
                        if (i == null) { continue; }
                        result.Add(new TestBlock(keySize.Value, i.Value, key, iv, pt, ct));
                        i = null; key = null; iv = null; ct = null; pt = null;
                    }
                }
            }

            return result;
        }

        private static byte[] ParseBytes(string hex) {
            Trace.Assert((hex.Length % 2) == 0);
            var result = new byte[hex.Length / 2];
            for (var i = 0; i < hex.Length; i += 2) {
                result[i / 2] = byte.Parse(hex.AsSpan(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return result;
        }

        [DebuggerDisplay("{KeySize}:{Index}")]
        private struct TestBlock {
            internal TestBlock(int keySize, int index, byte[] key, byte[] iv, byte[] plainText, byte[] cipherText) {
                KeySize = keySize;
                Index = index;
                Key = key;
                IV = iv;
                PlainText = plainText;
                CipherText = cipherText;
            }
            internal int KeySize { get; }
            internal int Index { get; }
            internal byte[] Key { get; }
            internal byte[] IV { get; }
            internal byte[] PlainText { get; }
            internal byte[] CipherText { get; }
        }

        #endregion

    }
}
