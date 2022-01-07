/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-01-07: Initial version

namespace Medo.Security.Cryptography;

using System;
using System.Security.Cryptography;

/// <summary>
/// Rivest Cipher 4 (RC4) algorithm implementation.
/// </summary>
/// <code>
/// using var algorithm = new RivestCipher4Managed();
/// using var transform = algorithm.CreateEncryptor(key, iv);
/// using var cs = new CryptoStream(outStream, transform, CryptoStreamMode.Write);
/// cs.Write(inStream, 0, inStream.Length);
/// </code>
public sealed class RivestCipher4Managed : SymmetricAlgorithm {

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public RivestCipher4Managed()
        : base() {
        base.KeySizeValue = 128;
        base.BlockSizeValue = 8;
        base.FeedbackSizeValue = base.BlockSizeValue;
        base.LegalBlockSizesValue = new KeySizes[] { new KeySizes(8, 8, 0) };  // byte at a time
        base.LegalKeySizesValue = new KeySizes[] { new KeySizes(0, 2048, 8) };  // allow essentially anything
    }


    /// <inheritdoc />
    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV) {
        if (rgbKey == null) { throw new ArgumentNullException(nameof(rgbKey), "Key cannot be null."); }
        return new Arc4ManagedTransform(rgbKey, rgbIV);
    }

    /// <inheritdoc />
    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgbIV) {
        if (rgbKey == null) { throw new ArgumentNullException(nameof(rgbKey), "Key cannot be null."); }
        return new Arc4ManagedTransform(rgbKey, rgbIV);
    }

    private static readonly Lazy<RandomNumberGenerator> Rng = new(() => RandomNumberGenerator.Create());

    /// <inheritdoc />
    public override void GenerateIV() {
        IVValue = new byte[KeySizeValue / 8];
        Rng.Value.GetBytes(IVValue);
    }

    /// <inheritdoc />
    public override void GenerateKey() {
        KeyValue = new byte[KeySizeValue / 8];
        Rng.Value.GetBytes(KeyValue);
    }

}


/// <summary>
/// Performs a cryptographic transformation of data using the Arc4 algorithm.
/// This class cannot be inherited.
/// </summary>
public sealed class Arc4ManagedTransform : ICryptoTransform {
    internal Arc4ManagedTransform(byte[] key, byte[]? iv) {
        var fullKey = new byte[key.Length + (iv?.Length ?? 0)];
        Buffer.BlockCopy(key, 0, fullKey, 0, key.Length);
        if (iv != null) { Buffer.BlockCopy(iv, 0, fullKey, key.Length, iv.Length); }  // just append IV to a key as Arc4 doesn't really do IV

        #region Key Setup

        for (var i = 0; i < 256; i++) {
            SBox[i] = (byte)i;
        }

        var j = 0;
        byte x;
        for (var i = 0; i < 256; i++) {
            j = (j + SBox[i] + fullKey[i % fullKey.Length]) % 256;
            x = SBox[i];
            SBox[i] = SBox[j];
            SBox[j] = x;
        }

#pragma warning disable IDE0059 // Unnecessary assignment of a value
        x = 0;  // just to clear it's value from memory
#pragma warning restore IDE0059 // Unnecessary assignment of a value

        #endregion Key Setup

        Array.Clear(fullKey);
    }

    private readonly byte[] SBox = new byte[256];
    private byte I, J;


    /// <summary>
    /// Gets a value indicating whether the current transform can be reused.
    /// </summary>
    public bool CanReuseTransform { get { return false; } }

    /// <summary>
    /// Gets a value indicating whether multiple blocks can be transformed.
    /// </summary>
    public bool CanTransformMultipleBlocks { get { return true; } }

    /// <summary>
    /// Gets the input block size (in bytes).
    /// </summary>
    public int InputBlockSize { get { return 1; } }  // block is always 8 bits

    /// <summary>
    /// Gets the output block size (in bytes).
    /// </summary>
    public int OutputBlockSize { get { return 1; } }  // block is always 8 bits

    /// <summary>
    /// Releases resources.
    /// </summary>
    public void Dispose() {
        Dispose(true);
    }

    private void Dispose(bool disposing) {
        if (disposing) {
            Array.Clear(SBox, 0, SBox.Length);
        }
    }


    /// <summary>
    /// Transforms the specified region of the input byte array and copies the resulting transform to the specified region of the output byte array.
    /// </summary>
    /// <param name="inputBuffer">The input for which to compute the transform.</param>
    /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
    /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
    /// <param name="outputBuffer">The output to which to write the transform.</param>
    /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
    public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset) {
        if (inputBuffer == null) { throw new ArgumentNullException(nameof(inputBuffer), "Input buffer cannot be null."); }
        if (inputOffset < 0) { throw new ArgumentOutOfRangeException(nameof(inputOffset), "Offset must be non-negative number."); }
        if ((inputCount <= 0) || (inputCount > inputBuffer.Length)) { throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input count."); }
        if ((inputBuffer.Length - inputCount) < inputOffset) { throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input length."); }
        if (outputBuffer == null) { throw new ArgumentNullException(nameof(outputBuffer), "Output buffer cannot be null."); }
        if (outputOffset + inputCount > outputBuffer.Length) { throw new ArgumentOutOfRangeException(nameof(outputOffset), "Insufficient buffer."); }

        #region Data Encryption

        byte x;
        for (var n = 0; n < inputCount; n++) {
            I = (byte)(I + 1);
            J = (byte)(J + SBox[I]);
            x = SBox[I];
            SBox[I] = SBox[J];
            SBox[J] = x;
            outputBuffer[outputOffset + n] = (byte)(inputBuffer[inputOffset + n] ^ SBox[(SBox[I] + SBox[J]) % 256]);
        }
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        x = 0;  // just to clear it's value from memory
#pragma warning restore IDE0059 // Unnecessary assignment of a value

        #endregion Data Encryption

        return inputCount;
    }

    /// <summary>
    /// Transforms the specified region of the specified byte array.
    /// </summary>
    /// <param name="inputBuffer">The input for which to compute the transform.</param>
    /// <param name="inputOffset">The offset into the byte array from which to begin using data.</param>
    /// <param name="inputCount">The number of bytes in the byte array to use as data.</param>
    public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount) {
        if (inputCount < 0) { throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input count."); }

        var outputBuffer = new byte[inputCount];
        if (inputCount > 0) { TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, 0); }
        return outputBuffer;
    }
}
