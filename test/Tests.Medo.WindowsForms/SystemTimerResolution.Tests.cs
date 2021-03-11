using System;
using Xunit;

namespace Medo.Windows.Forms.Tests.SystemTimerResolution {
    using Medo.Windows.Forms;

    public class Tests {

        [Fact(DisplayName = "SystemTimerResolution: Query values")]
        public void Query() {
            var minimum = SystemTimerResolution.MinimumResolutionInTicks;
            var maximum = SystemTimerResolution.MaximumResolutionInTicks;
            var current = SystemTimerResolution.ResolutionInTicks;
            Assert.True(minimum >= 0);
            Assert.True(maximum >= 0);
            Assert.True(current >= 0);
            Assert.True(current >= minimum);
            Assert.True(current <= maximum);
        }

        [Fact(DisplayName = "SystemTimerResolution: Out of range")]
        public void ThrowsOutGlobalOfRange() {
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                SystemTimerResolution.ResolutionInTicks = 0;
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var minimum = SystemTimerResolution.MinimumResolutionInTicks;
                SystemTimerResolution.ResolutionInTicks = minimum - 1;
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var maximum = SystemTimerResolution.MaximumResolutionInTicks;
                SystemTimerResolution.ResolutionInTicks = maximum + 1;
            });
        }

    }
}
