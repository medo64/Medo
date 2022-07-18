namespace BearBusMonitor;

using Medo.Device;
using System;

internal static class Output {

    public static void Line() {
        Console.WriteLine();
    }

    public static void Line(string text) {
        Console.WriteLine(text);
    }

    public static void Line(char ch, string text) {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("  [");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write(ch);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("] ");
        Console.ResetColor();
        Console.WriteLine(text);
    }

    public static void Header(string text) {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(text.ToUpperInvariant() + ":");
        Console.ResetColor();
    }

    public static void Error(string text, bool waitForEnter = false) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(text);
        Console.ResetColor();
        if (waitForEnter) {
            Console.Error.WriteLine("Press <Enter> to continue");
            while (true) {
                var key = Console.ReadKey(intercept: true);
                if (key.Key is ConsoleKey.Enter or ConsoleKey.Escape) { return; }
            }
        }
    }

    public static void PacketHeader() {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Time     From To   Cmd  F Data");
        Console.ResetColor();
    }

    internal static void Packet(IBBPacket packet) {
        var color = packet.FromAddress == null ? ConsoleColor.Yellow : ConsoleColor.Cyan;
        Console.Write(DateTime.Now.ToString("HH:mm:ss"));
        Console.Write(" ");

        Console.Write("0x");
        Console.ForegroundColor = color;
        Console.Write((packet.FromAddress != null) ? packet.FromAddress?.ToString("X2") : "00");
        Console.ResetColor();

        Console.Write(" 0x");
        Console.ForegroundColor = color;
        Console.Write((packet.ToAddress != null) ? packet.ToAddress?.ToString("X2") : "00");
        Console.ResetColor();

        Console.Write(" 0x");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(packet.CommandCode.ToString("X2"));
        Console.ResetColor();

        Console.Write(" ");
        Console.ForegroundColor = color;
        if (packet is IBBHostPacket hostPacket) {
            Console.Write(hostPacket.IsReplyRequested ? "R" : "-");
        } else if (packet is IBBDevicePacket devicePacket) {
            Console.Write(devicePacket.IsErrorReply ? "E" : "-");
        } else {  // should neve happen
            Console.Write(" ");
        }
        Console.ResetColor();

        Console.Write(" ");
        Console.ForegroundColor = color;
        Console.Write(BitConverter.ToString(packet.Data));
        Console.ResetColor();

        Console.WriteLine();
    }

    public static void Select(string title, KeySelection selection) {
        Header(title);
        foreach (var entry in selection.EnumerateEntries()) {
            Console.Write("  ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(entry.Item1);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("] ");
            Console.ResetColor();
            Console.WriteLine(entry.Item2);
        }

        while (true) {
            var key = Console.ReadKey(intercept: true);
            if (key.Key is ConsoleKey.Enter or ConsoleKey.Escape) { return; }
            if (selection.Invoke(char.ToUpperInvariant(key.KeyChar))) { return; }
        }
    }

}
