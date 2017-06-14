namespace Phonograph
{
	partial class PlayForm
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
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.CANCEL = new System.Windows.Forms.Button();
			this.TimeLabel = new System.Windows.Forms.Label();
			this.ModeLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(12, 37);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(342, 47);
			this.progressBar1.TabIndex = 0;
			// 
			// CANCEL
			// 
			this.CANCEL.Location = new System.Drawing.Point(201, 90);
			this.CANCEL.Name = "CANCEL";
			this.CANCEL.Size = new System.Drawing.Size(153, 34);
			this.CANCEL.TabIndex = 1;
			this.CANCEL.Text = "CANCEL";
			this.CANCEL.UseVisualStyleBackColor = true;
			this.CANCEL.Click += new System.EventHandler(this.CANCEL_Click);
			// 
			// TimeLabel
			// 
			this.TimeLabel.AutoSize = true;
			this.TimeLabel.Font = new System.Drawing.Font("MS UI Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.TimeLabel.Location = new System.Drawing.Point(12, 90);
			this.TimeLabel.Name = "TimeLabel";
			this.TimeLabel.Size = new System.Drawing.Size(117, 21);
			this.TimeLabel.TabIndex = 2;
			this.TimeLabel.Text = "00:00/00:00";
			// 
			// ModeLabel
			// 
			this.ModeLabel.AutoSize = true;
			this.ModeLabel.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.ModeLabel.Location = new System.Drawing.Point(12, 13);
			this.ModeLabel.Name = "ModeLabel";
			this.ModeLabel.Size = new System.Drawing.Size(65, 16);
			this.ModeLabel.TabIndex = 3;
			this.ModeLabel.Text = "再生中...";
			// 
			// PlayForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(366, 136);
			this.Controls.Add(this.ModeLabel);
			this.Controls.Add(this.TimeLabel);
			this.Controls.Add(this.CANCEL);
			this.Controls.Add(this.progressBar1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "PlayForm";
			this.Text = "再生中00:00/00:00";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Button CANCEL;
		private System.Windows.Forms.Label TimeLabel;
		private System.Windows.Forms.Label ModeLabel;
	}
}