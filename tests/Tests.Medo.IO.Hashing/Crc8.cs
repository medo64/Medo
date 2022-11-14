using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.IO.Hashing;

namespace Tests;

[TestClass]
public class Crc8_Tests {

    [TestMethod]
    public void Crc8_Custom() {
        var crc = Crc8.GetCustom(0x2F, 0x00, false, false, 0x00);
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x3E, crc.HashAsByte);
    }

    [TestMethod]
    public void Crc8_Custom_2() {
        var crc = Crc8.GetCustom(0x2F, 0x00, false, false, 0x00);
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("15", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("15", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }


    [TestMethod]  // AUTOSAR
    public void Crc8_Autosar() {
        var crc = Crc8.GetAutosar();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xDF, crc.HashAsByte);
    }

    [TestMethod]  // AUTOSAR
    public void Crc8_Autosar_2() {
        var crc = Crc8.GetAutosar();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("2F", $"{ crc.HashAsByte:X2}");
        Assert.AreEqual("2F", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // BLUETOOTH
    public void Crc8_Bluetooth() {
        var crc = Crc8.GetBluetooth();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x26, crc.HashAsByte);
    }

    [TestMethod]  // BLUETOOTH
    public void Crc8_Bluetooth_2() {
        var crc = Crc8.GetBluetooth();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("8F", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("8F", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // CDMA2000
    public void Crc8_Cdma2000() {
        var crc = Crc8.GetCdma2000();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xDA, crc.HashAsByte);
    }

    [TestMethod]  // CDMA2000
    public void Crc8_Cdma2000_2() {
        var crc = Crc8.GetCdma2000();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("CE", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("CE", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // DARC
    public void Crc8_Darc() {
        var crc = Crc8.GetDarc();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x15, crc.HashAsByte);
    }

    [TestMethod]  // DARC
    public void Crc8_Darc_2() {
        var crc = Crc8.GetDarc();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("24", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("24", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // DVB-S2
    public void Crc8_DvbS2() {
        var crc = Crc8.GetDvbS2();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xBC, crc.HashAsByte);
    }

    [TestMethod]  // DVB-S2
    public void Crc8_DvbS2_2() {
        var crc = Crc8.GetDvbS2();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("3E", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("3E", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // GSM-A
    public void Crc8_GsmA() {
        var crc = Crc8.GetGsmA();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x37, crc.HashAsByte);
    }

    [TestMethod]  // GSM-A
    public void Crc8_GsmA_2() {
        var crc = Crc8.GetGsmA();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("BB", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("BB", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // GSM-B
    public void Crc8_GsmB() {
        var crc = Crc8.GetGsmB();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x94, crc.HashAsByte);
    }

    [TestMethod]  // GSM-B
    public void Crc8_GsmB_2() {
        var crc = Crc8.GetGsmB();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("D6", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("D6", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // HITAG
    public void Crc8_Hitag() {
        var crc = Crc8.GetHitag();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xB4, crc.HashAsByte);
    }

    [TestMethod]  // HITAG
    public void Crc8_Hitag_2() {
        var crc = Crc8.GetHitag();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("EA", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("EA", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // I-432-1
    public void Crc8_I4321() {
        var crc = Crc8.GetI4321();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA1, crc.HashAsByte);
    }

    [TestMethod]  // I-432-1
    public void Crc8_I4321_2() {
        var crc = Crc8.GetI4321();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("8A", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("8A", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // I-432-1 / CCITT (A)
    public void Crc8_Ccitt_A() {
        var crc = Crc8.GetCcitt();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA1, crc.HashAsByte);
    }

    [TestMethod]  // I-432-1 / ITU (A)
    public void Crc8_Itu_A() {
        var crc = Crc8.GetItu();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA1, crc.HashAsByte);
    }

    [TestMethod]  // I-CODE
    public void Crc8_ICode() {
        var crc = Crc8.GetICode();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x7E, crc.HashAsByte);
    }

    [TestMethod]  // I-CODE
    public void Crc8_ICode_2() {
        var crc = Crc8.GetICode();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("A4", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("A4", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // I-CODE
    public void Crc8_ICode_3() {
        var crc = Crc8.GetICode();
        Assert.AreEqual(0xFD, crc.HashAsByte);
    }

    [TestMethod]  // I-CODE
    public void Crc8_ICode_4() {
        var crc = Crc8.GetICode();
        crc.Append(new byte[] { 0x30 });
        Assert.AreEqual(0xB4, crc.HashAsByte);
    }

    [TestMethod]  // I-CODE
    public void Crc8_ICode_5() {
        var crc = Crc8.GetICode();
        crc.Append(new byte[] { 0x30, 0x00 });
        Assert.AreEqual(0x18, crc.HashAsByte);
    }

    [TestMethod]  // I-CODE
    public void Crc8_ICode_6() {
        var crc = Crc8.GetICode();
        crc.Append(new byte[] { 0x30, 0x00, 0x00 });
        Assert.AreEqual(0x25, crc.HashAsByte);
    }

    [TestMethod]  // I-CODE
    public void Crc8_ICode_7() {
        var crc = Crc8.GetICode();
        crc.Append(new byte[] { 0x30, 0x00, 0x00, 0x25 });
        Assert.AreEqual(0x00, crc.HashAsByte);
    }

    [TestMethod]  // LTE
    public void Crc8_Lte() {
        var crc = Crc8.GetLte();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xEA, crc.HashAsByte);
    }

    [TestMethod]  // LTE
    public void Crc8_Lte_2() {
        var crc = Crc8.GetLte();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("DF", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("DF", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // MAXIM-DOW
    public void Crc8_MaximDow() {
        var crc = Crc8.GetMaximDow();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA1, crc.HashAsByte);
    }

    [TestMethod]  // MAXIM-DOW
    public void Crc8_MaximDow_2() {
        var crc = Crc8.GetMaximDow();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("80", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("80", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // MAXIM-DOW / DALLAS (A)
    public void Crc8_Dallas_A() {
        var crc = Crc8.GetDallas();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA1, crc.HashAsByte);
    }

    [TestMethod]  // MAXIM-DOW / MAXIM (A)
    public void Crc8_Maxim_A() {
        var crc = Crc8.GetMaxim();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xA1, crc.HashAsByte);
    }

    [TestMethod]  // MIFARE-MAD
    public void Crc8_MifareMad() {
        var crc = Crc8.GetMifareMad();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x99, crc.HashAsByte);
    }

    [TestMethod]  // MIFARE-MAD
    public void Crc8_MifareMad_2() {
        var crc = Crc8.GetMifareMad();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("11", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("11", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // MIFARE-MAD
    public void Crc8_MifareMad_3() {
        var crc = Crc8.GetMifareMad();
        crc.Append(new byte[] { 0x01, 0x01, 0x08, 0x01, 0x08, 0x01, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x03, 0x10, 0x03, 0x10, 0x02, 0x10, 0x02, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x11, 0x30 });
        Assert.AreEqual(0x89, crc.HashAsByte);
    }

    [TestMethod]  // MIFARE-MAD / MIFARE (A)
    public void Crc8_Mifare_A() {
        var crc = Crc8.GetMifare();
        crc.Append(new byte[] { 0x01, 0x01, 0x08, 0x01, 0x08, 0x01, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x03, 0x10, 0x03, 0x10, 0x02, 0x10, 0x02, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x11, 0x30 });
        Assert.AreEqual(0x89, crc.HashAsByte);
    }

    [TestMethod]  // NRSC-5
    public void Crc8_Nrsc5() {
        var crc = Crc8.GetNrsc5();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xF7, crc.HashAsByte);
    }

    [TestMethod]  // NRSC-5
    public void Crc8_Nrsc5_2() {
        var crc = Crc8.GetNrsc5();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("E3", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("E3", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // OPENSAFETY
    public void Crc8_OpenSafety() {
        var crc = Crc8.GetOpenSafety();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x3E, crc.HashAsByte);
    }

    [TestMethod]  // OPENSAFETY
    public void Crc8_OpenSafety_2() {
        var crc = Crc8.GetOpenSafety();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("15", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("15", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // OPENSAFETY
    public void Crc8_OpenSafety_3() {
        var crc = Crc8.GetOpenSafety();
        crc.Append(new byte[] { 0x02, 0xA8, 0x06, 0x00, 0x00, 0x60, 0x65, 0x00, 0x06, 0xA1 });
        Assert.AreEqual(0x1C, crc.HashAsByte);
    }

    [TestMethod]  // OPENSAFETY
    public void Crc8_OpenSafety_4() {
        var crc = Crc8.GetOpenSafety();
        crc.Append(new byte[] { 0x03, 0xA8, 0x00, 0x01, 0x00, 0x00, 0x60, 0x65, 0x00, 0x06, 0xA1 });
        Assert.AreEqual(0x31, crc.HashAsByte);
    }

    [TestMethod]  // OPENSAFETY
    public void Crc8_OpenSafety_5() {
        var crc = Crc8.GetOpenSafety();
        crc.Append(new byte[] { 0x03, 0xC4, 0x02, 0x9D, 0x00, 0x0F });
        Assert.AreEqual(0x9D, crc.HashAsByte);
    }

    [TestMethod]  // OPENSAFETY
    public void Crc8_OpenSafety_6() {
        var crc = Crc8.GetOpenSafety();
        crc.Append(new byte[] { 0x03, 0xC4, 0x02, 0x82, 0x00, 0x0F });
        Assert.AreEqual(0x27, crc.HashAsByte);
    }

    [TestMethod]  // C2
    public void Crc8_C2() {
        var crc = Crc8.GetC2();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x3E, crc.HashAsByte);
    }

    [TestMethod]  // C2
    public void Crc8_C2_2() {
        var crc = Crc8.GetC2();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("15", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("15", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // ROHC
    public void Crc8_Rohc() {
        var crc = Crc8.GetRohc();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xD0, crc.HashAsByte);
    }

    [TestMethod]  // ROHC
    public void Crc8_Rohc_2() {
        var crc = Crc8.GetRohc();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("A2", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("A2", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // SAE-J1850
    public void Crc8_SaeJ1850() {
        var crc = Crc8.GetSaeJ1850();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x4B, crc.HashAsByte);
    }

    [TestMethod]  // SAE-J1850
    public void Crc8_SaeJ1850_2() {
        var crc = Crc8.GetSaeJ1850();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("15", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("15", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // SMBUS
    public void Crc8_SMBus() {
        var crc = Crc8.GetSMBus();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xF4, crc.HashAsByte);
    }

    [TestMethod]  // SMBUS
    public void Crc8_SMBus_2() {
        var crc = Crc8.GetSMBus();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("DF", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("DF", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // TECH-3250
    public void Crc8_Tech3250() {
        var crc = Crc8.GetTech3250();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x97, crc.HashAsByte);
    }

    [TestMethod]  // TECH-3250
    public void Crc8_Tech3250_2() {
        var crc = Crc8.GetTech3250();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("70", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("70", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // WCDMA2000
    public void Crc8_Wcdma2000() {
        var crc = Crc8.GetWcdma2000();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x25, crc.HashAsByte);
    }

    [TestMethod]  // WCDMA2000
    public void Crc8_Wcdma2000_2() {
        var crc = Crc8.GetWcdma2000();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("D3", $"{crc.HashAsByte:X2}");
        Assert.AreEqual("D3", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }


    [TestMethod]
    public void Crc8_Reuse() {
        var checksum = Crc8.GetDallas();
        checksum.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("80", BitConverter.ToString(checksum.GetHashAndReset()));
        checksum.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
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
