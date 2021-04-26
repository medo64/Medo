using System;

// dotnet publish "test/Check.Args" --force -c Release -o build/args/ -p:Deterministic=true -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true -r linux-x64
//   build/args/Check.Args
// dotnet publish "test/Check.Args" --force -c Release -o build/args/ -p:Deterministic=true -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true -r win-x64
//   build/args/Check.Args.exe
namespace Check.Args {
    internal class App {

        private static void Main(string[] args) {
            WriteHeader("args[]");
            for (var i = 0; i < args.Length; i++) {
                WriteEntry(args[i], i);
            }

            Console.WriteLine();

            var envArgs = Environment.GetCommandLineArgs();
            WriteHeader("Environment.GetCommandLineArgs[]");
            for (var i = 0; i < envArgs.Length; i++) {
                WriteEntry(envArgs[i], i);
            }

            Console.WriteLine();

            WriteHeader("Environment.CommandLine");
            WriteEntry(Environment.CommandLine);
        }

        private static void WriteHeader(string text) {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(text + ":");
            Console.ResetColor();
        }

        private static void WriteEntry(string text, int index = -1) {
            if (index > -1) {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(index.ToString() + ":");
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ResetColor();
        }

    }
}
