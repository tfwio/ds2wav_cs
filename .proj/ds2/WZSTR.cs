/* tfwxo * 1/18/2016 * 9:56 PM */
using System;
using System.IO;
namespace on.iff
{
	public class WZSTR
	{
		public uint Tag {
			get;
			set;
		}

		public ZSTR Value {
			get;
			set;
		}

		// public void Read(BinaryReader writer) {}
		public void Write(BinaryWriter writer)
		{
			writer.WriteE(Tag);
			Value.Write(writer);
		}
	}
}
