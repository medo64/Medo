using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Math;

namespace Tests;

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
        Assert.AreEqual(12.1, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example2() {
        var stats = new WeightedMovingAverage();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000012.1, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example3() {
        var stats = new WeightedMovingAverage();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000012.1000001, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example4() {
        var stats = new WeightedMovingAverage();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.3, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example5() {
        var stats = new WeightedMovingAverage(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.9, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example6() {
        var stats = new WeightedMovingAverage();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5.944444444444445, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example7() {
        var stats = new WeightedMovingAverage();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.963636363636364, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example8() {
        var stats = new WeightedMovingAverage();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.02, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example9() {
        var stats = new WeightedMovingAverage();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(0.333333333333333, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example10() {
        var stats = new WeightedMovingAverage();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.333333333333333, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example11() {
        var stats = new WeightedMovingAverage(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(13.5, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example12() {
        var stats = new WeightedMovingAverage(3);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000013.5, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example13() {
        var stats = new WeightedMovingAverage(3);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000013.5, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example14() {
        var stats = new WeightedMovingAverage(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1.833333333333333, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example15() {
        var stats = new WeightedMovingAverage(3, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(5.5, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example16() {
        var stats = new WeightedMovingAverage(3);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7.666666666666667, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example17() {
        var stats = new WeightedMovingAverage(3);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example18() {
        var stats = new WeightedMovingAverage(3);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.9, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example19() {
        var stats = new WeightedMovingAverage(3);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1.666666666666667, stats.Average, 15);
    }

    [TestMethod]
    public void WeightedMovingAverage_Example20() {
        var stats = new WeightedMovingAverage(3);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.333333333333333, stats.Average, 15);
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
        Assert.AreEqual(1, stats.Average);
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
