using System;
using System.ComponentModel;
using Medo.Application;

internal static class App {

    public static void Main() {
        Console.WriteLine("Application Started");

        Console.WriteLine();

        Console.WriteLine("Checking Upgrade...");
        var upgradeFile = Upgrade.GetUpgradeFile(new Uri("http://medo64.com/upgrade"), "Test", new Version(0, 0, 1));
        if (upgradeFile == null) {
            Console.WriteLine("No upgrade.");
            return;
        }

        Console.WriteLine("Upgrade available.");

        Console.WriteLine();

        Console.Write("Downloading upgrade...");
        var pos = Console.GetCursorPosition();
        upgradeFile.ProgressChanged += delegate (object? sender, ProgressChangedEventArgs e) {
            Console.SetCursorPosition(pos.Left, pos.Top);
            Console.Write($" {e.ProgressPercentage}%");
        };
        var file = upgradeFile.DownloadFile();
        Console.WriteLine();
        if (file != null) {
            Console.WriteLine($"Download completed ({file.FullName}).");
        } else {
            Console.WriteLine("Download not completed!");
        }
    }

}
