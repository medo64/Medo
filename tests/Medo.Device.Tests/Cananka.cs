using System;
using System.IO.Ports;
using Xunit;

namespace Medo.Device.Tests.Cananka {
    using Medo.Device;

    public class Tests {

        [Fact(DisplayName = "Cananka: Out of range")]
        public void NullPort() {
            Assert.Throws<ArgumentNullException>(() => {
                var _ = new Cananka(null);
            });
            Assert.Throws<ArgumentNullException>(() => {
                var _ = new Cananka(null, 9600);
            });
            Assert.Throws<ArgumentNullException>(() => {
                var _ = new Cananka(null, 9600, Parity.None, 8, StopBits.One);
            });
        }

    }
}
