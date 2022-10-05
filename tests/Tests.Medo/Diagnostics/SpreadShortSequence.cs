using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Diagnostics;

namespace Tests;

[TestClass]
public class SpreadShortSequence_Tests {

    [TestMethod]
    public void SpreadShortSequence_AsNumber() {
        var seq = new SpreadShortSequence(0);
        Assert.AreEqual(0x9E33, seq.Next());
        Assert.AreEqual(0x3C66, seq.Next());
        Assert.AreEqual(0xDA99, seq.Next());
        Assert.AreEqual(0x78CC, seq.Next());
        Assert.AreEqual(0x16FF, seq.Next());
        Assert.AreEqual(0xB532, seq.Next());
        Assert.AreEqual(0x5365, seq.Next());
        Assert.AreEqual(0xF198, seq.Next());
        Assert.AreEqual(0x8FCB, seq.Next());
        Assert.AreEqual(0x2DFE, seq.Next());
        Assert.AreEqual(0xCC31, seq.Next());
        Assert.AreEqual(0x6A64, seq.Next());
        Assert.AreEqual(0x0897, seq.Next());
    }

    [TestMethod]
    public void SpreadShortSequence_AsString() {
        var seq = new SpreadShortSequence(0);
        Assert.AreEqual("9E33", seq.NextAsString());
        Assert.AreEqual("3C66", seq.NextAsString());
        Assert.AreEqual("DA99", seq.NextAsString());
        Assert.AreEqual("78CC", seq.NextAsString());
        Assert.AreEqual("16FF", seq.NextAsString());
        Assert.AreEqual("B532", seq.NextAsString());
        Assert.AreEqual("5365", seq.NextAsString());
        Assert.AreEqual("F198", seq.NextAsString());
        Assert.AreEqual("8FCB", seq.NextAsString());
        Assert.AreEqual("2DFE", seq.NextAsString());
        Assert.AreEqual("CC31", seq.NextAsString());
        Assert.AreEqual("6A64", seq.NextAsString());
        Assert.AreEqual("0897", seq.NextAsString());
    }
    [TestMethod]
    public void SpreadShortSequence_AsStringWithPrefix() {
        var seq = new SpreadShortSequence(0);
        Assert.AreEqual("C-9E33", seq.NextAsString("C-"));
        Assert.AreEqual("C-3C66", seq.NextAsString("C-"));
        Assert.AreEqual("C-DA99", seq.NextAsString("C-"));
        Assert.AreEqual("C-78CC", seq.NextAsString("C-"));
        Assert.AreEqual("C-16FF", seq.NextAsString("C-"));
        Assert.AreEqual("C-B532", seq.NextAsString("C-"));
        Assert.AreEqual("C-5365", seq.NextAsString("C-"));
        Assert.AreEqual("C-F198", seq.NextAsString("C-"));
        Assert.AreEqual("C-8FCB", seq.NextAsString("C-"));
        Assert.AreEqual("C-2DFE", seq.NextAsString("C-"));
        Assert.AreEqual("C-CC31", seq.NextAsString("C-"));
        Assert.AreEqual("C-6A64", seq.NextAsString("C-"));
        Assert.AreEqual("C-0897", seq.NextAsString("C-"));
    }

    [TestMethod]
    public void SpreadShortSequence_NonRepeat() {
        var seq = new SpreadShortSequence();
        var memory = new byte[65536];
        for (var i = 0; i < 65536; i++) {
            var value = seq.Next();
            memory[value]++;
        }
        foreach (var mem in memory) {
            Assert.AreEqual(1, mem);
        }
    }

    [TestMethod]
    public void SpreadShortSequence_Multithreaded() {
        var seq = new SpreadShortSequence();
        var memory = new ConcurrentDictionary<ushort, int>();
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
