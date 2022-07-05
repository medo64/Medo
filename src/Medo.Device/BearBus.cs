/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-07-04: Initial release

namespace Medo.Device;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Class handling BearBus access.
/// </summary>
public abstract class BearBus : IDisposable {

    internal protected BearBus(Stream stream) {
        if (stream == null) { throw new ArgumentNullException(nameof(stream), "Stream cannot be null."); }
        Stream = stream;

        ReadThread = new Thread(RunRead) {
            CurrentCulture = CultureInfo.InvariantCulture,
            IsBackground = true,
            Name = "BearBus:Read",
            Priority = ThreadPriority.Normal
        };
        ReadThread.Start();
    }

    private readonly Stream Stream;

    /// <summary>
    /// Creates a new instance that will restrict behavior to Host operations.
    /// </summary>
    /// <param name="stream">Stream.</param>
    public static BearBusHost CreateHost(Stream stream) {
        return new BearBusHost(stream);
    }

    /// <summary>
    /// Creates a new instance that will restrict behavior to Device operations.
    /// </summary>
    /// <param name="stream">Stream.</param>
    public static BearBusDevice CreateDevice(Stream stream) {
        return new BearBusDevice(stream);
    }


    #region Read

    private readonly Thread ReadThread;
    private readonly ManualResetEvent ReadCancelEvent = new(initialState: false);
    private readonly Queue<IBBPacket> ReadQueue = new();


    /// <summary>
    /// Reads one packet if it exists.
    /// </summary>
    /// <param name="packet">Output packet</param>
    private protected bool TryRead([MaybeNullWhen(false)] out IBBPacket packet) {
        lock (ReadQueue) {
            if (ReadQueue.Count > 0) {
                packet = ReadQueue.Dequeue();
                return true;
            } else {
                packet = null;
                return false;
            }
        }
    }

    private enum ParseState { SearchForHeader, InHeader, HeaderCRC8, ExtraData, DataCRC8, DataCRC16a, DataCRC16b }
    private ParseState ParsingState = ParseState.SearchForHeader;
    private byte ParsedDataCount = 0;
    private readonly byte[] ParsedData = new byte[247]; // maximum packet length
    private byte ParsedExtraDataExpected = 0;

    private void RunRead() {
        var buffer = new byte[1024];

        while (!ReadCancelEvent.WaitOne(0, false)) {
            try {
                var len = Stream.Read(buffer);
                if (len > 0) {
                    for (var i = 0; i < len; i++) {
                        var b = buffer[i];

                        ParsedData[ParsedDataCount] = b;

                        switch (ParsingState) {
                            case ParseState.SearchForHeader:
                                if (b == 0xBB) {
                                    ParsingState = ParseState.InHeader;
                                }
                                break;

                            case ParseState.InHeader:
                                if (ParsedDataCount == 3) {
                                    ParsingState = ParseState.HeaderCRC8;
                                }
                                break;

                            case ParseState.HeaderCRC8:
                                var headerCrc8 = BBPacket.GetCrc8(ParsedData, 0, 4);
                                if (headerCrc8 == ParsedData[4]) {
                                    var hasEmbeededData = (ParsedData[2] & 0x40) == 0x40;
                                    if (hasEmbeededData) {
                                        EnqueueReadPacket(ParsedData);
                                        ParsingState = ParseState.SearchForHeader;  // done with embeeded byte
                                    } else {
                                        ParsedExtraDataExpected = ParsedData[3];
                                        if (ParsedExtraDataExpected == 0) {
                                            EnqueueReadPacket(ParsedData);
                                            ParsingState = ParseState.SearchForHeader;  // done when no data
                                        } else {
                                            ParsingState = ParseState.ExtraData;
                                        }
                                    }
                                } else {  // CRC not OK
                                    //Debug.WriteLine("[BearBus] Invalid checksum 0x" + ParsedData[4].ToString("X2") + ", 0x" + headerCrc8.ToString("X2") + " expected");
                                    ParsingState = ParseState.SearchForHeader;
                                }
                                break;

                            case ParseState.ExtraData:
                                ParsedExtraDataExpected--;
                                if (ParsedExtraDataExpected == 0) {
                                    if (ParsedData[3] <= 13) {
                                        ParsingState = ParseState.DataCRC8;
                                    } else {
                                        ParsingState = ParseState.DataCRC16a;
                                    }
                                }
                                break;

                            case ParseState.DataCRC8:
                                var dataCrc8 = BBPacket.GetCrc8(ParsedData, 4, ParsedDataCount - 4);
                                if (dataCrc8 == ParsedData[ParsedDataCount]) {
                                    EnqueueReadPacket(ParsedData);
                                } else {  // CRC not OK
                                    //Debug.WriteLine("[BearBus] Invalid checksum 0x" + ParsedData[ParsedDataCount].ToString("X2") + ", 0x" + dataCrc8.ToString("X2") + " expected");
                                    ParsingState = ParseState.SearchForHeader;
                                }
                                break;

                            case ParseState.DataCRC16a:
                                ParsingState = ParseState.DataCRC16b;
                                break;

                            case ParseState.DataCRC16b:
                                var dataCrc16 = BBPacket.GetCrc16(ParsedData, 4, ParsedDataCount - 5);
                                var dataCrc16H = (byte)(dataCrc16 >> 8);
                                var dataCrc16L = (byte)(dataCrc16 & 0xFF);
                                if ((dataCrc16H == ParsedData[ParsedDataCount - 1]) && (dataCrc16L == ParsedData[ParsedDataCount])) {
                                    EnqueueReadPacket(ParsedData);
                                } else {  // CRC not OK
                                    //Debug.WriteLine("[BearBus] Invalid checksum 0x" + ParsedData[ParsedDataCount - 1].ToString("X2") + ParsedData[ParsedDataCount].ToString("X2") + ", 0x" + dataCrc16H.ToString("X2") + dataCrc16L.ToString("X2") + " expected");
                                }
                                ParsingState = ParseState.SearchForHeader;
                                break;
                        }

                        if (ParsingState == ParseState.SearchForHeader) {
                            ParsedDataCount = 0;
                        } else {
                            ParsedDataCount++;
                        }
                    }
                    continue;  // try immediatelly reading more
                }
            } catch (IOException) { }  // just a timeout
            Thread.Sleep(1);
        }
    }

    private void EnqueueReadPacket(byte[] bytes) {
        var isOriginHost = (ParsedData[1] & 0x80) == 0x80;
        var address = (byte)(ParsedData[1] & 0x7F);
        var isReplyOrError = (ParsedData[2] & 0x80) == 0x80;
        var hasEmbeededData = (ParsedData[2] & 0x40) == 0x40;
        var commandCode = (byte)(ParsedData[2] & 0x3F);
        var datumOrLength = ParsedData[3];

        if (hasEmbeededData) {
            EnqueueReadPacket(isOriginHost, address, isReplyOrError, commandCode, new byte[] { datumOrLength });
        } else if (datumOrLength == 0) {
            EnqueueReadPacket(isOriginHost, address, isReplyOrError, commandCode, Array.Empty<byte>());
        } else {
            var data = new byte[datumOrLength];
            Buffer.BlockCopy(bytes, 5, data, 0, datumOrLength);
            EnqueueReadPacket(isOriginHost, address, isReplyOrError, commandCode, data);
        }
    }

    private void EnqueueReadPacket(bool isOriginHost, byte address, bool isReplyOrError, byte commandCode, byte[] data) {
        if (isOriginHost) {
            switch (commandCode) {
                case 0x00: ReadQueue.Enqueue(new BBSystemHostPacket(address, data)); break;
                case 0x3D: ReadQueue.Enqueue(new BBPingPacket(address, isReplyOrError, data)); break;
                case 0x3E: ReadQueue.Enqueue(new BBStatusPacket(address, isReplyOrError, data)); break;
                case 0x3F: ReadQueue.Enqueue(new BBAddressPacket(address, isReplyOrError, data)); break;
                default: ReadQueue.Enqueue(new BBCustomPacket(address, isReplyOrError, commandCode, data)); break;
            }
        } else {
            switch (commandCode) {
                case 0x00: ReadQueue.Enqueue(new BBSystemDevicePacket(address, isReplyOrError, data)); break;
                case 0x3D: ReadQueue.Enqueue(new BBPingReplyPacket(address, isReplyOrError, data)); break;
                case 0x3E: ReadQueue.Enqueue(new BBStatusReplyPacket(address, isReplyOrError, data)); break;
                case 0x3F: ReadQueue.Enqueue(new BBAddressReplyPacket(address, isReplyOrError, data)); break;
                default: ReadQueue.Enqueue(new BBCustomReplyPacket(address, isReplyOrError, commandCode, data)); break;
            }
        }
    }

    #endregion Read


    #region Write

    private readonly SemaphoreSlim WriteSemaphore = new(1);

    /// <summary>
    /// Sends packet to the stream.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    private protected void Write(IBBPacket packet) {
        var bytes = packet.ToBytes();
        WriteSemaphore.Wait();
        try {
            Stream.Write(bytes);
        } finally {
            WriteSemaphore.Release();
        }
    }

    /// <summary>
    /// Sends packet to the stream.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    private protected async Task WriteAsync(IBBPacket packet, CancellationToken cancellationToken) {
        var bytes = packet.ToBytes();
        await WriteSemaphore.WaitAsync(cancellationToken);
        try {
            await Stream.WriteAsync(bytes, cancellationToken);
        } finally {
            WriteSemaphore.Release();
        }
    }

    #endregion Write


    #region IDisposable

    private bool disposedValue;

    protected virtual void Dispose(bool disposing) {
        if (!disposedValue) {
            if (disposing) {
                ReadCancelEvent.Set();
                Thread.Sleep(100);  // give it a bit of time to process cancel but don't bother checking
                Stream.Flush();
                Stream.Close();
            }
            disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion IDisposable

}


/// <summary>
/// BearBus host.
/// </summary>
public sealed class BearBusHost : BearBus {

    /// <summary>
    /// Createsa a new instance.
    /// </summary>
    /// <param name="stream">Stream.</param>
    /// <exception cref="ArgumentNullException">Stream cannot be null.</exception>
    public BearBusHost(Stream stream)
        : base(stream) {
    }


    /// <summary>
    /// Returns true if packet is received.
    /// Any non-device packet is dropped.
    /// </summary>
    /// <param name="packet">Packet received.</param>
    public bool TryRead([MaybeNullWhen(false)] out IBBDevicePacket packet) {
        while (base.TryRead(out var genericPacket)) {
            if (genericPacket is IBBDevicePacket devicePacket) {
                packet = devicePacket;
                return true;
            }
        }
        packet = null;
        return false;
    }

    /// <summary>
    /// Sends a packet.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    public void Write(IBBHostPacket packet) {
        if (packet == null) { throw new ArgumentNullException(nameof(packet), "Packet cannot be null."); }
        base.Write(packet);
    }

}


/// <summary>
/// BearBus device.
/// </summary>
public sealed class BearBusDevice : BearBus {

    /// <summary>
    /// Createsa a new instance.
    /// </summary>
    /// <param name="stream">Stream.</param>
    /// <exception cref="ArgumentNullException">Stream cannot be null.</exception>
    public BearBusDevice(Stream stream)
        : base(stream) {
    }


    /// <summary>
    /// Returns true if packet is received.
    /// Any non-host packet is dropped.
    /// </summary>
    /// <param name="packet">Packet received.</param>
    public bool TryRead([MaybeNullWhen(false)] out IBBHostPacket packet) {
        while (base.TryRead(out var genericPacket)) {
            if (genericPacket is IBBHostPacket hostPacket) {
                packet = hostPacket;
                return true;
            }
        }
        packet = null;
        return false;
    }

    /// <summary>
    /// Sends a packet.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    public void Write(IBBDevicePacket packet) {
        if (packet == null) { throw new ArgumentNullException(nameof(packet), "Packet cannot be null."); }
        base.Write(packet);
    }

}


/// <summary>
/// Base record for BearBus packet.
/// </summary>
public abstract record BBPacket {

    /// <summary>
    /// Creates a new instance.
    /// Only for internal use.
    /// Incorrect data is silently filtered.
    /// </summary>
    internal protected BBPacket(bool isOriginHost, byte address, bool isReplyRequestOrError, byte commandCode, byte[] data) {
        IsOriginHost = isOriginHost;
        Address = (byte)(address & 0x7F);
        IsReplyRequestOrError = isReplyRequestOrError;
        CommandCode = (byte)(commandCode & 0x3F);

        if (data == null) { data = Array.Empty<byte>(); }
        var newData = new byte[data.Length <= 240 ? data.Length : 240];
        Buffer.BlockCopy(data, 0, newData, 0, newData.Length);
        DataBytes = newData;
    }


    /// <summary>
    /// Gets if packet if from host.
    /// </summary>
    protected bool IsOriginHost { get; }

    /// <summary>
    /// Gets destination address (if origin is host) or source address (if origin is device).
    /// </summary>
    protected byte Address { get; }

    /// <summary>
    /// Address from which packet originated.
    /// Null if packet originated from host.
    /// 0 if packet originated from unknown device.
    /// 1-127 otherwise.
    /// </summary>
    public byte? FromAddress {
        get => IsOriginHost ? null : Address;
    }

    /// <summary>
    /// Address to which packet is destined.
    /// Null if packet is for host.
    /// 0 if packet is broadcast.
    /// 1-127 otherwise.
    /// </summary>
    public byte? ToAddress {
        get => IsOriginHost ? Address : null;
    }

    /// <summary>
    /// Gets if reply is requested (if origin is host) or if reply is error (if origin is device).
    /// </summary>
    protected bool IsReplyRequestOrError { get; }

    /// <summary>
    /// Gets command code.
    /// </summary>
    public byte CommandCode { get; }

    private readonly byte[] DataBytes;

    /// <summary>
    /// Gets data bytes.
    /// </summary>
    public byte[] Data {
        get {
            var outputData = new byte[DataBytes.Length];
            Buffer.BlockCopy(DataBytes, 0, outputData, 0, outputData.Length);
            return outputData;
        }
    }

    /// <summary>
    /// Gets value of a single byte or 0x00 if different count of bytes is set.
    /// </summary>
    protected byte Datum {
        get => (DataBytes.Length == 1) ? DataBytes[0] : (byte)0x00;
    }

    /// <summary>
    /// Returns bytes for the packet.
    /// </summary>
    public byte[] ToBytes() {
        var data = DataBytes;
        var dataLength = data.Length;
        var embedData = dataLength == 1;
        var trailingData = dataLength > 1;
        var needsCrc16 = dataLength > 13;
        var packetLength = 5;
        if (trailingData) { packetLength += needsCrc16 ? (dataLength + 2) : (dataLength + 1); }

        var packetBytes = new byte[packetLength];
        packetBytes[0] = 0xBB; // Header
        packetBytes[1] = (byte)((IsOriginHost ? 0x80 : 0x00) | Address);
        packetBytes[2] = (byte)((IsReplyRequestOrError ? 0x80 : 0x00) | (embedData ? 0x40 : 0x00) | CommandCode);
        if (embedData) {
            packetBytes[3] = DataBytes[0];
        } else {
            packetBytes[3] = (byte)dataLength;
        }
        packetBytes[4] = GetCrc8(packetBytes, 0, 4);

        if (trailingData) {
            Buffer.BlockCopy(data, 0, packetBytes, 5, data.Length);
            if (needsCrc16) {
                var crc = GetCrc16(packetBytes, 4, dataLength + 1);
                packetBytes[^2] = (byte)(crc >> 8);
                packetBytes[^1] = (byte)(crc & 0xFF);
            } else {
                packetBytes[^1] = GetCrc8(packetBytes, 4, dataLength + 1);
            }
        }

        return packetBytes;
    }

    #region CRC

    internal static byte GetCrc8(byte[] bytes, int offset, int length) {
        byte crc = 0;
        for (var i = offset; i < (offset + length); i++) {
            crc = Crc8Lookup[crc ^ BitReverse(bytes[i])];
        }
        return BitReverse(crc);
    }

    internal static ushort GetCrc16(byte[] bytes, int offset, int length) {
        ushort crc = 0;
        for (var i = offset; i < (offset + length); i++) {
            crc = (ushort)((crc >> 8) ^ Crc16Lookup[(crc & 0xff) ^ BitReverse(bytes[i])]);
        }
        return BitReverse(crc);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte BitReverse(byte value) {
        if (BitConverter.IsLittleEndian) {
            return BitReverseLookup[value];
        } else {
            return value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ushort BitReverse(ushort value) {
        if (BitConverter.IsLittleEndian) {
            return (ushort)((BitReverseLookup[value & 0xff] << 8) | (BitReverseLookup[value >> 8]));
        } else {
            return value;
        }
    }

    private static readonly byte[] BitReverseLookup = { 0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0, 0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8, 0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4, 0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4, 0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC, 0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2, 0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2, 0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA, 0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6, 0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE, 0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE, 0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1, 0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9, 0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5, 0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD, 0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3, 0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3, 0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB, 0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7, 0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF, };
    private static readonly byte[] Crc8Lookup = new byte[] { 0x00, 0xC7, 0x67, 0xA0, 0xCE, 0x09, 0xA9, 0x6E, 0x75, 0xB2, 0x12, 0xD5, 0xBB, 0x7C, 0xDC, 0x1B, 0xEA, 0x2D, 0x8D, 0x4A, 0x24, 0xE3, 0x43, 0x84, 0x9F, 0x58, 0xF8, 0x3F, 0x51, 0x96, 0x36, 0xF1, 0x3D, 0xFA, 0x5A, 0x9D, 0xF3, 0x34, 0x94, 0x53, 0x48, 0x8F, 0x2F, 0xE8, 0x86, 0x41, 0xE1, 0x26, 0xD7, 0x10, 0xB0, 0x77, 0x19, 0xDE, 0x7E, 0xB9, 0xA2, 0x65, 0xC5, 0x02, 0x6C, 0xAB, 0x0B, 0xCC, 0x7A, 0xBD, 0x1D, 0xDA, 0xB4, 0x73, 0xD3, 0x14, 0x0F, 0xC8, 0x68, 0xAF, 0xC1, 0x06, 0xA6, 0x61, 0x90, 0x57, 0xF7, 0x30, 0x5E, 0x99, 0x39, 0xFE, 0xE5, 0x22, 0x82, 0x45, 0x2B, 0xEC, 0x4C, 0x8B, 0x47, 0x80, 0x20, 0xE7, 0x89, 0x4E, 0xEE, 0x29, 0x32, 0xF5, 0x55, 0x92, 0xFC, 0x3B, 0x9B, 0x5C, 0xAD, 0x6A, 0xCA, 0x0D, 0x63, 0xA4, 0x04, 0xC3, 0xD8, 0x1F, 0xBF, 0x78, 0x16, 0xD1, 0x71, 0xB6, 0xF4, 0x33, 0x93, 0x54, 0x3A, 0xFD, 0x5D, 0x9A, 0x81, 0x46, 0xE6, 0x21, 0x4F, 0x88, 0x28, 0xEF, 0x1E, 0xD9, 0x79, 0xBE, 0xD0, 0x17, 0xB7, 0x70, 0x6B, 0xAC, 0x0C, 0xCB, 0xA5, 0x62, 0xC2, 0x05, 0xC9, 0x0E, 0xAE, 0x69, 0x07, 0xC0, 0x60, 0xA7, 0xBC, 0x7B, 0xDB, 0x1C, 0x72, 0xB5, 0x15, 0xD2, 0x23, 0xE4, 0x44, 0x83, 0xED, 0x2A, 0x8A, 0x4D, 0x56, 0x91, 0x31, 0xF6, 0x98, 0x5F, 0xFF, 0x38, 0x8E, 0x49, 0xE9, 0x2E, 0x40, 0x87, 0x27, 0xE0, 0xFB, 0x3C, 0x9C, 0x5B, 0x35, 0xF2, 0x52, 0x95, 0x64, 0xA3, 0x03, 0xC4, 0xAA, 0x6D, 0xCD, 0x0A, 0x11, 0xD6, 0x76, 0xB1, 0xDF, 0x18, 0xB8, 0x7F, 0xB3, 0x74, 0xD4, 0x13, 0x7D, 0xBA, 0x1A, 0xDD, 0xC6, 0x01, 0xA1, 0x66, 0x08, 0xCF, 0x6F, 0xA8, 0x59, 0x9E, 0x3E, 0xF9, 0x97, 0x50, 0xF0, 0x37, 0x2C, 0xEB, 0x4B, 0x8C, 0xE2, 0x25, 0x85, 0x42, };
    private static readonly ushort[] Crc16Lookup = new ushort[] { 0x0000, 0xBD33, 0xCF3B, 0x7208, 0x2B2B, 0x9618, 0xE410, 0x5923, 0x5656, 0xEB65, 0x996D, 0x245E, 0x7D7D, 0xC04E, 0xB246, 0x0F75, 0xACAC, 0x119F, 0x6397, 0xDEA4, 0x8787, 0x3AB4, 0x48BC, 0xF58F, 0xFAFA, 0x47C9, 0x35C1, 0x88F2, 0xD1D1, 0x6CE2, 0x1EEA, 0xA3D9, 0xEC05, 0x5136, 0x233E, 0x9E0D, 0xC72E, 0x7A1D, 0x0815, 0xB526, 0xBA53, 0x0760, 0x7568, 0xC85B, 0x9178, 0x2C4B, 0x5E43, 0xE370, 0x40A9, 0xFD9A, 0x8F92, 0x32A1, 0x6B82, 0xD6B1, 0xA4B9, 0x198A, 0x16FF, 0xABCC, 0xD9C4, 0x64F7, 0x3DD4, 0x80E7, 0xF2EF, 0x4FDC, 0x6D57, 0xD064, 0xA26C, 0x1F5F, 0x467C, 0xFB4F, 0x8947, 0x3474, 0x3B01, 0x8632, 0xF43A, 0x4909, 0x102A, 0xAD19, 0xDF11, 0x6222, 0xC1FB, 0x7CC8, 0x0EC0, 0xB3F3, 0xEAD0, 0x57E3, 0x25EB, 0x98D8, 0x97AD, 0x2A9E, 0x5896, 0xE5A5, 0xBC86, 0x01B5, 0x73BD, 0xCE8E, 0x8152, 0x3C61, 0x4E69, 0xF35A, 0xAA79, 0x174A, 0x6542, 0xD871, 0xD704, 0x6A37, 0x183F, 0xA50C, 0xFC2F, 0x411C, 0x3314, 0x8E27, 0x2DFE, 0x90CD, 0xE2C5, 0x5FF6, 0x06D5, 0xBBE6, 0xC9EE, 0x74DD, 0x7BA8, 0xC69B, 0xB493, 0x09A0, 0x5083, 0xEDB0, 0x9FB8, 0x228B, 0xDAAE, 0x679D, 0x1595, 0xA8A6, 0xF185, 0x4CB6, 0x3EBE, 0x838D, 0x8CF8, 0x31CB, 0x43C3, 0xFEF0, 0xA7D3, 0x1AE0, 0x68E8, 0xD5DB, 0x7602, 0xCB31, 0xB939, 0x040A, 0x5D29, 0xE01A, 0x9212, 0x2F21, 0x2054, 0x9D67, 0xEF6F, 0x525C, 0x0B7F, 0xB64C, 0xC444, 0x7977, 0x36AB, 0x8B98, 0xF990, 0x44A3, 0x1D80, 0xA0B3, 0xD2BB, 0x6F88, 0x60FD, 0xDDCE, 0xAFC6, 0x12F5, 0x4BD6, 0xF6E5, 0x84ED, 0x39DE, 0x9A07, 0x2734, 0x553C, 0xE80F, 0xB12C, 0x0C1F, 0x7E17, 0xC324, 0xCC51, 0x7162, 0x036A, 0xBE59, 0xE77A, 0x5A49, 0x2841, 0x9572, 0xB7F9, 0x0ACA, 0x78C2, 0xC5F1, 0x9CD2, 0x21E1, 0x53E9, 0xEEDA, 0xE1AF, 0x5C9C, 0x2E94, 0x93A7, 0xCA84, 0x77B7, 0x05BF, 0xB88C, 0x1B55, 0xA666, 0xD46E, 0x695D, 0x307E, 0x8D4D, 0xFF45, 0x4276, 0x4D03, 0xF030, 0x8238, 0x3F0B, 0x6628, 0xDB1B, 0xA913, 0x1420, 0x5BFC, 0xE6CF, 0x94C7, 0x29F4, 0x70D7, 0xCDE4, 0xBFEC, 0x02DF, 0x0DAA, 0xB099, 0xC291, 0x7FA2, 0x2681, 0x9BB2, 0xE9BA, 0x5489, 0xF750, 0x4A63, 0x386B, 0x8558, 0xDC7B, 0x6148, 0x1340, 0xAE73, 0xA106, 0x1C35, 0x6E3D, 0xD30E, 0x8A2D, 0x371E, 0x4516, 0xF825, };

    #endregion CRC

}


/// <summary>
/// Packet that is sent by the host.
/// </summary>
public interface IBBPacket {

    /// <summary>
    /// Address from which packet originated.
    /// Null if packet originated from host.
    /// 0 if packet originated from unknown device.
    /// 1-127 otherwise.
    /// </summary>
    public byte? FromAddress { get; }

    /// <summary>
    /// Address to which packet is destined.
    /// Null if packet is for host.
    /// 0 if packet is broadcast.
    /// 1-127 otherwise.
    /// </summary>
    public byte? ToAddress { get; }

    /// <summary>
    /// Returns data bytes for the packet.
    /// </summary>
    public byte[] Data { get; }


    /// <summary>
    /// Returns bytes for the packet.
    /// </summary>
    public byte[] ToBytes();

}


/// <summary>
/// Packet that is sent by the host.
/// </summary>
public interface IBBHostPacket : IBBPacket {
}


/// <summary>
/// Packet that is sent by the device.
/// </summary>
public interface IBBDevicePacket : IBBPacket {
}


/// <summary>
/// Custom request packet.
/// </summary>
public sealed record BBCustomPacket : BBPacket, IBBHostPacket {

    /// <summary>
    /// Creates a new instance.
    /// Only for internal use; not error checked.
    /// </summary>
    internal BBCustomPacket(byte destinationAddress, bool replyRequested, byte commandCode, byte[] data)
        : base(isOriginHost: true, destinationAddress, replyRequested, commandCode, data) {
    }


    /// <summary>
    /// Gets destination address.
    /// </summary>
    public byte DestinationAddress {
        get => Address;
    }

    /// <summary>
    /// Gets if reply to this packet is requested from a device.
    /// </summary>
    public bool ReplyRequested {
        get => IsReplyRequestOrError;
    }


    #region Create

    /// <summary>
    /// Returns a new custom request packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 127.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 60.</param>
    public static BBCustomPacket Create(byte destinationAddress, byte commandCode) {
        return Create(destinationAddress, commandCode, Array.Empty<byte>(), replyRequested: false);
    }

    /// <summary>
    /// Returns a new custom request packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 127.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 60.</param>
    /// <param name="datum">Data byte.</param>
    public static BBCustomPacket Create(byte destinationAddress, byte commandCode, byte datum) {
        return Create(destinationAddress, commandCode, new byte[] { datum }, replyRequested: false);
    }

    /// <summary>
    /// Returns a new custom request packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 127.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 60.</param>
    /// <param name="data">Data bytes.</param>
    public static BBCustomPacket Create(byte destinationAddress, byte commandCode, byte[] data) {
        return Create(destinationAddress, commandCode, data, replyRequested: false);
    }

    /// <summary>
    /// Returns a new custom request packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 127.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 60.</param>
    /// <param name="datum">Data byte.</param>
    /// <param name="replyRequested">If true, a reply packet will be requested from device.</param>
    public static BBCustomPacket Create(byte destinationAddress, byte commandCode, byte datum, bool replyRequested) {
        return Create(destinationAddress, commandCode, new byte[] { datum }, replyRequested);
    }

    /// <summary>
    /// Returns a new custom request packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 127.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 60.</param>
    /// <param name="data">Data bytes.</param>
    /// <param name="replyRequested">If true, a reply packet will be requested from device.</param>
    public static BBCustomPacket Create(byte destinationAddress, byte commandCode, byte[] data, bool replyRequested) {
        if (destinationAddress is < 0 or > 127) { throw new ArgumentOutOfRangeException(nameof(destinationAddress), "Destination address must be either 0 (broadcast) or between 1 and 127."); }
        if (commandCode is < 1 or > 60) { throw new ArgumentOutOfRangeException(nameof(commandCode), "Command code must be between 1 and 60."); }
        if (data == null) { throw new ArgumentNullException(nameof(data), "Data cannot be null."); }
        if (data.Length > 240) { throw new ArgumentOutOfRangeException(nameof(data), "Cannot have more than 240 bytes of data."); }
        if ((destinationAddress == 0) && replyRequested) { throw new ArgumentOutOfRangeException(nameof(replyRequested), "Cannot request reply to a broadcast packet."); }

        return new BBCustomPacket(destinationAddress, replyRequested, commandCode, data);
    }

    #endregion Create

    #region GetReply

    /// <summary>
    /// Creates a new custom reply packet based on request.
    /// </summary>
    public BBCustomReplyPacket GetReply() {
        return BBCustomReplyPacket.Create(DestinationAddress, CommandCode);
    }

    /// <summary>
    /// Creates a new custom reply packet based on request.
    /// </summary>
    /// <param name="datum">Data byte.</param>
    public BBCustomReplyPacket GetReply(byte datum) {
        return BBCustomReplyPacket.Create(DestinationAddress, CommandCode, datum);
    }

    /// <summary>
    /// Creates a new custom reply packet based on request.
    /// </summary>
    /// <param name="data">Data bytes.</param>
    public BBCustomReplyPacket GetReply(byte[] data) {
        return BBCustomReplyPacket.Create(DestinationAddress, CommandCode, data);
    }

    #endregion GetReply

    #region GetErrorReply

    /// <summary>
    /// Creates a new custom reply packet based on request.
    /// </summary>
    public BBCustomReplyPacket GetErrorReply() {
        return BBCustomReplyPacket.CreateError(DestinationAddress, CommandCode);
    }

    /// <summary>
    /// Creates a new custom reply packet based on request.
    /// </summary>
    /// <param name="datum">Data byte.</param>
    public BBCustomReplyPacket GetErrorReply(byte datum) {
        return BBCustomReplyPacket.CreateError(DestinationAddress, CommandCode, datum);
    }

    /// <summary>
    /// Creates a new custom reply packet based on request.
    /// </summary>
    /// <param name="data">Data bytes.</param>
    public BBCustomReplyPacket GetErrorReply(byte[] data) {
        return BBCustomReplyPacket.CreateError(DestinationAddress, CommandCode, data);
    }

    #endregion GetErrorReply

}


/// <summary>
/// Custom reply packet.
/// </summary>
public sealed record BBCustomReplyPacket : BBPacket, IBBDevicePacket {

    /// <summary>
    /// Creates a new instance.
    /// Only for internal use; not error checked.
    /// </summary>
    internal BBCustomReplyPacket(byte sourceAddress, bool errorReply, byte commandCode, byte[] data)
        : base(isOriginHost: false, sourceAddress, errorReply, commandCode, data) {
    }


    /// <summary>
    /// Gets source address.
    /// </summary>
    public byte SourceAddress {
        get => Address;
    }

    /// <summary>
    /// Gets if this is an error reply.
    /// </summary>
    public bool IsError {
        get => IsReplyRequestOrError;
    }


    #region Create

    /// <summary>
    /// Creates a new custom reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 127.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 60.</param>
    public static BBCustomReplyPacket Create(byte sourceAddress, byte commandCode) {
        return Create(sourceAddress, commandCode, Array.Empty<byte>());
    }

    /// <summary>
    /// Creates a new custom reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 127.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 60.</param>
    /// <param name="datum">Data byte.</param>
    public static BBCustomReplyPacket Create(byte sourceAddress, byte commandCode, byte datum) {
        return Create(sourceAddress, commandCode, new byte[] { datum });
    }

    /// <summary>
    /// Creates a new custom reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 127.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 60.</param>
    /// <param name="data">Data bytes.</param>
    public static BBCustomReplyPacket Create(byte sourceAddress, byte commandCode, byte[] data) {
        if (sourceAddress is 0) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Cannot send a reply to a broadcast packet."); }
        if (sourceAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Source address must be between 1 and 127."); }
        if (commandCode is < 1 or > 60) { throw new ArgumentOutOfRangeException(nameof(commandCode), "Command code must be between 1 and 60."); }
        if (data == null) { throw new ArgumentNullException(nameof(data), "Data cannot be null."); }
        if (data.Length > 240) { throw new ArgumentOutOfRangeException(nameof(data), "Cannot have more than 240 bytes of data."); }

        return new BBCustomReplyPacket(sourceAddress, errorReply: false, commandCode, data);
    }

    #endregion Create

    #region CreateError

    /// <summary>
    /// Creates a new custom error reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 127.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 60.</param>
    public static BBCustomReplyPacket CreateError(byte sourceAddress, byte commandCode) {
        return CreateError(sourceAddress, commandCode, Array.Empty<byte>());
    }

    /// <summary>
    /// Creates a new custom error reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 127.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 60.</param>
    /// <param name="datum">Data byte.</param>
    public static BBCustomReplyPacket CreateError(byte sourceAddress, byte commandCode, byte datum) {
        return CreateError(sourceAddress, commandCode, new byte[] { datum });
    }

    /// <summary>
    /// Creates a new custom error reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 127.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 60.</param>
    /// <param name="data">Data bytes.</param>
    public static BBCustomReplyPacket CreateError(byte sourceAddress, byte commandCode, byte[] data) {
        if (sourceAddress is 0) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Cannot send a reply to a broadcast packet."); }
        if (sourceAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Source address must be between 1 and 127."); }
        if (commandCode is < 1 or > 60) { throw new ArgumentOutOfRangeException(nameof(commandCode), "Command code must be between 1 and 60."); }
        if (data == null) { throw new ArgumentNullException(nameof(data), "Data cannot be null."); }
        if (data.Length > 240) { throw new ArgumentOutOfRangeException(nameof(data), "Cannot have more than 240 bytes of data."); }

        return new BBCustomReplyPacket(sourceAddress, errorReply: true, commandCode, data);
    }

    #endregion CreateError

}


/// <summary>
/// System packet coming from device.
/// </summary>
public sealed record BBSystemHostPacket : BBPacket, IBBHostPacket {

    /// <summary>
    /// Creates a new instance.
    /// Only for internal use; not error checked.
    /// </summary>
    internal BBSystemHostPacket(byte sourceAddress, byte[] data)
        : base(isOriginHost: true, sourceAddress, isReplyRequestOrError: false, 0x00, data) {
    }


    /// <summary>
    /// Gets destination address.
    /// </summary>
    public byte DestinationAddress {
        get => Address;
    }

    /// <summary>
    /// Gets action requested.
    /// </summary>
    public byte Action {
        get => Datum;
    }


    #region Create

    /// <summary>
    /// Creates a new unsolicited duplicate address report packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 127.</param>
    public static BBSystemHostPacket CreateReboot(byte destinationAddress) {
        if (destinationAddress is < 0 or > 127) { throw new ArgumentOutOfRangeException(nameof(destinationAddress), "Destination address must be either 0 (broadcast) or between 1 and 127."); }

        return new BBSystemHostPacket(destinationAddress, new byte[] { 0x06 });
    }

    #endregion Create

}


/// <summary>
/// System packet coming from device.
/// </summary>
public sealed record BBSystemDevicePacket : BBPacket, IBBDevicePacket {

    /// <summary>
    /// Creates a new instance.
    /// Only for internal use; not error checked.
    /// </summary>
    internal BBSystemDevicePacket(byte sourceAddress, bool errorReply, byte[] data)
        : base(isOriginHost: false, sourceAddress, errorReply, 0x00, data) {
    }


    /// <summary>
    /// Gets source address.
    /// </summary>
    public byte SourceAddress {
        get => Address;
    }

    /// <summary>
    /// Gets if this is an error reply.
    /// </summary>
    public bool IsError {
        get => IsReplyRequestOrError;
    }

    /// <summary>
    /// Gets if blink is active.
    /// </summary>
    public bool Blink {
        get { return ((Datum & 0x80) == 0x80); }
    }

    /// <summary>
    /// Gets current mode.
    /// </summary>
    public BBDeviceMode Mode {
        get { return (BBDeviceMode)((Datum >> 5) & 0x03); }
    }

    /// <summary>
    /// Gets error code.
    /// </summary>
    public int ErrorCode {
        get { return (byte)(Datum & 0x03); }
    }


    #region Create

    /// <summary>
    /// Creates a new unsolicited duplicate address report packet.
    /// </summary>
    /// <param name="sourceAddress">Source address.</param>
    /// <param name="blinking">True if LED blink is active.</param>
    /// <param name="mode">Current device mode.</param>
    /// <param name="errorCode">Current device error code.</param>
    public static BBSystemDevicePacket CreateDuplicateAddressReport(byte sourceAddress, bool blinking, BBDeviceMode mode, byte errorCode) {
        if (sourceAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Source address must be between 1 and 127."); }

        var datum = blinking ? (byte)0x80 : (byte)0x00;
        datum |= (byte)((int)mode << 5);
        datum |= errorCode;

        return new BBSystemDevicePacket(sourceAddress, errorReply: true, new byte[] { datum });
    }

    /// <summary>
    /// Creates a new unsolicited status report packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be either 0 (not set) or between 1 and 127.</param>
    /// <param name="blinking">True if LED blink is active.</param>
    /// <param name="mode">Current device mode.</param>
    /// <param name="errorCode">Current device error code.</param>
    public static BBSystemDevicePacket CreateStatusUpdate(byte sourceAddress, bool blinking, BBDeviceMode mode, byte errorCode) {
        if (sourceAddress is < 0 or > 127) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Source address must be either 0 (not set) or between 1 and 127."); }
        if ((int)mode is < 0 or > 3) { throw new ArgumentOutOfRangeException(nameof(mode), "Mode out of range."); }
        if (errorCode is < 0 or > 7) { throw new ArgumentOutOfRangeException(nameof(errorCode), "Error code must be between 0 and 7."); }

        var datum = blinking ? (byte)0x80 : (byte)0x00;
        datum |= (byte)((int)mode << 5);
        datum |= errorCode;

        return new BBSystemDevicePacket(sourceAddress, errorReply: false, new byte[] { datum });
    }

    #endregion Create

}


/// <summary>
/// Ping request packet.
/// </summary>
public sealed record BBPingPacket : BBPacket, IBBHostPacket {

    /// <summary>
    /// Creates a new instance.
    /// Only for internal use; not error checked.
    /// </summary>
    internal BBPingPacket(byte destinationAddress, bool replyRequested, byte[] data)
        : base(isOriginHost: true, destinationAddress, replyRequested, 0x3D, data) {
    }


    /// <summary>
    /// Gets destination address.
    /// </summary>
    public byte DestinationAddress {
        get => Address;
    }

    /// <summary>
    /// Gets if reply to this packet is requested from a device.
    /// </summary>
    public bool ReplyRequested {
        get => IsReplyRequestOrError;
    }


    #region Create

    /// <summary>
    /// Creates a new ping request packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be between 1 and 127.</param>
    public static BBPingPacket Create(byte destinationAddress) {
        return Create(destinationAddress, replyRequested: true);
    }

    /// <summary>
    /// Creates a new ping request packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be between 1 and 127.</param>
    /// <param name="replyRequested">If true, a reply packet will be requested from device.</param>
    public static BBPingPacket Create(byte destinationAddress, bool replyRequested) {
        if (destinationAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(destinationAddress), "Destination address must be between 1 and 127."); }

        return new BBPingPacket(destinationAddress, replyRequested, new byte[] { (byte)Random.Shared.Next(256) });
    }

    #endregion Create


    #region GetReply

    /// <summary>
    /// Creates a new ping reply packet.
    /// </summary>
    public BBPingReplyPacket GetReply() {
        return BBPingReplyPacket.Create(DestinationAddress, Data);
    }

    #endregion GetReply

}


/// <summary>
/// Ping reply packet.
/// </summary>
public sealed record BBPingReplyPacket : BBPacket, IBBDevicePacket {

    /// <summary>
    /// Creates a new instance.
    /// Only for internal use; not error checked.
    /// </summary>
    internal BBPingReplyPacket(byte sourceAddress, bool errorReply, byte[] data)
        : base(isOriginHost: false, sourceAddress, errorReply, 0x3D, data) {
    }


    /// <summary>
    /// Gets source address.
    /// </summary>
    public byte SourceAddress {
        get => Address;
    }

    /// <summary>
    /// Gets if this is an error reply.
    /// </summary>
    public bool ErrorReply {
        get => IsReplyRequestOrError;
    }


    #region Create

    /// <summary>
    /// Creates a new ping reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address.</param>
    public static BBPingReplyPacket Create(byte sourceAddress) {
        if (sourceAddress is 0) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Cannot send a reply to a broadcast packet."); }
        if (sourceAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Source address must be between 1 and 127."); }
        return new BBPingReplyPacket(sourceAddress, errorReply: false, Array.Empty<byte>());
    }

    /// <summary>
    /// Creates a new ping reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 127.</param>
    /// <param name="datum">Data byte.</param>
    public static BBPingReplyPacket Create(byte sourceAddress, byte datum) {
        return Create(sourceAddress, new byte[] { datum });
    }

    /// <summary>
    /// Creates a new ping reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 127.</param>
    /// <param name="datum">Data byte.</param>
    public static BBPingReplyPacket Create(byte sourceAddress, byte[] data) {
        if (sourceAddress is 0) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Cannot send a reply to a broadcast packet."); }
        if (sourceAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Source address must be between 1 and 127."); }
        return new BBPingReplyPacket(sourceAddress, errorReply: false, data);
    }

    #endregion Create

    #region CreateError

    /// <summary>
    /// Creates a new ping error reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address.</param>
    public static BBPingReplyPacket CreateError(byte sourceAddress) {
        if (sourceAddress is 0) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Cannot send a reply to a broadcast packet."); }
        if (sourceAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Source address must be between 1 and 127."); }
        return new BBPingReplyPacket(sourceAddress, errorReply: true, Array.Empty<byte>());
    }

    /// <summary>
    /// Creates a new ping error reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 127.</param>
    /// <param name="datum">Data byte.</param>
    public static BBPingReplyPacket CreateError(byte sourceAddress, byte datum) {
        if (sourceAddress is 0) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Cannot send a reply to a broadcast packet."); }
        if (sourceAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Source address must be between 1 and 127."); }
        return new BBPingReplyPacket(sourceAddress, errorReply: true, new byte[] { datum });
    }

    #endregion CreateError

}


/// <summary>
/// List of device modes.
/// </summary>
public enum BBDeviceMode {
    /// <summary>
    /// Normal mode.
    /// </summary>
    Normal = 0,
    /// <summary>
    /// Configuration mode.
    /// </summary>
    Config = 1,
    /// <summary>
    /// Test mode.
    /// </summary>
    Test = 2,
    /// <summary>
    /// Programming mode.
    /// </summary>
    Program = 3,
}


/// <summary>
/// Status request packet.
/// </summary>
public sealed record BBStatusPacket : BBPacket, IBBHostPacket {

    /// <summary>
    /// Creates a new instance.
    /// Only for internal use; not error checked.
    /// </summary>
    internal BBStatusPacket(byte destinationAddress, bool replyRequested, byte[] data)
        : base(isOriginHost: true, destinationAddress, replyRequested, 0x3E, data) {
    }


    /// <summary>
    /// Gets destination address.
    /// </summary>
    public byte DestinationAddress {
        get => Address;
    }

    /// <summary>
    /// Gets if reply to this packet is requested from a device.
    /// </summary>
    public bool ReplyRequested {
        get => IsReplyRequestOrError;
    }


    /// <summary>
    /// Gets if new blink is requested.
    /// </summary>
    public bool? NewBlink {
        get { return ((Datum & 0x10) == 0x10) ? ((Datum & 0x80) == 0x80) : null; }
    }

    /// <summary>
    /// Gets if new mode is requested.
    /// </summary>
    public BBDeviceMode? NewMode {
        get { return ((Datum & 0x08) == 0x08) ? (BBDeviceMode)((Datum >> 5) & 0x03) : null; }
    }

    #region New

    /// <summary>
    /// Creates a new status check packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be between 1 and 127.</param>
    public static BBStatusPacket New(byte destinationAddress) {
        if (destinationAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(destinationAddress), "Destination address must be between 1 and 127."); }

        return new BBStatusPacket(destinationAddress, replyRequested: true, Array.Empty<byte>());
    }

    /// <summary>
    /// Creates a new status packet with light update.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be between 1 and 127.</param>
    /// <param name="newBlink">If true, blink light will be requested.</param>
    public static BBStatusPacket New(byte destinationAddress, bool newBlink) {
        if (destinationAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(destinationAddress), "Destination address must be between 1 and 127."); }

        return New(destinationAddress, newBlink, replyRequested: true);
    }

    /// <summary>
    /// Creates a new status change packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 127.</param>
    /// <param name="newBlink">If true, blink light will be requested.</param>
    /// <param name="replyRequested">If true, a reply packet will be requested from device.</param>
    public static BBStatusPacket New(byte destinationAddress, bool newBlink, bool replyRequested) {
        if (destinationAddress is < 0 or > 127) { throw new ArgumentOutOfRangeException(nameof(destinationAddress), "Destination address must be either 0 (broadcast) or between 1 and 127."); }
        if ((destinationAddress == 0) && replyRequested) { throw new ArgumentOutOfRangeException(nameof(replyRequested), "Cannot request reply to a broadcast packet."); }

        var datum = (byte)0x10; // blink update request
        datum |= newBlink ? (byte)0x80 : (byte)0x00;

        return new BBStatusPacket(destinationAddress, replyRequested, new byte[] { datum });
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be between 1 and 127.</param>
    /// <param name="requestMode">Mode that device should switch to.</param>
    /// <param name="replyRequested">If true, a reply packet will be requested from device.</param>
    public static BBStatusPacket New(byte destinationAddress, BBDeviceMode requestMode) {
        if (destinationAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(destinationAddress), "Destination address must be between 1 and 127."); }

        return New(destinationAddress, requestMode, replyRequested: true);
    }

    /// <summary>
    /// Creates a new status change packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 127.</param>
    /// <param name="newMode">Mode that device should switch to.</param>
    /// <param name="replyRequested">If true, a reply packet will be requested from device.</param>
    public static BBStatusPacket New(byte destinationAddress, BBDeviceMode newMode, bool replyRequested) {
        if (destinationAddress is < 0 or > 127) { throw new ArgumentOutOfRangeException(nameof(destinationAddress), "Destination address must be either 0 (broadcast) or between 1 and 127."); }
        if ((destinationAddress == 0) && replyRequested) { throw new ArgumentOutOfRangeException(nameof(replyRequested), "Cannot request reply to a broadcast packet."); }
        if ((int)newMode is < 0 or > 3) { throw new ArgumentOutOfRangeException(nameof(newMode), "Mode out of range."); }

        var datum = (byte)0x08; // mode update request
        datum |= (byte)((int)newMode << 5);

        return new BBStatusPacket(destinationAddress, replyRequested, new byte[] { datum });
    }

    /// <summary>
    /// Creates a new status change packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be between 1 and 127.</param>
    /// <param name="newBlink">If true, blink light will be requested.</param>
    /// <param name="newMode">Mode that device should switch to.</param>
    /// <param name="replyRequested">If true, a reply packet will be requested from device.</param>
    public static BBStatusPacket New(byte destinationAddress, bool newBlink, BBDeviceMode newMode) {
        if (destinationAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(destinationAddress), "Destination address must be between 1 and 127."); }

        return New(destinationAddress, newBlink, newMode, replyRequested: true);
    }

    /// <summary>
    /// Creates a new status change packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 127.</param>
    /// <param name="newBlink">If true, UID light will be requested.</param>
    /// <param name="newMode">Mode that device should switch to.</param>
    /// <param name="replyRequested">If true, a reply packet will be requested from device.</param>
    public static BBStatusPacket New(byte destinationAddress, bool newBlink, BBDeviceMode newMode, bool replyRequested) {
        if (destinationAddress is < 0 or > 127) { throw new ArgumentOutOfRangeException(nameof(destinationAddress), "Destination address must be either 0 (broadcast) or between 1 and 127."); }
        if ((destinationAddress == 0) && replyRequested) { throw new ArgumentOutOfRangeException(nameof(replyRequested), "Cannot request reply to a broadcast packet."); }
        if ((int)newMode is < 0 or > 3) { throw new ArgumentOutOfRangeException(nameof(newMode), "Mode out of range."); }

        var datum = (byte)0x18; // update both blink and mode
        datum |= newBlink ? (byte)0x80 : (byte)0x00;
        datum |= (byte)((int)newMode << 5);

        return new BBStatusPacket(destinationAddress, replyRequested, new byte[] { datum });
    }

    #endregion New

    #region GetReply

    /// <summary>
    /// Creates a new status reply packet based on request.
    /// </summary>
    /// <param name="blinking">True if LED blink is active.</param>
    /// <param name="mode">Current device mode.</param>
    /// <param name="errorCode">Current device error code. Must be between 0 and 7</param>
    public BBStatusReplyPacket GetReply(bool blinking, BBDeviceMode mode, byte errorCode) {
        return BBStatusReplyPacket.New(DestinationAddress, blinking, mode, errorCode);
    }

    #endregion GetReply

    #region GetErrorReply

    /// <summary>
    /// Creates a new status error reply packet based on request.
    /// </summary>
    /// <param name="blinking">True if LED blink is active.</param>
    /// <param name="mode">Current device mode.</param>
    /// <param name="errorCode">Current device error code. Must be between 0 and 7</param>
    public BBStatusReplyPacket GetErrorReply(bool blinking, BBDeviceMode mode, byte errorCode) {
        return BBStatusReplyPacket.NewError(DestinationAddress, blinking, mode, errorCode);
    }

    #endregion GetErrorReply

}


/// <summary>
/// Status reply packet.
/// </summary>
public sealed record BBStatusReplyPacket : BBPacket, IBBDevicePacket {

    /// <summary>
    /// Creates a new instance.
    /// Only for internal use; not error checked.
    /// </summary>
    internal BBStatusReplyPacket(byte sourceAddress, bool errorReply, byte[] data)
        : base(isOriginHost: false, sourceAddress, errorReply, 0x3E, data) {
    }


    /// <summary>
    /// Gets source address.
    /// </summary>
    public byte SourceAddress {
        get => Address;
    }

    /// <summary>
    /// Gets if this is an error reply.
    /// </summary>
    public bool ErrorReply {
        get => IsReplyRequestOrError;
    }


    /// <summary>
    /// Gets if UID is active.
    /// </summary>
    public bool Blink {
        get { return (Datum & 0x80) == 0x80; }
    }

    /// <summary>
    /// Gets current device mode.
    /// </summary>
    public BBDeviceMode Mode {
        get { return (BBDeviceMode)((Datum >> 5) & 0x03); }
    }

    /// <summary>
    /// Gets error code.
    /// </summary>
    public byte ErrorCode {
        get { return (byte)(Datum & 0x07); }
    }


    #region New

    /// <summary>
    /// Creates a new status reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 127.</param>
    /// <param name="blinking">True if LED blink is active.</param>
    /// <param name="mode">Current device mode.</param>
    /// <param name="errorCode">Current device error code. Must be between 0 and 7</param>
    public static BBStatusReplyPacket New(byte sourceAddress, bool blinking, BBDeviceMode mode, byte errorCode) {
        if (sourceAddress is 0) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Cannot send a reply to a broadcast packet."); }
        if (sourceAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Source address must be between 1 and 127."); }
        if ((int)mode is < 0 or > 3) { throw new ArgumentOutOfRangeException(nameof(mode), "Mode out of range."); }
        if (errorCode is < 0 or > 7) { throw new ArgumentOutOfRangeException(nameof(errorCode), "Error code must be between 0 and 7."); }

        var datum = blinking ? (byte)0x80 : (byte)0x00;
        datum |= (byte)((int)mode << 5);
        datum |= errorCode;

        return new BBStatusReplyPacket(sourceAddress, errorReply: false, new byte[] { datum });
    }

    /// <summary>
    /// Creates a new status error reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 127.</param>
    /// <param name="blinking">True if LED blink is active.</param>
    /// <param name="mode">Current device mode.</param>
    /// <param name="errorCode">Current device error code. Must be between 0 and 7</param>
    public static BBStatusReplyPacket NewError(byte sourceAddress, bool blinking, BBDeviceMode mode, byte errorCode) {
        if (sourceAddress is 0) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Cannot send a reply to a broadcast packet."); }
        if (sourceAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Source address must be between 1 and 127."); }
        if ((int)mode is < 0 or > 3) { throw new ArgumentOutOfRangeException(nameof(mode), "Mode out of range."); }
        if (errorCode is < 0 or > 7) { throw new ArgumentOutOfRangeException(nameof(errorCode), "Error code must be between 0 and 7."); }

        var datum = blinking ? (byte)0x80 : (byte)0x00;
        datum |= (byte)((int)mode << 5);
        datum |= errorCode;

        return new BBStatusReplyPacket(sourceAddress, errorReply: true, new byte[] { datum });
    }

    #endregion New

}


/// <summary>
/// Address request packet.
/// </summary>
public sealed record BBAddressPacket : BBPacket, IBBHostPacket {

    /// <summary>
    /// Creates a new instance.
    /// Only for internal use; not error checked.
    /// </summary>
    internal BBAddressPacket(byte destinationAddress, bool replyRequested, byte[] data)
        : base(isOriginHost: true, destinationAddress, replyRequested, 0x3F, data) {
    }


    /// <summary>
    /// Gets destination address.
    /// </summary>
    public byte DestinationAddress {
        get => Address;
    }

    /// <summary>
    /// Gets if reply to this packet is requested from a device.
    /// </summary>
    public bool ReplyRequested {
        get => IsReplyRequestOrError;
    }

    /// <summary>
    /// Gets new address.
    /// </summary>
    public byte NewAddress {
        get => Datum;
    }


    #region Create

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="destinationAddress">Destination address.</param>
    /// <param name="newAddress">New device address.</param>
    public static BBAddressPacket Create(byte destinationAddress, byte newAddress) {
        return Create(destinationAddress, newAddress, replyRequested: true);
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 127.</param>
    /// <param name="newAddress">New device address. Must be either 0 (not set) or between 1 and 127.</param>
    /// <param name="replyRequested">If true, a reply packet will be requested from device.</param>
    public static BBAddressPacket Create(byte destinationAddress, byte newAddress, bool replyRequested) {
        if (destinationAddress is < 0 or > 127) { throw new ArgumentOutOfRangeException(nameof(destinationAddress), "Destination address must be either 0 (broadcast) or between 1 and 127."); }
        if (newAddress is < 0 or > 127) { throw new ArgumentOutOfRangeException(nameof(newAddress), "New address must be either 0 (not set) or between 1 and 127."); }
        if ((destinationAddress == 0) && replyRequested) { throw new ArgumentOutOfRangeException(nameof(replyRequested), "Cannot request reply to a broadcast packet."); }

        return new BBAddressPacket(destinationAddress, replyRequested, new byte[] { newAddress });
    }

    #endregion Create

    #region GetReply

    /// <summary>
    /// Returns packet that is a response to the request.
    /// </summary>
    public BBAddressReplyPacket GetReply() {
        return BBAddressReplyPacket.Create(Address, NewAddress);
    }

    /// <summary>
    /// Returns packet that is a negative response to the request.
    /// </summary>
    public BBAddressReplyPacket GetErrorReply() {
        return BBAddressReplyPacket.CreateError(Address, NewAddress);
    }

    #endregion GetReply
}


/// <summary>
/// Address reply packet.
/// </summary>
public sealed record BBAddressReplyPacket : BBPacket, IBBDevicePacket {

    /// <summary>
    /// Creates a new instance.
    /// Only for internal use; not error checked.
    /// </summary>
    internal BBAddressReplyPacket(byte sourceAddress, bool errorReply, byte[] data)
        : base(isOriginHost: false, sourceAddress, errorReply, 0x3F, data) {
    }


    /// <summary>
    /// Gets source address.
    /// </summary>
    public byte SourceAddress {
        get => Address;
    }

    /// <summary>
    /// Gets if this is an error reply.
    /// </summary>
    public bool ErrorReply {
        get => IsReplyRequestOrError;
    }

    /// <summary>
    /// Gets new address.
    /// </summary>
    public byte NewAddress {
        get => Datum;
    }


    #region Create

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 127.</param>
    /// <param name="newAddress">New address.</param>
    public static BBAddressReplyPacket Create(byte sourceAddress, byte newAddress) {
        if (sourceAddress is 0) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Cannot send a reply to a broadcast packet."); }
        if (sourceAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Source address must be between 1 and 127."); }
        if (newAddress is < 0 or > 127) { throw new ArgumentOutOfRangeException(nameof(newAddress), "New address must be between 0 and 127."); }

        return new BBAddressReplyPacket(sourceAddress, errorReply: false, new byte[] { newAddress });
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 127.</param>
    /// <param name="newAddress">New address.</param>
    public static BBAddressReplyPacket CreateError(byte sourceAddress, byte newAddress) {
        if (sourceAddress is 0) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Cannot send a reply to a broadcast packet."); }
        if (sourceAddress is < 1 or > 127) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Source address must be between 1 and 127."); }
        if (newAddress is < 0 or > 127) { throw new ArgumentOutOfRangeException(nameof(newAddress), "New address must be between 0 and 127."); }

        return new BBAddressReplyPacket(sourceAddress, errorReply: true, new byte[] { newAddress });
    }

    #endregion Create

}
