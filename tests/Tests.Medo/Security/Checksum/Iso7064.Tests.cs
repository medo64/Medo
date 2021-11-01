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

        [Fact(DisplayName = "Iso7064: Helper ComputeHash")]
        public void HelperComputeHash() {
            var hash = Iso7064.ComputeHash("572");
            Assert.Equal("1", hash);
        }

        [Fact(DisplayName = "Iso7064: Helper ComputeHash with spaces")]
        public void HelperComputeHashSpaces() {
            var hash = Iso7064.ComputeHash(" 5 7 2 ");
            Assert.Equal("1", hash);
        }

        [Fact(DisplayName = "Iso7064: Helper ComputeHash with dashes")]
        public void HelperComputeHashDashes() {
            var hash = Iso7064.ComputeHash("05-72");
            Assert.Equal("7", hash);
        }


        [Fact(DisplayName = "Iso7064: Helper ComputeHash (all digits)")]
        public void HelperComputeHashFull() {
            var hash = Iso7064.ComputeHash("572", returnAllDigits: true);
            Assert.Equal("5721", hash);
        }

        [Fact(DisplayName = "Iso7064: Helper ComputeHash (all digits) with spaces")]
        public void HelperComputeHashSpacesFull() {
            var hash = Iso7064.ComputeHash(" 5 7 2 ", returnAllDigits: true);
            Assert.Equal("5721", hash);
        }

        [Fact(DisplayName = "Iso7064: Helper ComputeHash (all digits) with dashes and leading zero")]
        public void HelperComputeHashDashesFull() {
            var hash = Iso7064.ComputeHash("05-72", returnAllDigits: true);
            Assert.Equal("05727", hash);
        }


        [Fact(DisplayName = "Iso7064: Helper ValidateHash")]
        public void HelperValidateHash() {
            var result = Iso7064.ValidateHash("5721");
            Assert.True(result);
        }

        [Fact(DisplayName = "Iso7064: Helper ValidateHash with spaces")]
        public void HelperValidateHashSpaces() {
            var result = Iso7064.ValidateHash(" 57 21 ");
            Assert.True(result);
        }

        [Fact(DisplayName = "Iso7064: Helper ValidateHash with dashes")]
        public void HelperValidateHashDashes() {
            var result = Iso7064.ValidateHash("-57-21-");
            Assert.True(result);
        }


        [Fact(DisplayName = "Iso7064: Helper ValidateHash fails")]
        public void HelperValidateHashFails() {
            var result = Iso7064.ValidateHash("5720");
            Assert.False(result);
        }


        [Fact(DisplayName = "Iso7064: Invalid characters (low ASCII)")]
        public void InvalidCharacters572() {
            using var checksum = new Iso7064();
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var hash = checksum.ComputeHash(new byte[] { 5, 7, 2 });
            });
        }

        [Fact(DisplayName = "Iso7064: Invalid characters (letters)")]
        public void InvalidCharactersABC() {
            using var checksum = new Iso7064();
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var hash = checksum.ComputeHash(Encoding.UTF8.GetBytes("ABC"));
            });
        }

    }
}
