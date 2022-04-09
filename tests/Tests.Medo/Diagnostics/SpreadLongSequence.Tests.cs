using Xunit;
using Medo.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Tests.Medo.Diagnostics;

public class SpreadLongSequenceTests {

    [Fact(DisplayName = "SpreadLongSequence: AsNumber")]
    public void AsNumber() {
        var seq = new SpreadLongSequence(0);
        Assert.Equal(0x9E3779B97F4A936DUL, seq.Next());
        Assert.Equal(0x3C6EF372FE9526DAUL, seq.Next());
        Assert.Equal(0xDAA66D2C7DDFBA47UL, seq.Next());
        Assert.Equal(0x78DDE6E5FD2A4DB4UL, seq.Next());
        Assert.Equal(0x1715609F7C74E121UL, seq.Next());
        Assert.Equal(0xB54CDA58FBBF748EUL, seq.Next());
        Assert.Equal(0x538454127B0A07FBUL, seq.Next());
        Assert.Equal(0xF1BBCDCBFA549B68UL, seq.Next());
        Assert.Equal(0x8FF34785799F2ED5UL, seq.Next());
        Assert.Equal(0x2E2AC13EF8E9C242UL, seq.Next());
        Assert.Equal(0xCC623AF8783455AFUL, seq.Next());
        Assert.Equal(0x6A99B4B1F77EE91CUL, seq.Next());
        Assert.Equal(0x08D12E6B76C97C89UL, seq.Next());
    }

    [Fact(DisplayName = "SpreadLongSequence: AsString")]
    public void AsString() {
        var seq = new SpreadLongSequence(0);
        Assert.Equal("9E3779B97F4A936D", seq.NextAsString());
        Assert.Equal("3C6EF372FE9526DA", seq.NextAsString());
        Assert.Equal("DAA66D2C7DDFBA47", seq.NextAsString());
        Assert.Equal("78DDE6E5FD2A4DB4", seq.NextAsString());
        Assert.Equal("1715609F7C74E121", seq.NextAsString());
        Assert.Equal("B54CDA58FBBF748E", seq.NextAsString());
        Assert.Equal("538454127B0A07FB", seq.NextAsString());
        Assert.Equal("F1BBCDCBFA549B68", seq.NextAsString());
        Assert.Equal("8FF34785799F2ED5", seq.NextAsString());
        Assert.Equal("2E2AC13EF8E9C242", seq.NextAsString());
        Assert.Equal("CC623AF8783455AF", seq.NextAsString());
        Assert.Equal("6A99B4B1F77EE91C", seq.NextAsString());
        Assert.Equal("08D12E6B76C97C89", seq.NextAsString());
    }
    [Fact(DisplayName = "SpreadLongSequence: AsStringWithPrefix")]
    public void AsStringWithPrefix() {
        var seq = new SpreadLongSequence(0);
        Assert.Equal("C-9E3779B97F4A936D", seq.NextAsString("C-"));
        Assert.Equal("C-3C6EF372FE9526DA", seq.NextAsString("C-"));
        Assert.Equal("C-DAA66D2C7DDFBA47", seq.NextAsString("C-"));
        Assert.Equal("C-78DDE6E5FD2A4DB4", seq.NextAsString("C-"));
        Assert.Equal("C-1715609F7C74E121", seq.NextAsString("C-"));
        Assert.Equal("C-B54CDA58FBBF748E", seq.NextAsString("C-"));
        Assert.Equal("C-538454127B0A07FB", seq.NextAsString("C-"));
        Assert.Equal("C-F1BBCDCBFA549B68", seq.NextAsString("C-"));
        Assert.Equal("C-8FF34785799F2ED5", seq.NextAsString("C-"));
        Assert.Equal("C-2E2AC13EF8E9C242", seq.NextAsString("C-"));
        Assert.Equal("C-CC623AF8783455AF", seq.NextAsString("C-"));
        Assert.Equal("C-6A99B4B1F77EE91C", seq.NextAsString("C-"));
        Assert.Equal("C-08D12E6B76C97C89", seq.NextAsString("C-"));
    }

    [Fact(DisplayName = "SpreadLongSequence: Multithreaded")]
    public void Multithreaded() {
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
            Assert.Equal(1, mem.Value);
        }
    }

}
