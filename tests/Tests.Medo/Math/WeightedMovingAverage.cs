using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Math;

namespace Tests;

#pragma warning disable CS0618 // Type or member is obsolete

[TestClass]
public class WeightedMovingAverage_Tests {

    [TestMethod]
    public void WeightedMovingAverage_Example1() {
        var stats = new WeightedMovingAverage();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(12.1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example2() {
        var stats = new WeightedMovingAverage();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000012.1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example3() {
        var stats = new WeightedMovingAverage();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000012.1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example4() {
        var stats = new WeightedMovingAverage();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.3, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example5() {
        var stats = new WeightedMovingAverage(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.9, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example6() {
        var stats = new WeightedMovingAverage();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5.9444444, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example7() {
        var stats = new WeightedMovingAverage();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.9636364, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example8() {
        var stats = new WeightedMovingAverage();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.02, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example9() {
        var stats = new WeightedMovingAverage();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(0.3333333, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example10() {
        var stats = new WeightedMovingAverage();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.3333333, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example11() {
        var stats = new WeightedMovingAverage(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(13.5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example12() {
        var stats = new WeightedMovingAverage(3);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000013.5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example13() {
        var stats = new WeightedMovingAverage(3);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000013.5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example14() {
        var stats = new WeightedMovingAverage(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1.8333333, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example15() {
        var stats = new WeightedMovingAverage(3, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(5.5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example16() {
        var stats = new WeightedMovingAverage(3);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7.6666667, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example17() {
        var stats = new WeightedMovingAverage(3);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example18() {
        var stats = new WeightedMovingAverage(3);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.9, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example19() {
        var stats = new WeightedMovingAverage(3);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1.6666667, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example20() {
        var stats = new WeightedMovingAverage(3);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.3333333, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_NoValue() {
        var stats = new WeightedMovingAverage();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(double.NaN, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_OneValue() {
        var stats = new WeightedMovingAverage();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_NoInfinity() {
        var stats = new WeightedMovingAverage();
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
    public void WeightedMovingAverage_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WeightedMovingAverage(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WeightedMovingAverage(10, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WeightedMovingAverage();
            stats.AddRange(null);
        });
    }

    [TestMethod]
    public void WeightedMovingAverage_NoCountOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new WeightedMovingAverage(0);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new WeightedMovingAverage(0, new double[] { 0 });
        });
    }

}

#pragma warning restore CS0618 // Type or member is obsolete


#if NET7_0_OR_GREATER

[TestClass]
public class WeightedMovingAverage_Decimal_Tests {

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example1() {
        var stats = new WeightedMovingAverage<decimal>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(12.1m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example2() {
        var stats = new WeightedMovingAverage<decimal>();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000012.1m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example3() {
        var stats = new WeightedMovingAverage<decimal>();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000012.1m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example4() {
        var stats = new WeightedMovingAverage<decimal>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.3m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example5() {
        var stats = new WeightedMovingAverage<decimal>(new decimal[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.9m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example6() {
        var stats = new WeightedMovingAverage<decimal>();
        stats.AddRange(new decimal[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5.9444444444444444444444444444m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example7() {
        var stats = new WeightedMovingAverage<decimal>();
        stats.AddRange(new decimal[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.9636363636363636363636363636m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example8() {
        var stats = new WeightedMovingAverage<decimal>();
        stats.AddRange(new decimal[] { 51.3m, 55.6m, 49.9m, 52.0m });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.02m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example9() {
        var stats = new WeightedMovingAverage<decimal>();
        stats.AddRange(new decimal[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(0.3333333333333333333333333333m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example10() {
        var stats = new WeightedMovingAverage<decimal>();
        stats.AddRange(new decimal[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.3333333333333333333333333333m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example11() {
        var stats = new WeightedMovingAverage<decimal>(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(13.5m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example12() {
        var stats = new WeightedMovingAverage<decimal>(3);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000013.5m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example13() {
        var stats = new WeightedMovingAverage<decimal>(3);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000013.5m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example14() {
        var stats = new WeightedMovingAverage<decimal>(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1.8333333333333333333333333333m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example15() {
        var stats = new WeightedMovingAverage<decimal>(3, new decimal[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(5.5m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example16() {
        var stats = new WeightedMovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7.6666666666666666666666666667m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example17() {
        var stats = new WeightedMovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example18() {
        var stats = new WeightedMovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { 51.3m, 55.6m, 49.9m, 52.0m });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.9m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example19() {
        var stats = new WeightedMovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1.6666666666666666666666666667m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_Example20() {
        var stats = new WeightedMovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.3333333333333333333333333333m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_NoValue() {
        var stats = new WeightedMovingAverage<decimal>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_OneValue() {
        var stats = new WeightedMovingAverage<decimal>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1m, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WeightedMovingAverage<decimal>(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WeightedMovingAverage<decimal>(10, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WeightedMovingAverage<decimal>();
            stats.AddRange(null);
        });
    }

    [TestMethod]
    public void WeightedMovingAverage_Decimal_NoCountOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new WeightedMovingAverage<double>(0);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new WeightedMovingAverage<double>(0, new double[] { 0 });
        });
    }

}


[TestClass]
public class WeightedMovingAverage_Double_Tests {

    [TestMethod]
    public void WeightedMovingAverage_Double_Example1() {
        var stats = new WeightedMovingAverage<double>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(12.1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example2() {
        var stats = new WeightedMovingAverage<double>();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000012.1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example3() {
        var stats = new WeightedMovingAverage<double>();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000012.1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example4() {
        var stats = new WeightedMovingAverage<double>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.3, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example5() {
        var stats = new WeightedMovingAverage<double>(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.9, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example6() {
        var stats = new WeightedMovingAverage<double>();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5.9444444, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example7() {
        var stats = new WeightedMovingAverage<double>();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.9636364, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example8() {
        var stats = new WeightedMovingAverage<double>();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.02, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example9() {
        var stats = new WeightedMovingAverage<double>();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(0.3333333, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example10() {
        var stats = new WeightedMovingAverage<double>();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.3333333, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example11() {
        var stats = new WeightedMovingAverage<double>(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(13.5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example12() {
        var stats = new WeightedMovingAverage<double>(3);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000013.5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example13() {
        var stats = new WeightedMovingAverage<double>(3);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000013.5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example14() {
        var stats = new WeightedMovingAverage<double>(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1.8333333, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example15() {
        var stats = new WeightedMovingAverage<double>(3, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(5.5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example16() {
        var stats = new WeightedMovingAverage<double>(3);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7.6666667, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example17() {
        var stats = new WeightedMovingAverage<double>(3);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example18() {
        var stats = new WeightedMovingAverage<double>(3);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.9, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example19() {
        var stats = new WeightedMovingAverage<double>(3);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1.6666667, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_Example20() {
        var stats = new WeightedMovingAverage<double>(3);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.3333333, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_NoValue() {
        var stats = new WeightedMovingAverage<double>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_OneValue() {
        var stats = new WeightedMovingAverage<double>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_NoInfinity() {
        var stats = new WeightedMovingAverage<double>();
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
    public void WeightedMovingAverage_Double_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WeightedMovingAverage<double>(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WeightedMovingAverage<double>(10, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WeightedMovingAverage<double>();
            stats.AddRange(null);
        });
    }

    [TestMethod]
    public void WeightedMovingAverage_Double_NoCountOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new WeightedMovingAverage<double>(0);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new WeightedMovingAverage<double>(0, new double[] { 0 });
        });
    }

}


[TestClass]
public class WeightedMovingAverage_Single_Tests {

    [TestMethod]
    public void WeightedMovingAverage_Single_Example1() {
        var stats = new WeightedMovingAverage<float>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(12.1000004, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example2() {
        var stats = new WeightedMovingAverage<float>();
        stats.Add(1000004);
        stats.Add(1000007);
        stats.Add(1000013);
        stats.Add(1000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000012.125, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example3() {
        var stats = new WeightedMovingAverage<float>();
        stats.Add(10000004);
        stats.Add(10000007);
        stats.Add(10000013);
        stats.Add(10000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10000012, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example4() {
        var stats = new WeightedMovingAverage<float>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.3, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example5() {
        var stats = new WeightedMovingAverage<float>(new float[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.9, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example6() {
        var stats = new WeightedMovingAverage<float>();
        stats.AddRange(new float[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5.9444447, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example7() {
        var stats = new WeightedMovingAverage<float>();
        stats.AddRange(new float[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.9636364, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example8() {
        var stats = new WeightedMovingAverage<float>();
        stats.AddRange(new float[] { 51.3f, 55.6f, 49.9f, 52.0f });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.0200005, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example9() {
        var stats = new WeightedMovingAverage<float>();
        stats.AddRange(new float[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(0.3333333, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example10() {
        var stats = new WeightedMovingAverage<float>();
        stats.AddRange(new float[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.3333333, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example11() {
        var stats = new WeightedMovingAverage<float>(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(13.5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example12() {
        var stats = new WeightedMovingAverage<float>(3);
        stats.Add(10000004);
        stats.Add(10000007);
        stats.Add(10000013);
        stats.Add(10000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10000013, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example13() {
        var stats = new WeightedMovingAverage<float>(3);
        stats.Add(10000004);
        stats.Add(10000007);
        stats.Add(10000013);
        stats.Add(10000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10000013, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example14() {
        var stats = new WeightedMovingAverage<float>(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1.8333333, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example15() {
        var stats = new WeightedMovingAverage<float>(3, new float[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(5.5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example16() {
        var stats = new WeightedMovingAverage<float>(3);
        stats.AddRange(new float[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7.6666665, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example17() {
        var stats = new WeightedMovingAverage<float>(3);
        stats.AddRange(new float[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example18() {
        var stats = new WeightedMovingAverage<float>(3);
        stats.AddRange(new float[] { 51.3f, 55.6f, 49.9f, 52.0f });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.8999977, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example19() {
        var stats = new WeightedMovingAverage<float>(3);
        stats.AddRange(new float[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1.6666667, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_Example20() {
        var stats = new WeightedMovingAverage<float>(3);
        stats.AddRange(new float[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.3333333, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_NoValue() {
        var stats = new WeightedMovingAverage<float>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0, stats.Average);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_OneValue() {
        var stats = new WeightedMovingAverage<float>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_NoInfinity() {
        var stats = new WeightedMovingAverage<float>();
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
    public void WeightedMovingAverage_Single_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WeightedMovingAverage<float>(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WeightedMovingAverage<float>(10, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new WeightedMovingAverage<float>();
            stats.AddRange(null);
        });
    }

    [TestMethod]
    public void WeightedMovingAverage_Single_NoCountOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new WeightedMovingAverage<float>(0);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new WeightedMovingAverage<float>(0, new float[] { 0 });
        });
    }

}

#endif
