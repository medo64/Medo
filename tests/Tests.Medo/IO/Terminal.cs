using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.IO;

namespace Tests;

[TestClass]
public class Terminal_Tests {

    [TestMethod]
    public void Terminal_BasicColor() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.Blue().Write("---").NoColor().Write("===");
        Assert.AreEqual("\x1B[94m---\x1B[39m===", Encoding.UTF8.GetString(memory.ToArray()));
    }

    [TestMethod]
    public void Terminal_ColorForeDark() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.Foreground(ConsoleColor.DarkRed).Write("---").NoForeground().Write("===");
        Assert.AreEqual("\x1B[31m---\x1B[39m===", Encoding.UTF8.GetString(memory.ToArray()));
    }

    [TestMethod]
    public void Terminal_ColorForeBright() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.Foreground(ConsoleColor.Green).Write("---").NoForeground().Write("===");
        Assert.AreEqual("\x1B[92m---\x1B[39m===", Encoding.UTF8.GetString(memory.ToArray()));
    }

    [TestMethod]
    public void Terminal_ColorBackDark() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.Background(ConsoleColor.DarkYellow).Write("---").NoBackground().Write("===");
        Assert.AreEqual("\x1B[43m---\x1B[49m===", Encoding.UTF8.GetString(memory.ToArray()));
    }

    [TestMethod]
    public void Terminal_ColorBackBright() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.Background(ConsoleColor.Cyan).Write("---").NoBackground().Write("===");
        Assert.AreEqual("\x1B[106m---\x1B[49m===", Encoding.UTF8.GetString(memory.ToArray()));
    }

    [TestMethod]
    public void Terminal_Clear() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.Clear();
        Assert.AreEqual("\x1B[2J\x1B[H", Encoding.UTF8.GetString(memory.ToArray()));
    }

    [TestMethod]
    public void Terminal_Reset() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.Reset();
        Assert.AreEqual("\x1B[0m", Encoding.UTF8.GetString(memory.ToArray()));
    }


    [TestMethod]
    public void Terminal_Bold() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.Bold().Write("---").NoBold().Write("===");
        Assert.AreEqual("\x1B[1m---\x1B[22m===", Encoding.UTF8.GetString(memory.ToArray()));
    }

    [TestMethod]
    public void Terminal_Underline() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.Underline().Write("---").NoUnderline().Write("===");
        Assert.AreEqual("\x1B[4m---\x1B[24m===", Encoding.UTF8.GetString(memory.ToArray()));
    }

    [TestMethod]
    public void Terminal_Invert() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.Invert().Write("---").NoInvert().Write("===");
        Assert.AreEqual("\x1B[7m---\x1B[27m===", Encoding.UTF8.GetString(memory.ToArray()));
    }


    [TestMethod]
    public void Terminal_MoveLeft() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.MoveLeft().Write("-").MoveLeft(2);
        Assert.AreEqual("\x1B[1D-\x1B[2D", Encoding.UTF8.GetString(memory.ToArray()));
    }

    [TestMethod]
    public void Terminal_MoveRight() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.MoveRight().Write("-").MoveRight(2);
        Assert.AreEqual("\x1B[1C-\x1B[2C", Encoding.UTF8.GetString(memory.ToArray()));
    }

    [TestMethod]
    public void Terminal_MoveUp() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.MoveUp().Write("-").MoveUp(2);
        Assert.AreEqual("\x1B[1A-\x1B[2A", Encoding.UTF8.GetString(memory.ToArray()));
    }

    [TestMethod]
    public void Terminal_MoveDown() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.MoveDown().Write("-").MoveDown(2);
        Assert.AreEqual("\x1B[1B-\x1B[2B", Encoding.UTF8.GetString(memory.ToArray()));
    }
    [TestMethod]
    public void Terminal_MoveAbsolute() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.Write("-").MoveTo(42, 223).Write("-").MoveTo(420, 0).Write("-").MoveTo(0, 420).Write("-");
        Assert.AreEqual("-\x1B[223;42H-\x1B[420G-\x1B[420d-", Encoding.UTF8.GetString(memory.ToArray()));
    }


    [TestMethod]
    public void Terminal_CursorStore() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.Write("-").StoreCursor().Write("-").RestoreCursor().Write("-");
        Assert.AreEqual("-\x1B[s-\x1B[u-", Encoding.UTF8.GetString(memory.ToArray()));
    }


    [TestMethod]
    public void Terminal_SuppressOutput() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.Foreground(ConsoleColor.DarkRed).Write("---");
        Terminal.SuppressAttributes = true;
        Terminal.NoForeground().Write("===");
        Terminal.SuppressAttributes = false;
        Terminal.Clear();
        Assert.AreEqual("\x1B[31m---===\x1B[2J\x1B[H", Encoding.UTF8.GetString(memory.ToArray()));
    }


    [TestMethod]
    public void Terminal_ColorWrite() {
        var memory = new MemoryStream();
        Terminal.Setup(memory);
        Terminal.Write("---", ConsoleColor.Yellow, ConsoleColor.DarkBlue);
        Terminal.Write("===", ConsoleColor.Magenta);
        Assert.AreEqual("\x1B[93m\x1B[44m---\x1B[49m\x1B[39m\x1B[95m===\x1B[39m", Encoding.UTF8.GetString(memory.ToArray()));
    }

}
