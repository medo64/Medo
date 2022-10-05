using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Diagnostics;

namespace Tests;

[TestClass]
public class SpreadLongSequence_Tests {

    [TestMethod]
    public void SpreadLongSequence_AsNumber() {
        var seq = new SpreadLongSequence(0);
        Assert.AreEqual(0x9E3779B97F4A936DUL, seq.Next());
        Assert.AreEqual(0x3C6EF372FE9526DAUL, seq.Next());
        Assert.AreEqual(0xDAA66D2C7DDFBA47UL, seq.Next());
        Assert.AreEqual(0x78DDE6E5FD2A4DB4UL, seq.Next());
        Assert.AreEqual(0x1715609F7C74E121UL, seq.Next());
        Assert.AreEqual(0xB54CDA58FBBF748EUL, seq.Next());
        Assert.AreEqual(0x538454127B0A07FBUL, seq.Next());
        Assert.AreEqual(0xF1BBCDCBFA549B68UL, seq.Next());
        Assert.AreEqual(0x8FF34785799F2ED5UL, seq.Next());
        Assert.AreEqual(0x2E2AC13EF8E9C242UL, seq.Next());
        Assert.AreEqual(0xCC623AF8783455AFUL, seq.Next());
        Assert.AreEqual(0x6A99B4B1F77EE91CUL, seq.Next());
        Assert.AreEqual(0x08D12E6B76C97C89UL, seq.Next());
    }

    [TestMethod]
    public void SpreadLongSequence_AsString() {
        var seq = new SpreadLongSequence(0);
        Assert.AreEqual("9E3779B97F4A936D", seq.NextAsString());
        Assert.AreEqual("3C6EF372FE9526DA", seq.NextAsString());
        Assert.AreEqual("DAA66D2C7DDFBA47", seq.NextAsString());
        Assert.AreEqual("78DDE6E5FD2A4DB4", seq.NextAsString());
        Assert.AreEqual("1715609F7C74E121", seq.NextAsString());
        Assert.AreEqual("B54CDA58FBBF748E", seq.NextAsString());
        Assert.AreEqual("538454127B0A07FB", seq.NextAsString());
        Assert.AreEqual("F1BBCDCBFA549B68", seq.NextAsString());
        Assert.AreEqual("8FF34785799F2ED5", seq.NextAsString());
        Assert.AreEqual("2E2AC13EF8E9C242", seq.NextAsString());
        Assert.AreEqual("CC623AF8783455AF", seq.NextAsString());
        Assert.AreEqual("6A99B4B1F77EE91C", seq.NextAsString());
        Assert.AreEqual("08D12E6B76C97C89", seq.NextAsString());
    }

    [TestMethod]
    public void SpreadLongSequence_AsStringWithPrefix() {
        var seq = new SpreadLongSequence(0);
        Assert.AreEqual("C-9E3779B97F4A936D", seq.NextAsString("C-"));
        Assert.AreEqual("C-3C6EF372FE9526DA", seq.NextAsString("C-"));
        Assert.AreEqual("C-DAA66D2C7DDFBA47", seq.NextAsString("C-"));
        Assert.AreEqual("C-78DDE6E5FD2A4DB4", seq.NextAsString("C-"));
        Assert.AreEqual("C-1715609F7C74E121", seq.NextAsString("C-"));
        Assert.AreEqual("C-B54CDA58FBBF748E", seq.NextAsString("C-"));
        Assert.AreEqual("C-538454127B0A07FB", seq.NextAsString("C-"));
        Assert.AreEqual("C-F1BBCDCBFA549B68", seq.NextAsString("C-"));
        Assert.AreEqual("C-8FF34785799F2ED5", seq.NextAsString("C-"));
        Assert.AreEqual("C-2E2AC13EF8E9C242", seq.NextAsString("C-"));
        Assert.AreEqual("C-CC623AF8783455AF", seq.NextAsString("C-"));
        Assert.AreEqual("C-6A99B4B1F77EE91C", seq.NextAsString("C-"));
        Assert.AreEqual("C-08D12E6B76C97C89", seq.NextAsString("C-"));
    }

    [TestMethod]
    public void SpreadLongSequence_Multithreaded() {
        var seq = new SpreadLongSequence();
        var memory = new ConcurrentDictionary<ulong, int>();
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
