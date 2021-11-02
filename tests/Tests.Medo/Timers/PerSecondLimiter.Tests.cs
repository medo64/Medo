using System;
using System.Diagnostics;
using System.Threading;
using Xunit;
using Medo.Timers;

namespace Tests.Medo.Timers {
    public class PerSecondLimiterTests {

        [Fact(DisplayName = "PerSecondLimiter: Basic")]
        public void Basic() {
            var tps = new PerSecondLimiter(1);

            while (DateTime.Now.Millisecond > 1) { Thread.Sleep(1); }  // just wait to get more of a second

            Assert.True(tps.Wait(0), "Should be ready (1)");
            Assert.False(tps.Wait(0), "Should be not ready (1)");

            Thread.Sleep(1000);

            Assert.True(tps.Wait(0), "Should be ready (2)");
            Assert.False(tps.Wait(0), "Should be not ready (2)");
        }

        [Fact(DisplayName = "PerSecondLimiter: Wait")]
        public void Wait() {
            var tps = new PerSecondLimiter(1);

            var sw = new Stopwatch();
            sw.Start();
            tps.Wait();
            tps.Wait();
            tps.Wait();
            Assert.True(sw.ElapsedMilliseconds > 1000);
        }

        [Fact(DisplayName = "PerSecondLimiter: Wait with timeout")]
        public void WaitWithTimeout() {
            var tps = new PerSecondLimiter(1);

            var sw = new Stopwatch();
            sw.Start();
            tps.Wait(500);
            tps.Wait();
            tps.Wait();
            Assert.False(tps.Wait(1));  // just wait for 1 ms to check if all is ok
            Assert.True(sw.ElapsedMilliseconds > 1000);
        }


        [Fact(DisplayName = "PerSecondLimiter: No limit")]
        public void NoLimit() {
            var tps = new PerSecondLimiter(0);

            for (var i = 0; i < 100; i++) {
                Assert.True(tps.Wait(0));
            }
        }

        [Fact(DisplayName = "PerSecondLimiter: Throw on negative")]
        public void ThrowOnNegative() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var _ = new PerSecondLimiter(-1);
            });
        }

    }
}
