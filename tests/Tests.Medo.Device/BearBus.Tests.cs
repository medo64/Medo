using System;
using Xunit;
using Medo.Device;

namespace Tests.Medo.Device;

public class BearBusTests {

    #region Packets

    [Fact(DisplayName = "BearBus: Custom Short Packet")]
    public void CustomShort() {
        var packet = BBCustomPacket.Create(destinationAddress: 5,
                                        commandCode: 29,
                                        datum: 0x42);
        Assert.Equal(5, packet.DestinationAddress);
        Assert.False(packet.ReplyRequested);
        Assert.Equal(29, packet.CommandCode);
        Assert.Equal("42", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-85-5D-42-DB", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply();
        Assert.Equal(5, packetReply.SourceAddress);
        Assert.False(packetReply.IsError);
        Assert.Equal(29, packetReply.CommandCode);
        Assert.Equal("", BitConverter.ToString(packetReply.Data));
        Assert.Equal("BB-05-1D-00-3A", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply();
        Assert.Equal(5, packetErrorReply.SourceAddress);
        Assert.True(packetErrorReply.IsError);
        Assert.Equal(29, packetErrorReply.CommandCode);
        Assert.Equal("", BitConverter.ToString(packetErrorReply.Data));
        Assert.Equal("BB-05-9D-00-3D", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Custom Basic Packet")]
    public void CustomBasic() {
        var packet = BBCustomPacket.Create(destinationAddress: 19,
                                        commandCode: 26,
                                        data: new byte[] { 0x42, 0x43, 0x44 });
        Assert.Equal(19, packet.DestinationAddress);
        Assert.False(packet.ReplyRequested);
        Assert.Equal(26, packet.CommandCode);
        Assert.Equal("42-43-44", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-93-1A-03-83-42-43-44-06", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(new byte[] { 0x5A, 0x5B });
        Assert.Equal(19, packetReply.SourceAddress);
        Assert.False(packetReply.IsError);
        Assert.Equal(26, packetReply.CommandCode);
        Assert.Equal("5A-5B", BitConverter.ToString(packetReply.Data));
        Assert.Equal("BB-13-1A-02-61-5A-5B-B7", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply(new byte[] { 0x5A, 0x5B });
        Assert.Equal(19, packetErrorReply.SourceAddress);
        Assert.True(packetErrorReply.IsError);
        Assert.Equal(26, packetErrorReply.CommandCode);
        Assert.Equal("5A-5B", BitConverter.ToString(packetErrorReply.Data));
        Assert.Equal("BB-13-9A-02-66-5A-5B-9D", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Custom Long Packet")]
    public void CustomLong() {
        var packet = BBCustomPacket.Create(destinationAddress: 1,
                                        commandCode: 1,
                                        data: new byte[] { 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F });
        Assert.Equal(1, packet.DestinationAddress);
        Assert.False(packet.ReplyRequested);
        Assert.Equal(1, packet.CommandCode);
        Assert.Equal("42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-81-01-0E-0F-42-43-44-45-46-47-48-49-4A-4B-4C-4D-4E-4F-C2-B2", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(new byte[] { 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E });
        Assert.Equal(1, packetReply.SourceAddress);
        Assert.False(packetReply.IsError);
        Assert.Equal(1, packetReply.CommandCode);
        Assert.Equal("51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E", BitConverter.ToString(packetReply.Data));
        Assert.Equal("BB-01-01-0E-C2-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-80-EA", BitConverter.ToString(packetReply.ToBytes()));

        var packetErrorReply = packet.GetErrorReply(new byte[] { 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E });
        Assert.Equal(1, packetErrorReply.SourceAddress);
        Assert.True(packetErrorReply.IsError);
        Assert.Equal(1, packetErrorReply.CommandCode);
        Assert.Equal("51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E", BitConverter.ToString(packetErrorReply.Data));
        Assert.Equal("BB-01-81-0E-C5-51-52-53-54-55-56-57-58-59-5A-5B-5C-5D-5E-D4-DF", BitConverter.ToString(packetErrorReply.ToBytes()));
    }

    #endregion Packets

    #region System

    [Fact(DisplayName = "BearBus: Device Reboot")]
    public void SystemReboot() {
        var packet = BBSystemHostPacket.CreateReboot(32);
        Assert.Equal(32, packet.DestinationAddress);
        Assert.Equal(6, packet.Action);
        Assert.Equal("06", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-A0-40-06-C4", BitConverter.ToString(packet.ToBytes()));
    }


    [Fact(DisplayName = "BearBus: Broadcasted Reboot")]
    public void SystemRebootAll() {
        var packet = BBSystemHostPacket.CreateReboot(0);
        Assert.Equal(0, packet.DestinationAddress);
        Assert.Equal(6, packet.Action);
        Assert.Equal("06", BitConverter.ToString(packet.Data));
        Assert.Equal("BB-80-40-06-2B", BitConverter.ToString(packet.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Unsolicited Power On Report")]
    public void SystemPowerOnReport() {
        var packet = BBSystemDevicePacket.CreateStatusUpdate(34, blinking: false, BBDeviceMode.Normal, 0);
        Assert.Equal(34, packet.SourceAddress);
        Assert.False(packet.IsError);
        Assert.Equal("00", BitConverter.ToString(packet.Data));
        Assert.False(packet.Blink);
        Assert.Equal(BBDeviceMode.Normal, packet.Mode);
        Assert.Equal(0x00, packet.ErrorCode);
        Assert.Equal("BB-22-40-00-F7", BitConverter.ToString(packet.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Unsolicited Duplicate Address Report")]
    public void SystemDuplicateAddressReport() {
        var packet = BBSystemDevicePacket.CreateDuplicateAddressReport(76, blinking: false, BBDeviceMode.Normal, 0);
        Assert.Equal(76, packet.SourceAddress);
        Assert.True(packet.IsError);
        Assert.Equal(0x00, packet.CommandCode);
        Assert.Equal("00", BitConverter.ToString(packet.Data));
        Assert.False(packet.Blink);
        Assert.Equal(BBDeviceMode.Normal, packet.Mode);
        Assert.Equal(0x00, packet.ErrorCode);
        Assert.Equal("BB-4C-C0-00-BA", BitConverter.ToString(packet.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Unsolicited Address Report After Change")]
    public void SystemAddressChange() {
        var packet = BBSystemDevicePacket.CreateStatusUpdate(77, blinking: true, BBDeviceMode.Normal, 0);
        Assert.Equal(77, packet.SourceAddress);
        Assert.False(packet.IsError);
        Assert.Equal(0x00, packet.CommandCode);
        Assert.Equal("80", BitConverter.ToString(packet.Data));
        Assert.True(packet.Blink);
        Assert.Equal(BBDeviceMode.Normal, packet.Mode);
        Assert.Equal(0x00, packet.ErrorCode);
        Assert.Equal("BB-4D-40-80-50", BitConverter.ToString(packet.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Unsolicited Status")]
    public void SystemStatus() {
        var packet = BBSystemDevicePacket.CreateStatusUpdate(47, false, BBDeviceMode.Config, 2);
        Assert.Equal(47, packet.SourceAddress);
        Assert.False(packet.IsError);
        Assert.Equal(0x00, packet.CommandCode);
        Assert.Equal("22", BitConverter.ToString(packet.Data));
        Assert.False(packet.Blink);
        Assert.Equal(BBDeviceMode.Config, packet.Mode);
        Assert.Equal(2, packet.ErrorCode);
        Assert.Equal("BB-2F-40-22-9C", BitConverter.ToString(packet.ToBytes()));
    }

    #endregion System

    #region Ping Packets

    [Fact(DisplayName = "BearBus: Ping")]
    public void ExamplePingRequest() {
        var packet = BBPingPacket.Create(15);
        Assert.Equal(15, packet.DestinationAddress);
        Assert.True(packet.ReplyRequested);
        Assert.Equal(0x3D, packet.CommandCode);
        var packetBytes = packet.ToBytes(); packetBytes[3] = 0x00; packetBytes[4] = 0x00;  // because we have random numbers here
        Assert.Equal("BB-8F-FD-00-00", BitConverter.ToString(packetBytes));

        var replyPacket = packet.GetReply();
        Assert.Equal(15, replyPacket.SourceAddress);
        Assert.False(replyPacket.ErrorReply);
        Assert.Equal(0x3D, replyPacket.CommandCode);
        Assert.Equal(packet.Data[0], replyPacket.Data[0]);  // same data
        var replyPacketBytes = replyPacket.ToBytes(); replyPacketBytes[3] = 0x00; replyPacketBytes[4] = 0x00;  // because we have random numbers here
        Assert.Equal("BB-0F-7D-00-00", BitConverter.ToString(replyPacketBytes));
    }
    #endregion Ping Packets

    #region Status Packets

    [Fact(DisplayName = "BearBus: Status Check")]
    public void ExampleStatusCheck() {
        var packet = BBStatusPacket.New(47);
        Assert.Equal(47, packet.DestinationAddress);
        Assert.True(packet.ReplyRequested);
        Assert.Equal(0x3E, packet.CommandCode);
        Assert.Equal("", BitConverter.ToString(packet.Data));
        Assert.Null(packet.NewBlink);
        Assert.Null(packet.NewMode);
        Assert.Equal("BB-AF-BE-00-2D", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(blinking: false, BBDeviceMode.Normal, 0x00);
        Assert.Equal(47, packetReply.SourceAddress);
        Assert.False(packetReply.ErrorReply);
        Assert.Equal(0x3E, packetReply.CommandCode);
        Assert.Equal("00", BitConverter.ToString(packetReply.Data));
        Assert.False(packetReply.Blink);
        Assert.Equal(BBDeviceMode.Normal, packetReply.Mode);
        Assert.Equal(0, packetReply.ErrorCode);
        Assert.Equal("BB-2F-7E-00-73", BitConverter.ToString(packetReply.ToBytes()));

        var packetReply2 = packet.GetReply(blinking: false, BBDeviceMode.Normal, 0x06);
        Assert.Equal(47, packetReply2.SourceAddress);
        Assert.False(packetReply2.ErrorReply);
        Assert.Equal(0x3E, packetReply2.CommandCode);
        Assert.Equal("00", BitConverter.ToString(packetReply.Data));
        Assert.False(packetReply2.Blink);
        Assert.Equal(BBDeviceMode.Normal, packetReply2.Mode);
        Assert.Equal(6, packetReply2.ErrorCode);
        Assert.Equal("BB-2F-7E-06-91", BitConverter.ToString(packetReply2.ToBytes()));

        var packetReplyError = packet.GetErrorReply(blinking: false, BBDeviceMode.Normal, 0x00);
        Assert.Equal(47, packetReplyError.SourceAddress);
        Assert.True(packetReplyError.ErrorReply);
        Assert.Equal(0x3E, packetReplyError.CommandCode);
        Assert.Equal("00", BitConverter.ToString(packetReplyError.Data));
        Assert.False(packetReplyError.Blink);
        Assert.Equal(BBDeviceMode.Normal, packetReplyError.Mode);
        Assert.Equal(0, packetReplyError.ErrorCode);
        Assert.Equal("BB-2F-FE-00-74", BitConverter.ToString(packetReplyError.ToBytes()));
    }


    [Fact(DisplayName = "BearBus: Change Blink")]
    public void ExampleStatusChangeBlink() {
        var packet = BBStatusPacket.New(47, newBlink: true);
        Assert.Equal(47, packet.DestinationAddress);
        Assert.True(packet.ReplyRequested);
        Assert.Equal(0x3E, packet.CommandCode);
        Assert.Equal("90", BitConverter.ToString(packet.Data));
        Assert.True(packet.NewBlink);
        Assert.Null(packet.NewMode);
        Assert.Equal("BB-AF-FE-90-F4", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(blinking: true, BBDeviceMode.Normal, 0x00);
        Assert.Equal(47, packetReply.SourceAddress);
        Assert.False(packetReply.ErrorReply);
        Assert.Equal(0x3E, packetReply.CommandCode);
        Assert.Equal("80", BitConverter.ToString(packetReply.Data));
        Assert.True(packetReply.Blink);
        Assert.Equal(BBDeviceMode.Normal, packetReply.Mode);
        Assert.Equal(0, packetReply.ErrorCode);
        Assert.Equal("BB-2F-7E-80-90", BitConverter.ToString(packetReply.ToBytes()));

        var packetReplyError = packet.GetErrorReply(blinking: false, BBDeviceMode.Normal, 0x00);
        Assert.Equal(47, packetReplyError.SourceAddress);
        Assert.True(packetReplyError.ErrorReply);
        Assert.Equal(0x3E, packetReplyError.CommandCode);
        Assert.Equal("00", BitConverter.ToString(packetReplyError.Data));
        Assert.False(packetReplyError.Blink);
        Assert.Equal(BBDeviceMode.Normal, packetReplyError.Mode);
        Assert.Equal(0, packetReplyError.ErrorCode);
        Assert.Equal("BB-2F-FE-00-74", BitConverter.ToString(packetReplyError.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Change Mode")]
    public void ExampleStatusChangeMode() {
        var packet = BBStatusPacket.New(47, BBDeviceMode.Config);
        Assert.Equal(47, packet.DestinationAddress);
        Assert.True(packet.ReplyRequested);
        Assert.Equal(0x3E, packet.CommandCode);
        Assert.Equal("28", BitConverter.ToString(packet.Data));
        Assert.Null(packet.NewBlink);
        Assert.Equal(BBDeviceMode.Config, packet.NewMode);
        Assert.Equal("BB-AF-FE-28-9D", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(blinking: false, BBDeviceMode.Config, 0x00);
        Assert.Equal(47, packetReply.SourceAddress);
        Assert.False(packetReply.ErrorReply);
        Assert.Equal(0x3E, packetReply.CommandCode);
        Assert.Equal("20", BitConverter.ToString(packetReply.Data));
        Assert.False(packetReply.Blink);
        Assert.Equal(BBDeviceMode.Config, packetReply.Mode);
        Assert.Equal(0, packetReply.ErrorCode);
        Assert.Equal("BB-2F-7E-20-00", BitConverter.ToString(packetReply.ToBytes()));

        var packetReplyError = packet.GetErrorReply(blinking: false, BBDeviceMode.Normal, 0x00);
        Assert.Equal(47, packetReplyError.SourceAddress);
        Assert.True(packetReplyError.ErrorReply);
        Assert.Equal(0x3E, packetReplyError.CommandCode);
        Assert.Equal("00", BitConverter.ToString(packetReplyError.Data));
        Assert.False(packetReplyError.Blink);
        Assert.Equal(BBDeviceMode.Normal, packetReplyError.Mode);
        Assert.Equal(0, packetReplyError.ErrorCode);
        Assert.Equal("BB-2F-FE-00-74", BitConverter.ToString(packetReplyError.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Change Blink and Mode")]
    public void ExampleStatusChangeBlinkAndMode() {
        var packet = BBStatusPacket.New(47, newBlink: false, BBDeviceMode.Test);
        Assert.Equal(47, packet.DestinationAddress);
        Assert.True(packet.ReplyRequested);
        Assert.Equal(0x3E, packet.CommandCode);
        Assert.Equal("58", BitConverter.ToString(packet.Data));
        Assert.False(packet.NewBlink);
        Assert.Equal(BBDeviceMode.Test, packet.NewMode);
        Assert.Equal("BB-AF-FE-58-A6", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply(blinking: false, BBDeviceMode.Test, 0x00);
        Assert.Equal(47, packetReply.SourceAddress);
        Assert.False(packetReply.ErrorReply);
        Assert.Equal(0x3E, packetReply.CommandCode);
        Assert.Equal("40", BitConverter.ToString(packetReply.Data));
        Assert.False(packetReply.Blink);
        Assert.Equal(BBDeviceMode.Test, packetReply.Mode);
        Assert.Equal(0, packetReply.ErrorCode);
        Assert.Equal("BB-2F-7E-40-95", BitConverter.ToString(packetReply.ToBytes()));

        var packetReplyError = packet.GetErrorReply(blinking: true, BBDeviceMode.Program, 0x00);
        Assert.Equal(47, packetReplyError.SourceAddress);
        Assert.True(packetReplyError.ErrorReply);
        Assert.Equal(0x3E, packetReplyError.CommandCode);
        Assert.Equal("E0", BitConverter.ToString(packetReplyError.Data));
        Assert.True(packetReplyError.Blink);
        Assert.Equal(BBDeviceMode.Program, packetReplyError.Mode);
        Assert.Equal(0, packetReplyError.ErrorCode);
        Assert.Equal("BB-2F-FE-E0-02", BitConverter.ToString(packetReplyError.ToBytes()));
    }

    #endregion Status Packets

    #region Address Packets

    [Fact(DisplayName = "BearBus: Address Setup")]
    public void ExampleAddressSetup() {
        var packet = BBAddressPacket.Create(0, newAddress: 77, replyRequested: false);
        Assert.Equal(0, packet.DestinationAddress);
        Assert.False(packet.ReplyRequested);
        Assert.Equal(0x3F, packet.CommandCode);
        Assert.Equal("4D", BitConverter.ToString(packet.Data));
        Assert.Equal(77, packet.NewAddress);
        Assert.Equal("BB-80-7F-4D-C0", BitConverter.ToString(packet.ToBytes()));
    }

    [Fact(DisplayName = "BearBus: Address Change")]
    public void ExampleAddressChange() {
        var packet = BBAddressPacket.Create(3, newAddress: 77);
        Assert.Equal(3, packet.DestinationAddress);
        Assert.True(packet.ReplyRequested);
        Assert.Equal(0x3F, packet.CommandCode);
        Assert.Equal("4D", BitConverter.ToString(packet.Data));
        Assert.Equal(77, packet.NewAddress);
        Assert.Equal("BB-83-FF-4D-D5", BitConverter.ToString(packet.ToBytes()));

        var packetReply = packet.GetReply();
        Assert.Equal(3, packetReply.SourceAddress);
        Assert.False(packetReply.ErrorReply);
        Assert.Equal(0x3F, packetReply.CommandCode);
        Assert.Equal("4D", BitConverter.ToString(packetReply.Data));
        Assert.Equal(77, packetReply.NewAddress);
        Assert.Equal("BB-03-7F-4D-1F", BitConverter.ToString(packetReply.ToBytes()));

        var packetReplyError = packet.GetErrorReply();
        Assert.Equal(3, packetReplyError.SourceAddress);
        Assert.True(packetReplyError.ErrorReply);
        Assert.Equal(0x3F, packetReplyError.CommandCode);
        Assert.Equal("4D", BitConverter.ToString(packetReplyError.Data));
        Assert.Equal(77, packetReplyError.NewAddress);
        Assert.Equal("BB-03-FF-4D-18", BitConverter.ToString(packetReplyError.ToBytes()));
    }

    #endregion Address Packets

}