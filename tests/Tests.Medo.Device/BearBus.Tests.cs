using System;
using Xunit;
using Medo.Device;
using System.IO;

namespace Tests.Medo.Device;

public class BearBusTests {

    #region Packets

    [Fact(DisplayName = "BearBus: Custom Short Packet")]
    public void CustomShort() {
        var packet = BBCustomPacket.Create(destinationAddress: 5,
                                        commandCode: 29,
                                        datum: 0x42);
        Assert.Equal(5, packet.DestinationAddress);
        Assert.True(packet.IsReplyRequested);
        Assert.Equal(29, packet.CommandCode);
        Assert.Equal("42", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-05-FD-42-5B", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply();
        Assert.Equal(5, packetReply.SourceAddress);
        Assert.False(packetReply.IsErrorReply);
        Assert.Equal(29, packetReply.CommandCode);
        Assert.Equal("", BitConverter.ToString(packetReply.Data));
        Assert.Equal("BB-05-1D-00-3A", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply();
        Assert.Equal(5, packetErrorReply.SourceAddress);
        Assert.True(packetErrorReply.IsErrorReply);
        Assert.Equal(29, packetErrorReply.CommandCode);
        Assert.Equal("", BitConverter.ToString(packetErrorReply.Data));
        Assert.Equal("BB-05-5D-00-AE", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Custom Short Packet (No Reply)")]
    public void CustomShortNoReply() {
        var packet = BBCustomPacket.Create(destinationAddress: 5,
                                        commandCode: 29,
                                        datum: 0x42,
                                        replyRequested: false);
        Assert.Equal(5, packet.DestinationAddress);
        Assert.False(packet.IsReplyRequested);
        Assert.Equal(29, packet.CommandCode);
        Assert.Equal("42", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-05-BD-42-CF", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply();
        Assert.Equal(5, packetReply.SourceAddress);
        Assert.False(packetReply.IsErrorReply);
        Assert.Equal(29, packetReply.CommandCode);
        Assert.Equal("", BitConverter.ToString(packetReply.Data));
        Assert.Equal("BB-05-1D-00-3A", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply();
        Assert.Equal(5, packetErrorReply.SourceAddress);
        Assert.True(packetErrorReply.IsErrorReply);
        Assert.Equal(29, packetErrorReply.CommandCode);
        Assert.Equal("", BitConverter.ToString(packetErrorReply.Data));
        Assert.Equal("BB-05-5D-00-AE", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Custom Basic packet")]
    public void CustomBasic() {
        var packet = BBCustomPacket.Create(42, 1, new byte[] { 1, 2, 3 });
        Assert.Equal(42, packet.DestinationAddress);
        Assert.True(packet.IsReplyRequested);
        Assert.Equal(1, packet.CommandCode);
        Assert.Equal("01-02-03", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-2A-C1-03-18-01-02-03-CF", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(new byte[] { 0x5A, 0x5B });
        Assert.Equal(42, packetReply.SourceAddress);
        Assert.False(packetReply.IsErrorReply);
        Assert.Equal(1, packetReply.CommandCode);
        Assert.Equal("5A-5B", BitConverter.ToString(packetReply.Data));
        Assert.Equal("BB-2A-01-02-A4-5A-5B-BD", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply(new byte[] { 0x6A, 0x6B });
        Assert.Equal(42, packetErrorReply.SourceAddress);
        Assert.True(packetErrorReply.IsErrorReply);
        Assert.Equal(1, packetErrorReply.CommandCode);
        Assert.Equal("6A-6B", BitConverter.ToString(packetErrorReply.Data));
        Assert.Equal("BB-2A-41-02-30-6A-6B-1A", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Custom Basic Packet (No Reply)")]
    public void CustomBasicNoReply() {
        var packet = BBCustomPacket.Create(destinationAddress: 19,
                                        commandCode: 26,
                                        data: new byte[] { 0x42, 0x43, 0x44 },
                                        replyRequested: false);
        Assert.Equal(19, packet.DestinationAddress);
        Assert.False(packet.IsReplyRequested);
        Assert.Equal(26, packet.CommandCode);
        Assert.Equal("42-43-44", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-13-9A-03-49-42-43-44-4E", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(new byte[] { 0x5A, 0x5B });
        Assert.Equal(19, packetReply.SourceAddress);
        Assert.False(packetReply.IsErrorReply);
        Assert.Equal(26, packetReply.CommandCode);
        Assert.Equal("5A-5B", BitConverter.ToString(packetReply.Data));
        Assert.Equal("BB-13-1A-02-61-5A-5B-B7", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply(new byte[] { 0x6A, 0x6B });
        Assert.Equal(19, packetErrorReply.SourceAddress);
        Assert.True(packetErrorReply.IsErrorReply);
        Assert.Equal(26, packetErrorReply.CommandCode);
        Assert.Equal("6A-6B", BitConverter.ToString(packetErrorReply.Data));
        Assert.Equal("BB-13-5A-02-F5-6A-6B-10", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Custom Basic packet (shortest)")]
    public void CustomBasicShortest() {
        var packet = BBCustomPacket.Create(42, 1, new byte[] { 1, 2 });
        Assert.Equal(42, packet.DestinationAddress);
        Assert.True(packet.IsReplyRequested);
        Assert.Equal(1, packet.CommandCode);
        Assert.Equal("01-02", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-2A-C1-02-37-01-02-92", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(new byte[] { 101, 102 });
        Assert.Equal(42, packetReply.SourceAddress);
        Assert.False(packetReply.IsErrorReply);
        Assert.Equal(1, packetReply.CommandCode);
        Assert.Equal("65-66", BitConverter.ToString(packetReply.Data));
        Assert.Equal("BB-2A-01-02-A4-65-66-8F", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply(new byte[] { 201, 202 });
        Assert.Equal(42, packetErrorReply.SourceAddress);
        Assert.True(packetErrorReply.IsErrorReply);
        Assert.Equal(1, packetErrorReply.CommandCode);
        Assert.Equal("C9-CA", BitConverter.ToString(packetErrorReply.Data));
        Assert.Equal("BB-2A-41-02-30-C9-CA-FC", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Custom Basic packet (longest)")]
    public void CustomBasicLongest() {
        var packet = BBCustomPacket.Create(42, 1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
        Assert.Equal(42, packet.DestinationAddress);
        Assert.True(packet.IsReplyRequested);
        Assert.Equal(1, packet.CommandCode);
        Assert.Equal("01-02-03-04-05-06-07-08-09-0A-0B-0C", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-2A-C1-0C-82-01-02-03-04-05-06-07-08-09-0A-0B-0C-0F", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(new byte[] { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112 });
        Assert.Equal(42, packetReply.SourceAddress);
        Assert.False(packetReply.IsErrorReply);
        Assert.Equal(1, packetReply.CommandCode);
        Assert.Equal("65-66-67-68-69-6A-6B-6C-6D-6E-6F-70", BitConverter.ToString(packetReply.Data));
        Assert.Equal("BB-2A-01-0C-11-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-22", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply(new byte[] { 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212 });
        Assert.Equal(42, packetErrorReply.SourceAddress);
        Assert.True(packetErrorReply.IsErrorReply);
        Assert.Equal(1, packetErrorReply.CommandCode);
        Assert.Equal("C9-CA-CB-CC-CD-CE-CF-D0-D1-D2-D3-D4", BitConverter.ToString(packetErrorReply.Data));
        Assert.Equal("BB-2A-41-0C-85-C9-CA-CB-CC-CD-CE-CF-D0-D1-D2-D3-D4-2B", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Custom Long Packet")]
    public void CustomLong() {
        var packet = BBCustomPacket.Create(destinationAddress: 1,
                                           commandCode: 1,
                                           data: new byte[] { 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E });
        Assert.Equal(1, packet.DestinationAddress);
        Assert.True(packet.IsReplyRequested);
        Assert.Equal(1, packet.CommandCode);
        Assert.Equal("42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-01-C1-0D-20-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-28-DB", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(new byte[] { 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D });
        Assert.Equal(1, packetReply.SourceAddress);
        Assert.False(packetReply.IsErrorReply);
        Assert.Equal(1, packetReply.CommandCode);
        Assert.Equal("51-52-53-54-55-56-57-58-59-5A-5B-5C-5D", BitConverter.ToString(packetReply.Data));
        Assert.Equal("BB-01-01-0D-B3-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-71-2C", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply(new byte[] { 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D });
        Assert.Equal(1, packetErrorReply.SourceAddress);
        Assert.True(packetErrorReply.IsErrorReply);
        Assert.Equal(1, packetErrorReply.CommandCode);
        Assert.Equal("61-62-63-64-65-66-67-68-69-6A-6B-6C-6D", BitConverter.ToString(packetErrorReply.Data));
        Assert.Equal("BB-01-41-0D-27-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-08-A0", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Custom Long Packet (no reply)")]
    public void CustomLongNoReply() {
        var packet = BBCustomPacket.Create(destinationAddress: 1,
                                           commandCode: 1,
                                           data: new byte[] { 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E },
                                           replyRequested: false);
        Assert.Equal(1, packet.DestinationAddress);
        Assert.False(packet.IsReplyRequested);
        Assert.Equal(1, packet.CommandCode);
        Assert.Equal("42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-01-81-0D-B4-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-DA-5C", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(new byte[] { 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D });
        Assert.Equal(1, packetReply.SourceAddress);
        Assert.False(packetReply.IsErrorReply);
        Assert.Equal(1, packetReply.CommandCode);
        Assert.Equal("51-52-53-54-55-56-57-58-59-5A-5B-5C-5D", BitConverter.ToString(packetReply.Data));
        Assert.Equal("BB-01-01-0D-B3-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-71-2C", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply(new byte[] { 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D });
        Assert.Equal(1, packetErrorReply.SourceAddress);
        Assert.True(packetErrorReply.IsErrorReply);
        Assert.Equal(1, packetErrorReply.CommandCode);
        Assert.Equal("61-62-63-64-65-66-67-68-69-6A-6B-6C-6D", BitConverter.ToString(packetErrorReply.Data));
        Assert.Equal("BB-01-41-0D-27-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-08-A0", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Custom Long Packet (longest)")]
    public void CustomLongLongest() {
        var dataBytes = new byte[248];
        for (var i = 0; i < dataBytes.Length; i++) { dataBytes[i] = (byte)(i + 1); }
        var packet = BBCustomPacket.Create(destinationAddress: 1,
                                           commandCode: 1,
                                           data: dataBytes);
        Assert.Equal(1, packet.DestinationAddress);
        Assert.True(packet.IsReplyRequested);
        Assert.Equal(1, packet.CommandCode);
        Assert.Equal("01-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F-80-81-82-83-84-85-86-87-88-89-8A-8B-8C-8D-8E-8F-90-91-92-93-94-95-96-97-98-99-9A-9B-9C-9D-9E-9F-A0-A1-A2-A3-A4-A5-A6-A7-A8-A9-AA-AB-AC-AD-AE-AF-B0-B1-B2-B3-B4-B5-B6-B7-B8-B9-BA-BB-BC-BD-BE-BF-C0-C1-C2-C3-C4-C5-C6-C7-C8-C9-CA-CB-CC-CD-CE-CF-D0-D1-D2-D3-D4-D5-D6-D7-D8-D9-DA-DB-DC-DD-DE-DF-E0-E1-E2-E3-E4-E5-E6-E7-E8-E9-EA-EB-EC-ED-EE-EF-F0-F1-F2-F3-F4-F5-F6-F7-F8", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-01-C1-F8-6B-01-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F-80-81-82-83-84-85-86-87-88-89-8A-8B-8C-8D-8E-8F-90-91-92-93-94-95-96-97-98-99-9A-9B-9C-9D-9E-9F-A0-A1-A2-A3-A4-A5-A6-A7-A8-A9-AA-AB-AC-AD-AE-AF-B0-B1-B2-B3-B4-B5-B6-B7-B8-B9-BA-BB-BC-BD-BE-BF-C0-C1-C2-C3-C4-C5-C6-C7-C8-C9-CA-CB-CC-CD-CE-CF-D0-D1-D2-D3-D4-D5-D6-D7-D8-D9-DA-DB-DC-DD-DE-DF-E0-E1-E2-E3-E4-E5-E6-E7-E8-E9-EA-EB-EC-ED-EE-EF-F0-F1-F2-F3-F4-F5-F6-F7-F8-EE-7C", BitConverter.ToString(packet.ToBytes()));

        dataBytes[0] = 101;
        var packetReply = packet.GetReply(dataBytes);
        Assert.Equal(1, packetReply.SourceAddress);
        Assert.False(packetReply.IsErrorReply);
        Assert.Equal(1, packetReply.CommandCode);
        Assert.Equal("65-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F-80-81-82-83-84-85-86-87-88-89-8A-8B-8C-8D-8E-8F-90-91-92-93-94-95-96-97-98-99-9A-9B-9C-9D-9E-9F-A0-A1-A2-A3-A4-A5-A6-A7-A8-A9-AA-AB-AC-AD-AE-AF-B0-B1-B2-B3-B4-B5-B6-B7-B8-B9-BA-BB-BC-BD-BE-BF-C0-C1-C2-C3-C4-C5-C6-C7-C8-C9-CA-CB-CC-CD-CE-CF-D0-D1-D2-D3-D4-D5-D6-D7-D8-D9-DA-DB-DC-DD-DE-DF-E0-E1-E2-E3-E4-E5-E6-E7-E8-E9-EA-EB-EC-ED-EE-EF-F0-F1-F2-F3-F4-F5-F6-F7-F8", BitConverter.ToString(packetReply.Data));
        Assert.Equal("BB-01-01-F8-F8-65-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F-80-81-82-83-84-85-86-87-88-89-8A-8B-8C-8D-8E-8F-90-91-92-93-94-95-96-97-98-99-9A-9B-9C-9D-9E-9F-A0-A1-A2-A3-A4-A5-A6-A7-A8-A9-AA-AB-AC-AD-AE-AF-B0-B1-B2-B3-B4-B5-B6-B7-B8-B9-BA-BB-BC-BD-BE-BF-C0-C1-C2-C3-C4-C5-C6-C7-C8-C9-CA-CB-CC-CD-CE-CF-D0-D1-D2-D3-D4-D5-D6-D7-D8-D9-DA-DB-DC-DD-DE-DF-E0-E1-E2-E3-E4-E5-E6-E7-E8-E9-EA-EB-EC-ED-EE-EF-F0-F1-F2-F3-F4-F5-F6-F7-F8-07-DB", BitConverter.ToString(packetReply.ToBytes()));

        dataBytes[0] = 201;
        var packetErrorReply = packet.GetErrorReply(dataBytes);
        Assert.Equal(1, packetErrorReply.SourceAddress);
        Assert.True(packetErrorReply.IsErrorReply);
        Assert.Equal(1, packetErrorReply.CommandCode);
        Assert.Equal("C9-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F-80-81-82-83-84-85-86-87-88-89-8A-8B-8C-8D-8E-8F-90-91-92-93-94-95-96-97-98-99-9A-9B-9C-9D-9E-9F-A0-A1-A2-A3-A4-A5-A6-A7-A8-A9-AA-AB-AC-AD-AE-AF-B0-B1-B2-B3-B4-B5-B6-B7-B8-B9-BA-BB-BC-BD-BE-BF-C0-C1-C2-C3-C4-C5-C6-C7-C8-C9-CA-CB-CC-CD-CE-CF-D0-D1-D2-D3-D4-D5-D6-D7-D8-D9-DA-DB-DC-DD-DE-DF-E0-E1-E2-E3-E4-E5-E6-E7-E8-E9-EA-EB-EC-ED-EE-EF-F0-F1-F2-F3-F4-F5-F6-F7-F8", BitConverter.ToString(packetErrorReply.Data));
        Assert.Equal("BB-01-41-F8-6C-C9-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F-80-81-82-83-84-85-86-87-88-89-8A-8B-8C-8D-8E-8F-90-91-92-93-94-95-96-97-98-99-9A-9B-9C-9D-9E-9F-A0-A1-A2-A3-A4-A5-A6-A7-A8-A9-AA-AB-AC-AD-AE-AF-B0-B1-B2-B3-B4-B5-B6-B7-B8-B9-BA-BB-BC-BD-BE-BF-C0-C1-C2-C3-C4-C5-C6-C7-C8-C9-CA-CB-CC-CD-CE-CF-D0-D1-D2-D3-D4-D5-D6-D7-D8-D9-DA-DB-DC-DD-DE-DF-E0-E1-E2-E3-E4-E5-E6-E7-E8-E9-EA-EB-EC-ED-EE-EF-F0-F1-F2-F3-F4-F5-F6-F7-F8-93-72", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Custom Long Packet (too long)")]
    public void CustomLongTooLong() {
        var dataBytes = new byte[249];
        Assert.Throws<ArgumentOutOfRangeException>(() => {
            var packet = BBCustomPacket.Create(destinationAddress: 1,
                                               commandCode: 1,
                                               data: dataBytes);
        });
    }

    #endregion Packets

    #region System

    [Fact(DisplayName = "BearBus: Unsolicited Update")]
    public void UnsoliticedUpdate() {
        var packet = BBSystemDevicePacket.CreateUnsolicitedUpdateReport(200);
        Assert.Equal(200, packet.SourceAddress);
        Assert.False(packet.IsErrorReply);
        Assert.Equal("", BitConverter.ToString(packet.Data));
        Assert.Equal(0x00, packet.ErrorCode);
        Assert.Equal("BB-C8-00-00-DC", BitConverter.ToString(packet.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Unsolicited Update (error)")]
    public void UnsoliticedUpdateWithError() {
        var packet = BBSystemDevicePacket.CreateUnsolicitedUpdateReport(201, 1);
        Assert.Equal(201, packet.SourceAddress);
        Assert.False(packet.IsErrorReply);
        Assert.Equal("01", BitConverter.ToString(packet.Data));
        Assert.Equal(0x01, packet.ErrorCode);
        Assert.Equal("BB-C9-20-01-B7", BitConverter.ToString(packet.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Unsolicited Duplicate Address Report")]
    public void SystemDuplicateAddressReport() {
        var packet = BBSystemDevicePacket.CreateDuplicateAddressReport(76);
        Assert.Equal(76, packet.SourceAddress);
        Assert.True(packet.IsErrorReply);
        Assert.Equal(0x00, packet.CommandCode);
        Assert.Equal("", BitConverter.ToString(packet.Data));
        Assert.Equal(0x00, packet.ErrorCode);
        Assert.Equal("BB-4C-40-00-BD", BitConverter.ToString(packet.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Update Request")]
    public void SystemUpdateRequest() {
        var packet = BBSystemHostPacket.CreateUpdateRequest(254);
        Assert.Equal(254, packet.DestinationAddress);
        Assert.Equal(BBDeviceAction.Ping, packet.Action);
        Assert.Equal("", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-FE-80-00-F0", BitConverter.ToString(packet.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Update Response")]
    public void SystemUpdateRewsponse() {
        var packet = BBSystemDevicePacket.CreateUnsolicitedUpdateReport(254, 0);
        Assert.Equal(254, packet.SourceAddress);
        Assert.False(packet.IsErrorReply);
        Assert.Equal("00", BitConverter.ToString(packet.Data));
        Assert.Equal(0x00, packet.ErrorCode);
        Assert.Equal("BB-FE-20-00-BD", BitConverter.ToString(packet.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Device Reboot")]
    public void SystemReboot() {
        var packet = BBSystemHostPacket.CreateRebootRequest(32);
        Assert.Equal(32, packet.DestinationAddress);
        Assert.Equal(BBDeviceAction.Reboot, packet.Action);
        Assert.Equal("06", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-20-A0-06-D0", BitConverter.ToString(packet.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Broadcasted Reboot")]
    public void SystemRebootAll() {
        var packet = BBSystemHostPacket.CreateRebootRequest(0);
        Assert.Equal(0, packet.DestinationAddress);
        Assert.Equal(BBDeviceAction.Reboot, packet.Action);
        Assert.Equal("06", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-00-A0-06-3F", BitConverter.ToString(packet.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Request Blink On")]
    public void SystemBlinkOn() {
        var packet = BBSystemHostPacket.CreateBlinkOnRequest(129);
        Assert.Equal(129, packet.DestinationAddress);
        Assert.Equal(BBDeviceAction.BlinkOn, packet.Action);
        Assert.Equal("08", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-81-A0-08-49", BitConverter.ToString(packet.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Request Blink Off")]
    public void SystemBlinkOff() {
        var packet = BBSystemHostPacket.CreateBlinkOffRequest(129);
        Assert.Equal(129, packet.DestinationAddress);
        Assert.Equal(BBDeviceAction.BlinkOff, packet.Action);
        Assert.Equal("09", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-81-A0-09-66", BitConverter.ToString(packet.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Enter firmware upgrade mode")]
    public void SystemEnterFirmwareUpgrade() {
        var packet = BBSystemHostPacket.CreateFirmwareUpgradeRequest(222);
        Assert.Equal(222, packet.DestinationAddress);
        Assert.Equal(BBDeviceAction.FirmwareUpgrade, packet.Action);
        Assert.Equal("FF-48-65-6C-6C-6F-20-57-6F-72-6C-64", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-DE-80-0C-F4-FF-48-65-6C-6C-6F-20-57-6F-72-6C-64-9F", BitConverter.ToString(packet.ToBytes()));
    }

    #endregion System

    #region Setup


    #endregion Setup


    #region Stream

    [Fact(DisplayName = "BearBus: Host Send And Receive")]
    public void HostSendAndReceive() {
        var stream = new MemoryStream();

        var hostBus = new BearBusHost(stream);
        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(0));
        Assert.Equal("BB-00-A0-06-3F", BitConverter.ToString(stream.ToArray()));

        stream = new MemoryStream(stream.ToArray());

        var deviceBus = new BearBusDevice(stream);
        var packet = deviceBus.Receive(100);
        Assert.Null(packet.FromAddress);
        Assert.Equal((byte?)0, packet.ToAddress);
        Assert.Equal(0, packet.DestinationAddress);
        Assert.Equal(0, packet.CommandCode);
        Assert.Equal("06", BitConverter.ToString(packet.Data));

        Assert.Throws<TimeoutException>(() => {
            deviceBus.Receive(100);
        });
    }

    [Fact(DisplayName = "BearBus: Host Send And Receive (255 data bytes)")]
    public void HostSendAndReceive255DataBytes() {
        var stream = new MemoryStream(new byte[] { 0xBB, 0x01, 0xC1, 0xFF, 0xA6, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F, 0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F, 0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0x9B, 0x9C, 0x9D, 0x9E, 0x9F, 0xA0, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xAB, 0xAC, 0xAD, 0xAE, 0xAF, 0xB0, 0xB1, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xBB, 0xBC, 0xBD, 0xBE, 0xBF, 0xC0, 0xC1, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xCB, 0xCC, 0xCD, 0xCE, 0xCF, 0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF, 0xE0, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xEB, 0xEC, 0xED, 0xEE, 0xEF, 0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xFF, 0x22, 0x66 });

        var deviceBus = new BearBusDevice(stream);
        var packet = deviceBus.Receive(100);
        Assert.Null(packet.FromAddress);
        Assert.Equal((byte?)1, packet.ToAddress);
        Assert.Equal(1, packet.DestinationAddress);
        Assert.Equal(1, packet.CommandCode);
        Assert.Equal("01-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F-80-81-82-83-84-85-86-87-88-89-8A-8B-8C-8D-8E-8F-90-91-92-93-94-95-96-97-98-99-9A-9B-9C-9D-9E-9F-A0-A1-A2-A3-A4-A5-A6-A7-A8-A9-AA-AB-AC-AD-AE-AF-B0-B1-B2-B3-B4-B5-B6-B7-B8-B9-BA-BB-BC-BD-BE-BF-C0-C1-C2-C3-C4-C5-C6-C7-C8-C9-CA-CB-CC-CD-CE-CF-D0-D1-D2-D3-D4-D5-D6-D7-D8-D9-DA-DB-DC-DD-DE-DF-E0-E1-E2-E3-E4-E5-E6-E7-E8-E9-EA-EB-EC-ED-EE-EF-F0-F1-F2-F3-F4-F5-F6-F7-F8-F9-FA-FB-FC-FD-FE-FF", BitConverter.ToString(packet.Data));

        Assert.Throws<TimeoutException>(() => {
            deviceBus.Receive(100);
        });
    }

    [Fact(DisplayName = "BearBus: Host Send And Receive Async")]
    public async void HostSendAndReceiveAsync() {
        var stream = new MemoryStream();

        var hostBus = new BearBusHost(stream);
        await hostBus.SendAsync(BBSystemHostPacket.CreateRebootRequest(0));
        Assert.Equal("BB-00-A0-06-3F", BitConverter.ToString(stream.ToArray()));

        stream = new MemoryStream(stream.ToArray());

        var deviceBus = new BearBusDevice(stream);
        var packet = await deviceBus.ReceiveAsync(100);
        Assert.Null(packet.FromAddress);
        Assert.Equal((byte?)0, packet.ToAddress);
        Assert.Equal(0, packet.DestinationAddress);
        Assert.Equal(0, packet.CommandCode);
        Assert.Equal("06", BitConverter.ToString(packet.Data));

        await Assert.ThrowsAsync<TimeoutException>(async () => {
            await deviceBus.ReceiveAsync(100);
        });
    }


    [Fact(DisplayName = "BearBus: Host Send And Receive (multiple)")]
    public void HostSendAndReceiveMulti() {
        var stream = new MemoryStream();
        var hostBus = new BearBusHost(stream);

        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(42));
        hostBus.Send(BBCustomPacket.Create(42, 1, new byte[] { 0x0D, 0x0A }));
        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(42));

        stream = new MemoryStream(stream.ToArray());
        var deviceBus = new BearBusDevice(stream);

        {
            var packet = deviceBus.Receive(100);
            Assert.Null(packet.FromAddress);
            Assert.Equal((byte?)42, packet.ToAddress);
            Assert.Equal(42, packet.DestinationAddress);
            Assert.Equal(0, packet.CommandCode);
            Assert.Equal("06", BitConverter.ToString(packet.Data));
        }

        {
            var packet = deviceBus.Receive(100);
            Assert.Null(packet.FromAddress);
            Assert.Equal((byte?)42, packet.ToAddress);
            Assert.Equal(42, packet.DestinationAddress);
            Assert.Equal(1, packet.CommandCode);
            Assert.Equal("0D-0A", BitConverter.ToString(packet.Data));
        }

        {
            var packet = deviceBus.Receive(100);
            Assert.Null(packet.FromAddress);
            Assert.Equal((byte?)42, packet.ToAddress);
            Assert.Equal(42, packet.DestinationAddress);
            Assert.Equal(0, packet.CommandCode);
            Assert.Equal("06", BitConverter.ToString(packet.Data));
        }
    }

    [Fact(DisplayName = "BearBus: Host Send And Receive (corrupted)")]
    public void HostSendAndReceiveErrors() {
        var stream = new MemoryStream();
        var hostBus = new BearBusHost(stream);

        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(0));
        stream.WriteByte(0x00); // bad byte
        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(42));

        stream = new MemoryStream(stream.ToArray());
        var deviceBus = new BearBusDevice(stream);

        {
            var packet = deviceBus.Receive(100);
            Assert.Null(packet.FromAddress);
            Assert.Equal((byte?)0, packet.ToAddress);
            Assert.Equal(0, packet.DestinationAddress);
            Assert.Equal(0, packet.CommandCode);
            Assert.Equal("06", BitConverter.ToString(packet.Data));
        }

        {
            var packet = deviceBus.Receive(100);
            Assert.Null(packet.FromAddress);
            Assert.Equal((byte?)42, packet.ToAddress);
            Assert.Equal(42, packet.DestinationAddress);
            Assert.Equal(0, packet.CommandCode);
            Assert.Equal("06", BitConverter.ToString(packet.Data));
        }
    }

    [Fact(DisplayName = "BearBus: Host Send And Receive (corrupted packet)")]
    public void HostSendAndReceiveErrorsPacket() {
        var stream = new MemoryStream();
        var hostBus = new BearBusHost(stream);

        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(0));
        stream.Write(new byte[] { 0xBB, 0x00, 0x00, 0x00, 0x00 }); // bad packet
        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(42));

        stream = new MemoryStream(stream.ToArray());
        var deviceBus = new BearBusDevice(stream);

        {
            var packet = deviceBus.Receive(100);
            Assert.Null(packet.FromAddress);
            Assert.Equal((byte?)0, packet.ToAddress);
            Assert.Equal(0, packet.DestinationAddress);
            Assert.Equal(0, packet.CommandCode);
            Assert.Equal("06", BitConverter.ToString(packet.Data));
        }

        {
            var packet = deviceBus.Receive(100);
            Assert.Null(packet.FromAddress);
            Assert.Equal((byte?)42, packet.ToAddress);
            Assert.Equal(42, packet.DestinationAddress);
            Assert.Equal(0, packet.CommandCode);
            Assert.Equal("06", BitConverter.ToString(packet.Data));
        }
    }

    [Fact(DisplayName = "BearBus: Host Send And Receive (corrupted, uff)")]
    public void HostSendAndReceiveErrorsUff() {
        var stream = new MemoryStream();
        var hostBus = new BearBusHost(stream);

        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(0));
        stream.WriteByte(0xBB); // bad byte
        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(42));

        stream = new MemoryStream(stream.ToArray());
        var deviceBus = new BearBusDevice(stream);

        {
            var packet = deviceBus.Receive(100);
            Assert.Null(packet.FromAddress);
            Assert.Equal((byte?)0, packet.ToAddress);
            Assert.Equal(0, packet.DestinationAddress);
            Assert.Equal(0, packet.CommandCode);
            Assert.Equal("06", BitConverter.ToString(packet.Data));
        }

        {
            var packet = deviceBus.Receive(100);
            Assert.Null(packet.FromAddress);
            Assert.Equal((byte?)42, packet.ToAddress);
            Assert.Equal(42, packet.DestinationAddress);
            Assert.Equal(0, packet.CommandCode);
            Assert.Equal("06", BitConverter.ToString(packet.Data));
        }
    }

    #endregion Stream

}
