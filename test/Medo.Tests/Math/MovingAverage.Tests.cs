using System;
using Xunit;

namespace Medo.Tests.Math.MovingAverage {
    using Medo.Math;

    public class Tests {

        [Fact(DisplayName = "MovingAverage: Example (1)")]
        public void Example1() {
            var stats = new MovingAverage();
            stats.Add(4);
            stats.Add(7);
            stats.Add(13);
            stats.Add(16);
            Assert.Equal(10, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (2)")]
        public void Example2() {
            var stats = new MovingAverage();
            stats.Add(100000004);
            stats.Add(100000007);
            stats.Add(100000013);
            stats.Add(100000016);
            Assert.Equal(100000010, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (3)")]
        public void Example3() {
            var stats = new MovingAverage();
            stats.Add(1000000004);
            stats.Add(1000000007);
            stats.Add(1000000013);
            stats.Add(1000000016);
            Assert.Equal(1000000010, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (4)")]
        public void Example4() {
            var stats = new MovingAverage();
            stats.Add(6);
            stats.Add(2);
            stats.Add(3);
            stats.Add(1);
            Assert.Equal(3, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (5)")]
        public void Example5() {
            var stats = new MovingAverage(new double[] { 2, 2, 5, 7 });
            Assert.Equal(4, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (6)")]
        public void Example6() {
            var stats = new MovingAverage();
            stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
            Assert.Equal(5, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (7)")]
        public void Example7() {
            var stats = new MovingAverage();
            stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
            Assert.Equal(7, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (8)")]
        public void Example8() {
            var stats = new MovingAverage();
            stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
            Assert.Equal(52.2, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (9)")]
        public void Example9() {
            var stats = new MovingAverage();
            stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
            Assert.Equal(-1, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (10)")]
        public void Example10() {
            var stats = new MovingAverage();
            stats.AddRange(new double[] { -1, 0, 1 });
            Assert.Equal(0, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (11)")]
        public void Example11() {
            var stats = new MovingAverage(3);
            stats.Add(4);
            stats.Add(7);
            stats.Add(13);
            stats.Add(16);
            Assert.Equal(12, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (12)")]
        public void Example12() {
            var stats = new MovingAverage(3);
            stats.Add(100000004);
            stats.Add(100000007);
            stats.Add(100000013);
            stats.Add(100000016);
            Assert.Equal(100000012, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (13)")]
        public void Example13() {
            var stats = new MovingAverage(3);
            stats.Add(1000000004);
            stats.Add(1000000007);
            stats.Add(1000000013);
            stats.Add(1000000016);
            Assert.Equal(1000000012, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (14)")]
        public void Example14() {
            var stats = new MovingAverage(3);
            stats.Add(6);
            stats.Add(2);
            stats.Add(3);
            stats.Add(1);
            Assert.Equal(2, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (15)")]
        public void Example15() {
            var stats = new MovingAverage(3, new double[] { 2, 2, 5, 7 });
            Assert.Equal(4.666666666666667, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (16)")]
        public void Example16() {
            var stats = new MovingAverage(3);
            stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
            Assert.Equal(7, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (17)")]
        public void Example17() {
            var stats = new MovingAverage(3);
            stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
            Assert.Equal(6.333333333333333, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (18)")]
        public void Example18() {
            var stats = new MovingAverage(3);
            stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
            Assert.Equal(52.5, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (19)")]
        public void Example19() {
            var stats = new MovingAverage(3);
            stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
            Assert.Equal(1, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: Example (20)")]
        public void Example20() {
            var stats = new MovingAverage(3);
            stats.AddRange(new double[] { -1, 0, 1 });
            Assert.Equal(0, stats.Average, 15);
        }

        [Fact(DisplayName = "MovingAverage: No values")]
        public void NoValue() {
            var stats = new MovingAverage();
            Assert.Equal(double.NaN, stats.Average);
        }

        [Fact(DisplayName = "MovingAverage: One value")]
        public void OneValue() {
            var stats = new MovingAverage();
            stats.Add(1);
            Assert.Equal(1, stats.Average);
        }

        [Fact(DisplayName = "MovingAverage: No infinities")]
        public void NoInfinity() {
            var stats = new MovingAverage();
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

    }
}
