using System;
using System.Windows.Forms;
using Medo.Diagnostics;
using Medo.Windows.Forms;

namespace Examples {
    internal static class App {
        [STAThread]
        private static void Main() {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            UnhandledCatch.UnhandledException += (sender, e) => {
                MsgBox.ShowError(null, e.Exception.Message, "Unhandled!");
            };
            UnhandledCatch.Attach();

            Application.Run(new MainForm());
        }
    }
}
