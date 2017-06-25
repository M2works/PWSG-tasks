namespace Puzzle
{
    partial class Settings
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.LifesNUD = new System.Windows.Forms.NumericUpDown();
            this.TimeNUD = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.LifesNUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TimeNUD)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Lifes:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Time [s]:";
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(217, 87);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 4;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(136, 87);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 5;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // LifesNUD
            // 
            this.LifesNUD.Location = new System.Drawing.Point(136, 21);
            this.LifesNUD.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.LifesNUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.LifesNUD.Name = "LifesNUD";
            this.LifesNUD.Size = new System.Drawing.Size(156, 20);
            this.LifesNUD.TabIndex = 6;
            this.LifesNUD.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // TimeNUD
            // 
            this.TimeNUD.Location = new System.Drawing.Point(136, 53);
            this.TimeNUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.TimeNUD.Name = "TimeNUD";
            this.TimeNUD.Size = new System.Drawing.Size(156, 20);
            this.TimeNUD.TabIndex = 7;
            this.TimeNUD.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 122);
            this.Controls.Add(this.TimeNUD);
            this.Controls.Add(this.LifesNUD);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Settings";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.LifesNUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TimeNUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.NumericUpDown LifesNUD;
        private System.Windows.Forms.NumericUpDown TimeNUD;
    }
}