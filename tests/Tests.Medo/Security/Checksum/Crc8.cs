using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Security.Checksum;

namespace Tests;

[TestClass]
public class Crc8_Tests {

    [TestMethod]
    public void Crc8_Custom() {
        string expected = "0x15";
        var crc = Crc8.GetCustom(0x2F, 0x00, false, false, 0x00);
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }


    [TestMethod]
    public void Crc8_Autosar() {
        string expected = "0x2F";
        var crc = Crc8.GetAutosar();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_Autosar_2() {
        var crc = Crc8.GetAutosar();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xDF, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_Bluetooth() {
        string expected = "0x8F";
        var crc = Crc8.GetBluetooth();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_Bluetooth_2() {
        var crc = Crc8.GetBluetooth();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x26, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_Cdma2000() {
        string expected = "0xCE";
        var crc = Crc8.GetCdma2000();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_Cdma2000_2() {
        var crc = Crc8.GetCdma2000();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xDA, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_Darc() {
        string expected = "0x24";
        var crc = Crc8.GetDarc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_Darc_2() {
        var crc = Crc8.GetDarc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x15, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_DvbS2() {
        string expected = "0x3E";
        var crc = Crc8.GetDvbS2();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_DvbS2_2() {
        var crc = Crc8.GetDvbS2();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xBC, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_GsmA() {
        string expected = "0xBB";
        var crc = Crc8.GetGsmA();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_GsmA_2() {
        var crc = Crc8.GetGsmA();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x37, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_GsmB() {
        string expected = "0xD6";
        var crc = Crc8.GetGsmB();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_GsmB_2() {
        var crc = Crc8.GetGsmB();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x94, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_Hitag() {
        string expected = "0xEA";
        var crc = Crc8.GetHitag();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_Hitag_2() {
        var crc = Crc8.GetHitag();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xB4, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_I4321() {
        string expected = "0x8A";
        var crc = Crc8.GetI4321();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_4321_2() {
        var crc = Crc8.GetI4321();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA1, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_Ccitt_A() {
        var crc = Crc8.GetCcitt();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA1, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_Itu_A() {
        var crc = Crc8.GetItu();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA1, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_ICode() {
        string expected = "0xA4";
        var crc = Crc8.GetICode();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_ICode_2() {
        var crc = Crc8.GetICode();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x7E, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_ICode_3() {
        var crc = Crc8.GetICode();
        Assert.AreEqual(0xFD, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_ICode_4() {
        var crc = Crc8.GetICode();
        crc.ComputeHash(new byte[] { 0x30 });
        Assert.AreEqual(0xB4, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_ICode_5() {
        var crc = Crc8.GetICode();
        crc.ComputeHash(new byte[] { 0x30, 0x00 });
        Assert.AreEqual(0x18, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_ICode_6() {
        var crc = Crc8.GetICode();
        crc.ComputeHash(new byte[] { 0x30, 0x00, 0x00 });
        Assert.AreEqual(0x25, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_ICode_7() {
        var crc = Crc8.GetICode();
        crc.ComputeHash(new byte[] { 0x30, 0x00, 0x00, 0x25 });
        Assert.AreEqual(0x00, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_Lte() {
        string expected = "0xDF";
        var crc = Crc8.GetLte();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_Lte_2() {
        var crc = Crc8.GetLte();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xEA, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_MaximDow() {
        string expected = "0x80";
        var crc = Crc8.GetMaximDow();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_MaximDow_2() {
        var crc = Crc8.GetMaximDow();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA1, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_Dallas_A() {
        var crc = Crc8.GetDallas();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA1, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_Maxim_A() {
        var crc = Crc8.GetMaxim();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA1, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_MifareMad() {
        string expected = "0x11";
        var crc = Crc8.GetMifareMad();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_MifareMad_2() {
        var crc = Crc8.GetMifareMad();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x99, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_MifareMad_3() {
        var crc = Crc8.GetMifareMad();
        crc.ComputeHash(new byte[] { 0x01, 0x01, 0x08, 0x01, 0x08, 0x01, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x03, 0x10, 0x03, 0x10, 0x02, 0x10, 0x02, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x11, 0x30 });
        Assert.AreEqual(0x89, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_Mifare_A() {
        var crc = Crc8.GetMifare();
        crc.ComputeHash(new byte[] { 0x01, 0x01, 0x08, 0x01, 0x08, 0x01, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x03, 0x10, 0x03, 0x10, 0x02, 0x10, 0x02, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x11, 0x30 });
        Assert.AreEqual(0x89, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_Nrsc5() {
        string expected = "0xE3";
        var crc = Crc8.GetNrsc5();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_Nrsc5_2() {
        var crc = Crc8.GetNrsc5();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xF7, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_OpenSafety() {
        string expected = "0x15";
        var crc = Crc8.GetOpenSafety();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_OpenSafety_2() {
        var crc = Crc8.GetOpenSafety();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x3E, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_OpenSafety_3() {
        var crc = Crc8.GetOpenSafety();
        crc.ComputeHash(new byte[] { 0x02, 0xA8, 0x06, 0x00, 0x00, 0x60, 0x65, 0x00, 0x06, 0xA1 });
        Assert.AreEqual(0x1C, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_OpenSafety_4() {
        var crc = Crc8.GetOpenSafety();
        crc.ComputeHash(new byte[] { 0x03, 0xA8, 0x00, 0x01, 0x00, 0x00, 0x60, 0x65, 0x00, 0x06, 0xA1 });
        Assert.AreEqual(0x31, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_OpenSafety_5() {
        var crc = Crc8.GetOpenSafety();
        crc.ComputeHash(new byte[] { 0x03, 0xC4, 0x02, 0x9D, 0x00, 0x0F });
        Assert.AreEqual(0x9D, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_OpenSafety_6() {
        var crc = Crc8.GetOpenSafety();
        crc.ComputeHash(new byte[] { 0x03, 0xC4, 0x02, 0x82, 0x00, 0x0F });
        Assert.AreEqual(0x27, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_C2() {
        string expected = "0x15";
        var crc = Crc8.GetC2();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_Rohc() {
        string expected = "0xA2";
        var crc = Crc8.GetRohc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_Rohc_2() {
        var crc = Crc8.GetRohc();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xD0, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_SaeJ1850() {
        string expected = "0x15";
        var crc = Crc8.GetSaeJ1850();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_SaeJ1850_2() {
        var crc = Crc8.GetSaeJ1850();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x4B, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_SMBus() {
        string expected = "0xDF";
        var crc = Crc8.GetSMBus();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_SMBus_2() {
        var crc = Crc8.GetSMBus();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xF4, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_Tech3250() {
        string expected = "0x70";
        var crc = Crc8.GetTech3250();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_Tech3250_2() {
        var crc = Crc8.GetTech3250();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x97, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_Wcdma2000() {
        string expected = "0xD3";
        var crc = Crc8.GetWcdma2000();
        crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual(expected, $"0x{crc.HashAsByte:X2}");
        Assert.AreEqual(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

    [TestMethod]
    public void Crc8_Wcdma2000_2() {
        var crc = Crc8.GetWcdma2000();
        crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x25, crc.HashAsByte);
    }


    [TestMethod]
    public void Crc8_Reuse() {
        var checksum = Crc8.GetDallas();
        checksum.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("80", checksum.HashAsByte.ToString("X2"));
        checksum.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("80", checksum.HashAsByte.ToString("X2"));
    }


    [TestMethod]
    public void Crc8_ToReversedReciprocal() {
        Assert.AreEqual(0xEA, Crc8.ToReversedReciprocalPolynomial(0xD5));
        Assert.AreEqual(0x97, Crc8.ToReversedReciprocalPolynomial(0x2F));
        Assert.AreEqual(0xD3, Crc8.ToReversedReciprocalPolynomial(0xA7));
        Assert.AreEqual(0x83, Crc8.ToReversedReciprocalPolynomial(0x07));
        Assert.AreEqual(0x98, Crc8.ToReversedReciprocalPolynomial(0x31));
        Assert.AreEqual(0x9C, Crc8.ToReversedReciprocalPolynomial(0x39));
        Assert.AreEqual(0xA4, Crc8.ToReversedReciprocalPolynomial(0x49));
        Assert.AreEqual(0x8E, Crc8.ToReversedReciprocalPolynomial(0x1D));
        Assert.AreEqual(0xCD, Crc8.ToReversedReciprocalPolynomial(0x9B));
    }

    [TestMethod]
    public void Crc8_FromReversedReciprocal() {
        Assert.AreEqual(0xD5, Crc8.FromReversedReciprocalPolynomial(0xEA));
        Assert.AreEqual(0x2F, Crc8.FromReversedReciprocalPolynomial(0x97));
        Assert.AreEqual(0xA7, Crc8.FromReversedReciprocalPolynomial(0xD3));
        Assert.AreEqual(0x07, Crc8.FromReversedReciprocalPolynomial(0x83));  // 119 @ HD=4
        Assert.AreEqual(0x31, Crc8.FromReversedReciprocalPolynomial(0x98));
        Assert.AreEqual(0x39, Crc8.FromReversedReciprocalPolynomial(0x9C));
        Assert.AreEqual(0x49, Crc8.FromReversedReciprocalPolynomial(0xA4));
        Assert.AreEqual(0x1D, Crc8.FromReversedReciprocalPolynomial(0x8E));
        Assert.AreEqual(0x9B, Crc8.FromReversedReciprocalPolynomial(0xCD));

        Assert.AreEqual(0xCF, Crc8.FromReversedReciprocalPolynomial(0xE7));  // 247 @ HD=3
        Assert.AreEqual(0xD7, Crc8.FromReversedReciprocalPolynomial(0xEB));  // 9 @ HD=5
        Assert.AreEqual(0x37, Crc8.FromReversedReciprocalPolynomial(0x9B));  // 4 @ HD=6
    }

}
