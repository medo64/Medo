using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.IO.Hashing;

namespace Tests;

[TestClass]
public class Crc16_Tests {

    [TestMethod]
    public void Crc16_GetCustom() {
        var crc = Crc16.GetCustom(unchecked(0x8005), 0x0000, true, true, 0x0000);
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xBB3D, crc.HashAsUInt16);
    }

    [TestMethod]
    public void Crc16_GetCustom_2() {
        var crc = Crc16.GetCustom(unchecked(0x8005), 0x0000, true, true, 0x0000);
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("178C", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("8C17", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }


    [TestMethod]  // ARC
    public void Crc16_Arc() {
        var crc = Crc16.GetArc();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xBB3D, crc.HashAsUInt16);
    }

    [TestMethod]  // ARC
    public void Crc16_Arc_2() {
        var crc = Crc16.GetArc();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("178C", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("8C17", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // ARC / LHA (A
    public void Crc16_Lha_A() {
        var crc = Crc16.GetLha();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xBB3D, crc.HashAsUInt16);
    }

    [TestMethod]  // ARC / IEEE-802.3 (A)
    public void Crc16_Ieee8023_A() {
        var crc = Crc16.GetIeee8023();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xBB3D, crc.HashAsUInt16);
    }

    [TestMethod]  // CDMA2000
    public void Crc16_Cdma2000() {
        var crc = Crc16.GetCdma2000();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x4C06, crc.HashAsUInt16);
    }

    [TestMethod]  // CDMA2000
    public void Crc16_Cdma2000_2() {
        var crc = Crc16.GetCdma2000();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("4A2D", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("4A2D", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // CMS
    public void Crc16_Cms() {
        var crc = Crc16.GetCms();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xAEE7, crc.HashAsUInt16);
    }

    [TestMethod]  // CMS
    public void Crc16_Cms_2() {
        var crc = Crc16.GetCms();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("2A12", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("2A12", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // DDS-110
    public void Crc16_Dds110() {
        var crc = Crc16.GetDds110();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x9ECF, crc.HashAsUInt16);
    }

    [TestMethod]  // DDS-110
    public void Crc16_Dds110_2() {
        var crc = Crc16.GetDds110();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("242A", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("242A", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // DECT-R
    public void Crc16_DectR() {
        var crc = Crc16.GetDectR();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x007E, crc.HashAsUInt16);
    }

    [TestMethod]  // DECT-R
    public void Crc16_DectR_2() {
        var crc = Crc16.GetDectR();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("55C3", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("55C3", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // DECT-X
    public void Crc16_DectX() {
        var crc = Crc16.GetDectX();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x007F, crc.HashAsUInt16);
    }

    [TestMethod]  // DECT-X
    public void Crc16_DectX_2() {
        var crc = Crc16.GetDectR();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("55C3", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("55C3", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // DNP
    public void Crc16_Dnp() {
        var crc = Crc16.GetDnp();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xEA82, crc.HashAsUInt16);
    }

    [TestMethod]  // DNP
    public void Crc16_Dnp_2() {
        var crc = Crc16.GetDnp();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("E7BC", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("BCE7", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // EN-13757
    public void Crc16_En13757() {
        var crc = Crc16.GetEn13757();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xC2B7, crc.HashAsUInt16);
    }

    [TestMethod]  // EN-13757
    public void Crc16_En13757_2() {
        var crc = Crc16.GetEn13757();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("7458", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("7458", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // GENIBUS
    public void Crc16_Genibus() {
        var crc = Crc16.GetGenibus();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xD64E, crc.HashAsUInt16);
    }

    [TestMethod]  // GENIBUS
    public void Crc16_Genibus_2() {
        var crc = Crc16.GetGenibus();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("20D1", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("20D1", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // GENIBUS / DARC (A)
    public void Crc16_Darc_A() {
        var crc = Crc16.GetDarc();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xD64E, crc.HashAsUInt16);
    }

    [TestMethod]  // GENIBUS / EPC (A)
    public void Crc16_Epc_A() {
        var crc = Crc16.GetEpc();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xD64E, crc.HashAsUInt16);
    }

    [TestMethod]  // GENIBUS / EPC-C1G2 (A)
    public void Crc16_EpcC1G2_A() {
        var crc = Crc16.GetEpcC1G2();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xD64E, crc.HashAsUInt16);
    }

    [TestMethod]  // GSM
    public void Crc16_Gsm() {
        var crc = Crc16.GetGsm();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xCE3C, crc.HashAsUInt16);
    }

    [TestMethod]  // GSM
    public void Crc16_Gsm_2() {
        var crc = Crc16.GetGsm();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("A1E4", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("A1E4", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // IBM-3740
    public void Crc16_Ibm3740() {
        var crc = Crc16.GetIbm3740();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x29B1, crc.HashAsUInt16);
    }

    [TestMethod]  // IBM-3740
    public void Crc16_Ibm3740_2() {
        var crc = Crc16.GetIbm3740();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("DF2E", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("DF2E", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // IBM-3740 / AUTOSAR (A)
    public void Crc16_Autosar_A() {
        var crc = Crc16.GetAutosar();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x29B1, crc.HashAsUInt16);
    }

    [TestMethod]  // IBM-3740 / CCITT-FALSE (A)
    public void Crc16_CcittFalse_A() {
        var crc = Crc16.GetCcittFalse();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x29B1, crc.HashAsUInt16);
    }

    [TestMethod]  // IBM-SDLC
    public void Crc16_IbmSdlc() {
        var crc = Crc16.GetIbmSdlc();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x906E, crc.HashAsUInt16);
    }

    [TestMethod]  // IBM-SDLC
    public void Crc16_IbmSdlc_2() {
        var crc = Crc16.GetIbmSdlc();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("CB47", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("47CB", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // IBM-SDLC / ISO-HDLC
    public void Crc16_IsoHdld_A() {
        var crc = Crc16.GetIsoHdld();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x906E, crc.HashAsUInt16);
    }

    [TestMethod]  // IBM-SDLC / ISO-IEC-14443-3-B
    public void Crc16_IsoIec144433B_A() {
        var crc = Crc16.GetIsoIec144433B();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x906E, crc.HashAsUInt16);
    }

    [TestMethod]  // IBM-SDLC / X-25
    public void Crc16_X25_A() {
        var crc = Crc16.GetX25();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x906E, crc.HashAsUInt16);
    }

    [TestMethod]  // I-CODE
    public void Crc16_ICode() {
        var crc = Crc16.GetICode();
        crc.Append(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x64, 0x32 });
        Assert.AreEqual(0x1D0F, crc.HashAsUInt16);
    }

    [TestMethod]  // I-CODE
    public void Crc16_ICode_2() {
        var crc = Crc16.GetIbmSdlc();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("CB47", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("47CB", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // KERMIT
    public void Crc16_Kermit() {
        var crc = Crc16.GetKermit();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x2189, crc.HashAsUInt16);
    }

    [TestMethod]  // KERMIT
    public void Crc16_Kermit_2() {
        var crc = Crc16.GetKermit();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("9839", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("3998", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // KERMIT / CCITT (A)
    public void Crc16_Ccitt_A() {
        var crc = Crc16.GetCcitt();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x2189, crc.HashAsUInt16);
    }

    [TestMethod]  // KERMIT / CCITT-TRUE (A)
    public void Crc16_CcittTrue_A() {
        var crc = Crc16.GetCcittTrue();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x2189, crc.HashAsUInt16);
    }

    [TestMethod]  // KERMIT / V-41-LSB (A)
    public void Crc16_V41Lsb_A() {
        var crc = Crc16.GetV41Lsb();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x2189, crc.HashAsUInt16);
    }

    [TestMethod]
    public void Crc16_Lj1200() {
        var crc = Crc16.GetLj1200();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xBDF4, crc.HashAsUInt16);
    }

    [TestMethod]  // LJ1200
    public void Crc16_Lj1200_2() {
        var crc = Crc16.GetLj1200();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("1507", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("1507", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // MAXIM-DOW
    public void Crc16_MaximDow() {
        var crc = Crc16.GetMaximDow();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x44C2, crc.HashAsUInt16);
    }

    [TestMethod]  // LJ1200
    public void Crc16_MaximDow_2() {
        var crc = Crc16.GetMaximDow();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("E873", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("73E8", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // MAXIM-DOW / MAXIM (A)
    public void Crc16_Maxim_A() {
        var crc = Crc16.GetMaxim();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x44C2, crc.HashAsUInt16);
    }

    [TestMethod]  // MCRF4XX
    public void Crc16_Mcrf4xx() {
        var crc = Crc16.GetMcrf4xx();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x6F91, crc.HashAsUInt16);
    }

    [TestMethod]  // MCRF4XX
    public void Crc16_Mcrf4xx_2() {
        var crc = Crc16.GetMcrf4xx();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("34B8", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("B834", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // MODBUS
    public void Crc16_Modbus() {
        var crc = Crc16.GetModbus();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x4B37, crc.HashAsUInt16);
    }

    [TestMethod]  // MODBUS
    public void Crc16_Modbus_2() {
        var crc = Crc16.GetModbus();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("07CC", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("CC07", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // NRSC-5
    public void Crc16_Nrsc5() {
        var crc = Crc16.GetNrsc5();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA066, crc.HashAsUInt16);
    }

    [TestMethod]  // NRSC-5
    public void Crc16_Nrsc5_2() {
        var crc = Crc16.GetNrsc5();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("8793", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("9387", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // OPENSAFETY-A
    public void Crc16_OpenSafetyA() {
        var crc = Crc16.GetOpenSafetyA();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x5D38, crc.HashAsUInt16);
    }

    [TestMethod]  // OPENSAFETY-A
    public void Crc16_OpenSafetyA_2() {
        var crc = Crc16.GetOpenSafetyA();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("CE51", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("CE51", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // OPENSAFETY-B
    public void Crc16_OpenSafetyB() {
        var crc = Crc16.GetOpenSafetyB();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x20FE, crc.HashAsUInt16);
    }

    [TestMethod]  // OPENSAFETY-B
    public void Crc16_OpenSafetyB_2() {
        var crc = Crc16.GetOpenSafetyB();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("DE12", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("DE12", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // PROFIBUS
    public void Crc16_Profibus() {
        var crc = Crc16.GetProfibus();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA819, crc.HashAsUInt16);
    }

    [TestMethod]  // PROFIBUS
    public void Crc16_Profibus_2() {
        var crc = Crc16.GetProfibus();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("D338", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("D338", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // PROFIBUS / IEC-61158-2 (A)
    public void Crc16_Iec611582_A() {
        var crc = Crc16.GetIec611582();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA819, crc.HashAsUInt16);
    }

    [TestMethod]  // RIELLO
    public void Crc16_Riello() {
        var crc = Crc16.GetRiello();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x63D0, crc.HashAsUInt16);
    }

    [TestMethod]  // RIELLO
    public void Crc16_Riello_2() {
        var crc = Crc16.GetRiello();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("2231", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("3122", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // SPI-FUJITSU
    public void Crc16_SpiFujitsu() {
        var crc = Crc16.GetSpiFujitsu();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xE5CC, crc.HashAsUInt16);
    }

    [TestMethod]  // SPI-FUJITSU
    public void Crc16_SpiFujitsu_2() {
        var crc = Crc16.GetSpiFujitsu();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("1044", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("1044", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // SPI-FUJITSU / AUG-CCITT (A)
    public void Crc16_SpiAugCcitt_A() {
        var crc = Crc16.GetSpiFujitsu();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xE5CC, crc.HashAsUInt16);
    }

    [TestMethod]  // T10-DIF
    public void Crc16_T10Dif() {
        var crc = Crc16.GetT10Dif();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xD0DB, crc.HashAsUInt16);
    }

    [TestMethod]  // T10-DIF
    public void Crc16_T10Dif_2() {
        var crc = Crc16.GetT10Dif();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("4C2F", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("4C2F", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // TELEDISK
    public void Crc16_Teledisk() {
        var crc = Crc16.GetTeledisk();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x0FB3, crc.HashAsUInt16);
    }

    [TestMethod]  // TELEDISK
    public void Crc16_Teledisk_2() {
        var crc = Crc16.GetTeledisk();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("4CBD", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("4CBD", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // TMS37157
    public void Crc16_Tms37157() {
        var crc = Crc16.GetTms37157();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x26B1, crc.HashAsUInt16);
    }

    [TestMethod]  // TMS37157
    public void Crc16_Tms37157_2() {
        var crc = Crc16.GetTms37157();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("7DB6", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("B67D", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // UMTS
    public void Crc16_Umts() {
        var crc = Crc16.GetUmts();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xFEE8, crc.HashAsUInt16);
    }

    [TestMethod]  // UMTS
    public void Crc16_Umts_2() {
        var crc = Crc16.GetUmts();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("281A", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("281A", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // UMTS / BUYPASS (A)
    public void Crc16_Buypass_2() {
        var crc = Crc16.GetBuypass();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xFEE8, crc.HashAsUInt16);
    }

    [TestMethod]  // UMTS / VERIFONE (A)
    public void Crc16_Verifone_2() {
        var crc = Crc16.GetVerifone();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xFEE8, crc.HashAsUInt16);
    }

    [TestMethod]  //  USB
    public void Crc16_Usb() {
        var crc = Crc16.GetUsb();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xB4C8, crc.HashAsUInt16);
    }

    [TestMethod]  // USB
    public void Crc16_Usb_2() {
        var crc = Crc16.GetUsb();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("F833", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("33F8", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // XMODEM
    public void Crc16_XModem() {
        var crc = Crc16.GetXModem();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x31C3, crc.HashAsUInt16);
    }

    [TestMethod]  // XMODEM
    public void Crc16_XModem_2() {
        var crc = Crc16.GetXModem();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("5E1B", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("5E1B", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // XMODEM / ACORN (A)
    public void Crc16_Acorn_A() {
        var crc = Crc16.GetAcorn();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x31C3, crc.HashAsUInt16);
    }

    [TestMethod]  // XMODEM / LTE (A)
    public void Crc16_Lte_A() {
        var crc = Crc16.GetLte();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x31C3, crc.HashAsUInt16);
    }

    [TestMethod]  // XMODEM / V-41-MSB (A)
    public void Crc16_V41Msb_A() {
        var crc = Crc16.GetV41Msb();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x31C3, crc.HashAsUInt16);
    }

    [TestMethod]  // XMODEM / ZMODEM (A)
    public void Crc16_ZModem_A() {
        var crc = Crc16.GetZModem();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x31C3, crc.HashAsUInt16);
    }

    [TestMethod]  // CRC-16F/4.2
    public void Crc16_Crc16F42() {
        var crc = Crc16.GetCustom(unchecked(0xA2EB), 0x0000, false, false, 0x0000);
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x5C1F, crc.HashAsUInt16);
    }

    [TestMethod]  // CRC-16F/4.2
    public void Crc16_Crc16F42_2() {
        var crc = Crc16.GetCustom(unchecked(0xA2EB), 0x0000, false, false, 0x0000);
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("3CE5", $"{crc.HashAsUInt16:X4}");
        Assert.AreEqual("3CE5", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // CRC-16F/4.2
    public void Crc16_Crc16F42_3() {
        var crc = Crc16.GetCustom(unchecked(0xA2EB), 0x0000, false, false, 0x0000);
        crc.Append(Encoding.ASCII.GetBytes(""));
        Assert.AreEqual(0x0000, crc.HashAsUInt16);
    }

    [TestMethod]  // CRC-16F/4.2
    public void Crc16_Crc16F42_4() {
        var crc = Crc16.GetCustom(unchecked(0xA2EB), 0x0000, false, false, 0x0000);
        crc.Append(Encoding.ASCII.GetBytes("1"));
        Assert.AreEqual(0x931A, crc.HashAsUInt16);
    }

    [TestMethod]  // CRC-16F/4.2
    public void Crc16_Crc16F42_5() {
        var crc = Crc16.GetCustom(unchecked(0xA2EB), 0x0000, false, false, 0x0000);
        crc.Append(Encoding.ASCII.GetBytes("12"));
        Assert.AreEqual(0x1CCD, crc.HashAsUInt16);
    }

    [TestMethod]  // CRC-16F/4.2
    public void Crc16_Crc16F42_6() {
        var crc = Crc16.GetCustom(unchecked(0xA2EB), 0x0000, false, false, 0x0000);
        crc.Append(Encoding.ASCII.GetBytes("123"));
        Assert.AreEqual(0x1C3B, crc.HashAsUInt16);
    }

    [TestMethod]  // CRC-16F/4.2
    public void Crc16_Crc16F42_7() {
        var crc = Crc16.GetCustom(unchecked(0xA2EB), 0x0000, false, false, 0x0000);
        crc.Append(Encoding.ASCII.GetBytes("1234"));
        Assert.AreEqual(0xC37C, crc.HashAsUInt16);
    }

    [TestMethod]  // CRC-16F/4.2
    public void Crc16_Crc16F42_8() {
        var crc = Crc16.GetCustom(unchecked(0xA2EB), 0x0000, false, false, 0x0000);
        crc.Append(Encoding.ASCII.GetBytes("12356"));
        Assert.AreEqual(0xEC54, crc.HashAsUInt16);
    }

    [TestMethod]  // CRC-16F/4.2
    public void Crc16_Crc16F42_9() {
        var crc = Crc16.GetCustom(unchecked(0xA2EB), 0x0000, false, false, 0x0000);
        crc.Append(Encoding.ASCII.GetBytes("1234567"));
        Assert.AreEqual(0x8925, crc.HashAsUInt16);
    }


    [TestMethod]
    public void Crc16_Reuse() {
        var checksum = Crc16.GetIeee8023();
        checksum.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("8C17", BitConverter.ToString(checksum.GetHashAndReset()).Replace("-", ""));
        checksum.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("8C17", BitConverter.ToString(checksum.GetCurrentHash()).Replace("-", ""));
    }


    [TestMethod]
    public void Crc16_ToReversedReciprocal() {
        unchecked {
            Assert.AreEqual(0x978A, Crc16.ToReversedReciprocalPolynomial(0x2F15));
            Assert.AreEqual(0xD015, Crc16.ToReversedReciprocalPolynomial(0xA02B));
            Assert.AreEqual(0x8810, Crc16.ToReversedReciprocalPolynomial(0x1021));
            Assert.AreEqual(0xE433, Crc16.ToReversedReciprocalPolynomial(0xC867));
            Assert.AreEqual(0x82C4, Crc16.ToReversedReciprocalPolynomial(0x0589));
            Assert.AreEqual(0xC5DB, Crc16.ToReversedReciprocalPolynomial(0x8BB7));
            Assert.AreEqual(0x9EB2, Crc16.ToReversedReciprocalPolynomial(0x3D65));
            Assert.AreEqual(0xC002, Crc16.ToReversedReciprocalPolynomial(0x8005));
            Assert.AreEqual(0xAC9A, Crc16.ToReversedReciprocalPolynomial(0x5935));
            Assert.AreEqual(0xBAAD, Crc16.ToReversedReciprocalPolynomial(0x755B));
            Assert.AreEqual(0x8EE7, Crc16.ToReversedReciprocalPolynomial(0x1DCF));
        }
    }

    [TestMethod]
    public void Crc16_FromReversedReciprocal() {
        unchecked {
            Assert.AreEqual(0x2F15, Crc16.FromReversedReciprocalPolynomial(0x978A));
            Assert.AreEqual(0xA02B, Crc16.FromReversedReciprocalPolynomial(0xD015));
            Assert.AreEqual(0x1021, Crc16.FromReversedReciprocalPolynomial(0x8810));
            Assert.AreEqual(0xC867, Crc16.FromReversedReciprocalPolynomial(0xE433));
            Assert.AreEqual(0x0589, Crc16.FromReversedReciprocalPolynomial(0x82C4));
            Assert.AreEqual(0x8BB7, Crc16.FromReversedReciprocalPolynomial(0xC5DB));
            Assert.AreEqual(0x3D65, Crc16.FromReversedReciprocalPolynomial(0x9EB2));  // 135 @ HD=6
            Assert.AreEqual(0x8005, Crc16.FromReversedReciprocalPolynomial(0xC002));
            Assert.AreEqual(0x5935, Crc16.FromReversedReciprocalPolynomial(0xAC9A));  // 241 @ HD=5
            Assert.AreEqual(0x755B, Crc16.FromReversedReciprocalPolynomial(0xBAAD));
            Assert.AreEqual(0x1DCF, Crc16.FromReversedReciprocalPolynomial(0x8EE7));

            Assert.AreEqual(0xA2EB, Crc16.FromReversedReciprocalPolynomial(0xD175));  // 32751 @ HD=4
            Assert.AreEqual(0x1B2B, Crc16.FromReversedReciprocalPolynomial(0x8D95));  // 65519 @ HD=3
            Assert.AreEqual(0x2D17, Crc16.FromReversedReciprocalPolynomial(0x968B));  // 19 @ HD=7
            Assert.AreEqual(0x1FB7, Crc16.FromReversedReciprocalPolynomial(0x8FDB));  // 15 @ HD=8
            Assert.AreEqual(0xD25F, Crc16.FromReversedReciprocalPolynomial(0xE92F));  // 6 @ HD=9
            Assert.AreEqual(0xDA5F, Crc16.FromReversedReciprocalPolynomial(0xED2F));  // 5 @ HD=10

            Assert.AreEqual(0xA2EB, Crc16.FromReversedReciprocalPolynomial(0xD175));
        }
    }

}
