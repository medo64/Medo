using Medo.Device;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace BearBusMonitor;

internal static class WorkBus {

    private static byte LastSource = 0;

    public static void Run(Medo.Device.BearBusMonitor bus) {
        Output.Line();
        Output.Header("Monitoring");
        Output.Line('D', "Generate duplicate (with Shift)");
        Output.Line('R', "Reboot (with Shift)");
        Output.Line('U', "Light (Shift for off)");

        Output.Line();
        Output.PacketHeader();

        while (true) {
            if (Console.KeyAvailable) {
                var key = Console.ReadKey(intercept: true);
                if (key.Key is ConsoleKey.Escape) {
                    return;
                } else if ((key.Key == ConsoleKey.D) && (key.Modifiers == ConsoleModifiers.Shift)) {  // Duplicate
                    if (LastSource != 0) {
                        var outPacket = BBSystemDevicePacket.CreateUnsolicitedUpdateReport(LastSource);
                        bus.Send(outPacket);
                        Output.Packet(outPacket);
                    }
                } else if (key.Key == ConsoleKey.E) {  // Edit
                    if (GetInput(out var commandCode, out var dataBytes)) {
                        var outPacket = BBCustomPacket.Create(LastSource, commandCode, dataBytes);
                        bus.Send(outPacket);
                        Output.Packet(outPacket);
                    } else {
                        Output.Error("Cannot parse");
                    }
                } else if ((key.Key == ConsoleKey.R) && (key.Modifiers == ConsoleModifiers.Shift)) {  // Reboot
                    if (LastSource != 0) {
                        var outPacket = BBSystemHostPacket.CreateRebootRequest(LastSource);
                        bus.Send(outPacket);
                        Output.Packet(outPacket);
                    }
                } else if (key.Key == ConsoleKey.U) {  // UID light
                    if (LastSource != 0) {
                        var shouldBlink = key.Modifiers != ConsoleModifiers.Shift;  // Shift turns it off
                        var outPacket = shouldBlink switch {
                            true => BBSetupHostPacket.CreateBlinkOnRequest(LastSource),
                            false => BBSetupHostPacket.CreateBlinkOffRequest(LastSource),
                        };
                        bus.Send(outPacket);
                        Output.Packet(outPacket);
                    }
                }
            }
            if (bus.TryReceive(out var packet)) {
                Output.Packet(packet);
                if (packet.FromAddress != null) { LastSource = packet.FromAddress.Value; }
            }
            Thread.Sleep(1);
        }
    }


    private static bool GetInput(out byte commandCode, out byte[] dataBytes) {
        Console.Write("Input: ");

        var line = Console.ReadLine();
        if (line == null) {
            commandCode = 0;
            dataBytes = Array.Empty<byte>();
            return false;
        }

        var sb = new StringBuilder();
        foreach (var ch in line) {
            if (ch is >= '0' and <= '9' or >= 'A' and <= 'F' or >= 'a' and <= 'f') {
                sb.Append(ch);
            }
        }
        if (sb.Length == 0) {
            commandCode = 0;
            dataBytes = Array.Empty<byte>();
            return false;
        } else if (sb.Length % 2 != 0) {
            sb.Insert(0, "0");
        }

        commandCode = byte.Parse(sb.ToString(0, 2), NumberStyles.HexNumber);

        var dataBuffer = new List<byte>();
        for (var i = 2; i < sb.Length; i += 2) {
            dataBuffer.Add(byte.Parse(sb.ToString(i, 2), NumberStyles.HexNumber));
        }
        dataBytes = dataBuffer.ToArray();

        return (commandCode is >= 1 and <= 60);
    }

}
