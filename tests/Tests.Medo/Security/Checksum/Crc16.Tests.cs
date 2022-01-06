using System;
using System.Text;
using Xunit;
using Medo.Security.Checksum;

namespace Tests.Medo.Security.Checksum {
    public class Crc16Tests {

        [Fact(DisplayName = "Crc16: Default")]
        public void Default() {
            string expected = "0x178C";
            var crc = new Crc16();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }


        [Fact(DisplayName = "Crc16: ARC")]
        public void Arc() {
            string expected = "0x178C";
            var crc = Crc16.GetArc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: ARC (2)")]
        public void Arc_2() {
            var crc = Crc16.GetArc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xBB3D, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: ARC / LHA (A)")]
        public void Lha_A() {
            var crc = Crc16.GetLha();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xBB3D, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: ARC / IEEE-802.3 (A)")]
        public void Ieee8023_A() {
            var crc = Crc16.GetIeee8023();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xBB3D, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: CDMA2000")]
        public void Cdma2000() {
            string expected = "0x4A2D";
            var crc = Crc16.GetCdma2000();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: CDMA2000 (2)")]
        public void Cdma2000_2() {
            var crc = Crc16.GetCdma2000();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x4C06, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: CMS")]
        public void Cms() {
            string expected = "0x2A12";
            var crc = Crc16.GetCms();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: CMS (2)")]
        public void Cms_2() {
            var crc = Crc16.GetCms();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xAEE7, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: DDS-110")]
        public void Dds110() {
            string expected = "0x242A";
            var crc = Crc16.GetDds110();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: DDS-110 (2)")]
        public void Dds110_2() {
            var crc = Crc16.GetDds110();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x9ECF, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: DECT-R")]
        public void DectR() {
            string expected = "0x55C3";
            var crc = Crc16.GetDectR();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: DECT-R (2)")]
        public void DectR_2() {
            var crc = Crc16.GetDectR();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x007E, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: DECT-X")]
        public void DectX() {
            string expected = "0x55C3";
            var crc = Crc16.GetDectR();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: DECT-X (2)")]
        public void DectX_2() {
            var crc = Crc16.GetDectX();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x007F, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: DNP")]
        public void Dnp() {
            string expected = "0xE7BC";
            var crc = Crc16.GetDnp();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: DNP (2)")]
        public void Dnp_2() {
            var crc = Crc16.GetDnp();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xEA82, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: EN-13757")]
        public void En13757() {
            string expected = "0x7458";
            var crc = Crc16.GetEn13757();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: EN-13757 (2)")]
        public void En13757_2() {
            var crc = Crc16.GetEn13757();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xC2B7, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: GENIBUS")]
        public void Genibus() {
            string expected = "0x20D1";
            var crc = Crc16.GetGenibus();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: GENIBUS (2)")]
        public void Genibus_2() {
            var crc = Crc16.GetGenibus();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xD64E, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: GENIBUS / DARC (A)")]
        public void Darc_A() {
            var crc = Crc16.GetDarc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xD64E, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: GENIBUS / EPC (A)")]
        public void Epc_A() {
            var crc = Crc16.GetEpc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xD64E, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: GENIBUS / EPC-C1G2 (A)")]
        public void EpcC1G2_A() {
            var crc = Crc16.GetEpcC1G2();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xD64E, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: GSM")]
        public void Gsm() {
            string expected = "0xA1E4";
            var crc = Crc16.GetGsm();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: GSM (2)")]
        public void Gsm_2() {
            var crc = Crc16.GetGsm();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xCE3C, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: IBM-3740")]
        public void Ibm3740() {
            string expected = "0xDF2E";
            var crc = Crc16.GetIbm3740();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: IBM-3740 (2)")]
        public void Ibm3740_2() {
            var crc = Crc16.GetIbm3740();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x29B1, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: IBM-3740 / AUTOSAR (A)")]
        public void Autosar_A() {
            var crc = Crc16.GetAutosar();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x29B1, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: IBM-3740 / CCITT-FALSE (A)")]
        public void CcittFalse_A() {
            var crc = Crc16.GetCcittFalse();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x29B1, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: IBM-SDLC")]
        public void IbmSdlc() {
            string expected = "0xCB47";
            var crc = Crc16.GetIbmSdlc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: IBM-SDLC (2)")]
        public void IbmSdlc_2() {
            var crc = Crc16.GetIbmSdlc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x906E, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: IBM-SDLC / ISO-HDLC (2)")]
        public void IsoHdld_A() {
            var crc = Crc16.GetIsoHdld();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x906E, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: IBM-SDLC / ISO-IEC-14443-3-B (2)")]
        public void IsoIec144433B_A() {
            var crc = Crc16.GetIsoIec144433B();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x906E, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: IBM-SDLC / X-25 (2)")]
        public void X25_A() {
            var crc = Crc16.GetX25();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x906E, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: I-CODE")]
        public void ICode() {
            string expected = "0xCB47";
            var crc = Crc16.GetIbmSdlc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: I-CODE (2)")]
        public void ICode_2() {
            var crc = Crc16.GetICode();
            crc.ComputeHash(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x64, 0x32 });
            Assert.Equal(0x1D0F, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: KERMIT")]
        public void Kermit() {
            string expected = "0x9839";
            var crc = Crc16.GetKermit();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: KERMIT (2)")]
        public void Kermit_2() {
            var crc = Crc16.GetKermit();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x2189, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: KERMIT / CCITT (A)")]
        public void Ccitt_A() {
            var crc = Crc16.GetCcitt();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x2189, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: KERMIT / CCITT-TRUE (A)")]
        public void CcittTrue_A() {
            var crc = Crc16.GetCcittTrue();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x2189, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: KERMIT / V-41-LSB (A)")]
        public void V41Lsb_A() {
            var crc = Crc16.GetV41Lsb();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x2189, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: LJ1200")]
        public void Lj1200() {
            string expected = "0x1507";
            var crc = Crc16.GetLj1200();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: LJ1200 (2)")]
        public void Lj1200_2() {
            var crc = Crc16.GetLj1200();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xBDF4, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: MAXIM-DOW")]
        public void MaximDow() {
            string expected = "0xE873";
            var crc = Crc16.GetMaximDow();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: MAXIM-DOW (2)")]
        public void MaximDow_2() {
            var crc = Crc16.GetMaximDow();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x44C2, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: MAXIM-DOW / MAXIM (A)")]
        public void Maxim_A() {
            var crc = Crc16.GetMaxim();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x44C2, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: MCRF4XX")]
        public void Mcrf4xx() {
            string expected = "0x34B8";
            var crc = Crc16.GetMcrf4xx();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: MCRF4XX (2)")]
        public void Mcrf4xx_2() {
            var crc = Crc16.GetMcrf4xx();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x6F91, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: MODBUS")]
        public void Modbus() {
            string expected = "0x07CC";
            var crc = Crc16.GetModbus();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: MODBUS (2)")]
        public void Modbus_2() {
            var crc = Crc16.GetModbus();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x4B37, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: NRSC-5")]
        public void Nrsc5() {
            string expected = "0x8793";
            var crc = Crc16.GetNrsc5();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: NRSC-5 (2)")]
        public void Nrsc5_2() {
            var crc = Crc16.GetNrsc5();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xA066, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: OPENSAFETY-A")]
        public void OpenSafetyA() {
            string expected = "0xCE51";
            var crc = Crc16.GetOpenSafetyA();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: OPENSAFETY-A (2)")]
        public void OpenSafetyA_2() {
            var crc = Crc16.GetOpenSafetyA();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x5D38, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: OPENSAFETY-B")]
        public void OpenSafetyB() {
            string expected = "0xDE12";
            var crc = Crc16.GetOpenSafetyB();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: OPENSAFETY-B (2)")]
        public void OpenSafetyB_2() {
            var crc = Crc16.GetOpenSafetyB();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x20FE, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: PROFIBUS")]
        public void Profibus() {
            string expected = "0xD338";
            var crc = Crc16.GetProfibus();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: PROFIBUS (2)")]
        public void Profibus_2() {
            var crc = Crc16.GetProfibus();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xA819, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: PROFIBUS / IEC-61158-2 (A)")]
        public void Iec611582_A() {
            var crc = Crc16.GetIec611582();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xA819, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: RIELLO")]
        public void Riello() {
            string expected = "0x2231";
            var crc = Crc16.GetRiello();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: RIELLO (2)")]
        public void Riello_2() {
            var crc = Crc16.GetRiello();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x63D0, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: SPI-FUJITSU")]
        public void SpiFujitsu() {
            string expected = "0x1044";
            var crc = Crc16.GetSpiFujitsu();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: SPI-FUJITSU (2)")]
        public void SpiFujitsu_2() {
            var crc = Crc16.GetSpiFujitsu();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xE5CC, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: SPI-FUJITSU / AUG-CCITT (A)")]
        public void SpiAugCcitt_A() {
            var crc = Crc16.GetSpiFujitsu();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xE5CC, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: T10-DIF")]
        public void T10Dif() {
            string expected = "0x4C2F";
            var crc = Crc16.GetT10Dif();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: T10-DIF (2)")]
        public void T10Dif_2() {
            var crc = Crc16.GetT10Dif();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xD0DB, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: TELEDISK")]
        public void Teledisk() {
            string expected = "0x4CBD";
            var crc = Crc16.GetTeledisk();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: TELEDISK (2)")]
        public void Teledisk_2() {
            var crc = Crc16.GetTeledisk();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x0FB3, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: TMS37157")]
        public void Tms37157() {
            string expected = "0x7DB6";
            var crc = Crc16.GetTms37157();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: TMS37157 (2)")]
        public void Tms37157_2() {
            var crc = Crc16.GetTms37157();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x26B1, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: UMTS")]
        public void Umts() {
            string expected = "0x281A";
            var crc = Crc16.GetUmts();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: UMTS (2)")]
        public void Umts_2() {
            var crc = Crc16.GetUmts();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xFEE8, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: UMTS / BUYPASS (A)")]
        public void Buypass_2() {
            var crc = Crc16.GetBuypass();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xFEE8, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: UMTS / VERIFONE (A)")]
        public void Verifone_2() {
            var crc = Crc16.GetVerifone();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xFEE8, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: USB")]
        public void Usb() {
            string expected = "0xF833";
            var crc = Crc16.GetUsb();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: USB (2)")]
        public void Usb_2() {
            var crc = Crc16.GetUsb();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xB4C8, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: XMODEM")]
        public void XModem() {
            string expected = "0x5E1B";
            var crc = Crc16.GetXModem();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsInt16:X4}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: XMODEM (2)")]
        public void XModem_2() {
            var crc = Crc16.GetXModem();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x31C3, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: XMODEM / ACORN (A)")]
        public void Acorn_A() {
            var crc = Crc16.GetAcorn();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x31C3, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: XMODEM / LTE (A)")]
        public void Lte_A() {
            var crc = Crc16.GetLte();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x31C3, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: XMODEM / V-41-MSB (A)")]
        public void V41Msb_A() {
            var crc = Crc16.GetV41Msb();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x31C3, (ushort)crc.HashAsInt16);
        }

        [Fact(DisplayName = "Crc16: XMODEM / ZMODEM (A)")]
        public void ZModem_A() {
            var crc = Crc16.GetZModem();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x31C3, (ushort)crc.HashAsInt16);
        }


        [Fact(DisplayName = "Crc16: Reuse same instance")]
        public void Reuse() {
            var checksum = Crc16.GetIeee8023();
            checksum.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal("178C", checksum.HashAsInt16.ToString("X4"));
        }

    }
}
