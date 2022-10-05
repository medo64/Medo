using System;
using System.IO;

namespace Tests;

internal class TestStream : Stream {

    public TestStream() {
    }

    public TestStream(byte[] buffer) {
        ReadStream.Write(buffer);
        Position = 0;
    }


    private readonly MemoryStream ReadStream = new();
    private readonly MemoryStream WriteStream = new();


    public void SetupRead(byte[] buffer) {
        var pos = ReadStream.Position;
        ReadStream.Write(buffer);
        ReadStream.Position = pos;
    }

    public byte[] ToWrittenArray() {
        return WriteStream.ToArray();
    }


    #region Stream

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => 0;
    public override long Position { get => ReadStream.Position; set => ReadStream.Position = value; }

    public override void Flush() {
        ReadStream.Flush();
        WriteStream.Flush();
    }

    public override long Seek(long offset, SeekOrigin origin) {
        throw new NotImplementedException();
    }

    public override void SetLength(long value) {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count) {
        return ReadStream.Read(buffer, offset, count);
    }

    public override void Write(byte[] buffer, int offset, int count) {
        WriteStream.Write(buffer, offset, count);
    }

    #endregion Stream

}
