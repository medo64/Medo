/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Math {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Calculates average for added items.
    /// </summary>
    public sealed class SimpleAverage {

        /// <summary>
        /// Creates new instance.
        /// </summary>
        public SimpleAverage() {
        }

        /// <summary>
        /// Creates a new instance.
        /// Only finite numbers from collection are added.
        /// </summary>
        /// <param name="collection">Collection.</param>
        /// <exception cref="ArgumentNullException">Collection cannot be null.</exception>
        public SimpleAverage(IEnumerable<double> collection)
            : this() {
            AddRange(collection);
        }


        /// <summary>
        /// Adds an value.
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
            get {
                return (_count > 0) ? _sum / _count : double.NaN;
            }
        }


        #region Algorithm

        private long _count;
        private double _sum;

        private void AddOne(double value) {
            _sum += value;
            _count += 1;
        }

        #endregion Algorithm

    }
}
