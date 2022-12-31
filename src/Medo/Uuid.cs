/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-12-31: Initial version

namespace Medo;

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

/// <summary>
/// Implements UUID version 7 as defined in RFC draft at https://datatracker.ietf.org/doc/html/draft-peabody-dispatch-new-uuid-format
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Uuid : IComparable<Guid>, IComparable<Uuid>, IEquatable<Uuid>, IEquatable<Guid> {

    private readonly static bool IsLittleEndian = BitConverter.IsLittleEndian;

    /// <summary>
    /// Creates a new instance filled with version 7 UUID.
    /// </summary>
    public Uuid() {
        Bytes = new byte[16];

        var ms = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Timestamp
        var msBytes = new byte[8];
        BinaryPrimitives.WriteInt64BigEndian(msBytes, ms);
        Buffer.BlockCopy(msBytes, 2, Bytes, 0, 6);

        // Randomness
        if (LastMillisecond != ms) {
            LastMillisecond = ms;
            RandomNumberGenerator.Fill(Bytes.AsSpan(6));  // 10-bit rand_a + all of rand_b (extra bits will be overwritten)
            RandomA = (ushort)(((Bytes[6] & 0x03) << 8) | Bytes[7]); // to use as monotonic random for future calls, using only 10 bits to have 2-bit counter rollover guard
        } else {
            RandomA++;
            Bytes[7] = (byte)(RandomA & 0xFF);  // lower bits of rand_a, high bits will be set with version
            RandomNumberGenerator.Fill(Bytes.AsSpan(8));  // rand_b
        }

        //Fixup
        Bytes[6] = (byte)(0x70 | ((RandomA >> 8) & 0x0F));  // set 4-bit version + high bits of rand_a
        Bytes[8] = (byte)(0x80 | (Bytes[8] & 0x3F));  // set 2-bit variant
    }

    /// <summary>
    /// Creates a new instance from given byte array.
    /// No check if array is version 7 UUID is made.
    /// </summary>
    /// <exception cref="ArgumentNullException">Buffer cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Buffer must be exactly 16 bytes in length.</exception>
    public Uuid(byte[] buffer) {
        if (buffer == null) { throw new ArgumentNullException(nameof(buffer), "Buffer cannot be null."); }
        if (buffer.Length != 16) { throw new ArgumentOutOfRangeException(nameof(buffer), "Buffer must be exactly 16 bytes in length."); }
        Bytes = new byte[16];
        Buffer.BlockCopy(buffer, 0, Bytes, 0, Bytes.Length);
    }

    /// <summary>
    /// Creates a new instance from given GUID bytes.
    /// No check if GUID is version 7 UUID is made.
    /// </summary>
    public Uuid(Guid guid) {
        Bytes = guid.ToByteArray();
    }


    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    private readonly byte[] Bytes;


    [ThreadStatic]
    private static long LastMillisecond;

    [ThreadStatic]
    private static ushort RandomA;


    /// <summary>
    /// A read-only instance of the Guid structure whose value is all zeros.
    /// Please note this is not a valid UUID7 as it lacks version bits.
    /// </summary>
    public static readonly Uuid Empty = new(new byte[16]);

    /// <summary>
    /// Returns new UUID version 7.
    /// </summary>
    /// <returns></returns>
    public static Uuid NewUuid7() {
        return new Uuid();
    }

    /// <summary>
    /// Returns current UUID version 7 as binary equivalent System.Guid.
    /// </summary>
    public Guid ToGuid() {
        return new Guid(Bytes);
    }

    /// <summary>
    /// Returns an array that contains UUID bytes.
    /// </summary>
    public byte[] ToByteArray() {
        var copy = new byte[16];
        Buffer.BlockCopy(Bytes, 0, copy, 0, copy.Length);
        return copy;
    }


    #region Overrides

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj) {
        if (obj is Uuid uuid) {
            return CompareArrays(Bytes, uuid.Bytes) == 0;
        } else if (obj is Guid guid) {
            return CompareArrays(Bytes, guid.ToByteArray()) == 0;
        }
        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() {
        return Bytes.GetHashCode();
    }

    /// <inheritdoc/>
    public override string ToString() {
        return $"{Bytes[0]:x2}{Bytes[1]:x2}{Bytes[2]:x2}{Bytes[3]:x2}-{Bytes[4]:x2}{Bytes[5]:x2}-{Bytes[6]:x2}{Bytes[7]:x2}-{Bytes[8]:x2}{Bytes[9]:x2}-{Bytes[10]:x2}{Bytes[11]:x2}{Bytes[12]:x2}{Bytes[13]:x2}{Bytes[14]:x2}{Bytes[15]:x2}";
    }

    #endregion Overrides

    #region Operators

    /// <inheritdoc/>
    public static bool operator ==(Uuid left, Uuid right) {
        return left.Equals(right);
    }

    /// <inheritdoc/>
    public static bool operator ==(Uuid left, Guid right) {
        return left.Equals(right);
    }

    /// <inheritdoc/>
    public static bool operator ==(Guid left, Uuid right) {
        return left.Equals(right);
    }

    /// <inheritdoc/>
    public static bool operator !=(Uuid left, Uuid right) {
        return !(left == right);
    }

    /// <inheritdoc/>
    public static bool operator !=(Uuid left, Guid right) {
        return !(left == right);
    }

    /// <inheritdoc/>
    public static bool operator !=(Guid left, Uuid right) {
        return !(left == right);
    }

    public static bool operator <(Uuid left, Uuid right) {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <(Uuid left, Guid right) {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <(Guid left, Uuid right) {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(Uuid left, Uuid right) {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >(Uuid left, Guid right) {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >(Guid left, Uuid right) {
        return left.CompareTo(right) > 0;
    }


    public static bool operator <=(Uuid left, Uuid right) {
        return left.CompareTo(right) is < 0 or 0;
    }

    public static bool operator <=(Uuid left, Guid right) {
        return left.CompareTo(right) is < 0 or 0;
    }

    public static bool operator <=(Guid left, Uuid right) {
        return left.CompareTo(right) is < 0 or 0;
    }

    public static bool operator >=(Uuid left, Uuid right) {
        return left.CompareTo(right) is > 0 or 0;
    }

    public static bool operator >=(Uuid left, Guid right) {
        return left.CompareTo(right) is > 0 or 0;
    }

    public static bool operator >=(Guid left, Uuid right) {
        return left.CompareTo(right) is > 0 or 0;
    }

    #endregion Operators

    #region IComparable<Guid>

    /// <inheritdoc/>
    public int CompareTo(Guid other) {
        return CompareArrays(Bytes, other.ToByteArray());
    }

    #endregion IComparable<Guid>

    #region IComparable<Uuid>

    /// <inheritdoc/>
    public int CompareTo(Uuid other) {
        return CompareArrays(Bytes, other.Bytes);
    }

    #endregion IComparable<Uuid>

    #region IEquatable<Uuid>

    /// <inheritdoc/>
    public bool Equals(Uuid other) {
        return CompareArrays(Bytes, other.Bytes) == 0;
    }

    #endregion IEquatable<Uuid>

    #region IEquatable<Guid>

    /// <inheritdoc/>
    public bool Equals(Guid other) {
        return CompareArrays(Bytes, other.ToByteArray()) == 0;
    }

    #endregion IEquatable<Guid>


    private static int CompareArrays(byte[] buffer1, byte[] buffer2) {
        Debug.Assert(buffer1.Length == 16);
        Debug.Assert(buffer2.Length == 16);
        var comparer = Comparer<byte>.Default;
        for (int i = 0; i < buffer1.Length; i++) {
            if (comparer.Compare(buffer1[i], buffer2[i]) < 0) { return -1; }
            if (comparer.Compare(buffer1[i], buffer2[i]) > 0) { return +1; }
        }
        return 0;  // they're equal
    }

}
