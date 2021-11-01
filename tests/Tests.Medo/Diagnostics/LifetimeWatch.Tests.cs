using Xunit;
using Medo.Diagnostics;
using System.Threading;

namespace Tests.Medo.Diagnostics {
    public class LifetimeWatchTests {

        [Fact(DisplayName = "LifetimeWatch: Basic")]
        public void Basic() {
            var lifetime = new LifetimeWatch();
            var t1 = lifetime.ElapsedTicks; Thread.Sleep(1);
            var t2 = lifetime.ElapsedTicks; Thread.Sleep(1);
            lifetime.Dispose();
            var t3 = lifetime.ElapsedTicks; Thread.Sleep(1);
            var t4 = lifetime.ElapsedTicks;
            Assert.True(t2 > t1);
            Assert.True(t3 > t2);
            Assert.True(t3 == t4);
        }

        [Fact(DisplayName = "LifetimeWatch: Double dispose")]
        public void DoubleDisposeOk() {
            var lifetime = new LifetimeWatch();
            var t1 = lifetime.ElapsedTicks; Thread.Sleep(1);
            var t2 = lifetime.ElapsedTicks; Thread.Sleep(1);
            lifetime.Dispose();
            lifetime.Dispose();
            var t3 = lifetime.ElapsedTicks; Thread.Sleep(1);
            lifetime.Dispose();
            var t4 = lifetime.ElapsedTicks;
            Assert.True(t2 > t1);
            Assert.True(t3 > t2);
            Assert.True(t3 == t4);
        }

    }
}
