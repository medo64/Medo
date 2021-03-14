using System;
using System.Windows.Forms;
using Medo.Diagnostics;

namespace Medo.Windows.Forms.Samples {
    static class App {
        [STAThread]
        static void Main() {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            UnhandledCatch.UnhandledException += (sender, e) => {
                MessageBox.ShowError(null, e.Exception.Message, "Unhandled!");
            };
            UnhandledCatch.Attach();

            Application.Run(new MainForm());
        }
    }
}
