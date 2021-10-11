using System;
using Medo.IO;

namespace Medo.Examples {
    internal static class TerminalExample {

        public static void RunAnsi() {
            Run(Terminal.AnsiConsole);
        }

        public static void RunConsole() {
            Run(Terminal.Console);
        }

        public static void Run(Terminal terminal) {
            terminal.Clear();
            terminal.WriteLine(" Terminal ", ConsoleColor.Yellow, ConsoleColor.DarkGray);
            terminal.WriteLine();

            terminal.WriteLine("Second line comes in default color");
            terminal.WriteLine("However, you can also set both background and foreground if you wish", ConsoleColor.DarkBlue, ConsoleColor.Red);

            terminal.Invert().WriteLine("One can invert the colors too").ResetInvert();
            terminal.Write("Or make them ").Bold().WriteLine("bold").ResetBold();
            terminal.Write("Ansi can do ").Underline().Write("underline").ResetUnderline().WriteLine(" too, but the standard Console ignores it");
            terminal.Write("Of course, color can be ").Foreground(ConsoleColor.Green).Write("changed ").ResetForeground().Write("inline", ConsoleColor.Black, ConsoleColor.White).WriteLine(" even in compatibility mode");

            terminal.MoveRight(4).Write("One can also skip some things").MoveLeft(6).WriteLine("characters");
            terminal.Write("Writing over ").MoveDown().Write("different").MoveUp().WriteLine(" lines is fun");

            terminal.Write("Storing cursor is also ").StoreCursor().Write("possible").RestoreCursor().WriteLine("supported");

            terminal.MoveTo(3, 12).Write("One can write at any location").MoveTo(40, 0).Write("X").MoveLeft().MoveTo(0, 11).Write("Y").WriteLine().WriteLine();

            terminal.WriteLine();
            terminal.Reset();

            while (true) {
                var ch = Terminal.ReadChar();
                var text = (int)ch switch {
                    0 => "<NULL>",
                    1 => "<SOH>",
                    2 => "<STX>",
                    3 => "<ETX>",
                    4 => "<EOT>",
                    5 => "<ENQ>",
                    6 => "<ACK>",
                    7 => "<BEL>",
                    8 => "<BS>",
                    9 => "<TAB>",
                    10 => "<LF>",
                    11 => "<VT>",
                    12 => "<FF>",
                    13 => "<CR>",
                    14 => "<SO>",
                    15 => "<SI>",
                    16 => "<DLE>",
                    17 => "<DC1>",
                    18 => "<DC2>",
                    19 => "<DC3>",
                    20 => "<DC4>",
                    21 => "<NAK>",
                    22 => "<SYN>",
                    23 => "<ETB>",
                    24 => "<CAN>",
                    25 => "<EM>",
                    26 => "<SUB>",
                    27 => "<ESC>",
                    28 => "<FS>",
                    29 => "<GS>",
                    30 => "<RS>",
                    31 => "<US>",
                    _ => ch.ToString(),
                };
                Console.Write(text);
                if (ch == '\x1B') { break; }
            }
        }

    }
}
