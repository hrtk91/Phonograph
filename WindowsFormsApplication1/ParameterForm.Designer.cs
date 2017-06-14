namespace Phonograph
{
	partial class ParameterForm
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
			this.FilePathBox = new System.Windows.Forms.TextBox();
			this.ReferenceButton = new System.Windows.Forms.Button();
			this.OkButton = new System.Windows.Forms.Button();
			this.MyCancelButton = new System.Windows.Forms.Button();
			this.MinuteBox = new System.Windows.Forms.ComboBox();
			this.SecBox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.FileLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label1.Location = new System.Drawing.Point(13, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(145, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "録音時間（60分まで）";
			// 
			// FilePathBox
			// 
			this.FilePathBox.Location = new System.Drawing.Point(132, 69);
			this.FilePathBox.Name = "FilePathBox";
			this.FilePathBox.Size = new System.Drawing.Size(254, 19);
			this.FilePathBox.TabIndex = 2;
			// 
			// ReferenceButton
			// 
			this.ReferenceButton.Location = new System.Drawing.Point(398, 69);
			this.ReferenceButton.Name = "ReferenceButton";
			this.ReferenceButton.Size = new System.Drawing.Size(75, 23);
			this.ReferenceButton.TabIndex = 3;
			this.ReferenceButton.Text = "参照";
			this.ReferenceButton.UseVisualStyleBackColor = true;
			this.ReferenceButton.Click += new System.EventHandler(this.ReferenceButton_Click);
			// 
			// OkButton
			// 
			this.OkButton.Location = new System.Drawing.Point(311, 108);
			this.OkButton.Name = "OkButton";
			this.OkButton.Size = new System.Drawing.Size(75, 23);
			this.OkButton.TabIndex = 4;
			this.OkButton.Text = "OK";
			this.OkButton.UseVisualStyleBackColor = true;
			this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
			// 
			// MyCancelButton
			// 
			this.MyCancelButton.Location = new System.Drawing.Point(398, 108);
			this.MyCancelButton.Name = "MyCancelButton";
			this.MyCancelButton.Size = new System.Drawing.Size(75, 23);
			this.MyCancelButton.TabIndex = 5;
			this.MyCancelButton.Text = "Cancel";
			this.MyCancelButton.UseVisualStyleBackColor = true;
			this.MyCancelButton.Click += new System.EventHandler(this.CancelButton_Click);
			// 
			// MinuteBox
			// 
			this.MinuteBox.DropDownWidth = 40;
			this.MinuteBox.FormattingEnabled = true;
			this.MinuteBox.Location = new System.Drawing.Point(164, 29);
			this.MinuteBox.Name = "MinuteBox";
			this.MinuteBox.Size = new System.Drawing.Size(50, 20);
			this.MinuteBox.TabIndex = 6;
			// 
			// SecBox
			// 
			this.SecBox.DropDownWidth = 40;
			this.SecBox.FormattingEnabled = true;
			this.SecBox.Location = new System.Drawing.Point(251, 29);
			this.SecBox.Name = "SecBox";
			this.SecBox.Size = new System.Drawing.Size(50, 20);
			this.SecBox.TabIndex = 7;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("MS UI Gothic", 12F);
			this.label2.Location = new System.Drawing.Point(221, 29);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(24, 16);
			this.label2.TabIndex = 8;
			this.label2.Text = "分";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("MS UI Gothic", 12F);
			this.label3.Location = new System.Drawing.Point(308, 29);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(24, 16);
			this.label3.TabIndex = 9;
			this.label3.Text = "秒";
			// 
			// FileLabel
			// 
			this.FileLabel.AutoSize = true;
			this.FileLabel.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.FileLabel.Location = new System.Drawing.Point(16, 69);
			this.FileLabel.Name = "FileLabel";
			this.FileLabel.Size = new System.Drawing.Size(110, 16);
			this.FileLabel.TabIndex = 10;
			this.FileLabel.Text = "再生ファイルパス";
			// 
			// ParameterForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(485, 139);
			this.Controls.Add(this.FileLabel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.SecBox);
			this.Controls.Add(this.MinuteBox);
			this.Controls.Add(this.MyCancelButton);
			this.Controls.Add(this.OkButton);
			this.Controls.Add(this.ReferenceButton);
			this.Controls.Add(this.FilePathBox);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "ParameterForm";
			this.Text = "パラメータ設定画面";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox FilePathBox;
		private System.Windows.Forms.Button ReferenceButton;
		private System.Windows.Forms.Button OkButton;
		private System.Windows.Forms.Button MyCancelButton;
		private System.Windows.Forms.ComboBox MinuteBox;
		private System.Windows.Forms.ComboBox SecBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.Label FileLabel;
	}
}