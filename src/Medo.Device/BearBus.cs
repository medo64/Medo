/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-10-01: Allowing CRC-8 for long data packets (UseCRC16 field)
//            Changed CRC-16 polynomial
//2022-09-17: Better handling of reads
//2022-08-03: Major protocol refactoring
//2022-07-18: Fixed packet receiving
//2022-07-17: Added BearBusMonitor
//2022-07-16: Updated for data length
//2022-07-05: Initial release

namespace Medo.Device;

using System;
using System.Collections.Generic;
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

        ReceiveThread = new Thread(RunReceive) {
            CurrentCulture = CultureInfo.InvariantCulture,
            IsBackground = true,
            Name = "BearBus:Read",
            Priority = ThreadPriority.Normal
        };
        ReceiveThread.Start();
    }

    private readonly Stream Stream;

    /// <summary>
    /// Creates a new instance with unrestricted behavior.
    /// Intended for monitoring. Use CreateHost or CreateDevice for specific behaviors.
    /// </summary>
    /// <param name="stream">Stream.</param>
    public static BearBusMonitor CreateMonitor(Stream stream) {
        return new BearBusMonitor(stream);
    }

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


    #region Receive

    private readonly Thread ReceiveThread;
    private readonly ManualResetEventSlim ReceiveCancelEvent = new(initialState: false);
    private readonly SemaphoreSlim ReceiveQueueSync = new(initialCount: 0, maxCount: Environment.ProcessorCount + 1);  // give it a few more since the whole waiting thing is a bit wishy-washy
    private readonly Queue<IBBPacket> ReceiveQueue = new();

    /// <summary>
    /// Returns true if packet is received.
    /// </summary>
    /// <param name="packet">Packet received.</param>
    private protected bool BaseTryReceive([MaybeNullWhen(false)] out IBBPacket packet) {
        lock (ReceiveQueue) {
            if (ReceiveQueue.Count > 0) {
                packet = ReceiveQueue.Dequeue();
                if ((ReceiveQueue.Count > 0) && (ReceiveQueueSync.CurrentCount == 0)) { ReceiveQueueSync.Release(); }  // give chance for one more
                return true;
            }
        }

        packet = null;
        return false;
    }

    /// <summary>
    /// Returns packet once received.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    private protected IBBPacket BaseReceive(int millisecondsTimeout) {
        while (true) {
            if (ReceiveQueueSync.Wait(millisecondsTimeout)) {
                if (BaseTryReceive(out var packet)) { return packet; }
            } else {
                throw new TimeoutException();
            }
            Thread.Sleep(1);
        }
    }

    /// <summary>
    /// Returns packet once received.
    /// Throws exception upon timeout.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    private protected IBBPacket BaseReceive(int millisecondsTimeout, CancellationToken cancellationToken) {
        while (true) {
            if (ReceiveQueueSync.Wait(millisecondsTimeout, cancellationToken)) {
                if (BaseTryReceive(out var packet)) { return packet; }
            } else {
                throw new TimeoutException();
            }
            Thread.Sleep(1);
        }
    }

    /// <summary>
    /// Awaits for the next available packet.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    private protected async Task<IBBPacket> BaseReceiveAsync(int millisecondsTimeout) {
        while (true) {
            if (await ReceiveQueueSync.WaitAsync(millisecondsTimeout)) {
                if (BaseTryReceive(out var packet)) { return packet; }
            } else {
                throw new TimeoutException();
            }
            Thread.Sleep(1);
        }
    }

    /// <summary>
    /// Awaits for the next available packet.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private protected async Task<IBBPacket> BaseReceiveAsync(int millisecondsTimeout, CancellationToken cancellationToken) {
        while (true) {
            if (await ReceiveQueueSync.WaitAsync(millisecondsTimeout, cancellationToken)) {
                if (BaseTryReceive(out var packet)) { return packet; }
            } else {
                throw new TimeoutException();
            }
            Thread.Sleep(1);
        }
    }


    private enum ParseState { SearchHeader, WaitAddress, WaitCommand, WaitDataLength, WaitHeaderCrc8, WaitExtraData, WaitExtraDataCrc8, WaitExtraDataCrc16a, WaitExtraDataCrc16b }
    private ParseState ParsingState = ParseState.SearchHeader;
    private int ParsedDataCount = 0;
    private readonly byte[] ParsedData = new byte[5 + 127 + 2]; // maximum packet length
    private byte ParsedExtraDataExpected = 0;

    private void RunReceive() {
        var buffer = new byte[1024];

        while (!ReceiveCancelEvent.Wait(0)) {
            try {
                int len = 0;
                try {
                    len = Stream.Read(buffer);
                } catch (OperationCanceledException) { }  // ignore this
                if (len > 0) {
                    int? iRollback = null;
                    for (var i = 0; i < len; i++) {
                        var b = buffer[i];

                        ParsedData[ParsedDataCount] = b;

                        //System.Diagnostics.Debug.WriteLine($"'{b:X2}' {ParsingState}[{ParsedDataCount}]");

                        switch (ParsingState) {
                            case ParseState.SearchHeader:
                                if (b == 0xBB) {
                                    ParsingState = ParseState.WaitAddress;
                                }
                                break;

                            case ParseState.WaitAddress:
                                if ((b == 0xBB) && (iRollback == null)) { iRollback = i; }
                                ParsingState = ParseState.WaitCommand;
                                break;

                            case ParseState.WaitCommand:
                                if ((b == 0xBB) && (iRollback == null)) { iRollback = i; }
                                ParsingState = ParseState.WaitDataLength;
                                break;

                            case ParseState.WaitDataLength:
                                if ((b == 0xBB) && (iRollback == null)) { iRollback = i; }
                                ParsingState = ParseState.WaitHeaderCrc8;
                                break;

                            case ParseState.WaitHeaderCrc8:
                                if ((b == 0xBB) && (iRollback == null)) { iRollback = i; }
                                var headerCrc8 = BBPacket.GetCrc8(ParsedData, 0, 4);
                                if (headerCrc8 == ParsedData[4]) {
                                    var hasEmbeededData = (ParsedData[2] & 0x20) == 0x20;
                                    if (hasEmbeededData) {
                                        ProcessReceivedPacket(ParsedData);
                                        ParsingState = ParseState.SearchHeader;  // done with embeeded byte
                                    } else {
                                        ParsedExtraDataExpected = (byte)(ParsedData[3] & 0x7F);
                                        if (ParsedExtraDataExpected == 0) {
                                            ProcessReceivedPacket(ParsedData);
                                            ParsingState = ParseState.SearchHeader;  // done when no data
                                        } else {
                                            ParsingState = ParseState.WaitExtraData;
                                        }
                                    }
                                } else {  // CRC not OK
                                    //System.Diagnostics.Debug.WriteLine("[BearBus] Invalid checksum 0x" + ParsedData[4].ToString("X2") + ", 0x" + headerCrc8.ToString("X2") + " expected");
                                    if (iRollback is not null and > 0) { i = iRollback.Value - 1; }  // try going back
                                    ParsingState = ParseState.SearchHeader;
                                }
                                break;

                            case ParseState.WaitExtraData:
                                ParsedExtraDataExpected--;
                                if (ParsedExtraDataExpected == 0) {
                                    if ((ParsedData[3] & 0x80) == 0x00) {
                                        ParsingState = ParseState.WaitExtraDataCrc8;
                                    } else {
                                        ParsingState = ParseState.WaitExtraDataCrc16a;
                                    }
                                }
                                break;

                            case ParseState.WaitExtraDataCrc8:
                                var dataCrc8 = BBPacket.GetCrc8(ParsedData, 4, ParsedDataCount - 4);
                                if (dataCrc8 == ParsedData[ParsedDataCount]) {
                                    ProcessReceivedPacket(ParsedData);
                                } else {  // CRC not OK
                                    //System.Diagnostics.Debug.WriteLine("[BearBus] Invalid checksum 0x" + ParsedData[ParsedDataCount].ToString("X2") + ", 0x" + dataCrc8.ToString("X2") + " expected");
                                }
                                ParsingState = ParseState.SearchHeader;
                                break;

                            case ParseState.WaitExtraDataCrc16a:
                                ParsingState = ParseState.WaitExtraDataCrc16b;
                                break;

                            case ParseState.WaitExtraDataCrc16b:
                                var dataCrc16 = BBPacket.GetCrc16(ParsedData, 4, ParsedDataCount - 5);
                                var dataCrc16H = (byte)(dataCrc16 >> 8);
                                var dataCrc16L = (byte)(dataCrc16 & 0xFF);
                                if ((dataCrc16H == ParsedData[ParsedDataCount - 1]) && (dataCrc16L == ParsedData[ParsedDataCount])) {
                                    ProcessReceivedPacket(ParsedData);
                                } else {  // CRC not OK
                                    //System.Diagnostics.Debug.WriteLine("[BearBus] Invalid checksum 0x" + ParsedData[ParsedDataCount - 1].ToString("X2") + ParsedData[ParsedDataCount].ToString("X2") + ", 0x" + dataCrc16H.ToString("X2") + dataCrc16L.ToString("X2") + " expected");
                                }
                                ParsingState = ParseState.SearchHeader;
                                break;
                        }

                        //System.Diagnostics.Debug.WriteLine($"     {ParsingState}");

                        if (ParsingState == ParseState.SearchHeader) {
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

    private void ProcessReceivedPacket(byte[] bytes) {
        var address = ParsedData[1];
        var isFromHost = (ParsedData[2] & 0x80) == 0x80;
        var isReplyOrError = (ParsedData[2] & 0x40) == 0x40;
        var hasEmbeededData = (ParsedData[2] & 0x20) == 0x20;
        var commandCode = (byte)(ParsedData[2] & 0x1F);
        var datumOrLength = ParsedData[3];

        if (hasEmbeededData) {
            EnqueueReceivedPacket(isFromHost, address, isReplyOrError, commandCode, new byte[] { datumOrLength });
        } else {
            var data = new byte[datumOrLength & 0x7F];
            Buffer.BlockCopy(bytes, 5, data, 0, data.Length);
            EnqueueReceivedPacket(isFromHost, address, isReplyOrError, commandCode, data);
        }
    }

    private void EnqueueReceivedPacket(bool isFromHost, byte address, bool isReplyOrError, byte commandCode, byte[] data) {
        lock (ReceiveQueue) {  // also ReceiveQueueSync inside
            if (isFromHost) {
                switch (commandCode) {
                    case 0x00: ReceiveQueue.Enqueue(new BBSystemHostPacket(address, data)); break;
                    default: ReceiveQueue.Enqueue(new BBCustomPacket(address, isReplyOrError, commandCode, data)); break;
                }
            } else {
                switch (commandCode) {
                    case 0x00: ReceiveQueue.Enqueue(new BBSystemDevicePacket(address, isReplyOrError, data)); break;
                    default: ReceiveQueue.Enqueue(new BBCustomReplyPacket(address, isReplyOrError, commandCode, data)); break;
                }
            }
            if (ReceiveQueueSync.CurrentCount == 0) { ReceiveQueueSync.Release(); }
        }
    }

    #endregion Receive


    #region Send

    private readonly SemaphoreSlim SendSemaphore = new(initialCount: 1);

    /// <summary>
    /// Sends packet to the stream.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    private protected void BaseSend(IBBPacket packet) {
        var bytes = packet.ToBytes();
        SendSemaphore.Wait();
        try {
            Stream.Write(bytes);
        } finally {
            SendSemaphore.Release();
        }
    }

    /// <summary>
    /// Sends packet to the stream.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    private protected async Task BaseSendAsync(IBBPacket packet) {
        var bytes = packet.ToBytes();
        await SendSemaphore.WaitAsync();
        try {
            await Stream.WriteAsync(bytes);
        } finally {
            SendSemaphore.Release();
        }
    }

    /// <summary>
    /// Sends packet to the stream.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private protected async Task BaseSendAsync(IBBPacket packet, CancellationToken cancellationToken) {
        var bytes = packet.ToBytes();
        await SendSemaphore.WaitAsync(cancellationToken);
        try {
            await Stream.WriteAsync(bytes, cancellationToken);
        } finally {
            SendSemaphore.Release();
        }
    }

    #endregion Send


    #region IDisposable

    private bool disposedValue;

    protected virtual void Dispose(bool disposing) {
        if (!disposedValue) {
            if (disposing) {
                ReceiveCancelEvent.Set();
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
public sealed class BearBusMonitor : BearBus {

    /// <summary>
    /// Createsa a new instance.
    /// </summary>
    /// <param name="stream">Stream.</param>
    /// <exception cref="ArgumentNullException">Stream cannot be null.</exception>
    public BearBusMonitor(Stream stream)
        : base(stream) {
    }


    /// <summary>
    /// Returns true if packet is received.
    /// </summary>
    /// <param name="packet">Packet received.</param>
    public bool TryReceive([MaybeNullWhen(false)] out IBBPacket packet) {
        return BaseTryReceive(out packet);
    }

    /// <summary>
    /// Returns packet once received.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    public IBBPacket Receive() {
        return Receive(Timeout.Infinite);
    }

    /// <summary>
    /// Returns packet once received.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    public IBBPacket Receive(int millisecondsTimeout) {
        return BaseReceive(millisecondsTimeout);
    }

    /// <summary>
    /// Returns once packet is received.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public IBBPacket Receive(CancellationToken cancellationToken) {
        return Receive(Timeout.Infinite, cancellationToken);
    }

    /// <summary>
    /// Returns once packet is received.
    /// Throws exception upon timeout.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public IBBPacket Receive(int millisecondsTimeout, CancellationToken cancellationToken) {
        return BaseReceive(millisecondsTimeout, cancellationToken);
    }

    /// <summary>
    /// Awaits for the next available packet.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    public async Task<IBBPacket> ReceiveAsync() {
        return await ReceiveAsync(Timeout.Infinite);
    }

    /// <summary>
    /// Awaits for the next available packet.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    public async Task<IBBPacket> ReceiveAsync(int millisecondsTimeout) {
        return await BaseReceiveAsync(millisecondsTimeout);
    }

    /// <summary>
    /// Awaits for the next available packet.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IBBPacket> ReceiveAsync(CancellationToken cancellationToken) {
        return await ReceiveAsync(Timeout.Infinite, cancellationToken);
    }

    /// <summary>
    /// Awaits for the next available packet.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IBBPacket> ReceiveAsync(int millisecondsTimeout, CancellationToken cancellationToken) {
        return await BaseReceiveAsync(millisecondsTimeout, cancellationToken);
    }

    /// <summary>
    /// Sends a packet.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    public void Send(IBBPacket packet) {
        if (packet == null) { throw new ArgumentNullException(nameof(packet), "Packet cannot be null."); }
        BaseSend(packet);
    }

    /// <summary>
    /// Sends a packet.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    public async Task SendAsync(IBBPacket packet) {
        if (packet == null) { throw new ArgumentNullException(nameof(packet), "Packet cannot be null."); }
        await BaseSendAsync(packet);
    }

    /// <summary>
    /// Sends a packet.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SendAsync(IBBPacket packet, CancellationToken cancellationToken) {
        if (packet == null) { throw new ArgumentNullException(nameof(packet), "Packet cannot be null."); }
        await BaseSendAsync(packet, cancellationToken);
    }

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
    public bool TryReceive([MaybeNullWhen(false)] out IBBDevicePacket packet) {
        while (BaseTryReceive(out var genericPacket)) {
            if (genericPacket is IBBDevicePacket devicePacket) {
                packet = devicePacket;
                return true;
            }
        }
        packet = null;
        return false;
    }

    /// <summary>
    /// Returns packet once received.
    /// Any non-device packet is dropped.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    public IBBDevicePacket Receive() {
        return Receive(Timeout.Infinite);
    }

    /// <summary>
    /// Returns packet once received.
    /// Any non-device packet is dropped.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    public IBBDevicePacket Receive(int millisecondsTimeout) {
        while (true) {
            var genericPacket = BaseReceive(millisecondsTimeout);
            if (genericPacket is IBBDevicePacket devicePacket) {
                return devicePacket;
            }
        }
    }

    /// <summary>
    /// Returns once packet is received.
    /// Any non-device packet is dropped.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public IBBDevicePacket Receive(CancellationToken cancellationToken) {
        return Receive(Timeout.Infinite, cancellationToken);
    }

    /// <summary>
    /// Returns once packet is received.
    /// Throws exception upon timeout.
    /// Any non-device packet is dropped.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public IBBDevicePacket Receive(int millisecondsTimeout, CancellationToken cancellationToken) {
        while (true) {
            var genericPacket = BaseReceive(millisecondsTimeout, cancellationToken);
            if (genericPacket is IBBDevicePacket devicePacket) {
                return devicePacket;
            }
        }
    }

    /// <summary>
    /// Awaits for the next available packet.
    /// Any non-device packet is dropped.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    public async Task<IBBDevicePacket> ReceiveAsync() {
        return await ReceiveAsync(Timeout.Infinite);
    }

    /// <summary>
    /// Awaits for the next available packet.
    /// Any non-device packet is dropped.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    public async Task<IBBDevicePacket> ReceiveAsync(int millisecondsTimeout) {
        while (true) {
            var genericPacket = await BaseReceiveAsync(millisecondsTimeout);
            if (genericPacket is IBBDevicePacket devicePacket) {
                return devicePacket;
            }
        }
    }

    /// <summary>
    /// Awaits for the next available packet.
    /// Any non-device packet is dropped.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IBBDevicePacket> ReceiveAsync(CancellationToken cancellationToken) {
        return await ReceiveAsync(Timeout.Infinite, cancellationToken);
    }

    /// <summary>
    /// Awaits for the next available packet.
    /// Any non-device packet is dropped.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IBBDevicePacket> ReceiveAsync(int millisecondsTimeout, CancellationToken cancellationToken) {
        while (true) {
            var genericPacket = await BaseReceiveAsync(millisecondsTimeout, cancellationToken);
            if (genericPacket is IBBDevicePacket devicePacket) {
                return devicePacket;
            }
        }
    }

    /// <summary>
    /// Sends a packet.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    public void Send(IBBHostPacket packet) {
        if (packet == null) { throw new ArgumentNullException(nameof(packet), "Packet cannot be null."); }
        BaseSend(packet);
    }

    /// <summary>
    /// Sends a packet.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    public async Task SendAsync(IBBHostPacket packet) {
        if (packet == null) { throw new ArgumentNullException(nameof(packet), "Packet cannot be null."); }
        await BaseSendAsync(packet);
    }

    /// <summary>
    /// Sends a packet.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SendAsync(IBBHostPacket packet, CancellationToken cancellationToken) {
        if (packet == null) { throw new ArgumentNullException(nameof(packet), "Packet cannot be null."); }
        await BaseSendAsync(packet, cancellationToken);
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
    public bool TryReceive([MaybeNullWhen(false)] out IBBHostPacket packet) {
        while (BaseTryReceive(out var genericPacket)) {
            if (genericPacket is IBBHostPacket hostPacket) {
                packet = hostPacket;
                return true;
            }
        }
        packet = null;
        return false;
    }


    /// <summary>
    /// Returns packet once received.
    /// Any non-host packet is dropped.
    /// </summary>
    public IBBHostPacket Receive() {
        return Receive(Timeout.Infinite);
    }

    /// <summary>
    /// Returns packet once received.
    /// Any non-host packet is dropped.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    public IBBHostPacket Receive(int millisecondsTimeout) {
        while (true) {
            var genericPacket = BaseReceive(millisecondsTimeout);
            if (genericPacket is IBBHostPacket hostPacket) {
                return hostPacket;
            }
        }
    }

    /// <summary>
    /// Returns once packet is received.
    /// Any non-host packet is dropped.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public IBBHostPacket Receive(CancellationToken cancellationToken) {
        return Receive(Timeout.Infinite, cancellationToken);
    }

    /// <summary>
    /// Returns once packet is received.
    /// Throws exception upon timeout.
    /// Any non-host packet is dropped.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public IBBHostPacket Receive(int millisecondsTimeout, CancellationToken cancellationToken) {
        while (true) {
            var genericPacket = BaseReceive(millisecondsTimeout, cancellationToken);
            if (genericPacket is IBBHostPacket hostPacket) {
                return hostPacket;
            }
        }
    }

    /// <summary>
    /// Awaits for the next available packet.
    /// Any non-host packet is dropped.
    /// </summary>
    public async Task<IBBHostPacket> ReceiveAsync() {
        return await ReceiveAsync(Timeout.Infinite);
    }

    /// <summary>
    /// Awaits for the next available packet.
    /// Any non-host packet is dropped.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    public async Task<IBBHostPacket> ReceiveAsync(int millisecondsTimeout) {
        while (true) {
            var genericPacket = await BaseReceiveAsync(millisecondsTimeout);
            if (genericPacket is IBBHostPacket devicePacket) {
                return devicePacket;
            }
        }
    }

    /// <summary>
    /// Awaits for the next available packet.
    /// Any non-host packet is dropped.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IBBHostPacket> ReceiveAsync(CancellationToken cancellationToken) {
        return await ReceiveAsync(Timeout.Infinite, cancellationToken);
    }

    /// <summary>
    /// Awaits for the next available packet.
    /// Any non-host packet is dropped.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, Infinite (-1) to wait indefinitely.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IBBHostPacket> ReceiveAsync(int millisecondsTimeout, CancellationToken cancellationToken) {
        while (true) {
            var genericPacket = await BaseReceiveAsync(millisecondsTimeout, cancellationToken);
            if (genericPacket is IBBHostPacket devicePacket) {
                return devicePacket;
            }
        }
    }

    /// <summary>
    /// Sends a packet.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    public void Send(IBBDevicePacket packet) {
        if (packet == null) { throw new ArgumentNullException(nameof(packet), "Packet cannot be null."); }
        BaseSend(packet);
    }

    /// <summary>
    /// Sends a packet.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    public async Task SendAsync(IBBDevicePacket packet) {
        if (packet == null) { throw new ArgumentNullException(nameof(packet), "Packet cannot be null."); }
        await BaseSendAsync(packet);
    }

    /// <summary>
    /// Sends a packet.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SendAsync(IBBDevicePacket packet, CancellationToken cancellationToken) {
        if (packet == null) { throw new ArgumentNullException(nameof(packet), "Packet cannot be null."); }
        await BaseSendAsync(packet, cancellationToken);
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
    internal protected BBPacket(bool isFromHost, byte address, bool isReplyRequestOrError, byte commandCode, byte[] data) {
        IsOriginHost = isFromHost;
        Address = address;
        IsReplyRequestOrError = isReplyRequestOrError;
        CommandCode = (byte)(commandCode & 0x1F);

        if (data == null) { data = Array.Empty<byte>(); }
        var newData = new byte[data.Length <= 127 ? data.Length : 127];
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
    /// 1-255 otherwise.
    /// </summary>
    public byte? FromAddress {
        get => IsOriginHost ? null : Address;
    }

    /// <summary>
    /// Address to which packet is destined.
    /// Null if packet is for host.
    /// 0 if packet is broadcast.
    /// 1-255 otherwise.
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
    /// Gets value of a first data byte or 0x00 if no data bytes are present.
    /// </summary>
    protected byte Datum {
        get => (DataBytes.Length >= 1) ? DataBytes[0] : (byte)0x00;
    }

    /// <summary>
    /// Returns bytes for the packet.
    /// </summary>
    public byte[] ToBytes() {
        var data = DataBytes;
        var dataLength = data.Length;
        var embedData = dataLength == 1;
        var trailingData = dataLength > 1;
        var needsCrc16 = dataLength > 12;
        var packetLength = 5;
        if (trailingData) { packetLength += needsCrc16 ? (dataLength + 2) : (dataLength + 1); }

        var packetBytes = new byte[packetLength];
        packetBytes[0] = 0xBB; // Header
        packetBytes[1] = Address;
        packetBytes[2] = (byte)((IsOriginHost ? 0x80 : 0x00)
                                | (IsReplyRequestOrError ? 0x40 : 0x00)
                                | (embedData ? 0x20 : 0x00)
                                | CommandCode);
        if (embedData) {
            packetBytes[3] = DataBytes[0];
        } else {
            packetBytes[3] = (byte)((needsCrc16 ? 0x80 : 0x00) | dataLength);
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
    private static readonly byte[] Crc8Lookup = new byte[] { 0x00, 0xC7, 0x67, 0xA0, 0xCE, 0x09, 0xA9, 0x6E, 0x75, 0xB2, 0x12, 0xD5, 0xBB, 0x7C, 0xDC, 0x1B, 0xEA, 0x2D, 0x8D, 0x4A, 0x24, 0xE3, 0x43, 0x84, 0x9F, 0x58, 0xF8, 0x3F, 0x51, 0x96, 0x36, 0xF1, 0x3D, 0xFA, 0x5A, 0x9D, 0xF3, 0x34, 0x94, 0x53, 0x48, 0x8F, 0x2F, 0xE8, 0x86, 0x41, 0xE1, 0x26, 0xD7, 0x10, 0xB0, 0x77, 0x19, 0xDE, 0x7E, 0xB9, 0xA2, 0x65, 0xC5, 0x02, 0x6C, 0xAB, 0x0B, 0xCC, 0x7A, 0xBD, 0x1D, 0xDA, 0xB4, 0x73, 0xD3, 0x14, 0x0F, 0xC8, 0x68, 0xAF, 0xC1, 0x06, 0xA6, 0x61, 0x90, 0x57, 0xF7, 0x30, 0x5E, 0x99, 0x39, 0xFE, 0xE5, 0x22, 0x82, 0x45, 0x2B, 0xEC, 0x4C, 0x8B, 0x47, 0x80, 0x20, 0xE7, 0x89, 0x4E, 0xEE, 0x29, 0x32, 0xF5, 0x55, 0x92, 0xFC, 0x3B, 0x9B, 0x5C, 0xAD, 0x6A, 0xCA, 0x0D, 0x63, 0xA4, 0x04, 0xC3, 0xD8, 0x1F, 0xBF, 0x78, 0x16, 0xD1, 0x71, 0xB6, 0xF4, 0x33, 0x93, 0x54, 0x3A, 0xFD, 0x5D, 0x9A, 0x81, 0x46, 0xE6, 0x21, 0x4F, 0x88, 0x28, 0xEF, 0x1E, 0xD9, 0x79, 0xBE, 0xD0, 0x17, 0xB7, 0x70, 0x6B, 0xAC, 0x0C, 0xCB, 0xA5, 0x62, 0xC2, 0x05, 0xC9, 0x0E, 0xAE, 0x69, 0x07, 0xC0, 0x60, 0xA7, 0xBC, 0x7B, 0xDB, 0x1C, 0x72, 0xB5, 0x15, 0xD2, 0x23, 0xE4, 0x44, 0x83, 0xED, 0x2A, 0x8A, 0x4D, 0x56, 0x91, 0x31, 0xF6, 0x98, 0x5F, 0xFF, 0x38, 0x8E, 0x49, 0xE9, 0x2E, 0x40, 0x87, 0x27, 0xE0, 0xFB, 0x3C, 0x9C, 0x5B, 0x35, 0xF2, 0x52, 0x95, 0x64, 0xA3, 0x03, 0xC4, 0xAA, 0x6D, 0xCD, 0x0A, 0x11, 0xD6, 0x76, 0xB1, 0xDF, 0x18, 0xB8, 0x7F, 0xB3, 0x74, 0xD4, 0x13, 0x7D, 0xBA, 0x1A, 0xDD, 0xC6, 0x01, 0xA1, 0x66, 0x08, 0xCF, 0x6F, 0xA8, 0x59, 0x9E, 0x3E, 0xF9, 0x97, 0x50, 0xF0, 0x37, 0x2C, 0xEB, 0x4B, 0x8C, 0xE2, 0x25, 0x85, 0x42, };  // 0x2F
    private static readonly ushort[] Crc16Lookup = new ushort[] { 0x0000, 0x1EA1, 0x3D42, 0x23E3, 0x7A84, 0x6425, 0x47C6, 0x5967, 0xF508, 0xEBA9, 0xC84A, 0xD6EB, 0x8F8C, 0x912D, 0xB2CE, 0xAC6F, 0x449B, 0x5A3A, 0x79D9, 0x6778, 0x3E1F, 0x20BE, 0x035D, 0x1DFC, 0xB193, 0xAF32, 0x8CD1, 0x9270, 0xCB17, 0xD5B6, 0xF655, 0xE8F4, 0x8936, 0x9797, 0xB474, 0xAAD5, 0xF3B2, 0xED13, 0xCEF0, 0xD051, 0x7C3E, 0x629F, 0x417C, 0x5FDD, 0x06BA, 0x181B, 0x3BF8, 0x2559, 0xCDAD, 0xD30C, 0xF0EF, 0xEE4E, 0xB729, 0xA988, 0x8A6B, 0x94CA, 0x38A5, 0x2604, 0x05E7, 0x1B46, 0x4221, 0x5C80, 0x7F63, 0x61C2, 0xBCE7, 0xA246, 0x81A5, 0x9F04, 0xC663, 0xD8C2, 0xFB21, 0xE580, 0x49EF, 0x574E, 0x74AD, 0x6A0C, 0x336B, 0x2DCA, 0x0E29, 0x1088, 0xF87C, 0xE6DD, 0xC53E, 0xDB9F, 0x82F8, 0x9C59, 0xBFBA, 0xA11B, 0x0D74, 0x13D5, 0x3036, 0x2E97, 0x77F0, 0x6951, 0x4AB2, 0x5413, 0x35D1, 0x2B70, 0x0893, 0x1632, 0x4F55, 0x51F4, 0x7217, 0x6CB6, 0xC0D9, 0xDE78, 0xFD9B, 0xE33A, 0xBA5D, 0xA4FC, 0x871F, 0x99BE, 0x714A, 0x6FEB, 0x4C08, 0x52A9, 0x0BCE, 0x156F, 0x368C, 0x282D, 0x8442, 0x9AE3, 0xB900, 0xA7A1, 0xFEC6, 0xE067, 0xC384, 0xDD25, 0xD745, 0xC9E4, 0xEA07, 0xF4A6, 0xADC1, 0xB360, 0x9083, 0x8E22, 0x224D, 0x3CEC, 0x1F0F, 0x01AE, 0x58C9, 0x4668, 0x658B, 0x7B2A, 0x93DE, 0x8D7F, 0xAE9C, 0xB03D, 0xE95A, 0xF7FB, 0xD418, 0xCAB9, 0x66D6, 0x7877, 0x5B94, 0x4535, 0x1C52, 0x02F3, 0x2110, 0x3FB1, 0x5E73, 0x40D2, 0x6331, 0x7D90, 0x24F7, 0x3A56, 0x19B5, 0x0714, 0xAB7B, 0xB5DA, 0x9639, 0x8898, 0xD1FF, 0xCF5E, 0xECBD, 0xF21C, 0x1AE8, 0x0449, 0x27AA, 0x390B, 0x606C, 0x7ECD, 0x5D2E, 0x438F, 0xEFE0, 0xF141, 0xD2A2, 0xCC03, 0x9564, 0x8BC5, 0xA826, 0xB687, 0x6BA2, 0x7503, 0x56E0, 0x4841, 0x1126, 0x0F87, 0x2C64, 0x32C5, 0x9EAA, 0x800B, 0xA3E8, 0xBD49, 0xE42E, 0xFA8F, 0xD96C, 0xC7CD, 0x2F39, 0x3198, 0x127B, 0x0CDA, 0x55BD, 0x4B1C, 0x68FF, 0x765E, 0xDA31, 0xC490, 0xE773, 0xF9D2, 0xA0B5, 0xBE14, 0x9DF7, 0x8356, 0xE294, 0xFC35, 0xDFD6, 0xC177, 0x9810, 0x86B1, 0xA552, 0xBBF3, 0x179C, 0x093D, 0x2ADE, 0x347F, 0x6D18, 0x73B9, 0x505A, 0x4EFB, 0xA60F, 0xB8AE, 0x9B4D, 0x85EC, 0xDC8B, 0xC22A, 0xE1C9, 0xFF68, 0x5307, 0x4DA6, 0x6E45, 0x70E4, 0x2983, 0x3722, 0x14C1, 0x0A60, };  // 0xA2EB

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
    /// 1-255 otherwise.
    /// </summary>
    public byte? FromAddress { get; }

    /// <summary>
    /// Address to which packet is destined.
    /// Null if packet is for host.
    /// 0 if packet is broadcast.
    /// 1-255 otherwise.
    /// </summary>
    public byte? ToAddress { get; }

    /// <summary>
    /// Gets command code.
    /// </summary>
    public byte CommandCode { get; }

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

    /// <summary>
    /// Address to which packet is destined.
    /// 0 if packet is broadcast.
    /// 1-255 otherwise.
    /// </summary>
    public byte DestinationAddress { get; }

    /// <summary>
    /// Gets if reply to this packet is requested from a device.
    /// </summary>
    public bool IsReplyRequested { get; }

}


/// <summary>
/// Packet that is sent by the device.
/// </summary>
public interface IBBDevicePacket : IBBPacket {

    /// <summary>
    /// Address from which packet originated.
    /// 0 if packet originated from unknown device.
    /// 1-255 otherwise.
    /// </summary>
    public byte SourceAddress { get; }

    /// <summary>
    /// Gets if this is an error reply.
    /// </summary>
    public bool IsErrorReply { get; }

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
        : base(isFromHost: true, destinationAddress, replyRequested, commandCode, data) {
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
    public bool IsReplyRequested {
        get => IsReplyRequestOrError;
    }


    #region Create

    /// <summary>
    /// Returns a new custom request packet.
    /// Reply will be requested unless destination is broadcast.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 255.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 31.</param>
    /// <exception cref="ArgumentOutOfRangeException">Command code must be between 1 and 31.</exception>
    public static BBCustomPacket Create(byte destinationAddress, byte commandCode) {
        return Create(destinationAddress, commandCode, Array.Empty<byte>(), replyRequested: (destinationAddress != 0));
    }

    /// <summary>
    /// Returns a new custom request packet.
    /// Reply will be requested unless destination is broadcast.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 255.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 31.</param>
    /// <param name="datum">Data byte.</param>
    /// <exception cref="ArgumentOutOfRangeException">Command code must be between 1 and 31.</exception>
    public static BBCustomPacket Create(byte destinationAddress, byte commandCode, byte datum) {
        return Create(destinationAddress, commandCode, new byte[] { datum }, replyRequested: (destinationAddress != 0));
    }

    /// <summary>
    /// Returns a new custom request packet.
    /// Reply will be requested unless destination is broadcast.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 255.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 31.</param>
    /// <param name="data">Data bytes.</param>
    /// <exception cref="ArgumentOutOfRangeException">Command code must be between 1 and 31. -or- Cannot have more than 127 bytes of data.</exception>
    public static BBCustomPacket Create(byte destinationAddress, byte commandCode, byte[] data) {
        return Create(destinationAddress, commandCode, data, replyRequested: (destinationAddress != 0));
    }

    /// <summary>
    /// Returns a new custom request packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 255.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 31.</param>
    /// <param name="datum">Data byte.</param>
    /// <param name="replyRequested">If true, a reply packet will be requested from device.</param>
    /// <exception cref="ArgumentOutOfRangeException">Command code must be between 1 and 31. -or- Cannot request reply to a broadcast packet.</exception>
    public static BBCustomPacket Create(byte destinationAddress, byte commandCode, byte datum, bool replyRequested) {
        return Create(destinationAddress, commandCode, new byte[] { datum }, replyRequested);
    }

    /// <summary>
    /// Returns a new custom request packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 255.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 31.</param>
    /// <param name="data">Data bytes.</param>
    /// <param name="replyRequested">If true, a reply packet will be requested from device.</param>
    /// <exception cref="ArgumentOutOfRangeException">Command code must be between 1 and 31. -or- Cannot have more than 127 bytes of data. -or- Cannot request reply to a broadcast packet.</exception>
    public static BBCustomPacket Create(byte destinationAddress, byte commandCode, byte[] data, bool replyRequested) {
        if (commandCode is < 1 or > 31) { throw new ArgumentOutOfRangeException(nameof(commandCode), "Command code must be between 1 and 31."); }
        if (data == null) { throw new ArgumentNullException(nameof(data), "Data cannot be null."); }
        if (data.Length > 127) { throw new ArgumentOutOfRangeException(nameof(data), "Cannot have more than 127 bytes of data."); }
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
        : base(isFromHost: false, sourceAddress, errorReply, commandCode, data) {
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
    public bool IsErrorReply {
        get => IsReplyRequestOrError;
    }


    #region Create

    /// <summary>
    /// Creates a new custom reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 255.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 31.</param>
    /// <exception cref="ArgumentOutOfRangeException">Cannot send a reply to a broadcast packet. -or- Command code must be between 1 and 31.</exception>
    public static BBCustomReplyPacket Create(byte sourceAddress, byte commandCode) {
        return Create(sourceAddress, commandCode, Array.Empty<byte>());
    }

    /// <summary>
    /// Creates a new custom reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 255.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 31.</param>
    /// <param name="datum">Data byte.</param>
    /// <exception cref="ArgumentOutOfRangeException">Cannot send a reply to a broadcast packet. -or- Command code must be between 1 and 31.</exception>
    public static BBCustomReplyPacket Create(byte sourceAddress, byte commandCode, byte datum) {
        return Create(sourceAddress, commandCode, new byte[] { datum });
    }

    /// <summary>
    /// Creates a new custom reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 255.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 31.</param>
    /// <param name="data">Data bytes.</param>
    /// <exception cref="ArgumentNullException">Data cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Cannot send a reply to a broadcast packet. -or- Command code must be between 1 and 31. -or- Cannot have more than 127 bytes of data.</exception>
    public static BBCustomReplyPacket Create(byte sourceAddress, byte commandCode, byte[] data) {
        if (sourceAddress is 0) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Cannot send a reply to a broadcast packet."); }
        if (commandCode is < 1 or > 31) { throw new ArgumentOutOfRangeException(nameof(commandCode), "Command code must be between 1 and 31."); }
        if (data == null) { throw new ArgumentNullException(nameof(data), "Data cannot be null."); }
        if (data.Length > 127) { throw new ArgumentOutOfRangeException(nameof(data), "Cannot have more than 127 bytes of data."); }

        return new BBCustomReplyPacket(sourceAddress, errorReply: false, commandCode, data);
    }

    #endregion Create

    #region CreateError

    /// <summary>
    /// Creates a new custom error reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 255.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 31.</param>
    /// <exception cref="ArgumentOutOfRangeException">Cannot send a reply to a broadcast packet. -or- Command code must be between 1 and 31.</exception>
    public static BBCustomReplyPacket CreateError(byte sourceAddress, byte commandCode) {
        return CreateError(sourceAddress, commandCode, Array.Empty<byte>());
    }

    /// <summary>
    /// Creates a new custom error reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 255.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 31.</param>
    /// <param name="datum">Data byte.</param>
    /// <exception cref="ArgumentOutOfRangeException">Cannot send a reply to a broadcast packet. -or- Command code must be between 1 and 31.</exception>
    public static BBCustomReplyPacket CreateError(byte sourceAddress, byte commandCode, byte datum) {
        return CreateError(sourceAddress, commandCode, new byte[] { datum });
    }

    /// <summary>
    /// Creates a new custom error reply packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be between 1 and 255.</param>
    /// <param name="commandCode">Command code. Must be between 1 and 31.</param>
    /// <param name="data">Data bytes.</param>
    /// <exception cref="ArgumentNullException">Data cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Cannot send a reply to a broadcast packet. -or- Command code must be between 1 and 31. -or- Cannot have more than 127 bytes of data.</exception>
    public static BBCustomReplyPacket CreateError(byte sourceAddress, byte commandCode, byte[] data) {
        if (sourceAddress is 0) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Cannot send a reply to a broadcast packet."); }
        if (commandCode is < 1 or > 31) { throw new ArgumentOutOfRangeException(nameof(commandCode), "Command code must be between 1 and 31."); }
        if (data == null) { throw new ArgumentNullException(nameof(data), "Data cannot be null."); }
        if (data.Length > 127) { throw new ArgumentOutOfRangeException(nameof(data), "Cannot have more than 127 bytes of data."); }

        return new BBCustomReplyPacket(sourceAddress, errorReply: true, commandCode, data);
    }

    #endregion CreateError

}


/// <summary>
/// System packet coming from host.
/// </summary>
public sealed record BBSystemHostPacket : BBPacket, IBBHostPacket {

    /// <summary>
    /// Creates a new instance.
    /// Only for internal use; not error checked.
    /// </summary>
    internal BBSystemHostPacket(byte sourceAddress, byte[] data)
        : base(isFromHost: true, sourceAddress, isReplyRequestOrError: false, 0x00, data) {
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
    public bool IsReplyRequested {
        get => false;  // must not have reply bit set
    }

    /// <summary>
    /// Gets action requested.
    /// </summary>
    public BBDeviceAction Action {
        get {
            return Datum switch {
                0x00 => BBDeviceAction.Ping,
                0x06 => BBDeviceAction.Reboot,
                0x08 => BBDeviceAction.BlinkOn,
                0x09 => BBDeviceAction.BlinkOff,
                0xFF => BBDeviceAction.FirmwareUpgrade,
                _ => BBDeviceAction.Unknown,
            };
        }
    }


    #region Create

    /// <summary>
    /// Creates a new packet to request an update.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 255.</param>
    public static BBSystemHostPacket CreateUpdateRequest(byte destinationAddress) {
        return new BBSystemHostPacket(destinationAddress, Array.Empty<byte>());
    }

    /// <summary>
    /// Creates a new packet for a reboot request.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 255.</param>
    public static BBSystemHostPacket CreateRebootRequest(byte destinationAddress) {
        return new BBSystemHostPacket(destinationAddress, new byte[] { 0x06 });
    }

    /// <summary>
    /// Creates a new unsolicited duplicate address report packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 255.</param>
    /// <param name="replyRequested">If true, a reply packet will be requested from device.</param>
    public static BBSystemHostPacket CreateBlinkOnRequest(byte destinationAddress) {
        return new BBSystemHostPacket(destinationAddress, new byte[] { 0x08 });
    }

    /// <summary>
    /// Creates a new unsolicited duplicate address report packet.
    /// </summary>
    /// <param name="destinationAddress">Destination address. Must be either 0 (broadcast) or between 1 and 255.</param>
    /// <param name="replyRequested">If true, a reply packet will be requested from device.</param>
    public static BBSystemHostPacket CreateBlinkOffRequest(byte destinationAddress) {
        return new BBSystemHostPacket(destinationAddress, new byte[] { 0x09 });
    }

    public static BBSystemHostPacket CreateFirmwareUpgradeRequest(byte destinationAddress) {
        return new BBSystemHostPacket(destinationAddress, new byte[] { 0xFF, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x57, 0x6F, 0x72, 0x6C, 0x64 });
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
        : base(isFromHost: false, sourceAddress, errorReply, 0x00, data) {
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
    public bool IsErrorReply {
        get => IsReplyRequestOrError;
    }

    /// <summary>
    /// Gets error code.
    /// </summary>
    public int ErrorCode {
        get { return Datum; }
    }


    #region Create

    /// <summary>
    /// Creates a new unsolicited status report packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be either 0 (not set) or between 1 and 255.</param>
    public static BBSystemDevicePacket CreateUnsolicitedUpdateReport(byte sourceAddress) {
        return new BBSystemDevicePacket(sourceAddress, errorReply: false, Array.Empty<byte>());
    }

    /// <summary>
    /// Creates a new unsolicited status report packet.
    /// </summary>
    /// <param name="sourceAddress">Source address. Must be either 0 (not set) or between 1 and 255.</param>
    /// <param name="blinking">True if LED blink is active.</param>
    /// <param name="mode">Current device mode.</param>
    /// <param name="errorCode">Current device error code.</param>
    public static BBSystemDevicePacket CreateUnsolicitedUpdateReport(byte sourceAddress, byte errorCode) {
        return new BBSystemDevicePacket(sourceAddress, errorReply: false, new byte[] { errorCode });
    }

    /// <summary>
    /// Creates a new unsolicited duplicate address report packet.
    /// </summary>
    /// <param name="sourceAddress">Source address.</param>
    /// <exception cref="ArgumentOutOfRangeException">Source address must be between 1 and 255.</exception>
    public static BBSystemDevicePacket CreateDuplicateAddressReport(byte sourceAddress) {
        if (sourceAddress is < 1 or > 255) { throw new ArgumentOutOfRangeException(nameof(sourceAddress), "Source address must be between 1 and 255."); }
        return new BBSystemDevicePacket(sourceAddress, errorReply: true, Array.Empty<byte>());
    }

    #endregion Create

}


/// <summary>
/// List of device actions.
/// </summary>
public enum BBDeviceAction {
    /// <summary>
    /// Unrecognized action.
    /// </summary>
    Unknown = -1,
    /// <summary>
    /// Ping.
    /// </summary>
    Ping = 0x00,
    /// <summary>
    /// Reboot.
    /// </summary>
    Reboot = 0x06,
    /// <summary>
    /// Blink on.
    /// </summary>
    BlinkOn = 0x08,
    /// <summary>
    /// Blink off.
    /// </summary>
    BlinkOff = 0x09,
    /// <summary>
    /// Firmware upgrade.
    /// </summary>
    FirmwareUpgrade = 0xFF,
}
