using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Math;

namespace Tests;

#pragma warning disable CS0618 // Type or member is obsolete

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
        Assert.AreEqual(10, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Example2() {
        var stats = new SimpleAverage();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000010, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Example3() {
        var stats = new SimpleAverage();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000010, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Example4() {
        var stats = new SimpleAverage();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Example5() {
        var stats = new SimpleAverage(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Example6() {
        var stats = new SimpleAverage();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Example7() {
        var stats = new SimpleAverage();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Example8() {
        var stats = new SimpleAverage();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.2, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Example9() {
        var stats = new SimpleAverage();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Example10() {
        var stats = new SimpleAverage();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Average, 0.0000001);
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
        Assert.AreEqual(1, stats.Average, 0.0000001);
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

#pragma warning restore CS0618 // Type or member is obsolete


#if NET7_0_OR_GREATER

[TestClass]
public class SimpleAverage_Decimal_Tests {

    [TestMethod]
    public void SimpleAverage_Decimal_Example1() {
        var stats = new SimpleAverage<decimal>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10, stats.Average);
    }

    [TestMethod]
    public void SimpleAverage_Decimal_Example2() {
        var stats = new SimpleAverage<decimal>();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000010, stats.Average);
    }

    [TestMethod]
    public void SimpleAverage_Decimal_Example3() {
        var stats = new SimpleAverage<decimal>();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000010, stats.Average);
    }

    [TestMethod]
    public void SimpleAverage_Decimal_Example4() {
        var stats = new SimpleAverage<decimal>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Average);
    }

    [TestMethod]
    public void SimpleAverage_Decimal_Example5() {
        var stats = new SimpleAverage<decimal>(new decimal[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Average);
    }

    [TestMethod]
    public void SimpleAverage_Decimal_Example6() {
        var stats = new SimpleAverage<decimal>();
        stats.AddRange(new decimal[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Average);
    }

    [TestMethod]
    public void SimpleAverage_Decimal_Example7() {
        var stats = new SimpleAverage<decimal>();
        stats.AddRange(new decimal[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7, stats.Average);
    }

    [TestMethod]
    public void SimpleAverage_Decimal_Example8() {
        var stats = new SimpleAverage<decimal>();
        stats.AddRange(new decimal[] { 51.3m, 55.6m, 49.9m, 52.0m });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.2m, stats.Average);
    }

    [TestMethod]
    public void SimpleAverage_Decimal_Example9() {
        var stats = new SimpleAverage<decimal>();
        stats.AddRange(new decimal[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Average);
    }

    [TestMethod]
    public void SimpleAverage_Decimal_Example10() {
        var stats = new SimpleAverage<decimal>();
        stats.AddRange(new decimal[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Average);
    }

    [TestMethod]
    public void SimpleAverage_Decimal_NoValue() {
        var stats = new SimpleAverage<decimal>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0, stats.Average);
    }

    [TestMethod]
    public void SimpleAverage_Decimal_OneValue() {
        var stats = new SimpleAverage<decimal>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1, stats.Average);
    }

    [TestMethod]
    public void SimpleAverage_Decimal_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new SimpleAverage<decimal>(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new SimpleAverage<decimal>();
            stats.AddRange(null);
        });
    }

}


[TestClass]
public class SimpleAverage_Double_Tests {

    [TestMethod]
    public void SimpleAverage_Double_Example1() {
        var stats = new SimpleAverage<double>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Double_Example2() {
        var stats = new SimpleAverage<double>();
        stats.Add(100000004);
        stats.Add(100000007);
        stats.Add(100000013);
        stats.Add(100000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(100000010, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Double_Example3() {
        var stats = new SimpleAverage<double>();
        stats.Add(1000000004);
        stats.Add(1000000007);
        stats.Add(1000000013);
        stats.Add(1000000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(1000000010, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Double_Example4() {
        var stats = new SimpleAverage<double>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Double_Example5() {
        var stats = new SimpleAverage<double>(new double[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Double_Example6() {
        var stats = new SimpleAverage<double>();
        stats.AddRange(new double[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Double_Example7() {
        var stats = new SimpleAverage<double>();
        stats.AddRange(new double[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Double_Example8() {
        var stats = new SimpleAverage<double>();
        stats.AddRange(new double[] { 51.3, 55.6, 49.9, 52.0 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.2, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Double_Example9() {
        var stats = new SimpleAverage<double>();
        stats.AddRange(new double[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Double_Example10() {
        var stats = new SimpleAverage<double>();
        stats.AddRange(new double[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Double_NoValue() {
        var stats = new SimpleAverage<double>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Double_OneValue() {
        var stats = new SimpleAverage<double>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Double_NoInfinity() {
        var stats = new SimpleAverage<double>();
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
    public void SimpleAverage_Double_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new SimpleAverage<double>(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new SimpleAverage<double>();
            stats.AddRange(null);
        });
    }

}


[TestClass]
public class SimpleAverage_Single_Tests {

    [TestMethod]
    public void SimpleAverage_Single_Example1() {
        var stats = new SimpleAverage<float>();
        stats.Add(4);
        stats.Add(7);
        stats.Add(13);
        stats.Add(16);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Single_Example2() {
        var stats = new SimpleAverage<float>();
        stats.Add(10000004);
        stats.Add(10000007);
        stats.Add(10000013);
        stats.Add(10000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10000010, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Single_Example3() {
        var stats = new SimpleAverage<float>();
        stats.Add(10000004);
        stats.Add(10000007);
        stats.Add(10000013);
        stats.Add(10000016);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(10000010, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Single_Example4() {
        var stats = new SimpleAverage<float>();
        stats.Add(6);
        stats.Add(2);
        stats.Add(3);
        stats.Add(1);
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(3, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Single_Example5() {
        var stats = new SimpleAverage<float>(new float[] { 2, 2, 5, 7 });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(4, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Single_Example6() {
        var stats = new SimpleAverage<float>();
        stats.AddRange(new float[] { 2, 4, 4, 4, 5, 5, 7, 9 });
        Assert.AreEqual(8, stats.Count);
        Assert.AreEqual(5, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Single_Example7() {
        var stats = new SimpleAverage<float>();
        stats.AddRange(new float[] { 9, 2, 5, 4, 12, 7, 8, 11, 9, 3, 7, 4, 12, 5, 4, 10, 9, 6, 9, 4 });
        Assert.AreEqual(20, stats.Count);
        Assert.AreEqual(7, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Single_Example8() {
        var stats = new SimpleAverage<float>();
        stats.AddRange(new float[] { 51.3f, 55.6f, 49.9f, 52.0f });
        Assert.AreEqual(4, stats.Count);
        Assert.AreEqual(52.1999969, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Single_Example9() {
        var stats = new SimpleAverage<float>();
        stats.AddRange(new float[] { -5, -3, -1, 1, 3 });
        Assert.AreEqual(5, stats.Count);
        Assert.AreEqual(-1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Single_Example10() {
        var stats = new SimpleAverage<float>();
        stats.AddRange(new float[] { -1, 0, 1 });
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual(0, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Single_NoValue() {
        var stats = new SimpleAverage<float>();
        Assert.AreEqual(0, stats.Count);
        Assert.AreEqual(0, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Single_OneValue() {
        var stats = new SimpleAverage<float>();
        stats.Add(1);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual(1, stats.Average, 0.0000001);
    }

    [TestMethod]
    public void SimpleAverage_Single_NoInfinity() {
        var stats = new SimpleAverage<float>();
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
    public void SimpleAverage_Single_NoNullCollection() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new SimpleAverage<float>(null);
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var stats = new SimpleAverage<float>();
            stats.AddRange(null);
        });
    }

}

#endif
