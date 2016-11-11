/* tfwxo * 1/18/2016 * 9:56 PM */
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
namespace on.iff
{
	/// <summary>
	/// <code>ZSTR myZstr = "foo\0"; // Complete setter.</code>
	/// <code>(uint)myZstr;   //casts: string length</code>
	/// <code>(int)myZstr;    //casts: string length</code>
	/// <code>(string)myZstr; //casts: string length</code>
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct ZSTR
	{
		public int Length {
			get {
				return StrValue.Length;
			}
		}

		public byte[] StrValue;

		// ANSI
		public void Write(BinaryWriter writer)
		{
			writer.Write(Length);
			writer.Write(StrValue);
		}

		static public implicit operator int(ZSTR input) {
			return input.Length;
		}

		static public implicit operator uint(ZSTR input) {
			return (uint)input.Length;
		}

		static public implicit operator ZSTR(string input) {
			return new ZSTR {
				StrValue = System.Text.Encoding.Default.GetBytes(input)
			};
		}

		static public implicit operator string(ZSTR input) {
			return System.Text.Encoding.Default.GetString(input.StrValue);
		}
	}
}






