/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2023-08-10: Initial version

namespace Medo.Security.Cryptography;

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

/// <summary>
/// Buffer that gets encrypted while in memory and destroyed upon disposal.
/// This is not a fully secure way to store data, but it is better than nothing.
/// On Windows, you should use ProtectedData methods instead.
/// Class is thread safe.
/// </summary>
/// <example>This sample shows how to initialize and use class.
/// <code>
/// using (var data = new ProtectedBuffer()) {
///     // store
///     data.ProtectData(arrayToProtect)
///
///     // load
///     var outputArray = data.UnprotectData();
///     try {
///         // do something with data
///     } finally {
///         Array.Clear(outputArray);
///     }
/// }
/// </code>
/// </example>
public sealed class ProtectedBuffer : IDisposable {

    /// <summary>
    /// Creates new instance.
    /// </summary>
    public ProtectedBuffer() {
        KeyBytes = new byte[16];
        var separator = new byte[(Environment.TickCount % 4096) * 16];  // separate key and IV a bit (aka security by obscurity);
        Random.GetBytes(separator, 0, separator.Length);
        IVBytes = new byte[16];

        KeyBytesHandle = GCHandle.Alloc(KeyBytes, GCHandleType.Pinned);
        Random.GetBytes(KeyBytes, 0, KeyBytes.Length);

        IVBytesHandle = GCHandle.Alloc(IVBytes, GCHandleType.Pinned);
        Random.GetBytes(IVBytes, 0, IVBytes.Length);
    }

    /// <summary>
    /// Disposes of the instance.
    /// </summary>
    ~ProtectedBuffer() {
        Dispose();
    }


    /// <summary>
    /// Protects given data buffer.
    /// Data will be cleared before returning.
    /// Maximum of 4064 bytes can be protected.
    /// </summary>
    /// <param name="data">Data to protect.</param>
    /// <exception cref="ArgumentNullException">Data cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Data must be be up to 4064 bytes.</exception>
    public void ProtectData(byte[] data) {
        ProtectData(data, clearData: true);
    }

    /// <summary>
    /// Protects given data buffer.
    /// Maximum of 4064 bytes can be protected.
    /// </summary>
    /// <param name="data">Data to protect.</param>
    /// <param name="clearData">If true, array will be cleared before function returns.</param>
    /// <exception cref="ArgumentNullException">Data cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Data can be up to 4064 bytes.</exception>
    public void ProtectData(byte[] data, bool clearData) {
        if (data == null) { throw new ArgumentNullException(nameof(data), "Data cannot be null."); }
        if (data.Length > 4064) { throw new ArgumentOutOfRangeException(nameof(data), "Data must be up to 4064 bytes."); }

        try {
            lock (SyncRoot) {
                if (KeyBytesHandle.IsAllocated == false) { throw new ObjectDisposedException(nameof(ProtectedBuffer), "Object has been disposed."); }
                if (IVBytesHandle.IsAllocated == false) { throw new ObjectDisposedException(nameof(ProtectedBuffer), "Object has been disposed."); }

                var blockCount = (data.Length + 15) / 16;
                var allocationLength = (blockCount + 1) * 16 + 1;  // always allocate 1 block extra for padding and an extra byte for length
                AllocateData(allocationLength);

                using var aes = Aes.Create();
                aes.Key = KeyBytes;
                aes.IV = IVBytes;
                aes.Padding = PaddingMode.PKCS7;

#pragma warning disable CA5401  // IV is generated using CSRNG
                using var encryptor = aes.CreateEncryptor();
#pragma warning restore CA5401
                using var memoryStream = new MemoryStream(DataBytes!, 0, DataBytes!.Length - 1);  // DataBytes are not null after AllocateData call
                using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                DataBytes[DataBytes.Length - 1] = (byte)(memoryStream.Position / 16);  // save block count - will be 255 block (maX)
            }
        } finally {
            if (clearData) { Array.Clear(data, 0, data.Length); }  // clear data before returning
        }
    }


    /// <summary>
    /// Returns true if data was successfully unprotected.
    /// Unused array bytes will be cleared.
    /// Length of unprotected data is returned in unprotectedDataLength.
    /// </summary>
    /// <param name="unprotectedData">Unprotected data bytes. Will be filled when function returns true.</param>
    /// <param name="unprotectedDataLength">Length of unprotected data. Will be filled when function returns true.</param>
    /// <exception cref="ArgumentNullException">Unprotected data array cannot be null.</exception>
    /// <exception cref="InvalidOperationException">Data has not been protected.</exception>
    public bool TryUnprotectData(byte[] unprotectedData, out int unprotectedDataLength) {
        return TryUnprotectData(unprotectedData, clearUnusedBytes: true, out unprotectedDataLength);
    }

    /// <summary>
    /// Returns true if data was successfully unprotected.
    /// Length of unprotected data is returned in unprotectedDataLength.
    /// </summary>
    /// <param name="unprotectedData">Unprotected data bytes. Will be filled when function returns true.</param>
    /// <param name="clearUnusedBytes">If true, unused array bytes will be cleared.</param>
    /// <param name="unprotectedDataLength">Length of unprotected data. Will be filled when function returns true.</param>
    /// <exception cref="ArgumentNullException">Unprotected data array cannot be null.</exception>
    /// <exception cref="InvalidOperationException">Data has not been protected.</exception>
    public bool TryUnprotectData(byte[] unprotectedData, bool clearUnusedBytes, out int unprotectedDataLength) {
        if (unprotectedData == null) { throw new ArgumentNullException(nameof(unprotectedData), "Unprotected data array cannot be null."); }

        lock (SyncRoot) {
            if (KeyBytesHandle.IsAllocated == false) { throw new ObjectDisposedException(nameof(ProtectedBuffer), "Object has been disposed."); }
            if (IVBytesHandle.IsAllocated == false) { throw new ObjectDisposedException(nameof(ProtectedBuffer), "Object has been disposed."); }

            if (DataBytes == null) { throw new InvalidOperationException("Data has not been protected."); }

            using var aes = Aes.Create();
            aes.Key = KeyBytes;
            aes.IV = IVBytes;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            using var memoryStream = new MemoryStream(unprotectedData, 0, unprotectedData.Length);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);
            unprotectedDataLength = 0;  // we'll overwrite later, this is just to keep finally happy
            try {
                var blockCount = DataBytes[DataBytes.Length - 1];
                cryptoStream.Write(DataBytes, 0, blockCount * 16);
                cryptoStream.FlushFinalBlock();
                unprotectedDataLength = (int)memoryStream.Position;
                return true;
            } catch (NotSupportedException) {  // Memory stream is not expandable
                return false;
            } catch (CryptographicException) {
                return false;
            } finally {
                if (clearUnusedBytes) {
                    for (var i = unprotectedDataLength; i < unprotectedData.Length; i++) {  // clear the rest of the buffer
                        unprotectedData[i] = 0;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Returns unprotected data.
    /// Array is pinned in memory (unless compiled as .NET Standard 2.0)
    /// </summary>
    /// <exception cref="InvalidOperationException">Data has not been protected. -or- Failed to unprotect data.</exception>
    public byte[] UnprotectData() {
        lock (SyncRoot) {
            if (KeyBytesHandle.IsAllocated == false) { throw new ObjectDisposedException(nameof(ProtectedBuffer), "Object has been disposed."); }
            if (IVBytesHandle.IsAllocated == false) { throw new ObjectDisposedException(nameof(ProtectedBuffer), "Object has been disposed."); }

            if (DataBytes == null) { throw new InvalidOperationException("Data has not been protected."); }

#if NET6_0_OR_GREATER
            var unprotectedData = GC.AllocateArray<byte>(DataBytes.Length, pinned: true);
            try {
                if (TryUnprotectData(unprotectedData, clearUnusedBytes: true, out var unprotectedDataLength)) {
                    var trimmedData = GC.AllocateArray<byte>(unprotectedDataLength, pinned: true);
                    Buffer.BlockCopy(unprotectedData, 0, trimmedData, 0, unprotectedDataLength);
                    return trimmedData!;
                } else {
                    throw new InvalidOperationException("Failed to unprotect data.");
                }
            } finally {
                Array.Clear(unprotectedData);
            }
#else
            var unprotectedData = new byte[DataBytes.Length];
            var unprotectedDataHandle = GCHandle.Alloc(unprotectedData, GCHandleType.Pinned);
            try {
                if (TryUnprotectData(unprotectedData, clearUnusedBytes: true, out var unprotectedDataLength)) {
                    var trimmedData = new byte[unprotectedDataLength];
                    Buffer.BlockCopy(unprotectedData, 0, trimmedData, 0, unprotectedDataLength);
                    return trimmedData!;  // not pinned
                } else {
                    throw new InvalidOperationException("Failed to unprotect data.");
                }
            } finally {
                Array.Clear(unprotectedData, 0, unprotectedData.Length);
                unprotectedDataHandle.Free();
            }
#endif
        }
    }


    #region IDisposable Members

    /// <inheritdoc />
    public void Dispose() {
        lock (SyncRoot) {
            if (KeyBytesHandle.IsAllocated) {
                Random.GetBytes(KeyBytes, 0, KeyBytes.Length);  // clear it with random data
                KeyBytesHandle.Free();
            }

            if (IVBytesHandle.IsAllocated) {
                Random.GetBytes(IVBytes, 0, IVBytes.Length);  // clear it with random data
                IVBytesHandle.Free();
            }

            FreeData();  // safe to call even if nothing was allocated
        }
        GC.SuppressFinalize(this);
    }

    #endregion


    #region Protected

    private readonly object SyncRoot = new();  // enforce thread safety
    private GCHandle KeyBytesHandle;
    private readonly byte[] KeyBytes;
    private readonly GCHandle IVBytesHandle;
    private readonly byte[] IVBytes;
    private byte[]? DataBytes;
    private GCHandle DataBytesHandle;
    private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();  // needed due to .NET Standard 2.0

    private void AllocateData(int byteLength) {
        lock (SyncRoot) {
            if ((DataBytes == null) || (byteLength > DataBytes.Length)) {  // allocate only if needed
                FreeData();  // free previous buffer, if any
                DataBytes = new byte[byteLength];
                DataBytesHandle = GCHandle.Alloc(DataBytes, GCHandleType.Pinned);  // Pin the array in memory
            }
            Random.GetBytes(DataBytes, 0, DataBytes.Length);  // clear it with random data
        }
    }

    private void FreeData() {
        lock (SyncRoot) {
            if (DataBytes == null) { return; }  // check if allocated first
            Random.GetBytes(DataBytes, 0, DataBytes.Length);  // clear it with random data
            DataBytesHandle.Free();
            DataBytes = null;
        }
    }

    #endregion Protected

}
