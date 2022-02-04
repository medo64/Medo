using Xunit;
using Medo.Extensions.HexString;
using System;

namespace Tests.Medo.Extensions {
    public class HexStringExtensionTests {

        [Fact(DisplayName = "HexStringExtension: Basic span")]
        public void BasicSpan() {
            var span = new byte[] { 1, 2, 42 }.AsSpan();
            Assert.Equal("01022a", span.ToHexString());
            Assert.Equal("022a", span[1..].ToHexString());

            Assert.Equal("01-02-2A", BitConverter.ToString("01022a".FromHexString()));

        }

        [Fact(DisplayName = "HexStringExtension: Basic array")]
        public void BasicArray() {
            var array = new byte[] { 1, 2, 42 };
            Assert.Equal("01022a", array.ToHexString());
            Assert.Equal("022a", array.ToHexString(1, 2));

            Assert.Equal("01-02-2A", BitConverter.ToString("01022a".FromHexString()));

        }

        [Fact(DisplayName = "HexStringExtension: Invalid array position")]
        public void InvalidArrayPosition() {
            var array = new byte[1];
            Assert.Throws<ArgumentOutOfRangeException>(() => { array.ToHexString(1, 1); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { array.ToHexString(0, -1); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { array.ToHexString(-1, 1); });
            Assert.Equal("00", array.ToHexString(0, 1));
            Assert.Equal("", array.ToHexString(1, 0));
        }

        [Fact(DisplayName = "HexStringExtension: Null")]
        public void Null() {
            Assert.Throws<ArgumentNullException>(() => {
                byte[] array = null;
                Assert.Null(array.ToHexString());
            });

            Assert.Throws<ArgumentNullException>(() => {
                string text = null;
                Assert.Null(text.FromHexString());
            });
        }

    }
}
