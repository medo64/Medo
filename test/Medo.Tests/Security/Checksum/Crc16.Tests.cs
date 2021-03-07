using System;
using Xunit;

namespace Medo.Tests.Security.Checksum.Crc16 {
    using System.Text;
    using Medo.Security.Checksum;

    public class Tests {

        [Fact(DisplayName = "Crc16: Zmodem")]
        public void Zmodem() {
            string expected = "5E1B";
            Crc16 actualCrc = Crc16.GetZmodem();
            actualCrc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, actualCrc.HashAsInt16.ToString("X4"));
            Assert.Equal(expected, BitConverter.ToString(actualCrc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: Xmodem")]
        public void Xmodem() {
            string expected = "16A3";
            Crc16 actualCrc = Crc16.GetXmodem();
            actualCrc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, actualCrc.HashAsInt16.ToString("X4"));
            Assert.Equal(expected, BitConverter.ToString(actualCrc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: X25")]
        public void X25() {
            string expected = "CB47";
            Crc16 actualCrc = Crc16.GetX25();
            actualCrc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, actualCrc.HashAsInt16.ToString("X4"));
            Assert.Equal(expected, BitConverter.ToString(actualCrc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: Kermit")]
        public void Kermit() {
            string expected = "9839";
            Crc16 actualCrc = Crc16.GetKermit();
            actualCrc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, actualCrc.HashAsInt16.ToString("X4"));
            Assert.Equal(expected, BitConverter.ToString(actualCrc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: Ieee")]
        public void Ieee() {
            string expected = "178C";
            Crc16 actualCrc = Crc16.GetIeee();
            actualCrc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, actualCrc.HashAsInt16.ToString("X4"));
            Assert.Equal(expected, BitConverter.ToString(actualCrc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: Ccitt")]
        public void Ccitt() {
            string expected = "DF2E";
            Crc16 actualCrc = Crc16.GetCcitt();
            actualCrc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, actualCrc.HashAsInt16.ToString("X4"));
            Assert.Equal(expected, BitConverter.ToString(actualCrc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc16: Arc")]
        public void Arc() {
            string expected = "178C";
            Crc16 actualCrc = Crc16.GetArc();
            actualCrc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, actualCrc.HashAsInt16.ToString("X4"));
            Assert.Equal(expected, BitConverter.ToString(actualCrc.Hash).Replace("-", ""));
        }

    }
}
