using System;
using Medo.IO;

namespace Medo.Examples {
    internal class App {

        private static void Main() {
            while (true) {
                Terminal.Clear();
                Terminal.Foreground(ConsoleColor.Cyan).Write("<A>").ResetForeground().WriteLine(" ANSI Terminal");
                Terminal.Foreground(ConsoleColor.Cyan).Write("<C>").ResetForeground().WriteLine(" Console Terminal");
                Terminal.Foreground(ConsoleColor.Cyan).Write("<T>").ResetForeground().WriteLine(" Throttling");

                switch (Terminal.ReadKey()) {
                    case ConsoleKey.A: TerminalExample.RunAnsi(); break;
                    case ConsoleKey.C: TerminalExample.RunConsole(); break;
                    case ConsoleKey.T: ThrottlingExample.Run(); break;
                    case ConsoleKey.Escape: Environment.Exit(0); break;
                }
            }
        }

    }
}
