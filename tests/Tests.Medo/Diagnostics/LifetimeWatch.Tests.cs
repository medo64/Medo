using Xunit;
using Medo.Diagnostics;

namespace Tests.Medo.Diagnostics {
    public class LifetimeWatchTests {

        [Fact(DisplayName = "LifetimeWatch: Basic")]
        public void Basic() {
            var lifetime = new LifetimeWatch();
            var t1 = lifetime.ElapsedTicks;
            var t2 = lifetime.ElapsedTicks;
            lifetime.Dispose();
            var t3 = lifetime.ElapsedTicks;
            var t4 = lifetime.ElapsedTicks;
            Assert.True(t2 > t1);
            Assert.True(t3 > t2);
            Assert.True(t3 == t4);
        }

        [Fact(DisplayName = "LifetimeWatch: Double dispose")]
        public void DoubleDisposeOk() {
            var lifetime = new LifetimeWatch();
            var t1 = lifetime.ElapsedTicks;
            var t2 = lifetime.ElapsedTicks;
            lifetime.Dispose();
            lifetime.Dispose();
            var t3 = lifetime.ElapsedTicks;
            lifetime.Dispose();
            var t4 = lifetime.ElapsedTicks;
            Assert.True(t2 > t1);
            Assert.True(t3 > t2);
            Assert.True(t3 == t4);
        }

    }
}
