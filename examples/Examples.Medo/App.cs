using System;
using Medo.IO;

namespace Medo.Examples {
    internal class App {

        private static void Main() {
            while (true) {
                Terminal.Clear();
                Terminal.Cyan().Write("<A>").NoColor().WriteLine(" ANSI Terminal");
                Terminal.Cyan().Write("<C>").NoColor().WriteLine(" Console Terminal");
                Terminal.Cyan().Write("<T>").NoColor().WriteLine(" Throttling");

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
