using System;

namespace Medo.Examples {
    internal class App {

        private static void Main() {
            while (true) {
                Console.Clear();
                Console.WriteLine("<T> Throttling");

                switch (Console.ReadKey(intercept: true).Key) {
                    case ConsoleKey.Escape: Environment.Exit(0); break;
                    case ConsoleKey.T: Throttling.Run(); break;
                }
            }
        }

    }
}
