/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */
/* Algorithm designed by Ron Rivest (RSA Security) */

//2022-12-20: Renamed to RivestCipher4 (was RivestCipher4Managed)
//2022-04-07: Minor refactoring
//2022-01-13: Added padding support
//2022-01-07: Initial version

namespace Medo.Security.Cryptography;

using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

/// <summary>
/// Rivest Cipher 4 (RC4) algorithm implementation.
/// </summary>
/// <code>
/// using var algorithm = new RivestCipher4();
/// using var transform = algorithm.CreateEncryptor(key, iv);
/// using var cs = new CryptoStream(outStream, transform, CryptoStreamMode.Write);
/// cs.Write(inStream, 0, inStream.Length);
/// </code>
public sealed class RivestCipher4 : SymmetricAlgorithm {

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public RivestCipher4()
        : base() {
        base.KeySizeValue = KeySizeInBits;
        base.BlockSizeValue = BlockSizeInBits;  // let's assume this is a replacement for 128-bit block size algorithm and work from there
        base.FeedbackSizeValue = base.BlockSizeValue;
        base.LegalBlockSizesValue = new KeySizes[] { new KeySizes(8, 256, 8) };  // allow anything from 1 to 32 bytes; RC4 internally outputs 1 byte at a time
        base.LegalKeySizesValue = new KeySizes[] { new KeySizes(40, 256, 8) };  // code accepts anything anyhow

        base.Mode = CipherMode.CBC;  // same as default
        base.Padding = PaddingMode.None;
    }


    #region SymmetricAlgorithm

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV) {
        return new RivestCipher4Transform(rgbKey, rgbIV, RivestCipher4TransformMode.Decrypt, Padding, BlockSize);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgbIV) {
        return new RivestCipher4Transform(rgbKey, rgbIV, RivestCipher4TransformMode.Encrypt, Padding, BlockSize);
    }

    /// <inheritdoc />
    public override void GenerateIV() {
        IVValue = Array.Empty<byte>();  // default IV is 0 bytes
        RandomNumberGenerator.Fill(IVValue);
    }

    /// <inheritdoc />
    public override void GenerateKey() {
        KeyValue = new byte[KeySize / 8];
        RandomNumberGenerator.Fill(KeyValue);
    }

    #endregion SymmetricAlgorithm

    #region SymmetricAlgorithm Overrides

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Block size must be between 8 and 256. -or- Block size must be divisible by 8.</exception>
    public override int BlockSize {
        get => base.BlockSize;
        set {
            if (value is < 8 or > 256) { throw new CryptographicException("Block size must be between 8 and 256."); }
            if (value % 8 != 0) { throw new CryptographicException("Block size must be divisible by 8."); }
            base.BlockSize = value;
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Feedback not supported.</exception>
    public override int FeedbackSize {
        get => base.FeedbackSize;
        set {
            throw new CryptographicException("Feedback not supported.");
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
        set {
            base.Padding = value switch {
                PaddingMode.None => value,
                PaddingMode.PKCS7 => value,
                PaddingMode.Zeros => value,
                PaddingMode.ANSIX923 => value,
                PaddingMode.ISO10126 => value,
                _ => throw new CryptographicException("Padding mode is not supported."),
            };
        }
    }

    #endregion SymmetricAlgorithm Overrides

    #region Constants

    private const int KeySizeInBits = 128;
    private const int BlockSizeInBits = 128;

    #endregion Constants

}


file enum RivestCipher4TransformMode {
    Encrypt = 0,
    Decrypt = 1
}


/// <summary>
/// Performs a cryptographic transformation of data using the RC4 algorithm.
/// This class cannot be inherited.
/// </summary>
file sealed class RivestCipher4Transform : ICryptoTransform {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="rgbKey">Key.</param>
    /// <param name="rgbIV">Optional IV.</param>
    /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
    internal RivestCipher4Transform(byte[] rgbKey, byte[]? rgbIV, RivestCipher4TransformMode transformMode, PaddingMode paddingMode, int blockSize) {
        if (rgbKey == null) { throw new ArgumentNullException(nameof(rgbKey), "Key cannot be null."); }
        TransformMode = transformMode;
        PaddingMode = paddingMode;
        BlockSizeInBytes = blockSize / 8;

        SBox = GC.AllocateUninitializedArray<byte>(256, pinned: true);
        DecryptionBuffer = GC.AllocateArray<byte>(BlockSizeInBytes, pinned: true);

        var key = GC.AllocateUninitializedArray<byte>(rgbKey.Length + (rgbIV?.Length ?? 0), pinned: true);
        Buffer.BlockCopy(rgbKey, 0, key, 0, rgbKey.Length);
        if (rgbIV != null) { Buffer.BlockCopy(rgbIV, 0, key, rgbKey.Length, rgbIV.Length); }  // just append IV to a key as Arc4 doesn't really do IV

        SetupKey(key);

        Array.Clear(key);
    }

    private readonly RivestCipher4TransformMode TransformMode;
    private readonly PaddingMode PaddingMode;
    private readonly int BlockSizeInBytes;

    #region ICryptoTransform

    /// <inheritdoc />
    public bool CanReuseTransform => false;

    /// <inheritdoc />
    public bool CanTransformMultipleBlocks => true;

    /// <inheritdoc />
    public int InputBlockSize => BlockSizeInBytes;  // in bytes

    /// <inheritdoc />
    public int OutputBlockSize => BlockSizeInBytes;  // in bytes

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Input buffer cannot be null. -or- Output buffer cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Input offset must be non-negative number. -or- Output offset must be non-negative number. -or- Invalid input count. -or- Invalid input length. -or- Insufficient output buffer.</exception>
    public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset) {
        if (inputBuffer == null) { throw new ArgumentNullException(nameof(inputBuffer), "Input buffer cannot be null."); }
        if (outputBuffer == null) { throw new ArgumentNullException(nameof(outputBuffer), "Output buffer cannot be null."); }
        if (inputOffset < 0) { throw new ArgumentOutOfRangeException(nameof(inputOffset), "Input offset must be non-negative number."); }
        if (outputOffset < 0) { throw new ArgumentOutOfRangeException(nameof(outputOffset), "Output offset must be non-negative number."); }
        if ((inputCount <= 0) || (inputCount % BlockSizeInBytes != 0) || (inputCount > inputBuffer.Length)) { throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input count."); }  // while RC4 outputs 8-bit values; we still want stuff in 128-bit blocks
        if ((inputBuffer.Length - inputCount) < inputOffset) { throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input length."); }
        if (outputOffset + inputCount > outputBuffer.Length) { throw new ArgumentOutOfRangeException(nameof(outputOffset), "Insufficient output buffer."); }

        if (TransformMode == RivestCipher4TransformMode.Encrypt) {

            for (var i = 0; i < inputCount; i += BlockSizeInBytes) {
                ProcessBytes(inputBuffer, inputOffset + i, BlockSizeInBytes, outputBuffer, outputOffset + i);
            }
            return inputCount;

        } else {  // Decrypt

            var bytesWritten = 0;

            if (DecryptionBufferInUse) {  // process the last block of previous round
                ProcessBytes(DecryptionBuffer, 0, BlockSizeInBytes, outputBuffer, outputOffset);
                DecryptionBufferInUse = false;
                outputOffset += BlockSizeInBytes;
                bytesWritten += BlockSizeInBytes;
            }

            for (var i = 0; i < inputCount - BlockSizeInBytes; i += BlockSizeInBytes) {
                ProcessBytes(inputBuffer, inputOffset + i, BlockSizeInBytes, outputBuffer, outputOffset);
                outputOffset += BlockSizeInBytes;
                bytesWritten += BlockSizeInBytes;
            }

            if (PaddingMode == PaddingMode.None) {
                ProcessBytes(inputBuffer, inputOffset + bytesWritten, inputCount - bytesWritten, outputBuffer, outputOffset);
                return inputCount;
            } else {  // save last block without processing because decryption otherwise cannot detect padding in CryptoStream
                Buffer.BlockCopy(inputBuffer, inputOffset + inputCount - BlockSizeInBytes, DecryptionBuffer, 0, BlockSizeInBytes);
                DecryptionBufferInUse = true;
            }

            return bytesWritten;

        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Input buffer cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Input offset must be non-negative number. -or- Invalid input count. -or- Invalid input length.</exception>
    public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount) {
        if (inputBuffer == null) { throw new ArgumentNullException(nameof(inputBuffer), "Input buffer cannot be null."); }
        if (inputOffset < 0) { throw new ArgumentOutOfRangeException(nameof(inputOffset), "Input offset must be non-negative number."); }
        if ((inputCount < 0) || (inputCount > inputBuffer.Length)) { throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input count."); }
        if ((inputBuffer.Length - inputCount) < inputOffset) { throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input length."); }

        if (TransformMode == RivestCipher4TransformMode.Encrypt) {

            int paddedLength;
            byte[] paddedInputBuffer;
            int paddedInputOffset;
            switch (PaddingMode) {
                case PaddingMode.None:
                    paddedLength = inputCount;
                    paddedInputBuffer = inputBuffer;
                    paddedInputOffset = inputOffset;
                    break;

                case PaddingMode.PKCS7:
                    paddedLength = inputCount / BlockSizeInBytes * BlockSizeInBytes + BlockSizeInBytes; //to round to next whole block
                    paddedInputBuffer = new byte[paddedLength];
                    paddedInputOffset = 0;
                    Buffer.BlockCopy(inputBuffer, inputOffset, paddedInputBuffer, 0, inputCount);
                    var added = (byte)(paddedLength - inputCount);
                    for (var i = inputCount; i < inputCount + added; i++) {
                        paddedInputBuffer[i] = added;
                    }
                    break;

                case PaddingMode.Zeros:
                    paddedLength = (inputCount + (BlockSizeInBytes - 1)) / BlockSizeInBytes * BlockSizeInBytes; //to round to next whole block
                    paddedInputBuffer = new byte[paddedLength];
                    paddedInputOffset = 0;
                    Buffer.BlockCopy(inputBuffer, inputOffset, paddedInputBuffer, 0, inputCount);
                    break;

                case PaddingMode.ANSIX923:
                    paddedLength = inputCount / BlockSizeInBytes * BlockSizeInBytes + BlockSizeInBytes; //to round to next whole block
                    paddedInputBuffer = new byte[paddedLength];
                    paddedInputOffset = 0;
                    Buffer.BlockCopy(inputBuffer, inputOffset, paddedInputBuffer, 0, inputCount);
                    paddedInputBuffer[^1] = (byte)(paddedLength - inputCount);
                    break;

                case PaddingMode.ISO10126:
                    paddedLength = inputCount / BlockSizeInBytes * BlockSizeInBytes + BlockSizeInBytes; //to round to next whole block
                    paddedInputBuffer = new byte[paddedLength];
                    RandomNumberGenerator.Fill(paddedInputBuffer.AsSpan(inputCount));
                    paddedInputOffset = 0;
                    Buffer.BlockCopy(inputBuffer, inputOffset, paddedInputBuffer, 0, inputCount);
                    paddedInputBuffer[^1] = (byte)(paddedLength - inputCount);
                    break;

                default: throw new CryptographicException("Unsupported padding mode.");
            }

            var outputBuffer = new byte[paddedLength];

            int remainingBytes = BlockSizeInBytes;
            for (var i = 0; i < paddedLength; i += BlockSizeInBytes) {
                if (PaddingMode == PaddingMode.None) {  // padding None is special as it doesn't extend buffer
                    remainingBytes = (i + BlockSizeInBytes > inputCount) ? inputCount % BlockSizeInBytes : BlockSizeInBytes;
                }
                ProcessBytes(paddedInputBuffer, paddedInputOffset + i, remainingBytes, outputBuffer, i);
            }

            return outputBuffer;

        } else {  // Decrypt

            byte[] outputBuffer;

            if (PaddingMode == PaddingMode.None) {
                outputBuffer = new byte[inputCount];
            } else if (inputCount % BlockSizeInBytes != 0) {
                throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input count.");
            } else {
                outputBuffer = new byte[inputCount + (DecryptionBufferInUse ? BlockSizeInBytes : 0)];
            }

            var outputOffset = 0;

            if (DecryptionBufferInUse) {  // process leftover padding buffer to keep CryptoStream happy
                ProcessBytes(DecryptionBuffer, 0, BlockSizeInBytes, outputBuffer, 0);
                DecryptionBufferInUse = false;
                outputOffset = BlockSizeInBytes;
            }

            for (var i = 0; i < inputCount; i += BlockSizeInBytes) {
                var remainingBytes = (i + BlockSizeInBytes > inputCount) ? inputCount % BlockSizeInBytes : BlockSizeInBytes;
                ProcessBytes(inputBuffer, inputOffset + i, remainingBytes, outputBuffer, outputOffset + i);
            }

            return RemovePadding(outputBuffer, PaddingMode);

        }
    }

    #endregion ICryptoTransform

    #region IDisposable

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

    #endregion IDisposable

    #region Helpers

    private readonly byte[] DecryptionBuffer; // used to store last block under decrypting as to work around CryptoStream implementation details
    private bool DecryptionBufferInUse;

    private void ProcessBytes(byte[] inputBuffer, int inputOffset, int count, byte[] outputBuffer, int outputOffset) {
        for (int i = 0; i < count; i++) {
            outputBuffer[outputOffset + i] = (Byte)(inputBuffer[inputOffset + i] ^ NextOutput());
        }
    }

    private byte[] RemovePadding(byte[] outputBuffer, PaddingMode paddingMode) {
        if (paddingMode == PaddingMode.PKCS7) {
            var padding = outputBuffer[^1];
            if ((padding < 1) || (padding > BlockSizeInBytes)) { throw new CryptographicException("Invalid padding."); }
            for (var i = outputBuffer.Length - padding; i < outputBuffer.Length; i++) {
                if (outputBuffer[i] != padding) { throw new CryptographicException("Invalid padding."); }
            }
            var newOutputBuffer = new byte[outputBuffer.Length - padding];
            Buffer.BlockCopy(outputBuffer, 0, newOutputBuffer, 0, newOutputBuffer.Length);
            return newOutputBuffer;
        } else if (paddingMode == PaddingMode.Zeros) {
            var newOutputLength = outputBuffer.Length;
            for (var i = outputBuffer.Length - 1; i >= Math.Max(outputBuffer.Length - BlockSizeInBytes, 0); i--) {
                if (outputBuffer[i] != 0) {
                    newOutputLength = i + 1;
                    break;
                }
            }
            if (newOutputLength == outputBuffer.Length) {
                return outputBuffer;
            } else {
                var newOutputBuffer = new byte[newOutputLength];
                Buffer.BlockCopy(outputBuffer, 0, newOutputBuffer, 0, newOutputBuffer.Length);
                return newOutputBuffer;
            }
        } else if (paddingMode == PaddingMode.ANSIX923) {
            var padding = outputBuffer[^1];
            if ((padding < 1) || (padding > BlockSizeInBytes)) { throw new CryptographicException("Invalid padding."); }
            for (var i = outputBuffer.Length - padding; i < outputBuffer.Length - 1; i++) {
                if (outputBuffer[i] != 0) { throw new CryptographicException("Invalid padding."); }
            }
            var newOutputBuffer = new byte[outputBuffer.Length - padding];
            Buffer.BlockCopy(outputBuffer, 0, newOutputBuffer, 0, newOutputBuffer.Length);
            return newOutputBuffer;
        } else if (paddingMode == PaddingMode.ISO10126) {
            var padding = outputBuffer[^1];
            if ((padding < 1) || (padding > BlockSizeInBytes)) { throw new CryptographicException("Invalid padding."); }
            var newOutputBuffer = new byte[outputBuffer.Length - padding];
            Buffer.BlockCopy(outputBuffer, 0, newOutputBuffer, 0, newOutputBuffer.Length);
            return newOutputBuffer;
        } else {  // None
            return outputBuffer;
        }
    }

    #endregion Helpers

    #region Implementation

    private readonly byte[] SBox;
    private byte I, J;

    private void SetupKey(byte[] key) {
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
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte NextOutput() {
        I = (byte)(I + 1);
        J = (byte)(J + SBox[I]);
        (SBox[J], SBox[I]) = (SBox[I], SBox[J]);
        return SBox[(SBox[I] + SBox[J]) % 256];
    }

    #endregion Implementation

}
