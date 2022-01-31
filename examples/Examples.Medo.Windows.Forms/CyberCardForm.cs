using Medo.Device;
using System;
using System.Windows.Forms;

namespace Medo.Windows.Forms.Examples {
    public partial class CyberCardForm : Form {
        public CyberCardForm() {
            InitializeComponent();
        }

        private void CyberCardForm_Load(object sender, EventArgs e) {
            foreach (var portName in CyberCard.GetSerialPortNames()) {
                mnuPort.Items.Add(portName);
            }
            if (mnuPort.Items.Count > 0) { mnuPort.SelectedIndex = 0; }
        }

        private void CyberCardForm_FormClosed(object sender, FormClosedEventArgs e) {
            tmrRefresh.Enabled = false;
            if (Device != null) { Device.Dispose(); }
        }

        private CyberCard Device;

        private void mnuOpen_Click(object sender, EventArgs e) {
            try {
                Device = new CyberCard(mnuPort.SelectedItem.ToString());
                tmrRefresh.Enabled = true;
                tmrRefresh_Tick(null, EventArgs.Empty);
                flow.Enabled = true;
            } catch (Exception ex) {
                MsgBox.ShowError(this, ex.Message);
            }
        }

        private void tmrRefresh_Tick(object sender, EventArgs e) {
            lblModel.Text = $"Model:\n{Device.GetDeviceModel()}";
            lblCapacity.Text= $"Capacity:\n{Device.GetDeviceCapacity():0} W  {Device.GetDeviceCapacityVA():0} VA";
            lblVoltageInput.Text = $"Input voltage:\n{Device.GetInputVoltage():0.0} V";
            lblVoltageOutput.Text = $"Output voltage:\n{Device.GetOutputVoltage():0.0} V";
            lblFrequency.Text = $"Frequency:\n{Device.GetFrequency():0.0} Hz";
            lblLoadPercentage.Text = $"Load:\n{Device.GetLoadPercentage():0%}";
            lblBatteryPercentage.Text = $"Battery remaining:\n{Device.GetBatteryPercentage():0%}";
            lblBatteryRuntime.Text = $"Battery runtime:\n{Device.GetBatteryRuntime():0} minutes";

            var status = "";
            if (Device.IsPoweredOn() == true) { status += "Powered on\n"; }
            if (Device.IsPoweredOff() == true) { status += "Powered off\n"; }
            if (Device.IsPendingPowerOn()==true) { status += "Pending power on\n"; }
            if (Device.IsPendingPowerOff() == true) { status += "Pending power off\n"; }
            if (Device.IsTestInProgress() == true) { status += "Testing\n"; }
            if (Device.IsAlarmActive() == true) { status += "Alarm active\n"; }
            if (Device.IsUsingBattery() == true) { status += "On battery power\n"; }
            if (Device.IsBatteryLow() == true) { status += "Battery low\n"; }
            if (Device.IsBatteryCharging() == true) { status += "Battery charging\n"; }
            if (Device.IsBatteryFull() == true) { status += "Battery full\n"; }
            lblStatus.Text = status.Trim();
        }

        private void btnPowerOff_Click(object sender, EventArgs e) {
            Device.PowerOff();
        }

        private void btnPowerOn_Click(object sender, EventArgs e) {
            Device.PowerOn();
        }

        private void btnPowerReset_Click(object sender, EventArgs e) {
            Device.PowerReset();
        }

        private void buttonAlarmEnable_Click(object sender, EventArgs e) {
            Device.AlarmEnable();
        }

        private void btnAlarmDisable_Click(object sender, EventArgs e) {
            Device.AlarmDisable();
        }

    }
}
