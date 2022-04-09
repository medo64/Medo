/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-04-09: Thread-safe operation
//2022-04-07: Initial version

namespace Medo.Diagnostics;

using System;
using System.Threading;

/// <summary>
/// Unordered sequence that repeats only after all values have been exhausted.
/// For 64-bit integer, the sequence will go over all 18,446,744,073,709,551,616 values before repeating.
/// Class is thread-safe.
/// </summary>
/// <example>
/// <code>
/// using var seq = new SpreadLongSequence();
/// var refDes = seq.NextAsString();
/// </code>
/// </example>
public sealed class SpreadLongSequence {

    /// <summary>
    /// Creates a new sequence.
    /// </summary>
    public SpreadLongSequence() {
        var randomBytes = new byte[8];
        Random.Shared.NextBytes(randomBytes);
        State = BitConverter.ToUInt64(randomBytes);
    }

    /// <summary>
    /// Creates a new sequence.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    public SpreadLongSequence(ulong seed) {
        State = seed;
    }

    /// <summary>
    /// Creates a new sequence.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    public SpreadLongSequence(long seed) {
        State = (ulong)seed;
    }


    private ulong State;
    private const ulong Increment = 11400714819323204461;  // prime number at (2^64)/ϕ


    /// <summary>
    /// Returns the next element in the sequence.
    /// </summary>
    public ulong Next() {
        return Interlocked.Add(ref State, Increment); ;
    }

    /// <summary>
    /// Returns the next element in the sequence as a hexadecimal string.
    /// </summary>
    public string NextAsString() {
        return Next().ToString("X16");
    }

    /// <summary>
    /// Returns the next element in the sequence as a hexadecimal string.
    /// </summary>
    /// <param name="prefix">Prefix text.</param>
    public string NextAsString(string prefix) {
        return (prefix ?? "") + NextAsString();
    }

}
