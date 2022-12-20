using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Medo.Timers;

namespace Examples {
    public partial class TpsForm : Form {
        public TpsForm() {
            InitializeComponent();
            bwGenerator.RunWorkerAsync(CancelSource.Token);
        }

        public PerSecondCounter TpsCounter = new();
        public PerSecondLimiter TpsLimiter = new(10);
        public CancellationTokenSource CancelSource = new();
        public long IncreasingValue = 0;

        private void Form_FormClosed(object sender, FormClosedEventArgs e) {
            CancelSource.Cancel();
        }


        private void tmrUpdateDisplay_Tick(object sender, EventArgs e) {
            txtMeasured.Text = TpsCounter.ValuePerSecond.ToString();
            lblIncrementing.Text = Interlocked.Read(ref IncreasingValue).ToString();
        }

        private void bwGenerator_DoWork(object sender, DoWorkEventArgs e) {
            var cancelToken = (CancellationToken)e.Argument;
            while (!cancelToken.IsCancellationRequested) {
                if (TpsLimiter.Wait(cancelToken)) {
                    TpsCounter.Increment(1);
                    Interlocked.Increment(ref IncreasingValue);
                }
            }
        }

        private void btnSetTps_Click(object sender, EventArgs e) {
            TpsLimiter.PerSecondRate = (int)nudTps.Value;
        }

    }
}
