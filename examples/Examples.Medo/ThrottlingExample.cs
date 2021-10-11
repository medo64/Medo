using System;
using Medo.IO;
using Medo.Timers;

namespace Medo.Examples {
    internal static class ThrottlingExample {

        public static void Run() {
            var terminal = Terminal.Console;
            terminal.Clear();
            terminal.WriteLine(" Terminal ", ConsoleColor.Yellow, ConsoleColor.DarkGray);
            terminal.WriteLine();

            using var counter = new PerSecondCounter();
            counter.Tick = delegate {
                Console.WriteLine($"TPS: {counter.ValuePerSecond} /s");
            };

            var limiter = new PerSecondLimiter(13);

            while (true) {
                foreach (var key in Terminal.ReadAvailableKeys()) {
                    switch (key) {
                        case ConsoleKey.OemPlus: limiter.PerSecondRate += 1; break;
                        case ConsoleKey.OemMinus:
                            if (limiter.PerSecondRate > 1) {
                                limiter.PerSecondRate -= 1;
                            }
                            break;
                        case ConsoleKey.Escape: return;
                    }
                }

                limiter.Wait();
                counter.Increment();
            }
        }

    }
}
