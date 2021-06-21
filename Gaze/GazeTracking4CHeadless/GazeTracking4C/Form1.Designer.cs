namespace GazeTracking4C
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.PIDLab = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.RunningLAB = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(68, 49);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "PartipantID";
            // 
            // TrayIcon
            // 
            this.TrayIcon.Text = "EyeTrackingRunning";
            this.TrayIcon.Visible = true;
            this.TrayIcon.DoubleClick += new System.EventHandler(this.TrayIcon_DoubleClick);
            // 
            // PIDLab
            // 
            this.PIDLab.AutoSize = true;
            this.PIDLab.Location = new System.Drawing.Point(155, 49);
            this.PIDLab.Name = "PIDLab";
            this.PIDLab.Size = new System.Drawing.Size(54, 13);
            this.PIDLab.TabIndex = 6;
            this.PIDLab.Text = "NOT SET";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(71, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Running";
            // 
            // RunningLAB
            // 
            this.RunningLAB.AutoSize = true;
            this.RunningLAB.Location = new System.Drawing.Point(155, 82);
            this.RunningLAB.Name = "RunningLAB";
            this.RunningLAB.Size = new System.Drawing.Size(32, 13);
            this.RunningLAB.TabIndex = 8;
            this.RunningLAB.Text = "False";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 301);
            this.Controls.Add(this.RunningLAB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PIDLab);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.Text = "IARPA Eye Tracking";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.Label PIDLab;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label RunningLAB;
    }
}

