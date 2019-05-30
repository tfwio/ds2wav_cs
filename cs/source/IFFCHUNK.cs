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
      this.Name   = writer.ReadUInt32e();
      this.Length = writer.ReadUInt32e();
      this.Tag    = writer.ReadUInt32e();
    }
    public void Write(BinaryWriter writer)
    {
      writer.WriteE(Name);
      writer.WriteE(Length);
      writer.WriteE(Tag);
    }
  }
  class SUBCHUNK
  {
    public uint Name { get; set; }
    public uint Length;
    
    public void Read(BinaryReader writer)
    {
      this.Name   = writer.ReadUInt32e();
      this.Length = writer.ReadUInt32e();
    }
    public void Write(BinaryWriter writer)
    {
      writer.WriteE(Name);
      writer.WriteE(Length);
    }
  }
}






