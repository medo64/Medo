using System;
using Medo.Application;

internal static class App {

    public static void Main(string[] args) {
        SingleInstance.Attach();  // will auto-exit for second instance
        SingleInstance.NewInstanceDetected += SingleInstance_NewInstanceDetected;

        Console.Write("First instance:");
        foreach (var arg in args) {
            Console.Write(" " + arg);
        }
        Console.WriteLine();

        Console.WriteLine("Waiting for <Escape>");
        while (Console.ReadKey().Key != ConsoleKey.Escape) { }
    }

    private static void SingleInstance_NewInstanceDetected(object? sender, NewInstanceEventArgs e) {
        Console.Write("Other instance:");
        foreach (var arg in e.Args) {
            Console.Write(" " + arg);
        }
        Console.WriteLine();
    }

}
