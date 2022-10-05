using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Math;

namespace Tests;

[TestClass]
public class ExponentialMovingAverage_Tests {

    [TestMethod]
    public void ExponentialMovingAverage_Example1() {
        var stats = new ExponentialMovingAverage();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(7.885800150262959, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example2() {
        var stats = new ExponentialMovingAverage();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000007.88580015, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example3() {
        var stats = new ExponentialMovingAverage();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000007.8858002, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example4() {
        var stats = new ExponentialMovingAverage();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.157776108189332, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example5() {
        var stats = new ExponentialMovingAverage(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3.355371900826446, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example6() {
        var stats = new ExponentialMovingAverage();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5.085784386045568, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example7() {
        var stats = new ExponentialMovingAverage();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.929979179740536, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example8() {
        var stats = new ExponentialMovingAverage();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.74237415477084, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example9() {
        var stats = new ExponentialMovingAverage();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1.966873847414794, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example10() {
        var stats = new ExponentialMovingAverage();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(-0.487603305785124, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example11() {
        var stats = new ExponentialMovingAverage(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(12.625, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example12() {
        var stats = new ExponentialMovingAverage(3);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000012.625, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example13() {
        var stats = new ExponentialMovingAverage(3);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000012.625, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example14() {
        var stats = new ExponentialMovingAverage(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.25, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example15() {
        var stats = new ExponentialMovingAverage(3, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(5.25, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example16() {
        var stats = new ExponentialMovingAverage(3);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7.421875, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example17() {
        var stats = new ExponentialMovingAverage(3);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.044046401977539, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example18() {
        var stats = new ExponentialMovingAverage(3);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.8375, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example19() {
        var stats = new ExponentialMovingAverage(3);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1.125, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example20() {
        var stats = new ExponentialMovingAverage(3);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.25, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example21() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(6.253, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example22() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000006.253, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example23() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000006.253, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example24() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.906, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example25() {
        var stats = new ExponentialMovingAverage(0.1, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.77, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example26() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(3.9673062, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example27() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7.239007925922224, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example28() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.592299999999994, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example29() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-3.1902, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example30() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(-0.71, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example31() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(15.637, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example32() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000015.63700001, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example33() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000015.637, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example34() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1.194, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example35() {
        var stats = new ExponentialMovingAverage(0.9, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(6.77, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example36() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(8.7798998, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example37() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(4.473041622661694, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example38() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.8427, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example39() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(2.7778, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example40() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.89, stats.Average, 15);
    }

    [TestMethod]
    public void ExponentialMovingAverage_NoValue() {
        var stats = new ExponentialMovingAverage();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(double.NaN, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_OneValue() {
        var stats = new ExponentialMovingAverage();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_NoInfinity() {
        var stats = new ExponentialMovingAverage();
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
    public void ExponentialMovingAverage_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage(10, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage(0.0, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage();
            stats.AddRange(null);
        });
    }

    [TestMethod]
    public void ExponentialMovingAverage_NoCountOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage(0);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage(0, new double[] { 0 });
        });
    }

    [TestMethod]
    public void ExponentialMovingAverage_NoSmoothingOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage(0 - double.Epsilon);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage(1.001);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage(0 - double.Epsilon, new double[] { 0 });
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage(1.001, new double[] { 0 });
        });
    }

}
