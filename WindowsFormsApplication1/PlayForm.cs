using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Phonograph
{
	/// <summary>
	/// 再生用フォーム
	/// </summary>
	public partial class PlayForm : Form
	{

		/// <summary>
		/// WaveOutインスタンス用メンバ
		/// </summary>
		private WaveOut wo;
		/// <summary>
		/// 総再生時間
		/// </summary>
		private int PlayBackTime;
		/// <summary>
		/// 総再生時間の文字列。
		/// 形式は00:00です。
		/// </summary>
		private string PlayBackTimeSTR;
		/// <summary>
		/// 再生時間のカウント
		/// </summary>
		private int ReadTimeCount;
		/// <summary>
		/// キャンセルフラグ
		/// </summary>
		private bool IsCancel;
		/// <summary>
		/// すでにキャンセルフラグが立っているかを示すフラグ
		/// </summary>
		private bool IsAlreadyCancel;

        private const int BUFFERSIZERATE = 10;

		/// <summary>
		/// フォームを初期化して再生を開始します。
		/// </summary>
		public PlayForm()
		{
			InitializeComponent();
			
			//インスタンスをつくる
			this.wo = new WaveOut(this.Handle, MainForm.FilePath);
			//キャンセルフラグを初期化
			this.IsCancel = false;
			this.IsAlreadyCancel = false;

			this.FormClosing += (sender, exception) => this.wo.CloseReader();

			//再生を開始
			try
			{
				this.wo.WaveOutOpen();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message + "\r\nエラーが発生したので再生を中止します。", "エラー");
				//フォーム表示後に終了（表示前だと例外発生する）
				this.Activated += (sender, exception) => this.Close();
			}
		}

		/// <summary>
		/// メッセージを処理するメソッド。
		/// </summary>
		/// <param name="m">メッセージオブジェクト</param>
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			switch (m.Msg)
			{
				//WavOutOpen()により送られるメッセージ
				case WaveOut.MM_WOM_OPEN:

					try
					{
						//waveファイルのヘッダを読み込んで再生時間を得る
						this.PlayBackTime = this.wo.ReadHeader();

						//プログレスバー初期化
						this.progressBar1.Maximum = this.PlayBackTime;
						this.progressBar1.Minimum = 0;

						//総再生時間を文字列化
						this.PlayBackTimeSTR = (this.PlayBackTime / 60).ToString("00") + ":" + (this.PlayBackTime % 60).ToString("00");
						//タイトルとラベルを初期化
						this.Text = "再生中00:00/" + this.PlayBackTimeSTR;
						this.TimeLabel.Text = "00:00/" + this.PlayBackTimeSTR;

						//データを読み込んで再生開始
						this.wo.ReadData();
						this.wo.WaveOutPrepareHeader();
						this.wo.WaveOutWrite();

						//次のデータも読み込み、バッファに追加する
						this.wo.SetNextBuffer();
					}
					catch (Exception e)
					{
						this.CatchException(e);
					}

					break;

				//バッファが空になると送られるメッセージ
				case WaveOut.MM_WOM_DONE:

					//再生中止フラグが立っているか
					if (this.IsCancel == false)
					{
						//再生時間が総再生時間を越えていないかチェック
						if (this.ReadTimeCount++ < this.PlayBackTime)
						{
							try
							{
								//次のバッファをセット
								this.wo.SetNextBuffer();

								//プログレスバーの更新
								this.progressBar1.Maximum++;
								this.progressBar1.Value = this.ReadTimeCount + 1;
								this.progressBar1.Value = this.ReadTimeCount;
								this.progressBar1.Maximum--;

								//タイトルとラベルの更新
								string nowtime = (this.ReadTimeCount / 60).ToString("00") + ":" + (this.ReadTimeCount % 60).ToString("00");
								this.Text = "再生中" + nowtime + "/" + this.PlayBackTimeSTR;
								this.TimeLabel.Text = nowtime + "/" + this.PlayBackTimeSTR;
							}
							catch (Exception e)
							{
								this.CatchException(e);
							}
						}

						//再生時間＞＝総再生時間なら再生停止
						else
						{
							//デバイス解放を試行
							try
							{
								//キャンセルボタン無効化
								this.CANCEL.Enabled = false;
								//停止処理
								this.wo.WaveOutReset();
								this.wo.WaveOutUnprepareHeader();
								this.wo.WaveOutClose();
							}
							catch (Exception e)
							{
								this.CatchException(e);
							}
						}
					}
					//再生中止フラグが立っていたら中止処理
					//IsCancel == true
					else
					{
						//一度だけ実行するための防波堤
						if (this.IsAlreadyCancel == false)
						{
							this.IsAlreadyCancel = true;

							//デバイスの解放を試行
							try
							{
								this.wo.WaveOutReset();
								this.wo.WaveOutUnprepareHeader();
								this.wo.WaveOutClose();
							}
							catch (Exception e)
							{
								this.CatchException(e);
							}
						}
					}

					break;

				//WaveOutClose()より送られるメッセージ
				case WaveOut.MM_WOM_CLOSE:

					if (this.IsCancel == false)
					{
						MessageBox.Show("再生が終了しました", "Completed");
					}
					this.Close();
					break;

				default:
					//メッセージ処理は必要な物以外は流すため、無処理
					break;
			}
		}

		/// <summary>
		/// キャンセル処理を行います。
		/// </summary>
		/// <param name="sender">イベントの発生源</param>
		/// <param name="e">イベントの内容</param>
		private void CANCEL_Click(object sender, EventArgs e)
		{
			//中止処理を行う
			this.IsCancel = true;
			this.CANCEL.Enabled = false;
		}


		/// <summary>
		/// 例外発生時に読み込み用のストリームをクローズし、エラー概要を表示します。
		/// </summary>
		/// <param name="e"></param>
		private void CatchException(Exception e)
		{
			MessageBox.Show(e.Message + "\r\nエラーが発生したので再生を中止します。", "エラー");
			this.Close();
		}
	}
}
