
namespace Examples {
    partial class TpsForm {
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
            this.lblTps = new System.Windows.Forms.Label();
            this.btnSetTps = new System.Windows.Forms.Button();
            this.nudTps = new System.Windows.Forms.NumericUpDown();
            this.lblMeasured = new System.Windows.Forms.Label();
            this.txtMeasured = new System.Windows.Forms.TextBox();
            this.lblIncrementing = new System.Windows.Forms.Label();
            this.tmrUpdateDisplay = new System.Windows.Forms.Timer(this.components);
            this.bwGenerator = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.nudTps)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTps
            // 
            this.lblTps.AutoSize = true;
            this.lblTps.Location = new System.Drawing.Point(12, 16);
            this.lblTps.Name = "lblTps";
            this.lblTps.Size = new System.Drawing.Size(36, 20);
            this.lblTps.TabIndex = 0;
            this.lblTps.Text = "TPS:";
            // 
            // btnSetTps
            // 
            this.btnSetTps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetTps.Location = new System.Drawing.Point(196, 12);
            this.btnSetTps.Name = "btnSetTps";
            this.btnSetTps.Size = new System.Drawing.Size(94, 29);
            this.btnSetTps.TabIndex = 2;
            this.btnSetTps.Text = "Set TPS";
            this.btnSetTps.UseVisualStyleBackColor = true;
            this.btnSetTps.Click += new System.EventHandler(this.btnSetTps_Click);
            // 
            // nudTps
            // 
            this.nudTps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudTps.Location = new System.Drawing.Point(90, 12);
            this.nudTps.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudTps.Name = "nudTps";
            this.nudTps.Size = new System.Drawing.Size(100, 27);
            this.nudTps.TabIndex = 1;
            this.nudTps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudTps.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lblMeasured
            // 
            this.lblMeasured.AutoSize = true;
            this.lblMeasured.Location = new System.Drawing.Point(12, 48);
            this.lblMeasured.Name = "lblMeasured";
            this.lblMeasured.Size = new System.Drawing.Size(77, 20);
            this.lblMeasured.TabIndex = 3;
            this.lblMeasured.Text = "Measured:";
            // 
            // txtMeasured
            // 
            this.txtMeasured.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMeasured.Location = new System.Drawing.Point(90, 45);
            this.txtMeasured.Name = "txtMeasured";
            this.txtMeasured.ReadOnly = true;
            this.txtMeasured.Size = new System.Drawing.Size(100, 27);
            this.txtMeasured.TabIndex = 4;
            this.txtMeasured.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblIncrementing
            // 
            this.lblIncrementing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIncrementing.Location = new System.Drawing.Point(196, 45);
            this.lblIncrementing.Name = "lblIncrementing";
            this.lblIncrementing.Size = new System.Drawing.Size(94, 30);
            this.lblIncrementing.TabIndex = 5;
            this.lblIncrementing.Text = "-";
            this.lblIncrementing.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmrUpdateDisplay
            // 
            this.tmrUpdateDisplay.Enabled = true;
            this.tmrUpdateDisplay.Interval = 200;
            this.tmrUpdateDisplay.Tick += new System.EventHandler(this.tmrUpdateDisplay_Tick);
            // 
            // bwGenerator
            // 
            this.bwGenerator.WorkerSupportsCancellation = true;
            this.bwGenerator.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwGenerator_DoWork);
            // 
            // TpsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 84);
            this.Controls.Add(this.lblIncrementing);
            this.Controls.Add(this.txtMeasured);
            this.Controls.Add(this.lblMeasured);
            this.Controls.Add(this.nudTps);
            this.Controls.Add(this.btnSetTps);
            this.Controls.Add(this.lblTps);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TpsForm";
            this.Text = "TPS";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.nudTps)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTps;
        private System.Windows.Forms.Button btnSetTps;
        private System.Windows.Forms.NumericUpDown nudTps;
        private System.Windows.Forms.Label lblMeasured;
        private System.Windows.Forms.TextBox txtMeasured;
        private System.Windows.Forms.Label lblIncrementing;
        private System.Windows.Forms.Timer tmrUpdateDisplay;
        private System.ComponentModel.BackgroundWorker bwGenerator;
    }
}
