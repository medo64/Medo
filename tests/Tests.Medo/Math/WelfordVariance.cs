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
        Assert.AreEqual(10, stats.Mean, 0.0000001);
        Assert.AreEqual(22.5, stats.Variance, 0.0000001);
        Assert.AreEqual(30, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(4.7434165, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(5.4772256, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.5477226, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Example2() {
        var stats = new WelfordVariance();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000010, stats.Mean, 0.0000001);
        Assert.AreEqual(22.5, stats.Variance, 0.0000001);
        Assert.AreEqual(30, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(4.7434165, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(5.4772256, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.0000001, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Example3() {
        var stats = new WelfordVariance();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000010, stats.Mean, 0.0000001);
        Assert.AreEqual(22.5, stats.Variance, 0.0000001);
        Assert.AreEqual(30, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(4.7434165, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(5.4772256, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.0000001, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Example4() {
        var stats = new WelfordVariance();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Mean, 0.0000001);
        Assert.AreEqual(3.5, stats.Variance, 0.0000001);
        Assert.AreEqual(4.6666667, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(1.8708287, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(2.1602469, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.7200822, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Example5() {
        var stats = new WelfordVariance(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Mean, 0.0000001);
        Assert.AreEqual(4.5, stats.Variance, 0.0000001);
        Assert.AreEqual(6, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2.1213203, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(2.4494897, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.6123724, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Example6() {
        var stats = new WelfordVariance();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Mean, 0.0000001);
        Assert.AreEqual(4, stats.Variance, 0.0000001);
        Assert.AreEqual(4.5714286, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(2.1380899, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.4276180, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Example7() {
        var stats = new WelfordVariance();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7, stats.Mean, 0.0000001);
        Assert.AreEqual(8.9, stats.Variance, 0.0000001);
        Assert.AreEqual(9.3684211, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2.9832868, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(3.0607877, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.4372554, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Example8() {
        var stats = new WelfordVariance();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.2, stats.Mean, 0.0000001);
        Assert.AreEqual(4.4250000, stats.Variance, 0.0000001);
        Assert.AreEqual(5.9000000, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2.1035684, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(2.4289916, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.0465324, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Example9() {
        var stats = new WelfordVariance();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Mean, 0.0000001);
        Assert.AreEqual(8, stats.Variance, 0.0000001);
        Assert.AreEqual(10, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2.8284271, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(3.1622777, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(3.1622777, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Example10() {
        var stats = new WelfordVariance();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Mean, 0.0000001);
        Assert.AreEqual(0.6666667, stats.Variance, 0.0000001);
        Assert.AreEqual(1, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(0.8164966, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(1, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(double.PositiveInfinity, stats.RelativeStandardDeviation, 0.0000001);
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
        Assert.AreEqual(1.5, stats.Mean, 0.0000001);
        Assert.AreEqual(0.25, stats.Variance, 0.0000001);
        Assert.AreEqual(0.5, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(0.5, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(0.7071068, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.4714045, stats.RelativeStandardDeviation, 0.0000001);
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


#if NET7_0_OR_GREATER

[TestClass]
public class WelfordVariance_Decimal_Tests {

    [TestMethod]
    public void WelfordVariance_Decimal_Example1() {
        var stats = new WelfordVariance<decimal>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10, stats.Mean);
        Assert.AreEqual(22.5m, stats.Variance);
        Assert.AreEqual(30, stats.SampleVariance);
        Assert.AreEqual(4.74341649025257m, stats.StandardDeviation);
        Assert.AreEqual(5.47722557505166m, stats.SampleStandardDeviation);
        Assert.AreEqual(0.547722557505166m, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_Decimal_Example2() {
        var stats = new WelfordVariance<decimal>();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000010, stats.Mean);
        Assert.AreEqual(22.5m, stats.Variance);
        Assert.AreEqual(30, stats.SampleVariance);
        Assert.AreEqual(4.74341649025257m, stats.StandardDeviation);
        Assert.AreEqual(5.47722557505166m, stats.SampleStandardDeviation);
        Assert.AreEqual(0.0000000547722502732915726708m, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_Decimal_Example3() {
        var stats = new WelfordVariance<decimal>();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000010, stats.Mean);
        Assert.AreEqual(22.5m, stats.Variance);
        Assert.AreEqual(30, stats.SampleVariance);
        Assert.AreEqual(4.74341649025257m, stats.StandardDeviation);
        Assert.AreEqual(5.47722557505166m, stats.SampleStandardDeviation);
        Assert.AreEqual(0.0000000054772255202794047972m, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_Decimal_Example4() {
        var stats = new WelfordVariance<decimal>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Mean);
        Assert.AreEqual(3.5m, stats.Variance);
        Assert.AreEqual(4.6666666666666666666666666667m, stats.SampleVariance);
        Assert.AreEqual(1.87082869338697m, stats.StandardDeviation);
        Assert.AreEqual(2.16024689946929m, stats.SampleStandardDeviation);
        Assert.AreEqual(0.7200822998230966666666666667m, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_Decimal_Example5() {
        var stats = new WelfordVariance<decimal>(new decimal[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Mean);
        Assert.AreEqual(4.5m, stats.Variance);
        Assert.AreEqual(6, stats.SampleVariance);
        Assert.AreEqual(2.12132034355964m, stats.StandardDeviation);
        Assert.AreEqual(2.44948974278318m, stats.SampleStandardDeviation);
        Assert.AreEqual(0.612372435695795m, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_Decimal_Example6() {
        var stats = new WelfordVariance<decimal>();
        stats.AddRange(new decimal[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Mean);
        Assert.AreEqual(4, stats.Variance);
        Assert.AreEqual(4.5714285714285714285714285714m, stats.SampleVariance);
        Assert.AreEqual(2, stats.StandardDeviation);
        Assert.AreEqual(2.1380899352994m, stats.SampleStandardDeviation);
        Assert.AreEqual(0.42761798705988m, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_Decimal_Example7() {
        var stats = new WelfordVariance<decimal>();
        stats.AddRange(new decimal[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7.0000000000000000000000000001m, stats.Mean);
        Assert.AreEqual(8.9m, stats.Variance);
        Assert.AreEqual(9.368421052631578947368421053m, stats.SampleVariance);
        Assert.AreEqual(2.983286778035260m, stats.StandardDeviation);
        Assert.AreEqual(3.06078765232604m, stats.SampleStandardDeviation);
        Assert.AreEqual(0.43725537890372m, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_Decimal_Example8() {
        var stats = new WelfordVariance<decimal>();
        stats.AddRange(new decimal[] { 51.3m, 55.6m, 49.9m, 52.0m });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.2m, stats.Mean);
        Assert.AreEqual(4.4250000000000000000000000002m, stats.Variance);
        Assert.AreEqual(5.9000000000000000000000000003m, stats.SampleVariance);
        Assert.AreEqual(2.10356839679626m, stats.StandardDeviation);
        Assert.AreEqual(2.42899156029822m, stats.SampleStandardDeviation);
        Assert.AreEqual(0.0465324053696977011494252874m, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_Decimal_Example9() {
        var stats = new WelfordVariance<decimal>();
        stats.AddRange(new decimal[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Mean);
        Assert.AreEqual(8, stats.Variance);
        Assert.AreEqual(10, stats.SampleVariance);
        Assert.AreEqual(2.82842712474619m, stats.StandardDeviation);
        Assert.AreEqual(3.16227766016838m, stats.SampleStandardDeviation);
        Assert.AreEqual(3.16227766016838m, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_Decimal_Example10() {
        var stats = new WelfordVariance<decimal>();
        stats.AddRange(new decimal[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Mean);
        Assert.AreEqual(0.6666666666666666666666666667m, stats.Variance);
        Assert.AreEqual(1, stats.SampleVariance);
        Assert.AreEqual(0.816496580927726m, stats.StandardDeviation);
        Assert.AreEqual(1, stats.SampleStandardDeviation);
        Assert.AreEqual(0, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_Decimal_NoValue() {
        var stats = new WelfordVariance<decimal>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0, stats.Mean);
        Assert.AreEqual(0, stats.Variance);
        Assert.AreEqual(0, stats.SampleVariance);
        Assert.AreEqual(0, stats.StandardDeviation);
        Assert.AreEqual(0, stats.SampleStandardDeviation);
        Assert.AreEqual(0, stats.RelativeStandardDeviation);
    }
    [TestMethod]
    public void WelfordVariance_Decimal_OneValue() {
        var stats = new WelfordVariance<decimal>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(0, stats.Mean);
        Assert.AreEqual(0, stats.Variance);
        Assert.AreEqual(0, stats.SampleVariance);
        Assert.AreEqual(0, stats.StandardDeviation);
        Assert.AreEqual(0, stats.SampleStandardDeviation);
        Assert.AreEqual(0, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_Decimal_TwoValues() {
        var stats = new WelfordVariance<decimal>();
        stats.Add(1);
        stats.Add(2);
        Assert.AreEqual(2, stats.Count);
        Assert.AreEqual(1.5m, stats.Mean);
        Assert.AreEqual(0.25m, stats.Variance);
        Assert.AreEqual(0.5m, stats.SampleVariance);
        Assert.AreEqual(0.5m, stats.StandardDeviation);
        Assert.AreEqual(0.707106781186548m, stats.SampleStandardDeviation);
        Assert.AreEqual(0.471404520791032m, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_Decimal_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WelfordVariance<decimal>(null);
        });

        var stats = new WelfordVariance<decimal>();
        Assert.ThrowsException<ArgumentNullException>(delegate {
            stats.AddRange(null);
        });
    }

}


[TestClass]
public class WelfordVariance_Double_Tests {

    [TestMethod]
    public void WelfordVariance_Double_Example1() {
        var stats = new WelfordVariance<double>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10, stats.Mean, 0.0000001);
        Assert.AreEqual(22.5, stats.Variance, 0.0000001);
        Assert.AreEqual(30, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(4.7434165, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(5.4772256, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.5477226, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Double_Example2() {
        var stats = new WelfordVariance<double>();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000010, stats.Mean, 0.0000001);
        Assert.AreEqual(22.5, stats.Variance, 0.0000001);
        Assert.AreEqual(30, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(4.7434165, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(5.4772256, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.0000001, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Double_Example3() {
        var stats = new WelfordVariance<double>();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000010, stats.Mean, 0.0000001);
        Assert.AreEqual(22.5, stats.Variance, 0.0000001);
        Assert.AreEqual(30, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(4.7434165, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(5.4772256, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.0000000, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Double_Example4() {
        var stats = new WelfordVariance<double>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Mean, 0.0000001);
        Assert.AreEqual(3.5, stats.Variance, 0.0000001);
        Assert.AreEqual(4.6666667, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(1.8708287, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(2.1602469, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.7200823, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Double_Example5() {
        var stats = new WelfordVariance<double>(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Mean, 0.0000001);
        Assert.AreEqual(4.5, stats.Variance, 0.0000001);
        Assert.AreEqual(6, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2.1213203, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(2.4494897, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.6123724, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Double_Example6() {
        var stats = new WelfordVariance<double>();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Mean, 0.0000001);
        Assert.AreEqual(4, stats.Variance, 0.0000001);
        Assert.AreEqual(4.5714286, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(2.1380900, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.4276180, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Double_Example7() {
        var stats = new WelfordVariance<double>();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7, stats.Mean, 0.0000001);
        Assert.AreEqual(8.9, stats.Variance, 0.0000001);
        Assert.AreEqual(9.3684211, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2.9832868, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(3.0607877, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.4372554, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Double_Example8() {
        var stats = new WelfordVariance<double>();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.2, stats.Mean, 0.0000001);
        Assert.AreEqual(4.4250000, stats.Variance, 0.0000001);
        Assert.AreEqual(5.9000000, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2.1035684, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(2.4289916, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.0465324, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Double_Example9() {
        var stats = new WelfordVariance<double>();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Mean, 0.0000001);
        Assert.AreEqual(8, stats.Variance, 0.0000001);
        Assert.AreEqual(10, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2.8284271, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(3.1622777, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(3.1622777, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Double_Example10() {
        var stats = new WelfordVariance<double>();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Mean, 0.0000001);
        Assert.AreEqual(0.6666667, stats.Variance, 0.0000001);
        Assert.AreEqual(1, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(0.8164966, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(1, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Double_NoValue() {
        var stats = new WelfordVariance<double>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0, stats.Mean);
        Assert.AreEqual(0, stats.Variance);
        Assert.AreEqual(0, stats.SampleVariance);
        Assert.AreEqual(0, stats.StandardDeviation);
        Assert.AreEqual(0, stats.SampleStandardDeviation);
        Assert.AreEqual(0, stats.RelativeStandardDeviation);
    }
    [TestMethod]
    public void WelfordVariance_Double_OneValue() {
        var stats = new WelfordVariance<double>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(0, stats.Mean);
        Assert.AreEqual(0, stats.Variance);
        Assert.AreEqual(0, stats.SampleVariance);
        Assert.AreEqual(0, stats.StandardDeviation);
        Assert.AreEqual(0, stats.SampleStandardDeviation);
        Assert.AreEqual(0, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_Double_TwoValues() {
        var stats = new WelfordVariance<double>();
        stats.Add(1);
        stats.Add(2);
        Assert.AreEqual(2, stats.Count);
        Assert.AreEqual(1.5, stats.Mean, 0.0000001);
        Assert.AreEqual(0.25, stats.Variance, 0.0000001);
        Assert.AreEqual(0.5, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(0.5, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(0.7071068, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.4714045, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Double_NoInfinity() {
        var stats = new WelfordVariance<double>();
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
    public void WelfordVariance_Double_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WelfordVariance<double>(null);
        });

        var stats = new WelfordVariance<double>();
        Assert.ThrowsException<ArgumentNullException>(delegate {
            stats.AddRange(null);
        });
    }

}


[TestClass]
public class WelfordVariance_Single_Tests {

    [TestMethod]
    public void WelfordVariance_Single_Example1() {
        var stats = new WelfordVariance<float>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10, stats.Mean, 0.0000001);
        Assert.AreEqual(22.5, stats.Variance, 0.0000001);
        Assert.AreEqual(30, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(4.7434163, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(5.4772258, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.5477226, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Single_Example2() {
        var stats = new WelfordVariance<float>();
        stats.Add(100004);
        stats.Add(100007);
        stats.Add(100013);
        stats.Add(100016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100010, stats.Mean, 0.0000001);
        Assert.AreEqual(22.5, stats.Variance, 0.0000001);
        Assert.AreEqual(30, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(4.7434163f, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(5.4772257f, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.0000547f, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Single_Example3() {
        var stats = new WelfordVariance<float>();
        stats.Add(10000004);
        stats.Add(10000007);
        stats.Add(10000013);
        stats.Add(10000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10000010, stats.Mean, 0.0000001);
        Assert.AreEqual(21.5, stats.Variance, 0.0000001);
        Assert.AreEqual(28.666666f, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(4.6368094f, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(5.3541260f, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.0000005f, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Single_Example4() {
        var stats = new WelfordVariance<float>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Mean, 0.0000001);
        Assert.AreEqual(3.5, stats.Variance, 0.0000001);
        Assert.AreEqual(4.6666666f, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(1.8708287f, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(2.1602469f, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.7200823f, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Single_Example5() {
        var stats = new WelfordVariance<float>(new float[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Mean, 0.0000001);
        Assert.AreEqual(4.5, stats.Variance, 0.0000001);
        Assert.AreEqual(6, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2.1213203f, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(2.4494898f, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.6123724f, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Single_Example6() {
        var stats = new WelfordVariance<float>();
        stats.AddRange(new float[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Mean, 0.0000001);
        Assert.AreEqual(4, stats.Variance, 0.0000001);
        Assert.AreEqual(4.5714288f, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(2.1380899f, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.4276180f, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Single_Example7() {
        var stats = new WelfordVariance<float>();
        stats.AddRange(new float[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7.0000005f, stats.Mean, 0.0000001);
        Assert.AreEqual(8.8999996f, stats.Variance, 0.0000001);
        Assert.AreEqual(9.3684206f, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2.9832866f, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(3.0607877f, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.4372554f, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Single_Example8() {
        var stats = new WelfordVariance<float>();
        stats.AddRange(new float[] { 51.3f, 55.6f, 49.9f, 52.0f });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.2000008f, stats.Mean, 0.0000001);
        Assert.AreEqual(4.4249969f, stats.Variance, 0.0000001);
        Assert.AreEqual(5.8999958f, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2.1035676f, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(2.4289906f, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.0465324f, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Single_Example9() {
        var stats = new WelfordVariance<float>();
        stats.AddRange(new float[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Mean, 0.0000001);
        Assert.AreEqual(8, stats.Variance, 0.0000001);
        Assert.AreEqual(10, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(2.8284271, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(3.1622777, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(3.1622777, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Single_Example10() {
        var stats = new WelfordVariance<float>();
        stats.AddRange(new float[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Mean, 0.0000001);
        Assert.AreEqual(0.6666667, stats.Variance, 0.0000001);
        Assert.AreEqual(1, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(0.8164966, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(1, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Single_NoValue() {
        var stats = new WelfordVariance<float>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0, stats.Mean);
        Assert.AreEqual(0, stats.Variance);
        Assert.AreEqual(0, stats.SampleVariance);
        Assert.AreEqual(0, stats.StandardDeviation);
        Assert.AreEqual(0, stats.SampleStandardDeviation);
        Assert.AreEqual(0, stats.RelativeStandardDeviation);
    }
    [TestMethod]
    public void WelfordVariance_Single_OneValue() {
        var stats = new WelfordVariance<float>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(0, stats.Mean);
        Assert.AreEqual(0, stats.Variance);
        Assert.AreEqual(0, stats.SampleVariance);
        Assert.AreEqual(0, stats.StandardDeviation);
        Assert.AreEqual(0, stats.SampleStandardDeviation);
        Assert.AreEqual(0, stats.RelativeStandardDeviation);
    }

    [TestMethod]
    public void WelfordVariance_Single_TwoValues() {
        var stats = new WelfordVariance<float>();
        stats.Add(1);
        stats.Add(2);
        Assert.AreEqual(2, stats.Count);
        Assert.AreEqual(1.5, stats.Mean, 0.0000001);
        Assert.AreEqual(0.25, stats.Variance, 0.0000001);
        Assert.AreEqual(0.5, stats.SampleVariance, 0.0000001);
        Assert.AreEqual(0.5, stats.StandardDeviation, 0.0000001);
        Assert.AreEqual(0.7071068, stats.SampleStandardDeviation, 0.0000001);
        Assert.AreEqual(0.4714045, stats.RelativeStandardDeviation, 0.0000001);
    }

    [TestMethod]
    public void WelfordVariance_Single_NoInfinity() {
        var stats = new WelfordVariance<float>();
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            stats.Add(float.NegativeInfinity);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            stats.Add(float.PositiveInfinity);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            stats.Add(float.NaN);
        });
    }

    [TestMethod]
    public void WelfordVariance_Single_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WelfordVariance<float>(null);
        });

        var stats = new WelfordVariance<float>();
        Assert.ThrowsException<ArgumentNullException>(delegate {
            stats.AddRange(null);
        });
    }

}

#endif
