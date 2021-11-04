using System;
using System.IO;
using System.Text;
using Xunit;
using Medo.Drawing;
using System.Globalization;
using System.Drawing;

namespace Tests.Medo.Drawing {
    public class BarcodePatternTests {

        [Fact(DisplayName = "BarcodePattern: Codabar Numbers")]
        public void CodabarNumbers() {
            var bp = BarcodePattern.GetNewCodabar("123");
            Assert.Equal("1-1-2-2-1-2-1-0-1-1-1-1-2-2-1-0-1-1-1-2-1-1-2-0-2-2-1-1-1-1-1-0-1-1-2-2-1-2-1", ToPatternString(bp.GetInterleavedPattern()));
            Assert.Equal("█ ██  █  █ █ █ ██  █ █ █  █ ██ ██  █ █ █ █ ██  █  █", ToPatternString(bp.GetBinaryPattern()));
            Assert.Equal(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
        }

        [Fact(DisplayName = "BarcodePattern: Codabar Custom Start/Stop")]
        public void CodaBarStartStop() {
            var bp = BarcodePattern.GetNewCodabar("123", 'D', 'B');
            Assert.Equal("1-1-1-2-2-2-1-0-1-1-1-1-2-2-1-0-1-1-1-2-1-1-2-0-2-2-1-1-1-1-1-0-1-1-1-2-1-2-2", ToPatternString(bp.GetInterleavedPattern()));
            Assert.Equal("█ █  ██  █ █ █ ██  █ █ █  █ ██ ██  █ █ █ █ █  █  ██", ToPatternString(bp.GetBinaryPattern()));
            Assert.Equal(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
        }

        [Fact(DisplayName = "BarcodePattern: Codabar Null Value")]
        public void CodaBarNullValue() {
            Assert.Throws<ArgumentNullException>(() => {
                var _ = BarcodePattern.GetNewCodabar(null);
            });
        }

        [Fact(DisplayName = "BarcodePattern: Codabar Invalid Value Character")]
        public void CodaBarInvalidValue() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var _ = BarcodePattern.GetNewCodabar("123A");
            });
        }

        [Fact(DisplayName = "BarcodePattern: Codabar Invalid Start Character")]
        public void CodaBarInvalidStart() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var _ = BarcodePattern.GetNewCodabar("123", 'E', 'A');
            });
        }

        [Fact(DisplayName = "BarcodePattern: Codabar Invalid End Character")]
        public void CodaBarInvalidEnd() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var _ = BarcodePattern.GetNewCodabar("123", 'A', 'E');
            });
        }

        [Fact(DisplayName = "BarcodePattern: Codabar Write PNG")]
        public void CodabarWritePng() {
            var bp = BarcodePattern.GetNewCodabar("1234");
            var memStream = new MemoryStream();
            bp.SaveAsPng(memStream);
            //bp.SaveAsPng(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "codabar-24.png"));
            Assert.Equal(144, memStream.Length);
            Assert.Equal("89504E470D0A1A0A0000000D49484452000000430000000F08020000003032E5330000005749444154789CED95CB0AC0201003E7FF7F3A45A5521F3D55B14AE62041836C5057740AAC2E6018AC2E604212089A88229482724CA2BBDADAB278DBAD32779DD5FC5304AD1B9C049F897CBBF03B71EF92BB30DFFE93DD7192FF71015FA30C3C0000000049454E44AE426082",
                         BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
        }

        [Fact(DisplayName = "BarcodePattern: Codabar Write Transparent PNG")]
        public void CodabarWriteTransparentPng() {
            var bp = BarcodePattern.GetNewCodabar("1234");
            var memStream = new MemoryStream();
            bp.SaveAsPng(memStream, Color.Blue, Color.Transparent);
            //bp.SaveAsPng(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "codabar-32.png"));
            Assert.Equal(208, memStream.Length);
            Assert.Equal("89504E470D0A1A0A0000000D49484452000000430000000F0806000000BF5072640000009749444154789CED93C10AC5200C04E7FF7F7A9EBE5648416D6E2D34874D5C70C02C069592FF0C2A088F206CA2174BF40C8841D0AB47E73C7BFADE0D67269DC43D170C499E05BBE3B899C5E18D8656AD30A830AC9F71D90CA3A155737BC6A493B8E78221C9B360771C37B338BCD1D0AA15061586F5332E9B6134B46A6ECF987412F75C30247916EC8EE366168737183F2E9E7E802F124F3FC017E907EB28F7C90000000049454E44AE426082",
                         BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
        }


        [Fact(DisplayName = "BarcodePattern: Code128 Numbers")]
        public void Code128Numbers() {
            var bp = BarcodePattern.GetNewCode128("123");
            Assert.Equal("2-1-1-2-3-2-1-1-2-2-3-2-3-1-1-1-4-1-2-2-1-1-3-2-1-4-1-1-2-2-2-3-3-1-1-1-2", ToPatternString(bp.GetInterleavedPattern()));
            Assert.Equal("██ █  ███  █ ██  ███  ███ █ ████ ██  █ ███  █    █ ██  ██   ███ █ ██", ToPatternString(bp.GetBinaryPattern()));
            Assert.Equal(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
        }

        [Fact(DisplayName = "BarcodePattern: Code128 Text")]
        public void Code128Text() {
            var bp = BarcodePattern.GetNewCode128("ABC");
            Assert.Equal("2-1-1-4-1-2-1-1-1-3-2-3-1-3-1-1-2-3-1-3-1-3-2-1-2-1-2-2-2-2-2-3-3-1-1-1-2", ToPatternString(bp.GetInterleavedPattern()));
            Assert.Equal("██ █    █  █ █   ██   █   █ ██   █   █   ██ ██ ██  ██  ██   ███ █ ██", ToPatternString(bp.GetBinaryPattern()));
            Assert.Equal(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
        }

        [Fact(DisplayName = "BarcodePattern: Code128 Control")]
        public void Code128Control() {
            var bp = BarcodePattern.GetNewCode128("\r\n");
            Assert.Equal("2-1-1-4-1-2-4-1-3-1-1-1-1-4-2-2-1-1-2-2-1-1-3-2-2-3-3-1-1-1-2", ToPatternString(bp.GetInterleavedPattern()));
            Assert.Equal("██ █    █  ████ ███ █ █    ██  █ ██  █ ███  ██   ███ █ ██", ToPatternString(bp.GetBinaryPattern()));
            Assert.Equal(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
        }

        [Fact(DisplayName = "BarcodePattern: Code128 Dense Numbers")]
        public void Code128DenseNumbers() {
            var bp = BarcodePattern.GetNewCode128("1234");
            Assert.Equal("2-1-1-2-3-2-1-1-2-2-3-2-1-3-1-1-2-3-1-2-1-2-4-1-2-3-3-1-1-1-2", ToPatternString(bp.GetInterleavedPattern()));
            Assert.Equal("██ █  ███  █ ██  ███  █   █ ██   █  █  ████ ██   ███ █ ██", ToPatternString(bp.GetBinaryPattern()));
            Assert.Equal(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
        }

        [Fact(DisplayName = "BarcodePattern: Code128 Null Value")]
        public void Code128NullValue() {
            Assert.Throws<ArgumentNullException>(() => {
                var _ = BarcodePattern.GetNewCode128(null);
            });
        }

        [Fact(DisplayName = "BarcodePattern: Code128 Invalid Value Character")]
        public void Code128InvalidValue() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var _ = BarcodePattern.GetNewCode128("Č");
            });
        }

        [Fact(DisplayName = "BarcodePattern: Code128 Write PNG")]
        public void Code128WritePng() {
            var bp = BarcodePattern.GetNewCode128("1234");
            var memStream = new MemoryStream();
            bp.SaveAsPng(memStream);
            //bp.SaveAsPng(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "code128-24.png"));
            Assert.Equal(158, memStream.Length);
            Assert.Equal("89504E470D0A1A0A0000000D494844520000003F0000000F0802000000DC4780250000006549444154789CED94510AC0300843BDFFA533E947964D1C8C0DA4907C149B3E4A28D6C0CE8AE9009F14D3017E4A1F4B5805C4D12DCD07A08315A83774BC1EA1026775E5E8E896269D0A74B002F5868E777ABF7D2ADB20D75B41C19DE35F0BCF1CBC99983BCAE9E77400667F9C630000000049454E44AE426082",
                         BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
        }

        [Fact(DisplayName = "BarcodePattern: Code128 Write Transparent PNG")]
        public void Code128WriteTransparentPng() {
            var bp = BarcodePattern.GetNewCode128("1234");
            var memStream = new MemoryStream();
            bp.SaveAsPng(memStream, Color.Blue, Color.Transparent);
            //bp.SaveAsPng(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "code128-32.png"));
            Assert.Equal(174, memStream.Length);
            Assert.Equal("89504E470D0A1A0A0000000D494844520000003F0000000F0806000000532517720000007549444154789CED95C10EC0200843FBFF3FFD16B29938C2C261070EF640AC48151051804E154D3B8083672409EF891E0D37CE7A25DDBE1698C696864B71EE8E95F6D6075F451CAA6C29880BC78883C7379FAB61AF8AC034B6345C92AD5CF6F8CD4715C80D0F77FBD513E4AF8EFFFFFC69A2690770F08C24E102AC515D030000000049454E44AE426082",
                         BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
        }


        #region Convert

        private static string ToPatternString(int[] pattern) {
            var sb = new StringBuilder();
            foreach (var entry in pattern) {
                if (sb.Length > 0) { sb.Append('-'); }
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", entry);
            }
            return sb.ToString();
        }

        private static string ToPatternString(bool[] pattern) {
            var sb = new StringBuilder();
            foreach (var entry in pattern) {
                sb.Append(entry ? '█' : ' ');
            }
            return sb.ToString();
        }

        #endregion Convert

    }
}
