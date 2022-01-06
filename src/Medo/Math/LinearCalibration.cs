/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-03-06: Refactored for .NET 5
//2012-10-30: Initial version

namespace Medo.Math {
    using System.Collections.Generic;

    /// <summary>
    /// Linear calibration using a least square regression.
    /// </summary>
    /// <example>
    /// <code>
    /// var cal = new LinearCalibration();
    /// cal.AddCalibrationPoint(0, 1);     // actual value is 0°C but we measure 1°C
    /// cal.AddCalibrationPoint(40, 42);   // actual value is 40°C but we measure 42°C
    /// cal.AddCalibrationPoint(100, 99);  // actual value is 100°C but we measure 99
    /// var output = target.GetAdjustedValue(26);  // gets presumed value when we measure 26°C
    /// </code>
    /// </example>
    public sealed class LinearCalibration {

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <example>
        /// var cal = new LinearCalibration();
        /// cal.AddCalibrationPoint(0, 1);     // actual value is 0°C but we measure 1°C
        /// cal.AddCalibrationPoint(40, 42);   // actual value is 40°C but we measure 42°C
        /// cal.AddCalibrationPoint(100, 99);  // actual value is 100°C but we measure 99°C
        /// var output = target.GetAdjustedValue(26);  // gets actual value when we measure 26°C
        /// </example>
        public LinearCalibration() {
        }


        private readonly List<KeyValuePair<double, double>> CalibrationPoints = new();

        /// <summary>
        /// Adds new calibration point.
        /// </summary>
        /// <param name="knownValue">Reference value.</param>
        /// <param name="measuredValue">Measured value.</param>
        public void AddCalibrationPoint(double knownValue, double measuredValue) {
            CalibrationPoints.Add(new KeyValuePair<double, double>(knownValue, measuredValue));
            Recalculated = false;
        }


        private double _slope;
        internal double Slope {
            get {
                if (Recalculated == false) { Prepare(); }
                return _slope;
            }
            private set { _slope = value; }
        }

        private double _intercept;
        internal double Intercept {
            get {
                if (Recalculated == false) { Prepare(); }
                return _intercept;
            }
            private set { _intercept = value; }
        }

        private double _correlationCoefficient;
        /// <summary>
        /// Gets correlation coefficient for calibration data set (R).
        /// </summary>
        public double CorrelationCoefficient {
            get {
                if (Recalculated == false) { Prepare(); }
                return _correlationCoefficient;
            }
            private set { _correlationCoefficient = value; }
        }

        private double _coefficientOfDetermination;
        /// <summary>
        /// Gets coefficient of determination (R^2) that indicates how well regression line fits calibration points.
        /// </summary>
        public double CoefficientOfDetermination {
            get {
                if (Recalculated == false) { Prepare(); }
                return _coefficientOfDetermination;
            }
            private set { _coefficientOfDetermination = value; }
        }


        private bool Recalculated = false;

        private void Prepare() {
            if (CalibrationPoints.Count == 0) {  // no calibration
                Slope = 1;
                Intercept = 0;
                CorrelationCoefficient = 1;
                CoefficientOfDetermination = 1;
            } else if (CalibrationPoints.Count == 1) {  // no calibration - just offset
                Slope = 1;
                Intercept = CalibrationPoints[0].Value - CalibrationPoints[0].Key;
                CorrelationCoefficient = 1;
                CoefficientOfDetermination = 1;
            } else {
                double n = CalibrationPoints.Count;
                double sumX = 0;
                double sumY = 0;
                double sumX2 = 0;
                double sumY2 = 0;
                double sumXY = 0;
                foreach (var point in CalibrationPoints) {
                    var x = point.Key;
                    var y = point.Value;
                    sumX += x;
                    sumY += y;
                    sumX2 += x * x;
                    sumY2 += y * y;
                    sumXY += x * y;
                }

                var mT = (n * sumXY - sumX * sumY);
                var mB = (n * sumX2 - sumX * sumX);
                var m = mT / mB;
                var r = mT / System.Math.Sqrt(mB * (n * sumY2 - sumY * sumY));

                Slope = m;
                Intercept = (sumY / n) - m * (sumX / n);
                CorrelationCoefficient = r;
                CoefficientOfDetermination = r * r;
            }

            Recalculated = true;
        }


        /// <summary>
        /// Returns value adjusted using a least square regression.
        /// </summary>
        /// <param name="value">Value to adjust.</param>
        public double GetAdjustedValue(double value) {
            return (value - Intercept) / Slope;
        }

    }

}
