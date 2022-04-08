using Xunit;
using Medo.Diagnostics;

namespace Tests.Medo.Diagnostics;

public class SpreadShortSequenceTests {

    [Fact(DisplayName = "SpreadShortSequence: AsNumber")]
    public void AsNumber() {
        var seq = new SpreadShortSequence(0);
        Assert.Equal(0x9E33, seq.Next());
        Assert.Equal(0x3C66, seq.Next());
        Assert.Equal(0xDA99, seq.Next());
        Assert.Equal(0x78CC, seq.Next());
        Assert.Equal(0x16FF, seq.Next());
        Assert.Equal(0xB532, seq.Next());
        Assert.Equal(0x5365, seq.Next());
        Assert.Equal(0xF198, seq.Next());
        Assert.Equal(0x8FCB, seq.Next());
        Assert.Equal(0x2DFE, seq.Next());
        Assert.Equal(0xCC31, seq.Next());
        Assert.Equal(0x6A64, seq.Next());
        Assert.Equal(0x0897, seq.Next());
    }

    [Fact(DisplayName = "SpreadShortSequence: AsString")]
    public void AsString() {
        var seq = new SpreadShortSequence(0);
        Assert.Equal("9E33", seq.NextAsString());
        Assert.Equal("3C66", seq.NextAsString());
        Assert.Equal("DA99", seq.NextAsString());
        Assert.Equal("78CC", seq.NextAsString());
        Assert.Equal("16FF", seq.NextAsString());
        Assert.Equal("B532", seq.NextAsString());
        Assert.Equal("5365", seq.NextAsString());
        Assert.Equal("F198", seq.NextAsString());
        Assert.Equal("8FCB", seq.NextAsString());
        Assert.Equal("2DFE", seq.NextAsString());
        Assert.Equal("CC31", seq.NextAsString());
        Assert.Equal("6A64", seq.NextAsString());
        Assert.Equal("0897", seq.NextAsString());
    }
    [Fact(DisplayName = "SpreadShortSequence: AsStringWithPrefix")]
    public void AsStringWithPrefix() {
        var seq = new SpreadShortSequence(0);
        Assert.Equal("C-9E33", seq.NextAsString("C-"));
        Assert.Equal("C-3C66", seq.NextAsString("C-"));
        Assert.Equal("C-DA99", seq.NextAsString("C-"));
        Assert.Equal("C-78CC", seq.NextAsString("C-"));
        Assert.Equal("C-16FF", seq.NextAsString("C-"));
        Assert.Equal("C-B532", seq.NextAsString("C-"));
        Assert.Equal("C-5365", seq.NextAsString("C-"));
        Assert.Equal("C-F198", seq.NextAsString("C-"));
        Assert.Equal("C-8FCB", seq.NextAsString("C-"));
        Assert.Equal("C-2DFE", seq.NextAsString("C-"));
        Assert.Equal("C-CC31", seq.NextAsString("C-"));
        Assert.Equal("C-6A64", seq.NextAsString("C-"));
        Assert.Equal("C-0897", seq.NextAsString("C-"));
    }

    [Fact(DisplayName = "SpreadShortSequence: NonRepeat")]
    public void NonRepeat() {
        var seq = new SpreadShortSequence();
        var memory = new byte[65536];
        for (var i = 0; i < 65536; i++) {
            var value = seq.Next();
            memory[value]++;
            Assert.Equal(1, memory[value]);
        }
    }

}
