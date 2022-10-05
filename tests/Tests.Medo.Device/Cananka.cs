using System;
using System.IO.Ports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Device;

namespace Tests;

[TestClass]
public class Cananka_Tests {

    [TestMethod]
    public void Cananka_NullPort() {
        Assert.ThrowsException<ArgumentNullException>(() => {
            var _ = new Cananka(null);
        });
        Assert.ThrowsException<ArgumentNullException>(() => {
            var _ = new Cananka(null, 9600);
        });
        Assert.ThrowsException<ArgumentNullException>(() => {
            var _ = new Cananka(null, 9600, Parity.None, 8, StopBits.One);
        });
    }

}
