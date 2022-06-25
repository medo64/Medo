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


        [Fact(DisplayName = "Crc32: Convert polynomial to reversed reciprocal")]
        public void ToReversedReciprocal() {
            unchecked {
                Assert.Equal((int)0x82608EDB, Crc32.ToReversedReciprocalPolynomial((int)0x04C11DB7));
                Assert.Equal((int)0x8F6E37A0, Crc32.ToReversedReciprocalPolynomial((int)0x1EDC6F41));
                Assert.Equal((int)0xBA0DC66B, Crc32.ToReversedReciprocalPolynomial((int)0x741B8CD7));
                Assert.Equal((int)0x992C1A4C, Crc32.ToReversedReciprocalPolynomial((int)0x32583499));
                Assert.Equal((int)0xC0A0A0D5, Crc32.ToReversedReciprocalPolynomial((int)0x814141AB));
            }
        }

        [Fact(DisplayName = "Crc32: Convert from reversed reciprocal polynomial")]
        public void FromReversedReciprocal() {
            unchecked {
                Assert.Equal((int)0x04C11DB7, Crc32.FromReversedReciprocalPolynomial((int)0x82608EDB));
                Assert.Equal((int)0x1EDC6F41, Crc32.FromReversedReciprocalPolynomial((int)0x8F6E37A0));
                Assert.Equal((int)0x741B8CD7, Crc32.FromReversedReciprocalPolynomial((int)0xBA0DC66B));
                Assert.Equal((int)0x32583499, Crc32.FromReversedReciprocalPolynomial((int)0x992C1A4C));
                Assert.Equal((int)0x814141AB, Crc32.FromReversedReciprocalPolynomial((int)0xC0A0A0D5));

                Assert.Equal((int)0x5A0849E7, Crc32.FromReversedReciprocalPolynomial((int)0xAD0424F3));  // 4294967263 @ HD=3
                Assert.Equal((int)0x93A409EB, Crc32.FromReversedReciprocalPolynomial((int)0xC9D204F5));  // 2147483615 @ HD=4
                Assert.Equal((int)0xA833982B, Crc32.FromReversedReciprocalPolynomial((int)0xD419CC15));  // 65505 @ HD=5
                Assert.Equal((int)0x32C00699, Crc32.FromReversedReciprocalPolynomial((int)0x9960034C));  // 32738 @ HD=6
                Assert.Equal((int)0xF1922815, Crc32.FromReversedReciprocalPolynomial((int)0xF8C9140A));  // 992 @ HD=7
                Assert.Equal((int)0xF1922815, Crc32.FromReversedReciprocalPolynomial((int)0xF8C9140A));  // 992 @ HD=8
                Assert.Equal((int)0x3AFF2FAD, Crc32.FromReversedReciprocalPolynomial((int)0x9D7F97D6));  // 223 @ HD=9
                Assert.Equal((int)0x6938392D, Crc32.FromReversedReciprocalPolynomial((int)0xB49C1C96));  // 100 @ HD=10
                Assert.Equal((int)0xB72AC3B, Crc32.FromReversedReciprocalPolynomial((int)0x85B9561D));  // 38 @ HD=11
                Assert.Equal((int)0x2A1D7F5D, Crc32.FromReversedReciprocalPolynomial((int)0x950EBFAE));  // 36 @ HD=12
                Assert.Equal((int)0x27673637, Crc32.FromReversedReciprocalPolynomial((int)0x93B39B1B));  // 20 @ HD=13
                Assert.Equal((int)0x41295F6B, Crc32.FromReversedReciprocalPolynomial((int)0xA094AFB5));  // 19 @ HD=14
                Assert.Equal((int)0x44AE52C5, Crc32.FromReversedReciprocalPolynomial((int)0xA2572962));  // 15 @ HD=15
                Assert.Equal((int)0xD120C3B7, Crc32.FromReversedReciprocalPolynomial((int)0xE89061DB));  // 13 @ HD=16
                Assert.Equal((int)0x50D7C9B7, Crc32.FromReversedReciprocalPolynomial((int)0xA86bE4DB));  // 7 @ HD=17
                Assert.Equal((int)0x2E75F6A3, Crc32.FromReversedReciprocalPolynomial((int)0x973AFB51));  // 5 @ HD=18
            }
        }

        [Fact(DisplayName = "Crc32: Convert polynomial to reversed reciprocal (long)")]
        public void ToReversedReciprocal2() {
            unchecked {
                Assert.Equal((int)0x82608EDB, Crc32.ToReversedReciprocalPolynomial(0x04C11DB7L));
                Assert.Equal((int)0x8F6E37A0, Crc32.ToReversedReciprocalPolynomial(0x1EDC6F41L));
                Assert.Equal((int)0xBA0DC66B, Crc32.ToReversedReciprocalPolynomial(0x741B8CD7L));
                Assert.Equal((int)0x992C1A4C, Crc32.ToReversedReciprocalPolynomial(0x32583499L));
                Assert.Equal((int)0xC0A0A0D5, Crc32.ToReversedReciprocalPolynomial(0x814141ABL));
            }
        }

        [Fact(DisplayName = "Crc32: Convert from reversed reciprocal polynomial (long)")]
        public void FromReversedReciprocal2() {
            unchecked {
                Assert.Equal((int)0x04C11DB7, Crc32.FromReversedReciprocalPolynomial(0x82608EDBL));
                Assert.Equal((int)0x1EDC6F41, Crc32.FromReversedReciprocalPolynomial(0x8F6E37A0L));
                Assert.Equal((int)0x741B8CD7, Crc32.FromReversedReciprocalPolynomial(0xBA0DC66BL));
                Assert.Equal((int)0x32583499, Crc32.FromReversedReciprocalPolynomial(0x992C1A4CL));
                Assert.Equal((int)0x814141AB, Crc32.FromReversedReciprocalPolynomial(0xC0A0A0D5L));
            }
        }

    }
}
