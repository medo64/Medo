/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-01-13: Optimizing a bit
//2022-01-12: Fixed large final block transformation
//2022-01-09: Initial version

namespace Medo.Security.Cryptography;

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

/// <summary>
/// Rabbit stream cipher implementation.
/// </summary>
/// <remarks>
/// See also:
/// - https://www.ecrypt.eu.org/stream/e2-rabbit.html
/// - https://www.ietf.org/rfc/rfc4503.txt
/// </remarks>
/// <code>
/// using var algorithm = new RabbitManaged();
/// using var transform = algorithm.CreateEncryptor(key, iv);
/// using var cs = new CryptoStream(outStream, transform, CryptoStreamMode.Write);
/// cs.Write(inStream, 0, inStream.Length);
/// </code>
public sealed class RabbitManaged : SymmetricAlgorithm {

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public RabbitManaged()
        : base() {
        base.KeySizeValue = KeySizeInBits;
        base.BlockSizeValue = BlockSizeInBits;
        base.FeedbackSizeValue = base.BlockSizeValue;
        base.LegalBlockSizesValue = new KeySizes[] { new KeySizes(BlockSizeInBits, BlockSizeInBits, 0) };  // 128-bit
        base.LegalKeySizesValue = new KeySizes[] { new KeySizes(KeySizeInBits, KeySizeInBits, 0) };  // 128-bit

        base.Mode = CipherMode.CBC;  // same as default
        base.Padding = PaddingMode.None;
    }


    #region SymmetricAlgorithm

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Key must be 128 bits (16 bytes). -or- IV must be 64 bits (8 bytes).</exception>
    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV) {
        return new RabbitManagedTransform(rgbKey, rgbIV, RabbitManagedTransformMode.Decrypt, Padding);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Key must be 128 bits (16 bytes). -or- IV must be 64 bits (8 bytes).</exception>
    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgbIV) {
        return new RabbitManagedTransform(rgbKey, rgbIV, RabbitManagedTransformMode.Encrypt, Padding);
    }

    /// <inheritdoc />
    public override void GenerateIV() {
        IVValue = new byte[IVSizeInBits / 8];  // IV is always 64 bits
        RandomNumberGenerator.Fill(IVValue);
    }

    /// <inheritdoc />
    public override void GenerateKey() {
        KeyValue = new byte[KeySizeInBits / 8];  // Key is always 128 bits
        RandomNumberGenerator.Fill(IVValue);
    }

    #endregion SymmetricAlgorithm

    #region SymmetricAlgorithm Overrides

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Block size must be 128 bits.</exception>
    public override int BlockSize {
        get => base.BlockSize;
        set {
            if (value != BlockSizeInBits) { throw new CryptographicException("Block size must be 128 bits."); }
            base.BlockSize = value;
        }
    }

    /// <inheritdoc />
    /// <exception cref="CryptographicException">Feedback not supported.</exception>
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
    private const int IVSizeInBits = 64;
    private const int BlockSizeInBits = 128;

    #endregion Constants

}


internal enum RabbitManagedTransformMode {
    Encrypt = 0,
    Decrypt = 1
}


/// <summary>
/// Performs a cryptographic transformation of data using the Rabbit algorithm.
/// This class cannot be inherited.
/// </summary>
internal sealed class RabbitManagedTransform : ICryptoTransform {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="rgbKey">128-bit key.</param>
    /// <param name="rgbIV">64-bit IV (optional).</param>
    /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Key must be 128 bits (16 bytes). -or- IV must be 64 bits (8 bytes).</exception>
    internal RabbitManagedTransform(byte[] rgbKey, byte[]? rgbIV, RabbitManagedTransformMode transformMode, PaddingMode paddingMode) {
        if (rgbKey == null) { throw new ArgumentNullException(nameof(rgbKey), "Key cannot be null."); }
        if (rgbKey.Length != 16) { throw new ArgumentOutOfRangeException(nameof(rgbKey), "Key must be 128 bits (16 bytes)."); }
        if ((rgbIV is not null) && (rgbIV.Length != 8)) { throw new ArgumentOutOfRangeException(nameof(rgbKey), "IV must be 64 bits (8 bytes)."); }
        TransformMode = transformMode;
        PaddingMode = paddingMode;

        X = GC.AllocateUninitializedArray<DWord>(8, pinned: true);
        C = GC.AllocateUninitializedArray<DWord>(8, pinned: true);

        G = GC.AllocateUninitializedArray<DWord>(8, pinned: true);
        S = GC.AllocateUninitializedArray<DWord>(4, pinned: true);

        DecryptionBuffer = GC.AllocateArray<byte>(16, pinned: true);

        SetupKey(rgbKey);
        if (rgbIV != null) { SetupIV(rgbIV); }
    }

    private readonly RabbitManagedTransformMode TransformMode;
    private readonly PaddingMode PaddingMode;

    #region ICryptoTransform

    /// <inheritdoc />
    public bool CanReuseTransform => false;

    /// <inheritdoc />
    public bool CanTransformMultipleBlocks => true;

    /// <inheritdoc />
    public int InputBlockSize => 16;  // in bytes

    /// <inheritdoc />
    public int OutputBlockSize => 16;  // in bytes

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Input buffer cannot be null. -or- Output buffer cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Input offset must be non-negative number. -or- Output offset must be non-negative number. -or- Invalid input count. -or- Invalid input length. -or- Insufficient output buffer.</exception>
    public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset) {
        if (inputBuffer == null) { throw new ArgumentNullException(nameof(inputBuffer), "Input buffer cannot be null."); }
        if (outputBuffer == null) { throw new ArgumentNullException(nameof(outputBuffer), "Output buffer cannot be null."); }
        if (inputOffset < 0) { throw new ArgumentOutOfRangeException(nameof(inputOffset), "Input offset must be non-negative number."); }
        if (outputOffset < 0) { throw new ArgumentOutOfRangeException(nameof(outputOffset), "Output offset must be non-negative number."); }
        if ((inputCount <= 0) || (inputCount % 16 != 0) || (inputCount > inputBuffer.Length)) { throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input count."); }
        if ((inputBuffer.Length - inputCount) < inputOffset) { throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input length."); }
        if (outputOffset + inputCount > outputBuffer.Length) { throw new ArgumentOutOfRangeException(nameof(outputOffset), "Insufficient output buffer."); }

        if (TransformMode == RabbitManagedTransformMode.Encrypt) {

            for (var i = 0; i < inputCount; i += 16) {
                ProcessBytes(inputBuffer, inputOffset + i, 16, outputBuffer, outputOffset + i);
            }
            return inputCount;

        } else {  // Decrypt

            var bytesWritten = 0;

            if (DecryptionBufferInUse) {  // process the last block of previous round
                ProcessBytes(DecryptionBuffer, 0, 16, outputBuffer, outputOffset);
                DecryptionBufferInUse = false;
                outputOffset += 16;
                bytesWritten += 16;
            }

            for (var i = 0; i < inputCount - 16; i += 16) {
                ProcessBytes(inputBuffer, inputOffset + i, 16, outputBuffer, outputOffset);
                outputOffset += 16;
                bytesWritten += 16;
            }

            if (PaddingMode == PaddingMode.None) {
                ProcessBytes(inputBuffer, inputOffset + bytesWritten, inputCount - bytesWritten, outputBuffer, outputOffset);
                return inputCount;
            } else {  // save last block without processing because decryption otherwise cannot detect padding in CryptoStream
                Buffer.BlockCopy(inputBuffer, inputOffset + inputCount - 16, DecryptionBuffer, 0, 16);
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

        if (TransformMode == RabbitManagedTransformMode.Encrypt) {

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
                    paddedLength = inputCount / 16 * 16 + 16; //to round to next whole block
                    paddedInputBuffer = new byte[paddedLength];
                    paddedInputOffset = 0;
                    Buffer.BlockCopy(inputBuffer, inputOffset, paddedInputBuffer, 0, inputCount);
                    var added = (byte)(paddedLength - inputCount);
                    for (var i = inputCount; i < inputCount + added; i++) {
                        paddedInputBuffer[i] = added;
                    }
                    break;

                case PaddingMode.Zeros:
                    paddedLength = (inputCount + 15) / 16 * 16; //to round to next whole block
                    paddedInputBuffer = new byte[paddedLength];
                    paddedInputOffset = 0;
                    Buffer.BlockCopy(inputBuffer, inputOffset, paddedInputBuffer, 0, inputCount);
                    break;

                case PaddingMode.ANSIX923:
                    paddedLength = inputCount / 16 * 16 + 16; //to round to next whole block
                    paddedInputBuffer = new byte[paddedLength];
                    paddedInputOffset = 0;
                    Buffer.BlockCopy(inputBuffer, inputOffset, paddedInputBuffer, 0, inputCount);
                    paddedInputBuffer[^1] = (byte)(paddedLength - inputCount);
                    break;

                case PaddingMode.ISO10126:
                    paddedLength = inputCount / 16 * 16 + 16; //to round to next whole block
                    paddedInputBuffer = new byte[paddedLength];
                    RandomNumberGenerator.Fill(paddedInputBuffer.AsSpan(inputCount));
                    paddedInputOffset = 0;
                    Buffer.BlockCopy(inputBuffer, inputOffset, paddedInputBuffer, 0, inputCount);
                    paddedInputBuffer[^1] = (byte)(paddedLength - inputCount);
                    break;

                default: throw new CryptographicException("Unsupported padding mode.");
            }

            var outputBuffer = new byte[paddedLength];

            int remainingBytes = 16;
            for (var i = 0; i < paddedLength; i += 16) {
                if (PaddingMode == PaddingMode.None) {  // padding None is special as it doesn't extend buffer
                    remainingBytes = (i + 16 > inputCount) ? inputCount % 16 : 16;
                }
                ProcessBytes(paddedInputBuffer, paddedInputOffset + i, remainingBytes, outputBuffer, i);
            }

            return outputBuffer;

        } else {  // Decrypt

            byte[] outputBuffer;

            if (PaddingMode == PaddingMode.None) {
                outputBuffer = new byte[inputCount];
            } else if (inputCount % 16 != 0) {
                throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input count.");
            } else {
                outputBuffer = new byte[inputCount + (DecryptionBufferInUse ? 16 : 0)];
            }

            var outputOffset = 0;

            if (DecryptionBufferInUse) {  // process leftover padding buffer to keep CryptoStream happy
                ProcessBytes(DecryptionBuffer, 0, 16, outputBuffer, 0);
                DecryptionBufferInUse = false;
                outputOffset = 16;
            }

            for (var i = 0; i < inputCount; i += 16) {
                var remainingBytes = (i + 16 > inputCount) ? inputCount % 16 : 16;
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
            Array.Clear(X);
            Array.Clear(C);
            Carry = DWord.False;
            Array.Clear(G);
            Array.Clear(S);
            Array.Clear(DecryptionBuffer);
        }
    }

    #endregion IDisposable

    #region Helpers

    private readonly byte[] DecryptionBuffer; // used to store last block under decrypting as to work around CryptoStream implementation details
    private bool DecryptionBufferInUse;

    private void ProcessBytes(byte[] inputBuffer, int inputOffset, int count, byte[] outputBuffer, int outputOffset) {
        CounterUpdate();
        NextState();
        ExtractOutput();
        for (int i = 0; i < count; i++) {
            var s = (Byte)(S[i / 4] >> ((i % 4) * 8));
            outputBuffer[outputOffset + i] = (Byte)(inputBuffer[inputOffset + i] ^ s);
        }
    }

    private static byte[] RemovePadding(byte[] outputBuffer, PaddingMode paddingMode) {
        if (paddingMode == PaddingMode.PKCS7) {
            var padding = outputBuffer[^1];
            if (padding is < 1 or > 16) { throw new CryptographicException("Invalid padding."); }
            for (var i = outputBuffer.Length - padding; i < outputBuffer.Length; i++) {
                if (outputBuffer[i] != padding) { throw new CryptographicException("Invalid padding."); }
            }
            var newOutputBuffer = new byte[outputBuffer.Length - padding];
            Buffer.BlockCopy(outputBuffer, 0, newOutputBuffer, 0, newOutputBuffer.Length);
            return newOutputBuffer;
        } else if (paddingMode == PaddingMode.Zeros) {
            var newOutputLength = outputBuffer.Length;
            for (var i = outputBuffer.Length - 1; i >= Math.Max(outputBuffer.Length - 16, 0); i--) {
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
            if (padding is < 1 or > 16) { throw new CryptographicException("Invalid padding."); }
            for (var i = outputBuffer.Length - padding; i < outputBuffer.Length - 1; i++) {
                if (outputBuffer[i] != 0) { throw new CryptographicException("Invalid padding."); }
            }
            var newOutputBuffer = new byte[outputBuffer.Length - padding];
            Buffer.BlockCopy(outputBuffer, 0, newOutputBuffer, 0, newOutputBuffer.Length);
            return newOutputBuffer;
        } else if (paddingMode == PaddingMode.ISO10126) {
            var padding = outputBuffer[^1];
            if (padding is < 1 or > 16) { throw new CryptographicException("Invalid padding."); }
            var newOutputBuffer = new byte[outputBuffer.Length - padding];
            Buffer.BlockCopy(outputBuffer, 0, newOutputBuffer, 0, newOutputBuffer.Length);
            return newOutputBuffer;
        } else {  // None
            return outputBuffer;
        }
    }

    #endregion Helpers

    #region Implementation

    private readonly DWord[] X;   // state variables
    private readonly DWord[] C;   // counter variable
    private DWord Carry;          // counter carry - actually just a boolean
    private static readonly DWord[] A = new DWord[8] { 0x4D34D34D, 0xD34D34D3, 0x34D34D34, 0x4D34D34D, 0xD34D34D3, 0x34D34D34, 0x4D34D34D, 0xD34D34D3 };
    private readonly DWord[] G;   // temporary state - to avoid allocating new array every time
    private readonly DWord[] S;   // temporary state - to avoid allocating new array every time

    private void SetupKey(byte[] key) {
        var k = GC.AllocateUninitializedArray<DWord>(4, pinned: true);
        for (var j = 0; j < 4; j++) {
            k[j] = new DWord(key, j * 4);
        }

        for (var j = 0; j < 8; j++) {
            var j2 = j / 2;
            if (j % 2 == 0) {
                X[j] = k[j2];
                C[j] = k[(j2 + 2) & 0x3].RotateLeft(16);
            } else {
                X[j] = (k[(j2 + 3) & 0x3] << 16) | (k[(j2 + 2) & 0x3] >> 16);
                C[j] = k[j2].MaskUpper() | k[(j2 + 1) & 0x3].MaskLower();
            }
        }
        Carry = DWord.False;

        Array.Clear(k, 0, k.Length);  // we might as well clean-up temporary key state

        // advance a bit
        for (var n = 0; n < 4; n++) {
            CounterUpdate();
            NextState();
        }

        // reinitialize counter variables
        for (var j = 0; j < 8; j++) {
            C[j] = C[j] ^ X[(j + 4) & 0x7];
        }
    }

    private void SetupIV(byte[] iv) {
        var i = GC.AllocateUninitializedArray<DWord>(4, pinned: true);
        i[0] = new DWord(iv, 0);
        i[2] = new DWord(iv, 4);
        i[1] = (i[0] >> 16) | i[2].MaskUpper();
        i[3] = (i[2] << 16) | i[0].MaskLower();

        for (var j = 0; j < 8; j++) {
            C[j] ^= i[j & 0x3];
        }

        Array.Clear(i, 0, i.Length);  // we might as well clean-up temporary IV expansion

        // advance a bit
        for (var n = 0; n < 4; n++) {
            CounterUpdate();
            NextState();
        }
    }

    private void CounterUpdate() {
        DWord b = Carry;
        DWord temp;
        for (int j = 0; j < 8; j++) {
            temp = C[j] + A[j] + b;
            b = (temp <= C[j]) ? DWord.True : DWord.False;  // same as temp div WORDSIZE
            C[j] = temp;
        }
        Carry = b;
    }

    private void NextState() {
        // g_func
        UInt64 temp;
        for (int i = 0; i < 8; i++) {
            temp = (UInt32)(X[i] + C[i]);
            temp *= temp;
            G[i] = (UInt32)(temp ^ (temp >> 32));
        }

        for (int j = 0; j < 8; j++) {
            if (j % 2 == 0) {
                X[j] = G[j] + G[(j + 7) & 0x7].RotateLeft(16) + G[(j + 6) & 0x7].RotateLeft(16);
            } else {
                X[j] = G[j] + G[(j + 7) & 0x7].RotateLeft(8) + G[(j + 6) & 0x7];
            }
        }
    }

    private void ExtractOutput() {
        S[0] = X[0] ^ (X[5] >> 16) ^ (X[3] << 16);
        S[1] = X[2] ^ (X[7] >> 16) ^ (X[5] << 16);
        S[2] = X[4] ^ (X[1] >> 16) ^ (X[7] << 16);
        S[3] = X[6] ^ (X[3] >> 16) ^ (X[1] << 16);
    }

    [DebuggerDisplay("{Value}")]
    private struct DWord {  // makes extracting bytes from uint faster and looks nicer
        private readonly UInt32 Value;

        public DWord(UInt32 value) : this() {
            Value = value;
        }

        public DWord(UInt64 value) : this() {
            Value = (UInt32)value;
        }

        public DWord(Byte[] buffer, int offset) : this() {
            Value = (UInt32)((buffer[offset + 3] << 24) | (buffer[offset + 2] << 16) | (buffer[offset + 1] << 8) | buffer[offset]);
        }

        public static implicit operator DWord(UInt32 value) {
            return new DWord(value);
        }

        public static explicit operator UInt32(DWord value) {
            return value.Value;
        }

        public static bool operator <=(DWord left, DWord right) {
            return left.Value <= right.Value;
        }

        public static bool operator >=(DWord left, DWord right) {
            return left.Value >= right.Value;
        }

        public static DWord operator +(DWord left, DWord right) {
            return unchecked(left.Value + right.Value);
        }

        public static DWord operator &(DWord left, DWord right) {
            return left.Value & right.Value;
        }

        public static DWord operator |(DWord left, DWord right) {
            return left.Value | right.Value;
        }

        public static DWord operator ^(DWord left, DWord right) {
            return left.Value ^ right.Value;
        }

        public static DWord operator <<(DWord value, int shiftAmount) {
            return value.Value << shiftAmount;
        }

        public static DWord operator >>(DWord value, int shiftAmount) {
            return value.Value >> shiftAmount;
        }

        public static readonly DWord False = 0;
        public static readonly DWord True = 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DWord RotateLeft(int n) {
            return (Value << n) | (Value >> (32 - n));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DWord MaskUpper() {
            return Value & 0xFFFF0000;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DWord MaskLower() {
            return Value & 0x0000FFFF;
        }
    }

    #endregion

}
