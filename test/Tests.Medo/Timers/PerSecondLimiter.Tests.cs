using System;
using Xunit;

namespace Medo.Tests.Timers.PerSecondLimiter {
    using System.Diagnostics;
    using System.Threading;
    using Medo.Timers;

    public class Tests {

        [Fact(DisplayName = "PerSecondLimiter: Basic")]
        public void Basic() {
            var count = 0;
            var tps = new PerSecondLimiter(1);
            tps.TicketsAvailable += delegate { Interlocked.Increment(ref count); };

            while (DateTime.Now.Millisecond > 10) { Thread.Sleep(1); }  // just wait to get more of a second

            Assert.True(tps.IsReadyForNext(), "Should be ready (1)");
            Assert.False(tps.IsReadyForNext(), "Should be not ready (1)");

            Thread.Sleep(1000);

            Assert.True(tps.IsReadyForNext(), "Should be ready (2)");
            Assert.False(tps.IsReadyForNext(), "Should be not ready (2)");

            Assert.True(count >= 2, $"Count {count} too small.");
        }

        [Fact(DisplayName = "PerSecondLimiter: Wait")]
        public void Wait() {
            var count = 0;
            var tps = new PerSecondLimiter(1);
            tps.TicketsAvailable += delegate { Interlocked.Increment(ref count); };

            var sw = new Stopwatch();
            sw.Start();
            tps.WaitForNext();
            tps.WaitForNext();
            tps.WaitForNext();
            Assert.True(sw.ElapsedMilliseconds > 1000);

            Assert.True(count >= 2, $"Count {count} too small.");
        }

        [Fact(DisplayName = "PerSecondLimiter: Wait with timeout")]
        public void WaitWithTimeout() {
            var count = 0;
            var tps = new PerSecondLimiter(1);
            tps.TicketsAvailable += delegate { Interlocked.Increment(ref count); };

            var sw = new Stopwatch();
            sw.Start();
            tps.WaitForNext(500);
            tps.WaitForNext();
            tps.WaitForNext();
            Assert.False(tps.WaitForNext(1));  // just wait for 1 ms to check if all is ok
            Assert.True(sw.ElapsedMilliseconds > 1000);

            Assert.True(count >= 2, $"Count {count} too small.");
        }


        [Fact(DisplayName = "PerSecondLimiter: No limit")]
        public void NoLimit() {
            var count = 0;
            var tps = new PerSecondLimiter(0);
            tps.TicketsAvailable += delegate { Interlocked.Increment(ref count); };

            for (var i = 0; i < 100; i++) {
                Assert.True(tps.IsReadyForNext());
            }

            Assert.Equal(0, count);  // no tickets when unlimited
        }

        [Fact(DisplayName = "PerSecondLimiter: Throw on negative")]
        public void ThrowOnNegative() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var _ = new PerSecondLimiter(-1);
            });
        }

    }
}
