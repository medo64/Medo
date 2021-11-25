using System.Text;
using System.Windows.Forms;

namespace Medo.Windows.Forms.Examples {
    internal partial class TimerResolutionForm : Form {
        public TimerResolutionForm() {
            InitializeComponent();
        }

        private void btnIncrease_Click(object sender, System.EventArgs e) {
            using var mmTimerRes = new TimerResolution(4);
            var sb = new StringBuilder();
            if (mmTimerRes.Successful) {
                sb.AppendLine($"Resolution (set): {mmTimerRes.DesiredResolutionInMilliseconds} ms");
            } else {
                sb.AppendLine($"Resolution (failed): {mmTimerRes.DesiredResolutionInMilliseconds} ms");
            }
            sb.AppendLine($"Resolution (current): {SystemTimerResolution.ResolutionInTicks / 10000.0} ms");
            sb.AppendLine($"Resolution (minimum): {SystemTimerResolution.MinimumResolutionInTicks / 10000.0} ms");
            sb.AppendLine($"Resolution (maximum): {SystemTimerResolution.MaximumResolutionInTicks / 10000.0} ms");
            MsgBox.ShowInformation(this, sb.ToString());
        }
    }
}
