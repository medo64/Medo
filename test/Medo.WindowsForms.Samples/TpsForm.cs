using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Medo.Timers;

namespace Medo.Windows.Forms.Samples {
    public partial class TpsForm : Form {
        public TpsForm() {
            InitializeComponent();
        }

        public PerSecondCounter TpsCounter = new();
        public PerSecondLimiter TpsLimiter = new(10);
        public long IncreasingValue = 0;

        private void tmrUpdateDisplay_Tick(object sender, EventArgs e) {
            txtMeasured.Text = TpsCounter.ValuePerSecond.ToString();
            lblIncrementing.Text = IncreasingValue.ToString();
        }

        private void tmrConsume_Tick(object sender, EventArgs e) {
            while (TpsLimiter.IsReadyForNext()) {
                IncreasingValue += 1;
                TpsCounter.Increment();
            }
        }

        private void btnSetTps_Click(object sender, EventArgs e) {
            TpsLimiter = new PerSecondLimiter((int)nudTps.Value);
        }
    }
}
