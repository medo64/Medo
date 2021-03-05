/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Math {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Calculates exponential moving average for added items.
    /// </summary>
    /// <remarks>
    /// All calculations are done with floats in order to harvest maximum precision.
    /// </remarks>
    public class ExponentialMovingAverage {

        /// <summary>
        /// Creates new instance with smoothing factor for 10 items (18.18%).
        /// </summary>
        public ExponentialMovingAverage()
            : this(10) {
        }

        /// <summary>
        /// Creates new instance with smoothing factor for 10 items (18.18%).
        /// Only finite numbers from collection are added.
        /// </summary>
        /// <param name="collection">Collection.</param>
        public ExponentialMovingAverage(IEnumerable<double> collection)
            : this(10, collection) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="count">Number of items to use for calculation of smoothing factor.</param>
        /// <exception cref="ArgumentOutOfRangeException">Count cannot be negative.</exception>
        public ExponentialMovingAverage(int count) {
            if (count < 0) { throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative."); }
            if (count == int.MaxValue) {
                smoothingFactor = 2.0 / (int.MaxValue);
            } else {
                smoothingFactor = 2.0 / (count + 1);
            }
        }

        /// <summary>
        /// Creates a new instance.
        /// Only finite numbers from collection are added.
        /// </summary>
        /// <param name="count">Number of items to use for calculation.</param>
        /// <param name="collection">Collection.</param>
        /// <exception cref="ArgumentOutOfRangeException">Count cannot be negative.</exception>
        public ExponentialMovingAverage(int count, IEnumerable<double> collection)
            : this(count) {
            AddRange(collection);
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="smoothingFactor">Smoothing factor. Must be between 0 and 1. Lower values result in a greated smooting.</param>
        /// <exception cref="ArgumentOutOfRangeException">Smoothing factor must be between 0 and 1 (inclusive).</exception>
        public ExponentialMovingAverage(double smoothingFactor) {
            if ((smoothingFactor < 0) || (smoothingFactor > 1)) { throw new ArgumentOutOfRangeException(nameof(smoothingFactor), "smoothingFactor", "Smoothing factor must be between 0 and 1."); }
            this.smoothingFactor = smoothingFactor;
        }

        /// <summary>
        /// Creates a new instance.
        /// Only finite numbers from collection are added.
        /// </summary>
        /// <param name="smoothingFactor">Smoothing factor. Must be between 0 and 1. Lower values result in a greated smooting.</param>
        /// <param name="collection">Collection.</param>
        /// <exception cref="ArgumentOutOfRangeException">Smoothing factor must be between 0 and 1.</exception>
        public ExponentialMovingAverage(double smoothingFactor, IEnumerable<double> collection)
            : this(smoothingFactor) {
            AddRange(collection);
        }


        /// <summary>
        /// Adds an item and returns current average.
        /// </summary>
        /// <param name="value">Value to be added.</param>
        /// <exception cref="ArgumentOutOfRangeException">Value must be a finite number.</exception>
        public void Add(double value) {
            if (!double.IsFinite(value)) { throw new ArgumentOutOfRangeException(nameof(value), "Value must be a finite number."); }
            AddOne(value);
        }

        /// <summary>
        /// Adds elements of collection.
        /// Only finite numbers from collection are added.
        /// </summary>
        /// <param name="collection">Collection to add.</param>
        /// <exception cref="NullReferenceException">Collection cannot be null.</exception>
        public void AddRange(IEnumerable<double> collection) {
            if (collection == null) { throw new ArgumentNullException(nameof(collection), "Collection cannot be null."); }
            foreach (var value in collection) {
                if (double.IsFinite(value)) { AddOne(value); }
            }
        }

        #region Algorithm

        private readonly double smoothingFactor;
        private double average = double.NaN;

        private void AddOne(double value) {
            if (!double.IsNaN(average)) {
                average += smoothingFactor * (value - average);
            } else {
                average = value;
            }
        }

        #endregion Algorithm

        /// <summary>
        /// Returns average or NaN if there is no data to calculate.
        /// </summary>
        public double Average {
            get { return average; }
        }

    }
}
