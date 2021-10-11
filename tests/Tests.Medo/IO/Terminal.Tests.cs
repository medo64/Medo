using System;
using System.IO;
using System.Text;
using Xunit;
using Medo.IO;

namespace Tests.Medo.IO {
    public class TerminalTests {

        [Fact(DisplayName = "Terminal: Basic Color")]
        public void BasicColor() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.Blue().Write("---").NoColor().Write("===");
            Assert.Equal("\x1B[94m---\x1B[39m===", Encoding.UTF8.GetString(memory.ToArray()));
        }

        [Fact(DisplayName = "Terminal: Foreground Dark Color")]
        public void ColorForeDark() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.Foreground(ConsoleColor.DarkRed).Write("---").ResetForeground().Write("===");
            Assert.Equal("\x1B[31m---\x1B[39m===", Encoding.UTF8.GetString(memory.ToArray()));
        }

        [Fact(DisplayName = "Terminal: Foreground Bright Color")]
        public void ColorForeBright() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.Foreground(ConsoleColor.Green).Write("---").ResetForeground().Write("===");
            Assert.Equal("\x1B[92m---\x1B[39m===", Encoding.UTF8.GetString(memory.ToArray()));
        }

        [Fact(DisplayName = "Terminal: Background Dark Color")]
        public void ColorBackDark() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.Background(ConsoleColor.DarkYellow).Write("---").ResetBackground().Write("===");
            Assert.Equal("\x1B[43m---\x1B[49m===", Encoding.UTF8.GetString(memory.ToArray()));
        }

        [Fact(DisplayName = "Terminal: Background Bright Color")]
        public void ColorBackBright() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.Background(ConsoleColor.Cyan).Write("---").ResetBackground().Write("===");
            Assert.Equal("\x1B[106m---\x1B[49m===", Encoding.UTF8.GetString(memory.ToArray()));
        }

        [Fact(DisplayName = "Terminal: Clear")]
        public void Clear() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.Clear();
            Assert.Equal("\x1B[2J\x1B[H", Encoding.UTF8.GetString(memory.ToArray()));
        }

        [Fact(DisplayName = "Terminal: Reset")]
        public void Reset() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.Reset();
            Assert.Equal("\x1B[0m", Encoding.UTF8.GetString(memory.ToArray()));
        }


        [Fact(DisplayName = "Terminal: Bold")]
        public void Bold() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.Bold().Write("---").ResetBold().Write("===");
            Assert.Equal("\x1B[1m---\x1B[22m===", Encoding.UTF8.GetString(memory.ToArray()));
        }

        [Fact(DisplayName = "Terminal: Underline")]
        public void Underline() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.Underline().Write("---").ResetUnderline().Write("===");
            Assert.Equal("\x1B[4m---\x1B[24m===", Encoding.UTF8.GetString(memory.ToArray()));
        }

        [Fact(DisplayName = "Terminal: Invert")]
        public void Invert() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.Invert().Write("---").ResetInvert().Write("===");
            Assert.Equal("\x1B[7m---\x1B[27m===", Encoding.UTF8.GetString(memory.ToArray()));
        }


        [Fact(DisplayName = "Terminal: Move Left")]
        public void MoveLeft() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.MoveLeft().Write("-").MoveLeft(2);
            Assert.Equal("\x1B[1D-\x1B[2D", Encoding.UTF8.GetString(memory.ToArray()));
        }

        [Fact(DisplayName = "Terminal: Move Right")]
        public void MoveRight() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.MoveRight().Write("-").MoveRight(2);
            Assert.Equal("\x1B[1C-\x1B[2C", Encoding.UTF8.GetString(memory.ToArray()));
        }

        [Fact(DisplayName = "Terminal: Move Up")]
        public void MoveUp() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.MoveUp().Write("-").MoveUp(2);
            Assert.Equal("\x1B[1A-\x1B[2A", Encoding.UTF8.GetString(memory.ToArray()));
        }

        [Fact(DisplayName = "Terminal: Move Down")]
        public void MoveDown() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.MoveDown().Write("-").MoveDown(2);
            Assert.Equal("\x1B[1B-\x1B[2B", Encoding.UTF8.GetString(memory.ToArray()));
        }

        [Fact(DisplayName = "Terminal: Move Absolute")]
        public void MoveAbsolute() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.Write("-").MoveTo(42, 223).Write("-").MoveTo(420, 0).Write("-").MoveTo(0, 420).Write("-");
            Assert.Equal("-\x1B[223;42H-\x1B[420G-\x1B[420d-", Encoding.UTF8.GetString(memory.ToArray()));
        }


        [Fact(DisplayName = "Terminal: Cursor Store")]
        public void CursorStore() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.Write("-").StoreCursor().Write("-").RestoreCursor().Write("-");
            Assert.Equal("-\x1B[s-\x1B[u-", Encoding.UTF8.GetString(memory.ToArray()));
        }


        [Fact(DisplayName = "Terminal: Suppress Output")]
        public void SuppressOutput() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.Foreground(ConsoleColor.DarkRed).Write("---");
            Terminal.SuppressAttributes = true;
            Terminal.ResetForeground().Write("===");
            Terminal.SuppressAttributes = false;
            Terminal.Clear();
            Assert.Equal("\x1B[31m---===\x1B[2J\x1B[H", Encoding.UTF8.GetString(memory.ToArray()));
        }


        [Fact(DisplayName = "Terminal: Color Write")]
        public void ColorWrite() {
            var memory = new MemoryStream();
            Terminal.Setup(memory);
            Terminal.Write("---", ConsoleColor.Yellow, ConsoleColor.DarkBlue);
            Terminal.Write("===", ConsoleColor.Magenta);
            Assert.Equal("\x1B[93m\x1B[44m---\x1B[49m\x1B[39m\x1B[95m===\x1B[39m", Encoding.UTF8.GetString(memory.ToArray()));
        }

    }
}
