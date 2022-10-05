using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Security.Checksum;

namespace Tests;

[TestClass]
public class Crc32_Tests {

    [TestMethod]
    public void Crc32_Custom() {
        string expected = "0x1A657BE2";
        var crc = Crc32.GetCustom(0x04C11DB7, unchecked((int)0xFFFFFFFF), true, true, unchecked((int)0xFFFFFFFF));
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt32:X8}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }


    [TestMethod]
    public void Crc32_Aixm() {
        string expected = "0x06D88232";
        var crc = Crc32.GetAixm();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt32:X8}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc32_Aixm_2() {
        var crc = Crc32.GetAixm();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0x3010BF7F, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_Autosar() {
        string expected = "0x510EF1D8";
        var crc = Crc32.GetAutosar();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt32:X8}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc32_Autosar_2() {
        var crc = Crc32.GetAutosar();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0x1697D06A, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_Base91D() {
        string expected = "0x2B61E75F";
        var crc = Crc32.GetBase91D();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt32:X8}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc32_Base91D_2() {
        var crc = Crc32.GetBase91D();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0x87315576, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_BZip2() {
        string expected = "0xBB7A12E7";
        var crc = Crc32.GetBZip2();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt32:X8}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc32_BZip2_2() {
        var crc = Crc32.GetBZip2();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xFC891918, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_Aal5_A() {
        var crc = Crc32.GetAal5();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xFC891918, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_DectB_A() {
        var crc = Crc32.GetDectB();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xFC891918, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_CdromEdc() {
        string expected = "0x0006D785";
        var crc = Crc32.GetCdromEdc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt32:X8}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc32_CdromEdc_2() {
        var crc = Crc32.GetCdromEdc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0x6EC2EDC4, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_Cksum() {
        string expected = "0xEFC8804E";
        var crc = Crc32.GetCksum();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt32:X8}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc32_Cksum_2() {
        var crc = Crc32.GetCksum();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0x765E7680, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_Posix_A() {
        var crc = Crc32.GetPosix();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0x765E7680, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_IScsi() {
        string expected = "0x1F9A516E";
        var crc = Crc32.GetIScsi();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt32:X8}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc32_IScsi_2() {
        var crc = Crc32.GetIScsi();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xE3069283, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_Base91C_A() {
        var crc = Crc32.GetBase91C();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xE3069283, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_Castagnoli_A() {
        var crc = Crc32.GetCastagnoli();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xE3069283, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_Interlaken_A() {
        var crc = Crc32.GetInterlaken();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xE3069283, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_IsoHdlc() {
        string expected = "0x1A657BE2";
        var crc = Crc32.GetIsoHdlc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt32:X8}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc32_IsoHdlc_2() {
        var crc = Crc32.GetIsoHdlc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xCBF43926, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_Adccp_A() {
        var crc = Crc32.GetAdccp();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xCBF43926, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_Ieee_A() {
        var crc = Crc32.GetIeee();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xCBF43926, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_Ieee8023_A() {
        var crc = Crc32.GetIeee8023();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xCBF43926, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_V42_A() {
        var crc = Crc32.GetV42();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xCBF43926, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_XZ_A() {
        var crc = Crc32.GetXZ();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xCBF43926, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_PkZip_A() {
        var crc = Crc32.GetPkZip();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xCBF43926, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_JamCrc() {
        string expected = "0xE59A841D";
        var crc = Crc32.GetJamCrc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt32:X8}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc32_JamCrc_2() {
        var crc = Crc32.GetJamCrc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0x340BC6D9, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_Jam_A() {
        var crc = Crc32.GetJamCrc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0x340BC6D9, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_Mpeg2() {
        string expected = "0x4485ED18";
        var crc = Crc32.GetMpeg2();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt32:X8}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc32_Mpeg2_2() {
        var crc = Crc32.GetMpeg2();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0x0376E6E7, (uint)crc.HashAsInt32);
    }

    [TestMethod]
    public void Crc32_Xfer() {
        string expected = "0x3A9C355C";
        var crc = Crc32.GetXfer();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt32:X8}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc32_Xfef_2() {
        var crc = Crc32.GetXfer();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual((uint)0xBD0BE338, (uint)crc.HashAsInt32);
    }


    [TestMethod]
    public void Crc32_Reuse() {
        var checksum = Crc32.GetIeee();
        checksum.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("1A657BE2", checksum.HashAsInt32.ToString("X8"));
        checksum.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("1A657BE2", checksum.HashAsInt32.ToString("X8"));
    }


    [TestMethod]
    public void Crc32_ToReversedReciprocal() {
        unchecked {
            Assert.AreEqual((int)0x82608EDB, Crc32.ToReversedReciprocalPolynomial((int)0x04C11DB7));
            Assert.AreEqual((int)0x8F6E37A0, Crc32.ToReversedReciprocalPolynomial((int)0x1EDC6F41));
            Assert.AreEqual((int)0xBA0DC66B, Crc32.ToReversedReciprocalPolynomial((int)0x741B8CD7));
            Assert.AreEqual((int)0x992C1A4C, Crc32.ToReversedReciprocalPolynomial((int)0x32583499));
            Assert.AreEqual((int)0xC0A0A0D5, Crc32.ToReversedReciprocalPolynomial((int)0x814141AB));
        }
    }

    [TestMethod]
    public void Crc32_FromReversedReciprocal() {
        unchecked {
            Assert.AreEqual((int)0x04C11DB7, Crc32.FromReversedReciprocalPolynomial((int)0x82608EDB));
            Assert.AreEqual((int)0x1EDC6F41, Crc32.FromReversedReciprocalPolynomial((int)0x8F6E37A0));
            Assert.AreEqual((int)0x741B8CD7, Crc32.FromReversedReciprocalPolynomial((int)0xBA0DC66B));
            Assert.AreEqual((int)0x32583499, Crc32.FromReversedReciprocalPolynomial((int)0x992C1A4C));
            Assert.AreEqual((int)0x814141AB, Crc32.FromReversedReciprocalPolynomial((int)0xC0A0A0D5));

            Assert.AreEqual((int)0x5A0849E7, Crc32.FromReversedReciprocalPolynomial((int)0xAD0424F3));  // 4294967263 @ HD=3
            Assert.AreEqual((int)0x93A409EB, Crc32.FromReversedReciprocalPolynomial((int)0xC9D204F5));  // 2147483615 @ HD=4
            Assert.AreEqual((int)0xA833982B, Crc32.FromReversedReciprocalPolynomial((int)0xD419CC15));  // 65505 @ HD=5
            Assert.AreEqual((int)0x32C00699, Crc32.FromReversedReciprocalPolynomial((int)0x9960034C));  // 32738 @ HD=6
            Assert.AreEqual((int)0xF1922815, Crc32.FromReversedReciprocalPolynomial((int)0xF8C9140A));  // 992 @ HD=7
            Assert.AreEqual((int)0xF1922815, Crc32.FromReversedReciprocalPolynomial((int)0xF8C9140A));  // 992 @ HD=8
            Assert.AreEqual((int)0x3AFF2FAD, Crc32.FromReversedReciprocalPolynomial((int)0x9D7F97D6));  // 223 @ HD=9
            Assert.AreEqual((int)0x6938392D, Crc32.FromReversedReciprocalPolynomial((int)0xB49C1C96));  // 100 @ HD=10
            Assert.AreEqual((int)0xB72AC3B, Crc32.FromReversedReciprocalPolynomial((int)0x85B9561D));  // 38 @ HD=11
            Assert.AreEqual((int)0x2A1D7F5D, Crc32.FromReversedReciprocalPolynomial((int)0x950EBFAE));  // 36 @ HD=12
            Assert.AreEqual((int)0x27673637, Crc32.FromReversedReciprocalPolynomial((int)0x93B39B1B));  // 20 @ HD=13
            Assert.AreEqual((int)0x41295F6B, Crc32.FromReversedReciprocalPolynomial((int)0xA094AFB5));  // 19 @ HD=14
            Assert.AreEqual((int)0x44AE52C5, Crc32.FromReversedReciprocalPolynomial((int)0xA2572962));  // 15 @ HD=15
            Assert.AreEqual((int)0xD120C3B7, Crc32.FromReversedReciprocalPolynomial((int)0xE89061DB));  // 13 @ HD=16
            Assert.AreEqual((int)0x50D7C9B7, Crc32.FromReversedReciprocalPolynomial((int)0xA86bE4DB));  // 7 @ HD=17
            Assert.AreEqual((int)0x2E75F6A3, Crc32.FromReversedReciprocalPolynomial((int)0x973AFB51));  // 5 @ HD=18
        }
    }

    [TestMethod]
    public void Crc32_ToReversedReciprocal2() {
        unchecked {
            Assert.AreEqual((int)0x82608EDB, Crc32.ToReversedReciprocalPolynomial(0x04C11DB7L));
            Assert.AreEqual((int)0x8F6E37A0, Crc32.ToReversedReciprocalPolynomial(0x1EDC6F41L));
            Assert.AreEqual((int)0xBA0DC66B, Crc32.ToReversedReciprocalPolynomial(0x741B8CD7L));
            Assert.AreEqual((int)0x992C1A4C, Crc32.ToReversedReciprocalPolynomial(0x32583499L));
            Assert.AreEqual((int)0xC0A0A0D5, Crc32.ToReversedReciprocalPolynomial(0x814141ABL));
        }
    }

    [TestMethod]
    public void Crc32_FromReversedReciprocal2() {
        unchecked {
            Assert.AreEqual((int)0x04C11DB7, Crc32.FromReversedReciprocalPolynomial(0x82608EDBL));
            Assert.AreEqual((int)0x1EDC6F41, Crc32.FromReversedReciprocalPolynomial(0x8F6E37A0L));
            Assert.AreEqual((int)0x741B8CD7, Crc32.FromReversedReciprocalPolynomial(0xBA0DC66BL));
            Assert.AreEqual((int)0x32583499, Crc32.FromReversedReciprocalPolynomial(0x992C1A4CL));
            Assert.AreEqual((int)0x814141AB, Crc32.FromReversedReciprocalPolynomial(0xC0A0A0D5L));
        }
    }

}
