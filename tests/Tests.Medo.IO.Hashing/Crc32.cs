using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.IO.Hashing;

namespace Tests;

[TestClass]
public class Crc32_Tests {

    [TestMethod]
    public void Crc32_Custom() {
        var crc = Crc32.GetCustom(0x04C11DB7, unchecked(0xFFFFFFFF), true, true, unchecked(0xFFFFFFFF));
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xCBF43926U, crc.HashAsUInt32);
    }

    [TestMethod]
    public void Crc32_Custom_2() {
        var crc = Crc32.GetCustom(0x04C11DB7, unchecked(0xFFFFFFFF), true, true, unchecked(0xFFFFFFFF));
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("1A657BE2", $"{crc.HashAsUInt32:X8}");
        Assert.AreEqual("E27B651A", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]
    public void Crc32_Default() {
        var crcDotNet = new System.IO.Hashing.Crc32();
        crcDotNet.Append(Encoding.ASCII.GetBytes("123456789"));

        var crc = Crc32.GetDefault();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xCBF43926, crc.HashAsUInt32);

        Assert.AreEqual(BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""),
                        BitConverter.ToString(crcDotNet.GetCurrentHash()).Replace("-", ""));
    }


    [TestMethod]  // AIXM
    public void Crc32_Aixm() {
        var crc = Crc32.GetAixm();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x3010BF7FU, crc.HashAsUInt32);
    }

    [TestMethod]  // AIXM
    public void Crc32_Aixm_2() {
        var crc = Crc32.GetAixm();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("06D88232", $"{crc.HashAsUInt32:X8}");
        Assert.AreEqual("06D88232", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // AUTOSAR
    public void Crc32_Autosar() {
        var crc = Crc32.GetAutosar();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x1697D06AU, crc.HashAsUInt32);
    }

    [TestMethod]  // AUTOSAR
    public void Crc32_Autosar_2() {
        var crc = Crc32.GetAutosar();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("510EF1D8", $"{crc.HashAsUInt32:X8}");
        Assert.AreEqual("D8F10E51", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // BASE91-D
    public void Crc32_Base91D() {
        var crc = Crc32.GetBase91D();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x87315576, crc.HashAsUInt32);
    }

    [TestMethod]  // BASE91-D
    public void Crc32_Base91D_2() {
        var crc = Crc32.GetBase91D();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("2B61E75F", $"{crc.HashAsUInt32:X8}");
        Assert.AreEqual("5FE7612B", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // BZIP2
    public void Crc32_BZip2() {
        var crc = Crc32.GetBZip2();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xFC891918, crc.HashAsUInt32);
    }

    [TestMethod]  // BZIP2
    public void Crc32_BZip2_2() {
        var crc = Crc32.GetBZip2();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("BB7A12E7", $"{crc.HashAsUInt32:X8}");
        Assert.AreEqual("BB7A12E7", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // BZIP2 / AAL5 (A)
    public void Crc32_Aal5_A() {
        var crc = Crc32.GetAal5();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xFC891918, crc.HashAsUInt32);
    }

    [TestMethod]  // BZIP2 / DECT-B (A)
    public void Crc32_DectB_A() {
        var crc = Crc32.GetDectB();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xFC891918, crc.HashAsUInt32);
    }

    [TestMethod]  // CD-ROM-EDC
    public void Crc32_CdromEdc() {
        var crc = Crc32.GetCdromEdc();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x6EC2EDC4U, crc.HashAsUInt32);
    }

    [TestMethod]  // CD-ROM-EDC
    public void Crc32_CdromEdc_2() {
        var crc = Crc32.GetCdromEdc();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("0006D785", $"{crc.HashAsUInt32:X8}");
        Assert.AreEqual("85D70600", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // CKSUM
    public void Crc32_Cksum() {
        var crc = Crc32.GetCksum();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x765E7680U, crc.HashAsUInt32);
    }

    [TestMethod]  // CKSUM
    public void Crc32_Cksum_2() {
        var crc = Crc32.GetCksum();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("EFC8804E", $"{crc.HashAsUInt32:X8}");
        Assert.AreEqual("EFC8804E", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // CKSUM / POSIX (A)
    public void Crc32_Posix_A() {
        var crc = Crc32.GetPosix();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x765E7680U, crc.HashAsUInt32);
    }

    [TestMethod]  // ISCSI
    public void Crc32_IScsi() {
        var crc = Crc32.GetIScsi();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xE3069283, crc.HashAsUInt32);
    }

    [TestMethod]  // ISCSI
    public void Crc32_IScsi_2() {
        var crc = Crc32.GetIScsi();
    crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("1F9A516E", $"{crc.HashAsUInt32:X8}");
        Assert.AreEqual("6E519A1F", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

[TestMethod]  // ISCSI / BASE91-C (A)
    public void Crc32_Base91C_A() {
        var crc = Crc32.GetBase91C();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xE3069283, crc.HashAsUInt32);
    }

    [TestMethod]  // ISCSI / CASTAGNOLI (A)
    public void Crc32_Castagnoli_A() {
        var crc = Crc32.GetCastagnoli();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xE3069283, crc.HashAsUInt32);
    }

    [TestMethod]  // ISCSI / INTERLAKEN (A)
    public void Crc32_Interlaken_A() {
        var crc = Crc32.GetInterlaken();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xE3069283, crc.HashAsUInt32);
    }

    [TestMethod]  // ISO-HDLC
    public void Crc32_IsoHdlc_1() {
        var crc = Crc32.GetIsoHdlc();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xCBF43926, crc.HashAsUInt32);
    }

    [TestMethod]  // ISO-HDLC
    public void Crc32_IsoHdlc_2() {
        var crc = Crc32.GetIsoHdlc();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("1A657BE2", $"{crc.HashAsUInt32:X8}");
        Assert.AreEqual("E27B651A", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // ISO-HDLC / ADCCP (A)
    public void Crc32_Adccp_A() {
        var crc = Crc32.GetAdccp();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xCBF43926, crc.HashAsUInt32);
    }

    [TestMethod]  // ISO-HDLC / IEEE (A)
    public void Crc32_Ieee_A() {
        var crc = Crc32.GetIeee();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xCBF43926, crc.HashAsUInt32);
    }

    [TestMethod]  // ISO-HDLC / IEEE-802.3 (A)
    public void Crc32_Ieee8023_A() {
        var crc = Crc32.GetIeee8023();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xCBF43926, crc.HashAsUInt32);
    }

    [TestMethod]  // ISO-HDLC / V-42 (A)
    public void Crc32_V42_A() {
        var crc = Crc32.GetV42();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xCBF43926, crc.HashAsUInt32);
    }

    [TestMethod]  // ISO-HDLC / XZ (A)
    public void Crc32_XZ_A() {
        var crc = Crc32.GetXZ();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xCBF43926, crc.HashAsUInt32);
    }

    [TestMethod]  // ISO-HDLC / PKZIP (A)
    public void Crc32_PkZip_A() {
        var crc = Crc32.GetPkZip();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xCBF43926, crc.HashAsUInt32);
    }

    [TestMethod]  // JAMCRC
    public void Crc32_JamCrc() {
        var crc = Crc32.GetJamCrc();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x340BC6D9U, crc.HashAsUInt32);
    }

    [TestMethod]  // JAMCRC
    public void Crc32_JamCrc_2() {
        var crc = Crc32.GetJamCrc();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("E59A841D", $"{crc.HashAsUInt32:X8}");
        Assert.AreEqual("1D849AE5", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // JAMCRC / JAM (A)
    public void Crc32_Jam_A() {
        var crc = Crc32.GetJamCrc();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x340BC6D9U, crc.HashAsUInt32);
    }

    [TestMethod]  // MPEG-2
    public void Crc32_Mpeg2() {
        var crc = Crc32.GetMpeg2();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x0376E6E7U, crc.HashAsUInt32);
    }

    [TestMethod]  // MPEG-2
    public void Crc32_Mpeg2_2() {
        var crc = Crc32.GetMpeg2();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("4485ED18", $"{crc.HashAsUInt32:X8}");
        Assert.AreEqual("4485ED18", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // XFER
    public void Crc32_Xfef_1() {
        var crc = Crc32.GetXfer();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xBD0BE338, crc.HashAsUInt32);
    }

    [TestMethod]  // XFER
    public void Crc32_Xfer_2() {
        var crc = Crc32.GetXfer();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("3A9C355C", $"{crc.HashAsUInt32:X8}");
        Assert.AreEqual("3A9C355C", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }


    [TestMethod]
    public void Crc32_Reuse() {
        var checksum = Crc32.GetIeee();
        checksum.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("E27B651A", BitConverter.ToString(checksum.GetHashAndReset()).Replace("-", ""));
        checksum.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("E27B651A", BitConverter.ToString(checksum.GetCurrentHash()).Replace("-", ""));
    }


    [TestMethod]
    public void Crc32_ToReversedReciprocal() {
        unchecked {
            Assert.AreEqual(0x82608EDB, Crc32.ToReversedReciprocalPolynomial(0x04C11DB7));
            Assert.AreEqual(0x8F6E37A0, Crc32.ToReversedReciprocalPolynomial(0x1EDC6F41));
            Assert.AreEqual(0xBA0DC66B, Crc32.ToReversedReciprocalPolynomial(0x741B8CD7));
            Assert.AreEqual(0x992C1A4C, Crc32.ToReversedReciprocalPolynomial(0x32583499));
            Assert.AreEqual(0xC0A0A0D5, Crc32.ToReversedReciprocalPolynomial(0x814141AB));

            Assert.AreEqual(0xAD0424F3, Crc32.ToReversedReciprocalPolynomial(0x5A0849E7U));  // 4294967263 @ HD=3
            Assert.AreEqual(0xC9D204F5, Crc32.ToReversedReciprocalPolynomial(0x93A409EBU));  // 2147483615 @ HD=4
            Assert.AreEqual(0xD419CC15, Crc32.ToReversedReciprocalPolynomial(0xA833982BU));  // 65505 @ HD=5
            Assert.AreEqual(0x9960034C, Crc32.ToReversedReciprocalPolynomial(0x32C00699U));  // 32738 @ HD=6
            Assert.AreEqual(0xF8C9140A, Crc32.ToReversedReciprocalPolynomial(0xF1922815U));  // 992 @ HD=7
            Assert.AreEqual(0xF8C9140A, Crc32.ToReversedReciprocalPolynomial(0xF1922815U));  // 992 @ HD=8
            Assert.AreEqual(0x9D7F97D6, Crc32.ToReversedReciprocalPolynomial(0x3AFF2FADU));  // 223 @ HD=9
            Assert.AreEqual(0xB49C1C96, Crc32.ToReversedReciprocalPolynomial(0x6938392DU));  // 100 @ HD=10
            Assert.AreEqual(0x85B9561D, Crc32.ToReversedReciprocalPolynomial(0x0B72AC3BU));  // 38 @ HD=11
            Assert.AreEqual(0x950EBFAE, Crc32.ToReversedReciprocalPolynomial(0x2A1D7F5DU));  // 36 @ HD=12
            Assert.AreEqual(0x93B39B1B, Crc32.ToReversedReciprocalPolynomial(0x27673637U));  // 20 @ HD=13
            Assert.AreEqual(0xA094AFB5, Crc32.ToReversedReciprocalPolynomial(0x41295F6BU));  // 19 @ HD=14
            Assert.AreEqual(0xA2572962, Crc32.ToReversedReciprocalPolynomial(0x44AE52C5U));  // 15 @ HD=15
            Assert.AreEqual(0xE89061DB, Crc32.ToReversedReciprocalPolynomial(0xD120C3B7U));  // 13 @ HD=16
            Assert.AreEqual(0xA86bE4DB, Crc32.ToReversedReciprocalPolynomial(0x50D7C9B7U));  // 7 @ HD=17
            Assert.AreEqual(0x973AFB51, Crc32.ToReversedReciprocalPolynomial(0x2E75F6A3U));  // 5 @ HD=18
        }
    }

    [TestMethod]
    public void Crc32_FromReversedReciprocal() {
        unchecked {
            Assert.AreEqual(0x04C11DB7U, Crc32.FromReversedReciprocalPolynomial(0x82608EDB));
            Assert.AreEqual(0x1EDC6F41U, Crc32.FromReversedReciprocalPolynomial(0x8F6E37A0));
            Assert.AreEqual(0x741B8CD7U, Crc32.FromReversedReciprocalPolynomial(0xBA0DC66B));
            Assert.AreEqual(0x32583499U, Crc32.FromReversedReciprocalPolynomial(0x992C1A4C));
            Assert.AreEqual(0x814141ABU, Crc32.FromReversedReciprocalPolynomial(0xC0A0A0D5));

            Assert.AreEqual(0x5A0849E7U, Crc32.FromReversedReciprocalPolynomial(0xAD0424F3));  // 4294967263 @ HD=3
            Assert.AreEqual(0x93A409EBU, Crc32.FromReversedReciprocalPolynomial(0xC9D204F5));  // 2147483615 @ HD=4
            Assert.AreEqual(0xA833982BU, Crc32.FromReversedReciprocalPolynomial(0xD419CC15));  // 65505 @ HD=5
            Assert.AreEqual(0x32C00699U, Crc32.FromReversedReciprocalPolynomial(0x9960034C));  // 32738 @ HD=6
            Assert.AreEqual(0xF1922815U, Crc32.FromReversedReciprocalPolynomial(0xF8C9140A));  // 992 @ HD=7
            Assert.AreEqual(0xF1922815U, Crc32.FromReversedReciprocalPolynomial(0xF8C9140A));  // 992 @ HD=8
            Assert.AreEqual(0x3AFF2FADU, Crc32.FromReversedReciprocalPolynomial(0x9D7F97D6));  // 223 @ HD=9
            Assert.AreEqual(0x6938392DU, Crc32.FromReversedReciprocalPolynomial(0xB49C1C96));  // 100 @ HD=10
            Assert.AreEqual(0x0B72AC3BU, Crc32.FromReversedReciprocalPolynomial(0x85B9561D));  // 38 @ HD=11
            Assert.AreEqual(0x2A1D7F5DU, Crc32.FromReversedReciprocalPolynomial(0x950EBFAE));  // 36 @ HD=12
            Assert.AreEqual(0x27673637U, Crc32.FromReversedReciprocalPolynomial(0x93B39B1B));  // 20 @ HD=13
            Assert.AreEqual(0x41295F6BU, Crc32.FromReversedReciprocalPolynomial(0xA094AFB5));  // 19 @ HD=14
            Assert.AreEqual(0x44AE52C5U, Crc32.FromReversedReciprocalPolynomial(0xA2572962));  // 15 @ HD=15
            Assert.AreEqual(0xD120C3B7U, Crc32.FromReversedReciprocalPolynomial(0xE89061DB));  // 13 @ HD=16
            Assert.AreEqual(0x50D7C9B7U, Crc32.FromReversedReciprocalPolynomial(0xA86bE4DB));  // 7 @ HD=17
            Assert.AreEqual(0x2E75F6A3U, Crc32.FromReversedReciprocalPolynomial(0x973AFB51));  // 5 @ HD=18
        }
    }

}
