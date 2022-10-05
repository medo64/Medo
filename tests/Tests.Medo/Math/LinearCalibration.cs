using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Math;

namespace Tests;

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
