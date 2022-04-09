using Xunit;
using Medo.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Tests.Medo.Diagnostics;

public class SpreadIntSequenceTests {

    [Fact(DisplayName = "SpreadIntSequence: AsNumber")]
    public void AsNumber() {
        var seq = new SpreadIntSequence(0);
        Assert.Equal(0x9E3779B1U, seq.Next());
        Assert.Equal(0x3C6EF362U, seq.Next());
        Assert.Equal(0xDAA66D13U, seq.Next());
        Assert.Equal(0x78DDE6C4U, seq.Next());
        Assert.Equal(0x17156075U, seq.Next());
        Assert.Equal(0xB54CDA26U, seq.Next());
        Assert.Equal(0x538453D7U, seq.Next());
        Assert.Equal(0xF1BBCD88U, seq.Next());
        Assert.Equal(0x8FF34739U, seq.Next());
        Assert.Equal(0x2E2AC0EAU, seq.Next());
        Assert.Equal(0xCC623A9BU, seq.Next());
        Assert.Equal(0x6A99B44CU, seq.Next());
        Assert.Equal(0x08D12DFDU, seq.Next());
    }

    [Fact(DisplayName = "SpreadIntSequence: AsString")]
    public void AsString() {
        var seq = new SpreadIntSequence(0);
        Assert.Equal("9E3779B1", seq.NextAsString());
        Assert.Equal("3C6EF362", seq.NextAsString());
        Assert.Equal("DAA66D13", seq.NextAsString());
        Assert.Equal("78DDE6C4", seq.NextAsString());
        Assert.Equal("17156075", seq.NextAsString());
        Assert.Equal("B54CDA26", seq.NextAsString());
        Assert.Equal("538453D7", seq.NextAsString());
        Assert.Equal("F1BBCD88", seq.NextAsString());
        Assert.Equal("8FF34739", seq.NextAsString());
        Assert.Equal("2E2AC0EA", seq.NextAsString());
        Assert.Equal("CC623A9B", seq.NextAsString());
        Assert.Equal("6A99B44C", seq.NextAsString());
        Assert.Equal("08D12DFD", seq.NextAsString());
    }
    [Fact(DisplayName = "SpreadIntSequence: AsStringWithPrefix")]
    public void AsStringWithPrefix() {
        var seq = new SpreadIntSequence(0);
        Assert.Equal("C-9E3779B1", seq.NextAsString("C-"));
        Assert.Equal("C-3C6EF362", seq.NextAsString("C-"));
        Assert.Equal("C-DAA66D13", seq.NextAsString("C-"));
        Assert.Equal("C-78DDE6C4", seq.NextAsString("C-"));
        Assert.Equal("C-17156075", seq.NextAsString("C-"));
        Assert.Equal("C-B54CDA26", seq.NextAsString("C-"));
        Assert.Equal("C-538453D7", seq.NextAsString("C-"));
        Assert.Equal("C-F1BBCD88", seq.NextAsString("C-"));
        Assert.Equal("C-8FF34739", seq.NextAsString("C-"));
        Assert.Equal("C-2E2AC0EA", seq.NextAsString("C-"));
        Assert.Equal("C-CC623A9B", seq.NextAsString("C-"));
        Assert.Equal("C-6A99B44C", seq.NextAsString("C-"));
        Assert.Equal("C-08D12DFD", seq.NextAsString("C-"));
    }

    [Fact(DisplayName = "SpreadIntSequence: Multithreaded")]
    public void Multithreaded() {
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
            Assert.Equal(1, mem.Value);
        }
    }

}
