using System;
using System.Text;
using Xunit;
using Medo.Security.Checksum;

namespace Tests.Medo.Security.Checksum {
    public class Crc32Tests {

        [Fact(DisplayName = "Crc32: Custom")]
        public void Custom() {
            string expected = "0x1A657BE2";
            var crc = Crc32.GetCustom(0x04C11DB7, unchecked((int)0xFFFFFFFF), true, true, unchecked((int)0xFFFFFFFF));
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt32:X8}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }


        [Fact(DisplayName = "Crc32: AIXM")]
        public void Aixm() {
            string expected = "0x06D88232";
            var crc = Crc32.GetAixm();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt32:X8}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: AIXM (2)")]
        public void Aixm_2() {
            var crc = Crc32.GetAixm();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0x3010BF7F, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: AUTOSAR")]
        public void Autosar() {
            string expected = "0x510EF1D8";
            var crc = Crc32.GetAutosar();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt32:X8}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: AUTOSAR (2)")]
        public void Autosar_2() {
            var crc = Crc32.GetAutosar();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0x1697D06A, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: BASE91-D")]
        public void Base91D() {
            string expected = "0x2B61E75F";
            var crc = Crc32.GetBase91D();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt32:X8}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: BASE91-D (2)")]
        public void Base91D_2() {
            var crc = Crc32.GetBase91D();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0x87315576, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: BZIP2")]
        public void BZip2() {
            string expected = "0xBB7A12E7";
            var crc = Crc32.GetBZip2();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt32:X8}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: BZIP2 (2)")]
        public void BZip2_2() {
            var crc = Crc32.GetBZip2();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xFC891918, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: BZIP2 / AAL5 (A)")]
        public void Aal5_A() {
            var crc = Crc32.GetAal5();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xFC891918, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: BZIP2 / DECT-B (A)")]
        public void DectB_A() {
            var crc = Crc32.GetDectB();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xFC891918, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: CD-ROM-EDC")]
        public void CdromEdc() {
            string expected = "0x0006D785";
            var crc = Crc32.GetCdromEdc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt32:X8}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: CD-ROM-EDC (2)")]
        public void CdromEdc_2() {
            var crc = Crc32.GetCdromEdc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0x6EC2EDC4, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: CKSUM")]
        public void Cksum() {
            string expected = "0xEFC8804E";
            var crc = Crc32.GetCksum();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt32:X8}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: CKSUM (2)")]
        public void Cksum_2() {
            var crc = Crc32.GetCksum();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0x765E7680, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: CKSUM / POSIX (A)")]
        public void Posix_A() {
            var crc = Crc32.GetPosix();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0x765E7680, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: ISCSI")]
        public void IScsi() {
            string expected = "0x1F9A516E";
            var crc = Crc32.GetIScsi();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt32:X8}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: ISCSI (2)")]
        public void IScsi_2() {
            var crc = Crc32.GetIScsi();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xE3069283, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: ISCSI / BASE91-C (A)")]
        public void Base91C_A() {
            var crc = Crc32.GetBase91C();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xE3069283, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: ISCSI / CASTAGNOLI (A)")]
        public void Castagnoli_A() {
            var crc = Crc32.GetCastagnoli();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xE3069283, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: ISCSI / INTERLAKEN (A)")]
        public void Interlaken_A() {
            var crc = Crc32.GetInterlaken();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xE3069283, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: ISO-HDLC")]
        public void IsoHdlc() {
            string expected = "0x1A657BE2";
            var crc = Crc32.GetIsoHdlc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt32:X8}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: ISO-HDLC (2)")]
        public void IsoHdlc_2() {
            var crc = Crc32.GetIsoHdlc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xCBF43926, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: ISO-HDLC / ADCCP (A)")]
        public void Adccp_A() {
            var crc = Crc32.GetAdccp();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xCBF43926, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: ISO-HDLC / IEEE (A)")]
        public void Ieee_A() {
            var crc = Crc32.GetIeee();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xCBF43926, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: ISO-HDLC / IEEE-802.3 (A)")]
        public void Ieee8023_A() {
            var crc = Crc32.GetIeee8023();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xCBF43926, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: ISO-HDLC / V-42 (A)")]
        public void V42_A() {
            var crc = Crc32.GetV42();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xCBF43926, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: ISO-HDLC / XZ (A)")]
        public void XZ_A() {
            var crc = Crc32.GetXZ();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xCBF43926, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: ISO-HDLC / PKZIP (A)")]
        public void PkZip_A() {
            var crc = Crc32.GetPkZip();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xCBF43926, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: JAMCRC")]
        public void JamCrc() {
            string expected = "0xE59A841D";
            var crc = Crc32.GetJamCrc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt32:X8}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: JAMCRC (2)")]
        public void JamCrc_2() {
            var crc = Crc32.GetJamCrc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0x340BC6D9, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: JAMCRC / JAM (A)")]
        public void Jam_A() {
            var crc = Crc32.GetJamCrc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0x340BC6D9, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: MPEG-2")]
        public void Mpeg2() {
            string expected = "0x4485ED18";
            var crc = Crc32.GetMpeg2();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt32:X8}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: MPEG-2 (2)")]
        public void Mpeg2_2() {
            var crc = Crc32.GetMpeg2();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0x0376E6E7, (uint)crc.HashAsInt32);
        }

        [Fact(DisplayName = "Crc32: XFER")]
        public void Xfer() {
            string expected = "0x3A9C355C";
            var crc = Crc32.GetXfer();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt32:X8}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: XFER (2)")]
        public void Xfef_2() {
            var crc = Crc32.GetXfer();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal((uint)0xBD0BE338, (uint)crc.HashAsInt32);
        }


        [Fact(DisplayName = "Crc32: Reuse the instance")]
        public void Reuse() {
            var checksum = Crc32.GetIeee();
            checksum.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal("1A657BE2", checksum.HashAsInt32.ToString("X8"));
            checksum.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal("1A657BE2", checksum.HashAsInt32.ToString("X8"));
        }

    }
}
