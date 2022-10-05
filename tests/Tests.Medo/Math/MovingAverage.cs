using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Math;

namespace Tests;

[TestClass]
public class MovingAverage_Tests {

    [TestMethod]
    public void MovingAverag_Example1() {
        var stats = new MovingAverage();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example2() {
        var stats = new MovingAverage();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000010, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example3() {
        var stats = new MovingAverage();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000010, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example4() {
        var stats = new MovingAverage();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example5() {
        var stats = new MovingAverage(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example6() {
        var stats = new MovingAverage();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example7() {
        var stats = new MovingAverage();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example8() {
        var stats = new MovingAverage();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.2, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example9() {
        var stats = new MovingAverage();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example10() {
        var stats = new MovingAverage();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example11() {
        var stats = new MovingAverage(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(12, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example12() {
        var stats = new MovingAverage(3);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000012, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example13() {
        var stats = new MovingAverage(3);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000012, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example14() {
        var stats = new MovingAverage(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example15() {
        var stats = new MovingAverage(3, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.666666666666667, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example16() {
        var stats = new MovingAverage(3);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example17() {
        var stats = new MovingAverage(3);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.333333333333333, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example18() {
        var stats = new MovingAverage(3);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.5, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example19() {
        var stats = new MovingAverage(3);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_Example20() {
        var stats = new MovingAverage(3);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Average, 15);
    }

    [TestMethod]
    public void MovingAverag_NoValue() {
        var stats = new MovingAverage();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(double.NaN, stats.Average);
    }

    [TestMethod]
    public void MovingAverag_OneValue() {
        var stats = new MovingAverage();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1, stats.Average);
    }

    [TestMethod]
    public void MovingAverag_NoInfinity() {
        var stats = new MovingAverage();
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
    public void MovingAverag_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new MovingAverage(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new MovingAverage(10, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new MovingAverage();
            stats.AddRange(null);
        });
    }

    [TestMethod]
    public void MovingAverag_NoCountOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new MovingAverage(0);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new MovingAverage(0, new double[] { 0 });
        });
    }

}
