/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-04-09: Thread-safe operation
//2022-04-07: Initial version

namespace Medo.Diagnostics;

using System;
using System.Threading;

/// <summary>
/// Unordered sequence that repeats only after all values have been exhausted.
/// For 16-bit integer, the sequence will go over all 65,536 values before repeating.
/// Class is thread-safe.
/// </summary>
/// <example>
/// <code>
/// using var seq = new SpreadShortSequence();
/// var refDes = seq.NextAsString();
/// </code>
/// </example>
public sealed class SpreadShortSequence {

    /// <summary>
    /// Creates a new sequence.
    /// </summary>
    public SpreadShortSequence() {
        var randomBytes = new byte[2];
        Random.Shared.NextBytes(randomBytes);
        State = BitConverter.ToUInt16(randomBytes);
    }

    /// <summary>
    /// Creates a new sequence.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    public SpreadShortSequence(ushort seed) {
        State = seed;
    }

    /// <summary>
    /// Creates a new sequence.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    public SpreadShortSequence(short seed) {
        State = (ushort)seed;
    }


    private uint State;
    private const uint Increment = 40499;  // prime number at (2^16)/ϕ


    /// <summary>
    /// Returns the next element in the sequence.
    /// </summary>
    public ushort Next() {
        return (ushort)Interlocked.Add(ref State, Increment); ;
    }

    /// <summary>
    /// Returns the next element in the sequence as a hexadecimal string.
    /// </summary>
    public string NextAsString() {
        return Next().ToString("X4");
    }

    /// <summary>
    /// Returns the next element in the sequence as a hexadecimal string.
    /// </summary>
    /// <param name="prefix">Prefix text.</param>
    public string NextAsString(string prefix) {
        return (prefix ?? "") + NextAsString();
    }

}
