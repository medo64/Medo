using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Diagnostics;

namespace Tests;

[TestClass]
public class SpreadByteSequence_Tests {

    [TestMethod]
    public void SpreadByteSequence_AsNumber() {
        var seq = new SpreadByteSequence(0);
        Assert.AreEqual(0x9D, seq.Next());
        Assert.AreEqual(0x3A, seq.Next());
        Assert.AreEqual(0xD7, seq.Next());
        Assert.AreEqual(0x74, seq.Next());
        Assert.AreEqual(0x11, seq.Next());
        Assert.AreEqual(0xAE, seq.Next());
        Assert.AreEqual(0x4B, seq.Next());
        Assert.AreEqual(0xE8, seq.Next());
        Assert.AreEqual(0x85, seq.Next());
        Assert.AreEqual(0x22, seq.Next());
        Assert.AreEqual(0xBF, seq.Next());
        Assert.AreEqual(0x5C, seq.Next());
        Assert.AreEqual(0xF9, seq.Next());
    }

    [TestMethod]
    public void SpreadByteSequence_AsString() {
        var seq = new SpreadByteSequence(0);
        Assert.AreEqual("9D", seq.NextAsString());
        Assert.AreEqual("3A", seq.NextAsString());
        Assert.AreEqual("D7", seq.NextAsString());
        Assert.AreEqual("74", seq.NextAsString());
        Assert.AreEqual("11", seq.NextAsString());
        Assert.AreEqual("AE", seq.NextAsString());
        Assert.AreEqual("4B", seq.NextAsString());
        Assert.AreEqual("E8", seq.NextAsString());
        Assert.AreEqual("85", seq.NextAsString());
        Assert.AreEqual("22", seq.NextAsString());
        Assert.AreEqual("BF", seq.NextAsString());
        Assert.AreEqual("5C", seq.NextAsString());
        Assert.AreEqual("F9", seq.NextAsString());
    }

    [TestMethod]
    public void SpreadByteSequence_AsStringWithPrefix() {
        var seq = new SpreadByteSequence(0);
        Assert.AreEqual("C-9D", seq.NextAsString("C-"));
        Assert.AreEqual("C-3A", seq.NextAsString("C-"));
        Assert.AreEqual("C-D7", seq.NextAsString("C-"));
        Assert.AreEqual("C-74", seq.NextAsString("C-"));
        Assert.AreEqual("C-11", seq.NextAsString("C-"));
        Assert.AreEqual("C-AE", seq.NextAsString("C-"));
        Assert.AreEqual("C-4B", seq.NextAsString("C-"));
        Assert.AreEqual("C-E8", seq.NextAsString("C-"));
        Assert.AreEqual("C-85", seq.NextAsString("C-"));
        Assert.AreEqual("C-22", seq.NextAsString("C-"));
        Assert.AreEqual("C-BF", seq.NextAsString("C-"));
        Assert.AreEqual("C-5C", seq.NextAsString("C-"));
        Assert.AreEqual("C-F9", seq.NextAsString("C-"));
    }

    [TestMethod]
    public void SpreadByteSequence_NonRepeat() {
        var seq = new SpreadByteSequence();
        var memory = new byte[256];
        for (var i = 0; i < 256; i++) {
            var value = seq.Next();
            memory[value]++;
            Assert.AreEqual(1, memory[value]);
        }
    }

    [TestMethod]
    public void SpreadByteSequence_NonRepeat2() {
        var seq = new SpreadByteSequence();
        var memory = new byte[256];
        for (var i = 0; i < 10 * 256; i++) {
            var value = seq.Next();
            memory[value]++;
        }
        foreach (var mem in memory) {
            Assert.AreEqual(10, mem);
        }
    }

    [TestMethod]
    public void SpreadByteSequence_Multithreaded() {
        var seq = new SpreadByteSequence();
        var memory = new byte[256];
        var tasks = new Task[16];
        for (var t = 0;t < tasks.Length; t++){
            tasks[t]= Task.Run(delegate {
                for (var i = 0; i < 256; i++) {
                    memory[seq.Next()] += 1;
                }
            });
        }
        foreach (var task in tasks) {
            task.Wait();
        }

        foreach (var mem in memory) {
            Assert.AreEqual(16, mem);
        }
    }

}
