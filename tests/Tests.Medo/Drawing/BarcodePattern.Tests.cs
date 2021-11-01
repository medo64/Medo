using System;
using System.IO;
using System.Text;
using Xunit;
using Medo.Drawing;
using System.Globalization;

namespace Tests.Medo.Drawing {
    public class BarcodePatternTests {

        [Fact(DisplayName = "BarcodePattern: Codabar Numbers")]
        public void CodabarNumbers() {
            var bp = BarcodePattern.GetNewCodabar("123");
            Assert.Equal("1-1-2-2-1-2-1-0-1-1-1-1-2-2-1-0-1-1-1-2-1-1-2-0-2-2-1-1-1-1-1-0-1-1-2-2-1-2-1", ToPatternString(bp.GetInterleavedPattern()));
            Assert.Equal("█ ██  █  █ █ █ ██  █ █ █  █ ██ ██  █ █ █ █ ██  █  █", ToPatternString(bp.GetBinaryPattern()));
            Assert.Equal(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
        }

        [Fact(DisplayName = "BarcodePattern: Codabar Custom Start/Stop")]
        public void CodaBarStartStop() {
            var bp = BarcodePattern.GetNewCodabar("123", 'D', 'B');
            Assert.Equal("1-1-1-2-2-2-1-0-1-1-1-1-2-2-1-0-1-1-1-2-1-1-2-0-2-2-1-1-1-1-1-0-1-1-1-2-1-2-2", ToPatternString(bp.GetInterleavedPattern()));
            Assert.Equal("█ █  ██  █ █ █ ██  █ █ █  █ ██ ██  █ █ █ █ █  █  ██", ToPatternString(bp.GetBinaryPattern()));
            Assert.Equal(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
        }

        [Fact(DisplayName = "BarcodePattern: Codabar Null Value")]
        public void CodaBarNullValue() {
            Assert.Throws<ArgumentNullException>(() => {
                var _ = BarcodePattern.GetNewCodabar(null);
            });
        }

        [Fact(DisplayName = "BarcodePattern: Codabar Invalid Value Character")]
        public void CodaBarInvalidValue() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var _ = BarcodePattern.GetNewCodabar("123A");
            });
        }

        [Fact(DisplayName = "BarcodePattern: Codabar Invalid Start Character")]
        public void CodaBarInvalidStart() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var _ = BarcodePattern.GetNewCodabar("123", 'E', 'A');
            });
        }

        [Fact(DisplayName = "BarcodePattern: Codabar Invalid End Character")]
        public void CodaBarInvalidEnd() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var _ = BarcodePattern.GetNewCodabar("123", 'A', 'E');
            });
        }


        [Fact(DisplayName = "BarcodePattern: Code128 Numbers")]
        public void Code128Numbers() {
            var bp = BarcodePattern.GetNewCode128("123");
            Assert.Equal("2-1-1-2-3-2-1-1-2-2-3-2-3-1-1-1-4-1-2-2-1-1-3-2-1-4-1-1-2-2-2-3-3-1-1-1-2", ToPatternString(bp.GetInterleavedPattern()));
            Assert.Equal("██ █  ███  █ ██  ███  ███ █ ████ ██  █ ███  █    █ ██  ██   ███ █ ██", ToPatternString(bp.GetBinaryPattern()));
            Assert.Equal(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
        }

        [Fact(DisplayName = "BarcodePattern: Code128 Text")]
        public void Code128Text() {
            var bp = BarcodePattern.GetNewCode128("ABC");
            Assert.Equal("2-1-1-4-1-2-1-1-1-3-2-3-1-3-1-1-2-3-1-3-1-3-2-1-2-1-2-2-2-2-2-3-3-1-1-1-2", ToPatternString(bp.GetInterleavedPattern()));
            Assert.Equal("██ █    █  █ █   ██   █   █ ██   █   █   ██ ██ ██  ██  ██   ███ █ ██", ToPatternString(bp.GetBinaryPattern()));
            Assert.Equal(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
        }

        [Fact(DisplayName = "BarcodePattern: Code128 Control")]
        public void Code128Control() {
            var bp = BarcodePattern.GetNewCode128("\r\n");
            Assert.Equal("2-1-1-4-1-2-4-1-3-1-1-1-1-4-2-2-1-1-2-2-1-1-3-2-2-3-3-1-1-1-2", ToPatternString(bp.GetInterleavedPattern()));
            Assert.Equal("██ █    █  ████ ███ █ █    ██  █ ██  █ ███  ██   ███ █ ██", ToPatternString(bp.GetBinaryPattern()));
            Assert.Equal(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
        }

        [Fact(DisplayName = "BarcodePattern: Code128 Dense Numbers")]
        public void Code128DenseNumbers() {
            var bp = BarcodePattern.GetNewCode128("1234");
            Assert.Equal("2-1-1-2-3-2-1-1-2-2-3-2-1-3-1-1-2-3-1-2-1-2-4-1-2-3-3-1-1-1-2", ToPatternString(bp.GetInterleavedPattern()));
            Assert.Equal("██ █  ███  █ ██  ███  █   █ ██   █  █  ████ ██   ███ █ ██", ToPatternString(bp.GetBinaryPattern()));
            Assert.Equal(bp.GetBinaryPattern().Length, bp.GetPatternWidth());
        }

        [Fact(DisplayName = "BarcodePattern: Code128 Null Value")]
        public void Code128NullValue() {
            Assert.Throws<ArgumentNullException>(() => {
                var _ = BarcodePattern.GetNewCode128(null);
            });
        }

        [Fact(DisplayName = "BarcodePattern: Code128 Invalid Value Character")]
        public void Code128InvalidValue() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var _ = BarcodePattern.GetNewCode128("Č");
            });
        }


        #region Convert

        private static string ToPatternString(int[] pattern) {
            var sb = new StringBuilder();
            foreach (var entry in pattern) {
                if (sb.Length > 0) { sb.Append('-'); }
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", entry);
            }
            return sb.ToString();
        }

        private static string ToPatternString(bool[] pattern) {
            var sb = new StringBuilder();
            foreach (var entry in pattern) {
                sb.Append(entry ? '█' : ' ');
            }
            return sb.ToString();
        }

        #endregion Convert

    }
}
