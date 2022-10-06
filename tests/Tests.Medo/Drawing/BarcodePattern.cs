using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Drawing;

namespace Tests;

[TestClass]
public class BarcodePattern_Tests {

    [TestMethod]
    public void BarcodePattern_CodabarNumbers() {
        var bp = BarcodePattern.GetNewCodabar("123");
        Assert.AreEqual("1-1-2-2-1-2-1-0-1-1-1-1-2-2-1-0-1-1-1-2-1-1-2-0-2-2-1-1-1-1-1-0-1-1-2-2-1-2-1", ToPatternString(bp.GetInterleavedPattern()));
        Assert.AreEqual("█ ██  █  █ █ █ ██  █ █ █  █ ██ ██  █ █ █ █ ██  █  █", ToPatternString(bp.GetBinaryPattern()));
        Assert.AreEqual(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
    }

    [TestMethod]
    public void BarcodePattern_CodaBarStartStop() {
        var bp = BarcodePattern.GetNewCodabar("123", 'D', 'B');
        Assert.AreEqual("1-1-1-2-2-2-1-0-1-1-1-1-2-2-1-0-1-1-1-2-1-1-2-0-2-2-1-1-1-1-1-0-1-1-1-2-1-2-2", ToPatternString(bp.GetInterleavedPattern()));
        Assert.AreEqual("█ █  ██  █ █ █ ██  █ █ █  █ ██ ██  █ █ █ █ █  █  ██", ToPatternString(bp.GetBinaryPattern()));
        Assert.AreEqual(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
    }

    [TestMethod]
    public void BarcodePattern_CodaBarNullValue() {
        Assert.ThrowsException<ArgumentNullException>(() => {
            var _ = BarcodePattern.GetNewCodabar(null);
        });
    }

    [TestMethod]
    public void BarcodePattern_CodaBarInvalidValue() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var _ = BarcodePattern.GetNewCodabar("123A");
        });
    }

    [TestMethod]
    public void BarcodePattern_CodaBarInvalidStart() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var _ = BarcodePattern.GetNewCodabar("123", 'E', 'A');
        });
    }

    [TestMethod]
    public void BarcodePattern_CodaBarInvalidEnd() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var _ = BarcodePattern.GetNewCodabar("123", 'A', 'E');
        });
    }

    [TestMethod]
    public void BarcodePattern_CodabarWritePng() {
        var bp = BarcodePattern.GetNewCodabar("1234");
        var memStream = new MemoryStream();
        bp.SaveAsPng(memStream);
        //bp.SaveAsPng(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "codabar-24.png"));
#if NET6_0
        Assert.AreEqual(144, memStream.Length);
        Assert.AreEqual("89504E470D0A1A0A0000000D49484452000000430000000F08020000003032E5330000005749444154789CED95CB0AC0201003E7FF7F3A45A5521F3D55B14AE62041836C5057740AAC2E6018AC2E604212089A88229482724CA2BBDADAB278DBAD32779DD5FC5304AD1B9C049F897CBBF03B71EF92BB30DFFE93DD7192FF71015FA30C3C0000000049454E44AE426082",
                        BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
#elif NET7_0_OR_GREATER
        Assert.AreEqual(126, memStream.Length);
        Assert.AreEqual("89504E470D0A1A0A0000000D49484452000000430000000F08020000003032E5330000004549444154789C63F83F5C00C3403B806A6038FA848101C4069210065C04590A8F32641253197EF5988AB1AAC4E5B0519F8CC6C968EA1ACD27A365D768294C8BFA64A883519F0C3E0000E83DFC640000000049454E44AE426082",
                        BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
#endif
    }

    [TestMethod]
    public void BarcodePattern_CodabarWriteTransparentPng() {
        var bp = BarcodePattern.GetNewCodabar("1234");
        var memStream = new MemoryStream();
        bp.SaveAsPng(memStream, Color.Blue, Color.Transparent);
        //bp.SaveAsPng(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "codabar-32.png"));
#if NET6_0
        Assert.AreEqual(208, memStream.Length);
        Assert.AreEqual("89504E470D0A1A0A0000000D49484452000000430000000F0806000000BF5072640000009749444154789CED93C10AC5200C04E7FF7F7A9EBE5648416D6E2D34874D5C70C02C069592FF0C2A088F206CA2174BF40C8841D0AB47E73C7BFADE0D67269DC43D170C499E05BBE3B899C5E18D8656AD30A830AC9F71D90CA3A155737BC6A493B8E78221C9B360771C37B338BCD1D0AA15061586F5332E9B6134B46A6ECF987412F75C30247916EC8EE366168737183F2E9E7E802F124F3FC017E907EB28F7C90000000049454E44AE426082",
                        BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
#elif NET7_0_OR_GREATER
        Assert.AreEqual(142, memStream.Length);
        Assert.AreEqual("89504E470D0A1A0A0000000D49484452000000430000000F0806000000BF5072640000005549444154789C63F8FFFF3FC3288684C16840202586D1C0C01518601E9486B191C5D1D510A3079DC6A58758FDB8F4E2D347C82FA381311A18A32963349B8C9619A305E8686D325AB5D2B59D31D2F1803B6030E10177C060C200A239C9CC0000000049454E44AE426082",
                        BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
#endif
    }


    [TestMethod]
    public void BarcodePattern_Code128Numbers() {
        var bp = BarcodePattern.GetNewCode128("123");
        Assert.AreEqual("2-1-1-2-3-2-1-1-2-2-3-2-3-1-1-1-4-1-2-2-1-1-3-2-1-4-1-1-2-2-2-3-3-1-1-1-2", ToPatternString(bp.GetInterleavedPattern()));
        Assert.AreEqual("██ █  ███  █ ██  ███  ███ █ ████ ██  █ ███  █    █ ██  ██   ███ █ ██", ToPatternString(bp.GetBinaryPattern()));
        Assert.AreEqual(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
    }

    [TestMethod]
    public void BarcodePattern_Code128Text() {
        var bp = BarcodePattern.GetNewCode128("ABC");
        Assert.AreEqual("2-1-1-4-1-2-1-1-1-3-2-3-1-3-1-1-2-3-1-3-1-3-2-1-2-1-2-2-2-2-2-3-3-1-1-1-2", ToPatternString(bp.GetInterleavedPattern()));
        Assert.AreEqual("██ █    █  █ █   ██   █   █ ██   █   █   ██ ██ ██  ██  ██   ███ █ ██", ToPatternString(bp.GetBinaryPattern()));
        Assert.AreEqual(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
    }

    [TestMethod]
    public void BarcodePattern_Code128Control() {
        var bp = BarcodePattern.GetNewCode128("\r\n");
        Assert.AreEqual("2-1-1-4-1-2-4-1-3-1-1-1-1-4-2-2-1-1-2-2-1-1-3-2-2-3-3-1-1-1-2", ToPatternString(bp.GetInterleavedPattern()));
        Assert.AreEqual("██ █    █  ████ ███ █ █    ██  █ ██  █ ███  ██   ███ █ ██", ToPatternString(bp.GetBinaryPattern()));
        Assert.AreEqual(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
    }

    [TestMethod]
    public void BarcodePattern_Code128DenseNumbers() {
        var bp = BarcodePattern.GetNewCode128("1234");
        Assert.AreEqual("2-1-1-2-3-2-1-1-2-2-3-2-1-3-1-1-2-3-1-2-1-2-4-1-2-3-3-1-1-1-2", ToPatternString(bp.GetInterleavedPattern()));
        Assert.AreEqual("██ █  ███  █ ██  ███  █   █ ██   █  █  ████ ██   ███ █ ██", ToPatternString(bp.GetBinaryPattern()));
        Assert.AreEqual(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
    }

    [TestMethod]
    public void BarcodePattern_Code128NullValue() {
        Assert.ThrowsException<ArgumentNullException>(() => {
            var _ = BarcodePattern.GetNewCode128(null);
        });
    }

    [TestMethod]
    public void BarcodePattern_Code128InvalidValue() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var _ = BarcodePattern.GetNewCode128("Č");
        });
    }

    [TestMethod]
    public void BarcodePattern_Code128WritePng() {
        var bp = BarcodePattern.GetNewCode128("1234");
        var memStream = new MemoryStream();
        bp.SaveAsPng(memStream);
        //bp.SaveAsPng(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "code128-24.png"));
#if NET6_0
        Assert.AreEqual(158, memStream.Length);
        Assert.AreEqual("89504E470D0A1A0A0000000D494844520000003F0000000F0802000000DC4780250000006549444154789CED94510AC0300843BDFFA533E947964D1C8C0DA4907C149B3E4A28D6C0CE8AE9009F14D3017E4A1F4B5805C4D12DCD07A08315A83774BC1EA1026775E5E8E896269D0A74B002F5868E777ABF7D2ADB20D75B41C19DE35F0BCF1CBC99983BCAE9E77400667F9C630000000049454E44AE426082",
                        BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
#elif NET7_0_OR_GREATER
        Assert.AreEqual(123, memStream.Length);
        Assert.AreEqual("89504E470D0A1A0A0000000D494844520000003F0000000F0802000000DC4780250000004249444154789C63F83F9401C3403B8022305C5CCF00061006B208321759192E05B814A3998C66022EF568AE425730EAFAD1B01F4D39A3B976B4CC196125E65004A3AE1F380000CA0E14590000000049454E44AE426082",
                        BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
#endif
    }

    [TestMethod]
    public void BarcodePattern_Code128WriteTransparentPng() {
        var bp = BarcodePattern.GetNewCode128("1234");
        var memStream = new MemoryStream();
        bp.SaveAsPng(memStream, Color.Blue, Color.Transparent);
        //bp.SaveAsPng(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "code128-32.png"));
#if NET6_0
        Assert.AreEqual(174, memStream.Length);
        Assert.AreEqual("89504E470D0A1A0A0000000D494844520000003F0000000F0806000000532517720000007549444154789CED95C10EC0200843FBFF3FFD16B29938C2C261070EF640AC48151051804E154D3B8083672409EF891E0D37CE7A25DDBE1698C696864B71EE8E95F6D6075F451CAA6C29880BC78883C7379FAB61AF8AC034B6345C92AD5CF6F8CD4715C80D0F77FBD513E4AF8EFFFFFC69A2690770F08C24E102AC515D030000000049454E44AE426082",
                        BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
#elif NET7_0_OR_GREATER
        Assert.AreEqual(137, memStream.Length);
        Assert.AreEqual("89504E470D0A1A0A0000000D494844520000003F0000000F0806000000532517720000005049444154789C63F8FFFF3FC348C503EE8051CF0F869807F1C0225036BA38BA18BA1E426A09E9C5662F367309E9C7E60FAC6A473D3F1AF3A3C97E34CF8F1678A3A5FD68553732EAF9918607DC01A39E1FA040000020E1C65B0000000049454E44AE426082",
                        BitConverter.ToString(memStream.ToArray()).Replace("-", ""));
#endif
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
