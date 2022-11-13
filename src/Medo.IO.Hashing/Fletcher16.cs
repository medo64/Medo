/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-11-13: Using unsigned integers for hash output
//2022-11-11: Using machine-endianness when bytes are returned
//2022-09-27: Moved to Medo.IO.Hashing
//            Inheriting from NonCryptographicHashAlgorithm
//2021-03-06: Refactored for .NET 5
//2017-04-24: Initial version.

namespace Medo.IO.Hashing;

using System;
using System.Buffers.Binary;
using System.IO.Hashing;

/// <summary>
/// Computes checksum using Fletcher-16 algorithm.
/// Only numbers 0 to 9 are supported.
/// </summary>
/// <example>
/// <code>
/// var checksum = new Fletcher16();
/// checksum.Append(new byte[] { 0x01, 0x02 });
/// var checksumValue = checksum.HashAsUInt16;
/// </code>
/// </example>
public sealed class Fletcher16 : NonCryptographicHashAlgorithm {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public Fletcher16()
        : base(2) { }


    #region NonCryptographicHashAlgorithm

    /// <inheritdoc/>
    public override void Append(ReadOnlySpan<byte> source) {
        ProcessBytes(source);
    }

    /// <inheritdoc/>
    public override void Reset() {
        ProcessInitialization();
    }

    protected override void GetCurrentHashCore(Span<byte> destination) {
        if (BitConverter.IsLittleEndian) {
            BinaryPrimitives.WriteUInt16LittleEndian(destination, HashAsUInt16);
        } else {
            BinaryPrimitives.WriteUInt16BigEndian(destination, HashAsUInt16);
        }
    }

    #endregion NonCryptographicHashAlgorithm


    #region Algorithm

    private int _sum1 = 0;
    private int _sum2 = 0;
    private const int _maximumRunningSum = int.MaxValue / 2; //to avoid modulus every run

    private void ProcessInitialization() {
        _sum1 = 0;
        _sum2 = 0;
    }

    private void ProcessBytes(ReadOnlySpan<byte> source) {
        foreach (var b in source) {
            _sum1 += b;
            _sum2 += _sum1;
            if (_sum2 > _maximumRunningSum) {
                _sum1 %= 255;
                _sum2 %= 255;
            }
        }

        _sum1 %= 255;
        _sum2 %= 255;
    }


    /// <summary>
    /// Gets hash as 16-bit integer.
    /// </summary>
    [Obsolete("Use HashAsUInt16 instead.")]
    public short HashAsInt16 => unchecked((short)HashAsUInt16);

    /// <summary>
    /// Gets hash as 16-bit integer.
    /// </summary>
    public ushort HashAsUInt16 => (ushort)((_sum2 << 8) | _sum1);

    #endregion Algorithm

}
