using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Diagnostics;

namespace Tests;

[TestClass]
public class LifetimeWatch_Tests {

    [TestMethod]
    public void LifetimeWatch_Basic() {
        var lifetime = new LifetimeWatch();
        var t1 = lifetime.ElapsedTicks; Thread.Sleep(1);
        var t2 = lifetime.ElapsedTicks; Thread.Sleep(1);
        lifetime.Dispose();
        var t3 = lifetime.ElapsedTicks; Thread.Sleep(1);
        var t4 = lifetime.ElapsedTicks;
        Assert.IsTrue(t2 > t1);
        Assert.IsTrue(t3 > t2);
        Assert.IsTrue(t3 == t4);
    }

    [TestMethod]
    public void LifetimeWatch_DoubleDisposeOk() {
        var lifetime = new LifetimeWatch();
        var t1 = lifetime.ElapsedTicks; Thread.Sleep(1);
        var t2 = lifetime.ElapsedTicks; Thread.Sleep(1);
        lifetime.Dispose();
        lifetime.Dispose();
        var t3 = lifetime.ElapsedTicks; Thread.Sleep(1);
        lifetime.Dispose();
        var t4 = lifetime.ElapsedTicks;
        Assert.IsTrue(t2 > t1);
        Assert.IsTrue(t3 > t2);
        Assert.IsTrue(t3 == t4);
    }

}
