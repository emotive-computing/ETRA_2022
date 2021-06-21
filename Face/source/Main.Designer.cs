using System.IO;
using System;

namespace FaceLauncher
{
    partial class Main
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
            Console.WriteLine("Top of init.");
            if (!readConfig())
            {
                this.components = new System.ComponentModel.Container();
                this.lblInstructions = new System.Windows.Forms.Label();
                this.txtId = new System.Windows.Forms.TextBox();
                this.btnOk = new System.Windows.Forms.Button();
                //this.btnStop = new System.Windows.Forms.Button();
                this.panelCamera = new System.Windows.Forms.Panel();
                this.cmbCamera = new System.Windows.Forms.ComboBox();
                this.lblCamera = new System.Windows.Forms.Label();
                this.timerCamera = new System.Windows.Forms.Timer(this.components);
                this.SuspendLayout();
                // 
                // lblInstructions
                // 
                this.lblInstructions.AutoSize = true;
                this.lblInstructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.lblInstructions.Location = new System.Drawing.Point(47, 51);
                this.lblInstructions.Name = "lblInstructions";
                this.lblInstructions.Size = new System.Drawing.Size(328, 20);
                this.lblInstructions.TabIndex = 0;
                this.lblInstructions.Text = "Please enter a username.";
                // 
                // txtId
                // 
                this.txtId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.txtId.Location = new System.Drawing.Point(51, 84);
                this.txtId.Name = "txtId";
                this.txtId.Size = new System.Drawing.Size(216, 26);
                this.txtId.TabIndex = 1;
                // 
                // btnOk
                // 
                this.btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.btnOk.Location = new System.Drawing.Point(273, 79);
                this.btnOk.Name = "btnOk";
                this.btnOk.Size = new System.Drawing.Size(101, 36);
                this.btnOk.TabIndex = 2;
                this.btnOk.Text = "OK";
                this.btnOk.UseVisualStyleBackColor = true;
                this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
                // 
                // btnStop
                // 
                /*this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.btnStop.Location = new System.Drawing.Point(157, 79);
                this.btnStop.Name = "btnStop";
                this.btnStop.Size = new System.Drawing.Size(101, 36);
                this.btnStop.TabIndex = 3;
                this.btnStop.Text = "Stop";
                this.btnStop.UseVisualStyleBackColor = true;
                this.btnStop.Visible = false;
                this.btnStop.Click += new System.EventHandler(this.btnStop_Click);*/
                // 
                // panelCamera
                // 
                this.panelCamera.Location = new System.Drawing.Point(51, 150);
                this.panelCamera.Name = "panelCamera";
                this.panelCamera.Size = new System.Drawing.Size(324, 221);
                this.panelCamera.TabIndex = 4;
                // 
                // cmbCamera
                // 
                this.cmbCamera.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.cmbCamera.FormattingEnabled = true;
                this.cmbCamera.Items.AddRange(new object[] { "0", "1" });
                //this.cmbCamera.Items.AddRange(new object[] {"0","1","2","3","4","5"});
                this.cmbCamera.Location = new System.Drawing.Point(207, 377);
                this.cmbCamera.Name = "cmbCamera";
                this.cmbCamera.Size = new System.Drawing.Size(143, 28);
                this.cmbCamera.TabIndex = 5;
                this.cmbCamera.Text = "0";
                this.cmbCamera.SelectedIndexChanged += new System.EventHandler(this.cmbCamera_SelectedIndexChanged);
                // 
                // lblCamera
                // 
                this.lblCamera.AutoSize = true;
                this.lblCamera.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.lblCamera.Location = new System.Drawing.Point(77, 380);
                this.lblCamera.Name = "lblCamera";
                this.lblCamera.Size = new System.Drawing.Size(124, 20);
                this.lblCamera.TabIndex = 6;
                this.lblCamera.Text = "Select a camera";
                // 
                // timerCamera
                // 
                this.timerCamera.Enabled = true;
                this.timerCamera.Interval = 1500;
                this.timerCamera.Tick += new System.EventHandler(this.timerCamera_Tick);
                // 
                // Main
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(438, 470);
                this.Controls.Add(this.lblCamera);
                this.Controls.Add(this.cmbCamera);
                this.Controls.Add(this.panelCamera);
                //this.Controls.Add(this.btnStop);
                this.Controls.Add(this.btnOk);
                this.Controls.Add(this.txtId);
                this.Controls.Add(this.lblInstructions);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "Main";
                this.Text = "Launch Openface";
                this.Load += new System.EventHandler(this.Main_Load);
                this.ResumeLayout(false);
                this.PerformLayout();
                /*                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(438, 470);
                this.Controls.Add(this.lblCamera);
                this.Controls.Add(this.cmbCamera);
                this.Controls.Add(this.panelCamera);
                this.Controls.Add(this.btnOk);
                this.Controls.Add(this.txtId);
                this.Controls.Add(this.lblInstructions);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "Main";
                this.Text = "CLM-Framework";
                this.Load += new System.EventHandler(this.Main_Load);
                this.ResumeLayout(false);
                this.PerformLayout();*/
            }

        }

        #endregion

        private System.Windows.Forms.Label lblInstructions;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Button btnOk;
        //private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Panel panelCamera;
        private System.Windows.Forms.ComboBox cmbCamera;
        private System.Windows.Forms.Label lblCamera;
        private System.Windows.Forms.Timer timerCamera;
    }
}

