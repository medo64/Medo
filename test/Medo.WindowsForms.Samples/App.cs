using System;
using System.Threading;
using System.Windows.Forms;

namespace Medo.Windows.Forms.Samples {
    static class App {
        [STAThread]
        static void Main() {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            UnhandledCatch.ThreadException += delegate (object sender, ThreadExceptionEventArgs e) {
                MessageBox.ShowError(null, e.Exception.Message, "Unhandled!");
            };
            UnhandledCatch.Attach();

            Application.Run(new MainForm());
        }
    }
}
