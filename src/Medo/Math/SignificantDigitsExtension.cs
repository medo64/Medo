/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-12-04: Initial version

namespace Medo.Math;

using System;

/// <summary>
/// Extension allowing for rounding to a number of significant digits.
/// </summary>
public static class SignificantDigitsExtension {

    /// <summary>
    /// Rounds number to a significant digit count.
    /// </summary>
    /// <param name="number">Number to round.</param>
    /// <param name="significantDigits">Number of significant digits between 1 and 5.</param>
    /// <exception cref="ArgumentOutOfRangeException">Number of significant digits cannot be less than 1 or more than 7.</exception>
    public static double ToSignificantDigits(this double number, int significantDigits) {
        if (significantDigits is < 1 or > 5) { throw new ArgumentOutOfRangeException(nameof(significantDigits), "Number of significant digits cannot be less than 1 or more than 5."); }
        var magnitude = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(number))) + 1);
        var normalized = number / magnitude;
        var rounded = Math.Round(normalized, significantDigits);
        return rounded * magnitude;
    }

}
