using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Medo.Windows.Forms;

namespace Examples {
    internal partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private void btnMessageBoxInformation_Click(object sender, EventArgs e) {
            MsgBox.ShowInformation(this, "Some information.");
        }

        private void btnMessageBoxWarning_Click(object sender, EventArgs e) {
            MsgBox.ShowWarning(this, "A warning!");
        }

        private void btnMessageBoxError_Click(object sender, EventArgs e) {
            MsgBox.ShowError(this, "A grave error!");
        }

        private void btnMessageBoxQuestion_Click(object sender, EventArgs e) {
            switch (MsgBox.ShowQuestion(this, "Are you sure?", MessageBoxButtons.YesNo)) {
                case DialogResult.Yes:
                    MsgBox.ShowQuestion(this, "How come?", "Answer?");
                    break;
                case DialogResult.No:
                    MsgBox.ShowQuestion(this, "Why now?", "Answer?");
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

        private void btnWaitCursor_Click(object sender, EventArgs e) {
            using var x = new WaitCursor(this);
            Thread.Sleep(1000);
        }

        private void btnErrorReport_Click(object sender, EventArgs e) {
            ErrorReportBox.ShowDialog(this, new Uri("http://example.com/"), new InvalidOperationException("Something truly dreadful error."));
        }

        private void btnFeedback_Click(object sender, EventArgs e) {
            ErrorReportBox.ShowDialog(this, new Uri("http://example.com/"));
        }

        private void btnUpgrade_Click(object sender, EventArgs e) {
            UpgradeBox.ShowDialog(this, new Uri("http://example.com/"));
        }

        private void btnCyberCard_Click(object sender, EventArgs e) {
            using var frm = new CyberCardForm();
            frm.ShowDialog(this);
        }
    }
}
