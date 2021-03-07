using System;
using Xunit;

namespace Medo.Tests.Security.Checksum.Crc8 {
    using System.Text;
    using Medo.Security.Checksum;

    public class Tests {

        [Fact(DisplayName = "Crc8: Dallas")]
        public void Dallas() {
            string expected = "80";
            Crc8 actualCrc = Crc8.GetDallas();
            actualCrc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, actualCrc.HashAsByte.ToString("X2"));
            Assert.Equal(expected, BitConverter.ToString(actualCrc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Crc8: Maxim")]
        public void Maxim() {
            string expected = "80";
            Crc8 actualCrc = Crc8.GetDallas();
            actualCrc.ComputeHash(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.Equal(expected, actualCrc.HashAsByte.ToString("X2"));
            Assert.Equal(expected, BitConverter.ToString(actualCrc.Hash).Replace("-", ""));
        }

    }
}
