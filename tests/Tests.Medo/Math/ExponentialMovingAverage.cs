using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Math;

namespace Tests;

#pragma warning disable CS0618 // Type or member is obsolete

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
        Assert.AreEqual(7.8858002, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example2() {
        var stats = new ExponentialMovingAverage();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000007.8858002, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example3() {
        var stats = new ExponentialMovingAverage();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000007.8858001, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example4() {
        var stats = new ExponentialMovingAverage();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.1577761, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example5() {
        var stats = new ExponentialMovingAverage(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3.3553719, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example6() {
        var stats = new ExponentialMovingAverage();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5.0857844, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example7() {
        var stats = new ExponentialMovingAverage();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.9299792, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example8() {
        var stats = new ExponentialMovingAverage();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.7423742, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example9() {
        var stats = new ExponentialMovingAverage();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1.9668738, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example10() {
        var stats = new ExponentialMovingAverage();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(-0.4876033, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example11() {
        var stats = new ExponentialMovingAverage(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(12.625, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example12() {
        var stats = new ExponentialMovingAverage(3);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000012.625, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example13() {
        var stats = new ExponentialMovingAverage(3);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000012.625, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example14() {
        var stats = new ExponentialMovingAverage(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.25, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example15() {
        var stats = new ExponentialMovingAverage(3, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(5.25, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example16() {
        var stats = new ExponentialMovingAverage(3);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7.421875, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example17() {
        var stats = new ExponentialMovingAverage(3);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.0440464, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example18() {
        var stats = new ExponentialMovingAverage(3);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.8375, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example19() {
        var stats = new ExponentialMovingAverage(3);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1.125, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example20() {
        var stats = new ExponentialMovingAverage(3);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.25, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example21() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(6.253, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example22() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000006.253, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example23() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000006.253, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example24() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.906, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example25() {
        var stats = new ExponentialMovingAverage(0.1, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.77, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example26() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(3.9673062, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example27() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7.2390079, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example28() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.5923, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example29() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-3.1902, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example30() {
        var stats = new ExponentialMovingAverage(0.1);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(-0.71, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example31() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(15.637, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example32() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000015.637, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example33() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000015.637, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example34() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1.194, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example35() {
        var stats = new ExponentialMovingAverage(0.9, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(6.77, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example36() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(8.7798998, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example37() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(4.4730416, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example38() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.8427, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example39() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(2.7778, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Example40() {
        var stats = new ExponentialMovingAverage(0.9);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.89, stats.Average, 0.0000001);
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
        Assert.AreEqual(1, stats.Average, 0.0000001);
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

#pragma warning restore CS0618 // Type or member is obsolete


#if NET7_0_OR_GREATER

[TestClass]
public class ExponentialMovingAverage_Decimal_Tests {

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example1() {
        var stats = new ExponentialMovingAverage<decimal>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(7.8858001502629636063110443275m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example2() {
        var stats = new ExponentialMovingAverage<decimal>();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000007.88580015026296360631m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example3() {
        var stats = new ExponentialMovingAverage<decimal>();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000007.8858001502629636063m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example4() {
        var stats = new ExponentialMovingAverage<decimal>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.1577761081893298031555221638m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example5() {
        var stats = new ExponentialMovingAverage<decimal>(new decimal[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3.3553719008264475371900826446m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example6() {
        var stats = new ExponentialMovingAverage<decimal>();
        stats.AddRange(new decimal[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5.0857843860455702536122867706m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example7() {
        var stats = new ExponentialMovingAverage<decimal>();
        stats.AddRange(new decimal[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.9299791797405354991252533676m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example8() {
        var stats = new ExponentialMovingAverage<decimal>();
        stats.AddRange(new decimal[] { 51.3m, 55.6m, 49.9m, 52.0m });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.742374154770849241773102930m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example9() {
        var stats = new ExponentialMovingAverage<decimal>();
        stats.AddRange(new decimal[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1.9668738474147915858206406666m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example10() {
        var stats = new ExponentialMovingAverage<decimal>();
        stats.AddRange(new decimal[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(-0.4876033057851234876033057851m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example11() {
        var stats = new ExponentialMovingAverage<decimal>(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(12.625m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example12() {
        var stats = new ExponentialMovingAverage<decimal>(3);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000012.625m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example13() {
        var stats = new ExponentialMovingAverage<decimal>(3);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000012.625m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example14() {
        var stats = new ExponentialMovingAverage<decimal>(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.25m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example15() {
        var stats = new ExponentialMovingAverage<decimal>(3, new decimal[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(5.25m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example16() {
        var stats = new ExponentialMovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7.421875m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example17() {
        var stats = new ExponentialMovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.0440464019775390625m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example18() {
        var stats = new ExponentialMovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { 51.3m, 55.6m, 49.9m, 52.0m });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.8375m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example19() {
        var stats = new ExponentialMovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1.125m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example20() {
        var stats = new ExponentialMovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.25m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example21() {
        var stats = new ExponentialMovingAverage<decimal>(0.1m);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(6.253m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example22() {
        var stats = new ExponentialMovingAverage<decimal>(0.1m);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000006.253m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example23() {
        var stats = new ExponentialMovingAverage<decimal>(0.1m);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000006.253m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example24() {
        var stats = new ExponentialMovingAverage<decimal>(0.1m);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.906m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example25() {
        var stats = new ExponentialMovingAverage<decimal>(0.1m, new decimal[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.77m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example26() {
        var stats = new ExponentialMovingAverage<decimal>(0.1m);
        stats.AddRange(new decimal[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(3.9673062m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example27() {
        var stats = new ExponentialMovingAverage<decimal>(0.1m);
        stats.AddRange(new decimal[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7.2390079259222249893m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example28() {
        var stats = new ExponentialMovingAverage<decimal>(0.1m);
        stats.AddRange(new decimal[] { 51.3m, 55.6m, 49.9m, 52.0m });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.5923m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example29() {
        var stats = new ExponentialMovingAverage<decimal>(0.1m);
        stats.AddRange(new decimal[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-3.1902m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example30() {
        var stats = new ExponentialMovingAverage<decimal>(0.1m);
        stats.AddRange(new decimal[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(-0.71m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example31() {
        var stats = new ExponentialMovingAverage<decimal>(0.9m);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(15.637m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example32() {
        var stats = new ExponentialMovingAverage<decimal>(0.9m);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000015.637m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example33() {
        var stats = new ExponentialMovingAverage<decimal>(0.9m);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000015.637m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example34() {
        var stats = new ExponentialMovingAverage<decimal>(0.9m);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1.194m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example35() {
        var stats = new ExponentialMovingAverage<decimal>(0.9m, new decimal[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(6.77m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example36() {
        var stats = new ExponentialMovingAverage<decimal>(0.9m);
        stats.AddRange(new decimal[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(8.7798998m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example37() {
        var stats = new ExponentialMovingAverage<decimal>(0.9m);
        stats.AddRange(new decimal[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(4.4730416226616942077m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example38() {
        var stats = new ExponentialMovingAverage<decimal>(0.9m);
        stats.AddRange(new decimal[] { 51.3m, 55.6m, 49.9m, 52.0m });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.8427m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example39() {
        var stats = new ExponentialMovingAverage<decimal>(0.9m);
        stats.AddRange(new decimal[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(2.7778m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_Example40() {
        var stats = new ExponentialMovingAverage<decimal>(0.9m);
        stats.AddRange(new decimal[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.89m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_NoValue() {
        var stats = new ExponentialMovingAverage<decimal>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_OneValue() {
        var stats = new ExponentialMovingAverage<decimal>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1m, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage<decimal>(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage<decimal>(10, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage<decimal>(0.0m, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage<decimal>();
            stats.AddRange(null);
        });
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_NoCountOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<decimal>(0);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<decimal>(0, new decimal[] { 0 });
        });
    }

    [TestMethod]
    public void ExponentialMovingAverage_Decimal_NoSmoothingOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<decimal>(-0.001m);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<decimal>(1.001m);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<decimal>(-0.001m, new decimal[] { 0 });
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<decimal>(1.001m, new decimal[] { 0 });
        });
    }

}


[TestClass]
public class ExponentialMovingAverage_Double_Tests {

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example1() {
        var stats = new ExponentialMovingAverage<double>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(7.8858002, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example2() {
        var stats = new ExponentialMovingAverage<double>();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000007.8858002, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example3() {
        var stats = new ExponentialMovingAverage<double>();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000007.8858001, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example4() {
        var stats = new ExponentialMovingAverage<double>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.1577761, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example5() {
        var stats = new ExponentialMovingAverage<double>(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3.3553719, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example6() {
        var stats = new ExponentialMovingAverage<double>();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5.0857844, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example7() {
        var stats = new ExponentialMovingAverage<double>();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.9299792, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example8() {
        var stats = new ExponentialMovingAverage<double>();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.7423742, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example9() {
        var stats = new ExponentialMovingAverage<double>();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1.9668738, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example10() {
        var stats = new ExponentialMovingAverage<double>();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(-0.4876033, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example11() {
        var stats = new ExponentialMovingAverage<double>(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(12.625, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example12() {
        var stats = new ExponentialMovingAverage<double>(3);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000012.625, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example13() {
        var stats = new ExponentialMovingAverage<double>(3);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000012.625, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example14() {
        var stats = new ExponentialMovingAverage<double>(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.25, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example15() {
        var stats = new ExponentialMovingAverage<double>(3, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(5.25, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example16() {
        var stats = new ExponentialMovingAverage<double>(3);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7.421875, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example17() {
        var stats = new ExponentialMovingAverage<double>(3);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.0440464, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example18() {
        var stats = new ExponentialMovingAverage<double>(3);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.8375, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example19() {
        var stats = new ExponentialMovingAverage<double>(3);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1.125, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example20() {
        var stats = new ExponentialMovingAverage<double>(3);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.25, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example21() {
        var stats = new ExponentialMovingAverage<double>(0.1);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(6.253, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example22() {
        var stats = new ExponentialMovingAverage<double>(0.1);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000006.253, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example23() {
        var stats = new ExponentialMovingAverage<double>(0.1);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000006.253, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example24() {
        var stats = new ExponentialMovingAverage<double>(0.1);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.906, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example25() {
        var stats = new ExponentialMovingAverage<double>(0.1, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.77, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example26() {
        var stats = new ExponentialMovingAverage<double>(0.1);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(3.9673062, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example27() {
        var stats = new ExponentialMovingAverage<double>(0.1);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7.2390079, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example28() {
        var stats = new ExponentialMovingAverage<double>(0.1);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.5923, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example29() {
        var stats = new ExponentialMovingAverage<double>(0.1);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-3.1902, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example30() {
        var stats = new ExponentialMovingAverage<double>(0.1);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(-0.71, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example31() {
        var stats = new ExponentialMovingAverage<double>(0.9);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(15.637, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example32() {
        var stats = new ExponentialMovingAverage<double>(0.9);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000015.637, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example33() {
        var stats = new ExponentialMovingAverage<double>(0.9);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000015.637, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example34() {
        var stats = new ExponentialMovingAverage<double>(0.9);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1.194, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example35() {
        var stats = new ExponentialMovingAverage<double>(0.9, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(6.77, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example36() {
        var stats = new ExponentialMovingAverage<double>(0.9);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(8.7798998, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example37() {
        var stats = new ExponentialMovingAverage<double>(0.9);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(4.4730416, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example38() {
        var stats = new ExponentialMovingAverage<double>(0.9);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.8427, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example39() {
        var stats = new ExponentialMovingAverage<double>(0.9);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(2.7778, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_Example40() {
        var stats = new ExponentialMovingAverage<double>(0.9);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.89, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_NoValue() {
        var stats = new ExponentialMovingAverage<double>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_OneValue() {
        var stats = new ExponentialMovingAverage<double>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_NoInfinity() {
        var stats = new ExponentialMovingAverage<double>();
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
    public void ExponentialMovingAverage_Double_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage<double>(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage<double>(10, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage<double>(0.0, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage<double>();
            stats.AddRange(null);
        });
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_NoCountOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<double>(0);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<double>(0, new double[] { 0 });
        });
    }

    [TestMethod]
    public void ExponentialMovingAverage_Double_NoSmoothingOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<double>(0 - double.Epsilon);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<double>(1.001);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<double>(0 - double.Epsilon, new double[] { 0 });
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<double>(1.001, new double[] { 0 });
        });
    }

}


[TestClass]
public class ExponentialMovingAverage_Single_Tests {

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example1() {
        var stats = new ExponentialMovingAverage<float>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(7.8858004, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example2() {
        var stats = new ExponentialMovingAverage<float>();
        stats.Add(1000004);
        stats.Add(1000007);
        stats.Add(1000013);
        stats.Add(1000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000007.9375, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example3() {
        var stats = new ExponentialMovingAverage<float>();
        stats.Add(100004);
        stats.Add(100007);
        stats.Add(100013);
        stats.Add(100016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100007.890625, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example4() {
        var stats = new ExponentialMovingAverage<float>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.1577759, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example5() {
        var stats = new ExponentialMovingAverage<float>(new float[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3.3553719, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example6() {
        var stats = new ExponentialMovingAverage<float>();
        stats.AddRange(new float[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5.0857844, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example7() {
        var stats = new ExponentialMovingAverage<float>();
        stats.AddRange(new float[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.9299793, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example8() {
        var stats = new ExponentialMovingAverage<float>();
        stats.AddRange(new float[] { 51.3f, 55.6f, 49.9f, 52.0f });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.7423744, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example9() {
        var stats = new ExponentialMovingAverage<float>();
        stats.AddRange(new float[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1.9668736, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example10() {
        var stats = new ExponentialMovingAverage<float>();
        stats.AddRange(new float[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(-0.4876033, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example11() {
        var stats = new ExponentialMovingAverage<float>(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(12.625, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example12() {
        var stats = new ExponentialMovingAverage<float>(3);
        stats.Add(100004);
        stats.Add(100007);
        stats.Add(100013);
        stats.Add(100016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100012.625, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example13() {
        var stats = new ExponentialMovingAverage<float>(3);
        stats.Add(1000004);
        stats.Add(1000007);
        stats.Add(1000013);
        stats.Add(1000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000012.625, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example14() {
        var stats = new ExponentialMovingAverage<float>(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.25, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example15() {
        var stats = new ExponentialMovingAverage<float>(3, new float[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(5.25, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example16() {
        var stats = new ExponentialMovingAverage<float>(3);
        stats.AddRange(new float[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7.421875, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example17() {
        var stats = new ExponentialMovingAverage<float>(3);
        stats.AddRange(new float[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.0440464, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example18() {
        var stats = new ExponentialMovingAverage<float>(3);
        stats.AddRange(new float[] { 51.3f, 55.6f, 49.9f, 52.0f });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.8375015, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example19() {
        var stats = new ExponentialMovingAverage<float>(3);
        stats.AddRange(new float[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1.125, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example20() {
        var stats = new ExponentialMovingAverage<float>(3);
        stats.AddRange(new float[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.25, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example21() {
        var stats = new ExponentialMovingAverage<float>(0.1f);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(6.2530003, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example22() {
        var stats = new ExponentialMovingAverage<float>(0.1f);
        stats.Add(1000004);
        stats.Add(1000007);
        stats.Add(1000013);
        stats.Add(1000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000006.25, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example23() {
        var stats = new ExponentialMovingAverage<float>(0.1f);
        stats.Add(1000004);
        stats.Add(1000007);
        stats.Add(1000013);
        stats.Add(1000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000006.25, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example24() {
        var stats = new ExponentialMovingAverage<float>(0.1f);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.9060001, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example25() {
        var stats = new ExponentialMovingAverage<float>(0.1f, new float[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2.77, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example26() {
        var stats = new ExponentialMovingAverage<float>(0.1f);
        stats.AddRange(new float[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(3.9673061, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example27() {
        var stats = new ExponentialMovingAverage<float>(0.1f);
        stats.AddRange(new float[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7.2390079, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example28() {
        var stats = new ExponentialMovingAverage<float>(0.1f);
        stats.AddRange(new float[] { 51.3f, 55.6f, 49.9f, 52.0f });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.5923004, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example29() {
        var stats = new ExponentialMovingAverage<float>(0.1f);
        stats.AddRange(new float[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-3.1901999, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example30() {
        var stats = new ExponentialMovingAverage<float>(0.1f);
        stats.AddRange(new float[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(-0.71, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example31() {
        var stats = new ExponentialMovingAverage<float>(0.9f);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(15.637, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example32() {
        var stats = new ExponentialMovingAverage<float>(0.9f);
        stats.Add(1000004);
        stats.Add(1000007);
        stats.Add(1000013);
        stats.Add(1000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000015.625, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example33() {
        var stats = new ExponentialMovingAverage<float>(0.9f);
        stats.Add(1000004);
        stats.Add(1000007);
        stats.Add(1000013);
        stats.Add(1000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000015.625, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example34() {
        var stats = new ExponentialMovingAverage<float>(0.9f);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1.194, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example35() {
        var stats = new ExponentialMovingAverage<float>(0.9f, new float[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(6.77, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example36() {
        var stats = new ExponentialMovingAverage<float>(0.9f);
        stats.AddRange(new float[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(8.7798996, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example37() {
        var stats = new ExponentialMovingAverage<float>(0.9f);
        stats.AddRange(new float[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(4.4730416, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example38() {
        var stats = new ExponentialMovingAverage<float>(0.9f);
        stats.AddRange(new float[] { 51.3f, 55.6f, 49.9f, 52.0f });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(51.8427010, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example39() {
        var stats = new ExponentialMovingAverage<float>(0.9f);
        stats.AddRange(new float[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(2.7778, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_Example40() {
        var stats = new ExponentialMovingAverage<float>(0.9f);
        stats.AddRange(new float[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0.89, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_NoValue() {
        var stats = new ExponentialMovingAverage<float>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0, stats.Average);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_OneValue() {
        var stats = new ExponentialMovingAverage<float>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_NoInfinity() {
        var stats = new ExponentialMovingAverage<float>();
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
    public void ExponentialMovingAverage_Single_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage<float>(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage<float>(10, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage<float>(0.0f, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new ExponentialMovingAverage<float>();
            stats.AddRange(null);
        });
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_NoCountOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<float>(0);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<float>(0, new float[] { 0 });
        });
    }

    [TestMethod]
    public void ExponentialMovingAverage_Single_NoSmoothingOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<float>(0 - float.Epsilon);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<float>(1.001f);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<float>(0 - float.Epsilon, new float[] { 0 });
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new ExponentialMovingAverage<float>(1.001f, new float[] { 0 });
        });
    }

}

#endif
