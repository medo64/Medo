/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-10-05: Added generic variant
//2021-03-06: Refactored for .NET 5
//2012-10-30: Initial version

namespace Medo.Math;

using System;
using System.Collections.Generic;
using System.Numerics;

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
#if NET7_0_OR_GREATER
[Obsolete("Use generic LinearCalibration instead (e.g. LinearCalibration<double>).")]
#endif
public sealed class LinearCalibration {

    /// <summary>
    /// Creates new instance.
    /// </summary>
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


#if NET7_0_OR_GREATER

/// <summary>
/// Linear calibration using a least square regression.
/// </summary>
/// <example>
/// <code>
/// var cal = new LinearCalibration&lt;double&gt;();
/// cal.AddCalibrationPoint(0, 1);     // actual value is 0°C but we measure 1°C
/// cal.AddCalibrationPoint(40, 42);   // actual value is 40°C but we measure 42°C
/// cal.AddCalibrationPoint(100, 99);  // actual value is 100°C but we measure 99
/// var output = target.GetAdjustedValue(26);  // gets presumed value when we measure 26°C
/// </code>
/// </example>
public sealed class LinearCalibration<T> where T : IFloatingPoint<T> {

    /// <summary>
    /// Creates new instance.
    /// </summary>
    public LinearCalibration() {
    }


    private readonly List<KeyValuePair<T, T>> CalibrationPoints = new();

    /// <summary>
    /// Adds new calibration point.
    /// </summary>
    /// <param name="knownValue">Reference value.</param>
    /// <param name="measuredValue">Measured value.</param>
    public void AddCalibrationPoint(T knownValue, T measuredValue) {
        CalibrationPoints.Add(new KeyValuePair<T, T>(knownValue, measuredValue));
        Recalculated = false;
    }


    private T _slope = T.One;
    internal T Slope {
        get {
            if (Recalculated == false) { Prepare(); }
            return _slope;
        }
        private set { _slope = value; }
    }

    private T _intercept = T.Zero;
    internal T Intercept {
        get {
            if (Recalculated == false) { Prepare(); }
            return _intercept;
        }
        private set { _intercept = value; }
    }

    private T _correlationCoefficient = T.One;
    /// <summary>
    /// Gets correlation coefficient for calibration data set (R).
    /// </summary>
    public T CorrelationCoefficient {
        get {
            if (Recalculated == false) { Prepare(); }
            return _correlationCoefficient;
        }
        private set { _correlationCoefficient = value; }
    }

    private T _coefficientOfDetermination = T.One;
    /// <summary>
    /// Gets coefficient of determination (R^2) that indicates how well regression line fits calibration points.
    /// </summary>
    public T CoefficientOfDetermination {
        get {
            if (Recalculated == false) { Prepare(); }
            return _coefficientOfDetermination;
        }
        private set { _coefficientOfDetermination = value; }
    }


    private bool Recalculated = false;

    private void Prepare() {
        if (CalibrationPoints.Count == 0) {  // no calibration
            Slope = T.One;
            Intercept = T.Zero;
            CorrelationCoefficient = T.One;
            CoefficientOfDetermination = T.One;
        } else if (CalibrationPoints.Count == 1) {  // no calibration - just offset
            Slope = T.One;
            Intercept = CalibrationPoints[0].Value - CalibrationPoints[0].Key;
            CorrelationCoefficient = T.One;
            CoefficientOfDetermination = T.One;
        } else {
            var n = (T)Convert.ChangeType(CalibrationPoints.Count, typeof(T));
            var sumX = T.Zero;
            var sumY = T.Zero;
            var sumX2 = T.Zero;
            var sumY2 = T.Zero;
            var sumXY = T.Zero;
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
            var r = mT / (T)Convert.ChangeType(double.Sqrt((double)Convert.ChangeType(mB * (n * sumY2 - sumY * sumY), typeof(double))), typeof(T));  // conversions because decimal doesn't do IRootFunctions

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
    public T GetAdjustedValue(T value) {
        return (value - Intercept) / Slope;
    }

}

#endif
