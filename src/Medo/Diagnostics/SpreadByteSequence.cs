/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-04-07: Initial version

namespace Medo.Diagnostics;

using System;

/// <summary>
/// Unordered sequence that repeats only after all values have been exhausted.
/// For 8-bit integer, the sequence will go over all 256 values before repeating.
/// </summary>
/// <example>
/// <code>
/// using var seq = new SpreadByteSequence();
/// var refDes = seq.NextAsString();
/// </code>
/// </example>
public sealed class SpreadByteSequence {

    /// <summary>
    /// Creates a new sequence.
    /// </summary>
    public SpreadByteSequence() {
        var randomBytes = new byte[1];
        Random.Shared.NextBytes(randomBytes);
        State = randomBytes[0];
    }

    /// <summary>
    /// Creates a new sequence.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    public SpreadByteSequence(byte seed) {
        State = seed;
    }

    /// <summary>
    /// Creates a new sequence.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    public SpreadByteSequence(sbyte seed) {
        State = (byte)seed;
    }


    private byte State;
    private const byte Increment = 157;  // prime number at (2^8)/ϕ


    /// <summary>
    /// Returns the next element in the sequence.
    /// </summary>
    public byte Next() {
        State += Increment;
        return State;
    }

    /// <summary>
    /// Returns the next element in the sequence as a hexadecimal string.
    /// </summary>
    public string NextAsString() {
        return Next().ToString("X2");
    }

    /// <summary>
    /// Returns the next element in the sequence as a hexadecimal string.
    /// </summary>
    /// <param name="prefix">Prefix text.</param>
    public string NextAsString(string prefix) {
        return (prefix ?? "") + NextAsString();
    }

}
