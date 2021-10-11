using System;
using System.IO.Ports;
using Xunit;
using Medo.Device;

namespace Tests.Medo.Device {
    public class CanankaTests {

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
