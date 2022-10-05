using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Timers;

namespace Tests;

[TestClass]
public class PerSecondCounter_Tests {

    [TestMethod]
    public void PerSecondCounter_Basic() {
        var tps = new PerSecondCounter();
        while (DateTime.Now.Millisecond > 100) { Thread.Sleep(1); }  // just wait to get more of a second

        tps.Increment();
        tps.Increment(2);

        Assert.AreEqual(0, tps.Value);
        Assert.AreEqual(0.0, tps.ValuePerSecond);

        Thread.Sleep(1000);

        Assert.AreEqual(3, tps.Value);
        Assert.AreEqual(3.0, tps.ValuePerSecond);

        Thread.Sleep(1000);

        Assert.AreEqual(0, tps.Value);
        Assert.AreEqual(0.0, tps.ValuePerSecond);
    }

    [TestMethod]
    public void PerSecondCounter_HalfASecond() {
        var tps = new PerSecondCounter(500);
        while (DateTime.Now.Millisecond > 100) { Thread.Sleep(1); }  // just wait to get more of a second

        tps.Increment();
        tps.Increment(2);

        Assert.AreEqual(0, tps.Value);
        Assert.AreEqual(0.0, tps.ValuePerSecond);

        Thread.Sleep(500);

        Assert.AreEqual(3, tps.Value);
        Assert.AreEqual(6.0, tps.ValuePerSecond);

        Thread.Sleep(500);

        Assert.AreEqual(0, tps.Value);
        Assert.AreEqual(0.0, tps.ValuePerSecond);
    }

    [TestMethod]
    public void PerSecondCounter_TwoSeconds() {
        var tps = new PerSecondCounter(2000);
        while (DateTime.Now.Millisecond > 100) { Thread.Sleep(1); }  // just wait to get more of a second

        tps.Increment();
        tps.Increment(2);

        Assert.AreEqual(0, tps.Value);
        Assert.AreEqual(0.0, tps.ValuePerSecond);

        Thread.Sleep(2000);

        Assert.AreEqual(3, tps.Value);
        Assert.AreEqual(1.5, tps.ValuePerSecond);

        Thread.Sleep(2000);

        Assert.AreEqual(0, tps.Value);
        Assert.AreEqual(0.0, tps.ValuePerSecond);
    }

    [TestMethod]
    public void PerSecondCounter_ThrowOnOutOfRange() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var _ = new PerSecondCounter(99);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var _ = new PerSecondCounter(10001);
        });
    }

}
