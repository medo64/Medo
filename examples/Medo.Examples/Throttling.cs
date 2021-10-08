using System;
using Medo.Timers;

namespace Medo.Examples {
    internal static class Throttling {

        public static void Run() {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Throttling");
            Console.ResetColor();

            var counter = new PerSecondCounter();
            counter.Tick = delegate {
                Console.WriteLine($"TPS: {counter.ValuePerSecond} /s");
            };

            var limiter = new PerSecondLimiter(13);
            while (true) {
                limiter.Wait();
                counter.Increment();
            }
        }

    }
}
