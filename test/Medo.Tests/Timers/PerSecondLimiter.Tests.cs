using System;
using Xunit;

namespace Medo.Tests.Timers.PerSecondLimiter {
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

    }
}
