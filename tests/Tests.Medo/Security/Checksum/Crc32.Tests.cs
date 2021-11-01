using System;
using System.Text;
using Xunit;
using Medo.Security.Checksum;

namespace Tests.Medo.Security.Checksum {
    public class Crc32Tests {

        [Fact(DisplayName = "Crc32: Xfer")]
        public void Xfer() {
            string expected = "3A9C355C";
            var crc = Crc32.GetXfer();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, crc.HashAsInt32.ToString("X8"));
            Assert.Equal(expected, BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: Posix")]
        public void Posix() {
            string expected = "EFC8804E";
            var crc = Crc32.GetPosix();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, crc.HashAsInt32.ToString("X8"));
            Assert.Equal(expected, BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: Jam")]
        public void Jam() {
            string expected = "E59A841D";
            var crc = Crc32.GetJam();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, crc.HashAsInt32.ToString("X8"));
            Assert.Equal(expected, BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: Ieee")]
        public void Ieee() {
            string expected = "1A657BE2";
            var crc = Crc32.GetIeee();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, crc.HashAsInt32.ToString("X8"));
            Assert.Equal(expected, BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc32: Reuse")]
        public void Reuse() {
            var checksum = Crc32.GetIeee();
            checksum.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal("1A657BE2", checksum.HashAsInt32.ToString("X8"));
            checksum.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal("1A657BE2", checksum.HashAsInt32.ToString("X8"));
        }

    }
}
