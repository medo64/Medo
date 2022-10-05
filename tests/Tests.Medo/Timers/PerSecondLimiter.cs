using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Timers;

namespace Tests;

[TestClass]
public class PerSecondLimiter_Tests {

    [TestMethod]
    public void PerSecondLimiter_Basic() {
        var tps = new PerSecondLimiter(1);

        while (DateTime.Now.Millisecond > 1) { Thread.Sleep(1); }  // just wait to get more of a second

        Assert.IsTrue(tps.Wait(0), "Should be ready (1)");
        Assert.IsFalse(tps.Wait(0), "Should be not ready (1)");

        Thread.Sleep(1000);

        Assert.IsTrue(tps.Wait(0), "Should be ready (2)");
        Assert.IsFalse(tps.Wait(0), "Should be not ready (2)");
    }

    [TestMethod]
    public void PerSecondLimiter_Wait() {
        var tps = new PerSecondLimiter(1);

        var sw = new Stopwatch();
        sw.Start();
        tps.Wait();
        tps.Wait();
        tps.Wait();
        Assert.IsTrue(sw.ElapsedMilliseconds > 1000);
    }

    [TestMethod]
    public void PerSecondLimiter_WaitWithTimeout() {
        var tps = new PerSecondLimiter(1);

        var sw = new Stopwatch();
        sw.Start();
        tps.Wait(500);
        tps.Wait();
        tps.Wait();
        Assert.IsFalse(tps.Wait(1));  // just wait for 1 ms to check if all is ok
        Assert.IsTrue(sw.ElapsedMilliseconds > 1000);
    }

    [TestMethod]
    public void PerSecondLimiter_NoLimit() {
        var tps = new PerSecondLimiter(0);

        for (var i = 0; i < 100; i++) {
            Assert.IsTrue(tps.Wait(0));
        }
    }

    [TestMethod]
    public void PerSecondLimiter_ThrowOnNegative() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var _ = new PerSecondLimiter(-1);
        });
    }

}
