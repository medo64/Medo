/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-03-24: Added Count property
//2021-03-07: Refactored for .NET 5
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
    /// Calculates average for added items.
    /// </summary>
    /// <example>
    /// <code>
    /// var stats = new SimpleAverage();
    /// stats.Add(4);
    /// stats.Add(2);
    /// var output = stats.Average;
    /// </code>
    /// </example>
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
