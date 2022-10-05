using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Diagnostics;

namespace Tests;

[TestClass]
public class SpreadIntSequence_Tests {

    [TestMethod]
    public void SpreadIntSequence_AsNumber() {
        var seq = new SpreadIntSequence(0);
        Assert.AreEqual(0x9E3779B1U, seq.Next());
        Assert.AreEqual(0x3C6EF362U, seq.Next());
        Assert.AreEqual(0xDAA66D13U, seq.Next());
        Assert.AreEqual(0x78DDE6C4U, seq.Next());
        Assert.AreEqual(0x17156075U, seq.Next());
        Assert.AreEqual(0xB54CDA26U, seq.Next());
        Assert.AreEqual(0x538453D7U, seq.Next());
        Assert.AreEqual(0xF1BBCD88U, seq.Next());
        Assert.AreEqual(0x8FF34739U, seq.Next());
        Assert.AreEqual(0x2E2AC0EAU, seq.Next());
        Assert.AreEqual(0xCC623A9BU, seq.Next());
        Assert.AreEqual(0x6A99B44CU, seq.Next());
        Assert.AreEqual(0x08D12DFDU, seq.Next());
    }

    [TestMethod]
    public void SpreadIntSequence_AsString() {
        var seq = new SpreadIntSequence(0);
        Assert.AreEqual("9E3779B1", seq.NextAsString());
        Assert.AreEqual("3C6EF362", seq.NextAsString());
        Assert.AreEqual("DAA66D13", seq.NextAsString());
        Assert.AreEqual("78DDE6C4", seq.NextAsString());
        Assert.AreEqual("17156075", seq.NextAsString());
        Assert.AreEqual("B54CDA26", seq.NextAsString());
        Assert.AreEqual("538453D7", seq.NextAsString());
        Assert.AreEqual("F1BBCD88", seq.NextAsString());
        Assert.AreEqual("8FF34739", seq.NextAsString());
        Assert.AreEqual("2E2AC0EA", seq.NextAsString());
        Assert.AreEqual("CC623A9B", seq.NextAsString());
        Assert.AreEqual("6A99B44C", seq.NextAsString());
        Assert.AreEqual("08D12DFD", seq.NextAsString());
    }

    [TestMethod]
    public void SpreadIntSequence_AsStringWithPrefix() {
        var seq = new SpreadIntSequence(0);
        Assert.AreEqual("C-9E3779B1", seq.NextAsString("C-"));
        Assert.AreEqual("C-3C6EF362", seq.NextAsString("C-"));
        Assert.AreEqual("C-DAA66D13", seq.NextAsString("C-"));
        Assert.AreEqual("C-78DDE6C4", seq.NextAsString("C-"));
        Assert.AreEqual("C-17156075", seq.NextAsString("C-"));
        Assert.AreEqual("C-B54CDA26", seq.NextAsString("C-"));
        Assert.AreEqual("C-538453D7", seq.NextAsString("C-"));
        Assert.AreEqual("C-F1BBCD88", seq.NextAsString("C-"));
        Assert.AreEqual("C-8FF34739", seq.NextAsString("C-"));
        Assert.AreEqual("C-2E2AC0EA", seq.NextAsString("C-"));
        Assert.AreEqual("C-CC623A9B", seq.NextAsString("C-"));
        Assert.AreEqual("C-6A99B44C", seq.NextAsString("C-"));
        Assert.AreEqual("C-08D12DFD", seq.NextAsString("C-"));
    }

    [TestMethod]
    public void SpreadIntSequence_Multithreaded() {
        var seq = new SpreadIntSequence();
        var memory = new ConcurrentDictionary<uint, int>();
        var tasks = new Task[16];
        for (var t = 0; t < tasks.Length; t++) {
            tasks[t] = Task.Run(delegate {
                for (var i = 0; i < 256; i++) {
                    var key = seq.Next();
                    memory.AddOrUpdate(key, 1, (key, oldValue) => oldValue + 1);
                }
            });
        }
        foreach (var task in tasks) {
            task.Wait();
        }

        foreach (var mem in memory) {
            Assert.AreEqual(1, mem.Value);
        }
    }

}
