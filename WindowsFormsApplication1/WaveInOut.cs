using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace Phonograph
{


	/// <summary>
	/// waveIn関数群をラップしてみたクラス
	/// </summary>
	class WaveIn
	{
		/// <summary>
		/// WaveIn関数群の返り値
		/// </summary>
		public enum MMRESULT
		{
			MMSYSERR_NOERROR = 0,
			MMSYSERR_ERROR = (MMSYSERR_NOERROR + 1),
			MMSYSERR_BADDEVICEID = (MMSYSERR_NOERROR + 2),
			MMSYSERR_NOTENABLED = (MMSYSERR_NOERROR + 3),
			MMSYSERR_ALLOCATED = (MMSYSERR_NOERROR + 4),
			MMSYSERR_INVALHANDLE = (MMSYSERR_NOERROR + 5),
			MMSYSERR_NODRIVER = (MMSYSERR_NOERROR + 6),
			MMSYSERR_NOMEM = (MMSYSERR_NOERROR + 7),
			MMSYSERR_NOTSUPPORTED = (MMSYSERR_NOERROR + 8),
			MMSYSERR_BADERRNUM = (MMSYSERR_NOERROR + 9),
			MMSYSERR_INVALFLAG = (MMSYSERR_NOERROR + 10),
			MMSYSERR_INVALPARAM = (MMSYSERR_NOERROR + 11),
			MMSYSERR_HANDLEBUSY = (MMSYSERR_NOERROR + 12),
			MMSYSERR_INVALIDALIAS = (MMSYSERR_NOERROR + 13),
			MMSYSERR_BADDB = (MMSYSERR_NOERROR + 14),
			MMSYSERR_KEYNOTFOUND = (MMSYSERR_NOERROR + 15),
			MMSYSERR_READERROR = (MMSYSERR_NOERROR + 16),
			MMSYSERR_WRITEERROR = (MMSYSERR_NOERROR + 17),
			MMSYSERR_DELETEERROR = (MMSYSERR_NOERROR + 18),
			MMSYSERR_VALNOTFOUND = (MMSYSERR_NOERROR + 19),
			MMSYSERR_NODRIVERCB = (MMSYSERR_NOERROR + 20),
			MMSYSERR_MOREDATA = (MMSYSERR_NOERROR + 21),
			MMSYSERR_LASTERROR = (MMSYSERR_NOERROR + 21)
		}
		/// <summary>
		/// WAVEHDR構造体のdwFlagsがとる値
		/// </summary>
		public enum WAVEHDR_FLAG
		{
			WHDR_DONE = 0x00000001,
			WHDR_PREPARED = 0x00000002,
			WHDR_BEGINLOOP = 0x00000004,
			WHDR_ENDLOOP = 0x00000008,
			WHDR_INQUEUE = 0x00000010
		}
		/// <summary>
		/// コールバックプロシージャに送られるメッセージ
		/// </summary>
		public enum WaveInMessage
		{
			OPEN = 0x3BE,
			DATA = 0x3C0,
			CLOSE = 0x3BF
		}

		#region DLL使用定義


		/// <summary>
		/// マーシャリングされたWAVEFORMATEX構造体
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct WAVEFORMATEX
		{
			public ushort wFormatTag;
			public ushort nChannels;
			public uint nSamplesPerSec;
			public uint nAvgBytesPerSec;
			public ushort nBlockAlign;
			public ushort wBitsPerSample;
			public ushort cbSize;
		}

		/// <summary>
		/// マーシャリングされたWAVEHDR構造体
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct WAVEHDR
		{
			public IntPtr lpData;
			public uint dwBufferLength;
			public uint dwBytesRecorded;
			public uint dwUser;
			public uint dwFlags;
			public uint dwLoops;
			public IntPtr lpNext;
			public uint reserved;
		}

		/// <summary>
		/// 入力デバイスをオープンします
		/// </summary>
		/// <param name="phwi">デバイスのIDを格納するポインタ</param>
		/// <param name="uDeviceID">デバイス識別子</param>
		/// <param name="pwfx">初期化用構造体</param>
		/// <param name="dwCallback">コールバック先のハンドル</param>
		/// <param name="dwCallbackInstance">コールバック先に渡す値</param>
		/// <param name="fdwOpen">コールバック先の指定。CALLBACK_xxx形式定数</param>
		/// <returns>MM_SYSERR定数が返ります。</returns>
		[DllImport("winmm.dll")]
		private static extern MMRESULT waveInOpen(ref IntPtr phwi, uint uDeviceID, ref WAVEFORMATEX pwfx, IntPtr dwCallback, uint dwCallbackInstance, uint fdwOpen);
		/// <summary>
		/// ヘッダをバッファに格納前の準備をします。
		/// </summary>
		/// <param name="hwi">使用するデバイスのID</param>
		/// <param name="pwh">WAVEHDR構造体（ヘッダ）のポインタ</param>
		/// <param name="cbwh">WAVEHDR構造体のサイズ</param>
		/// <returns>MM_SYSERR定数が返ります。</returns>
		[DllImport("winmm.dll")]
		private static extern MMRESULT waveInPrepareHeader(IntPtr hwi, IntPtr pwhdr, uint cbwh);
		/// <summary>
		/// waveInPrepareHeader()にて準備されたヘッダをバッファに追加します。
		/// </summary>
		/// <param name="hwi">使用する入力デバイスID</param>
		/// <param name="pwh">追加するWAVEHDR構造体ヘッダのポインタ</param>
		/// <param name="cbwh">WAVEHDR構造体のサイズ</param>
		/// <returns>MM_SYSERR定数が返ります。</returns>
		[DllImport("winmm.dll")]
		private static extern MMRESULT waveInAddBuffer(IntPtr hwi, IntPtr pwhdr, uint cbwh);
		/// <summary>
		/// 録音を開始します。
		/// </summary>
		/// <param name="hwi">使用する入力デバイスID</param>
		/// <returns>MM_SYSERR定数が返ります。</returns>
		[DllImport("winmm.dll")]
		private static extern MMRESULT waveInStart(IntPtr hwi);
		/// <summary>
		/// 録音を停止します。未使用のバッファはそのままバッファに残ります。
		/// </summary>
		/// <param name="hwi">使用する入力デバイスID</param>
		/// <returns>MM_SYSERR定数が返ります。</returns>
		[DllImport("winmm.dll")]
		private static extern MMRESULT waveInStop(IntPtr hwi);
		/// <summary>
		/// waveInPrepareHeader()にて準備されたヘッダを初期化します。
		/// MM_WIM_DONEメッセージが送られた時、ヘッダはバッファから解放されていない可能性があります。
		/// </summary>
		/// <param name="hwi">使用する入力デバイスID</param>
		/// <param name="pwh">初期化したいWAVEHDR構造体へのポインタ</param>
		/// <param name="cbwh">WAVEHDR構造体のサイズ</param>
		/// <returns>MM_SYSERR定数が返ります。</returns>
		[DllImport("winmm.dll")]
		private static extern MMRESULT waveInUnprepareHeader( IntPtr hwi, IntPtr pwh, uint cbwh);
		/// <summary>
		/// 入力を中止します。バッファにあるヘッダも使用済みとして処理するので、
		/// バッファに残ったヘッダ数のMM_WIM_DONEメッセージが送られます。
		/// </summary>
		/// <param name="hwi">使用する入力デバイスID。</param>
		/// <returns>MM_SYSERR定数が返ります。</returns>
		[DllImport("winmm.dll", CallingConvention = CallingConvention.StdCall)]
		private static extern MMRESULT waveInReset(IntPtr hwi);
		/// <summary>
		/// 入力デバイスをクローズします。MM_WIM_CLOSEメッセージが送られます。
		/// </summary>
		/// <param name="hwi">クローズする入力デバイスID。</param>
		/// <returns>MM_SYSERR定数が返ります。</returns>
		[DllImport("winmm.dll", CallingConvention = CallingConvention.StdCall)]
		private static extern MMRESULT waveInClose(IntPtr hwi);


		#endregion

		#region MM_WIM定数の定義

		/// <summary>
		/// 0x3BE
		/// </summary>
		public const int MM_WIM_OPEN = 0x3BE;
		/// <summary>
		/// 0x3C0
		/// </summary>
		public const int MM_WIM_DATA = 0x3C0;
		/// <summary>
		/// 0x3BF
		/// </summary>
		public const int MM_WIM_CLOSE = 0x3BF;

		#endregion

		#region 定数の定義


		/// <summary>
		/// サンプリングレート
		/// </summary>
		public const int SRATE = 44100;
		/// <summary>
		/// 量子ビット数
		/// </summary>
		public const int QUANTUMBIT = 16;
		/// <summary>
		/// 録音チャンネル数
		/// </summary>
		public const int CHANNELS = 1;
		/// <summary>
		/// 一バイトあたりのビット数
		/// </summary>
		public const int BYTE = 8;
		/// <summary>
		/// ウィンドウにメッセージを送るときの定数
		/// </summary>
		public const uint CALLBACK_WINDOW = 0x10000;
		/// <summary>
		/// 関数にメッセージを送るときの定数
		/// </summary>
		public const uint CALLBACK_FUNCTION = 0x30000;

		#endregion

		#region フィールド


		/// <summary>
		/// 入力デバイスのID？
		/// </summary>
		private IntPtr WaveInHandle;
		/// <summary>
		/// クラス作成元のウィンドウハンドル
		/// </summary>
		private IntPtr WindowHandle;
		/// <summary>
		/// WAVEHDRヘッダへのポインタ
		/// </summary>
		private IntPtr WaveHeaderPtr;

		/// <summary>
		/// 目標録音時間
		/// </summary>
		private uint RecTime { get; set; }
		/// <summary>
		/// 保存するファイルのパス
		/// </summary>
		private string FilePath { get; set; }

		/// <summary>
		/// 書き込み用バッファ
		/// </summary>
		private short[] WriteBuffer;
		/// <summary>
		/// 書き込み用ストリーム
		/// </summary>
		private FileStream fs;
		/// <summary>
		/// 書き込み用ライター
		/// </summary>
		private BinaryWriter bw;
		/// <summary>
		/// ライターが閉じられたか示すフラグ
		/// </summary>
		private bool WriterClosed;


		#endregion


		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="handle">インスタンスを作成したウィンドウのハンドル</param>
		/// <param name="recTime">録音時間</param>
		public WaveIn(IntPtr handle, int rectime, string filepath)
		{
			//呼び出し元ハンドルの保存
			this.WindowHandle = handle;

			//録音時間とファイルパスを保存
			this.RecTime = (uint)rectime;
			this.FilePath = filepath;

			//フラグ初期化
			this.WriterClosed = false;

			//書き込み用バッファの用意
			this.WriteBuffer = new short[SRATE];

			//ヘッダ用のポインタにヘッダサイズのアンマネージメモリを確保
			this.WaveHeaderPtr = new IntPtr();
			this.WaveHeaderPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WAVEHDR)));
		}

		/// <summary>
		/// デストラクタ。
		/// アンマネージメモリの解放を任せる。
		/// </summary>
		~WaveIn()
		{
			Marshal.FreeHGlobal(this.WaveHeaderPtr);
		}


		#region WaveInをラップしたメソッド

		/// <summary>
		/// クラス作成時に指定したハンドルを使って入力デバイスをオープンします。
		/// MM_WIM_OPENメッセージが指定されたハンドルに送られます。
		/// </summary>
		public void WaveInOpen(ref WAVEFORMATEX wfe)
		{
			//音声デバイスをオープン
			MMRESULT result = waveInOpen( ref this.WaveInHandle, 0, ref wfe, this.WindowHandle, 0, CALLBACK_WINDOW);

			//エラーが発生したら例外を発生させる
			if(result != MMRESULT.MMSYSERR_NOERROR)
			{
				//録音デバイスが見つからない場合
				if(result == MMRESULT.MMSYSERR_BADDEVICEID)
				{
					Exception e = new Exception("WaveInOpen:" + result + "\r\n録音デバイスが見つかりません。\r\n端子を確認してください。");
					throw e;
				}
				//それ以外
				else
				{
					Exception e = new Exception("WaveInOpen:" + result + "\r\n録音デバイスのオープンに失敗しました。");
					throw e;
				}
			}
		}


		/// <summary>
		/// wavInヘッダをひとつ準備します。
		/// 使用前にWAVEHDR構造体のlpData,dwBufferLength,dwFlagsを初期化してください。
		/// dwFlags = 0にしてください。
		/// </summary>
		public void WaveInPrepareHeader(ref WAVEHDR whdr)
		{
			//初期化
			whdr.dwBytesRecorded = 0;
			whdr.dwFlags = 0;

			//ヘッダの中身をポインタに写す。写す前にアンマネージ領域
			Marshal.StructureToPtr(whdr, this.WaveHeaderPtr, true);


			//エラーが発生したら例外を投げる
			MMRESULT result = waveInPrepareHeader(this.WaveInHandle, this.WaveHeaderPtr, (uint)Marshal.SizeOf(whdr));
			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("WaveInPrepareHeader:" + result + "\r\nエラーが発生しました。");
			}
		}


		/// <summary>
		/// 準備されたヘッダをバッファに割り当てます。
		/// </summary>
		public void WaveInAddBuffer()
		{
			//エラーが発生したら例外を投げる
			MMRESULT result = waveInAddBuffer(this.WaveInHandle, this.WaveHeaderPtr, (uint)Marshal.SizeOf(typeof(WAVEHDR)));
			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("WaveInAddBuffer:" + result + "\r\nエラーが発生しました。");
			}
		}


		/// <summary>
		/// 録音を開始します。
		/// バッファがいっぱいになるとMM_WIM_DATAメッセージが送られます。
		/// </summary>
		public void WaveInStart()
		{
			MMRESULT result = waveInStart(this.WaveInHandle);
			
			//エラーが発生したら例外を投げる
			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("WaveInStart:" + result + "\r\nエラーが発生しました。");
			}

		}


		/// <summary>
		/// 録音を停止します。
		/// バッファは未処理のままです。
		/// </summary>
		public void WaveInStop()
		{
			MMRESULT result = waveInStop(this.WaveInHandle);

			//エラーが発生したら例外を投げる
			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("WaveInStop:" + result + "\r\nエラーが発生しました。");
			}
		}


		/// <summary>
		/// デバイスをクローズします。
		/// 成功するとMM_WIM_CLOSEメッセージが送られます。
		/// </summary>
		public void WaveInClose()
		{
			MMRESULT result = waveInClose(this.WaveInHandle);
			
			//エラーが発生したら例外を投げる
			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("WaveInClose:" + result + "\r\nエラーが発生しました。");
			}
		}


		/// <summary>
		/// 入力を中断する関数。
		/// 未使用バッファは処理されます。
		/// もし、複数のバッファが追加されていた場合、バッファの数だけMM_WIM_DATAメッセージが送られます。
		/// </summary>
		public void WaveInReset()
		{
			MMRESULT result = waveInReset(this.WaveInHandle);

			//エラーが発生したら例外を投げる
			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("WaveInReset:" + result + "\r\nエラーが発生しました。");
			}
		}


		/// <summary>
		/// WaveInPrepareHeader()メソッドで準備したヘッダを解放します。
		/// </summary>
		public void WaveInUnprepareHeader()
		{
			//エラーが発生したら例外を投げる
			MMRESULT result = waveInUnprepareHeader(this.WaveInHandle, this.WaveHeaderPtr, (uint)Marshal.SizeOf(typeof(WAVEHDR)));
			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("WaveInStop:" + result + "\r\nエラーが発生しました。");
			}
		}

		#endregion


		#region ファイル操作関連


		/// <summary>
		/// wavファイル作成時に必要なヘッダ情報
		/// </summary>
		struct WavHeader
		{
			//riff
			public byte[] riffID;
			//ファイルサイズ-8
			public uint size;
			//wave
			public byte[] wavID;
			//fmt
			public byte[] fmtID;
			//fmtチャンクのバイト数
			public uint fmtSize;
			//フォーマット
			public ushort format;
			//チャンネル数
			public ushort channels;
			//サンプリングレート
			public uint sampleRate;
			//データ速度
			public uint bytePerSec;
			//ブロックサイズ
			public ushort blockSize;
			//量子化ビット数(8?)
			public ushort bit;
			//実データ部？
			public byte[] dataID;
			//波形データのバイト数
			public uint dataSize;
		}

		/// <summary>
		/// ファイルストリームを開きヘッダ情報のみのwavファイルを作成します。
		/// 同名のファイルがある場合上書きされます。
		/// ファイルストリームを開くので書き込みが終了したらCloseWriter()を実行してください。
		/// </summary>
		/// <returns>作成したファイルのパス</returns>
		public string HeaderWrite()
		{
			WavHeader Header = new WavHeader();

			//riffID
			Header.riffID = new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' };
			//データサイズ
			Header.size = (SRATE * (QUANTUMBIT / BYTE) * CHANNELS * this.RecTime) + (uint)Marshal.SizeOf(typeof(WavHeader));
			//waveID
			Header.wavID = new byte[] { (byte)'W', (byte)'A', (byte)'V', (byte)'E' };
			//fmtID
			Header.fmtID = new byte[] { (byte)'f', (byte)'m', (byte)'t', (byte)' ' };
			//fmtチャンクのバイト数
			Header.fmtSize = 16;
			//WAVE_FORMAT_PCMであることを示す＝１
			Header.format = 1;
			//チャンネル数
			Header.channels = CHANNELS;
			//サンプリングレート
			Header.sampleRate = SRATE;
			//samplerate/byte/channels
			Header.bytePerSec = SRATE * (QUANTUMBIT / BYTE) * CHANNELS;
			//byte/sample*channels 16bit/stereoなら2*2=4 blockAlignと同等？
			Header.blockSize = (ushort)((QUANTUMBIT / BYTE) * CHANNELS);
			//ビットレート
			Header.bit = (ushort)QUANTUMBIT;
			//dataチャンクを示す
			Header.dataID = new byte[] { (byte)'d', (byte)'a', (byte)'t', (byte)'a' };
			//データ長
			Header.dataSize = SRATE * (QUANTUMBIT / BYTE) * this.RecTime;


			//ヘッダ情報を書き込みます。
			this.fs = new FileStream(this.FilePath, FileMode.Create);
			this.bw = new BinaryWriter(this.fs);

			bw.Write(Header.riffID);
			bw.Write(Header.size);
			bw.Write(Header.wavID);
			bw.Write(Header.fmtID);
			bw.Write(Header.fmtSize);
			bw.Write(Header.format);
			bw.Write(Header.channels);
			bw.Write(Header.sampleRate);
			bw.Write(Header.bytePerSec);
			bw.Write(Header.blockSize);
			bw.Write(Header.bit);
			bw.Write(Header.dataID);
			bw.Write(Header.dataSize);

			return this.FilePath;
		}


		/// <summary>
		/// 事前に用意されたwavファイルにバッファの内容を書き込みます。
		/// 書き込まれる秒数は1秒です(SamplingRate)
		/// </summary>
		public void DataWrite(IntPtr lpData)
		{
			//一秒間のshort型バッファの配列数はSRATE=44100
			int size = (int)(SRATE);

			//バッファの内容を書き込む
			Marshal.Copy(lpData, this.WriteBuffer, 0, size);
			//マネージメモリに写したデータを書き込み
			foreach (var i in this.WriteBuffer)
			{
				this.bw.Write(i);
			}
		}

		/// <summary>
		/// 書き込みに使用したファイルストリームをクローズします。
		/// </summary>
		public void CloseWriter()
		{
			if (this.WriterClosed == false)
			{
				this.WriterClosed = true;
				this.bw.Close();
				this.fs.Close();
			}
		}

		#endregion

	}

	/// <summary>
	/// waveOut関数群をラップしてみたクラス
	/// </summary>
	class WaveOut
	{
		/// <summary>
		/// waveOut関数群の返り値
		/// </summary>
		public enum MMRESULT
		{
			MMSYSERR_NOERROR = 0,
			MMSYSERR_ERROR = (MMSYSERR_NOERROR + 1),
			MMSYSERR_BADDEVICEID = (MMSYSERR_NOERROR + 2),
			MMSYSERR_NOTENABLED = (MMSYSERR_NOERROR + 3),
			MMSYSERR_ALLOCATED = (MMSYSERR_NOERROR + 4),
			MMSYSERR_INVALHANDLE = (MMSYSERR_NOERROR + 5),
			MMSYSERR_NODRIVER = (MMSYSERR_NOERROR + 6),
			MMSYSERR_NOMEM = (MMSYSERR_NOERROR + 7),
			MMSYSERR_NOTSUPPORTED = (MMSYSERR_NOERROR + 8),
			MMSYSERR_BADERRNUM = (MMSYSERR_NOERROR + 9),
			MMSYSERR_INVALFLAG = (MMSYSERR_NOERROR + 10),
			MMSYSERR_INVALPARAM = (MMSYSERR_NOERROR + 11),
			MMSYSERR_HANDLEBUSY = (MMSYSERR_NOERROR + 12),
			MMSYSERR_INVALIDALIAS = (MMSYSERR_NOERROR + 13),
			MMSYSERR_BADDB = (MMSYSERR_NOERROR + 14),
			MMSYSERR_KEYNOTFOUND = (MMSYSERR_NOERROR + 15),
			MMSYSERR_READERROR = (MMSYSERR_NOERROR + 16),
			MMSYSERR_WRITEERROR = (MMSYSERR_NOERROR + 17),
			MMSYSERR_DELETEERROR = (MMSYSERR_NOERROR + 18),
			MMSYSERR_VALNOTFOUND = (MMSYSERR_NOERROR + 19),
			MMSYSERR_NODRIVERCB = (MMSYSERR_NOERROR + 20),
			MMSYSERR_MOREDATA = (MMSYSERR_NOERROR + 21),
			MMSYSERR_LASTERROR = (MMSYSERR_NOERROR + 21)
		}
		/// <summary>
		/// WAVEHDR構造体dwFlagがとりうる値
		/// </summary>
		public enum WAVEHDR_FLAG
		{
			WHDR_DONE = 0x00000001,
			WHDR_PREPARED = 0x00000002,
			WHDR_BEGINLOOP = 0x00000004,
			WHDR_ENDLOOP = 0x00000008,
			WHDR_INQUEUE = 0x00000010
		}
		/// <summary>
		/// コールバックプロシージャに送られるメッセージの値
		/// </summary>
		public enum WaveOutMessage
		{
			OPEN = 0x3BB,
			DONE = 0x3BD,
			CLOSE = 0x3BC
		}

		#region DLL使用定義

		/// <summary>
		/// マーシャリングされたWAVEFORMATEX構造体
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct WAVEFORMATEX
		{
			public ushort wFormatTag;
			public ushort nChannels;
			public uint nSamplesPerSec;
			public uint nAvgBytesPerSec;
			public ushort nBlockAlign;
			public ushort wBitsPerSample;
			public ushort cbSize;
		}

		/// <summary>
		/// マーシャリングされたWAVEHDR構造体
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct WAVEHDR
		{
			public IntPtr lpData;
			public uint dwBufferLength;
			public uint dwBytesRecorded;
			public uint dwUser;
			public uint dwFlags;
			public uint dwLoops;
			public IntPtr lpNext;
			public uint reserved;
		}

		/// <summary>
		/// 出力デバイスをオープンします。
		/// </summary>
		/// <param name="phwo">出力デバイスIDを格納するポインタ</param>
		/// <param name="uDeviceID">デバイス識別ID</param>
		/// <param name="pwfx">初期化用WAVEFORMATEX構造体</param>
		/// <param name="dwCallback">コールバック先ハンドル</param>
		/// <param name="dwCallbackInstance">コールバック先に渡すデータ</param>
		/// <param name="fdwOpen">コースバック先を決める定数。CALLBACK_xxx形式定数</param>
		/// <returns>MM_SYSERR形式定数</returns>
		[DllImport("winmm.dll")]
		private static extern MMRESULT waveOutOpen(ref IntPtr phwo, uint uDeviceID, ref WAVEFORMATEX pwfx, IntPtr dwCallback, uint dwCallbackInstance, uint fdwOpen);
		/// <summary>
		/// バッファ追加用にWAVEHDR構造体を初期化します。
		/// </summary>
		/// <param name="hwo">使用する出力デバイスID</param>
		/// <param name="whdr">WAVEHDR構造体ヘッダへのポインタ</param>
		/// <param name="cbwh">WAVEHDR構造体のサイズ</param>
		/// <returns>MM_SYSERR形式定数</returns>
		[DllImport("winmm.dll")]
		private static extern MMRESULT waveOutPrepareHeader(IntPtr hwo, IntPtr whdr, uint cbwh);
		/// <summary>
		/// バッファにヘッダを追加します。
		/// </summary>
		/// <param name="hwo">使用する出力デバイスID</param>
		/// <param name="whdr">追加するWAVEHDR構造体ヘッダへのポインタ</param>
		/// <param name="cbwh">WAVEHDR構造体のサイズ</param>
		/// <returns>MM_SYSERR形式定数</returns>
		[DllImport("winmm.dll")]
		private static extern MMRESULT waveOutWrite(IntPtr hwo, IntPtr whdr, uint cbwh);
		/// <summary>
		/// waveOutPrepareHeader()にて準備したヘッダを初期化します。
		/// </summary>
		/// <param name="hwo">使用する出力デバイスID</param>
		/// <param name="whdr">初期化するWAVEHDR構造体ヘッダのポインタ</param>
		/// <param name="cbwh">WAVEHDR構造体のサイズ</param>
		/// <returns>MM_SYSERR形式定数</returns>
		[DllImport("winmm.dll")]
		private static extern MMRESULT waveOutUnprepareHeader(IntPtr hwo, IntPtr whdr, uint cbwh);
		/// <summary>
		/// 出力デバイスをクローズします。
		/// </summary>
		/// <param name="hwo">クローズする出力デバイスID</param>
		/// <returns>MM_SYSERR形式定数</returns>
		[DllImport("winmm.dll")]
		private static extern MMRESULT waveOutClose(IntPtr hwo);
		/// <summary>
		/// 再生を中断します。未使用のバッファを使用済みとするため、
		/// 残りのバッファ数に応じたMM_WOM_DATAメッセージが送られます。
		/// </summary>
		/// <param name="hwo">使用する出力デバイスID</param>
		/// <returns>MM_SYSERR形式定数</returns>
		[DllImport("winmm.dll")]
		private static extern MMRESULT waveOutReset(IntPtr hwo);

		#endregion

		#region MM_WOM定数の定義

		public const int MM_WOM_OPEN = 0x3BB;
		public const int MM_WOM_DONE = 0x3BD;
		public const int MM_WOM_CLOSE = 0x3BC;

		#endregion

		#region 定数の宣言


		/// <summary>
		/// サンプリングレート
		/// </summary>
		public const int SRATE = 44100;
		/// <summary>
		/// 量子ビット数
		/// </summary>
		public const int QUANTUMBIT = 16;
		/// <summary>
		/// 録音チャンネル数
		/// </summary>
		public const int CHANNELS = 1;
		/// <summary>
		/// 一バイトあたりのビット数
		/// </summary>
		public const int BYTE = 8;
		/// <summary>
		/// ウィンドウにメッセージを送るときの定数
		/// </summary>
		public const uint CALLBACK_WINDOW = 0x10000;
		/// <summary>
		/// バッファの最大数
		/// </summary>
		public const int BUFFSIZE = 3;


		#endregion

		#region プロパティ

		/// <summary>
		/// 再生デバイスID
		/// </summary>
		private IntPtr WaveOutHandle;
		/// <summary>
		/// クラス作成元のハンドル
		/// </summary>
		private IntPtr WindowHandle;
		/// <summary>
		/// waveOutOpen()初期化時フォーマット構造体
		/// </summary>
		private WAVEFORMATEX wfe;
		/// <summary>
		/// バッファ用ヘッダ
		/// </summary>
		private WAVEHDR[] whdr = new WAVEHDR[BUFFSIZE];
		/// <summary>
		/// ヘッダをアンマネージ領域に確保するための領域
		/// </summary>
		private IntPtr[] WaveHeaderPtr = new IntPtr[BUFFSIZE];
		/// <summary>
		/// バッファ番号が入るフィールド
		/// </summary>
		private int _buffCount = 0;
		/// <summary>
		/// 次のバッファ番号を取得します。
		/// </summary>
		private int NextBuffNum { get; set; }
		/// <summary>
		/// 前のバッファ番号を取得します。
		/// </summary>
		private int PreBuffNum { get; set; }
		/// <summary>
		/// バッファの番号を入出力します。セットするとNextBuffNumとPreBuffNumが自動でセットされます。
		/// </summary>
		public int BuffCount
		{
			get
			{
				return _buffCount;
			}
			set
			{
				if (value >= BUFFSIZE)
				{
					_buffCount = 0;
					NextBuffNum = _buffCount + 1;
					PreBuffNum = BUFFSIZE - 1;
				}
				else if (value == BUFFSIZE - 1)
				{
					_buffCount = value;
					NextBuffNum = 0;
					PreBuffNum = _buffCount - 1;
				}
				else if (value <= 0)
				{
					_buffCount = value;
					NextBuffNum = _buffCount + 1;
					PreBuffNum = BUFFSIZE - 1;
				}
				else
				{
					_buffCount = value;
					NextBuffNum = _buffCount + 1;
					PreBuffNum = _buffCount - 1;
				}
			}
		}

		/// <summary>
		/// 再生するファイルのパス
		/// </summary>
		private string FilePath;
		/// <summary>
		/// 読み込み用ファイルストリーム
		/// </summary>
		private FileStream fs;
		/// <summary>
		/// 読み込み用バイナリリーダ
		/// </summary>
		private BinaryReader br;
		/// <summary>
		/// ファイルリーダが閉じられたか示すフラグ
		/// </summary>
		private bool ReaderClosed;
		/// <summary>
		/// ファイル読み込み用バッファ
		/// </summary>
		private short[] ReadBuffer;
		/// <summary>
		/// ファイル読み込み回数（秒数）
		/// </summary>
		private int FileReadCount;
		/// <summary>
		/// ヘッダ情報保存用構造体
		/// </summary>
		private WavHeader Header;
		/// <summary>
		/// 総再生時間
		/// </summary>
		private int PlayBackTime { get; set; }


		#endregion


		/// <summary>
		/// WaveOutクラスを初期化します
		/// </summary>
		/// <param name="handle">メッセージを送るウィンドウのハンドル</param>
		/// <param name="filepath">再生したいファイルへのパス</param>
		public WaveOut(IntPtr handle, string filepath)
		{
			//呼び出し元ハンドルの保存と録音時間の取得
			this.WindowHandle = handle;


			#region WAVEFORMATEX構造体初期化


			//PCM形式指定？
			this.wfe.wFormatTag = 0x0001;
			//チャンネル数
			this.wfe.nChannels = CHANNELS;
			//量子化ビット数
			this.wfe.wBitsPerSample = (ushort)QUANTUMBIT;
			//標本化周波数
			this.wfe.nSamplesPerSec = SRATE;
			//チャンネルを考慮したサンプル毎のバイト	1サンプル＝QUANTUMBIT * CHANNELS
			this.wfe.nBlockAlign = (ushort)(wfe.wBitsPerSample / BYTE * wfe.nChannels);
			//平均bps値	サンプリングレート×nBlockAlign
			this.wfe.nAvgBytesPerSec = wfe.nSamplesPerSec * wfe.nBlockAlign;
			//wFormatTagに情報を追加する変数。今のところ使用する必要なし。
			this.wfe.cbSize = 0;


			#endregion


			//確保したいメモリのバイト数
			int size = (int)(SRATE * QUANTUMBIT / BYTE);

			try
			{
				//マルチバッファの確保
				for (int i = 0; i < BUFFSIZE; i++)
				{
					//アンマネージメモリを確保する
					this.whdr[i].lpData = Marshal.AllocHGlobal(size);
					this.WaveHeaderPtr[i] = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WAVEHDR)));
					//バッファの長さを設定
					this.whdr[i].dwBufferLength = (uint)size;
					//フラグ初期化
					this.whdr[i].dwFlags = 0;
					//確保した領域に構造体をコピー
					Marshal.StructureToPtr(this.whdr[i], this.WaveHeaderPtr[i], true);
				}
			}
			catch
			{
				throw;
			}
			//バッファ番号を初期化
			this.BuffCount = 0;

			//ファイル読み込み時につかうバッファを用意
			this.ReadBuffer = new short[SRATE];
			//読み込み秒数を初期化
			this.FileReadCount = 0;

			//読み込むファイルのパスを取得
			this.FilePath = filepath;
		}

		/// <summary>
		/// デストラクタ～
		/// </summary>
		~WaveOut()
		{
			for (int i = 0; i < BUFFSIZE; i++)
			{
				try
				{
					Marshal.FreeHGlobal(this.whdr[i].lpData);
					Marshal.FreeHGlobal(this.WaveHeaderPtr[i]);
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message, "メッセージ");
				}
			}
		}


		#region waveOutラップ関数群


		/// <summary>
		/// 再生デバイスをオープンします。
		/// </summary>
		public void WaveOutOpen()
		{
			MMRESULT result = waveOutOpen(
				ref this.WaveOutHandle,
				0,
				ref this.wfe,
				this.WindowHandle,
				0,
				CALLBACK_WINDOW
				);

			//エラーが発生したら例外を投げる
			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("waveOutOpen：" + result);
				e.Data.Add("waveOutOpen", result);
				throw e;
			}
		}

		/// <summary>
		/// ヘッダを準備します。
		/// </summary>
		public void WaveOutPrepareHeader()
		{
			//ヘッダを初期化する
			this.whdr[this.BuffCount].dwBytesRecorded = 0;
			this.whdr[this.BuffCount].dwFlags = 0;
			//初期化したヘッダをアンマネージ領域に書き込む
			Marshal.StructureToPtr(this.whdr[this.BuffCount], this.WaveHeaderPtr[this.BuffCount], true);

			MMRESULT result = waveOutPrepareHeader(
				this.WaveOutHandle,
				this.WaveHeaderPtr[this.BuffCount],
				(uint)Marshal.SizeOf(this.whdr[this.BuffCount])
			);

			//エラーが発生したら例外を投げる
			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("waveOutPrepareHeader：" + result);
				e.Data.Add("waveOutPrepareHeader", result);
				throw e;
			}
		}
		/// <summary>
		/// 次に使われるヘッダの初期化を行います。
		/// </summary>
		public void WaveOutPrepareNextHeader()
		{
			//次のヘッダを初期化
			this.whdr[this.NextBuffNum].dwBytesRecorded = 0;
			this.whdr[this.NextBuffNum].dwFlags = 0;
			//初期化したヘッダをアンマネージ領域に書き込み
			Marshal.StructureToPtr(this.whdr[this.NextBuffNum], this.WaveHeaderPtr[this.NextBuffNum], true);

			MMRESULT result = waveOutPrepareHeader(
				this.WaveOutHandle,
				this.WaveHeaderPtr[this.NextBuffNum],
				(uint)Marshal.SizeOf(typeof(WAVEHDR))
			);

			//エラーが発生したら例外を投げる
			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("waveOutPrepareNextHeader：" + result);
				e.Data.Add("waveOutPrepareNextHeader", result);
				throw e;
			}
		}

		/// <summary>
		/// データを再生バッファに送ります。
		/// </summary>
		public void WaveOutWrite()
		{
			MMRESULT result = waveOutWrite(
				this.WaveOutHandle,
				this.WaveHeaderPtr[this.BuffCount],
				(uint)Marshal.SizeOf(typeof(WAVEHDR))
			);

			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("waveOutWrite：" + result);
				e.Data.Add("waveOutWrite", result);
				throw e;
			}
		}
		/// <summary>
		/// 次のバッファを書き込みます。
		/// </summary>
		public void WaveOutWriteNext()
		{
			MMRESULT result = waveOutWrite(
				this.WaveOutHandle,
				this.WaveHeaderPtr[this.NextBuffNum],
				(uint)Marshal.SizeOf(typeof(WAVEHDR))
			);

			//エラーが発生したら例外を投げる
			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("waveOutWriteNext：" + result);
				e.Data.Add("waveOutWriteNext", result);
				throw e;
			}
		}

		/// <summary>
		/// ヘッダーを解放します。
		/// </summary>
		public void WaveOutUnprepareHeader()
		{
			MMRESULT result = waveOutUnprepareHeader(
				this.WaveOutHandle,
				this.WaveHeaderPtr[this.BuffCount],
				(uint)Marshal.SizeOf(typeof(WAVEHDR))
			);

			//エラーが発生したら例外を投げる
			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("waveOutUnprepareHeader：" + result);
				e.Data.Add("waveOutUnprepareHeader", result);
				throw e;
			}
		}
		/// <summary>
		/// 次に使われるヘッダを再生デバイスか解放します。
		/// </summary>
		public void WaveOutUnprepareNextHeader()
		{
			MMRESULT result = waveOutUnprepareHeader(
				this.WaveOutHandle,
				this.WaveHeaderPtr[this.NextBuffNum],
				(uint)Marshal.SizeOf(typeof(WAVEHDR))
			);

			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("waveOutUnprepareNextHeader：" + result);
				e.Data.Add("waveOutUnprepareNextHeader", result);
				throw e;
			}
		}


		/// <summary>
		/// 再生デバイスをクローズします。
		/// </summary>
		public void WaveOutClose()
		{
			MMRESULT result = waveOutClose(this.WaveOutHandle);

			//エラーが発生したら例外を投げる
			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("waveOutClose：" + result);
				e.Data.Add("waveOutClose", result);
				throw e;
			}
		}


		/// <summary>
		/// 再生を停止します。
		/// </summary>
		public void WaveOutReset()
		{
			MMRESULT result = waveOutReset(this.WaveOutHandle);

			//エラーが発生したら例外を投げる
			if (result != MMRESULT.MMSYSERR_NOERROR)
			{
				Exception e = new Exception("waveOutReset：" + result);
				e.Data.Add("waveOutReset", result);
				throw e;
			}
		}


		#endregion


		#region バッファ操作


		/// <summary>
		/// 次に使われるヘッダにデータを読み込みます。MM_WOM_DATAで使われることを想定しています。
		/// </summary>
		public void SetNextBuffer()
		{
			//再生時間内なら次のデータを読み込む
			if (this.FileReadCount <= this.PlayBackTime)
			{
				//今からつかうバッファをデバイスから解放
				this.WaveOutUnprepareNextHeader();
				//データを読み込む
				this.ReadDataToNextBuff();
				//準備して
				this.WaveOutPrepareNextHeader();
				//書き込む
				this.WaveOutWriteNext();
				//次に使うバッファの番号を設定する
				this.BuffCount++;
			}
		}


		#endregion


		#region ファイル操作関連


		/// <summary>
		/// wavファイル作成時に必要なヘッダ情報
		/// </summary>
		struct WavHeader
		{
			//riff
			public byte[] riffID;
			//ファイルサイズ-8
			public uint size;
			//wave
			public byte[] wavID;
			//fmt
			public byte[] fmtID;
			//fmtチャンクのバイト数
			public uint fmtSize;
			//フォーマット
			public ushort format;
			//チャンネル数
			public ushort channels;
			//サンプリングレート
			public uint sampleRate;
			//データ速度
			public uint bytePerSec;
			//ブロックサイズ
			public ushort blockSize;
			//量子化ビット数(8?)
			public ushort bit;
			//実データ部の始まりを意味する文字列
			public byte[] dataID;
			//波形データのバイト数
			public uint dataSize;
		}

		/// <summary>
		/// ストリームをオープンしてwavファイルのヘッダ部を読み出します。
		/// それにより総再生時間を割り出します。
		/// 読み込みが終了したらCloseReader()を必ず実行してください。
		/// </summary>
		public int ReadHeader()
		{
			this.Header = new WavHeader();

			this.fs = new FileStream(this.FilePath, FileMode.Open);
			this.br = new BinaryReader(fs);

			this.Header.riffID = br.ReadBytes(4);
			this.Header.size = br.ReadUInt32();
			this.Header.wavID = br.ReadBytes(4);
			//byte(1byte)からchar(2byte),charからstringへ型変換
			char[] charstr = new char[4] { (char)this.Header.wavID[0], (char)this.Header.wavID[1], (char)this.Header.wavID[2], (char)this.Header.wavID[3] };
			string str = new String(charstr);
			//wavファイルでなければ弾く
			if (str != "WAVE")
			{
				throw new ArgumentException(this.FilePath + "には対応していません:wavID");
			}
			this.Header.fmtID = br.ReadBytes(4);
			this.Header.fmtSize = br.ReadUInt32();
			this.Header.format = br.ReadUInt16();
			this.Header.channels = br.ReadUInt16();
			//モノラルじゃなければ弾く
			if (this.Header.channels != CHANNELS)
			{
				throw new ArgumentException(this.FilePath + "には対応していません:channnels");
			}
			this.Header.sampleRate = br.ReadUInt32();
			//サンプリングレートが違うと弾く
			if (this.Header.sampleRate != SRATE)
			{
				throw new ArgumentException(this.FilePath + "には対応していません:SRATE");
			}
			this.Header.bytePerSec = br.ReadUInt32();
			this.Header.blockSize = br.ReadUInt16();
			this.Header.bit = br.ReadUInt16();
			//量子化ビット数が違うと弾く
			if (this.Header.bit != QUANTUMBIT)
			{
				throw new ArgumentException(this.FilePath + "には対応していません:QBIT");
			}
			this.Header.dataID = br.ReadBytes(4);
			this.Header.dataSize = br.ReadUInt32();
			//44バイト読み込み
						
			//ファイルサイズから再生時間を逆算
			this.PlayBackTime = (int)(Header.dataSize / (Header.sampleRate * (Header.bit / BYTE) * Header.channels));
			
			return this.PlayBackTime;
		}

		/// <summary>
		/// 1秒間のデータを読み込んで現在使用されているバッファにコピーします。
		/// </summary>
		public void ReadData()
		{
			//バッファにデータを読み込む
			int size = (int)(SRATE);
			for (int i = 0; i < size; i++)
			{
				this.ReadBuffer[i] = br.ReadInt16();
			}

			//使用中のバッファに書き込む
			Marshal.Copy(this.ReadBuffer, 0, this.whdr[this.BuffCount].lpData, size);
			this.FileReadCount++;
		}

		/// <summary>
		/// 1秒間のデータを読み込んで次に使用されるバッファにコピーします。
		/// </summary>
		public void ReadDataToNextBuff()
		{
			//バッファにデータを読み込む
			int size = (int)(SRATE);
			for (int i = 0; i < size; i++)
			{
				this.ReadBuffer[i] = br.ReadInt16();
			}

			//使用中のバッファに書き込む
			Marshal.Copy(this.ReadBuffer, 0, this.whdr[this.NextBuffNum].lpData, size);
			this.FileReadCount++;
		}

		/// <summary>
		/// ファイルリードに使用したストリームをクローズします。
		/// 終了処理で必ず行ってください。
		/// </summary>
		public void CloseReader()
		{
			if (this.ReaderClosed == false)
			{
				this.ReaderClosed = true;
				this.br.Close();
				this.fs.Close();
			}
		}

		#endregion


	}


}
