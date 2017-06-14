using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Phonograph
{
	public class IniFile
	{

		#region DLLImport


		[DllImport("kernel32.dll")]
		private static extern int GetPrivateProfileString(
			string lpApplicationName,
			string lpKeyName,
			string lpDefault,
			StringBuilder lpReturnedstring,
			int nSize,
			string lpFileName);

		[DllImport("kernel32.dll")]
		private static extern int WritePrivateProfileString(
			string lpApplicationName,
			string lpKeyName,
			string lpstring,
			string lpFileName);


		#endregion


		private string FilePath;


		/// <summary>
		/// 引数のファイルを読み込んでクラスを初期化します。
		/// ファイルがなければ作成します。
		/// </summary>
		/// <param name="filePath"></param>
		public IniFile(string filePath)
		{
			this.FilePath = filePath;
		}


		/// <summary>
		/// sectionとkey情報から目的のデータを読み書きします。
		/// </summary>
		/// <param name="section">iniファイルのセクション。</param>
		/// <param name="key">iniファイルのキー。</param>
		/// <returns>読み取った値が返されます。</returns>
		public string this[string section, string key]
		{
			set
			{
				WritePrivateProfileString(section, key, value, FilePath);
			}
			get
			{
				StringBuilder sb = new StringBuilder(256);
				GetPrivateProfileString(section, key, string.Empty, sb, sb.Capacity, FilePath);
				return sb.ToString();
			}
		}
	}
}
