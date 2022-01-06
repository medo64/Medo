/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-04-03: Refactoring
//2021-03-24: Added Count property
//2021-03-04: Refactoring
//2021-03-03: Initial version

namespace Medo.Math {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Calculating variance and standard deviation from the streaming data using Welford's online algorithm.
    /// </summary>
    /// <example>
    /// <code>
    /// var stats = new WelfordVariance();
    /// stats.Add(4);
    /// stats.Add(2);
    /// var mean = stats.Mean;
    /// var stdev = stats.StandardDeviation;
    /// var variance = stats.Variance;
    /// var sampleStdev = stats.SampleStandardDeviation;
    /// var sampleVariance = stats.SampleVariance;
    /// var relativeStDev = stats.RelativeStandardDeviation;
    /// </code>
    /// </example>
    public sealed class WelfordVariance {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public WelfordVariance() { }

        /// <summary>
        /// Creates a new instance.
        /// Only finite numbers from collection are added.
        /// </summary>
        /// <param name="collection">Collection.</param>
        /// <exception cref="ArgumentNullException">Collection cannot be null.</exception>
        public WelfordVariance(IEnumerable<double> collection) {
            AddRange(collection);
        }


        /// <summary>
        /// Adds value.
        /// Only finite numbers are supported (i.e. no NaN or infinity).
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
        /// <exception cref="ArgumentNullException">Collection cannot be null.</exception>
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
        /// Gets current mean.
        /// Double.NaN if less than two values are present.
        /// </summary>
        public double Mean {
            get { return (_count >= 2) ? _mean : double.NaN; }
        }

        /// <summary>
        /// Gets current variance.
        /// Double.NaN if less than two values are present.
        /// </summary>
        public double Variance {
            get { return (_count >= 2) ? _m2 / _count : double.NaN; }
        }

        /// <summary>
        /// Gets current sample variance.
        /// Double.NaN if less than two values are present.
        /// </summary>
        public double SampleVariance {
            get { return (_count >= 2) ? _m2 / (_count - 1) : double.NaN; }
        }

        /// <summary>
        /// Gets current standard deviation.
        /// Double.NaN if less than two values are present.
        /// </summary>
        public double StandardDeviation {
            get { return (_count >= 2) ? Math.Sqrt(Variance) : double.NaN; }
        }

        /// <summary>
        /// Gets current sample standard deviation.
        /// Double.NaN if less than two values are present.
        /// </summary>
        public double SampleStandardDeviation {
            get { return (_count >= 2) ? Math.Sqrt(SampleVariance) : double.NaN; }
        }

        /// <summary>
        /// Gets current relative standard deviation.
        /// Double.NaN if less than two values are present.
        /// Value will be expressed as decimal number (e.g. 0.42 is 42%).
        /// </summary>
        public double RelativeStandardDeviation {
            get { return (_count >= 2) ? SampleStandardDeviation / Math.Abs(Mean) : double.NaN; }
        }


        #region Algorithm

        private long _count;
        private double _mean;
        private double _m2;

        private void AddOne(double value) {
            _count += 1;
            var delta = value - _mean;
            _mean += delta / _count;
            var delta2 = value - _mean;
            _m2 += delta * delta2;
        }

        #endregion Algorithm

    }
}
