using System;
using Xunit;

namespace Medo.Tests.Timers.PerSecondCounter {
    using System.Threading;
    using Medo.Timers;

    public class Tests {

        [Fact(DisplayName = "PerSecondCounter: Basic")]
        public void Basic() {
            var tps = new PerSecondCounter();
            while (DateTime.Now.Millisecond > 100) { Thread.Sleep(1); }  // just wait to get more of a second

            tps.Increment();
            tps.Increment(2);

            Assert.Equal(0, tps.Value);
            Assert.Equal(0.0, tps.ValuePerSecond);

            Thread.Sleep(1000);

            Assert.Equal(3, tps.Value);
            Assert.Equal(3.0, tps.ValuePerSecond);

            Thread.Sleep(1000);

            Assert.Equal(0, tps.Value);
            Assert.Equal(0.0, tps.ValuePerSecond);
        }

        [Fact(DisplayName = "PerSecondCounter: Shorter timebase")]
        public void HalfASecond() {
            var tps = new PerSecondCounter(500);
            while (DateTime.Now.Millisecond > 100) { Thread.Sleep(1); }  // just wait to get more of a second

            tps.Increment();
            tps.Increment(2);

            Assert.Equal(0, tps.Value);
            Assert.Equal(0.0, tps.ValuePerSecond);

            Thread.Sleep(500);

            Assert.Equal(3, tps.Value);
            Assert.Equal(6.0, tps.ValuePerSecond);

            Thread.Sleep(500);

            Assert.Equal(0, tps.Value);
            Assert.Equal(0.0, tps.ValuePerSecond);
        }

        [Fact(DisplayName = "PerSecondCounter: Longer timebase")]
        public void TwoSeconds() {
            var tps = new PerSecondCounter(2000);
            while (DateTime.Now.Millisecond > 100) { Thread.Sleep(1); }  // just wait to get more of a second

            tps.Increment();
            tps.Increment(2);

            Assert.Equal(0, tps.Value);
            Assert.Equal(0.0, tps.ValuePerSecond);

            Thread.Sleep(2000);

            Assert.Equal(3, tps.Value);
            Assert.Equal(1.5, tps.ValuePerSecond);

            Thread.Sleep(2000);

            Assert.Equal(0, tps.Value);
            Assert.Equal(0.0, tps.ValuePerSecond);
        }

    }
}
