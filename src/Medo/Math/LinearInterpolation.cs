/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-03-06: Refactored for .NET 5
//2011-03-13: Changed Add method
//            Line is approximated between two points when calculating adjustments below or above
//2010-04-24: Initial version

namespace Medo.Math {
    using System.Collections.Generic;

    /// <summary>
    /// Returns adjusted value based on given points.
    /// Value is interpolated between two nearest points.
    /// If interpolation needs to use all the points, check LinearCalibration.
    /// </summary>
    /// <example>
    /// <code>
    /// var cal = new LinearInterpolation();
    /// cal.AddReferencePoint(0, 1);     // actual value is 0°C but we measure 1°C
    /// cal.AddReferencePoint(40, 42);   // actual value is 40°C but we measure 42°C
    /// cal.AddReferencePoint(100, 99);  // actual value is 100°C but we measure 99
    /// var output = target.GetAdjustedValue(26);  // gets actual value when we measure 26°C - interpolated between 40°C and 100°C points
    /// </code>
    /// </example>
    public class LinearInterpolation {

        /// <summary>
        /// Creates new instance.
        /// </summary>
        public LinearInterpolation() { }


        /// <summary>
        /// Adds new calibration point.
        /// </summary>
        /// <param name="referenceValue">Reference value.</param>
        /// <param name="measuredValue">Value as measured by target device.</param>
        public void Add(double referenceValue, double measuredValue) {
            ReferencePoints.Add(referenceValue, measuredValue);
        }

        /// <summary>
        /// Returns value adjusted with linear aproximation between the two nearest calibration points.
        /// </summary>
        /// <param name="value">Value to adjust.</param>
        public double GetAdjustedValue(double value) {
            KeyValuePair<double, double>? itemBelowF = null;
            KeyValuePair<double, double>? itemBelowN = null;
            KeyValuePair<double, double>? itemAboveN = null;
            KeyValuePair<double, double>? itemAboveF = null;

            foreach (var item in ReferencePoints) {
                if (item.Value == value) {  // just sent it as output
                    return item.Key;
                } else if (item.Value < value) {  // store for future reference - it may be more than one.
                    itemBelowF = itemBelowN;
                    itemBelowN = item;
                } else if (item.Value > value) {  // first above limit
                    itemAboveF = itemAboveN;
                    itemAboveN = item;
                    if (itemAboveF.HasValue) { break; }
                }
            }

            if (itemBelowN.HasValue && itemAboveN.HasValue) {  // both reference points
                var range = itemAboveN.Value.Value - itemBelowN.Value.Value;
                var point = value - itemBelowN.Value.Value;
                var percentageAbove = point / range;
                var percentageBelow = 1 - percentageAbove;
                return value + (itemBelowN.Value.Key - itemBelowN.Value.Value) * percentageBelow + (itemAboveN.Value.Key - itemAboveN.Value.Value) * percentageAbove;
            } else if (itemBelowN.HasValue) {  // just lower reference point
                if (itemBelowF.HasValue) {
                    var m = (itemBelowF.Value.Key - itemBelowN.Value.Key) / ((itemBelowF.Value.Value - itemBelowN.Value.Value));
                    var b = itemBelowN.Value.Key - m * itemBelowN.Value.Value;
                    return m * value + b;
                } else {
                    return value + (itemBelowN.Value.Key - itemBelowN.Value.Value);  // just offset
                }
            } else if (itemAboveN.HasValue) {  // just upper reference point
                if (itemAboveF.HasValue) {
                    var m = (itemAboveF.Value.Key - itemAboveN.Value.Key) / ((itemAboveF.Value.Value - itemAboveN.Value.Value));
                    var b = itemAboveN.Value.Key - m * itemAboveN.Value.Value;
                    return m * value + b;
                } else {
                    return value + (itemAboveN.Value.Key - itemAboveN.Value.Value);  // just offset
                }
            } else {  // no reference point
                return value;
            }
        }

        private readonly SortedDictionary<double, double> ReferencePoints = new();

    }
}
