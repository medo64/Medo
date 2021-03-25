/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Math {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Calculates weighted moving average for the added items.
    /// </summary>
    public sealed class WeightedMovingAverage {

        /// <summary>
        /// Creates new instance with a total of 10 items.
        /// </summary>
        public WeightedMovingAverage()
            : this(10) {
        }

        /// <summary>
        /// Creates a new instance.
        /// Only finite numbers from collection are added.
        /// </summary>
        /// <param name="collection">Collection.</param>
        /// <exception cref="ArgumentNullException">Collection cannot be null.</exception>
        public WeightedMovingAverage(IEnumerable<double> collection)
            : this(10, collection) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="subsetSize">Number of items to use for calculation.</param>
        /// <exception cref="ArgumentOutOfRangeException">Count must be larger than 0.</exception>
        public WeightedMovingAverage(int subsetSize) {
            if (subsetSize < 1) { throw new ArgumentOutOfRangeException(nameof(subsetSize), "Count must be larger than 0."); }
            _subsetSize = subsetSize;
        }

        /// <summary>
        /// Creates a new instance.
        /// Only finite numbers from collection are added.
        /// </summary>
        /// <param name="subsetSize">Number of items to use for calculation.</param>
        /// <param name="collection">Collection.</param>
        /// <exception cref="ArgumentOutOfRangeException">Count must be larger than 0.</exception>
        /// <exception cref="ArgumentNullException">Collection cannot be null.</exception>
        public WeightedMovingAverage(int subsetSize, IEnumerable<double> collection)
            : this(subsetSize) {
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
                if (_items.Count > 0) {
                    var divider = 0.0;
                    double sum = 0.0;
                    int i = 0;
                    foreach (var value in _items) {
                        sum += value * (i + 1);
                        divider += (i + 1);
                        i += 1;
                    }
                    return sum / divider;
                } else {
                    return double.NaN;
                }
            }
        }


        #region Algorithm

        private long _count;
        private readonly Queue<double> _items = new();
        private readonly int _subsetSize;

        private void AddOne(double value) {
            _items.Enqueue(value);
            while (_items.Count > _subsetSize) {
                _items.Dequeue();
            }
            _count += 1;
        }

        #endregion Algorithm

    }
}
