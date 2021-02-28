using System;
using System.Text;
using Xunit;
using Medo.Security.Checksum;

namespace Tests {
    public class DammTests {

        [Fact(DisplayName = "Damm: Basic")]
        public void Basic() {
            using var checksum = new Damm();
            var hash = checksum.ComputeHash(new byte[] { 0x35, 0x37, 0x32 });
            Assert.Equal("34", BitConverter.ToString(hash));
            Assert.Equal(4, checksum.HashAsNumber);
            Assert.Equal('4', checksum.HashAsChar);
        }

        [Fact(DisplayName = "Damm: Basic ASCII")]
        public void BasicAscii() {
            using var checksum = new Damm();
            var hash = checksum.ComputeHash(Encoding.ASCII.GetBytes("572"));
            Assert.Equal("4", Encoding.ASCII.GetString(hash));
            Assert.Equal(4, checksum.HashAsNumber);
            Assert.Equal('4', checksum.HashAsChar);
        }

        [Fact(DisplayName = "Damm: Basic UTF8")]
        public void BasicUtf8() {
            using var checksum = new Damm();
            var hash = checksum.ComputeHash(Encoding.UTF8.GetBytes("572"));
            Assert.Equal("4", Encoding.UTF8.GetString(hash));
            Assert.Equal(4, checksum.HashAsNumber);
            Assert.Equal('4', checksum.HashAsChar);
        }

        [Fact(DisplayName = "Damm: Basic UTF8 with prefix")]
        public void BasicUtf8WithPrefix() {
            using var checksum = new Damm();
            var hash = checksum.ComputeHash(Encoding.UTF8.GetBytes("00572")); //any number of zeros can be added at front
            Assert.Equal("4", Encoding.UTF8.GetString(hash));
            Assert.Equal(4, checksum.HashAsNumber);
            Assert.Equal('4', checksum.HashAsChar);
        }


        [Fact(DisplayName = "Damm: Helper ComputeHash")]
        public void HelperComputeHash() {
            var hash = Damm.ComputeHash("572");
            Assert.Equal("4", hash);
        }

        [Fact(DisplayName = "Damm: Helper ComputeHash with spaces")]
        public void HelperComputeHashSpaces() {
            var hash = Damm.ComputeHash(" 5 7 2 ");
            Assert.Equal("4", hash);
        }

        [Fact(DisplayName = "Damm: Helper ComputeHash with dashes")]
        public void HelperComputeHashDashes() {
            var hash = Damm.ComputeHash("05-72");
            Assert.Equal("4", hash);
        }


        [Fact(DisplayName = "Damm: Helper ComputeHash (all digits)")]
        public void HelperComputeHashFull() {
            var hash = Damm.ComputeHash("572", returnAllDigits: true);
            Assert.Equal("5724", hash);
        }

        [Fact(DisplayName = "Damm: Helper ComputeHash (all digits) with spaces")]
        public void HelperComputeHashSpacesFull() {
            var hash = Damm.ComputeHash(" 5 7 2 ", returnAllDigits: true);
            Assert.Equal("5724", hash);
        }

        [Fact(DisplayName = "Damm: Helper ComputeHash (all digits) with dashes and leading zero")]
        public void HelperComputeHashDashesFull() {
            var hash = Damm.ComputeHash("05-72", returnAllDigits: true);
            Assert.Equal("05724", hash);
        }


        [Fact(DisplayName = "Damm: Helper ValidateHash")]
        public void HelperValidateHash() {
            var result = Damm.ValidateHash("5724");
            Assert.True(result);
        }

        [Fact(DisplayName = "Damm: Helper ValidateHash with spaces")]
        public void HelperValidateHashSpaces() {
            var result = Damm.ValidateHash(" 57 24 ");
            Assert.True(result);
        }

        [Fact(DisplayName = "Damm: Helper ValidateHash with dashes")]
        public void HelperValidateHashDashes() {
            var result = Damm.ValidateHash("-00-57-24-");
            Assert.True(result);
        }


        [Fact(DisplayName = "Damm: Helper ValidateHash fails")]
        public void HelperValidateHashFails() {
            var result = Damm.ValidateHash("5720");
            Assert.False(result);
        }


        [Fact(DisplayName = "Damm: Invalid characters (low ASCII)")]
        public void InvalidCharacters572() {
            using var checksum = new Damm();
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var hash = checksum.ComputeHash(new byte[] { 5, 7, 2 });
            });
        }

        [Fact(DisplayName = "Damm: Invalid characters (letters)")]
        public void InvalidCharactersABC() {
            using var checksum = new Damm();
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var hash = checksum.ComputeHash(Encoding.UTF8.GetBytes("ABC"));
            });
        }

    }
}
