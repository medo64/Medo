/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Math {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Calculating variance and standard deviation from the streaming data using Welford's online algorithm.
    /// </summary>
    public class WelfordVariance {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public WelfordVariance() { }

        /// <summary>
        /// Creates a new instance.
        /// Only finite numbers from collection are added.
        /// </summary>
        /// <param name="collection">Collection.</param>
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
        /// <exception cref="NullReferenceException">Collection cannot be null.</exception>
        public void AddRange(IEnumerable<double> collection) {
            if (collection == null) { throw new ArgumentNullException(nameof(collection), "Collection cannot be null."); }
            foreach (var value in collection) {
                if (double.IsFinite(value)) { AddOne(value); }
            }
        }

        #region Algorithm

        private void AddOne(double value) {
            count += 1;
            var delta = value - mean;
            mean += delta / count;
            var delta2 = value - mean;
            m2 += delta * delta2;
        }

        long count = 0;
        double mean = 0.0;
        double m2 = 0.0;

        #endregion Algorithm

        /// <summary>
        /// Gets current mean.
        /// Double.NaN if less than two values are present.
        /// </summary>
        public double Mean {
            get { return (count >= 2) ? mean : double.NaN; }
        }

        /// <summary>
        /// Gets current variance.
        /// Double.NaN if less than two values are present.
        /// </summary>
        public double Variance {
            get { return (count >= 2) ? m2 / count : double.NaN; }
        }

        /// <summary>
        /// Gets current sample variance.
        /// Double.NaN if less than two values are present.
        /// </summary>
        public double SampleVariance {
            get { return (count >= 2) ? m2 / (count - 1) : double.NaN; }
        }

        /// <summary>
        /// Gets current standard deviation.
        /// Double.NaN if less than two values are present.
        /// </summary>
        public double StandardDeviation {
            get { return (count >= 2) ? Math.Sqrt(Variance) : double.NaN; }
        }

        /// <summary>
        /// Gets current sample standard deviation.
        /// Double.NaN if less than two values are present.
        /// </summary>
        public double SampleStandardDeviation {
            get { return (count >= 2) ? Math.Sqrt(SampleVariance) : double.NaN; }
        }

        /// <summary>
        /// Gets current relative standard deviation.
        /// Double.NaN if less than two values are present.
        /// Value will be expressed as decimal number (e.g. 0.42 is 42%).
        /// </summary>
        public double RelativeStandardDeviation {
            get { return (count >= 2) ? SampleStandardDeviation / Math.Abs(Mean) : double.NaN; }
        }

    }
}
