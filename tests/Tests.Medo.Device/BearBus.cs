using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Device;
using System.Threading.Tasks;

namespace Tests;

[TestClass]
public class BearBus_Tests {

    #region Packets

    [TestMethod]
    public void BearBus_CustomShort() {
        var packet = BBCustomPacket.Create(destinationAddress: 5,
                                        commandCode: 29,
                                        datum: 0x42);
        Assert.AreEqual(5, packet.DestinationAddress);
        Assert.IsTrue(packet.IsReplyRequested);
        Assert.AreEqual(29, packet.CommandCode);
        Assert.AreEqual("42", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-05-FD-42-5B", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply();
        Assert.AreEqual(5, packetReply.SourceAddress);
        Assert.IsFalse(packetReply.IsErrorReply);
        Assert.AreEqual(29, packetReply.CommandCode);
        Assert.AreEqual("", BitConverter.ToString(packetReply.Data));
        Assert.AreEqual("BB-05-1D-00-3A", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply();
        Assert.AreEqual(5, packetErrorReply.SourceAddress);
        Assert.IsTrue(packetErrorReply.IsErrorReply);
        Assert.AreEqual(29, packetErrorReply.CommandCode);
        Assert.AreEqual("", BitConverter.ToString(packetErrorReply.Data));
        Assert.AreEqual("BB-05-5D-00-AE", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [TestMethod]
    public void BearBus_CustomShortNoReply() {
        var packet = BBCustomPacket.Create(destinationAddress: 5,
                                        commandCode: 29,
                                        datum: 0x42,
                                        replyRequested: false);
        Assert.AreEqual(5, packet.DestinationAddress);
        Assert.IsFalse(packet.IsReplyRequested);
        Assert.AreEqual(29, packet.CommandCode);
        Assert.AreEqual("42", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-05-BD-42-CF", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply();
        Assert.AreEqual(5, packetReply.SourceAddress);
        Assert.IsFalse(packetReply.IsErrorReply);
        Assert.AreEqual(29, packetReply.CommandCode);
        Assert.AreEqual("", BitConverter.ToString(packetReply.Data));
        Assert.AreEqual("BB-05-1D-00-3A", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply();
        Assert.AreEqual(5, packetErrorReply.SourceAddress);
        Assert.IsTrue(packetErrorReply.IsErrorReply);
        Assert.AreEqual(29, packetErrorReply.CommandCode);
        Assert.AreEqual("", BitConverter.ToString(packetErrorReply.Data));
        Assert.AreEqual("BB-05-5D-00-AE", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [TestMethod]
    public void BearBus_CustomBasic() {
        var packet = BBCustomPacket.Create(42, 1, new byte[] { 1, 2, 3 });
        Assert.AreEqual(42, packet.DestinationAddress);
        Assert.IsTrue(packet.IsReplyRequested);
        Assert.AreEqual(1, packet.CommandCode);
        Assert.AreEqual("01-02-03", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-2A-C1-03-18-01-02-03-CF", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(new byte[] { 0x5A, 0x5B });
        Assert.AreEqual(42, packetReply.SourceAddress);
        Assert.IsFalse(packetReply.IsErrorReply);
        Assert.AreEqual(1, packetReply.CommandCode);
        Assert.AreEqual("5A-5B", BitConverter.ToString(packetReply.Data));
        Assert.AreEqual("BB-2A-01-02-A4-5A-5B-BD", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply(new byte[] { 0x6A, 0x6B });
        Assert.AreEqual(42, packetErrorReply.SourceAddress);
        Assert.IsTrue(packetErrorReply.IsErrorReply);
        Assert.AreEqual(1, packetErrorReply.CommandCode);
        Assert.AreEqual("6A-6B", BitConverter.ToString(packetErrorReply.Data));
        Assert.AreEqual("BB-2A-41-02-30-6A-6B-1A", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [TestMethod]
    public void BearBus_CustomBasicNoReply() {
        var packet = BBCustomPacket.Create(destinationAddress: 19,
                                        commandCode: 26,
                                        data: new byte[] { 0x42, 0x43, 0x44 },
                                        replyRequested: false);
        Assert.AreEqual(19, packet.DestinationAddress);
        Assert.IsFalse(packet.IsReplyRequested);
        Assert.AreEqual(26, packet.CommandCode);
        Assert.AreEqual("42-43-44", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-13-9A-03-49-42-43-44-4E", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(new byte[] { 0x5A, 0x5B });
        Assert.AreEqual(19, packetReply.SourceAddress);
        Assert.IsFalse(packetReply.IsErrorReply);
        Assert.AreEqual(26, packetReply.CommandCode);
        Assert.AreEqual("5A-5B", BitConverter.ToString(packetReply.Data));
        Assert.AreEqual("BB-13-1A-02-61-5A-5B-B7", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply(new byte[] { 0x6A, 0x6B });
        Assert.AreEqual(19, packetErrorReply.SourceAddress);
        Assert.IsTrue(packetErrorReply.IsErrorReply);
        Assert.AreEqual(26, packetErrorReply.CommandCode);
        Assert.AreEqual("6A-6B", BitConverter.ToString(packetErrorReply.Data));
        Assert.AreEqual("BB-13-5A-02-F5-6A-6B-10", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [TestMethod]
    public void BearBus_CustomBasicShortest() {
        var packet = BBCustomPacket.Create(42, 1, new byte[] { 1, 2 });
        Assert.AreEqual(42, packet.DestinationAddress);
        Assert.IsTrue(packet.IsReplyRequested);
        Assert.AreEqual(1, packet.CommandCode);
        Assert.AreEqual("01-02", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-2A-C1-02-37-01-02-92", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(new byte[] { 101, 102 });
        Assert.AreEqual(42, packetReply.SourceAddress);
        Assert.IsFalse(packetReply.IsErrorReply);
        Assert.AreEqual(1, packetReply.CommandCode);
        Assert.AreEqual("65-66", BitConverter.ToString(packetReply.Data));
        Assert.AreEqual("BB-2A-01-02-A4-65-66-8F", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply(new byte[] { 201, 202 });
        Assert.AreEqual(42, packetErrorReply.SourceAddress);
        Assert.IsTrue(packetErrorReply.IsErrorReply);
        Assert.AreEqual(1, packetErrorReply.CommandCode);
        Assert.AreEqual("C9-CA", BitConverter.ToString(packetErrorReply.Data));
        Assert.AreEqual("BB-2A-41-02-30-C9-CA-FC", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [TestMethod]
    public void BearBus_CustomBasicLongest() {
        var packet = BBCustomPacket.Create(42, 1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
        Assert.AreEqual(42, packet.DestinationAddress);
        Assert.IsTrue(packet.IsReplyRequested);
        Assert.AreEqual(1, packet.CommandCode);
        Assert.AreEqual("01-02-03-04-05-06-07-08-09-0A-0B-0C", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-2A-C1-0C-82-01-02-03-04-05-06-07-08-09-0A-0B-0C-0F", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(new byte[] { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112 });
        Assert.AreEqual(42, packetReply.SourceAddress);
        Assert.IsFalse(packetReply.IsErrorReply);
        Assert.AreEqual(1, packetReply.CommandCode);
        Assert.AreEqual("65-66-67-68-69-6A-6B-6C-6D-6E-6F-70", BitConverter.ToString(packetReply.Data));
        Assert.AreEqual("BB-2A-01-0C-11-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-22", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply(new byte[] { 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212 });
        Assert.AreEqual(42, packetErrorReply.SourceAddress);
        Assert.IsTrue(packetErrorReply.IsErrorReply);
        Assert.AreEqual(1, packetErrorReply.CommandCode);
        Assert.AreEqual("C9-CA-CB-CC-CD-CE-CF-D0-D1-D2-D3-D4", BitConverter.ToString(packetErrorReply.Data));
        Assert.AreEqual("BB-2A-41-0C-85-C9-CA-CB-CC-CD-CE-CF-D0-D1-D2-D3-D4-2B", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [TestMethod]
    public void BearBus_CustomLong() {
        var packet = BBCustomPacket.Create(destinationAddress: 1,
                                           commandCode: 1,
                                           data: new byte[] { 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E });
        Assert.AreEqual(1, packet.DestinationAddress);
        Assert.IsTrue(packet.IsReplyRequested);
        Assert.AreEqual(1, packet.CommandCode);
        Assert.AreEqual("42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-01-C1-8D-C3-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-05-4B", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(new byte[] { 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D });
        Assert.AreEqual(1, packetReply.SourceAddress);
        Assert.IsFalse(packetReply.IsErrorReply);
        Assert.AreEqual(1, packetReply.CommandCode);
        Assert.AreEqual("51-52-53-54-55-56-57-58-59-5A-5B-5C-5D", BitConverter.ToString(packetReply.Data));
        Assert.AreEqual("BB-01-01-8D-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-EC-9A", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply(new byte[] { 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D });
        Assert.AreEqual(1, packetErrorReply.SourceAddress);
        Assert.IsTrue(packetErrorReply.IsErrorReply);
        Assert.AreEqual(1, packetErrorReply.CommandCode);
        Assert.AreEqual("61-62-63-64-65-66-67-68-69-6A-6B-6C-6D", BitConverter.ToString(packetErrorReply.Data));
        Assert.AreEqual("BB-01-41-8D-C4-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-43-FB", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [TestMethod]
    public void BearBus_CustomLongNoReply() {
        var packet = BBCustomPacket.Create(destinationAddress: 1,
                                           commandCode: 1,
                                           data: new byte[] { 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E },
                                           replyRequested: false);
        Assert.AreEqual(1, packet.DestinationAddress);
        Assert.IsFalse(packet.IsReplyRequested);
        Assert.AreEqual(1, packet.CommandCode);
        Assert.AreEqual("42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-01-81-8D-57-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-20-A9", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(new byte[] { 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D });
        Assert.AreEqual(1, packetReply.SourceAddress);
        Assert.IsFalse(packetReply.IsErrorReply);
        Assert.AreEqual(1, packetReply.CommandCode);
        Assert.AreEqual("51-52-53-54-55-56-57-58-59-5A-5B-5C-5D", BitConverter.ToString(packetReply.Data));
        Assert.AreEqual("BB-01-01-8D-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-EC-9A", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply(new byte[] { 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D });
        Assert.AreEqual(1, packetErrorReply.SourceAddress);
        Assert.IsTrue(packetErrorReply.IsErrorReply);
        Assert.AreEqual(1, packetErrorReply.CommandCode);
        Assert.AreEqual("61-62-63-64-65-66-67-68-69-6A-6B-6C-6D", BitConverter.ToString(packetErrorReply.Data));
        Assert.AreEqual("BB-01-41-8D-C4-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-43-FB", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [TestMethod]
    public void BearBus_CustomLongLongest() {
        var dataBytes = new byte[127];
        for (var i = 0; i < dataBytes.Length; i++) { dataBytes[i] = (byte)(i + 1); }
        var packet = BBCustomPacket.Create(destinationAddress: 1,
                                           commandCode: 1,
                                           data: dataBytes);
        Assert.AreEqual(1, packet.DestinationAddress);
        Assert.IsTrue(packet.IsReplyRequested);
        Assert.AreEqual(1, packet.CommandCode);
        Assert.AreEqual("01-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-01-C1-FF-A6-01-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F-53-E1", BitConverter.ToString(packet.ToBytes()));

        dataBytes[0] = 101;
        var packetReply = packet.GetReply(dataBytes);
        Assert.AreEqual(1, packetReply.SourceAddress);
        Assert.IsFalse(packetReply.IsErrorReply);
        Assert.AreEqual(1, packetReply.CommandCode);
        Assert.AreEqual("65-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F", BitConverter.ToString(packetReply.Data));
        Assert.AreEqual("BB-01-01-FF-35-65-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F-1E-85", BitConverter.ToString(packetReply.ToBytes()));

        dataBytes[0] = 201;
        var packetErrorReply = packet.GetErrorReply(dataBytes);
        Assert.AreEqual(1, packetErrorReply.SourceAddress);
        Assert.IsTrue(packetErrorReply.IsErrorReply);
        Assert.AreEqual(1, packetErrorReply.CommandCode);
        Assert.AreEqual("C9-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F", BitConverter.ToString(packetErrorReply.Data));
        Assert.AreEqual("BB-01-41-FF-A1-C9-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F-A1-F2", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [TestMethod]
    public void BearBus_CustomLongTooLong() {
        var dataBytes = new byte[128];
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var packet = BBCustomPacket.Create(destinationAddress: 1,
                                               commandCode: 1,
                                               data: dataBytes);
        });
    }

    #endregion Packets

    #region System

    [TestMethod]
    public void BearBus_UnsoliticedUpdate() {
        var packet = BBSystemDevicePacket.CreateUnsolicitedUpdateReport(200);
        Assert.AreEqual(200, packet.SourceAddress);
        Assert.IsFalse(packet.IsErrorReply);
        Assert.AreEqual("", BitConverter.ToString(packet.Data));
        Assert.AreEqual(0x00, packet.ErrorCode);
        Assert.AreEqual("BB-C8-00-00-DC", BitConverter.ToString(packet.ToBytes()));
    }

    [TestMethod]
    public void BearBus_UnsoliticedUpdateWithError() {
        var packet = BBSystemDevicePacket.CreateUnsolicitedUpdateReport(201, 1);
        Assert.AreEqual(201, packet.SourceAddress);
        Assert.IsFalse(packet.IsErrorReply);
        Assert.AreEqual("01", BitConverter.ToString(packet.Data));
        Assert.AreEqual(0x01, packet.ErrorCode);
        Assert.AreEqual("BB-C9-20-01-B7", BitConverter.ToString(packet.ToBytes()));
    }

    [TestMethod]
    public void BearBus_SystemDuplicateAddressReport() {
        var packet = BBSystemDevicePacket.CreateDuplicateAddressReport(76);
        Assert.AreEqual(76, packet.SourceAddress);
        Assert.IsTrue(packet.IsErrorReply);
        Assert.AreEqual(0x00, packet.CommandCode);
        Assert.AreEqual("", BitConverter.ToString(packet.Data));
        Assert.AreEqual(0x00, packet.ErrorCode);
        Assert.AreEqual("BB-4C-40-00-BD", BitConverter.ToString(packet.ToBytes()));
    }

    [TestMethod]
    public void BearBus_SystemUpdateRequest() {
        var packet = BBSystemHostPacket.CreateUpdateRequest(254);
        Assert.AreEqual(254, packet.DestinationAddress);
        Assert.AreEqual(BBDeviceAction.Ping, packet.Action);
        Assert.AreEqual("", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-FE-80-00-F0", BitConverter.ToString(packet.ToBytes()));
    }

    [TestMethod]
    public void BearBus_SystemUpdateRewsponse() {
        var packet = BBSystemDevicePacket.CreateUnsolicitedUpdateReport(254, 0);
        Assert.AreEqual(254, packet.SourceAddress);
        Assert.IsFalse(packet.IsErrorReply);
        Assert.AreEqual("00", BitConverter.ToString(packet.Data));
        Assert.AreEqual(0x00, packet.ErrorCode);
        Assert.AreEqual("BB-FE-20-00-BD", BitConverter.ToString(packet.ToBytes()));
    }

    [TestMethod]
    public void BearBus_SystemReboot() {
        var packet = BBSystemHostPacket.CreateRebootRequest(32);
        Assert.AreEqual(32, packet.DestinationAddress);
        Assert.AreEqual(BBDeviceAction.Reboot, packet.Action);
        Assert.AreEqual("06", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-20-A0-06-D0", BitConverter.ToString(packet.ToBytes()));
    }

    [TestMethod]
    public void BearBus_SystemRebootAll() {
        var packet = BBSystemHostPacket.CreateRebootRequest(0);
        Assert.AreEqual(0, packet.DestinationAddress);
        Assert.AreEqual(BBDeviceAction.Reboot, packet.Action);
        Assert.AreEqual("06", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-00-A0-06-3F", BitConverter.ToString(packet.ToBytes()));
    }

    [TestMethod]
    public void BearBus_SystemBlinkOn() {
        var packet = BBSystemHostPacket.CreateBlinkOnRequest(129);
        Assert.AreEqual(129, packet.DestinationAddress);
        Assert.AreEqual(BBDeviceAction.BlinkOn, packet.Action);
        Assert.AreEqual("08", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-81-A0-08-49", BitConverter.ToString(packet.ToBytes()));
    }

    [TestMethod]
    public void BearBus_SystemBlinkOff() {
        var packet = BBSystemHostPacket.CreateBlinkOffRequest(129);
        Assert.AreEqual(129, packet.DestinationAddress);
        Assert.AreEqual(BBDeviceAction.BlinkOff, packet.Action);
        Assert.AreEqual("09", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-81-A0-09-66", BitConverter.ToString(packet.ToBytes()));
    }

    [TestMethod]
    public void BearBus_SystemEnterFirmwareUpgrade() {
        var packet = BBSystemHostPacket.CreateFirmwareUpgradeRequest(222);
        Assert.AreEqual(222, packet.DestinationAddress);
        Assert.AreEqual(BBDeviceAction.FirmwareUpgrade, packet.Action);
        Assert.AreEqual("FF-48-65-6C-6C-6F-20-57-6F-72-6C-64", BitConverter.ToString(packet.Data));
        Assert.AreEqual("BB-DE-80-0C-F4-FF-48-65-6C-6C-6F-20-57-6F-72-6C-64-9F", BitConverter.ToString(packet.ToBytes()));
    }

    #endregion System

    #region Setup


    #endregion Setup


    #region Stream

    [TestMethod]
    public void BearBus_HostSendAndReceive() {
        var stream = new MemoryStream();

        var hostBus = new BearBusHost(stream);
        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(0));
        Assert.AreEqual("BB-00-A0-06-3F", BitConverter.ToString(stream.ToArray()));

        stream = new MemoryStream(stream.ToArray());

        var deviceBus = new BearBusDevice(stream);
        var packet = deviceBus.Receive(100);
        Assert.IsNull(packet.FromAddress);
        Assert.AreEqual((byte?)0, packet.ToAddress);
        Assert.AreEqual(0, packet.DestinationAddress);
        Assert.AreEqual(0, packet.CommandCode);
        Assert.AreEqual("06", BitConverter.ToString(packet.Data));

        Assert.ThrowsException<TimeoutException>(() => {
            deviceBus.Receive(100);
        });
    }

    [TestMethod]
    public void BearBus_HostSendAndReceiveNoData() {
        var stream = new MemoryStream(new byte[] { 0xBB, 0xC8, 0x00, 0x00, 0xDC });

        var host = new BearBusHost(stream);
        var packet = host.Receive(100);
        Assert.IsNull(packet.ToAddress);
        Assert.AreEqual((byte?)200, packet.FromAddress);
        Assert.AreEqual(200, packet.SourceAddress);
        Assert.AreEqual(0, packet.CommandCode);
        Assert.AreEqual("", BitConverter.ToString(packet.Data));

        Assert.ThrowsException<TimeoutException>(() => {
            host.Receive(100);
        });
    }

    [TestMethod]
    public void BearBus_HostSendAndReceive127Crc8DataBytes() {
        var stream = new MemoryStream(new byte[] { 0xBB, 0x01, 0xC1, 0x7F, 0x45, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F, 0xC8 });

        var deviceBus = new BearBusDevice(stream);
        var packet = deviceBus.Receive(100);
        Assert.IsNull(packet.FromAddress);
        Assert.AreEqual((byte?)1, packet.ToAddress);
        Assert.AreEqual(1, packet.DestinationAddress);
        Assert.AreEqual(1, packet.CommandCode);
        Assert.AreEqual("01-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F", BitConverter.ToString(packet.Data));

        Assert.ThrowsException<TimeoutException>(() => {
            deviceBus.Receive(100);
        });
    }

    [TestMethod]
    public void BearBus_HostSendAndReceive127Crc16DataBytes() {
        var stream = new MemoryStream(new byte[] { 0xBB, 0x01, 0xC1, 0xFF, 0xA6, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F, 0x53, 0xE1 });

        var deviceBus = new BearBusDevice(stream);
        var packet = deviceBus.Receive(100);
        Assert.IsNull(packet.FromAddress);
        Assert.AreEqual((byte?)1, packet.ToAddress);
        Assert.AreEqual(1, packet.DestinationAddress);
        Assert.AreEqual(1, packet.CommandCode);
        Assert.AreEqual("01-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10-11-12-13-14-15-16-17-18-19-1A-1B-1C-1D-1E-1F-20-21-22-23-24-25-26-27-28-29-2A-2B-2C-2D-2E-2F-30-31-32-33-34-35-36-37-38-39-3A-3B-3C-3D-3E-3F-40-41-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-50-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-5F-60-61-62-63-64-65-66-67-68-69-6A-6B-6C-6D-6E-6F-70-71-72-73-74-75-76-77-78-79-7A-7B-7C-7D-7E-7F", BitConverter.ToString(packet.Data));

        Assert.ThrowsException<TimeoutException>(() => {
            deviceBus.Receive(100);
        });
    }

    [TestMethod]
    public async Task BearBus_HostSendAndReceiveAsync() {
        var stream = new MemoryStream();

        var hostBus = new BearBusHost(stream);
        await hostBus.SendAsync(BBSystemHostPacket.CreateRebootRequest(0));
        Assert.AreEqual("BB-00-A0-06-3F", BitConverter.ToString(stream.ToArray()));

        stream = new MemoryStream(stream.ToArray());

        var deviceBus = new BearBusDevice(stream);
        var packet = await deviceBus.ReceiveAsync(100);
        Assert.IsNull(packet.FromAddress);
        Assert.AreEqual((byte?)0, packet.ToAddress);
        Assert.AreEqual(0, packet.DestinationAddress);
        Assert.AreEqual(0, packet.CommandCode);
        Assert.AreEqual("06", BitConverter.ToString(packet.Data));

        await Assert.ThrowsExceptionAsync<TimeoutException>(async () => {
            await deviceBus.ReceiveAsync(100);
        });
    }

    [TestMethod]
    public void BearBus_HostSendAndReceiveMulti() {
        var stream = new MemoryStream();
        var hostBus = new BearBusHost(stream);

        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(42));
        hostBus.Send(BBCustomPacket.Create(42, 1, new byte[] { 0x0D, 0x0A }));
        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(42));

        stream = new MemoryStream(stream.ToArray());
        var deviceBus = new BearBusDevice(stream);

        {
            var packet = deviceBus.Receive(100);
            Assert.IsNull(packet.FromAddress);
            Assert.AreEqual((byte?)42, packet.ToAddress);
            Assert.AreEqual(42, packet.DestinationAddress);
            Assert.AreEqual(0, packet.CommandCode);
            Assert.AreEqual("06", BitConverter.ToString(packet.Data));
        }

        {
            var packet = deviceBus.Receive(100);
            Assert.IsNull(packet.FromAddress);
            Assert.AreEqual((byte?)42, packet.ToAddress);
            Assert.AreEqual(42, packet.DestinationAddress);
            Assert.AreEqual(1, packet.CommandCode);
            Assert.AreEqual("0D-0A", BitConverter.ToString(packet.Data));
        }

        {
            var packet = deviceBus.Receive(100);
            Assert.IsNull(packet.FromAddress);
            Assert.AreEqual((byte?)42, packet.ToAddress);
            Assert.AreEqual(42, packet.DestinationAddress);
            Assert.AreEqual(0, packet.CommandCode);
            Assert.AreEqual("06", BitConverter.ToString(packet.Data));
        }
    }

    [TestMethod]
    public void BearBus_HostSendAndReceiveErrors() {
        var stream = new MemoryStream();
        var hostBus = new BearBusHost(stream);

        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(0));
        stream.WriteByte(0x00); // bad byte
        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(42));

        stream = new MemoryStream(stream.ToArray());
        var deviceBus = new BearBusDevice(stream);

        {
            var packet = deviceBus.Receive(100);
            Assert.IsNull(packet.FromAddress);
            Assert.AreEqual((byte?)0, packet.ToAddress);
            Assert.AreEqual(0, packet.DestinationAddress);
            Assert.AreEqual(0, packet.CommandCode);
            Assert.AreEqual("06", BitConverter.ToString(packet.Data));
        }

        {
            var packet = deviceBus.Receive(100);
            Assert.IsNull(packet.FromAddress);
            Assert.AreEqual((byte?)42, packet.ToAddress);
            Assert.AreEqual(42, packet.DestinationAddress);
            Assert.AreEqual(0, packet.CommandCode);
            Assert.AreEqual("06", BitConverter.ToString(packet.Data));
        }
    }

    [TestMethod]
    public void BearBus_HostSendAndReceiveErrorsPacket() {
        var stream = new MemoryStream();
        var hostBus = new BearBusHost(stream);

        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(0));
        stream.Write(new byte[] { 0xBB, 0x00, 0x00, 0x00, 0x00 }); // bad packet
        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(42));

        stream = new MemoryStream(stream.ToArray());
        var deviceBus = new BearBusDevice(stream);

        {
            var packet = deviceBus.Receive(100);
            Assert.IsNull(packet.FromAddress);
            Assert.AreEqual((byte?)0, packet.ToAddress);
            Assert.AreEqual(0, packet.DestinationAddress);
            Assert.AreEqual(0, packet.CommandCode);
            Assert.AreEqual("06", BitConverter.ToString(packet.Data));
        }

        {
            var packet = deviceBus.Receive(100);
            Assert.IsNull(packet.FromAddress);
            Assert.AreEqual((byte?)42, packet.ToAddress);
            Assert.AreEqual(42, packet.DestinationAddress);
            Assert.AreEqual(0, packet.CommandCode);
            Assert.AreEqual("06", BitConverter.ToString(packet.Data));
        }
    }

    [TestMethod]
    public void BearBus_HostSendAndReceiveErrorsUff() {
        var stream = new MemoryStream();
        var hostBus = new BearBusHost(stream);

        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(0));
        stream.WriteByte(0xBB); // bad byte
        hostBus.Send(BBSystemHostPacket.CreateRebootRequest(42));

        stream = new MemoryStream(stream.ToArray());
        var deviceBus = new BearBusDevice(stream);

        {
            var packet = deviceBus.Receive(100);
            Assert.IsNull(packet.FromAddress);
            Assert.AreEqual((byte?)0, packet.ToAddress);
            Assert.AreEqual(0, packet.DestinationAddress);
            Assert.AreEqual(0, packet.CommandCode);
            Assert.AreEqual("06", BitConverter.ToString(packet.Data));
        }

        {
            var packet = deviceBus.Receive(100);
            Assert.IsNull(packet.FromAddress);
            Assert.AreEqual((byte?)42, packet.ToAddress);
            Assert.AreEqual(42, packet.DestinationAddress);
            Assert.AreEqual(0, packet.CommandCode);
            Assert.AreEqual("06", BitConverter.ToString(packet.Data));
        }
    }

    #endregion Stream

}
