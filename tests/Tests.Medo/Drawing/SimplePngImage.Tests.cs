using System;
using System.Drawing;
using System.IO;
using Xunit;
using Medo.Drawing;

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

    }
}
