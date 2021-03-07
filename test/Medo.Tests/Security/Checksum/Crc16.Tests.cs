using System;
using Xunit;

namespace Medo.Tests.Security.Checksum.Crc16 {
    using System.Text;
    using Medo.Security.Checksum;

    public class Tests {

        [Fact(DisplayName = "Crc16: Zmodem")]
        public void Zmodem() {
            string expected = "5E1B";
            var crc = Crc16.GetZmodem();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, crc.HashAsInt16.ToString("X4"));
            Assert.Equal(expected, BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: Xmodem")]
        public void Xmodem() {
            string expected = "16A3";
            var crc = Crc16.GetXmodem();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, crc.HashAsInt16.ToString("X4"));
            Assert.Equal(expected, BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: X25")]
        public void X25() {
            string expected = "CB47";
            var crc = Crc16.GetX25();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, crc.HashAsInt16.ToString("X4"));
            Assert.Equal(expected, BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: Kermit")]
        public void Kermit() {
            string expected = "9839";
            var crc = Crc16.GetKermit();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, crc.HashAsInt16.ToString("X4"));
            Assert.Equal(expected, BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: Ieee")]
        public void Ieee() {
            string expected = "178C";
            var crc = Crc16.GetIeee();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, crc.HashAsInt16.ToString("X4"));
            Assert.Equal(expected, BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: Ccitt")]
        public void Ccitt() {
            string expected = "DF2E";
            var crc = Crc16.GetCcitt();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, crc.HashAsInt16.ToString("X4"));
            Assert.Equal(expected, BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: Arc")]
        public void Arc() {
            string expected = "178C";
            var crc = Crc16.GetArc();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, crc.HashAsInt16.ToString("X4"));
            Assert.Equal(expected, BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

    }
}
