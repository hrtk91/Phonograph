using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Phonograph
{
	/// <summary>
	/// 録音を実行するためのフォーム
	/// </summary>
	public partial class RecForm : Form
	{
		#region フィールド


		/// <summary>
		/// 現在の録音時間
		/// </summary>
		private int TimeCount;
		/// <summary>
		/// 総録音時間
		/// </summary>
		private int RecTime;
		/// <summary>
		/// 総録音時間の文字列
		/// </summary>
		private string RecTimeStr;

		/// <summary>
		/// 保存用ファイルパス
		/// </summary>
		private string SavePath;

		/// <summary>
		/// 録音が中止されたときTrueをセット。
		/// </summary>
		private bool IsCancel;
		/// <summary>
		/// 録音中止処理が実行された後、trueをセット。
		/// </summary>
		private bool AlreadyCanceled;
		/// <summary>
		/// 録音が終了したときTrueをセット。
		/// </summary>
		private bool IsEnd;

		/// <summary>
		/// 録音用クラスの変数
		/// </summary>
		WaveIn wi;
		/// <summary>
		/// 録音デバイス初期化用情報構造体。
		/// </summary>
		WaveIn.WAVEFORMATEX wfe;
		/// <summary>
		/// WAVEヘッダ構造体
		/// </summary>
		WaveIn.WAVEHDR whdr;


		#endregion


		/// <summary>
		/// フォームを初期化します。
		/// </summary>
		public RecForm()
		{
			InitializeComponent();

			//録音時間を記憶
			this.RecTime = MainForm.RecordingTime;
			//保存するファイルパス
			this.SavePath = Directory.GetCurrentDirectory() + @"\wave\" + DateTime.Now.ToString("yyyyMMddHHmmss") +".wav";


			//総録音時間を○分○秒の形に直した文字列を用意
			this.RecTimeStr = (this.RecTime / 60).ToString("00") + ":" + (this.RecTime % 60).ToString("00");
			//タイトルとラベルを初期化
			this.Text = "録音中00:00/" + this.RecTimeStr;
			this.TimeLabel.Text = "00:00/" + this.RecTimeStr;


			//プログレスバーセッティング
			this.progressBar1.Maximum = this.RecTime;
			this.progressBar1.Minimum = 0;


			//フラグ初期化
			this.IsCancel = false;
			this.AlreadyCanceled = false;
			this.IsEnd = false;


			//PCM形式指定
			this.wfe.wFormatTag = 0x0001;
			//チャンネル数
			this.wfe.nChannels = WaveIn.CHANNELS;
			//量子化ビット数(16bit)
			this.wfe.wBitsPerSample = (ushort)WaveIn.QUANTUMBIT;
			//標本化周波数
			this.wfe.nSamplesPerSec = WaveIn.SRATE;
			//チャンネルを考慮したサンプル毎のバイト	1サンプル＝QUANTUMBIT * CHANNELS
			this.wfe.nBlockAlign = (ushort)(wfe.wBitsPerSample / WaveIn.BYTE * wfe.nChannels);
			//平均bps値	サンプリングレート×nBlockAlign
			this.wfe.nAvgBytesPerSec = wfe.nSamplesPerSec * wfe.nBlockAlign;
			//wFormatTagに情報を追加する変数。今のところ使用する必要なし。
			this.wfe.cbSize = 0;


			//録音開始
			this.wi = new WaveIn(this.Handle, this.RecTime, this.SavePath);
			//録音デバイスオープンを試みる
			try
			{
				this.wi.WaveInOpen(ref this.wfe);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "エラー");

				//終了処理を記述
				this.Activated +=
					(sender, exception) =>
					{
						this.DeleteCreatedFile();
						this.Close();
					};
			}
		}

		/// <summary>
		/// デストラクタ。
		/// アンマネージメモリの解放を行います。
		/// </summary>
		~RecForm()
		{
			//バッファの解放
			Marshal.FreeHGlobal(this.whdr.lpData);
		}

		/// <summary>
		/// キャンセルボタン処理。
		/// フラグを立てるだけ。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.IsCancel = true;
			this.CANCEL.Enabled = false;
		}


		/// <summary>
		/// メッセージを受け取るメソッド。
		/// </summary>
		/// <param name="m">メッセージが格納されます。</param>
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			switch (m.Msg)
			{
				//WaveInOpen()によって送られてくるメッセージ
				case WaveIn.MM_WIM_OPEN:

					try
					{
						//バッファを用意
						this.whdr.lpData = Marshal.AllocHGlobal(WaveIn.SRATE * WaveIn.QUANTUMBIT / WaveIn.BYTE);
						//バッファサイズセット
						this.whdr.dwBufferLength = WaveIn.SRATE * WaveIn.QUANTUMBIT / WaveIn.BYTE;
						//フラグを初期化
						this.whdr.dwFlags = 0;

						//ヘッダを準備してバッファに入れる
						this.wi.WaveInPrepareHeader(ref this.whdr);
						this.wi.WaveInAddBuffer();
						//ファイルを作成しヘッダ情報を書き込む
						this.wi.HeaderWrite();

						//録音スタート
						this.wi.WaveInStart();
					}
					catch (Exception e)
					{
						this.CatchException(e);
					}

					break;

				//バッファがいっぱいになったとき送られてくるメッセージ
				case WaveIn.MM_WIM_DATA:

					//中止フラグと終了フラグを見る
					if ( (this.IsCancel == false) && (this.IsEnd == false) )
					{

						//録音時間に達したなら、バッファに残ったデータを書き込み、録音を終了する
						if (this.TimeCount++ >= this.RecTime)
						{
							//終了のフラグを立てる
							this.IsEnd = true;
							this.CANCEL.Enabled = false;
							//デバイス解放を試行
							try
							{
								//録音中止
								this.wi.WaveInReset();
								//現在のバッファのデータを書き込み
								this.wi.DataWrite(this.whdr.lpData);
								//ヘッダを解放
								this.wi.WaveInUnprepareHeader();
								//デバイスを閉じる
								this.wi.WaveInClose();
								//書き込みに使用したファイルストリームをクローズ
								this.wi.CloseWriter();
							}
							catch (Exception e)
							{
								this.CatchException(e);
							}
						}
						//まだならバッファを入れ替えて録音を継続
						else
						{
							try
							{
								//バッファのデータを書き込み
								this.wi.DataWrite(this.whdr.lpData);

								//バッファを初期化して再度追加
								this.wi.WaveInUnprepareHeader();
								this.wi.WaveInPrepareHeader(ref this.whdr);
								this.wi.WaveInAddBuffer();

								//プログレスバーの更新
								this.progressBar1.Maximum++;
								this.progressBar1.Value = this.TimeCount + 1;
								this.progressBar1.Value = this.TimeCount;
								this.progressBar1.Maximum--;

								//再生時間の更新
								string nowtime = (this.TimeCount / 60).ToString("00") + ":" + (this.TimeCount % 60).ToString("00");
								this.Text = "録音中" + nowtime + "/" + this.RecTimeStr;
								this.TimeLabel.Text = nowtime + "/" + this.RecTimeStr;
							}
							catch (Exception e)
							{
								this.CatchException(e);
							}
						}

					}
					//中止フラグか、終了フラグのどちらかが立っていたら
					else
					{
						//すでにキャンセル処理がなされていたら二度実行しない
						if ( (this.AlreadyCanceled == false) && (this.IsEnd == false) )
						{
							//以下の処理は一度で十分なので、それ以降実行しないためのフラグ
							this.AlreadyCanceled = true;

							try
							{
								//録音の停止、作成されたファイルの消去
								this.wi.WaveInReset();
								this.wi.WaveInUnprepareHeader();
								this.wi.WaveInClose();
								this.wi.CloseWriter();
								this.DeleteCreatedFile();
								this.SavePath = "";
							}
							catch (Exception e)
							{
								this.CatchException(e);
							}
						}
					}

					break;

				//WaveInClose()関数により送られるメッセージ
				case WaveIn.MM_WIM_CLOSE:
					//フラグごとにメッセージを表示
					if (this.IsEnd == true)
					{
						MessageBox.Show("録音終了", "Completed");
						//保存に成功したらファイルパスを保存
						MainForm.FilePath = this.SavePath;
					}
					else if (this.IsCancel == true)
					{
						//キャンセル時の空処理
					}
					else
					{
						//例外処理
						var e = new Exception("予期しないエラーが発生しました。");
						this.CatchException(e);
					}

					this.Close();
					break;

				default:
					//メッセージ処理は必要な物以外は流すため、無処理
					break;
			}
		}


		/// <summary>
		/// 例外発生時に例外オブジェクトのメッセージを表示して終了します。アンマネージメモリの解放や作られた録音ファイルの削除も同時に行います。
		/// </summary>
		/// <param name="e"></param>
		private void CatchException(Exception e)
		{
			//ファイルストリームを閉じる
			this.wi.CloseWriter();
			//ファイルを削除
			this.DeleteCreatedFile();
			//メッセージを表示して終了
			MessageBox.Show(e.Message + "\r\nエラーが発生したため録音を中断します。", "エラー");

			this.Close();
		}

		/// <summary>
		/// 録音したファイルを消します
		/// </summary>
		private void DeleteCreatedFile()
		{
			//ファイルが存在すれば削除
			if (File.Exists(this.SavePath) == true)
			{
				File.Delete(this.SavePath);
			}
		}

	}
}
