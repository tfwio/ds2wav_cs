/* tfwxo * 1/18/2016 * 9:57 PM */
using System;
using System.IO;
using on.iff;
using on.dsformat;
namespace System.IO
{
	static class WriterExt
	{
		static public byte[] flip(this byte[] input)
		{
			Array.Reverse(input);
			return input;
		}

    static public void WriteE(this BinaryWriter w, int input)
    {
      var bs = BitConverter.GetBytes(input);
      w.Write(BitConverter.IsLittleEndian ? bs : bs.flip(), 0, 4);
    }
    static public void WriteE(this BinaryWriter w, uint input)
    {
      var bs = BitConverter.GetBytes(input);
      w.Write(BitConverter.IsLittleEndian ? bs : bs.flip(), 0, 4);
    }
    static public void WriteE(this BinaryWriter w, ushort input)
    {
      var bs = BitConverter.GetBytes(input);
      w.Write(BitConverter.IsLittleEndian ? bs : bs.flip(), 0, 2);
    }

		static public void WriteE(this BinaryWriter w, short input)
		{
			var bs = BitConverter.GetBytes(input);
			w.Write(BitConverter.IsLittleEndian ? bs : bs.flip(), 0, 2);
		}
		static public void WriteE(this BinaryWriter w, byte input)
		{
			w.Write(input);
		}
		static public void WriteE(this BinaryWriter w, sbyte input)
		{
			w.Write(input);
		}
    static public int Flip32(this BinaryReader r, int len) { return BitConverter.ToInt32(r.ReadBytes(len).flip(), 0); }
    static public System.Double ReadDoublee(this BinaryReader b) { return BitConverter.IsLittleEndian ? b.ReadDouble() : BitConverter.ToDouble(b.ReadBytes(8).flip(), 0); }
    static public System.Single ReadSinglee(this BinaryReader b) { return BitConverter.IsLittleEndian ? b.ReadSingle() : BitConverter.ToSingle(b.ReadBytes(4).flip(), 0); }
    static public System.Int16 ReadInt16e(this BinaryReader b) { return BitConverter.IsLittleEndian ? b.ReadInt16() : BitConverter.ToInt16(b.ReadBytes(2).flip(), 0); }
    static public System.Int32 ReadInt32e(this BinaryReader b) { return BitConverter.IsLittleEndian ? b.ReadInt32() : BitConverter.ToInt32(b.ReadBytes(4).flip(), 0); }
    static public System.Int64 ReadInt64e(this BinaryReader b) { return BitConverter.IsLittleEndian ? b.ReadInt64() : BitConverter.ToInt64(b.ReadBytes(8).flip(), 0); }
    static public System.UInt16 ReadUInt16e(this BinaryReader b) { return BitConverter.IsLittleEndian ? b.ReadUInt16() : BitConverter.ToUInt16(b.ReadBytes(2).flip(), 0); }
    static public System.UInt32 ReadUInt32e(this BinaryReader b) { return BitConverter.IsLittleEndian ? b.ReadUInt32() : BitConverter.ToUInt32(b.ReadBytes(4).flip(), 0); }
    static public System.UInt64 ReadUInt64e(this BinaryReader b) { return BitConverter.IsLittleEndian ? b.ReadUInt64() : BitConverter.ToUInt64(b.ReadBytes(8).flip(), 0); }
	}
}




