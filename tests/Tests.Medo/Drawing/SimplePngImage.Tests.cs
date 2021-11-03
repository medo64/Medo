using System;
using System.Drawing;
using System.IO;
using Xunit;
using Medo.Drawing;
using System.Reflection;

namespace Tests.Medo.Drawing {
    public class SimplePngImageTests {

        [Fact(DisplayName = "SimplePngImage: Basic")]
        public void Basic() {
            var bmp = new SimplePngImage(2, 3);
            Assert.Equal(2, bmp.Width);
            Assert.Equal(3, bmp.Height);

            bmp.SetPixel(0, 0, Color.Red);
            bmp.SetPixel(0, 1, Color.Green);
            bmp.SetPixel(0, 2, Color.Blue);
            bmp.SetPixel(1, 0, Color.White);
            bmp.SetPixel(1, 1, Color.Black);
            bmp.SetPixel(1, 2, Color.Purple);
            Assert.Equal(Color.Red, bmp.GetPixel(0, 0));
            Assert.Equal(Color.Green, bmp.GetPixel(0, 1));
            Assert.Equal(Color.Blue, bmp.GetPixel(0, 2));
            Assert.Equal(Color.White, bmp.GetPixel(1, 0));
            Assert.Equal(Color.Black, bmp.GetPixel(1, 1));
            Assert.Equal(Color.Purple, bmp.GetPixel(1, 2));

            //bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-basic.png"));
            var memStream = new MemoryStream();
            bmp.Save(memStream);
            Assert.Equal("89504E470D0A1A0A0000000D4948445200000002000000030802000000368849D60000001349444154789C63F8CFC0F0FF3F9068608080FF0D0C0D00D6C212940000000049454E44AE426082",
                         BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
        }


        [Fact(DisplayName = "SimplePngImage: Basic Transparency")]
        public void Transparency() {
            var bmp = new SimplePngImage(2, 3);
            Assert.Equal(2, bmp.Width);
            Assert.Equal(3, bmp.Height);

            bmp.SetPixel(0, 0, Color.Red);
            bmp.SetPixel(0, 1, Color.Green);
            bmp.SetPixel(0, 2, Color.Blue);
            bmp.SetPixel(1, 0, Color.White);
            bmp.SetPixel(1, 1, Color.Black);
            bmp.SetPixel(1, 2, Color.FromArgb(128, Color.Purple));
            Assert.Equal(Color.Red, bmp.GetPixel(0, 0));
            Assert.Equal(Color.Green, bmp.GetPixel(0, 1));
            Assert.Equal(Color.Blue, bmp.GetPixel(0, 2));
            Assert.Equal(Color.White, bmp.GetPixel(1, 0));
            Assert.Equal(Color.Black, bmp.GetPixel(1, 1));
            Assert.Equal(Color.FromArgb(128, Color.Purple), bmp.GetPixel(1, 2));

            //bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-transparency.png"));
            var memStream = new MemoryStream();
            bmp.Save(memStream);
            Assert.Equal("89504E470D0A1A0A0000000D4948445200000002000000030806000000B9EADE810000001749444154789C63F8CFC0F01F0418181A18800404FF6F6068680000700275140000000049454E44AE426082",
                         BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
        }

        [Fact(DisplayName = "SimplePngImage: Clone")]
        public void Clone() {
            var original = new SimplePngImage(2, 3);

            original.SetPixel(0, 0, Color.Red);
            original.SetPixel(0, 1, Color.Green);
            original.SetPixel(0, 2, Color.Blue);
            original.SetPixel(1, 0, Color.White);
            original.SetPixel(1, 1, Color.Black);
            original.SetPixel(1, 2, Color.Purple);

            var bmp = original.Clone();
            original.SetPixel(1, 2, Color.Cyan);

            Assert.Equal(Color.Red, bmp.GetPixel(0, 0));
            Assert.Equal(Color.Green, bmp.GetPixel(0, 1));
            Assert.Equal(Color.Blue, bmp.GetPixel(0, 2));
            Assert.Equal(Color.White, bmp.GetPixel(1, 0));
            Assert.Equal(Color.Black, bmp.GetPixel(1, 1));
            Assert.Equal(Color.Purple, bmp.GetPixel(1, 2));

            //bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-clone.png"));
            var memStream = new MemoryStream();
            bmp.Save(memStream);
            Assert.Equal("89504E470D0A1A0A0000000D4948445200000002000000030802000000368849D60000001349444154789C63F8CFC0F0FF3F9068608080FF0D0C0D00D6C212940000000049454E44AE426082",
                         BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
        }

        [Fact(DisplayName = "SimplePngImage: Save/Open 24-bit")]
        public void SaveOpen24() {
            var bmp = new SimplePngImage(2, 3);
            bmp.SetPixel(0, 0, Color.Red);
            bmp.SetPixel(0, 1, Color.Green);
            bmp.SetPixel(0, 2, Color.Blue);
            bmp.SetPixel(1, 0, Color.White);
            bmp.SetPixel(1, 1, Color.Black);
            bmp.SetPixel(1, 2, Color.Purple);

            var memStream = new MemoryStream();
            bmp.Save(memStream);
            memStream.Position = 0;

            var bmp2 = new SimplePngImage(memStream);
            Assert.Equal(Color.Red.ToArgb(), bmp2.GetPixel(0, 0).ToArgb());
            Assert.Equal(Color.Green.ToArgb(), bmp2.GetPixel(0, 1).ToArgb());
            Assert.Equal(Color.Blue.ToArgb(), bmp2.GetPixel(0, 2).ToArgb());
            Assert.Equal(Color.White.ToArgb(), bmp2.GetPixel(1, 0).ToArgb());
            Assert.Equal(Color.Black.ToArgb(), bmp2.GetPixel(1, 1).ToArgb());
            Assert.Equal(Color.Purple.ToArgb(), bmp2.GetPixel(1, 2).ToArgb());
        }

        [Fact(DisplayName = "SimplePngImage: Save/Open 32-bit")]
        public void SaveOpen32() {
            var bmp = new SimplePngImage(2, 3);
            bmp.SetPixel(0, 0, Color.Red);
            bmp.SetPixel(0, 1, Color.Green);
            bmp.SetPixel(0, 2, Color.Blue);
            bmp.SetPixel(1, 0, Color.White);
            bmp.SetPixel(1, 1, Color.Black);
            bmp.SetPixel(1, 2, Color.FromArgb(128, Color.Purple));

            var memStream = new MemoryStream();
            bmp.Save(memStream);
            memStream.Position = 0;

            var bmp2 = new SimplePngImage(memStream);
            Assert.Equal(Color.Red.ToArgb(), bmp2.GetPixel(0, 0).ToArgb());
            Assert.Equal(Color.Green.ToArgb(), bmp2.GetPixel(0, 1).ToArgb());
            Assert.Equal(Color.Blue.ToArgb(), bmp2.GetPixel(0, 2).ToArgb());
            Assert.Equal(Color.White.ToArgb(), bmp2.GetPixel(1, 0).ToArgb());
            Assert.Equal(Color.Black.ToArgb(), bmp2.GetPixel(1, 1).ToArgb());
            Assert.Equal(Color.FromArgb(128, Color.Purple).ToArgb(), bmp2.GetPixel(1, 2).ToArgb());
        }


        [Theory(DisplayName = "SimplePngImage: Validate Colors 2")]
        [InlineData("Example2-Color32.png")]
        [InlineData("Example2-Color24.png")]
        [InlineData("Example2-Color8.png")]
        [InlineData("Example2-Color4.png")]
        [InlineData("Example2-Color2.png")]
        [InlineData("Example2-Color1.png")]
        public void ValidateColor2(string fileName) {
            var bmpOriginal = GetExampleImageColor2();
            var bmp = new SimplePngImage(GetResourceStreamBytes(fileName));
            //bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-" + fileName));
            Assert.True(AreImagesSame(bmp, bmpOriginal));
        }

        [Theory(DisplayName = "SimplePngImage: Validate Colors 4")]
        [InlineData("Example4-Color32.png")]
        [InlineData("Example4-Color24.png")]
        [InlineData("Example4-Color8.png")]
        [InlineData("Example4-Color4.png")]
        [InlineData("Example4-Color2.png")]
        public void ValidateColor4(string fileName) {
            var bmpOriginal = GetExampleImageColor4();
            var bmp = new SimplePngImage(GetResourceStreamBytes(fileName));
            //bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-" + fileName));
            Assert.True(AreImagesSame(bmp, bmpOriginal));
        }

        [Theory(DisplayName = "SimplePngImage: Validate Colors 16")]
        [InlineData("Example16-Color32.png")]
        [InlineData("Example16-Color24.png")]
        [InlineData("Example16-Color8.png")]
        [InlineData("Example16-Color4.png")]
        public void ValidateColor16(string fileName) {
            var bmpOriginal = GetExampleImageColor16();
            var bmp = new SimplePngImage(GetResourceStreamBytes(fileName));
            //bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-" + fileName));
            Assert.True(AreImagesSame(bmp, bmpOriginal));
        }

        [Theory(DisplayName = "SimplePngImage: Validate Colors 64")]
        [InlineData("Example64-Color32.png")]
        [InlineData("Example64-Color24.png")]
        [InlineData("Example64-Color8.png")]
        public void ValidateColor64(string fileName) {
            var bmpOriginal = GetExampleImageColor64();
            var bmp = new SimplePngImage(GetResourceStreamBytes(fileName));
            //bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-" + fileName));
            Assert.True(AreImagesSame(bmp, bmpOriginal));
        }

        [Theory(DisplayName = "SimplePngImage: Validate Colors 256")]
        [InlineData("Example256-Color32.png")]
        [InlineData("Example256-Color24.png")]
        [InlineData("Example256-Color8.png")]
        public void ValidateColor256(string fileName) {
            var bmpOriginal = GetExampleImageColor256();
            var bmp = new SimplePngImage(GetResourceStreamBytes(fileName));
            //bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-" + fileName));
            Assert.True(AreImagesSame(bmp, bmpOriginal));
        }

        [Theory(DisplayName = "SimplePngImage: Validate Monochrome 256")]
        [InlineData("Example256-Mono32.png")]
        [InlineData("Example256-Mono24.png")]
        [InlineData("Example256-Mono8.png")]
        public void ValidateMono256(string fileName) {
            var bmpOriginal = GetExampleImageMonochrome256();
            var bmp = new SimplePngImage(GetResourceStreamBytes(fileName));
            //bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-" + fileName));
            Assert.True(AreImagesSame(bmp, bmpOriginal));
        }


        #region Helper

        private static Stream GetResourceStreamBytes(string fileName) {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Medo._Resources.Drawing.SimplePngImage." + fileName);
        }

        private static bool AreImagesSame(SimplePngImage image1, SimplePngImage image2) {
            if (image1.Width != image2.Width) { return false; }
            if (image1.Height != image2.Height) { return false; }
            for (var x = 0; x < image1.Width; x++) {
                for (var y = 0; y < image1.Height; y++) {
                    if (image1.GetPixel(x, y).ToArgb() != image2.GetPixel(x, y).ToArgb()) { return false; }
                }
            }
            return true;
        }

        private static SimplePngImage GetExampleImageColor2() {
            var bmp = new SimplePngImage(8, 8);
            for (var x = 0; x < 8; x++) {
                for (var y = 0; y < 8; y++) {
                    bmp.SetPixel(x, y, (x % 2 == y % 2) ? Color.Red : Color.Blue);
                }
            }
            //bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-color-2.png"));
            return bmp;
        }

        private static SimplePngImage GetExampleImageColor4() {
            var bmp = new SimplePngImage(8, 8);
            for (var x = 0; x < 2; x++) {
                for (var y = 0; y < 2; y++) {
                    var r = (byte)((7 - x) * 129);
                    var g = (byte)(y * 128);
                    var b = (byte)(((x * 128) & 0xF0) | ((y * 128) >> 4));

                    for (var i = 0; i < 4; i++) {
                        for (var j = 0; j < 4; j++) {
                            bmp.SetPixel(x * 4 + i, y * 4 + j, Color.FromArgb(r, g, b));
                        }
                    }
                }
            }
            //bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-color-4.png"));
            return bmp;
        }

        private static SimplePngImage GetExampleImageColor16() {
            var bmp = new SimplePngImage(8, 8);
            for (var x = 0; x < 4; x++) {
                for (var y = 0; y < 4; y++) {
                    var r = (byte)((7 - x) * 64);
                    var g = (byte)(y * 64);
                    var b = (byte)(((x * 64) & 0xF0) | ((y * 64) >> 4));

                    bmp.SetPixel(x * 2 + 0, y * 2 + 0, Color.FromArgb(r, g, b));
                    bmp.SetPixel(x * 2 + 0, y * 2 + 1, Color.FromArgb(r, g, b));
                    bmp.SetPixel(x * 2 + 1, y * 2 + 0, Color.FromArgb(r, g, b));
                    bmp.SetPixel(x * 2 + 1, y * 2 + 1, Color.FromArgb(r, g, b));
                }
            }
            //bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-color-16.png"));
            return bmp;
        }

        private static SimplePngImage GetExampleImageColor64() {
            var bmp = new SimplePngImage(8, 8);
            for (var x = 0; x < 8; x++) {
                for (var y = 0; y < 8; y++) {
                    var r = (byte)((7 - x) * 32);
                    var g = (byte)(y * 32);
                    var b = (byte)(((x * 32) & 0xF0) | ((y * 32) >> 4));
                    bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            //bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-color-64.png"));
            return bmp;
        }

        private static SimplePngImage GetExampleImageColor256() {
            var bmp = new SimplePngImage(16, 16);
            for (var x = 0; x < 16; x++) {
                for (var y = 0; y < 16; y++) {
                    var r = (byte)((15 - x) * 16);
                    var g = (byte)(y * 16);
                    var b = (byte)(((x * 16) & 0xF0) | ((y * 16) >> 4));
                    bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            //bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-color-256.png"));
            return bmp;
        }

        private static SimplePngImage GetExampleImageMonochrome256() {
            var bmp = new SimplePngImage(16, 16);
            for (var x = 0; x < 16; x++) {
                for (var y = 0; y < 16; y++) {
                    var m = x << 4 | y;
                    bmp.SetPixel(x, y, Color.FromArgb(m, m, m));
                }
            }
            bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "png-mono-256.png"));
            return bmp;
        }

        #endregion Helper

    }
}
