using System;
using System.Text;
using Xunit;
using Medo.Security.Checksum;

namespace Tests.Medo.Security.Checksum {
    public class Crc8Tests {

        [Fact(DisplayName = "Crc8: Dallas")]
        public void Dallas() {
            string expected = "80";
            var crc = Crc8.GetDallas();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, crc.HashAsByte.ToString("X2"));
            Assert.Equal(expected, BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: Maxim")]
        public void Maxim() {
            string expected = "80";
            var crc = Crc8.GetDallas();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, crc.HashAsByte.ToString("X2"));
            Assert.Equal(expected, BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

    }
}
