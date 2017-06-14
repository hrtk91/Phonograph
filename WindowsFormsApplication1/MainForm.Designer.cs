namespace Phonograph
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
			this.ParametaButton = new System.Windows.Forms.Button();
			this.RecButton = new System.Windows.Forms.Button();
			this.PlayButton = new System.Windows.Forms.Button();
			this.RecTimeLabel = new System.Windows.Forms.Label();
			this.PlayFileLabel = new System.Windows.Forms.Label();
			this.CloseButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ParametaButton
			// 
			this.ParametaButton.Location = new System.Drawing.Point(12, 89);
			this.ParametaButton.Name = "ParametaButton";
			this.ParametaButton.Size = new System.Drawing.Size(87, 23);
			this.ParametaButton.TabIndex = 0;
			this.ParametaButton.Text = "パラメータ設定";
			this.ParametaButton.UseVisualStyleBackColor = true;
			this.ParametaButton.Click += new System.EventHandler(this.ParametaButton_Click);
			// 
			// RecButton
			// 
			this.RecButton.Location = new System.Drawing.Point(114, 89);
			this.RecButton.Name = "RecButton";
			this.RecButton.Size = new System.Drawing.Size(75, 23);
			this.RecButton.TabIndex = 1;
			this.RecButton.Text = "録音";
			this.RecButton.UseVisualStyleBackColor = true;
			this.RecButton.Click += new System.EventHandler(this.RecButton_Click);
			// 
			// PlayButton
			// 
			this.PlayButton.Location = new System.Drawing.Point(205, 89);
			this.PlayButton.Name = "PlayButton";
			this.PlayButton.Size = new System.Drawing.Size(75, 23);
			this.PlayButton.TabIndex = 2;
			this.PlayButton.Text = "再生";
			this.PlayButton.UseVisualStyleBackColor = true;
			this.PlayButton.Click += new System.EventHandler(this.PlayButton_Click);
			// 
			// RecTimeLabel
			// 
			this.RecTimeLabel.AutoSize = true;
			this.RecTimeLabel.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.RecTimeLabel.Location = new System.Drawing.Point(25, 21);
			this.RecTimeLabel.Name = "RecTimeLabel";
			this.RecTimeLabel.Size = new System.Drawing.Size(80, 16);
			this.RecTimeLabel.TabIndex = 4;
			this.RecTimeLabel.Text = "録音時間：";
			// 
			// PlayFileLabel
			// 
			this.PlayFileLabel.AutoSize = true;
			this.PlayFileLabel.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.PlayFileLabel.Location = new System.Drawing.Point(12, 54);
			this.PlayFileLabel.Name = "PlayFileLabel";
			this.PlayFileLabel.Size = new System.Drawing.Size(93, 16);
			this.PlayFileLabel.TabIndex = 5;
			this.PlayFileLabel.Text = "再生ファイル：";
			// 
			// CloseButton
			// 
			this.CloseButton.Location = new System.Drawing.Point(297, 89);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(75, 23);
			this.CloseButton.TabIndex = 6;
			this.CloseButton.Text = "終了";
			this.CloseButton.UseVisualStyleBackColor = true;
			this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(383, 127);
			this.Controls.Add(this.CloseButton);
			this.Controls.Add(this.PlayFileLabel);
			this.Controls.Add(this.RecTimeLabel);
			this.Controls.Add(this.PlayButton);
			this.Controls.Add(this.RecButton);
			this.Controls.Add(this.ParametaButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "音響解析システム";
			this.TransparencyKey = System.Drawing.Color.White;
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ParametaButton;
        private System.Windows.Forms.Button RecButton;
		private System.Windows.Forms.Button PlayButton;
		private System.Windows.Forms.Label RecTimeLabel;
		private System.Windows.Forms.Label PlayFileLabel;
		private System.Windows.Forms.Button CloseButton;
    }
}

