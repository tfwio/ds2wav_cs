/* tfwxo * 1/19/2016 * 2:10 AM */
using System;
using System.Runtime.InteropServices;
using System.Text;
namespace on.drumsynth2
{

  static class InteropHelper
  {
    //[DllImport("kernel32", CharSet = CharSet.Unicode)]
    //public static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

    //static public float GetPrivateProfileFloat(string sec, string key, float v, string ini)
    //{
    //  var tmp = new StringBuilder(16);
    //  float f = v;
    //  GetPrivateProfileString(sec, key, "", tmp, 16, ini);
    //  string vtmp = tmp.ToString().Trim(' ');
    //  if (string.IsNullOrEmpty(vtmp)) return f;
    //  float.TryParse(vtmp, out f);
    //  return f;
    //}

    //[DllImport("kernel32")]
    //public static extern int GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);
    //int GetPrivateProfileInt(string sec, string key, int v, string ini)
    //{
    //  var tmp = new StringBuilder(16);
    //  int f = v;
    //
    //  GetPrivateProfileString(sec, key, "", tmp, 16, ini);
    //  string vtmp = tmp.ToString().Trim();
    //  tmp = null;
    //  if (string.IsNullOrEmpty(vtmp)) return f;
    //
    //  int.TryParse(vtmp, out f);
    //  return f;
    //}
    //static public byte[] GetBytes(object myStruct)
    //{
    //  int size = Marshal.SizeOf(myStruct);
    //  var arr = new byte[size];
    //
    //  IntPtr ptr = Marshal.AllocHGlobal(size);
    //  Marshal.StructureToPtr(myStruct, ptr, true);
    //  Marshal.Copy(ptr, arr, 0, size);
    //  Marshal.FreeHGlobal(ptr);
    //  return arr;
    //}
  }
  
  
  /// <summary>
  /// sndPlaySound API
  /// </summary>
  static class MM_Sys
  {
    /// <summary>
    /// sndPlaySound API parameter;
    /// </summary>
    [Flags]
    public enum sndFlags : int
    {
      SYNC        = 0x00000000,
      ASYNC       = 0x00000001,
      NODEFAULT   = 0x00000002,
      MEMORY      = 0x00000004,
      LOOP        = 0x00000008,
      NOSTOP      = 0x00000010,
      NOWAIT      = 0x00000020,
      PURGE       = 0x00000040, // WINVER >= 0x0400
      APPLICATION = 0x00000080, // WINVER >= 0x0400
      ALIAS       = 0x00010000,
      FILENAME    = 0x00020000,
      RESOURCE    = 0x00040004,
      ALIAS_ID    = 0x00110000
    }
    
    /// <summary>
    /// winmm api
    /// </summary>
    /// <param name="snd">File path pointing to the wav file (ACM compatible)</param>
    /// <param name="sflg">sndFlags</param>
    /// <returns></returns>
    [DllImport( "winmm", CharSet = CharSet.Ansi )]
    public static extern bool sndPlaySound ([MarshalAs(UnmanagedType.LPStr)] string snd, sndFlags sflg);
    
    [DllImport( "winmm", CharSet = CharSet.Ansi )]
    public static extern bool sndPlaySound (int snd, sndFlags sflg);
    /// <summary>
    /// Note that when playing a file in memory, we are limited to about 100K of data
    /// </summary>
    /// <param name="snd"></param>
    /// <param name="sflg"></param>
    /// <returns></returns>
    [DllImport( "winmm", CharSet = CharSet.Ansi )]
    public static extern bool sndPlaySound (IntPtr snd, sndFlags sflg);

  }
}




