/* tfwxo * 1/18/2016 * 9:56 PM */
using System;
using System.IO;
namespace on.iff
{
  /// <summary>
  /// This class is used generally to write a simple
  /// Microsoft RIFF Wave file.
  /// </summary>
  class DsWaveFile
  {
    IFFCHUNK riff { get; set; } = new IFFCHUNK {
      Name = RiffType.RIFF,
      Tag  = RiffType.WAVE
    };
    WAVE_FMT fmt_ { get; set; } = new WAVE_FMT();
    LISTINFO list { get; set; } = new LISTINFO();
    
    const  uint   data = RiffType.data;
    uint          dataLength;
    
    public DsWaveFile(string comment, long nSamples, int Fs=44100, ushort nch=1, ushort bps=16)
    {
      fmt_.Config(Fs,nch,bps);
      list.Comment.Value = comment;
      dataLength = Convert.ToUInt32(nSamples*2);
      uint listLen = list.GetLength();
      string strLength = string.Format("{0:X4}",nSamples*2);
      // calculate total size to assign to (IFFCHUNK) riff section
      // ignore first
      riff.Length = 4 + // 'WAVE' (4) +
        (4+4+16) + // 'fmt ' (4) + 0x10000000 (4) + format (16)
        (8+dataLength) + //
        (8+listLen);
    }
    
    public void WriteHeader(BinaryWriter writer)
    {
      // we need to assign the total length of the RIFF-FILE to the riff header (IFFCHUNK)
      // see the constructor method...
      riff.Write(writer);
      fmt_.Write(writer);
      writer.Write(data);
      writer.Write(dataLength);
    }
    public void WriteTerminal(BinaryWriter writer)
    {
      // we need to assign the total length of the RIFF-FILE to the riff header (IFFCHUNK)
      // see the constructor method...
      list.Write(writer);
    }
  }
  
  class WAVE_FMT
  {
    readonly internal SUBCHUNK CKID = new SUBCHUNK{ Name=RiffType.fmt, Length=16 };
    /// <summary>
    /// 4 ('fmt ') + 16 (inner-data) = 20
    /// </summary>
    internal const int ChunkLength = 20;
    // 
    public ushort wFormatTag  = (ushort)WaveFormatEncoding.Pcm;
    public ushort nChannels;
    public uint   nSamplesPerSec;
    public uint   nAvgBytesPerSec;
    public ushort nBlockAlign;
    public ushort wBitsPerSample;
    
    internal void Config(int Fs=44100, ushort nch=1, ushort bps=16)
    {
      nChannels       = nch;
      nSamplesPerSec  = (uint)Fs;
      nBlockAlign     = 2; // Convert.ToUInt16(bps / 8); // sizeof(short);
			nAvgBytesPerSec = Convert.ToUInt32(nBlockAlign * Fs);
      wBitsPerSample  = bps;
    }
    /// <summary>
    /// Write all but data which is to be written directly after calling this.
    /// </summary>
    /// <param name="writer"></param>
    public void Write(BinaryWriter writer)
    {
      // begin format block
      CKID.Write(writer);            // 08
      // ------------------------------ bytes written
      writer.Write(wFormatTag);      // 02 : 0x0001 (PCM) --- two bytes written
      writer.Write(nChannels);       // 04 : 1
      writer.Write(nSamplesPerSec);  // 08 : 44100 or whatever
      writer.Write(nAvgBytesPerSec); // 12 : 
      writer.Write(nBlockAlign);     // 14 : 2 // see wBitsPerSample, as we're aligning to two bytes
      writer.Write(wBitsPerSample);  // 16 : 0x0000 is two 8bit bytes = 16bit or 16
    }
  }
  /// should probably be titled something like DsListInfo
  class LISTINFO
  {
    IFFCHUNK ListInfo = new IFFCHUNK{ Name = ListType.LIST, Tag  = ListType.INFO };

    public uint  Length { get { return ListInfo.Length; } set { ListInfo.Length = value; } }
    public WZSTR Software = new WZSTR { Tag=ListType.Software, Value="DrumSynth v2.0 \0" };
    public WZSTR Comment = new WZSTR  { Tag=ListType.Comment, Value=" \0" };
    
    internal uint GetLength() {
      uint result = 4;
      /*if (!string.IsNullOrEmpty(Comment.Value))*/  result += (uint)8 + (uint)Comment.Value;
      /*if (!string.IsNullOrEmpty(Software.Value))*/ result += (uint)8 + (uint)Software.Value;
      return result;
    }
    
    public void Write(BinaryWriter writer)
    {
      Length = GetLength(); // IFFCHUNK length before writing
      ListInfo.Write(writer); // 12 - bytes written
      Software.Write(writer); // + 04 (bytes) + software (string) 
      Comment .Write(writer); // + 04 (bytes) + comment  (string)
    }
  }
  
}
