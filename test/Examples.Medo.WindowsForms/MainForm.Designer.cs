namespace Medo.Windows.Forms.Examples
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnMessageBoxInformation = new System.Windows.Forms.Button();
            this.btnMessageBoxWarning = new System.Windows.Forms.Button();
            this.btnMessageBoxError = new System.Windows.Forms.Button();
            this.btnMessageBoxQuestion = new System.Windows.Forms.Button();
            this.btnUnhandledCatch = new System.Windows.Forms.Button();
            this.btnUnhandledCatchTask = new System.Windows.Forms.Button();
            this.btnUnhandledCatchWorker = new System.Windows.Forms.Button();
            this.btnAboutBox = new System.Windows.Forms.Button();
            this.btnTps = new System.Windows.Forms.Button();
            this.btnTimerResolution = new System.Windows.Forms.Button();
            this.bwUnhandledCatchBackground = new System.ComponentModel.BackgroundWorker();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnMessageBoxInformation);
            this.flowLayoutPanel1.Controls.Add(this.btnMessageBoxWarning);
            this.flowLayoutPanel1.Controls.Add(this.btnMessageBoxError);
            this.flowLayoutPanel1.Controls.Add(this.btnMessageBoxQuestion);
            this.flowLayoutPanel1.Controls.Add(this.btnUnhandledCatch);
            this.flowLayoutPanel1.Controls.Add(this.btnUnhandledCatchTask);
            this.flowLayoutPanel1.Controls.Add(this.btnUnhandledCatchWorker);
            this.flowLayoutPanel1.Controls.Add(this.btnAboutBox);
            this.flowLayoutPanel1.Controls.Add(this.btnTps);
            this.flowLayoutPanel1.Controls.Add(this.btnTimerResolution);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btnMessageBoxInformation
            // 
            this.btnMessageBoxInformation.Location = new System.Drawing.Point(3, 3);
            this.btnMessageBoxInformation.Name = "btnMessageBoxInformation";
            this.btnMessageBoxInformation.Size = new System.Drawing.Size(200, 29);
            this.btnMessageBoxInformation.TabIndex = 0;
            this.btnMessageBoxInformation.Text = "MessageBox: Information";
            this.btnMessageBoxInformation.UseVisualStyleBackColor = true;
            this.btnMessageBoxInformation.Click += new System.EventHandler(this.btnMessageBoxInformation_Click);
            // 
            // btnMessageBoxWarning
            // 
            this.btnMessageBoxWarning.Location = new System.Drawing.Point(209, 3);
            this.btnMessageBoxWarning.Name = "btnMessageBoxWarning";
            this.btnMessageBoxWarning.Size = new System.Drawing.Size(200, 29);
            this.btnMessageBoxWarning.TabIndex = 1;
            this.btnMessageBoxWarning.Text = "MessageBox: Warning";
            this.btnMessageBoxWarning.UseVisualStyleBackColor = true;
            this.btnMessageBoxWarning.Click += new System.EventHandler(this.btnMessageBoxWarning_Click);
            // 
            // btnMessageBoxError
            // 
            this.btnMessageBoxError.Location = new System.Drawing.Point(415, 3);
            this.btnMessageBoxError.Name = "btnMessageBoxError";
            this.btnMessageBoxError.Size = new System.Drawing.Size(200, 29);
            this.btnMessageBoxError.TabIndex = 2;
            this.btnMessageBoxError.Text = "MessageBox: Error";
            this.btnMessageBoxError.UseVisualStyleBackColor = true;
            this.btnMessageBoxError.Click += new System.EventHandler(this.btnMessageBoxError_Click);
            // 
            // btnMessageBoxQuestion
            // 
            this.btnMessageBoxQuestion.Location = new System.Drawing.Point(3, 38);
            this.btnMessageBoxQuestion.Name = "btnMessageBoxQuestion";
            this.btnMessageBoxQuestion.Size = new System.Drawing.Size(200, 29);
            this.btnMessageBoxQuestion.TabIndex = 3;
            this.btnMessageBoxQuestion.Text = "MessageBox: Question";
            this.btnMessageBoxQuestion.UseVisualStyleBackColor = true;
            this.btnMessageBoxQuestion.Click += new System.EventHandler(this.btnMessageBoxQuestion_Click);
            // 
            // btnUnhandledCatch
            // 
            this.btnUnhandledCatch.Location = new System.Drawing.Point(209, 38);
            this.btnUnhandledCatch.Name = "btnUnhandledCatch";
            this.btnUnhandledCatch.Size = new System.Drawing.Size(200, 29);
            this.btnUnhandledCatch.TabIndex = 4;
            this.btnUnhandledCatch.Text = "Unhandled catch";
            this.btnUnhandledCatch.UseVisualStyleBackColor = true;
            this.btnUnhandledCatch.Click += new System.EventHandler(this.btnUnhandledCatch_Click);
            // 
            // btnUnhandledCatchTask
            // 
            this.btnUnhandledCatchTask.Location = new System.Drawing.Point(415, 38);
            this.btnUnhandledCatchTask.Name = "btnUnhandledCatchTask";
            this.btnUnhandledCatchTask.Size = new System.Drawing.Size(200, 29);
            this.btnUnhandledCatchTask.TabIndex = 9;
            this.btnUnhandledCatchTask.Text = "Unhandled catch (task)";
            this.btnUnhandledCatchTask.UseVisualStyleBackColor = true;
            this.btnUnhandledCatchTask.Click += new System.EventHandler(this.btnUnhandledCatchTask_Click);
            // 
            // btnUnhandledCatchWorker
            // 
            this.btnUnhandledCatchWorker.Location = new System.Drawing.Point(3, 73);
            this.btnUnhandledCatchWorker.Name = "btnUnhandledCatchWorker";
            this.btnUnhandledCatchWorker.Size = new System.Drawing.Size(200, 29);
            this.btnUnhandledCatchWorker.TabIndex = 8;
            this.btnUnhandledCatchWorker.Text = "Unhandled catch (thread)";
            this.btnUnhandledCatchWorker.UseVisualStyleBackColor = true;
            this.btnUnhandledCatchWorker.Click += new System.EventHandler(this.btnUnhandledCatchBackground_Click);
            // 
            // btnAboutBox
            // 
            this.btnAboutBox.Location = new System.Drawing.Point(209, 73);
            this.btnAboutBox.Name = "btnAboutBox";
            this.btnAboutBox.Size = new System.Drawing.Size(200, 29);
            this.btnAboutBox.TabIndex = 5;
            this.btnAboutBox.Text = "About box";
            this.btnAboutBox.UseVisualStyleBackColor = true;
            this.btnAboutBox.Click += new System.EventHandler(this.btnAboutBox_Click);
            // 
            // btnTps
            // 
            this.btnTps.Location = new System.Drawing.Point(415, 73);
            this.btnTps.Name = "btnTps";
            this.btnTps.Size = new System.Drawing.Size(200, 29);
            this.btnTps.TabIndex = 6;
            this.btnTps.Text = "TPS";
            this.btnTps.UseVisualStyleBackColor = true;
            this.btnTps.Click += new System.EventHandler(this.btnTps_Click);
            // 
            // btnTimerResolution
            // 
            this.btnTimerResolution.Location = new System.Drawing.Point(3, 108);
            this.btnTimerResolution.Name = "btnTimerResolution";
            this.btnTimerResolution.Size = new System.Drawing.Size(200, 29);
            this.btnTimerResolution.TabIndex = 7;
            this.btnTimerResolution.Text = "Timer resolution";
            this.btnTimerResolution.UseVisualStyleBackColor = true;
            this.btnTimerResolution.Click += new System.EventHandler(this.btnTimerResolution_Click);
            // 
            // bwUnhandledCatchBackground
            // 
            this.bwUnhandledCatchBackground.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwUnhandledCatchWorker_DoWork);
            this.bwUnhandledCatchBackground.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwUnhandledCatchBackground_RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnMessageBoxInformation;
        private System.Windows.Forms.Button btnMessageBoxWarning;
        private System.Windows.Forms.Button btnMessageBoxError;
        private System.Windows.Forms.Button btnMessageBoxQuestion;
        private System.Windows.Forms.Button btnUnhandledCatch;
        private System.Windows.Forms.Button btnAboutBox;
        private System.Windows.Forms.Button btnTps;
        private System.Windows.Forms.Button btnTimerResolution;
        private System.Windows.Forms.Button btnUnhandledCatchWorker;
        private System.ComponentModel.BackgroundWorker bwUnhandledCatchBackground;
        private System.Windows.Forms.Button btnUnhandledCatchTask;
    }
}

