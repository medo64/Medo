/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-04-09: Thread-safe operation
//2022-04-07: Initial version

namespace Medo.Diagnostics;

using System;
using System.Threading;

/// <summary>
/// Unordered sequence that repeats only after all values have been exhausted.
/// For 32-bit integer, the sequence will go over all 4,294,967,296 values before repeating.
/// Class is thread-safe.
/// </summary>
/// <example>
/// <code>
/// using var seq = new SpreadIntSequence();
/// var refDes = seq.NextAsString();
/// </code>
/// </example>
public sealed class SpreadIntSequence {

    /// <summary>
    /// Creates a new sequence.
    /// </summary>
    public SpreadIntSequence() {
        var randomBytes = new byte[4];
        Random.Shared.NextBytes(randomBytes);
        State = BitConverter.ToUInt32(randomBytes);
    }

    /// <summary>
    /// Creates a new sequence.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    public SpreadIntSequence(uint seed) {
        State = seed;
    }

    /// <summary>
    /// Creates a new sequence.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    public SpreadIntSequence(int seed) {
        State = (uint)seed;
    }


    private uint State;
    private const uint Increment = 2654435761;  // prime number at (2^32)/ϕ


    /// <summary>
    /// Returns the next element in the sequence.
    /// </summary>
    public uint Next() {
        return Interlocked.Add(ref State, Increment); ;
    }

    /// <summary>
    /// Returns the next element in the sequence as a hexadecimal string.
    /// </summary>
    public string NextAsString() {
        return Next().ToString("X8");
    }

    /// <summary>
    /// Returns the next element in the sequence as a hexadecimal string.
    /// </summary>
    /// <param name="prefix">Prefix text.</param>
    public string NextAsString(string prefix) {
        return (prefix ?? "") + NextAsString();
    }

}
