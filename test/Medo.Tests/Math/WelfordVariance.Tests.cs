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
            Assert.Equal(10, stats.Mean);
            Assert.Equal(22.5, stats.Variance);
            Assert.Equal(30, stats.SampleVariance);
            Assert.Equal(4.743416490252569, stats.StandardDeviation);
            Assert.Equal(5.477225575051661, stats.SampleStandardDeviation);
        }

        [Fact(DisplayName = "WelfordVariance: Example (2)")]
        public void Example2() {
            var stats = new WelfordVariance();
            stats.Add(100000004);
            stats.Add(100000007);
            stats.Add(100000013);
            stats.Add(100000016);
            Assert.Equal(100000010, stats.Mean);
            Assert.Equal(22.5, stats.Variance);
            Assert.Equal(30, stats.SampleVariance);
            Assert.Equal(4.743416490252569, stats.StandardDeviation);
            Assert.Equal(5.477225575051661, stats.SampleStandardDeviation);
        }

        [Fact(DisplayName = "WelfordVariance: Example (3)")]
        public void Example3() {
            var stats = new WelfordVariance();
            stats.Add(1000000004);
            stats.Add(1000000007);
            stats.Add(1000000013);
            stats.Add(1000000016);
            Assert.Equal(1000000010, stats.Mean);
            Assert.Equal(22.5, stats.Variance);
            Assert.Equal(30, stats.SampleVariance);
            Assert.Equal(4.743416490252569, stats.StandardDeviation);
            Assert.Equal(5.477225575051661, stats.SampleStandardDeviation);
        }

        [Fact(DisplayName = "WelfordVariance: Example (4)")]
        public void Example4() {
            var stats = new WelfordVariance();
            stats.Add(6);
            stats.Add(2);
            stats.Add(3);
            stats.Add(1);
            Assert.Equal(3, stats.Mean);
            Assert.Equal(3.5, stats.Variance);
            Assert.Equal(4.666666666666667, stats.SampleVariance);
            Assert.Equal(1.8708286933869707, stats.StandardDeviation);
            Assert.Equal(2.160246899469287, stats.SampleStandardDeviation);
        }

        [Fact(DisplayName = "WelfordVariance: Example (5)")]
        public void Example5() {
            var stats = new WelfordVariance(new double[] { 2, 2, 5, 7 });
            Assert.Equal(4, stats.Mean);
            Assert.Equal(4.5, stats.Variance);
            Assert.Equal(6, stats.SampleVariance);
            Assert.Equal(2.1213203435596424, stats.StandardDeviation);
            Assert.Equal(2.449489742783178, stats.SampleStandardDeviation);
        }

        [Fact(DisplayName = "WelfordVariance: Example (6)")]
        public void Example6() {
            var stats = new WelfordVariance();
            stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
            Assert.Equal(5, stats.Mean);
            Assert.Equal(4, stats.Variance);
            Assert.Equal(4.571428571428571, stats.SampleVariance);
            Assert.Equal(2, stats.StandardDeviation);
            Assert.Equal(2.138089935299395, stats.SampleStandardDeviation);
        }

        [Fact(DisplayName = "WelfordVariance: Example (7)")]
        public void Example7() {
            var stats = new WelfordVariance();
            stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
            Assert.Equal(7, stats.Mean);
            Assert.Equal(8.9, stats.Variance);
            Assert.Equal(9.368421052631579, stats.SampleVariance);
            Assert.Equal(2.9832867780352594, stats.StandardDeviation);
            Assert.Equal(3.0607876523260447, stats.SampleStandardDeviation);
        }

        [Fact(DisplayName = "WelfordVariance: No values")]
        public void NoValue() {
            var stats = new WelfordVariance();
            Assert.Equal(double.NaN, stats.Mean);
            Assert.Equal(double.NaN, stats.Variance);
            Assert.Equal(double.NaN, stats.SampleVariance);
            Assert.Equal(double.NaN, stats.StandardDeviation);
            Assert.Equal(double.NaN, stats.SampleStandardDeviation);
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
        }

        [Fact(DisplayName = "WelfordVariance: Two values")]
        public void TwoValues() {
            var stats = new WelfordVariance();
            stats.Add(1);
            stats.Add(2);
            Assert.Equal(1.5, stats.Mean);
            Assert.Equal(0.25, stats.Variance);
            Assert.Equal(0.5, stats.SampleVariance);
            Assert.Equal(0.5, stats.StandardDeviation);
            Assert.Equal(0.7071067811865476, stats.SampleStandardDeviation);
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

    }
}
