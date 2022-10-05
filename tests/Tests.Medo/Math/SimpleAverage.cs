using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Math;

namespace Tests;

[TestClass]
public class SimpleAverage_Tests {

    [TestMethod]
    public void SimpleAverage_Example1() {
        var stats = new SimpleAverage();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10, stats.Average, 15);
    }

    [TestMethod]
    public void SimpleAverage_Example2() {
        var stats = new SimpleAverage();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000010, stats.Average, 15);
    }

    [TestMethod]
    public void SimpleAverage_Example3() {
        var stats = new SimpleAverage();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000010, stats.Average, 15);
    }

    [TestMethod]
    public void SimpleAverage_Example4() {
        var stats = new SimpleAverage();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Average, 15);
    }

    [TestMethod]
    public void SimpleAverage_Example5() {
        var stats = new SimpleAverage(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Average, 15);
    }

    [TestMethod]
    public void SimpleAverage_Example6() {
        var stats = new SimpleAverage();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Average, 15);
    }

    [TestMethod]
    public void SimpleAverage_Example7() {
        var stats = new SimpleAverage();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7, stats.Average, 15);
    }

    [TestMethod]
    public void SimpleAverage_Example8() {
        var stats = new SimpleAverage();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.2, stats.Average, 15);
    }

    [TestMethod]
    public void SimpleAverage_Example9() {
        var stats = new SimpleAverage();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Average, 15);
    }

    [TestMethod]
    public void SimpleAverage_Example10() {
        var stats = new SimpleAverage();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Average, 15);
    }

    [TestMethod]
    public void SimpleAverage_NoValue() {
        var stats = new SimpleAverage();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(double.NaN, stats.Average);
    }

    [TestMethod]
    public void SimpleAverage_OneValue() {
        var stats = new SimpleAverage();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1, stats.Average);
    }

    [TestMethod]
    public void SimpleAverage_NoInfinity() {
        var stats = new SimpleAverage();
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
    public void SimpleAverage_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new SimpleAverage(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new SimpleAverage();
            stats.AddRange(null);
        });
    }

}
