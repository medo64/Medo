using System;
using System.Text;
using Xunit;
using Medo.Security.Checksum;

namespace Tests.Medo.Security.Checksum {
    public class Crc8Tests {

        [Fact(DisplayName = "Crc8: Default")]
        public void Default() {
            string expected = "0xDF";
            var crc = new Crc8();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }


        [Fact(DisplayName = "Crc8: AUTOSAR")]
        public void Autosar() {
            string expected = "0x2F";
            var crc = Crc8.GetAutosar();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: AUTOSAR (2)")]
        public void Autosar_2() {
            var crc = Crc8.GetAutosar();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xDF, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: BLUETOOTH")]
        public void Bluetooth() {
            string expected = "0x8F";
            var crc = Crc8.GetBluetooth();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: BLUETOOTH (2)")]
        public void Bluetooth_2() {
            var crc = Crc8.GetBluetooth();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x26, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: CDMA2000")]
        public void Cdma2000() {
            string expected = "0xCE";
            var crc = Crc8.GetCdma2000();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: CDMA2000 (2)")]
        public void Cdma2000_2() {
            var crc = Crc8.GetCdma2000();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xDA, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: DARC")]
        public void Darc() {
            string expected = "0x24";
            var crc = Crc8.GetDarc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: DARC (2)")]
        public void Darc_2() {
            var crc = Crc8.GetDarc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x15, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: DVB-S2")]
        public void DvbS2() {
            string expected = "0x3E";
            var crc = Crc8.GetDvbS2();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: DVB-S2 (2)")]
        public void DvbS2_2() {
            var crc = Crc8.GetDvbS2();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xBC, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: GSM-A")]
        public void GsmA() {
            string expected = "0xBB";
            var crc = Crc8.GetGsmA();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: GSM-A (2)")]
        public void GsmA_2() {
            var crc = Crc8.GetGsmA();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x37, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: GSM-B")]
        public void GsmB() {
            string expected = "0xD6";
            var crc = Crc8.GetGsmB();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: GSM-B (2)")]
        public void GsmB_2() {
            var crc = Crc8.GetGsmB();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x94, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: HITAG")]
        public void Hitag() {
            string expected = "0xEA";
            var crc = Crc8.GetHitag();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: HITAG (2)")]
        public void Hitag_2() {
            var crc = Crc8.GetHitag();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xB4, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: I-432-1")]
        public void I4321() {
            string expected = "0x8A";
            var crc = Crc8.GetI4321();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: I-432-1 (2)")]
        public void I4321_2() {
            var crc = Crc8.GetI4321();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xA1, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: I-432-1 / CCITT (A)")]
        public void Ccitt_A() {
            var crc = Crc8.GetCcitt();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xA1, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: I-432-1 / ITU (A)")]
        public void Itu_A() {
            var crc = Crc8.GetItu();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xA1, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: LTE")]
        public void Lte() {
            string expected = "0xDF";
            var crc = Crc8.GetLte();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: LTE (2)")]
        public void Lte_2() {
            var crc = Crc8.GetLte();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xEA, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: MAXIM-DOW")]
        public void MaximDow() {
            string expected = "0x80";
            var crc = Crc8.GetMaximDow();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: MAXIM-DOW (2)")]
        public void MaximDow_2() {
            var crc = Crc8.GetMaximDow();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xA1, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: MAXIM-DOW / DALLAS (A)")]
        public void Dallas_A() {
            var crc = Crc8.GetDallas();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xA1, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: MAXIM-DOW / MAXIM (A)")]
        public void Maxim_A() {
            var crc = Crc8.GetMaxim();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xA1, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: MIFARE-MAD")]
        public void MifareMad() {
            string expected = "0x11";
            var crc = Crc8.GetMifareMad();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: MIFARE-MAD (2)")]
        public void MifareMad_2() {
            var crc = Crc8.GetMifareMad();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x99, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: MIFARE-MAD (3)")]
        public void MifareMad_3() {
            var crc = Crc8.GetMifareMad();
            crc.ComputeHash(new byte[] { 0x01, 0x01, 0x08, 0x01, 0x08, 0x01, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x03, 0x10, 0x03, 0x10, 0x02, 0x10, 0x02, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x11, 0x30 });
            Assert.Equal(0x89, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: MIFARE-MAD / MIFARE (A)")]
        public void Mifare_A() {
            var crc = Crc8.GetMifare();
            crc.ComputeHash(new byte[] { 0x01, 0x01, 0x08, 0x01, 0x08, 0x01, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x03, 0x10, 0x03, 0x10, 0x02, 0x10, 0x02, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x11, 0x30 });
            Assert.Equal(0x89, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: NRSC-5")]
        public void Nrsc5() {
            string expected = "0xE3";
            var crc = Crc8.GetNrsc5();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: NRSC-5 (2)")]
        public void Nrsc5_2() {
            var crc = Crc8.GetNrsc5();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xF7, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: OPENSAFETY")]
        public void OpenSafety() {
            string expected = "0x15";
            var crc = Crc8.GetOpenSafety();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: OPENSAFETY (2)")]
        public void OpenSafety_2() {
            var crc = Crc8.GetOpenSafety();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x3E, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: ROHC")]
        public void Rohc() {
            string expected = "0xA2";
            var crc = Crc8.GetRohc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: ROHC (2)")]
        public void Rohc_2() {
            var crc = Crc8.GetRohc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xD0, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: SAE-J1850")]
        public void SaeJ1850() {
            string expected = "0x15";
            var crc = Crc8.GetSaeJ1850();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: SAE-J1850 (2)")]
        public void SaeJ1850_2() {
            var crc = Crc8.GetSaeJ1850();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x4B, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: SMBUS")]
        public void SMBus() {
            string expected = "0xDF";
            var crc = Crc8.GetSMBus();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: SMBUS (2)")]
        public void SMBus_2() {
            var crc = Crc8.GetSMBus();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0xF4, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: TECH-3250")]
        public void Tech3250() {
            string expected = "0x70";
            var crc = Crc8.GetTech3250();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: TECH-3250 (2)")]
        public void Tech3250_2() {
            var crc = Crc8.GetTech3250();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x97, crc.HashAsByte);
        }

        [Fact(DisplayName = "Crc8: WCDMA2000")]
        public void Wcdma2000() {
            string expected = "0xD3";
            var crc = Crc8.GetWcdma2000();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, $"0x{crc.HashAsByte:X2}");
            Assert.Equal(expected, "0x" + BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: WCDMA2000 (2)")]
        public void Wcdma2000_2() {
            var crc = Crc8.GetWcdma2000();
            crc.ComputeHash(Encoding.ASCII.GetBytes("123456789"));
            Assert.Equal(0x25, crc.HashAsByte);
        }


        [Fact(DisplayName = "Crc8: Reuse same instance")]
        public void Reuse() {
            var checksum = Crc8.GetDallas();
            checksum.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal("80", checksum.HashAsByte.ToString("X2"));
            checksum.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal("80", checksum.HashAsByte.ToString("X2"));
        }

    }
}
