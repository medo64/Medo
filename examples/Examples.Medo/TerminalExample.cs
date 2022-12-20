using System;
using Medo.IO;

namespace Examples {
    internal static class TerminalExample {

        public static void RunAnsi() {
            Terminal.Setup();
            Run();
        }

        public static void RunConsole() {
            Terminal.SetupPlain();
            Run();
        }

        public static void Run() {
            Terminal.Clear();
            Terminal.WriteLine(" Terminal ", ConsoleColor.Yellow, ConsoleColor.DarkGray);
            Terminal.WriteLine();

            Terminal.WriteLine("Second line comes in default color");
            Terminal.WriteLine("However, you can also set both background and foreground if you wish", ConsoleColor.DarkBlue, ConsoleColor.Red);

            Terminal.Invert().WriteLine("One can invert the colors too").NoInvert();
            Terminal.Write("Or make them ").Bold().WriteLine("bold").NoBold();
            Terminal.Write("Ansi can do ").Underline().Write("underline").NoUnderline().WriteLine(" too, but the standard console ignores it");
            Terminal.Write("Of course, color can be ").Green().Write("changed ").NoColor().Write("inline", ConsoleColor.Black, ConsoleColor.White).WriteLine(" even in compatibility mode");

            Terminal.MoveRight(4).Write("One can also skip some things").MoveLeft(6).WriteLine("characters");
            Terminal.Write("Writing over ").MoveDown().Write("different").MoveUp().WriteLine(" lines is fun");
            Terminal.WriteLine();

            Terminal.Write("Storing cursor is also ").StoreCursor().Write("possible").RestoreCursor().WriteLine("supported");

            Terminal.MoveTo(3, 14).Write("One can write at any location").MoveTo(40, 0).Write("X").MoveLeft().MoveTo(0, 13).Write("Y").WriteLine().WriteLine();

            Terminal.WriteLine();
            Terminal.Reset();

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
                Terminal.Write(text);
                if (ch == '\x1B') { break; }
            }
        }

    }
}
