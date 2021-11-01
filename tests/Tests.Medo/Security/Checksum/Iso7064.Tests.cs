using System;
using System.Text;
using Xunit;
using Medo.Security.Checksum;

namespace Tests.Medo.Security.Checksum {
    public class Iso7064Tests {

        [Fact(DisplayName = "Iso7064: Single digit (1)")]
        public void SingleDigit1() {
            var crc = new Iso7064();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0"));
            Assert.Equal(2, crc.HashAsNumber);
            Assert.Equal("32", BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Iso7064: Single digit (2)")]
        public void SingleDigit2() {
            var crc = new Iso7064();
            crc.ComputeHash(Encoding.ASCII.GetBytes("1"));
            Assert.Equal(9, crc.HashAsNumber);
            Assert.Equal("39", BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Iso7064: Single digit (3)")]
        public void SingleDigit3() {
            var crc = new Iso7064();
            crc.ComputeHash(Encoding.ASCII.GetBytes("6"));
            Assert.Equal(0, crc.HashAsNumber);
            Assert.Equal("30", BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Iso7064: Single digit (4)")]
        public void SingleDigit4() {
            var crc = new Iso7064();
            crc.ComputeHash(Encoding.ASCII.GetBytes("9"));
            Assert.Equal(4, crc.HashAsNumber);
            Assert.Equal("34", BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Iso7064: Numbers (1)")]
        public void Numbers1() {
            var crc = new Iso7064();
            crc.ComputeHash(Encoding.ASCII.GetBytes("0823"));
            Assert.Equal(5, crc.HashAsNumber);
            Assert.Equal("35", BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Iso7064: Numbers (2)")]
        public void Numbers2() {
            var crc = new Iso7064();
            crc.ComputeHash(Encoding.ASCII.GetBytes("276616973212561"));
            Assert.Equal(5, crc.HashAsNumber);
            Assert.Equal("35", BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Iso7064: Numbers (3)")]
        public void Numbers3() {
            var crc = new Iso7064();
            crc.ComputeHash(Encoding.ASCII.GetBytes("65"));
            Assert.Equal(0, crc.HashAsNumber);
            Assert.Equal("30", BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Iso7064: Numbers (4)")]
        public void Numbers4() {
            var crc = new Iso7064();
            crc.ComputeHash(Encoding.ASCII.GetBytes("56"));
            Assert.Equal(0, crc.HashAsNumber);
            Assert.Equal("30", BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Iso7064: Numbers (5)")]
        public void Numbers5() {
            var crc = new Iso7064();
            crc.ComputeHash(Encoding.ASCII.GetBytes("732"));
            Assert.Equal(5, crc.HashAsNumber);
            Assert.Equal("35", BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Iso7064: Numbers (6)")]
        public void Numbers6() {
            var crc = new Iso7064();
            crc.ComputeHash(Encoding.ASCII.GetBytes("723"));
            Assert.Equal(5, crc.HashAsNumber);
            Assert.Equal("35", BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Iso7064: Numbers (7)")]
        public void Numbers7() {
            var crc = new Iso7064();
            crc.ComputeHash(Encoding.ASCII.GetBytes("8373426074"));
            Assert.Equal(9, crc.HashAsNumber);
            Assert.Equal("39", BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Iso7064: Numbers (8)")]
        public void Numbers8() {
            var crc = new Iso7064();
            crc.ComputeHash(Encoding.ASCII.GetBytes("4428922675"));
            Assert.Equal(7, crc.HashAsNumber);
            Assert.Equal("37", BitConverter.ToString(crc.Hash).Replace("-", ""));
        }


        [Fact(DisplayName = "Iso7064: Reuse")]
        public void Reuse() {
            var crc = new Iso7064();
            crc.ComputeHash(Encoding.ASCII.GetBytes("4428922675"));
            Assert.Equal(7, crc.HashAsNumber);
            Assert.Equal("37", BitConverter.ToString(crc.Hash).Replace("-", ""));
            crc.ComputeHash(Encoding.ASCII.GetBytes("4428922675"));
            Assert.Equal(7, crc.HashAsNumber);
            Assert.Equal("37", BitConverter.ToString(crc.Hash).Replace("-", ""));
        }

        [Fact(DisplayName = "Iso7064: Validate")]
        public void Validate() {
            Assert.Equal("7", Iso7064.ComputeHash("4428922675"));
            Assert.True(Iso7064.ValidateHash("44289226757"));
        }

    }
}
