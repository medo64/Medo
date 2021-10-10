using System;
using Medo.Timers;

namespace Medo.Examples {
    internal static class Throttling {

        public static void Run() {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Throttling");
            Console.ResetColor();

            using var counter = new PerSecondCounter();
            counter.Tick = delegate {
                Console.WriteLine($"TPS: {counter.ValuePerSecond} /s");
            };

            var limiter = new PerSecondLimiter(13);

            while (true) {
                while (Console.KeyAvailable) {
                    switch (Console.ReadKey(intercept: true).Key) {
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
