/* tfwxo * 1/18/2016 * 9:56 PM */
using System;
using System.IO;
namespace on.iff
{
  class IFFCHUNK
  {
    public uint Name { get; set; }
    public uint Length;
    public uint Tag { get; set; }
    
    public void Read(BinaryReader writer)
    {
      this.Name   = writer.ReadUInt32();
      this.Length = writer.ReadUInt32();
      this.Tag    = writer.ReadUInt32();
    }
    public void Write(BinaryWriter writer)
    {
      writer.Write(Name);
      writer.Write(Length);
      writer.Write(Tag);
    }
  }
  class SUBCHUNK
  {
    public uint Name { get; set; }
    public uint Length;
    
    public void Read(BinaryReader writer)
    {
      this.Name   = writer.ReadUInt32();
      this.Length = writer.ReadUInt32();
    }
    public void Write(BinaryWriter writer)
    {
      writer.Write(Name);
      writer.Write(Length);
    }
  }
}






