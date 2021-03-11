using System;
using Xunit;

namespace Medo.Tests.Math.ExponentialMovingAverage {
    using Medo.Math;

    public class Tests {

        [Fact(DisplayName = "ExponentialMovingAverage: Example (1)")]
        public void Example1() {
            var stats = new ExponentialMovingAverage();
            stats.Add(4);
            stats.Add(7);
            stats.Add(13);
            stats.Add(16);
            Assert.Equal(7.885800150262959, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (2)")]
        public void Example2() {
            var stats = new ExponentialMovingAverage();
            stats.Add(100000004);
            stats.Add(100000007);
            stats.Add(100000013);
            stats.Add(100000016);
            Assert.Equal(100000007.88580015, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (3)")]
        public void Example3() {
            var stats = new ExponentialMovingAverage();
            stats.Add(1000000004);
            stats.Add(1000000007);
            stats.Add(1000000013);
            stats.Add(1000000016);
            Assert.Equal(1000000007.8858002, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (4)")]
        public void Example4() {
            var stats = new ExponentialMovingAverage();
            stats.Add(6);
            stats.Add(2);
            stats.Add(3);
            stats.Add(1);
            Assert.Equal(4.157776108189332, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (5)")]
        public void Example5() {
            var stats = new ExponentialMovingAverage(new double[] { 2, 2, 5, 7 });
            Assert.Equal(3.355371900826446, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (6)")]
        public void Example6() {
            var stats = new ExponentialMovingAverage();
            stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
            Assert.Equal(5.085784386045568, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (7)")]
        public void Example7() {
            var stats = new ExponentialMovingAverage();
            stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
            Assert.Equal(6.929979179740536, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (8)")]
        public void Example8() {
            var stats = new ExponentialMovingAverage();
            stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
            Assert.Equal(51.74237415477084, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (9)")]
        public void Example9() {
            var stats = new ExponentialMovingAverage();
            stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
            Assert.Equal(-1.966873847414794, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (10)")]
        public void Example10() {
            var stats = new ExponentialMovingAverage();
            stats.AddRange(new double[] { -1, 0, 1 });
            Assert.Equal(-0.487603305785124, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (11)")]
        public void Example11() {
            var stats = new ExponentialMovingAverage(3);
            stats.Add(4);
            stats.Add(7);
            stats.Add(13);
            stats.Add(16);
            Assert.Equal(12.625, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (12)")]
        public void Example12() {
            var stats = new ExponentialMovingAverage(3);
            stats.Add(100000004);
            stats.Add(100000007);
            stats.Add(100000013);
            stats.Add(100000016);
            Assert.Equal(100000012.625, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (13)")]
        public void Example13() {
            var stats = new ExponentialMovingAverage(3);
            stats.Add(1000000004);
            stats.Add(1000000007);
            stats.Add(1000000013);
            stats.Add(1000000016);
            Assert.Equal(1000000012.625, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (14)")]
        public void Example14() {
            var stats = new ExponentialMovingAverage(3);
            stats.Add(6);
            stats.Add(2);
            stats.Add(3);
            stats.Add(1);
            Assert.Equal(2.25, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (15)")]
        public void Example15() {
            var stats = new ExponentialMovingAverage(3, new double[] { 2, 2, 5, 7 });
            Assert.Equal(5.25, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (16)")]
        public void Example16() {
            var stats = new ExponentialMovingAverage(3);
            stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
            Assert.Equal(7.421875, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (17)")]
        public void Example17() {
            var stats = new ExponentialMovingAverage(3);
            stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
            Assert.Equal(6.044046401977539, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (18)")]
        public void Example18() {
            var stats = new ExponentialMovingAverage(3);
            stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
            Assert.Equal(51.8375, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (19)")]
        public void Example19() {
            var stats = new ExponentialMovingAverage(3);
            stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
            Assert.Equal(1.125, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (20)")]
        public void Example20() {
            var stats = new ExponentialMovingAverage(3);
            stats.AddRange(new double[] { -1, 0, 1 });
            Assert.Equal(0.25, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (21)")]
        public void Example21() {
            var stats = new ExponentialMovingAverage(0.1);
            stats.Add(4);
            stats.Add(7);
            stats.Add(13);
            stats.Add(16);
            Assert.Equal(6.253, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (22)")]
        public void Example22() {
            var stats = new ExponentialMovingAverage(0.1);
            stats.Add(100000004);
            stats.Add(100000007);
            stats.Add(100000013);
            stats.Add(100000016);
            Assert.Equal(100000006.253, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (23)")]
        public void Example23() {
            var stats = new ExponentialMovingAverage(0.1);
            stats.Add(1000000004);
            stats.Add(1000000007);
            stats.Add(1000000013);
            stats.Add(1000000016);
            Assert.Equal(1000000006.253, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (24)")]
        public void Example24() {
            var stats = new ExponentialMovingAverage(0.1);
            stats.Add(6);
            stats.Add(2);
            stats.Add(3);
            stats.Add(1);
            Assert.Equal(4.906, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (25)")]
        public void Example25() {
            var stats = new ExponentialMovingAverage(0.1, new double[] { 2, 2, 5, 7 });
            Assert.Equal(2.77, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (26)")]
        public void Example26() {
            var stats = new ExponentialMovingAverage(0.1);
            stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
            Assert.Equal(3.9673062, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (27)")]
        public void Example27() {
            var stats = new ExponentialMovingAverage(0.1);
            stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
            Assert.Equal(7.239007925922224, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (28)")]
        public void Example28() {
            var stats = new ExponentialMovingAverage(0.1);
            stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
            Assert.Equal(51.592299999999994, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (29)")]
        public void Example29() {
            var stats = new ExponentialMovingAverage(0.1);
            stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
            Assert.Equal(-3.1902, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (30)")]
        public void Example30() {
            var stats = new ExponentialMovingAverage(0.1);
            stats.AddRange(new double[] { -1, 0, 1 });
            Assert.Equal(-0.71, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (31)")]
        public void Example31() {
            var stats = new ExponentialMovingAverage(0.9);
            stats.Add(4);
            stats.Add(7);
            stats.Add(13);
            stats.Add(16);
            Assert.Equal(15.637, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (32)")]
        public void Example32() {
            var stats = new ExponentialMovingAverage(0.9);
            stats.Add(100000004);
            stats.Add(100000007);
            stats.Add(100000013);
            stats.Add(100000016);
            Assert.Equal(100000015.63700001, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (33)")]
        public void Example33() {
            var stats = new ExponentialMovingAverage(0.9);
            stats.Add(1000000004);
            stats.Add(1000000007);
            stats.Add(1000000013);
            stats.Add(1000000016);
            Assert.Equal(1000000015.637, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (34)")]
        public void Example34() {
            var stats = new ExponentialMovingAverage(0.9);
            stats.Add(6);
            stats.Add(2);
            stats.Add(3);
            stats.Add(1);
            Assert.Equal(1.194, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (35)")]
        public void Example35() {
            var stats = new ExponentialMovingAverage(0.9, new double[] { 2, 2, 5, 7 });
            Assert.Equal(6.77, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (36)")]
        public void Example36() {
            var stats = new ExponentialMovingAverage(0.9);
            stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
            Assert.Equal(8.7798998, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (37)")]
        public void Example37() {
            var stats = new ExponentialMovingAverage(0.9);
            stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
            Assert.Equal(4.473041622661694, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (38)")]
        public void Example38() {
            var stats = new ExponentialMovingAverage(0.9);
            stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
            Assert.Equal(51.8427, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (39)")]
        public void Example39() {
            var stats = new ExponentialMovingAverage(0.9);
            stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
            Assert.Equal(2.7778, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: Example (40)")]
        public void Example40() {
            var stats = new ExponentialMovingAverage(0.9);
            stats.AddRange(new double[] { -1, 0, 1 });
            Assert.Equal(0.89, stats.Average, 15);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: No values")]
        public void NoValue() {
            var stats = new ExponentialMovingAverage();
            Assert.Equal(double.NaN, stats.Average);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: One value")]
        public void OneValue() {
            var stats = new ExponentialMovingAverage();
            stats.Add(1);
            Assert.Equal(1, stats.Average);
        }

        [Fact(DisplayName = "ExponentialMovingAverage: No infinities")]
        public void NoInfinity() {
            var stats = new ExponentialMovingAverage();
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

        [Fact(DisplayName = "ExponentialMovingAverage: No null collection")]
        public void NoNullCollection() {
            Assert.Throws<ArgumentNullException>(delegate {
                var stats = new ExponentialMovingAverage(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                var stats = new ExponentialMovingAverage(10, null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                var stats = new ExponentialMovingAverage(0.0, null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                var stats = new ExponentialMovingAverage();
                stats.AddRange(null);
            });
        }

        [Fact(DisplayName = "ExponentialMovingAverage: No count out of range")]
        public void NoCountOutOfRange() {
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var stats = new ExponentialMovingAverage(0);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var stats = new ExponentialMovingAverage(0, new double[] { 0 });
            });
        }

        [Fact(DisplayName = "ExponentialMovingAverage: No smoothing out of range")]
        public void NoSmoothingOutOfRange() {
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var stats = new ExponentialMovingAverage(0 - double.Epsilon);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var stats = new ExponentialMovingAverage(1.001);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var stats = new ExponentialMovingAverage(0 - double.Epsilon, new double[] { 0 });
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var stats = new ExponentialMovingAverage(1.001, new double[] { 0 });
            });
        }

    }
}
