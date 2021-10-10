using System;
using System.Runtime.Versioning;
using Xunit;
using Medo.Windows.Forms;

namespace Tests.Medo.Windows.Forms {
    [SupportedOSPlatform("windows")]
    public class TimerResolutionTests {

        [Fact(DisplayName = "TimerResolution: Basic")]
        public void Basic() {
            using var mmTimer = new TimerResolution(4);
            Assert.True(mmTimer.Successful);
            Assert.Equal(4, mmTimer.DesiredResolutionInMilliseconds);
        }


        [Fact(DisplayName = "TimerResolution: Out of range")]
        public void ThrowsOutOfRange() {
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var mmTimer = new TimerResolution(0);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var mmTimer = new TimerResolution(16);
            });
        }

    }
}
