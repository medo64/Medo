using Medo.Device;
using System;
using System.IO;
using System.Threading;

internal class App {

    private static void Main() {
        Console.TreatControlCAsInput = true;

        Console.Write("Create [S]erver or [C]lient? ");
        while (true) {
            var key = Console.ReadKey(intercept: true);
            if ((key.Modifiers == ConsoleModifiers.Control) && (key.Key == ConsoleKey.C)) {
                CtrlC();
                break;
            } else if (key.Key == ConsoleKey.S) {
                Console.Write("S");
                TcpBus.CreateServer();
                Console.Clear();
                RunHost(TcpBus.ConnectClient());
            } else if (key.Key == ConsoleKey.C) {
                Console.Write("C");
                Console.Clear();
                RunDevice(TcpBus.ConnectClient());
                break;
            }
        }
    }


    public static void RunHost(Stream stream) {
        Console.WriteLine("[P]ing ....: pings random address");
        Console.WriteLine("[R]eboot ..: reboots all");
        Console.WriteLine("[X]L packet: send maximum size packet");
        Console.WriteLine("[Y]L packet: send maximum CRC8 packet");
        Console.WriteLine();

        var bus = BearBus.CreateHost(stream);

        while (true) {
            if (Console.KeyAvailable) {
                var key = Console.ReadKey(intercept: true);
                if ((key.Key == ConsoleKey.Escape) || ((key.Modifiers == ConsoleModifiers.Control) && (key.Key == ConsoleKey.C))) {
                    CtrlC();
                    break;
                } else if (key.Key == ConsoleKey.P) {
                    bus.Send(BBPingPacket.Create((byte)(1 + Random.Shared.Next(126))));
                } else if (key.Key == ConsoleKey.R) {
                    bus.Send(BBSystemHostPacket.CreateReboot(0));
                } else if (key.Key == ConsoleKey.X) {
                    var data = new byte[240];
                    for (var i = 0; i < data.Length; i++) { data[i] = (byte)i; }
                    bus.Send(BBCustomPacket.Create((byte)(1 + Random.Shared.Next(126)), 42, data, true));
                } else if (key.Key == ConsoleKey.Y) {
                    var data = new byte[13];
                    for (var i = 0; i < data.Length; i++) { data[i] = (byte)i; }
                    bus.Send(BBCustomPacket.Create((byte)(1 + Random.Shared.Next(126)), 42, data, true));
                }
            }

            if (bus.TryReceive(out var packet)) {
                WritePacket(packet);
            }
            Thread.Sleep(1);
        }
    }


    public static void RunDevice(Stream stream) {
        Console.WriteLine("[A]ddress: increase own address by one");
        Console.WriteLine("[B]utton : unsolicited address report");
        Console.WriteLine("[C]onfig : switch to config mode");
        Console.WriteLine("[E]rror .: increase error code by one");
        Console.WriteLine("[N]ormal : switch to normal mode");
        Console.WriteLine("[P]rogram: switch to program mode");
        Console.WriteLine("[T]est ..: switch to test mode");
        Console.WriteLine("[X]L packet: send maximum size packet");
        Console.WriteLine("[Y]L packet: send maximum CRC8 packet");
        Console.WriteLine();

        var bus = BearBus.CreateDevice(stream);
        var address = (byte)Random.Shared.Next(127);
        var blinking = false;
        var mode = BBDeviceMode.Normal;
        var errorCode = (byte)0;

        while (true) {
            if (Console.KeyAvailable) {
                var key = Console.ReadKey(intercept: true);
                if ((key.Key == ConsoleKey.Escape) || ((key.Modifiers == ConsoleModifiers.Control) && (key.Key == ConsoleKey.C))) {
                    CtrlC();
                    break;
                } else if (key.Key == ConsoleKey.A) {
                    address = (byte)((address + 1) % 128);
                    bus.Send(BBSystemDevicePacket.CreateStatusUpdate(address, blinking, mode, errorCode));
                } else if (key.Key == ConsoleKey.B) {
                    bus.Send(BBSystemDevicePacket.CreateStatusUpdate(address, blinking, mode, errorCode));
                } else if (key.Key == ConsoleKey.C) {
                    mode = BBDeviceMode.Config;
                    bus.Send(BBSystemDevicePacket.CreateStatusUpdate(address, blinking, mode, errorCode));
                } else if (key.Key == ConsoleKey.E) {
                    errorCode = (byte)((errorCode + 1) % 8);
                    bus.Send(BBSystemDevicePacket.CreateStatusUpdate(address, blinking, mode, errorCode));
                } else if (key.Key == ConsoleKey.N) {
                    mode = BBDeviceMode.Normal;
                    bus.Send(BBSystemDevicePacket.CreateStatusUpdate(address, blinking, mode, errorCode));
                } else if (key.Key == ConsoleKey.P) {
                    mode = BBDeviceMode.Program;
                    bus.Send(BBSystemDevicePacket.CreateStatusUpdate(address, blinking, mode, errorCode));
                } else if (key.Key == ConsoleKey.T) {
                    mode = BBDeviceMode.Test;
                    bus.Send(BBSystemDevicePacket.CreateStatusUpdate(address, blinking, mode, errorCode));
                } else if (key.Key == ConsoleKey.X) {
                    var data = new byte[240];
                    for (var i = 0; i < data.Length; i++) { data[i] = (byte)i; }
                    bus.Send(BBCustomReplyPacket.Create(address, 42, data));
                } else if (key.Key == ConsoleKey.Y) {
                    var data = new byte[13];
                    for (var i = 0; i < data.Length; i++) { data[i] = (byte)i; }
                    bus.Send(BBCustomReplyPacket.Create(address, 42, data));
                }
            }

            if (bus.TryReceive(out var packet)) {
                WritePacket(packet);
            }

            Thread.Sleep(1);
        }
    }


    private static void WritePacket(IBBPacket packet) {
        Console.Write(DateTime.Now.ToString("HH:mm:ss"));

        var typeText = packet.GetType().ToString().Split(".")[^1];
        Console.Write("  ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(typeText);
        Console.ResetColor();

        Console.Write("  ");
        Console.Write(packet.FromAddress?.ToString("X2") ?? "Host");
        Console.Write(" to ");
        Console.Write(packet.ToAddress?.ToString("X2") ?? "Host");

        Console.WriteLine();

        Console.WriteLine(BitConverter.ToString(packet.Data));
        Console.WriteLine();
    }


    private static void CtrlC() {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("^C");
        Console.ResetColor();
        Environment.Exit(1);
    }

}
