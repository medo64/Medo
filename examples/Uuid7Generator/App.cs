using System;
using System.Diagnostics;
using System.Threading;
using Medo;

internal static class App {

    public static void Main() {
        var one = Uuid.NewUuid7();
        Console.WriteLine($"UUID: {one}");
        Console.WriteLine($"ID25: {one.ToId25String()}");
        Console.WriteLine();

        Thread.Sleep(1000);

        {
            var guidCount = 0;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 3000) {
                _ = Guid.NewGuid();
                guidCount++;
            }
            sw.Stop();
            Console.WriteLine($"Generated {guidCount:#,##0} GUIDs in {sw.ElapsedMilliseconds:#,##0} millisecond ({guidCount / sw.ElapsedMilliseconds * 1000:#,##0} per second)");
        }

        Thread.Sleep(1000);

        {
            var uuidCount = 0;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 3000) {
                _ = Uuid.NewUuid7();
                uuidCount++;
            }
            sw.Stop();
            Console.WriteLine($"Generated {uuidCount:#,##0} UUIDs in {sw.ElapsedMilliseconds:#,##0} millisecond ({uuidCount / sw.ElapsedMilliseconds * 1000:#,##0} per second)");
        }
    }

}
