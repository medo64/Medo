using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Math;

namespace Tests;

[TestClass]
public class WelfordVariance_Tests {

    [TestMethod]
    public void WelfordVariance_Example1() {
        var stats = new WelfordVariance();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10, stats.Mean, 15);
        Assert.AreEqual(22.5, stats.Variance, 15);
        Assert.AreEqual(30, stats.SampleVariance, 15);
        Assert.AreEqual(4.743416490252569, stats.StandardDeviation, 15);
        Assert.AreEqual(5.477225575051661, stats.SampleStandardDeviation, 15);
        Assert.AreEqual(0.547722557505166, stats.RelativeStandardDeviation, 15);
    }

    [TestMethod]
    public void WelfordVariance_Example2() {
        var stats = new WelfordVariance();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000010, stats.Mean, 15);
        Assert.AreEqual(22.5, stats.Variance, 15);
        Assert.AreEqual(30, stats.SampleVariance, 15);
        Assert.AreEqual(4.743416490252569, stats.StandardDeviation, 15);
        Assert.AreEqual(5.477225575051661, stats.SampleStandardDeviation, 15);
        Assert.AreEqual(0.000000054772250, stats.RelativeStandardDeviation, 15);
    }

    [TestMethod]
    public void WelfordVariance_Example3() {
        var stats = new WelfordVariance();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000010, stats.Mean, 15);
        Assert.AreEqual(22.5, stats.Variance, 15);
        Assert.AreEqual(30, stats.SampleVariance, 15);
        Assert.AreEqual(4.743416490252569, stats.StandardDeviation, 15);
        Assert.AreEqual(5.477225575051661, stats.SampleStandardDeviation, 15);
        Assert.AreEqual(0.000000005477226, stats.RelativeStandardDeviation, 15);
    }

    [TestMethod]
    public void WelfordVariance_Example4() {
        var stats = new WelfordVariance();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Mean, 15);
        Assert.AreEqual(3.5, stats.Variance, 15);
        Assert.AreEqual(4.666666666666667, stats.SampleVariance, 15);
        Assert.AreEqual(1.870828693386971, stats.StandardDeviation, 15);
        Assert.AreEqual(2.160246899469287, stats.SampleStandardDeviation, 15);
        Assert.AreEqual(0.720082299823096, stats.RelativeStandardDeviation, 15);
    }

    [TestMethod]
    public void WelfordVariance_Example5() {
        var stats = new WelfordVariance(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Mean, 15);
        Assert.AreEqual(4.5, stats.Variance, 15);
        Assert.AreEqual(6, stats.SampleVariance, 15);
        Assert.AreEqual(2.121320343559642, stats.StandardDeviation, 15);
        Assert.AreEqual(2.449489742783178, stats.SampleStandardDeviation, 15);
        Assert.AreEqual(0.612372435695794, stats.RelativeStandardDeviation, 15);
    }

    [TestMethod]
    public void WelfordVariance_Example6() {
        var stats = new WelfordVariance();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Mean, 15);
        Assert.AreEqual(4, stats.Variance, 15);
        Assert.AreEqual(4.571428571428571, stats.SampleVariance, 15);
        Assert.AreEqual(2, stats.StandardDeviation, 15);
        Assert.AreEqual(2.138089935299395, stats.SampleStandardDeviation, 15);
        Assert.AreEqual(0.427617987059879, stats.RelativeStandardDeviation, 15);
    }

    [TestMethod]
    public void WelfordVariance_Example7() {
        var stats = new WelfordVariance();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7, stats.Mean, 15);
        Assert.AreEqual(8.9, stats.Variance, 15);
        Assert.AreEqual(9.368421052631579, stats.SampleVariance, 15);
        Assert.AreEqual(2.983286778035260, stats.StandardDeviation, 15);
        Assert.AreEqual(3.060787652326044, stats.SampleStandardDeviation, 15);
        Assert.AreEqual(0.437255378903721, stats.RelativeStandardDeviation, 15);
    }

    [TestMethod]
    public void WelfordVariance_Example8() {
        var stats = new WelfordVariance();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.2, stats.Mean, 15);
        Assert.AreEqual(4.425000000000004, stats.Variance, 15);
        Assert.AreEqual(5.900000000000006, stats.SampleVariance, 15);
        Assert.AreEqual(2.103568396796264, stats.StandardDeviation, 15);
        Assert.AreEqual(2.428991560298225, stats.SampleStandardDeviation, 15);
        Assert.AreEqual(0.046532405369698, stats.RelativeStandardDeviation, 15);
    }

    [TestMethod]
    public void WelfordVariance_Example9() {
        var stats = new WelfordVariance();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Mean, 15);
        Assert.AreEqual(8, stats.Variance, 15);
        Assert.AreEqual(10, stats.SampleVariance, 15);
        Assert.AreEqual(2.82842712474619, stats.StandardDeviation, 15);
        Assert.AreEqual(3.16227766016838, stats.SampleStandardDeviation, 15);
        Assert.AreEqual(3.16227766016838, stats.RelativeStandardDeviation, 15);
    }

    [TestMethod]
    public void WelfordVariance_Example10() {
        var stats = new WelfordVariance();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Mean, 15);
        Assert.AreEqual(0.666666666666667, stats.Variance, 15);
        Assert.AreEqual(1, stats.SampleVariance, 15);
        Assert.AreEqual(0.816496580927726, stats.StandardDeviation, 15);
        Assert.AreEqual(1, stats.SampleStandardDeviation, 15);
        Assert.AreEqual(double.PositiveInfinity, stats.RelativeStandardDeviation, 15);
    }

    [TestMethod]
    public void WelfordVariance_NoValue() {
        var stats = new WelfordVariance();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(double.NaN, stats.Mean);
        Assert.AreEqual(double.NaN, stats.Variance);
        Assert.AreEqual(double.NaN, stats.SampleVariance);
        Assert.AreEqual(double.NaN, stats.StandardDeviation);
        Assert.AreEqual(double.NaN, stats.SampleStandardDeviation);
        Assert.AreEqual(double.NaN, stats.RelativeStandardDeviation);
    }
    [TestMethod]
    public void WelfordVariance_OneValue() {
        var stats = new WelfordVariance();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(double.NaN, stats.Mean);
        Assert.AreEqual(double.NaN, stats.Variance);
        Assert.AreEqual(double.NaN, stats.SampleVariance);
        Assert.AreEqual(double.NaN, stats.StandardDeviation);
        Assert.AreEqual(double.NaN, stats.SampleStandardDeviation);
        Assert.AreEqual(double.NaN, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_TwoValues() {
        var stats = new WelfordVariance();
        stats.Add(1);
        stats.Add(2);
        Assert.AreEqual(2, stats.Count);
        Assert.AreEqual(1.5, stats.Mean, 15);
        Assert.AreEqual(0.25, stats.Variance, 15);
        Assert.AreEqual(0.5, stats.SampleVariance, 15);
        Assert.AreEqual(0.5, stats.StandardDeviation, 15);
        Assert.AreEqual(0.707106781186548, stats.SampleStandardDeviation, 15);
        Assert.AreEqual(0.471404520791032, stats.RelativeStandardDeviation, 15);
    }

    [TestMethod]
    public void WelfordVariance_NoInfinity() {
        var stats = new WelfordVariance();
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            stats.Add(double.NegativeInfinity);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            stats.Add(double.PositiveInfinity);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            stats.Add(double.NaN);
        });
    }

    [TestMethod]
    public void WelfordVariance_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WelfordVariance(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WelfordVariance();
            stats.AddRange(null);
        });
    }

}
