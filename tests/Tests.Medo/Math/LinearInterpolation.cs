using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Math;

namespace Tests;

#pragma warning disable CS0618 // Type or member is obsolete

[TestClass]
public class LinearInterpolation_Tests {

    [TestMethod]
    public void LinearInterpolation_Middle_1() {
        var target = new LinearInterpolation();
        target.AddReferencePoint(2, 1);
        target.AddReferencePoint(6, 3);
        Assert.AreEqual(2, target.GetAdjustedValue(1));
        Assert.AreEqual(4, target.GetAdjustedValue(2));
        Assert.AreEqual(6, target.GetAdjustedValue(3));
    }

    [TestMethod]
    public void LinearInterpolation_Middle_2() {
        var target = new LinearInterpolation();
        target.AddReferencePoint(-1, -10);
        target.AddReferencePoint(1, 10);
        Assert.AreEqual(0, target.GetAdjustedValue(0));
        Assert.AreEqual(0.5, target.GetAdjustedValue(5));
    }

    [TestMethod]
    public void LinearInterpolation_Middle_3() {
        var target = new LinearInterpolation();
        target.AddReferencePoint(0, 0);
        target.AddReferencePoint(1, 10);
        Assert.AreEqual(0.5, target.GetAdjustedValue(5));
    }

    [TestMethod]
    public void LinearInterpolation_Below_1() {
        var target = new LinearInterpolation();
        target.AddReferencePoint(1, 0);
        Assert.AreEqual(1, target.GetAdjustedValue(0));
        Assert.AreEqual(2, target.GetAdjustedValue(1));
    }

    [TestMethod]
    public void LinearInterpolation_Below_2() {
        var target = new LinearInterpolation();
        target.AddReferencePoint(2, 1);
        target.AddReferencePoint(4, 2);
        target.AddReferencePoint(6, 3);
        Assert.AreEqual(8, target.GetAdjustedValue(4));
        Assert.AreEqual(10, target.GetAdjustedValue(5));
    }

    [TestMethod]
    public void LinearInterpolation_Above_1() {
        var target = new LinearInterpolation();
        target.AddReferencePoint(11, 10);
        Assert.AreEqual(11, target.GetAdjustedValue(10));
        Assert.AreEqual(10, target.GetAdjustedValue(9));
    }

    [TestMethod]
    public void LinearInterpolation_Above_2() {
        var target = new LinearInterpolation();
        target.AddReferencePoint(6, 3);
        target.AddReferencePoint(8, 4);
        target.AddReferencePoint(10, 5);
        Assert.AreEqual(2, target.GetAdjustedValue(1));
        Assert.AreEqual(4, target.GetAdjustedValue(2));
    }

    [TestMethod]
    public void LinearInterpolation_CalibratedThreePoints() {
        var target = new LinearInterpolation();
        target.AddReferencePoint(1, 1.1);
        target.AddReferencePoint(2, 1.2);
        target.AddReferencePoint(3, 1.3);
        Assert.AreEqual(2.5, target.GetAdjustedValue(1.25));
    }

    [TestMethod]
    public void LinearInterpolation_CalibratedValueHit() {
        var target = new LinearInterpolation();
        target.AddReferencePoint(1, 0);
        target.AddReferencePoint(2, 1);
        target.AddReferencePoint(3, 2);
        Assert.AreEqual(3, target.GetAdjustedValue(2));
    }

    [TestMethod]
    public void LinearInterpolation_NoCalibration() {
        var target = new LinearInterpolation();
        Assert.AreEqual(0, target.GetAdjustedValue(0));
        Assert.AreEqual(1, target.GetAdjustedValue(1));
    }

    [TestMethod]
    public void LinearInterpolation_OnePoint() {
        var target = new LinearInterpolation();
        target.AddReferencePoint(0, 1);
        Assert.AreEqual(0, target.GetAdjustedValue(1));
    }

}

#pragma warning restore CS0618 // Type or member is obsolete


#if NET7_0_OR_GREATER

[TestClass]
public class LinearInterpolation_Decimal_Tests {

    [TestMethod]
    public void LinearInterpolation_Decimal_Middle_1() {
        var target = new LinearInterpolation<decimal>();
        target.AddReferencePoint(2, 1);
        target.AddReferencePoint(6, 3);
        Assert.AreEqual(2, target.GetAdjustedValue(1));
        Assert.AreEqual(4, target.GetAdjustedValue(2));
        Assert.AreEqual(6, target.GetAdjustedValue(3));
    }

    [TestMethod]
    public void LinearInterpolation_Decimal_Middle_2() {
        var target = new LinearInterpolation<decimal>();
        target.AddReferencePoint(-1, -10);
        target.AddReferencePoint(1, 10);
        Assert.AreEqual(0, target.GetAdjustedValue(0));
        Assert.AreEqual(0.5m, target.GetAdjustedValue(5));
    }

    [TestMethod]
    public void LinearInterpolation_Decimal_Middle_3() {
        var target = new LinearInterpolation<decimal>();
        target.AddReferencePoint(0, 0);
        target.AddReferencePoint(1, 10);
        Assert.AreEqual(0.5m, target.GetAdjustedValue(5));
    }

    [TestMethod]
    public void LinearInterpolation_Decimal_Below_1() {
        var target = new LinearInterpolation<decimal>();
        target.AddReferencePoint(1, 0);
        Assert.AreEqual(1, target.GetAdjustedValue(0));
        Assert.AreEqual(2, target.GetAdjustedValue(1));
    }

    [TestMethod]
    public void LinearInterpolation_Decimal_Below_2() {
        var target = new LinearInterpolation<decimal>();
        target.AddReferencePoint(2, 1);
        target.AddReferencePoint(4, 2);
        target.AddReferencePoint(6, 3);
        Assert.AreEqual(8, target.GetAdjustedValue(4));
        Assert.AreEqual(10, target.GetAdjustedValue(5));
    }

    [TestMethod]
    public void LinearInterpolation_Decimal_Above_1() {
        var target = new LinearInterpolation<decimal>();
        target.AddReferencePoint(11, 10);
        Assert.AreEqual(11, target.GetAdjustedValue(10));
        Assert.AreEqual(10, target.GetAdjustedValue(9));
    }

    [TestMethod]
    public void LinearInterpolation_Decimal_Above_2() {
        var target = new LinearInterpolation<decimal>();
        target.AddReferencePoint(6, 3);
        target.AddReferencePoint(8, 4);
        target.AddReferencePoint(10, 5);
        Assert.AreEqual(2, target.GetAdjustedValue(1));
        Assert.AreEqual(4, target.GetAdjustedValue(2));
    }

    [TestMethod]
    public void LinearInterpolation_Decimal_CalibratedThreePoints() {
        var target = new LinearInterpolation<decimal>();
        target.AddReferencePoint(1, 1.1m);
        target.AddReferencePoint(2, 1.2m);
        target.AddReferencePoint(3, 1.3m);
        Assert.AreEqual(2.5m, target.GetAdjustedValue(1.25m));
    }

    [TestMethod]
    public void LinearInterpolation_Decimal_CalibratedValueHit() {
        var target = new LinearInterpolation<decimal>();
        target.AddReferencePoint(1, 0);
        target.AddReferencePoint(2, 1);
        target.AddReferencePoint(3, 2);
        Assert.AreEqual(3, target.GetAdjustedValue(2));
    }

    [TestMethod]
    public void LinearInterpolation_Decimal_NoCalibration() {
        var target = new LinearInterpolation<decimal>();
        Assert.AreEqual(0, target.GetAdjustedValue(0));
        Assert.AreEqual(1, target.GetAdjustedValue(1));
    }

    [TestMethod]
    public void LinearInterpolation_Decimal_OnePoint() {
        var target = new LinearInterpolation<decimal>();
        target.AddReferencePoint(0, 1);
        Assert.AreEqual(0, target.GetAdjustedValue(1));
    }

}


[TestClass]
public class LinearInterpolation_Double_Tests {

    [TestMethod]
    public void LinearInterpolation_Double_Middle_1() {
        var target = new LinearInterpolation<double>();
        target.AddReferencePoint(2, 1);
        target.AddReferencePoint(6, 3);
        Assert.AreEqual(2, target.GetAdjustedValue(1));
        Assert.AreEqual(4, target.GetAdjustedValue(2));
        Assert.AreEqual(6, target.GetAdjustedValue(3));
    }

    [TestMethod]
    public void LinearInterpolation_Double_Middle_2() {
        var target = new LinearInterpolation<double>();
        target.AddReferencePoint(-1, -10);
        target.AddReferencePoint(1, 10);
        Assert.AreEqual(0, target.GetAdjustedValue(0));
        Assert.AreEqual(0.5, target.GetAdjustedValue(5));
    }

    [TestMethod]
    public void LinearInterpolation_Double_Middle_3() {
        var target = new LinearInterpolation<double>();
        target.AddReferencePoint(0, 0);
        target.AddReferencePoint(1, 10);
        Assert.AreEqual(0.5, target.GetAdjustedValue(5));
    }

    [TestMethod]
    public void LinearInterpolation_Double_Below_1() {
        var target = new LinearInterpolation<double>();
        target.AddReferencePoint(1, 0);
        Assert.AreEqual(1, target.GetAdjustedValue(0));
        Assert.AreEqual(2, target.GetAdjustedValue(1));
    }

    [TestMethod]
    public void LinearInterpolation_Double_Below_2() {
        var target = new LinearInterpolation<double>();
        target.AddReferencePoint(2, 1);
        target.AddReferencePoint(4, 2);
        target.AddReferencePoint(6, 3);
        Assert.AreEqual(8, target.GetAdjustedValue(4));
        Assert.AreEqual(10, target.GetAdjustedValue(5));
    }

    [TestMethod]
    public void LinearInterpolation_Double_Above_1() {
        var target = new LinearInterpolation<double>();
        target.AddReferencePoint(11, 10);
        Assert.AreEqual(11, target.GetAdjustedValue(10));
        Assert.AreEqual(10, target.GetAdjustedValue(9));
    }

    [TestMethod]
    public void LinearInterpolation_Double_Above_2() {
        var target = new LinearInterpolation<double>();
        target.AddReferencePoint(6, 3);
        target.AddReferencePoint(8, 4);
        target.AddReferencePoint(10, 5);
        Assert.AreEqual(2, target.GetAdjustedValue(1));
        Assert.AreEqual(4, target.GetAdjustedValue(2));
    }

    [TestMethod]
    public void LinearInterpolation_Double_CalibratedThreePoints() {
        var target = new LinearInterpolation<double>();
        target.AddReferencePoint(1, 1.1);
        target.AddReferencePoint(2, 1.2);
        target.AddReferencePoint(3, 1.3);
        Assert.AreEqual(2.5, target.GetAdjustedValue(1.25));
    }

    [TestMethod]
    public void LinearInterpolation_Double_CalibratedValueHit() {
        var target = new LinearInterpolation<double>();
        target.AddReferencePoint(1, 0);
        target.AddReferencePoint(2, 1);
        target.AddReferencePoint(3, 2);
        Assert.AreEqual(3, target.GetAdjustedValue(2));
    }

    [TestMethod]
    public void LinearInterpolation_Double_NoCalibration() {
        var target = new LinearInterpolation<double>();
        Assert.AreEqual(0, target.GetAdjustedValue(0));
        Assert.AreEqual(1, target.GetAdjustedValue(1));
    }

    [TestMethod]
    public void LinearInterpolation_Double_OnePoint() {
        var target = new LinearInterpolation<double>();
        target.AddReferencePoint(0, 1);
        Assert.AreEqual(0, target.GetAdjustedValue(1));
    }

}


[TestClass]
public class LinearInterpolation_Single_Tests {

    [TestMethod]
    public void LinearInterpolation_Single_Middle_1() {
        var target = new LinearInterpolation<float>();
        target.AddReferencePoint(2, 1);
        target.AddReferencePoint(6, 3);
        Assert.AreEqual(2, target.GetAdjustedValue(1));
        Assert.AreEqual(4, target.GetAdjustedValue(2));
        Assert.AreEqual(6, target.GetAdjustedValue(3));
    }

    [TestMethod]
    public void LinearInterpolation_Single_Middle_2() {
        var target = new LinearInterpolation<float>();
        target.AddReferencePoint(-1, -10);
        target.AddReferencePoint(1, 10);
        Assert.AreEqual(0, target.GetAdjustedValue(0));
        Assert.AreEqual(0.5, target.GetAdjustedValue(5));
    }

    [TestMethod]
    public void LinearInterpolation_Single_Middle_3() {
        var target = new LinearInterpolation<float>();
        target.AddReferencePoint(0, 0);
        target.AddReferencePoint(1, 10);
        Assert.AreEqual(0.5, target.GetAdjustedValue(5));
    }

    [TestMethod]
    public void LinearInterpolation_Single_Below_1() {
        var target = new LinearInterpolation<float>();
        target.AddReferencePoint(1, 0);
        Assert.AreEqual(1, target.GetAdjustedValue(0));
        Assert.AreEqual(2, target.GetAdjustedValue(1));
    }

    [TestMethod]
    public void LinearInterpolation_Single_Below_2() {
        var target = new LinearInterpolation<float>();
        target.AddReferencePoint(2, 1);
        target.AddReferencePoint(4, 2);
        target.AddReferencePoint(6, 3);
        Assert.AreEqual(8, target.GetAdjustedValue(4));
        Assert.AreEqual(10, target.GetAdjustedValue(5));
    }

    [TestMethod]
    public void LinearInterpolation_Single_Above_1() {
        var target = new LinearInterpolation<float>();
        target.AddReferencePoint(11, 10);
        Assert.AreEqual(11, target.GetAdjustedValue(10));
        Assert.AreEqual(10, target.GetAdjustedValue(9));
    }

    [TestMethod]
    public void LinearInterpolation_Single_Above_2() {
        var target = new LinearInterpolation<float>();
        target.AddReferencePoint(6, 3);
        target.AddReferencePoint(8, 4);
        target.AddReferencePoint(10, 5);
        Assert.AreEqual(2, target.GetAdjustedValue(1));
        Assert.AreEqual(4, target.GetAdjustedValue(2));
    }

    [TestMethod]
    public void LinearInterpolation_Single_CalibratedThreePoints() {
        var target = new LinearInterpolation<float>();
        target.AddReferencePoint(1, 1.1f);
        target.AddReferencePoint(2, 1.2f);
        target.AddReferencePoint(3, 1.3f);
        Assert.AreEqual(2.5, target.GetAdjustedValue(1.25f));
    }

    [TestMethod]
    public void LinearInterpolation_Single_CalibratedValueHit() {
        var target = new LinearInterpolation<float>();
        target.AddReferencePoint(1, 0);
        target.AddReferencePoint(2, 1);
        target.AddReferencePoint(3, 2);
        Assert.AreEqual(3, target.GetAdjustedValue(2));
    }

    [TestMethod]
    public void LinearInterpolation_Single_NoCalibration() {
        var target = new LinearInterpolation<float>();
        Assert.AreEqual(0, target.GetAdjustedValue(0));
        Assert.AreEqual(1, target.GetAdjustedValue(1));
    }

    [TestMethod]
    public void LinearInterpolation_Single_OnePoint() {
        var target = new LinearInterpolation<float>();
        target.AddReferencePoint(0, 1);
        Assert.AreEqual(0, target.GetAdjustedValue(1));
    }

}

#endif
