/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-02-03: Updated for .NET 6
//2010-05-14: Changed namespace
//2008-03-30: Initial version

namespace Medo.Extensions.HexString;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

/// <summary>
/// Conversions for hexadecimal byte arrays.
/// </summary>
public static class HexStringExtension {

    /// <summary>
    /// Converts bytes to hexadecimal string.
    /// </summary>
    /// <param name="buffer">Bytes to convert.</param>
    /// <exception cref="ArgumentNullException">Buffer must not be null.</exception>
    public static string ToHexString(this ReadOnlySpan<byte> buffer) {
        if (buffer == null) { throw new ArgumentNullException(nameof(buffer), "Buffer must not be null."); }
        if (buffer.Length == 0) { return String.Empty; }

        var sb = new StringBuilder();
        foreach (var b in buffer) {
            sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
        }
        return sb.ToString();
    }

    /// <summary>
    /// Converts bytes to hexadecimal string.
    /// </summary>
    /// <param name="buffer">Bytes to convert.</param>
    /// <exception cref="ArgumentNullException">Buffer must not be null.</exception>
    public static string ToHexString(this Span<byte> buffer) {
        if (buffer == null) { throw new ArgumentNullException(nameof(buffer), "Buffer must not be null."); }
        if (buffer.Length == 0) { return String.Empty; }

        return ToHexString((ReadOnlySpan<byte>)buffer);
    }

    /// <summary>
    /// Converts byte array to hexadecimal string.
    /// </summary>
    /// <param name="array">Array to convert.</param>
    /// <exception cref="ArgumentNullException">Array must not be null.</exception>
    public static string ToHexString(this byte[] array) {
        if (array == null) { throw new ArgumentNullException(nameof(array), "Array must not be null."); }

        return ToHexString(array.AsSpan());
    }

    /// <summary>
    /// Converts byte array to hexadecimal string.
    /// </summary>
    /// <param name="array">Array to convert.</param>
    /// <param name="offset">An offset in inArray.</param>
    /// <param name="length">The number of elements of inArray to convert.</param>
    /// <exception cref="ArgumentNullException">Array must not be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Offset must be non-negative number. -or- Length must be non-negative number. -or- Invalid length.</exception>
    public static string ToHexString(this byte[] array, int offset, int length) {
        if (array == null) { throw new ArgumentNullException(nameof(array), "Array must not be null."); }
        if (offset < 0) { throw new ArgumentOutOfRangeException(nameof(offset), "Offset must be non-negative number."); }
        if (length < 0) { throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative number."); }
        if ((array.Length - length) < offset) { throw new ArgumentOutOfRangeException(nameof(length), "Invalid length."); }

        return ToHexString(array.AsSpan(offset, length));
    }

    /// <summary>
    /// Converts byte array to hexadecimal string.
    /// </summary>
    /// <param name="value">Data to convert.</param>
    public static byte[] FromHexString(this string value) {
        if (value == null) { throw new ArgumentNullException(nameof(value), "Value must not be null."); }

        var result = new List<byte>();
        int start = 0;
        if (value.Length % 2 == 1) {
            result.Add(byte.Parse(value.AsSpan(0, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            start = 1;
        }
        for (int i = start; i < value.Length; i += 2) {
            result.Add(byte.Parse(value.AsSpan(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
        }
        return result.ToArray();
    }

}
