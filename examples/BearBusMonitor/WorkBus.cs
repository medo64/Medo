using Medo.Device;
using System;
using System.Threading;

namespace BearBusMonitor;

internal static class WorkBus {

    private static byte LastSource = 0;

    public static void Run(Medo.Device.BearBusMonitor bus) {
        Output.Line();
        Output.PacketHeader();
        while (true) {
            if (Console.KeyAvailable) {
                var key = Console.ReadKey(intercept: true);
                if (key.Key is ConsoleKey.Escape) {
                    return;
                } else if (key.Key == ConsoleKey.P) {  // Ping
                    if (LastSource != 0) {
                        var outPacket = BBPingPacket.Create(LastSource);
                        bus.Send(outPacket);
                        Output.Packet(outPacket);
                    }
                } else if (key.Key == ConsoleKey.R) {  // Reset
                    if (LastSource != 0) {
                        var outPacket = BBSystemHostPacket.CreateReboot(LastSource);
                        bus.Send(outPacket);
                        Output.Packet(outPacket);
                    }
                } else if (key.Key == ConsoleKey.U) {  // UID light
                    if (LastSource != 0) {
                        var shouldBlink = key.Modifiers != ConsoleModifiers.Shift;  // Shift turns it off
                        var outPacket = BBStatusPacket.Create(LastSource, newBlink: shouldBlink);
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

}
