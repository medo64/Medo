using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Security.Checksum;

namespace Tests;

[TestClass]
public class Crc16_Tests {

    [TestMethod]
    public void Crc16_GetCustom() {
        string expected = "0x178C";
        var crc = Crc16.GetCustom(unchecked((short)0x8005), 0x0000, true, true, 0x0000);
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }


    [TestMethod]
    public void Crc16_Arc() {
        string expected = "0x178C";
        var crc = Crc16.GetArc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Arc_2() {
        var crc = Crc16.GetArc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xBB3D, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Lha_A() {
        var crc = Crc16.GetLha();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xBB3D, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Ieee8023_A() {
        var crc = Crc16.GetIeee8023();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xBB3D, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Cdma2000() {
        string expected = "0x4A2D";
        var crc = Crc16.GetCdma2000();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Cdma2000_2() {
        var crc = Crc16.GetCdma2000();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x4C06, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Cms() {
        string expected = "0x2A12";
        var crc = Crc16.GetCms();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Cms_2() {
        var crc = Crc16.GetCms();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xAEE7, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Dds110() {
        string expected = "0x242A";
        var crc = Crc16.GetDds110();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Dds110_2() {
        var crc = Crc16.GetDds110();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x9ECF, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_DectR() {
        string expected = "0x55C3";
        var crc = Crc16.GetDectR();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_DectR_2() {
        var crc = Crc16.GetDectR();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x007E, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_DectX() {
        string expected = "0x55C3";
        var crc = Crc16.GetDectR();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_DectX_2() {
        var crc = Crc16.GetDectX();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x007F, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Dnp() {
        string expected = "0xE7BC";
        var crc = Crc16.GetDnp();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Dnp_2() {
        var crc = Crc16.GetDnp();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xEA82, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_En13757() {
        string expected = "0x7458";
        var crc = Crc16.GetEn13757();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_En13757_2() {
        var crc = Crc16.GetEn13757();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xC2B7, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Genibus() {
        string expected = "0x20D1";
        var crc = Crc16.GetGenibus();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Genibus_2() {
        var crc = Crc16.GetGenibus();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xD64E, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Darc_A() {
        var crc = Crc16.GetDarc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xD64E, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Epc_A() {
        var crc = Crc16.GetEpc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xD64E, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_EpcC1G2_A() {
        var crc = Crc16.GetEpcC1G2();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xD64E, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Gsm() {
        string expected = "0xA1E4";
        var crc = Crc16.GetGsm();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Gsm_2() {
        var crc = Crc16.GetGsm();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xCE3C, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Ibm3740() {
        string expected = "0xDF2E";
        var crc = Crc16.GetIbm3740();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Ibm3740_2() {
        var crc = Crc16.GetIbm3740();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x29B1, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Autosar_A() {
        var crc = Crc16.GetAutosar();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x29B1, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_CcittFalse_A() {
        var crc = Crc16.GetCcittFalse();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x29B1, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_IbmSdlc() {
        string expected = "0xCB47";
        var crc = Crc16.GetIbmSdlc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_IbmSdlc_2() {
        var crc = Crc16.GetIbmSdlc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x906E, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_IsoHdld_A() {
        var crc = Crc16.GetIsoHdld();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x906E, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_IsoIec144433B_A() {
        var crc = Crc16.GetIsoIec144433B();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x906E, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_X25_A() {
        var crc = Crc16.GetX25();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x906E, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_ICode() {
        string expected = "0xCB47";
        var crc = Crc16.GetIbmSdlc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_ICode_2() {
        var crc = Crc16.GetICode();
        crc.ComputeHash(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x64, 0x32 });
        Assert.AreEqual(0x1D0F, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Kermit() {
        string expected = "0x9839";
        var crc = Crc16.GetKermit();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Kermit_2() {
        var crc = Crc16.GetKermit();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x2189, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Ccitt_A() {
        var crc = Crc16.GetCcitt();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x2189, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_CcittTrue_A() {
        var crc = Crc16.GetCcittTrue();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x2189, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_V41Lsb_A() {
        var crc = Crc16.GetV41Lsb();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x2189, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Lj1200() {
        string expected = "0x1507";
        var crc = Crc16.GetLj1200();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Lj1200_2() {
        var crc = Crc16.GetLj1200();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xBDF4, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_MaximDow() {
        string expected = "0xE873";
        var crc = Crc16.GetMaximDow();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_MaximDow_2() {
        var crc = Crc16.GetMaximDow();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x44C2, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Maxim_A() {
        var crc = Crc16.GetMaxim();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x44C2, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Mcrf4xx() {
        string expected = "0x34B8";
        var crc = Crc16.GetMcrf4xx();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Mcrf4xx_2() {
        var crc = Crc16.GetMcrf4xx();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x6F91, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Modbus() {
        string expected = "0x07CC";
        var crc = Crc16.GetModbus();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Modbus_2() {
        var crc = Crc16.GetModbus();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x4B37, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Nrsc5() {
        string expected = "0x8793";
        var crc = Crc16.GetNrsc5();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Nrsc5_2() {
        var crc = Crc16.GetNrsc5();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA066, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_OpenSafetyA() {
        string expected = "0xCE51";
        var crc = Crc16.GetOpenSafetyA();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_OpenSafetyA_2() {
        var crc = Crc16.GetOpenSafetyA();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x5D38, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_OpenSafetyB() {
        string expected = "0xDE12";
        var crc = Crc16.GetOpenSafetyB();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_OpenSafetyB_2() {
        var crc = Crc16.GetOpenSafetyB();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x20FE, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Profibus() {
        string expected = "0xD338";
        var crc = Crc16.GetProfibus();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Profibus_2() {
        var crc = Crc16.GetProfibus();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA819, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Iec611582_A() {
        var crc = Crc16.GetIec611582();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA819, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Riello() {
        string expected = "0x2231";
        var crc = Crc16.GetRiello();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Riello_2() {
        var crc = Crc16.GetRiello();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x63D0, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_SpiFujitsu() {
        string expected = "0x1044";
        var crc = Crc16.GetSpiFujitsu();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_SpiFujitsu_2() {
        var crc = Crc16.GetSpiFujitsu();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xE5CC, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_SpiAugCcitt_A() {
        var crc = Crc16.GetSpiFujitsu();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xE5CC, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_T10Dif() {
        string expected = "0x4C2F";
        var crc = Crc16.GetT10Dif();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_T10Dif_2() {
        var crc = Crc16.GetT10Dif();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xD0DB, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Teledisk() {
        string expected = "0x4CBD";
        var crc = Crc16.GetTeledisk();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Teledisk_2() {
        var crc = Crc16.GetTeledisk();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x0FB3, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Tms37157() {
        string expected = "0x7DB6";
        var crc = Crc16.GetTms37157();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Tms37157_2() {
        var crc = Crc16.GetTms37157();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x26B1, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Umts() {
        string expected = "0x281A";
        var crc = Crc16.GetUmts();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Umts_2() {
        var crc = Crc16.GetUmts();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xFEE8, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Buypass_2() {
        var crc = Crc16.GetBuypass();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xFEE8, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Verifone_2() {
        var crc = Crc16.GetVerifone();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xFEE8, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Usb() {
        string expected = "0xF833";
        var crc = Crc16.GetUsb();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_Usb_2() {
        var crc = Crc16.GetUsb();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xB4C8, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_XModem() {
        string expected = "0x5E1B";
        var crc = Crc16.GetXModem();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsInt16:X4}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc16_XModem_2() {
        var crc = Crc16.GetXModem();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x31C3, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Acorn_A() {
        var crc = Crc16.GetAcorn();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x31C3, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_Lte_A() {
        var crc = Crc16.GetLte();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x31C3, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_V41Msb_A() {
        var crc = Crc16.GetV41Msb();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x31C3, (ushort)crc.HashAsInt16);
    }

    [TestMethod]
    public void Crc16_ZModem_A() {
        var crc = Crc16.GetZModem();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x31C3, (ushort)crc.HashAsInt16);
    }


    [TestMethod]
    public void Crc16_Reuse() {
        var checksum = Crc16.GetIeee8023();
        checksum.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("178C", checksum.HashAsInt16.ToString("X4"));
    }


    [TestMethod]
    public void Crc16_ToReversedReciprocal() {
        unchecked {
            Assert.AreEqual((short)0x978A, Crc16.ToReversedReciprocalPolynomial((short)0x2F15));
            Assert.AreEqual((short)0xD015, Crc16.ToReversedReciprocalPolynomial((short)0xA02B));
            Assert.AreEqual((short)0x8810, Crc16.ToReversedReciprocalPolynomial((short)0x1021));
            Assert.AreEqual((short)0xE433, Crc16.ToReversedReciprocalPolynomial((short)0xC867));
            Assert.AreEqual((short)0x82C4, Crc16.ToReversedReciprocalPolynomial((short)0x0589));
            Assert.AreEqual((short)0xC5DB, Crc16.ToReversedReciprocalPolynomial((short)0x8BB7));
            Assert.AreEqual((short)0x9EB2, Crc16.ToReversedReciprocalPolynomial((short)0x3D65));
            Assert.AreEqual((short)0xC002, Crc16.ToReversedReciprocalPolynomial((short)0x8005));
            Assert.AreEqual((short)0xAC9A, Crc16.ToReversedReciprocalPolynomial((short)0x5935));
            Assert.AreEqual((short)0xBAAD, Crc16.ToReversedReciprocalPolynomial((short)0x755B));
            Assert.AreEqual((short)0x8EE7, Crc16.ToReversedReciprocalPolynomial((short)0x1DCF));
        }
    }

    [TestMethod]
    public void Crc16_FromReversedReciprocal() {
        unchecked {
            Assert.AreEqual((short)0x2F15, Crc16.FromReversedReciprocalPolynomial((short)0x978A));
            Assert.AreEqual((short)0xA02B, Crc16.FromReversedReciprocalPolynomial((short)0xD015));
            Assert.AreEqual((short)0x1021, Crc16.FromReversedReciprocalPolynomial((short)0x8810));
            Assert.AreEqual((short)0xC867, Crc16.FromReversedReciprocalPolynomial((short)0xE433));
            Assert.AreEqual((short)0x0589, Crc16.FromReversedReciprocalPolynomial((short)0x82C4));
            Assert.AreEqual((short)0x8BB7, Crc16.FromReversedReciprocalPolynomial((short)0xC5DB));
            Assert.AreEqual((short)0x3D65, Crc16.FromReversedReciprocalPolynomial((short)0x9EB2));
            Assert.AreEqual((short)0x8005, Crc16.FromReversedReciprocalPolynomial((short)0xC002));
            Assert.AreEqual((short)0x5935, Crc16.FromReversedReciprocalPolynomial((short)0xAC9A));
            Assert.AreEqual((short)0x755B, Crc16.FromReversedReciprocalPolynomial((short)0xBAAD));
            Assert.AreEqual((short)0x1DCF, Crc16.FromReversedReciprocalPolynomial((short)0x8EE7));
        }
    }

    [TestMethod]
    public void Crc16_ToReversedReciprocal2() {
        unchecked {
            Assert.AreEqual((short)0x978A, Crc16.ToReversedReciprocalPolynomial((int)0x2F15));
            Assert.AreEqual((short)0xD015, Crc16.ToReversedReciprocalPolynomial((int)0xA02B));
            Assert.AreEqual((short)0x8810, Crc16.ToReversedReciprocalPolynomial((int)0x1021));
            Assert.AreEqual((short)0xE433, Crc16.ToReversedReciprocalPolynomial((int)0xC867));
            Assert.AreEqual((short)0x82C4, Crc16.ToReversedReciprocalPolynomial((int)0x0589));
            Assert.AreEqual((short)0xC5DB, Crc16.ToReversedReciprocalPolynomial((int)0x8BB7));
            Assert.AreEqual((short)0x9EB2, Crc16.ToReversedReciprocalPolynomial((int)0x3D65));
            Assert.AreEqual((short)0xC002, Crc16.ToReversedReciprocalPolynomial((int)0x8005));
            Assert.AreEqual((short)0xAC9A, Crc16.ToReversedReciprocalPolynomial((int)0x5935));
            Assert.AreEqual((short)0xBAAD, Crc16.ToReversedReciprocalPolynomial((int)0x755B));
            Assert.AreEqual((short)0x8EE7, Crc16.ToReversedReciprocalPolynomial((int)0x1DCF));
        }
    }

    [TestMethod]
    public void Crc16_FromReversedReciprocal2() {
        unchecked {
            Assert.AreEqual((short)0x2F15, Crc16.FromReversedReciprocalPolynomial((int)0x978A));
            Assert.AreEqual((short)0xA02B, Crc16.FromReversedReciprocalPolynomial((int)0xD015));
            Assert.AreEqual((short)0x1021, Crc16.FromReversedReciprocalPolynomial((int)0x8810));
            Assert.AreEqual((short)0xC867, Crc16.FromReversedReciprocalPolynomial((int)0xE433));
            Assert.AreEqual((short)0x0589, Crc16.FromReversedReciprocalPolynomial((int)0x82C4));
            Assert.AreEqual((short)0x8BB7, Crc16.FromReversedReciprocalPolynomial((int)0xC5DB));
            Assert.AreEqual((short)0x3D65, Crc16.FromReversedReciprocalPolynomial((int)0x9EB2));  // 135 @ HD=6
            Assert.AreEqual((short)0x8005, Crc16.FromReversedReciprocalPolynomial((int)0xC002));
            Assert.AreEqual((short)0x5935, Crc16.FromReversedReciprocalPolynomial((int)0xAC9A));  // 241 @ HD=5
            Assert.AreEqual((short)0x755B, Crc16.FromReversedReciprocalPolynomial((int)0xBAAD));
            Assert.AreEqual((short)0x1DCF, Crc16.FromReversedReciprocalPolynomial((int)0x8EE7));

            Assert.AreEqual((short)0xA2EB, Crc16.FromReversedReciprocalPolynomial((int)0xD175));  // 32751 @ HD=4
            Assert.AreEqual((short)0x1B2B, Crc16.FromReversedReciprocalPolynomial((int)0x8D95));  // 65519 @ HD=3
            Assert.AreEqual((short)0x2D17, Crc16.FromReversedReciprocalPolynomial((int)0x968B));  // 19 @ HD=7
            Assert.AreEqual((short)0x1FB7, Crc16.FromReversedReciprocalPolynomial((int)0x8FDB));  // 15 @ HD=8
            Assert.AreEqual((short)0xD25F, Crc16.FromReversedReciprocalPolynomial((int)0xE92F));  // 6 @ HD=9
            Assert.AreEqual((short)0xDA5F, Crc16.FromReversedReciprocalPolynomial((int)0xED2F));  // 5 @ HD=10
        }
    }

}
