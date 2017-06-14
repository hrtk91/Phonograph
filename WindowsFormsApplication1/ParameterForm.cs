using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace Phonograph
{
	/// <summary>
	/// パラメータフォームクラス
	/// </summary>
	public partial class ParameterForm : Form
	{
		/// <summary>
		/// フォームを初期化します。
		/// 初期化時にiniファイルがなければ作成されます。
		/// </summary>
		public ParameterForm()
		{
			InitializeComponent();

			//コンボボックスの初期化
			InitializeComboBox();

			//値が変更された時、秒数リストを更新する
			this.MinuteBox.SelectedValueChanged += MinuteBox_SelectedValueChanged;

			//ファイル情報が記録されていればテキストボックスに書き写す
			//たぶん無駄にチェックしてる
			if (File.Exists(MainForm.FilePath) == true)
			{
				this.FilePathBox.Text = MainForm.FilePath;
			}
			//なければ空文字を入れておく
			else
			{
				this.FilePathBox.Text = "";
			}
		}

		/// <summary>
		/// コンボボックスを録音時間に応じて初期化します。
		/// </summary>
		private void InitializeComboBox()
		{
			int minute = 0;
			int sec = 0;

			//コンボボックスをドロップダウンリストに変更
			this.MinuteBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.SecBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.MinuteBox.FormatString = "00";
			this.SecBox.FormatString = "00";

			//録音時間が1分以上のときのドロップダウンリストの初期化
			if(MainForm.RecordingTime > 59)
			{
				minute = MainForm.RecordingTime / 60;
				sec = MainForm.RecordingTime % 60;

				//分ドロップダウンリストの初期化
				for (int i = 0; i <= 60; i++)
				{
					this.MinuteBox.Items.Add(i);
				}

				//60分のとき秒ドロップダウンリストを0に
				if (minute == 60)
				{
					this.SecBox.Items.Add(0);
				}
				//1~59分のとき
				else
				{
					//秒ドロップダウンリストの初期化
					for (int i = 0; i < 60; i++)
					{
						this.SecBox.Items.Add(i);
					}
				}

				//選択インデックスを更新
				this.MinuteBox.SelectedIndex = minute;
				this.SecBox.SelectedIndex = sec;
			}
			//録音時間が1分未満のときのドロップダウンリストの初期化
			else
			{
				minute = 0;
				sec = MainForm.RecordingTime;


				//分ドロップダウンリストの初期化
				for(int i = 0; i <= 60; i++)
				{
					this.MinuteBox.Items.Add(i);
				}
				//秒ドロップダウンリストの初期化
				for (int i = 1; i < 60; i++)
				{
					this.SecBox.Items.Add(i);
				}

				//選択インデックスを更新
				this.MinuteBox.SelectedIndex = minute;
				this.SecBox.SelectedIndex = sec - 1;
			}
		}


		/// <summary>
		/// 分数のドロップダウンリストが変更されると選択された分数に応じた秒数のリストを用意します。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void MinuteBox_SelectedValueChanged(object sender, EventArgs e)
		{
			int minute = (int)this.MinuteBox.SelectedItem;
			int sec = (int)this.SecBox.SelectedItem;

			//分が60なら秒数選択不可
			if(minute == 60)
			{
				this.SecBox.Items.Clear();
				this.SecBox.Items.Add(0);
				this.SecBox.SelectedIndex = 0;
			}
			//分が0なら0秒選択不可
			else if (minute == 0)
			{
				//ボックスのクリア
				this.SecBox.Items.Clear();

				//ボックスの初期化
				for (int i = 1; i < 60; i++)
				{
					this.SecBox.Items.Add(i);
				}
				//初期化後のインデックスを調整（以前の値を設定）
				if (sec > 0)
				{
					this.SecBox.SelectedIndex = sec - 1;
				}
				else
				{
					this.SecBox.SelectedIndex = sec;
				}
			}
			//分が1~59の間
			else
			{
				//分が0または分は60未満で秒数選択肢が0しかないとき初期化
				if ( ((int)this.SecBox.Items[0] == 1) || (minute != 60 && this.SecBox.Items.Count <= 1))
				{
					//ボックスのクリア
					this.SecBox.Items.Clear();

					//ボックスの初期化
					for (int i = 0; i < 60; i++)
					{
						this.SecBox.Items.Add(i);
					}
					//インデックスの更新
					this.SecBox.SelectedIndex = sec;
				}
			}
		}


		/// <summary>
		/// 参照ボタンを押したときの動作が記述されています。
		/// 参照ダイアログを開き、FilePathBoxに値を格納します。
		/// </summary>
		/// <param name="sender">イベントの発生源</param>
		/// <param name="e">イベントの内容</param>
		private void ReferenceButton_Click(object sender, EventArgs e)
		{
			var ofd = new OpenFileDialog();

			ofd.InitialDirectory = Directory.GetCurrentDirectory() + @"\wave";

			ofd.Filter = "WAVEファイル(*.wav)|*.wav";

			ofd.FilterIndex = 0;

			ofd.Title = "開くファイルを選択してください";

			ofd.RestoreDirectory = true;

			ofd.CheckFileExists = true;

			ofd.CheckPathExists = true;

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				this.FilePathBox.Text = ofd.FileName;
			}
		}


		/// <summary>
		/// OKボタン処理
		/// setting.iniに書き込みしてフォームを閉じる処理を行う
		/// </summary>
		/// <param name="sender">イベントの発生源</param>
		/// <param name="e">イベントの内容</param>
		private void OkButton_Click(object sender, EventArgs e)
		{
			//分と秒を計算して秒数に直す
			int minute = (int)this.MinuteBox.SelectedItem;
			int sec = (int)this.SecBox.SelectedItem;
			int data = (minute * 60) + sec;

			//録音時間に変更
			MainForm.RecordingTime = data;
			MainForm.FilePath = this.FilePathBox.Text;

			this.Close();
		}


		/// <summary>
		/// Cancelボタンが押されると入力された値を無視してフォームを閉じます。
		/// </summary>
		/// <param name="sender">イベントの発生源</param>
		/// <param name="e">イベントの内容</param>
		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
