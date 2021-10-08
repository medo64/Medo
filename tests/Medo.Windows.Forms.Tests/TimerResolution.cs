using System;
using System.Runtime.Versioning;
using Xunit;

namespace Medo.Windows.Forms.Tests.TimerResolution {
    using Medo.Windows.Forms;

    [SupportedOSPlatform("windows")]
    public class Tests {

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
