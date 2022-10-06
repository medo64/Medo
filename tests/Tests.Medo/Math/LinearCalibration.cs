using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Math;

namespace Tests;

#pragma warning disable CS0618 // Type or member is obsolete

[TestClass]
public class LinearCalibration_Tests {

    [TestMethod]
    public void LinearCalibration_NoCalibration() {
        var target = new LinearCalibration();
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(0, target.GetAdjustedValue(0));
        Assert.AreEqual(1, target.GetAdjustedValue(1));
    }


    [TestMethod]
    public void LinearCalibration_SinglePoint() {
        var target = new LinearCalibration();
        target.AddCalibrationPoint(0, 1);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(0, target.GetAdjustedValue(1));
    }

    [TestMethod]
    public void LinearCalibration_SinglePoint_NoChange() {
        var target = new LinearCalibration();
        target.AddCalibrationPoint(0, 0);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(1, target.GetAdjustedValue(1));
    }


    [TestMethod]
    public void LinearCalibration_TwoPoints_Simple() {
        var target = new LinearCalibration();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(100, 101);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(50, target.GetAdjustedValue(51));
    }

    [TestMethod]
    public void LinearCalibration_TwoPoints_Double() {
        var target = new LinearCalibration();
        target.AddCalibrationPoint(0, 0);
        target.AddCalibrationPoint(100, 50);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(50, target.GetAdjustedValue(25));
    }


    [TestMethod]
    public void LinearCalibration_ThreePoints_Simple() {
        var target = new LinearCalibration();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(50, 51);
        target.AddCalibrationPoint(100, 101);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(25, target.GetAdjustedValue(26));
    }

    [TestMethod]
    public void LinearCalibration_ThreePoints_Double() {
        var target = new LinearCalibration();
        target.AddCalibrationPoint(0, 0);
        target.AddCalibrationPoint(50, 25);
        target.AddCalibrationPoint(100, 50);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(80, target.GetAdjustedValue(40));
    }

    [TestMethod]
    public void LinearCalibration_ThreePoints_Halve() {
        var target = new LinearCalibration();
        target.AddCalibrationPoint(0, 0);
        target.AddCalibrationPoint(50, 100);
        target.AddCalibrationPoint(100, 200);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(75, target.GetAdjustedValue(150));
    }

    [TestMethod]
    public void LinearCalibration_ThreePoints_Freestyle_1() {
        var target = new LinearCalibration();
        target.AddCalibrationPoint(0, 0.5);
        target.AddCalibrationPoint(50, 50);
        target.AddCalibrationPoint(100, 99.5);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(24.74747, target.GetAdjustedValue(25), 5);
        Assert.AreEqual(50, target.GetAdjustedValue(50));
        Assert.AreEqual(75.25253, target.GetAdjustedValue(75), 5);
    }

    [TestMethod]
    public void LinearCalibration_ThreePoints_Freestyle_2() {
        var target = new LinearCalibration();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(50, 49);
        target.AddCalibrationPoint(100, 100);
        Assert.AreEqual(0.99985, target.CorrelationCoefficient, 5);
        Assert.AreEqual(24.74747, target.GetAdjustedValue(25), 5);
        Assert.AreEqual(50, target.GetAdjustedValue(50));
        Assert.AreEqual(75.25253, target.GetAdjustedValue(75), 5);
    }

    [TestMethod]
    public void LinearCalibration_ThreePoints_Freestyle_3() {
        var target = new LinearCalibration();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(20, -10);
        target.AddCalibrationPoint(100, 50);
        Assert.AreEqual(0.93477, target.CorrelationCoefficient, 5);
        Assert.AreEqual(60.08439, target.GetAdjustedValue(25), 5);
        Assert.AreEqual(104.38819, target.GetAdjustedValue(50), 5);
        Assert.AreEqual(148.69198, target.GetAdjustedValue(75), 5);
    }


    [TestMethod]
    public void LinearCalibration_Multipoint() {
        var target = new LinearCalibration();
        target.AddCalibrationPoint(0, 0.025);
        target.AddCalibrationPoint(1, 0.217);
        target.AddCalibrationPoint(2, 0.388);
        target.AddCalibrationPoint(3, 0.634);
        target.AddCalibrationPoint(4, 0.777);
        target.AddCalibrationPoint(5, 1.011);
        target.AddCalibrationPoint(6, 1.166);
        Assert.AreEqual(0.9976, target.CoefficientOfDetermination, 4);
        Assert.AreEqual(1.19, target.GetAdjustedValue(0.254), 2);
    }

}

#pragma warning restore CS0618 // Type or member is obsolete


#if NET7_0_OR_GREATER

[TestClass]
public class LinearCalibration_Decimal_Tests {

    [TestMethod]
    public void LinearCalibration_Decimal_NoCalibration() {
        var target = new LinearCalibration<decimal>();
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(0, target.GetAdjustedValue(0));
        Assert.AreEqual(1, target.GetAdjustedValue(1));
    }


    [TestMethod]
    public void LinearCalibration_Decimal_SinglePoint() {
        var target = new LinearCalibration<decimal>();
        target.AddCalibrationPoint(0, 1);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(0, target.GetAdjustedValue(1));
    }

    [TestMethod]
    public void LinearCalibration_Decimal_SinglePoint_NoChange() {
        var target = new LinearCalibration<decimal>();
        target.AddCalibrationPoint(0, 0);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(1, target.GetAdjustedValue(1));
    }


    [TestMethod]
    public void LinearCalibration_Decimal_TwoPoints_Simple() {
        var target = new LinearCalibration<decimal>();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(100, 101);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(50, target.GetAdjustedValue(51));
    }

    [TestMethod]
    public void LinearCalibration_Decimal_TwoPoints_Double() {
        var target = new LinearCalibration<decimal>();
        target.AddCalibrationPoint(0, 0);
        target.AddCalibrationPoint(100, 50);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(50, target.GetAdjustedValue(25));
    }


    [TestMethod]
    public void LinearCalibration_Decimal_ThreePoints_Simple() {
        var target = new LinearCalibration<decimal>();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(50, 51);
        target.AddCalibrationPoint(100, 101);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(25, target.GetAdjustedValue(26));
    }

    [TestMethod]
    public void LinearCalibration_Decimal_ThreePoints_Double() {
        var target = new LinearCalibration<decimal>();
        target.AddCalibrationPoint(0, 0);
        target.AddCalibrationPoint(50, 25);
        target.AddCalibrationPoint(100, 50);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(80, target.GetAdjustedValue(40));
    }

    [TestMethod]
    public void LinearCalibration_Decimal_ThreePoints_Halve() {
        var target = new LinearCalibration<decimal>();
        target.AddCalibrationPoint(0, 0);
        target.AddCalibrationPoint(50, 100);
        target.AddCalibrationPoint(100, 200);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(75, target.GetAdjustedValue(150));
    }

    [TestMethod]
    public void LinearCalibration_Decimal_ThreePoints_Freestyle_1() {
        var target = new LinearCalibration<decimal>();
        target.AddCalibrationPoint(0, 0.5m);
        target.AddCalibrationPoint(50, 50m);
        target.AddCalibrationPoint(100, 99.5m);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(24.74747m, target.GetAdjustedValue(25), 0.00001m);
        Assert.AreEqual(50, target.GetAdjustedValue(50));
        Assert.AreEqual(75.25253m, target.GetAdjustedValue(75), 0.00001m);
    }

    [TestMethod]
    public void LinearCalibration_Decimal_ThreePoints_Freestyle_2() {
        var target = new LinearCalibration<decimal>();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(50, 49);
        target.AddCalibrationPoint(100, 100);
        Assert.AreEqual(0.99985m, target.CorrelationCoefficient, 5);
        Assert.AreEqual(24.74747m, target.GetAdjustedValue(25), 0.00001m);
        Assert.AreEqual(50, target.GetAdjustedValue(50));
        Assert.AreEqual(75.25253m, target.GetAdjustedValue(75), 0.00001m);
    }

    [TestMethod]
    public void LinearCalibration_Decimal_ThreePoints_Freestyle_3() {
        var target = new LinearCalibration<decimal>();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(20, -10);
        target.AddCalibrationPoint(100, 50);
        Assert.AreEqual(0.93477m, target.CorrelationCoefficient, 0.00001m);
        Assert.AreEqual(60.08439m, target.GetAdjustedValue(25), 0.00001m);
        Assert.AreEqual(104.38819m, target.GetAdjustedValue(50), 0.00001m);
        Assert.AreEqual(148.69198m, target.GetAdjustedValue(75), 0.00001m);
    }


    [TestMethod]
    public void LinearCalibration_Decimal_Multipoint() {
        var target = new LinearCalibration<decimal>();
        target.AddCalibrationPoint(0, 0.025m);
        target.AddCalibrationPoint(1, 0.217m);
        target.AddCalibrationPoint(2, 0.388m);
        target.AddCalibrationPoint(3, 0.634m);
        target.AddCalibrationPoint(4, 0.777m);
        target.AddCalibrationPoint(5, 1.011m);
        target.AddCalibrationPoint(6, 1.166m);
        Assert.AreEqual(0.9976m, target.CoefficientOfDetermination, 0.0001m);
        Assert.AreEqual(1.19259m, target.GetAdjustedValue(0.254m), 0.00001m);
    }

}

[TestClass]
public class LinearCalibration_Double_Tests {

    [TestMethod]
    public void LinearCalibration_Double_NoCalibration() {
        var target = new LinearCalibration<double>();
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(0, target.GetAdjustedValue(0));
        Assert.AreEqual(1, target.GetAdjustedValue(1));
    }


    [TestMethod]
    public void LinearCalibration_Double_SinglePoint() {
        var target = new LinearCalibration<double>();
        target.AddCalibrationPoint(0, 1);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(0, target.GetAdjustedValue(1));
    }

    [TestMethod]
    public void LinearCalibration_Double_SinglePoint_NoChange() {
        var target = new LinearCalibration<double>();
        target.AddCalibrationPoint(0, 0);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(1, target.GetAdjustedValue(1));
    }


    [TestMethod]
    public void LinearCalibration_Double_TwoPoints_Simple() {
        var target = new LinearCalibration<double>();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(100, 101);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(50, target.GetAdjustedValue(51));
    }

    [TestMethod]
    public void LinearCalibration_Double_TwoPoints_Double() {
        var target = new LinearCalibration<double>();
        target.AddCalibrationPoint(0, 0);
        target.AddCalibrationPoint(100, 50);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(50, target.GetAdjustedValue(25));
    }


    [TestMethod]
    public void LinearCalibration_Double_ThreePoints_Simple() {
        var target = new LinearCalibration<double>();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(50, 51);
        target.AddCalibrationPoint(100, 101);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(25, target.GetAdjustedValue(26));
    }

    [TestMethod]
    public void LinearCalibration_Double_ThreePoints_Double() {
        var target = new LinearCalibration<double>();
        target.AddCalibrationPoint(0, 0);
        target.AddCalibrationPoint(50, 25);
        target.AddCalibrationPoint(100, 50);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(80, target.GetAdjustedValue(40));
    }

    [TestMethod]
    public void LinearCalibration_Double_ThreePoints_Halve() {
        var target = new LinearCalibration<double>();
        target.AddCalibrationPoint(0, 0);
        target.AddCalibrationPoint(50, 100);
        target.AddCalibrationPoint(100, 200);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(75, target.GetAdjustedValue(150));
    }

    [TestMethod]
    public void LinearCalibration_Double_ThreePoints_Freestyle_1() {
        var target = new LinearCalibration<double>();
        target.AddCalibrationPoint(0, 0.5);
        target.AddCalibrationPoint(50, 50);
        target.AddCalibrationPoint(100, 99.5);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(24.74747, target.GetAdjustedValue(25), 5);
        Assert.AreEqual(50, target.GetAdjustedValue(50));
        Assert.AreEqual(75.25253, target.GetAdjustedValue(75), 5);
    }

    [TestMethod]
    public void LinearCalibration_Double_ThreePoints_Freestyle_2() {
        var target = new LinearCalibration<double>();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(50, 49);
        target.AddCalibrationPoint(100, 100);
        Assert.AreEqual(0.99985, target.CorrelationCoefficient, 5);
        Assert.AreEqual(24.74747, target.GetAdjustedValue(25), 5);
        Assert.AreEqual(50, target.GetAdjustedValue(50));
        Assert.AreEqual(75.25253, target.GetAdjustedValue(75), 5);
    }

    [TestMethod]
    public void LinearCalibration_Double_ThreePoints_Freestyle_3() {
        var target = new LinearCalibration<double>();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(20, -10);
        target.AddCalibrationPoint(100, 50);
        Assert.AreEqual(0.93477, target.CorrelationCoefficient, 5);
        Assert.AreEqual(60.08439, target.GetAdjustedValue(25), 5);
        Assert.AreEqual(104.38819, target.GetAdjustedValue(50), 5);
        Assert.AreEqual(148.69198, target.GetAdjustedValue(75), 5);
    }


    [TestMethod]
    public void LinearCalibration_Double_Multipoint() {
        var target = new LinearCalibration<double>();
        target.AddCalibrationPoint(0, 0.025);
        target.AddCalibrationPoint(1, 0.217);
        target.AddCalibrationPoint(2, 0.388);
        target.AddCalibrationPoint(3, 0.634);
        target.AddCalibrationPoint(4, 0.777);
        target.AddCalibrationPoint(5, 1.011);
        target.AddCalibrationPoint(6, 1.166);
        Assert.AreEqual(0.9976, target.CoefficientOfDetermination, 4);
        Assert.AreEqual(1.19, target.GetAdjustedValue(0.254), 2);
    }

}

[TestClass]
public class LinearCalibration_Single_Tests {

    [TestMethod]
    public void LinearCalibration_Single_NoCalibration() {
        var target = new LinearCalibration<float>();
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(0, target.GetAdjustedValue(0));
        Assert.AreEqual(1, target.GetAdjustedValue(1));
    }


    [TestMethod]
    public void LinearCalibration_Single_SinglePoint() {
        var target = new LinearCalibration<float>();
        target.AddCalibrationPoint(0, 1);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(0, target.GetAdjustedValue(1));
    }

    [TestMethod]
    public void LinearCalibration_Single_SinglePoint_NoChange() {
        var target = new LinearCalibration<float>();
        target.AddCalibrationPoint(0, 0);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(1, target.GetAdjustedValue(1));
    }


    [TestMethod]
    public void LinearCalibration_Single_TwoPoints_Simple() {
        var target = new LinearCalibration<float>();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(100, 101);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(50, target.GetAdjustedValue(51));
    }

    [TestMethod]
    public void LinearCalibration_Single_TwoPoints_Double() {
        var target = new LinearCalibration<float>();
        target.AddCalibrationPoint(0, 0);
        target.AddCalibrationPoint(100, 50);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(50, target.GetAdjustedValue(25));
    }


    [TestMethod]
    public void LinearCalibration_Single_ThreePoints_Simple() {
        var target = new LinearCalibration<float>();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(50, 51);
        target.AddCalibrationPoint(100, 101);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(25, target.GetAdjustedValue(26));
    }

    [TestMethod]
    public void LinearCalibration_Single_ThreePoints_Double() {
        var target = new LinearCalibration<float>();
        target.AddCalibrationPoint(0, 0);
        target.AddCalibrationPoint(50, 25);
        target.AddCalibrationPoint(100, 50);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(80, target.GetAdjustedValue(40));
    }

    [TestMethod]
    public void LinearCalibration_Single_ThreePoints_Halve() {
        var target = new LinearCalibration<float>();
        target.AddCalibrationPoint(0, 0);
        target.AddCalibrationPoint(50, 100);
        target.AddCalibrationPoint(100, 200);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(75, target.GetAdjustedValue(150));
    }

    [TestMethod]
    public void LinearCalibration_Single_ThreePoints_Freestyle_1() {
        var target = new LinearCalibration<float>();
        target.AddCalibrationPoint(0, 0.5f);
        target.AddCalibrationPoint(50, 50);
        target.AddCalibrationPoint(100, 99.5f);
        Assert.AreEqual(1, target.CorrelationCoefficient);
        Assert.AreEqual(24.74747f, target.GetAdjustedValue(25), 5);
        Assert.AreEqual(50, target.GetAdjustedValue(50));
        Assert.AreEqual(75.25253f, target.GetAdjustedValue(75), 5);
    }

    [TestMethod]
    public void LinearCalibration_Single_ThreePoints_Freestyle_2() {
        var target = new LinearCalibration<float>();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(50, 49);
        target.AddCalibrationPoint(100, 100);
        Assert.AreEqual(0.99985f, target.CorrelationCoefficient, 5);
        Assert.AreEqual(24.74747f, target.GetAdjustedValue(25), 5);
        Assert.AreEqual(50f, target.GetAdjustedValue(50));
        Assert.AreEqual(75.25253f, target.GetAdjustedValue(75), 5);
    }

    [TestMethod]
    public void LinearCalibration_Single_ThreePoints_Freestyle_3() {
        var target = new LinearCalibration<float>();
        target.AddCalibrationPoint(0, 1);
        target.AddCalibrationPoint(20, -10);
        target.AddCalibrationPoint(100, 50);
        Assert.AreEqual(0.93477f, target.CorrelationCoefficient, 5);
        Assert.AreEqual(60.08439f, target.GetAdjustedValue(25), 5);
        Assert.AreEqual(104.38819f, target.GetAdjustedValue(50), 5);
        Assert.AreEqual(148.69198f, target.GetAdjustedValue(75), 5);
    }


    [TestMethod]
    public void LinearCalibration_Single_Multipoint() {
        var target = new LinearCalibration<float>();
        target.AddCalibrationPoint(0, 0.025f);
        target.AddCalibrationPoint(1, 0.217f);
        target.AddCalibrationPoint(2, 0.388f);
        target.AddCalibrationPoint(3, 0.634f);
        target.AddCalibrationPoint(4, 0.777f);
        target.AddCalibrationPoint(5, 1.011f);
        target.AddCalibrationPoint(6, 1.166f);
        Assert.AreEqual(0.9976, target.CoefficientOfDetermination, 4);
        Assert.AreEqual(1.19, target.GetAdjustedValue(0.254f), 2);
    }

}

#endif
