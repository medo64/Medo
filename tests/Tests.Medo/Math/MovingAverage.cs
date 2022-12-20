using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Math;

namespace Tests;

[TestClass]
public class MovingAverage_Tests {

    [TestMethod]
    public void MovingAverage_Example1() {
        var stats = new MovingAverage();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example2() {
        var stats = new MovingAverage();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000010, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example3() {
        var stats = new MovingAverage();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000010, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example4() {
        var stats = new MovingAverage();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example5() {
        var stats = new MovingAverage(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example6() {
        var stats = new MovingAverage();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example7() {
        var stats = new MovingAverage();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example8() {
        var stats = new MovingAverage();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.2, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example9() {
        var stats = new MovingAverage();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example10() {
        var stats = new MovingAverage();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example11() {
        var stats = new MovingAverage(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(12, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example12() {
        var stats = new MovingAverage(3);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000012, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example13() {
        var stats = new MovingAverage(3);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000012, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example14() {
        var stats = new MovingAverage(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example15() {
        var stats = new MovingAverage(3, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.6666667, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example16() {
        var stats = new MovingAverage(3);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example17() {
        var stats = new MovingAverage(3);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.3333333, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example18() {
        var stats = new MovingAverage(3);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example19() {
        var stats = new MovingAverage(3);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Example20() {
        var stats = new MovingAverage(3);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_NoValue() {
        var stats = new MovingAverage();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(double.NaN, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_OneValue() {
        var stats = new MovingAverage();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_NoInfinity() {
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
    public void MovingAverage_NoNullCollection() {
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
    public void MovingAverage_NoCountOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new MovingAverage(0);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var stats = new MovingAverage(0, new double[] { 0 });
        });
    }

}


#if NET7_0_OR_GREATER

[TestClass]
public class MovingAverage_Decimal_Tests {

    [TestMethod]
    public void MovingAverage_Decimal_Example1() {
        var stats = new MovingAverage<decimal>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example2() {
        var stats = new MovingAverage<decimal>();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000010, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example3() {
        var stats = new MovingAverage<decimal>();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000010, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example4() {
        var stats = new MovingAverage<decimal>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example5() {
        var stats = new MovingAverage<decimal>(new decimal[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example6() {
        var stats = new MovingAverage<decimal>();
        stats.AddRange(new decimal[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example7() {
        var stats = new MovingAverage<decimal>();
        stats.AddRange(new decimal[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7m, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example8() {
        var stats = new MovingAverage<decimal>();
        stats.AddRange(new decimal[] { 51.3m, 55.6m, 49.9m, 52.0m });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.2m, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example9() {
        var stats = new MovingAverage<decimal>();
        stats.AddRange(new decimal[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example10() {
        var stats = new MovingAverage<decimal>();
        stats.AddRange(new decimal[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example11() {
        var stats = new MovingAverage<decimal>(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(12, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example12() {
        var stats = new MovingAverage<decimal>(3);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000012, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example13() {
        var stats = new MovingAverage<decimal>(3);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000012, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example14() {
        var stats = new MovingAverage<decimal>(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example15() {
        var stats = new MovingAverage<decimal>(3, new decimal[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.6666666666666666666666666667m, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example16() {
        var stats = new MovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example17() {
        var stats = new MovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.3333333333333333333333333333m, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example18() {
        var stats = new MovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { 51.3m, 55.6m, 49.9m, 52.0m });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.5m, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example19() {
        var stats = new MovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1m, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_Example20() {
        var stats = new MovingAverage<decimal>(3);
        stats.AddRange(new decimal[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0m, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_NoValue() {
        var stats = new MovingAverage<decimal>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0m, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_OneValue() {
        var stats = new MovingAverage<decimal>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1m, stats.Average);
    }

    [TestMethod]
    public void MovingAverage_Decimal_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new MovingAverage<decimal>(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new MovingAverage<decimal>(10, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new MovingAverage<decimal>();
            stats.AddRange(null);
        });
    }

    [TestMethod]
    public void MovingAverage_Decimal_NoCountOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new MovingAverage<decimal>(0);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new MovingAverage<decimal>(0, new decimal[] { 0 });
        });
    }

}

[TestClass]
public class MovingAverage_Double_Tests {

    [TestMethod]
    public void MovingAverage_Double_Example1() {
        var stats = new MovingAverage<double>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example2() {
        var stats = new MovingAverage<double>();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000010, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example3() {
        var stats = new MovingAverage<double>();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000010, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example4() {
        var stats = new MovingAverage<double>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example5() {
        var stats = new MovingAverage<double>(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example6() {
        var stats = new MovingAverage<double>();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example7() {
        var stats = new MovingAverage<double>();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example8() {
        var stats = new MovingAverage<double>();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.2, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example9() {
        var stats = new MovingAverage<double>();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example10() {
        var stats = new MovingAverage<double>();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example11() {
        var stats = new MovingAverage<double>(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(12, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example12() {
        var stats = new MovingAverage<double>(3);
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000012, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example13() {
        var stats = new MovingAverage<double>(3);
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000012, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example14() {
        var stats = new MovingAverage<double>(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example15() {
        var stats = new MovingAverage<double>(3, new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.6666667, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example16() {
        var stats = new MovingAverage<double>(3);
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example17() {
        var stats = new MovingAverage<double>(3);
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.3333333, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example18() {
        var stats = new MovingAverage<double>(3);
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example19() {
        var stats = new MovingAverage<double>(3);
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_Example20() {
        var stats = new MovingAverage<double>(3);
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_NoValue() {
        var stats = new MovingAverage<double>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_OneValue() {
        var stats = new MovingAverage<double>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Double_NoInfinity() {
        var stats = new MovingAverage<double>();
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
    public void MovingAverage_Double_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new MovingAverage<double>(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new MovingAverage<double>(10, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new MovingAverage<double>();
            stats.AddRange(null);
        });
    }

    [TestMethod]
    public void MovingAverage_Double_NoCountOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new MovingAverage<double>(0);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new MovingAverage<double>(0, new double[] { 0 });
        });
    }

}

[TestClass]
public class MovingAverage_Single_Tests {

    [TestMethod]
    public void MovingAverage_Single_Example1() {
        var stats = new MovingAverage<float>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example2() {
        var stats = new MovingAverage<float>();
        stats.Add(1000004);
        stats.Add(1000007);
        stats.Add(1000013);
        stats.Add(1000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000010, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example3() {
        var stats = new MovingAverage<float>();
        stats.Add(10000004);
        stats.Add(10000007);
        stats.Add(10000013);
        stats.Add(10000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10000010, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example4() {
        var stats = new MovingAverage<float>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example5() {
        var stats = new MovingAverage<float>(new float[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example6() {
        var stats = new MovingAverage<float>();
        stats.AddRange(new float[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example7() {
        var stats = new MovingAverage<float>();
        stats.AddRange(new float[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example8() {
        var stats = new MovingAverage<float>();
        stats.AddRange(new float[] { 51.3f, 55.6f, 49.9f, 52.0f });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.1999970, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example9() {
        var stats = new MovingAverage<float>();
        stats.AddRange(new float[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example10() {
        var stats = new MovingAverage<float>();
        stats.AddRange(new float[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example11() {
        var stats = new MovingAverage<float>(3);
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(12, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example12() {
        var stats = new MovingAverage<float>(3);
        stats.Add(10000004);
        stats.Add(10000007);
        stats.Add(10000013);
        stats.Add(10000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10000012, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example13() {
        var stats = new MovingAverage<float>(3);
        stats.Add(10000004);
        stats.Add(10000007);
        stats.Add(10000013);
        stats.Add(10000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10000012, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example14() {
        var stats = new MovingAverage<float>(3);
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(2, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example15() {
        var stats = new MovingAverage<float>(3, new float[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4.6666665, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example16() {
        var stats = new MovingAverage<float>(3);
        stats.AddRange(new float[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(7, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example17() {
        var stats = new MovingAverage<float>(3);
        stats.AddRange(new float[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(6.3333335, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example18() {
        var stats = new MovingAverage<float>(3);
        stats.AddRange(new float[] { 51.3f, 55.6f, 49.9f, 52.0f });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example19() {
        var stats = new MovingAverage<float>(3);
        stats.AddRange(new float[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_Example20() {
        var stats = new MovingAverage<float>(3);
        stats.AddRange(new float[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_NoValue() {
        var stats = new MovingAverage<float>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_OneValue() {
        var stats = new MovingAverage<float>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void MovingAverage_Single_NoInfinity() {
        var stats = new MovingAverage<float>();
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
    public void MovingAverage_Single_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new MovingAverage<float>(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new MovingAverage<float>(10, null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new MovingAverage<float>();
            stats.AddRange(null);
        });
    }

    [TestMethod]
    public void MovingAverage_Single_NoCountOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new MovingAverage<float>(0);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new MovingAverage<float>(0, new float[] { 0 });
        });
    }

}

#endif
