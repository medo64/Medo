using Xunit;
using Medo.Math;

namespace Tests.Medo.Math {
    public class LinearInterpolationTests {

        [Fact(DisplayName = "Middle (1)")]
        public void Middle_1() {
            var target = new LinearInterpolation();
            target.AddReferencePoint(2, 1);
            target.AddReferencePoint(6, 3);
            Assert.Equal(2, target.GetAdjustedValue(1));
            Assert.Equal(4, target.GetAdjustedValue(2));
            Assert.Equal(6, target.GetAdjustedValue(3));
        }

        [Fact(DisplayName = "Middle (2)")]
        public void Middle_2() {
            var target = new LinearInterpolation();
            target.AddReferencePoint(-1, -10);
            target.AddReferencePoint(1, 10);
            Assert.Equal(0, target.GetAdjustedValue(0));
            Assert.Equal(0.5, target.GetAdjustedValue(5));
        }

        [Fact(DisplayName = "Middle (3)")]
        public void Middle_3() {
            var target = new LinearInterpolation();
            target.AddReferencePoint(0, 0);
            target.AddReferencePoint(1, 10);
            Assert.Equal(0.5, target.GetAdjustedValue(5));
        }

        [Fact(DisplayName = "Below (1)")]
        public void Below_1() {
            var target = new LinearInterpolation();
            target.AddReferencePoint(1, 0);
            Assert.Equal(1, target.GetAdjustedValue(0));
            Assert.Equal(2, target.GetAdjustedValue(1));
        }

        [Fact(DisplayName = "Below (2)")]
        public void Below_2() {
            var target = new LinearInterpolation();
            target.AddReferencePoint(2, 1);
            target.AddReferencePoint(4, 2);
            target.AddReferencePoint(6, 3);
            Assert.Equal(8, target.GetAdjustedValue(4));
            Assert.Equal(10, target.GetAdjustedValue(5));
        }

        [Fact(DisplayName = "Above (1)")]
        public void Above_1() {
            var target = new LinearInterpolation();
            target.AddReferencePoint(11, 10);
            Assert.Equal(11, target.GetAdjustedValue(10));
            Assert.Equal(10, target.GetAdjustedValue(9));
        }

        [Fact(DisplayName = "Above (2)")]
        public void Above_2() {
            var target = new LinearInterpolation();
            target.AddReferencePoint(6, 3);
            target.AddReferencePoint(8, 4);
            target.AddReferencePoint(10, 5);
            Assert.Equal(2, target.GetAdjustedValue(1));
            Assert.Equal(4, target.GetAdjustedValue(2));
        }

        [Fact(DisplayName = "Three points")]
        public void CalibratedThreePoints() {
            var target = new LinearInterpolation();
            target.AddReferencePoint(1, 1.1);
            target.AddReferencePoint(2, 1.2);
            target.AddReferencePoint(3, 1.3);
            Assert.Equal(2.5, target.GetAdjustedValue(1.25));
        }

        [Fact(DisplayName = "Calibrated value hit")]
        public void CalibratedValueHit() {
            var target = new LinearInterpolation();
            target.AddReferencePoint(1, 0);
            target.AddReferencePoint(2, 1);
            target.AddReferencePoint(3, 2);
            Assert.Equal(3, target.GetAdjustedValue(2));
        }

        [Fact(DisplayName = "No calibration")]
        public void NoCalibration() {
            var target = new LinearInterpolation();
            Assert.Equal(0, target.GetAdjustedValue(0));
            Assert.Equal(1, target.GetAdjustedValue(1));
        }

        [Fact(DisplayName = "One point")]
        public void OnePoint() {
            var target = new LinearInterpolation();
            target.AddReferencePoint(0, 1);
            Assert.Equal(0, target.GetAdjustedValue(1));
        }

    }
}
