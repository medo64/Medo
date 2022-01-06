/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-11-25: Refactored to use pattern matching
//2021-03-24: Added Count property
//2021-03-04: Refactored for .NET 5
//2011-03-05: Moved to Medo.Math
//2010-05-14: Changed namespace from Medo.Math.Averaging to Medo.Math
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2 (DoNotRaiseExceptionsInUnexpectedLocations)
//2008-01-05: Moved to Medo.Math.Averaging
//2008-01-03: Added Resources
//2007-09-19: Moved to common

namespace Medo.Math {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Calculates exponential moving average for added items.
    /// </summary>
    /// <example>
    /// <code>
    /// var stats = new ExponentialMovingAverage();
    /// stats.Add(4);
    /// stats.Add(2);
    /// var output = stats.Average;
    /// </code>
    /// </example>
    public sealed class ExponentialMovingAverage {

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
        /// <param name="smoothingFactorCount">Number of items to use for calculation of smoothing factor.</param>
        /// <exception cref="ArgumentOutOfRangeException">Count must be larger than 0.</exception>
        public ExponentialMovingAverage(int smoothingFactorCount) {
            if (smoothingFactorCount < 1) { throw new ArgumentOutOfRangeException(nameof(smoothingFactorCount), "Count must be larger than 0."); }
            if (smoothingFactorCount < int.MaxValue) {
                _smoothingFactor = 2.0 / (smoothingFactorCount + 1);
            } else {
                _smoothingFactor = 2.0 / int.MaxValue;
            }
        }

        /// <summary>
        /// Creates a new instance.
        /// Only finite numbers from collection are added.
        /// </summary>
        /// <param name="smoothingFactorCount">Number of items to use for calculation.</param>
        /// <param name="collection">Collection.</param>
        /// <exception cref="ArgumentOutOfRangeException">Count must be larger than 0.</exception>
        public ExponentialMovingAverage(int smoothingFactorCount, IEnumerable<double> collection)
            : this(smoothingFactorCount) {
            AddRange(collection);
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="smoothingFactor">Smoothing factor. Must be between 0 and 1. Lower values result in a greated smooting.</param>
        /// <exception cref="ArgumentOutOfRangeException">Smoothing factor must be between 0 and 1 (inclusive).</exception>
        public ExponentialMovingAverage(double smoothingFactor) {
            var v = (smoothingFactor < 0);
            if (v || (smoothingFactor > 1)) { throw new ArgumentOutOfRangeException(nameof(smoothingFactor), "smoothingFactor", "Smoothing factor must be between 0 and 1."); }
            _smoothingFactor = smoothingFactor;
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


        /// <summary>
        /// Gets current count.
        /// </summary>
        public long Count {
            get { return _count; }
        }

        /// <summary>
        /// Returns average or NaN if there is no data to calculate.
        /// </summary>
        public double Average {
            get { return average; }
        }


        #region Algorithm

        private long _count;
        private readonly double _smoothingFactor;
        private double average = double.NaN;

        private void AddOne(double value) {
            if (!double.IsNaN(average)) {
                average += _smoothingFactor * (value - average);
            } else {
                average = value;
            }
            _count += 1;
        }

        #endregion Algorithm

    }
}
