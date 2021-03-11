using Xunit;

namespace Medo.Tests.Math.LinearCalibration {
    using Medo.Math;

    public class Tests {

        [Fact(DisplayName = "No calibration")]
        public void NoCalibration() {
            var target = new LinearCalibration();
            Assert.Equal(1, target.CorrelationCoefficient);
            Assert.Equal(0, target.GetAdjustedValue(0));
            Assert.Equal(1, target.GetAdjustedValue(1));
        }


        [Fact(DisplayName = "Single point")]
        public void SinglePoint() {
            var target = new LinearCalibration();
            target.AddCalibrationPoint(0, 1);
            Assert.Equal(1, target.CorrelationCoefficient);
            Assert.Equal(0, target.GetAdjustedValue(1));
        }

        [Fact(DisplayName = "Single point - no change")]
        public void SinglePoint_NoChange() {
            var target = new LinearCalibration();
            target.AddCalibrationPoint(0, 0);
            Assert.Equal(1, target.CorrelationCoefficient);
            Assert.Equal(1, target.GetAdjustedValue(1));
        }


        [Fact(DisplayName = "Two points (1)")]
        public void TwoPoints_Simple() {
            var target = new LinearCalibration();
            target.AddCalibrationPoint(0, 1);
            target.AddCalibrationPoint(100, 101);
            Assert.Equal(1, target.CorrelationCoefficient);
            Assert.Equal(50, target.GetAdjustedValue(51));
        }

        [Fact(DisplayName = "Two points (2)")]
        public void TwoPoints_Double() {
            var target = new LinearCalibration();
            target.AddCalibrationPoint(0, 0);
            target.AddCalibrationPoint(100, 50);
            Assert.Equal(1, target.CorrelationCoefficient);
            Assert.Equal(50, target.GetAdjustedValue(25));
        }


        [Fact(DisplayName = "Three points (1)")]
        public void ThreePoints_Simple() {
            var target = new LinearCalibration();
            target.AddCalibrationPoint(0, 1);
            target.AddCalibrationPoint(50, 51);
            target.AddCalibrationPoint(100, 101);
            Assert.Equal(1, target.CorrelationCoefficient);
            Assert.Equal(25, target.GetAdjustedValue(26));
        }

        [Fact(DisplayName = "Three points (2)")]
        public void ThreePoints_Double() {
            var target = new LinearCalibration();
            target.AddCalibrationPoint(0, 0);
            target.AddCalibrationPoint(50, 25);
            target.AddCalibrationPoint(100, 50);
            Assert.Equal(1, target.CorrelationCoefficient);
            Assert.Equal(80, target.GetAdjustedValue(40));
        }

        [Fact(DisplayName = "Three points (3)")]
        public void ThreePoints_Halve() {
            var target = new LinearCalibration();
            target.AddCalibrationPoint(0, 0);
            target.AddCalibrationPoint(50, 100);
            target.AddCalibrationPoint(100, 200);
            Assert.Equal(1, target.CorrelationCoefficient);
            Assert.Equal(75, target.GetAdjustedValue(150));
        }

        [Fact(DisplayName = "Three points (4)")]
        public void ThreePoints_Freestyle_1() {
            var target = new LinearCalibration();
            target.AddCalibrationPoint(0, 0.5);
            target.AddCalibrationPoint(50, 50);
            target.AddCalibrationPoint(100, 99.5);
            Assert.Equal(1, target.CorrelationCoefficient);
            Assert.Equal(24.74747, target.GetAdjustedValue(25), 5);
            Assert.Equal(50, target.GetAdjustedValue(50));
            Assert.Equal(75.25253, target.GetAdjustedValue(75), 5);
        }

        [Fact(DisplayName = "Three points (5)")]
        public void ThreePoints_Freestyle_2() {
            var target = new LinearCalibration();
            target.AddCalibrationPoint(0, 1);
            target.AddCalibrationPoint(50, 49);
            target.AddCalibrationPoint(100, 100);
            Assert.Equal(0.99985, target.CorrelationCoefficient, 5);
            Assert.Equal(24.74747, target.GetAdjustedValue(25), 5);
            Assert.Equal(50, target.GetAdjustedValue(50));
            Assert.Equal(75.25253, target.GetAdjustedValue(75), 5);
        }

        [Fact(DisplayName = "Three points (6)")]
        public void ThreePoints_Freestyle_3() {
            var target = new LinearCalibration();
            target.AddCalibrationPoint(0, 1);
            target.AddCalibrationPoint(20, -10);
            target.AddCalibrationPoint(100, 50);
            Assert.Equal(0.93477, target.CorrelationCoefficient, 5);
            Assert.Equal(60.08439, target.GetAdjustedValue(25), 5);
            Assert.Equal(104.38819, target.GetAdjustedValue(50), 5);
            Assert.Equal(148.69198, target.GetAdjustedValue(75), 5);
        }


        [Fact(DisplayName = "Multi point")]
        public void Multipoint() {
            var target = new LinearCalibration();
            target.AddCalibrationPoint(0, 0.025);
            target.AddCalibrationPoint(1, 0.217);
            target.AddCalibrationPoint(2, 0.388);
            target.AddCalibrationPoint(3, 0.634);
            target.AddCalibrationPoint(4, 0.777);
            target.AddCalibrationPoint(5, 1.011);
            target.AddCalibrationPoint(6, 1.166);
            Assert.Equal(0.9976, target.CoefficientOfDetermination, 4);
            Assert.Equal(1.19, target.GetAdjustedValue(0.254), 2);
        }

    }
}
