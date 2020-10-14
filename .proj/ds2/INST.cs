/* tfwxo * 1/18/2016 * 9:56 PM */
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
namespace on.iff
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
	struct INST
	// http://www.piclist.com/techref/io/serial/midi/wave.html
	{
    // 'inst'
		public uint ckID;

		public uint ckLength; // 7

    /// (0-127?)
		public sbyte uNote;

    /// -50 to +50
		public byte fineTune;

    /// -128 to +127
		public byte Gain;

		public sbyte noteLow;

		/// (0-127?)
		public sbyte noteHigh;

    // 1 to 127
		public sbyte velLow;

		/// 1 to 127
		public sbyte velHigh;

		public void Write(BinaryWriter writer)
		{
			writer.WriteE(ckID);
			writer.WriteE(ckLength);
			writer.WriteE(uNote);
			writer.WriteE(fineTune);
			writer.WriteE(Gain);
			writer.WriteE(noteLow);
			writer.WriteE(noteHigh);
			writer.WriteE(velLow);
			writer.WriteE(velHigh);
		}
    
		public void Prepare(sbyte note, byte tune, byte gain, sbyte klo, sbyte khi, sbyte vlo = 1, sbyte vhi = 127)
		{
			ckID = ListType.INST;
			ckLength = 7;
			// +4=15;
			uNote = note;
			fineTune = tune;
			Gain = gain;
			noteLow = klo;
			noteHigh = khi;
			velLow = vlo;
			velHigh = vhi;
		}
	}
}






