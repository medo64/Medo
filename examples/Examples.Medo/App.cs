using System;
using Medo.IO;

namespace Medo.Examples {
    internal class App {

        private static void Main() {
            var terminal = Terminal.Console;
            while (true) {
                terminal.Clear();
                terminal.Foreground(ConsoleColor.Cyan).Write("<A>").ResetForeground().WriteLine(" ANSI Terminal");
                terminal.Foreground(ConsoleColor.Cyan).Write("<C>").ResetForeground().WriteLine(" Console Terminal");
                terminal.Foreground(ConsoleColor.Cyan).Write("<T>").ResetForeground().WriteLine(" Throttling");

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
