using System;
using Xunit;

namespace Medo.Tests.Math.WelfordVariance {
    using Medo.Math;

    public class Tests {

        [Fact(DisplayName = "WelfordVariance: Example (1)")]
        public void Example1() {
            var stats = new WelfordVariance();
            stats.Add(4);
            stats.Add(7);
            stats.Add(13);
            stats.Add(16);
            Assert.Equal(10, stats.Mean, 15);
            Assert.Equal(22.5, stats.Variance, 15);
            Assert.Equal(30, stats.SampleVariance, 15);
            Assert.Equal(4.743416490252569, stats.StandardDeviation, 15);
            Assert.Equal(5.477225575051661, stats.SampleStandardDeviation, 15);
            Assert.Equal(0.547722557505166, stats.RelativeStandardDeviation, 15);
        }

        [Fact(DisplayName = "WelfordVariance: Example (2)")]
        public void Example2() {
            var stats = new WelfordVariance();
            stats.Add(100000004);
            stats.Add(100000007);
            stats.Add(100000013);
            stats.Add(100000016);
            Assert.Equal(100000010, stats.Mean, 15);
            Assert.Equal(22.5, stats.Variance, 15);
            Assert.Equal(30, stats.SampleVariance, 15);
            Assert.Equal(4.743416490252569, stats.StandardDeviation, 15);
            Assert.Equal(5.477225575051661, stats.SampleStandardDeviation, 15);
            Assert.Equal(0.000000054772250, stats.RelativeStandardDeviation, 15);
        }

        [Fact(DisplayName = "WelfordVariance: Example (3)")]
        public void Example3() {
            var stats = new WelfordVariance();
            stats.Add(1000000004);
            stats.Add(1000000007);
            stats.Add(1000000013);
            stats.Add(1000000016);
            Assert.Equal(1000000010, stats.Mean, 15);
            Assert.Equal(22.5, stats.Variance, 15);
            Assert.Equal(30, stats.SampleVariance, 15);
            Assert.Equal(4.743416490252569, stats.StandardDeviation, 15);
            Assert.Equal(5.477225575051661, stats.SampleStandardDeviation, 15);
            Assert.Equal(0.000000005477226, stats.RelativeStandardDeviation, 15);
        }

        [Fact(DisplayName = "WelfordVariance: Example (4)")]
        public void Example4() {
            var stats = new WelfordVariance();
            stats.Add(6);
            stats.Add(2);
            stats.Add(3);
            stats.Add(1);
            Assert.Equal(3, stats.Mean, 15);
            Assert.Equal(3.5, stats.Variance, 15);
            Assert.Equal(4.666666666666667, stats.SampleVariance, 15);
            Assert.Equal(1.870828693386971, stats.StandardDeviation, 15);
            Assert.Equal(2.160246899469287, stats.SampleStandardDeviation, 15);
            Assert.Equal(0.720082299823096, stats.RelativeStandardDeviation, 15);
        }

        [Fact(DisplayName = "WelfordVariance: Example (5)")]
        public void Example5() {
            var stats = new WelfordVariance(new double[] { 2, 2, 5, 7 });
            Assert.Equal(4, stats.Mean, 15);
            Assert.Equal(4.5, stats.Variance, 15);
            Assert.Equal(6, stats.SampleVariance, 15);
            Assert.Equal(2.121320343559642, stats.StandardDeviation, 15);
            Assert.Equal(2.449489742783178, stats.SampleStandardDeviation, 15);
            Assert.Equal(0.612372435695794, stats.RelativeStandardDeviation, 15);
        }

        [Fact(DisplayName = "WelfordVariance: Example (6)")]
        public void Example6() {
            var stats = new WelfordVariance();
            stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
            Assert.Equal(5, stats.Mean, 15);
            Assert.Equal(4, stats.Variance, 15);
            Assert.Equal(4.571428571428571, stats.SampleVariance, 15);
            Assert.Equal(2, stats.StandardDeviation, 15);
            Assert.Equal(2.138089935299395, stats.SampleStandardDeviation, 15);
            Assert.Equal(0.427617987059879, stats.RelativeStandardDeviation, 15);
        }

        [Fact(DisplayName = "WelfordVariance: Example (7)")]
        public void Example7() {
            var stats = new WelfordVariance();
            stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
            Assert.Equal(7, stats.Mean, 15);
            Assert.Equal(8.9, stats.Variance, 15);
            Assert.Equal(9.368421052631579, stats.SampleVariance, 15);
            Assert.Equal(2.983286778035260, stats.StandardDeviation, 15);
            Assert.Equal(3.060787652326044, stats.SampleStandardDeviation, 15);
            Assert.Equal(0.437255378903721, stats.RelativeStandardDeviation, 15);
        }

        [Fact(DisplayName = "WelfordVariance: Example (8)")]
        public void Example8() {
            var stats = new WelfordVariance();
            stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
            Assert.Equal(52.2, stats.Mean, 15);
            Assert.Equal(4.425000000000004, stats.Variance, 15);
            Assert.Equal(5.900000000000006, stats.SampleVariance, 15);
            Assert.Equal(2.103568396796264, stats.StandardDeviation, 15);
            Assert.Equal(2.428991560298225, stats.SampleStandardDeviation, 15);
            Assert.Equal(0.046532405369698, stats.RelativeStandardDeviation, 15);
        }

        [Fact(DisplayName = "WelfordVariance: Example (9)")]
        public void Example9() {
            var stats = new WelfordVariance();
            stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
            Assert.Equal(-1, stats.Mean, 15);
            Assert.Equal(8, stats.Variance, 15);
            Assert.Equal(10, stats.SampleVariance, 15);
            Assert.Equal(2.82842712474619, stats.StandardDeviation, 15);
            Assert.Equal(3.16227766016838, stats.SampleStandardDeviation, 15);
            Assert.Equal(3.16227766016838, stats.RelativeStandardDeviation, 15);
        }

        [Fact(DisplayName = "WelfordVariance: Example (10)")]
        public void Example10() {
            var stats = new WelfordVariance();
            stats.AddRange(new double[] { -1, 0, 1 });
            Assert.Equal(0, stats.Mean, 15);
            Assert.Equal(0.666666666666667, stats.Variance, 15);
            Assert.Equal(1, stats.SampleVariance, 15);
            Assert.Equal(0.816496580927726, stats.StandardDeviation, 15);
            Assert.Equal(1, stats.SampleStandardDeviation, 15);
            Assert.Equal(double.PositiveInfinity, stats.RelativeStandardDeviation, 15);
        }

        [Fact(DisplayName = "WelfordVariance: No values")]
        public void NoValue() {
            var stats = new WelfordVariance();
            Assert.Equal(double.NaN, stats.Mean);
            Assert.Equal(double.NaN, stats.Variance);
            Assert.Equal(double.NaN, stats.SampleVariance);
            Assert.Equal(double.NaN, stats.StandardDeviation);
            Assert.Equal(double.NaN, stats.SampleStandardDeviation);
            Assert.Equal(double.NaN, stats.RelativeStandardDeviation);
        }

        [Fact(DisplayName = "WelfordVariance: One value")]
        public void OneValue() {
            var stats = new WelfordVariance();
            stats.Add(1);
            Assert.Equal(double.NaN, stats.Mean);
            Assert.Equal(double.NaN, stats.Variance);
            Assert.Equal(double.NaN, stats.SampleVariance);
            Assert.Equal(double.NaN, stats.StandardDeviation);
            Assert.Equal(double.NaN, stats.SampleStandardDeviation);
            Assert.Equal(double.NaN, stats.RelativeStandardDeviation);
        }

        [Fact(DisplayName = "WelfordVariance: Two values")]
        public void TwoValues() {
            var stats = new WelfordVariance();
            stats.Add(1);
            stats.Add(2);
            Assert.Equal(1.5, stats.Mean, 15);
            Assert.Equal(0.25, stats.Variance, 15);
            Assert.Equal(0.5, stats.SampleVariance, 15);
            Assert.Equal(0.5, stats.StandardDeviation, 15);
            Assert.Equal(0.707106781186548, stats.SampleStandardDeviation, 15);
            Assert.Equal(0.471404520791032, stats.RelativeStandardDeviation, 15);
        }

        [Fact(DisplayName = "WelfordVariance: No infinities")]
        public void NoInfinity() {
            var stats = new WelfordVariance();
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

        [Fact(DisplayName = "WelfordVariance: No null collection")]
        public void NoNullCollection() {
            Assert.Throws<ArgumentNullException>(delegate {
                var stats = new WelfordVariance(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                var stats = new WelfordVariance();
                stats.AddRange(null);
            });
        }

    }
}
