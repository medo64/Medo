using Xunit;
using Medo.Diagnostics;

namespace Tests.Medo.Diagnostics;

public class SpreadByteSequenceTests {

    [Fact(DisplayName = "SpreadByteSequence: AsNumber")]
    public void AsNumber() {
        var seq = new SpreadByteSequence(0);
        Assert.Equal(0x9D, seq.Next());
        Assert.Equal(0x3A, seq.Next());
        Assert.Equal(0xD7, seq.Next());
        Assert.Equal(0x74, seq.Next());
        Assert.Equal(0x11, seq.Next());
        Assert.Equal(0xAE, seq.Next());
        Assert.Equal(0x4B, seq.Next());
        Assert.Equal(0xE8, seq.Next());
        Assert.Equal(0x85, seq.Next());
        Assert.Equal(0x22, seq.Next());
        Assert.Equal(0xBF, seq.Next());
        Assert.Equal(0x5C, seq.Next());
        Assert.Equal(0xF9, seq.Next());
    }

    [Fact(DisplayName = "SpreadByteSequence: AsString")]
    public void AsString() {
        var seq = new SpreadByteSequence(0);
        Assert.Equal("9D", seq.NextAsString());
        Assert.Equal("3A", seq.NextAsString());
        Assert.Equal("D7", seq.NextAsString());
        Assert.Equal("74", seq.NextAsString());
        Assert.Equal("11", seq.NextAsString());
        Assert.Equal("AE", seq.NextAsString());
        Assert.Equal("4B", seq.NextAsString());
        Assert.Equal("E8", seq.NextAsString());
        Assert.Equal("85", seq.NextAsString());
        Assert.Equal("22", seq.NextAsString());
        Assert.Equal("BF", seq.NextAsString());
        Assert.Equal("5C", seq.NextAsString());
        Assert.Equal("F9", seq.NextAsString());
    }
    [Fact(DisplayName = "SpreadByteSequence: AsStringWithPrefix")]
    public void AsStringWithPrefix() {
        var seq = new SpreadByteSequence(0);
        Assert.Equal("C-9D", seq.NextAsString("C-"));
        Assert.Equal("C-3A", seq.NextAsString("C-"));
        Assert.Equal("C-D7", seq.NextAsString("C-"));
        Assert.Equal("C-74", seq.NextAsString("C-"));
        Assert.Equal("C-11", seq.NextAsString("C-"));
        Assert.Equal("C-AE", seq.NextAsString("C-"));
        Assert.Equal("C-4B", seq.NextAsString("C-"));
        Assert.Equal("C-E8", seq.NextAsString("C-"));
        Assert.Equal("C-85", seq.NextAsString("C-"));
        Assert.Equal("C-22", seq.NextAsString("C-"));
        Assert.Equal("C-BF", seq.NextAsString("C-"));
        Assert.Equal("C-5C", seq.NextAsString("C-"));
        Assert.Equal("C-F9", seq.NextAsString("C-"));
    }

    [Fact(DisplayName = "SpreadByteSequence: NonRepeat")]
    public void NonRepeat() {
        var seq = new SpreadByteSequence();
        var memory = new byte[256];
        for (var i = 0; i < 256; i++) {
            var value = seq.Next();
            memory[value]++;
            Assert.Equal(1, memory[value]);
        }
    }

}
