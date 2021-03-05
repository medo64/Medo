/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Math {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Calculates moving average for the added items.
    /// </summary>
    public class MovingAverage {

        /// <summary>
        /// Creates new instance with a total of 10 items.
        /// </summary>
        public MovingAverage()
            : this(10) {
        }

        /// <summary>
        /// Creates a new instance.
        /// Only finite numbers from collection are added.
        /// </summary>
        /// <param name="collection">Collection.</param>
        /// <exception cref="ArgumentNullException">Collection cannot be null.</exception>
        public MovingAverage(IEnumerable<double> collection)
            : this(10, collection) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="count">Number of items to use for calculation.</param>
        /// <exception cref="ArgumentOutOfRangeException">Count must be larger than 0.</exception>
        public MovingAverage(int count) {
            if (count < 1) { throw new ArgumentOutOfRangeException(nameof(count), "Count must be larger than 0."); }
            _count = count;
        }

        /// <summary>
        /// Creates a new instance.
        /// Only finite numbers from collection are added.
        /// </summary>
        /// <param name="count">Number of items to use for calculation.</param>
        /// <param name="collection">Collection.</param>
        /// <exception cref="ArgumentOutOfRangeException">Count must be larger than 0.</exception>
        /// <exception cref="ArgumentNullException">Collection cannot be null.</exception>
        public MovingAverage(int count, IEnumerable<double> collection)
            : this(count) {
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
        /// Returns average or NaN if there is no data to calculate.
        /// </summary>
        public double Average {
            get {
                if (_items.Count > 0) {
                    var count = _items.Count;
                    double sum = 0;
                    foreach (var value in _items) {
                        sum += value;
                    }
                    return (sum / count);
                } else {
                    return double.NaN;
                }
            }
        }


        #region Algorithm

        private readonly Queue<double> _items = new();
        private readonly int _count;

        private void AddOne(double value) {
            _items.Enqueue(value);
            while (_items.Count > _count) {
                _items.Dequeue();
            }
        }

        #endregion Algorithm

    }
}
