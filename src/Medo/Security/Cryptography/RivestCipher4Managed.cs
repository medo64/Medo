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


    #region SymmetricAlgorithm

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV) {
        return new RivestCipher4ManagedTransform(rgbKey, rgbIV);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgbIV) {
        return new RivestCipher4ManagedTransform(rgbKey, rgbIV);
    }

    /// <inheritdoc />
    public override void GenerateIV() {
        IVValue = new byte[KeySizeValue / 8];
        RandomNumberGenerator.Fill(IVValue);
    }

    /// <inheritdoc />
    public override void GenerateKey() {
        KeyValue = new byte[KeySizeValue / 8];
        RandomNumberGenerator.Fill(KeyValue);
    }

    #endregion SymmetricAlgorithm


    #region SymmetricAlgorithm Overrides

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Block size must be 8 bits.</exception>
    public override int BlockSize {
        get => base.BlockSize;
        set {
            if (value != 8) { throw new CryptographicException("Block size must be 8 bits."); }
            base.BlockSize = value;
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Feedback size must be 8 bits.</exception>
    public override int FeedbackSize {
        get => base.FeedbackSize;
        set {
            if (value != 8) { throw new CryptographicException("Feedback size must be 8 bits."); }
            base.FeedbackSize = value;
        }
    }

    /// <inheritdoc />
    /// <exception cref="CryptographicException">Cipher mode is not supported.</exception>
    public override CipherMode Mode {
        get => base.Mode;
        set {  // stream cipher is closest to CBC
            if (value is not CipherMode.CBC) { throw new CryptographicException("Cipher mode is not supported."); }
            base.Mode = value;
        }
    }

    /// <inheritdoc />
    /// <exception cref="CryptographicException">Padding mode is not supported.</exception>
    public override PaddingMode Padding {
        get => base.Padding;
        set {  // no padding makes sense with RC4
            if (value is not PaddingMode.None) { throw new CryptographicException("Padding mode is not supported."); }
            base.Padding = value;
        }
    }

    #endregion SymmetricAlgorithm Overrides

}


/// <summary>
/// Performs a cryptographic transformation of data using the RC4 algorithm.
/// This class cannot be inherited.
/// </summary>
internal sealed class RivestCipher4ManagedTransform : ICryptoTransform {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="rgbKey">Key.</param>
    /// <param name="rgbIV">Optional IV.</param>
    /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
    internal RivestCipher4ManagedTransform(byte[] rgbKey, byte[]? rgbIV) {
        var key = new byte[rgbKey.Length + (rgbIV?.Length ?? 0)];
        Buffer.BlockCopy(rgbKey, 0, key, 0, rgbKey.Length);
        if (rgbIV != null) { Buffer.BlockCopy(rgbIV, 0, key, rgbKey.Length, rgbIV.Length); }  // just append IV to a key as Arc4 doesn't really do IV

        SBox = GC.AllocateUninitializedArray<byte>(256, pinned: true);

        for (var i = 0; i < 256; i++) {
            SBox[i] = (byte)i;
        }

        var j = 0;
        byte x;
        for (var i = 0; i < 256; i++) {
            j = (j + SBox[i] + key[i % key.Length]) % 256;
            x = SBox[i];
            SBox[i] = SBox[j];
            SBox[j] = x;
        }

        Array.Clear(key);
    }

    private readonly byte[] SBox;
    private byte I, J;


    /// <inheritdoc />
    public bool CanReuseTransform => false;

    /// <inheritdoc />
    public bool CanTransformMultipleBlocks => true;

    /// <inheritdoc />
    public int InputBlockSize => 1;  // in bytes

    /// <inheritdoc />
    public int OutputBlockSize => 1;  // in bytes

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


    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Input buffer cannot be null. -or- Output buffer cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Input offset must be non-negative number. -or- Output offset must be non-negative number. -or- Invalid input count. -or- Invalid input length. -or- Insufficient output buffer.</exception>
    public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset) {
        if (inputBuffer == null) { throw new ArgumentNullException(nameof(inputBuffer), "Input buffer cannot be null."); }
        if (outputBuffer == null) { throw new ArgumentNullException(nameof(outputBuffer), "Output buffer cannot be null."); }
        if (inputOffset < 0) { throw new ArgumentOutOfRangeException(nameof(inputOffset), "Input offset must be non-negative number."); }
        if (outputOffset < 0) { throw new ArgumentOutOfRangeException(nameof(outputOffset), "Output offset must be non-negative number."); }
        if ((inputCount <= 0) || (inputCount > inputBuffer.Length)) { throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input count."); }
        if ((inputBuffer.Length - inputCount) < inputOffset) { throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input length."); }
        if (outputOffset + inputCount > outputBuffer.Length) { throw new ArgumentOutOfRangeException(nameof(outputOffset), "Insufficient output buffer."); }

        byte x;
        for (var n = 0; n < inputCount; n++) {
            I = (byte)(I + 1);
            J = (byte)(J + SBox[I]);
            x = SBox[I];
            SBox[I] = SBox[J];
            SBox[J] = x;
            outputBuffer[outputOffset + n] = (byte)(inputBuffer[inputOffset + n] ^ SBox[(SBox[I] + SBox[J]) % 256]);
        }
        return inputCount;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Input buffer cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Input offset must be non-negative number. -or- Invalid input count. -or- Invalid input length.</exception>
    public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount) {
        if (inputBuffer == null) { throw new ArgumentNullException(nameof(inputBuffer), "Input buffer cannot be null."); }
        if (inputOffset < 0) { throw new ArgumentOutOfRangeException(nameof(inputOffset), "Input offset must be non-negative number."); }
        if ((inputCount < 0) || (inputCount > inputBuffer.Length)) { throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input count."); }
        if ((inputBuffer.Length - inputCount) < inputOffset) { throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input length."); }

        var outputBuffer = new byte[inputCount];
        if (inputCount > 0) { TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, 0); }
        return outputBuffer;
    }

}
