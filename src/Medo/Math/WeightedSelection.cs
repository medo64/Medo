/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-12-26: Initial version

namespace Medo.Math;

using System;
using System.Collections.Generic;

/// <summary>
/// Provies selection of object based on its relative weight.
/// If weight is 0, object will be considered non-eligible for selection.
/// Random selection will be based on pseudo-random numbers (not cryptographically secure).
/// Class is not thread-safe.
/// </summary>
/// <typeparam name="T">Object type.</typeparam>
public sealed class WeightedSelection<T> {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public WeightedSelection() {
        Random = new Random();
    }

    /// <summary>
    /// Creates a new instance with a custom seed.
    /// Usable for testing and repeatable output.
    /// </summary>
    /// <param name="seed">Seed for a Random Number Generator.</param>
    public WeightedSelection(int seed) {
        Random = new Random(seed);
    }

    private readonly Random Random;
    private readonly List<SingleItem> Elements = new();
    private int CurrentBound = 0;

    private record SingleItem {
        public SingleItem(int lowerBound, int upperBound, T item) {
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Item = item;
        }
        public int LowerBound { get; }
        public int UpperBound { get; }
        public T Item { get; }
    }


    /// <summary>
    /// Gets if a disabled item (weight 0) will be returned if no other non-zero weight item is present.
    /// </summary>
    public bool ReturnDisabledIfNoOtherItemQualifies;

    /// <summary>
    /// Gets number of elements.
    /// </summary>
    public int Count => Elements.Count;

    /// <summary>
    /// Adds object with a specified weight.
    /// </summary>
    /// <param name="weight">Weight. Must be between 0 and 65535.</param>
    /// <param name="item">Item.</param>
    /// <exception cref="ArgumentOutOfRangeException">Weight must be between 0 and 65535.</exception>
    public void Add(int weight, T item) {
        if (weight > ushort.MaxValue) { throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be between 0 and 65535."); }
        var lower = CurrentBound;
        var upper = CurrentBound + weight;
        CurrentBound = upper;
        Elements.Add(new SingleItem(lower, upper, item));
    }

    /// <summary>
    /// Retrieves an item taking bounds into consideration.
    /// Exception will be thrown if no items can be found.
    /// </summary>
    public T GetItem() {
        var elementCount = Elements.Count;
        if (elementCount > 0) {
            if ((CurrentBound == 0) && ReturnDisabledIfNoOtherItemQualifies) {  // all elements have weight 0
                var index = Random.Next(Elements.Count);
                return Elements[index].Item;
            }

            // some elements are non-zero
            var bound = Random.Next(CurrentBound);
            foreach (var element in Elements) {
                if ((bound >= element.LowerBound) && (bound < element.UpperBound)) {
                    return element.Item;
                }
            }
        }
        throw new InvalidOperationException("Cannot retrieve element.");
    }

}
