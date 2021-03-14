using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Medo.Windows.Forms.Samples {
    internal partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private void btnMessageBoxInformation_Click(object sender, EventArgs e) {
            MessageBox.ShowInformation(this, "Some information.");
        }

        private void btnMessageBoxWarning_Click(object sender, EventArgs e) {
            MessageBox.ShowWarning(this, "A warning!");
        }

        private void btnMessageBoxError_Click(object sender, EventArgs e) {
            MessageBox.ShowError(this, "A grave error!");
        }

        private void btnMessageBoxQuestion_Click(object sender, EventArgs e) {
            switch (MessageBox.ShowQuestion(this, "Are you sure?", MessageBoxButtons.YesNo)) {
                case DialogResult.Yes:
                    MessageBox.ShowQuestion(this, "How come?", "Answer?");
                    break;
                case DialogResult.No:
                    MessageBox.ShowQuestion(this, "Why now?", "Answer?");
                    break;
            }
        }

        private void btnUnhandledCatch_Click(object sender, EventArgs e) {
            var x = 0;
            var y = (int)0.1;
            _ = x / y;
        }

        private void btnUnhandledCatchBackground_Click(object sender, EventArgs e) {
            bwUnhandledCatchBackground.RunWorkerAsync();
        }

        private void btnUnhandledCatchTask_Click(object sender, EventArgs e) {
            Task.Run(() => {
                var x = 0;
                var y = (int)0.1;
                _ = x / y;
            }).Wait();
        }

        private void bwUnhandledCatchWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            var x = 0;
            var y = (int)0.1;
            _ = x / y;
        }

        private void bwUnhandledCatchBackground_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            _ = e.Result;
        }

        private void btnAboutBox_Click(object sender, EventArgs e) {
            AboutBox.ShowDialog(this, new Uri("https://medo64.com"));
        }

        private void btnTps_Click(object sender, EventArgs e) {
            using var frm = new TpsForm();
            frm.ShowDialog(this);
        }

        private void btnTimerResolution_Click(object sender, EventArgs e) {
            using var frm = new TimerResolutionForm();
            frm.ShowDialog(this);
        }

    }
}
