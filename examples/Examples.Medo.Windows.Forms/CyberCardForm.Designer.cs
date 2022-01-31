namespace Medo.Windows.Forms.Examples {
    partial class CyberCardForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CyberCardForm));
            this.flow = new System.Windows.Forms.FlowLayoutPanel();
            this.lblModel = new System.Windows.Forms.Label();
            this.lblCapacity = new System.Windows.Forms.Label();
            this.lblVoltageInput = new System.Windows.Forms.Label();
            this.lblVoltageOutput = new System.Windows.Forms.Label();
            this.lblFrequency = new System.Windows.Forms.Label();
            this.lblLoadPercentage = new System.Windows.Forms.Label();
            this.lblBatteryPercentage = new System.Windows.Forms.Label();
            this.lblBatteryRuntime = new System.Windows.Forms.Label();
            this.btnPowerOff = new System.Windows.Forms.Button();
            this.btnPowerOn = new System.Windows.Forms.Button();
            this.btnPowerReset = new System.Windows.Forms.Button();
            this.buttonAlarmEnable = new System.Windows.Forms.Button();
            this.btnAlarmDisable = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.mnu = new System.Windows.Forms.ToolStrip();
            this.mnuPort = new System.Windows.Forms.ToolStripComboBox();
            this.mnuOpen = new System.Windows.Forms.ToolStripButton();
            this.tmrRefresh = new System.Windows.Forms.Timer(this.components);
            this.flow.SuspendLayout();
            this.mnu.SuspendLayout();
            this.SuspendLayout();
            // 
            // flow
            // 
            this.flow.Controls.Add(this.lblModel);
            this.flow.Controls.Add(this.lblCapacity);
            this.flow.Controls.Add(this.lblVoltageInput);
            this.flow.Controls.Add(this.lblVoltageOutput);
            this.flow.Controls.Add(this.lblFrequency);
            this.flow.Controls.Add(this.lblLoadPercentage);
            this.flow.Controls.Add(this.lblBatteryPercentage);
            this.flow.Controls.Add(this.lblBatteryRuntime);
            this.flow.Controls.Add(this.btnPowerOff);
            this.flow.Controls.Add(this.btnPowerOn);
            this.flow.Controls.Add(this.btnPowerReset);
            this.flow.Controls.Add(this.buttonAlarmEnable);
            this.flow.Controls.Add(this.btnAlarmDisable);
            this.flow.Controls.Add(this.lblStatus);
            this.flow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flow.Enabled = false;
            this.flow.Location = new System.Drawing.Point(0, 28);
            this.flow.Name = "flow";
            this.flow.Padding = new System.Windows.Forms.Padding(3);
            this.flow.Size = new System.Drawing.Size(782, 425);
            this.flow.TabIndex = 0;
            // 
            // lblModel
            // 
            this.lblModel.BackColor = System.Drawing.SystemColors.Window;
            this.lblModel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblModel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblModel.Location = new System.Drawing.Point(6, 6);
            this.lblModel.Margin = new System.Windows.Forms.Padding(3);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(160, 80);
            this.lblModel.TabIndex = 6;
            this.lblModel.Text = "Model";
            this.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCapacity
            // 
            this.lblCapacity.BackColor = System.Drawing.SystemColors.Window;
            this.lblCapacity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCapacity.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblCapacity.Location = new System.Drawing.Point(172, 6);
            this.lblCapacity.Margin = new System.Windows.Forms.Padding(3);
            this.lblCapacity.Name = "lblCapacity";
            this.lblCapacity.Size = new System.Drawing.Size(160, 80);
            this.lblCapacity.TabIndex = 7;
            this.lblCapacity.Text = "Capacity";
            this.lblCapacity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVoltageInput
            // 
            this.lblVoltageInput.BackColor = System.Drawing.SystemColors.Window;
            this.lblVoltageInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVoltageInput.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblVoltageInput.Location = new System.Drawing.Point(338, 6);
            this.lblVoltageInput.Margin = new System.Windows.Forms.Padding(3);
            this.lblVoltageInput.Name = "lblVoltageInput";
            this.lblVoltageInput.Size = new System.Drawing.Size(160, 80);
            this.lblVoltageInput.TabIndex = 0;
            this.lblVoltageInput.Text = "Input voltage";
            this.lblVoltageInput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVoltageOutput
            // 
            this.lblVoltageOutput.BackColor = System.Drawing.SystemColors.Window;
            this.lblVoltageOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVoltageOutput.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblVoltageOutput.Location = new System.Drawing.Point(504, 6);
            this.lblVoltageOutput.Margin = new System.Windows.Forms.Padding(3);
            this.lblVoltageOutput.Name = "lblVoltageOutput";
            this.lblVoltageOutput.Size = new System.Drawing.Size(160, 80);
            this.lblVoltageOutput.TabIndex = 1;
            this.lblVoltageOutput.Text = "Output voltage";
            this.lblVoltageOutput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFrequency
            // 
            this.lblFrequency.BackColor = System.Drawing.SystemColors.Window;
            this.lblFrequency.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFrequency.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblFrequency.Location = new System.Drawing.Point(6, 92);
            this.lblFrequency.Margin = new System.Windows.Forms.Padding(3);
            this.lblFrequency.Name = "lblFrequency";
            this.lblFrequency.Size = new System.Drawing.Size(160, 80);
            this.lblFrequency.TabIndex = 4;
            this.lblFrequency.Text = "Frequency";
            this.lblFrequency.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLoadPercentage
            // 
            this.lblLoadPercentage.BackColor = System.Drawing.SystemColors.Window;
            this.lblLoadPercentage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLoadPercentage.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblLoadPercentage.Location = new System.Drawing.Point(172, 92);
            this.lblLoadPercentage.Margin = new System.Windows.Forms.Padding(3);
            this.lblLoadPercentage.Name = "lblLoadPercentage";
            this.lblLoadPercentage.Size = new System.Drawing.Size(160, 80);
            this.lblLoadPercentage.TabIndex = 2;
            this.lblLoadPercentage.Text = "Load";
            this.lblLoadPercentage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBatteryPercentage
            // 
            this.lblBatteryPercentage.BackColor = System.Drawing.SystemColors.Window;
            this.lblBatteryPercentage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBatteryPercentage.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblBatteryPercentage.Location = new System.Drawing.Point(338, 92);
            this.lblBatteryPercentage.Margin = new System.Windows.Forms.Padding(3);
            this.lblBatteryPercentage.Name = "lblBatteryPercentage";
            this.lblBatteryPercentage.Size = new System.Drawing.Size(160, 80);
            this.lblBatteryPercentage.TabIndex = 3;
            this.lblBatteryPercentage.Text = "Battery";
            this.lblBatteryPercentage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBatteryRuntime
            // 
            this.lblBatteryRuntime.BackColor = System.Drawing.SystemColors.Window;
            this.lblBatteryRuntime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBatteryRuntime.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblBatteryRuntime.Location = new System.Drawing.Point(504, 92);
            this.lblBatteryRuntime.Margin = new System.Windows.Forms.Padding(3);
            this.lblBatteryRuntime.Name = "lblBatteryRuntime";
            this.lblBatteryRuntime.Size = new System.Drawing.Size(160, 80);
            this.lblBatteryRuntime.TabIndex = 5;
            this.lblBatteryRuntime.Text = "Runtime";
            this.lblBatteryRuntime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnPowerOff
            // 
            this.btnPowerOff.Location = new System.Drawing.Point(6, 178);
            this.btnPowerOff.Name = "btnPowerOff";
            this.btnPowerOff.Size = new System.Drawing.Size(160, 80);
            this.btnPowerOff.TabIndex = 8;
            this.btnPowerOff.Text = "Power off";
            this.btnPowerOff.UseVisualStyleBackColor = true;
            this.btnPowerOff.Click += new System.EventHandler(this.btnPowerOff_Click);
            // 
            // btnPowerOn
            // 
            this.btnPowerOn.Location = new System.Drawing.Point(172, 178);
            this.btnPowerOn.Name = "btnPowerOn";
            this.btnPowerOn.Size = new System.Drawing.Size(160, 80);
            this.btnPowerOn.TabIndex = 9;
            this.btnPowerOn.Text = "Power on";
            this.btnPowerOn.UseVisualStyleBackColor = true;
            this.btnPowerOn.Click += new System.EventHandler(this.btnPowerOn_Click);
            // 
            // btnPowerReset
            // 
            this.btnPowerReset.Location = new System.Drawing.Point(338, 178);
            this.btnPowerReset.Name = "btnPowerReset";
            this.btnPowerReset.Size = new System.Drawing.Size(160, 80);
            this.btnPowerReset.TabIndex = 10;
            this.btnPowerReset.Text = "Power reset";
            this.btnPowerReset.UseVisualStyleBackColor = true;
            this.btnPowerReset.Click += new System.EventHandler(this.btnPowerReset_Click);
            // 
            // buttonAlarmEnable
            // 
            this.buttonAlarmEnable.Location = new System.Drawing.Point(504, 178);
            this.buttonAlarmEnable.Name = "buttonAlarmEnable";
            this.buttonAlarmEnable.Size = new System.Drawing.Size(160, 80);
            this.buttonAlarmEnable.TabIndex = 11;
            this.buttonAlarmEnable.Text = "Alarm enable";
            this.buttonAlarmEnable.UseVisualStyleBackColor = true;
            this.buttonAlarmEnable.Click += new System.EventHandler(this.buttonAlarmEnable_Click);
            // 
            // btnAlarmDisable
            // 
            this.btnAlarmDisable.Location = new System.Drawing.Point(6, 264);
            this.btnAlarmDisable.Name = "btnAlarmDisable";
            this.btnAlarmDisable.Size = new System.Drawing.Size(160, 80);
            this.btnAlarmDisable.TabIndex = 12;
            this.btnAlarmDisable.Text = "Alarm disable";
            this.btnAlarmDisable.UseVisualStyleBackColor = true;
            this.btnAlarmDisable.Click += new System.EventHandler(this.btnAlarmDisable_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.SystemColors.Control;
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStatus.Location = new System.Drawing.Point(172, 264);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(160, 120);
            this.lblStatus.TabIndex = 13;
            this.lblStatus.Text = "Status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mnu
            // 
            this.mnu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.mnu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuPort,
            this.mnuOpen});
            this.mnu.Location = new System.Drawing.Point(0, 0);
            this.mnu.Name = "mnu";
            this.mnu.Size = new System.Drawing.Size(782, 28);
            this.mnu.TabIndex = 1;
            // 
            // mnuPort
            // 
            this.mnuPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mnuPort.Name = "mnuPort";
            this.mnuPort.Size = new System.Drawing.Size(121, 28);
            // 
            // mnuOpen
            // 
            this.mnuOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuOpen.Image = ((System.Drawing.Image)(resources.GetObject("mnuOpen.Image")));
            this.mnuOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuOpen.Name = "mnuOpen";
            this.mnuOpen.Size = new System.Drawing.Size(29, 25);
            this.mnuOpen.Text = "Open";
            this.mnuOpen.Click += new System.EventHandler(this.mnuOpen_Click);
            // 
            // tmrRefresh
            // 
            this.tmrRefresh.Interval = 1000;
            this.tmrRefresh.Tick += new System.EventHandler(this.tmrRefresh_Tick);
            // 
            // CyberCardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 453);
            this.Controls.Add(this.flow);
            this.Controls.Add(this.mnu);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CyberCardForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CyberCard";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CyberCardForm_FormClosed);
            this.Load += new System.EventHandler(this.CyberCardForm_Load);
            this.flow.ResumeLayout(false);
            this.mnu.ResumeLayout(false);
            this.mnu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flow;
        private System.Windows.Forms.Label lblVoltageInput;
        private System.Windows.Forms.Label lblVoltageOutput;
        private System.Windows.Forms.Label lblLoadPercentage;
        private System.Windows.Forms.Label lblBatteryPercentage;
        private System.Windows.Forms.Label lblFrequency;
        private System.Windows.Forms.Label lblBatteryRuntime;
        private System.Windows.Forms.ToolStrip mnu;
        private System.Windows.Forms.ToolStripComboBox mnuPort;
        private System.Windows.Forms.ToolStripButton mnuOpen;
        private System.Windows.Forms.Timer tmrRefresh;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.Label lblCapacity;
        private System.Windows.Forms.Button btnPowerOff;
        private System.Windows.Forms.Button btnPowerOn;
        private System.Windows.Forms.Button btnPowerReset;
        private System.Windows.Forms.Button buttonAlarmEnable;
        private System.Windows.Forms.Button btnAlarmDisable;
        private System.Windows.Forms.Label lblStatus;
    }
}
