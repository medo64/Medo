using System;
using Xunit;

namespace Medo.Tests.Timers.PerSecondLimiter {
    using System.Diagnostics;
    using System.Threading;
    using Medo.Timers;

    public class Tests {

        [Fact(DisplayName = "PerSecondLimiter: Basic")]
        public void Basic() {
            var tps = new PerSecondLimiter(1);
            while (DateTime.Now.Millisecond > 100) { Thread.Sleep(1); }  // just wait to get more of a second

            Assert.True(tps.IsReadyForNext());
            Assert.False(tps.IsReadyForNext());

            Thread.Sleep(1000);

            Assert.True(tps.IsReadyForNext());
            Assert.False(tps.IsReadyForNext());
        }

        [Fact(DisplayName = "PerSecondLimiter: Wait")]
        public void Wait() {
            var tps = new PerSecondLimiter(1);
            var sw = new Stopwatch();
            sw.Start();
            tps.WaitForNext();
            tps.WaitForNext();
            tps.WaitForNext();
            Assert.True(sw.ElapsedMilliseconds > 1000);
        }

        [Fact(DisplayName = "PerSecondLimiter: Wait with timeout")]
        public void WaitWithTimeout() {
            var tps = new PerSecondLimiter(1);
            var sw = new Stopwatch();
            sw.Start();
            tps.WaitForNext(500);
            tps.WaitForNext();
            tps.WaitForNext();
            Assert.False(tps.WaitForNext(1));  // just wait for 1 ms to check if all is ok
            Assert.True(sw.ElapsedMilliseconds > 1000);
        }


        [Fact(DisplayName = "PerSecondLimiter: No limit")]
        public void NoLimit() {
            var tps = new PerSecondLimiter(0);

            for (var i = 0; i < 100; i++) {
                Assert.True(tps.IsReadyForNext());
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