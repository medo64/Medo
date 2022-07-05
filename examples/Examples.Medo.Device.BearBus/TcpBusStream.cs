using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

internal class TcpBusStream : Stream {

    public ConcurrentDictionary<Stream, object?> InnerStreams = new();
    public ConcurrentQueue<byte> BytesToRead = new();

    public TcpBusStream() { }

    public void AddStream(Stream stream) {
        while (!InnerStreams.TryAdd(stream, null)) { }

        ThreadPool.QueueUserWorkItem(delegate {
            while (true) {
                byte value;
                try {
                    value = (byte)stream.ReadByte();  // just read byte-by-byte to maximize mixing with other streams and possibility of corruption
                } catch (IOException) { continue; }  // just ignore

                BytesToRead.Enqueue(value);
                foreach (var kvp in InnerStreams) { // distribute to all streams (including yourself)
                    var innerStream = kvp.Key;
                    // if (innerStream == stream) { continue; }  // don't write to self
                    ThreadPool.QueueUserWorkItem(delegate {
                        try {
                            innerStream.WriteByte(value);
                        } catch (IOException) {  // stream is probably gone
                            InnerStreams.TryRemove(kvp);
                        }
                    });
                }
            }
        });
    }


    public override bool CanSeek => throw new NotImplementedException();
    public override long Length => throw new NotImplementedException();
    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override long Seek(long offset, SeekOrigin origin) { throw new NotImplementedException(); }
    public override void SetLength(long value) { throw new NotImplementedException(); }

    public override bool CanRead => true;
    public override bool CanWrite => true;
    public override void Flush() { }

    public override int Read(byte[] buffer, int offset, int count) {
        while (BytesToRead.IsEmpty) { Thread.Sleep(1); }

        var output = new List<byte>();
        while (output.Count < count) {
            if (BytesToRead.TryDequeue(out var oneByte)) {
                output.Add(oneByte);
            } else { // let's call it done if we fail to dequeue it
                break;
            }
        }
        Buffer.BlockCopy(output.ToArray(), 0, buffer, offset, count);
        return output.Count;
    }

    public override void Write(byte[] buffer, int offset, int count) {
        foreach (var kvp in InnerStreams) { // distribute to all other streams
            var innerStream = kvp.Key;
            ThreadPool.QueueUserWorkItem(delegate {
                try {
                    innerStream.Write(buffer, offset, count);
                } catch (IOException) {  // stream is probably gone
                    InnerStreams.TryRemove(kvp);
                }
            });
        }
    }
}
