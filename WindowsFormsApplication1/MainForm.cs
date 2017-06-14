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
	/// メインフォーム
	/// </summary>
    public partial class MainForm : Form
	{
		#region フィールド
		/// <summary>
		/// FilePathのフィールド
		/// </summary>
		private static string _filePath;
		/// <summary>
		/// ファイルパスを設定または取得します。もしファイルが無ければ空文字が入ります。
		/// </summary>
		public static string FilePath
		{
			get
			{
				return _filePath;
			}
			set
			{
				if (File.Exists(value) == true)
				{
					_filePath = value;
				}
				else
				{
					_filePath = "";
				}
			}
		}

		/// <summary>
		/// FilePathより読み出したファイル名を保存するフィールド
		/// </summary>
		private string _fileName;
		/// <summary>
		/// FilePathからファイル名のみ抜き取って_fileNameフィールドに格納するアクセサ
		/// </summary>
		public string FileName
		{
			get
			{
				return _fileName;
			}
			set
			{
				//ファイルかどうかのチェックしなさい
				_fileName = Path.GetFileName(value);
			}
		}

		/// <summary>
		/// RecordingTimeのフィールド
		/// </summary>
		private static int _recordingTime;
		/// <summary>
		/// iniファイルから読み出した録音時間。
		/// 1~3600以外の値は修正されます。
		/// </summary>
		public static int RecordingTime
		{
			get
			{
				return _recordingTime;
			}
			set
			{
				//値の入力制限1~3600まで
				if (value > 3600)
				{
					_recordingTime = 3600;
				}
				else if (value <= 0)
				{
					_recordingTime = 1;
				}
				else
				{
					_recordingTime = value;
				}
			}
		}
		#endregion


		/// <summary>
		/// フォームを初期化します。
		/// </summary>
        public MainForm()
        {
            InitializeComponent();

			//iniファイルを読み込んで値を変数に保存
			this.ReadSettings();

			int minute = MainForm.RecordingTime / 60;
			int second = MainForm.RecordingTime % 60;

			//ラベルの初期化
			this.RecTimeLabel.Text = "録音時間：" + minute.ToString("00") +"分" + second.ToString("00") + "秒";
			this.PlayFileLabel.Text = "再生ファイル：" + this.FileName;

			//終了時の処理。iniファイルに値を保存している。
			this.FormClosing += MainForm_FormClosing;
        }
		/// <summary>
		/// 終了時に録音時間とファイルパスをiniファイルに保存します。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			DialogResult result = MessageBox.Show("終了しますか？", "メッセージ", MessageBoxButtons.YesNo);
			if (result == DialogResult.Yes)
			{
				IniFile ini = new IniFile("./setting.ini");

				ini["SECTION1", "RecordingTime"] = MainForm.RecordingTime.ToString();
				ini["SECTION1", "FilePath"] = MainForm.FilePath;
			}
			else
			{
				e.Cancel = true;
			}
		}
		/// <summary>
		/// iniファイルから録音時間と再生ファイルパスを得ます。
		/// 値はそれぞれ、MainForm.RecordingTimeとMainForm.FilePathに保存されます。
		/// </summary>
		private void ReadSettings()
		{
			IniFile ini = new IniFile("./setting.ini");
			int rectime;
			string filepath = ini["SECTION1", "FilePath"];

			//iniファイルから録音時間を取得
			if (Int32.TryParse(ini["SECTION1", "RecordingTime"], out rectime) == true)
			{
				MainForm.RecordingTime = rectime;
			}
			else
			{
				MainForm.RecordingTime = 1;
			}

			//iniファイルからファイルパスを取得
			//ちぇっくいらない
			if (File.Exists(filepath) == true)
			{
				MainForm.FilePath = filepath;
			}
			else
			{
				MainForm.FilePath = "";
			}
			this.FileName = MainForm.FilePath;
		}
		/// <summary>
		/// パラメータボタンクリック時の動作が記述されています。
		/// </summary>
		/// <param name="sender">イベントの発生源</param>
		/// <param name="e">イベントの内容</param>
		private void ParametaButton_Click(object sender, EventArgs e)
		{
			//パラメータフォームを開く
			var pf = new ParameterForm();
			pf.ShowDialog();

			//パラメータ設定後、変更された値をラベルに読み込む
			this.RecTimeLabel.Text = "録音時間：" + (MainForm.RecordingTime / 60).ToString("00") + "分" + (MainForm.RecordingTime % 60).ToString("00") + "秒";
			this.FileName = MainForm.FilePath;
			this.PlayFileLabel.Text = "再生ファイル：" + this.FileName;
		}
		/// <summary>
		/// 録音ボタンクリック時の動作が記述されています。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RecButton_Click(object sender, EventArgs e)
		{
			//保存フォルダがなければ作成
			if (Directory.Exists("./wave") != true)
			{
				//フォルダを作成
				Directory.CreateDirectory("./wave");
			}

			//録音フォームを表示
			var rf = new RecForm();
			rf.ShowDialog();

			//録音したファイルのパスを読み込んでテキストボックスに表示
			this.FileName = MainForm.FilePath;
			this.PlayFileLabel.Text = "再生ファイル：" + this.FileName;
		}
		/// <summary>
		/// 再生を開始します。
		/// FilePath変数に書き込まれたファイルパスにファイルがなければエラーになります。
		/// </summary>
		/// <param name="sender">イベントの発生源</param>
		/// <param name="e">イベントの発生源</param>
		private void PlayButton_Click(object sender, EventArgs e)
		{
			//ファイルの存在をチェック
			if (File.Exists(MainForm.FilePath) == true)
			{
				PlayForm pf = new PlayForm();
				pf.ShowDialog();
			}
			else
			{
				MessageBox.Show("再生するファイルをパラメータ設定から指定してください", "エラー");
			}
		}
		/// <summary>
		/// 終了ボタン押下時、確認メッセージを取って終了
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CloseButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

	}

}
