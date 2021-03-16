using System;
using System.Threading.Tasks;
using Medo.Timers;

namespace Medo.Example.Throttling {
    internal class App {
        private static void Main() {
            var counter = new PerSecondCounter();
            counter.Tick = delegate {
                Console.WriteLine($"TPS: {counter.ValuePerSecond} /s");
            };

            var limiter = new PerSecondLimiter(13);
            while (true) {
                limiter.WaitForNext();
                counter.Increment();
            }

        }

    }
}
