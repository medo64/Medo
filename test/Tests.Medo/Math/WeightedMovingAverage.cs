using System;
using Xunit;

namespace Medo.Tests.Math.WeightedMovingAverage {
    using Medo.Math;

    public class Tests {

        [Fact(DisplayName = "WeightedMovingAverage: Example (1)")]
        public void Example1() {
            var stats = new WeightedMovingAverage();
            stats.Add(4);
            stats.Add(7);
            stats.Add(13);
            stats.Add(16);
            Assert.Equal(12.1, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (2)")]
        public void Example2() {
            var stats = new WeightedMovingAverage();
            stats.Add(100000004);
            stats.Add(100000007);
            stats.Add(100000013);
            stats.Add(100000016);
            Assert.Equal(100000012.1, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (3)")]
        public void Example3() {
            var stats = new WeightedMovingAverage();
            stats.Add(1000000004);
            stats.Add(1000000007);
            stats.Add(1000000013);
            stats.Add(1000000016);
            Assert.Equal(1000000012.1000001, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (4)")]
        public void Example4() {
            var stats = new WeightedMovingAverage();
            stats.Add(6);
            stats.Add(2);
            stats.Add(3);
            stats.Add(1);
            Assert.Equal(2.3, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (5)")]
        public void Example5() {
            var stats = new WeightedMovingAverage(new double[] { 2, 2, 5, 7 });
            Assert.Equal(4.9, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (6)")]
        public void Example6() {
            var stats = new WeightedMovingAverage();
            stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
            Assert.Equal(5.944444444444445, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (7)")]
        public void Example7() {
            var stats = new WeightedMovingAverage();
            stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
            Assert.Equal(6.963636363636364, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (8)")]
        public void Example8() {
            var stats = new WeightedMovingAverage();
            stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
            Assert.Equal(52.02, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (9)")]
        public void Example9() {
            var stats = new WeightedMovingAverage();
            stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
            Assert.Equal(0.333333333333333, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (10)")]
        public void Example10() {
            var stats = new WeightedMovingAverage();
            stats.AddRange(new double[] { -1, 0, 1 });
            Assert.Equal(0.333333333333333, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (11)")]
        public void Example11() {
            var stats = new WeightedMovingAverage(3);
            stats.Add(4);
            stats.Add(7);
            stats.Add(13);
            stats.Add(16);
            Assert.Equal(13.5, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (12)")]
        public void Example12() {
            var stats = new WeightedMovingAverage(3);
            stats.Add(100000004);
            stats.Add(100000007);
            stats.Add(100000013);
            stats.Add(100000016);
            Assert.Equal(100000013.5, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (13)")]
        public void Example13() {
            var stats = new WeightedMovingAverage(3);
            stats.Add(1000000004);
            stats.Add(1000000007);
            stats.Add(1000000013);
            stats.Add(1000000016);
            Assert.Equal(1000000013.5, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (14)")]
        public void Example14() {
            var stats = new WeightedMovingAverage(3);
            stats.Add(6);
            stats.Add(2);
            stats.Add(3);
            stats.Add(1);
            Assert.Equal(1.833333333333333, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (15)")]
        public void Example15() {
            var stats = new WeightedMovingAverage(3, new double[] { 2, 2, 5, 7 });
            Assert.Equal(5.5, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (16)")]
        public void Example16() {
            var stats = new WeightedMovingAverage(3);
            stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
            Assert.Equal(7.666666666666667, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (17)")]
        public void Example17() {
            var stats = new WeightedMovingAverage(3);
            stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
            Assert.Equal(6, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (18)")]
        public void Example18() {
            var stats = new WeightedMovingAverage(3);
            stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
            Assert.Equal(51.9, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (19)")]
        public void Example19() {
            var stats = new WeightedMovingAverage(3);
            stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
            Assert.Equal(1.666666666666667, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: Example (20)")]
        public void Example20() {
            var stats = new WeightedMovingAverage(3);
            stats.AddRange(new double[] { -1, 0, 1 });
            Assert.Equal(0.333333333333333, stats.Average, 15);
        }

        [Fact(DisplayName = "WeightedMovingAverage: No values")]
        public void NoValue() {
            var stats = new WeightedMovingAverage();
            Assert.Equal(double.NaN, stats.Average);
        }

        [Fact(DisplayName = "WeightedMovingAverage: One value")]
        public void OneValue() {
            var stats = new WeightedMovingAverage();
            stats.Add(1);
            Assert.Equal(1, stats.Average);
        }

        [Fact(DisplayName = "WeightedMovingAverage: No infinities")]
        public void NoInfinity() {
            var stats = new WeightedMovingAverage();
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                stats.Add(double.NegativeInfinity);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                stats.Add(double.PositiveInfinity);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                stats.Add(double.NaN);
            });
        }

        [Fact(DisplayName = "WeightedMovingAverage: No null collection")]
        public void NoNullCollection() {
            Assert.Throws<ArgumentNullException>(delegate {
                var stats = new WeightedMovingAverage(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                var stats = new WeightedMovingAverage(10, null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                var stats = new WeightedMovingAverage();
                stats.AddRange(null);
            });
        }

        [Fact(DisplayName = "WeightedMovingAverage: No count out of range")]
        public void NoCountOutOfRange() {
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var stats = new WeightedMovingAverage(0);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var stats = new WeightedMovingAverage(0, new double[] { 0 });
            });
        }

    }
}
