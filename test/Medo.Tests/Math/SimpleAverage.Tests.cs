using System;
using Xunit;

namespace Medo.Tests.Math.SimpleAverage {
    using Medo.Math;

    public class Tests {

        [Fact(DisplayName = "SimpleAverage: Example (1)")]
        public void Example1() {
            var stats = new SimpleAverage();
            stats.Add(4);
            stats.Add(7);
            stats.Add(13);
            stats.Add(16);
            Assert.Equal(10, stats.Average, 15);
        }

        [Fact(DisplayName = "SimpleAverage: Example (2)")]
        public void Example2() {
            var stats = new SimpleAverage();
            stats.Add(100000004);
            stats.Add(100000007);
            stats.Add(100000013);
            stats.Add(100000016);
            Assert.Equal(100000010, stats.Average, 15);
        }

        [Fact(DisplayName = "SimpleAverage: Example (3)")]
        public void Example3() {
            var stats = new SimpleAverage();
            stats.Add(1000000004);
            stats.Add(1000000007);
            stats.Add(1000000013);
            stats.Add(1000000016);
            Assert.Equal(1000000010, stats.Average, 15);
        }

        [Fact(DisplayName = "SimpleAverage: Example (4)")]
        public void Example4() {
            var stats = new SimpleAverage();
            stats.Add(6);
            stats.Add(2);
            stats.Add(3);
            stats.Add(1);
            Assert.Equal(3, stats.Average, 15);
        }

        [Fact(DisplayName = "SimpleAverage: Example (5)")]
        public void Example5() {
            var stats = new SimpleAverage(new double[] { 2, 2, 5, 7 });
            Assert.Equal(4, stats.Average, 15);
        }

        [Fact(DisplayName = "SimpleAverage: Example (6)")]
        public void Example6() {
            var stats = new SimpleAverage();
            stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
            Assert.Equal(5, stats.Average, 15);
        }

        [Fact(DisplayName = "SimpleAverage: Example (7)")]
        public void Example7() {
            var stats = new SimpleAverage();
            stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
            Assert.Equal(7, stats.Average, 15);
        }

        [Fact(DisplayName = "SimpleAverage: Example (8)")]
        public void Example8() {
            var stats = new SimpleAverage();
            stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
            Assert.Equal(52.2, stats.Average, 15);
        }

        [Fact(DisplayName = "SimpleAverage: Example (9)")]
        public void Example9() {
            var stats = new SimpleAverage();
            stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
            Assert.Equal(-1, stats.Average, 15);
        }

        [Fact(DisplayName = "SimpleAverage: Example (10)")]
        public void Example10() {
            var stats = new SimpleAverage();
            stats.AddRange(new double[] { -1, 0, 1 });
            Assert.Equal(0, stats.Average, 15);
        }

        [Fact(DisplayName = "SimpleAverage: No values")]
        public void NoValue() {
            var stats = new SimpleAverage();
            Assert.Equal(double.NaN, stats.Average);
        }

        [Fact(DisplayName = "SimpleAverage: One value")]
        public void OneValue() {
            var stats = new SimpleAverage();
            stats.Add(1);
            Assert.Equal(1, stats.Average);
        }

        [Fact(DisplayName = "SimpleAverage: No infinities")]
        public void NoInfinity() {
            var stats = new SimpleAverage();
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

        [Fact(DisplayName = "SimpleAverage: No null collection")]
        public void NoNullCollection() {
            Assert.Throws<ArgumentNullException>(delegate {
                var stats = new SimpleAverage(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                var stats = new SimpleAverage();
                stats.AddRange(null);
            });
        }

    }
}
