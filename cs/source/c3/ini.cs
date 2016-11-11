// - copyright 20160119 tfwroble [gmail]
namespace System.IO
{
	using System;
  using System.Collections.Generic;

	class IniReader : IDisposable
  {
    #region IDisposable implementation

    public void Dispose()
    {
      Dic.Clear();
    }

    #endregion
    
    public bool ContainsKey(string Key)
    {
      return Dic.ContainsKey(Key);
    }
    
    readonly Dictionary<string,List<Pair>> Dic = new Dictionary<string,List<Pair>>();

    /// <summary>
    /// Returns a formatted 'ini' Key/Value string such as
    /// "Key=Value"
    /// </summary>
    /// <returns>The string.</returns>
    /// <param name="Name">Name.</param>
    /// <param name="Value">Value.</param>
    static string KeyStr(string Name, object Value)
    {
      return string.Format("{0}={1}",Name,Value);
    }

    // 
    // Self Referencing Dictionary Lookups
    // ===========================================================

    public string this[string k1, string k2] {
      get { return Dic == null ? null : this[k1]?.Find(p => p.A == k2)?.B; }
    }
    public List<Pair> this[string k1] {
      get { return Dic.ContainsKey(k1) ? Dic[k1] : null; }
    }

    // 
    // Get Value Methods
    // ===========================================================

    // getInt
    public int GetInt32(string k1, string k2, int defaultValue = 0)
    {
      var v = this[k1,k2];
      int result = defaultValue;
      return v == null ? defaultValue : int.TryParse(v, out result) ? result : defaultValue;
    }

    // getFloat
    public float GetFloat(string k1, string k2, float nullValue = 0.0f)
    {
      var v = this[k1,k2];
      float result = nullValue;
      return (v == null) ? nullValue : float.TryParse(v, out result) ? result : nullValue;
    }

    // getString
    public string GetChars(string k1, string k2, string nullValue=null)
    {
      return this[k1,k2] ?? nullValue;
    }
    public string GetChars(Pair Key, string nullValue=null)
    {
      return this[Key.A,Key.B] ?? nullValue;
    }

    public void Load(string file)
    {
      if (!File.Exists(file)) return;
      Dic.Clear();
      var data = File.ReadAllLines(file, Text.Encoding.UTF8);
      string k1 = "", k2 = "";
      foreach (var line in data)
      {
        int i = -1;
        var start = line.Trim(' ');
        if (string.IsNullOrEmpty(start)) continue;
        if (start[0]=='[')
        {
          k1 = start.Trim('[',']');
          Dic.Add(k1,new List<Pair>());
          //Debug.Print("Key: {0} — Added",k1);
          continue;
        }
        else if ((i=start.IndexOf('=')) >= 0)
        {
          k2 = start.Substring(0,i);
          i++;
          var v = start.Substring(i,start.Length-i);
          Dic[k1].Add(new Pair(k2,v));
          //Debug.Print("Sub: {0} — {1}",k2, v);
        }
      }
      data = null;
    }

    //  public struct fenvelope
    //  {
    //    public float V;
    //    public float T;
    //  }
    public class Pair
    {
      public string A { get; set; }
      public string B { get; set; }
      public Pair() { }
      public Pair(string a, string b) { A=a; B=b; }
    }
  }

  class IniWriter : IDisposable
  {
    Stream       Stream { get; set; }
    StreamWriter Writer { get; set; }
#if __WIN
    public string NewLine { get; set; } = "\r\n";
#else
		public string NewLine { get; set; } = "\n";
#endif
		#region .ctor

		public IniWriter(string outputFile)
    {
      if (File.Exists (outputFile)) File.Delete (outputFile);
      Stream = new FileStream(outputFile, FileMode.OpenOrCreate);
      Writer = new StreamWriter(Stream);
      Writer.NewLine = NewLine;
      IsDisposed = false;
    }
    public IniWriter() : this(new MemoryStream())
    {
    }
    public IniWriter(Stream stream)
    {
      Stream = stream;
      Writer = new StreamWriter(Stream);
      IsDisposed = false;
    }
    ~IniWriter()
    {
      if (!IsDisposed)
        Dispose ();
    }

    #endregion

    /// <summary>
    /// Write a Key. If the key isn't surrounded in square-braces,
    /// it will be [wrapped] in them.
    /// </summary>
    /// <param name="Key">Key.</param>
    public void Write(string Key)
    {
      Writer.WriteLine(string.Format(Key[0]=='[' ? "{0}" : "[{0}]",Key));
    }

    /// <summary>
    /// Writes such as "Key=Value".
    /// </summary>
    /// <param name="Key">Key.</param>
    /// <param name="Value">Value.</param>
    public void Write(string Key, object Value)
    {
      Writer.WriteLine (KeyStr (Key,Value));
    }

    /// <summary>
    /// Writes a line-break using
    /// </summary>
    public void Write()
    {
      Writer.WriteLine ();
    }

    string KeyStr(string Name, object Value)
    {
      return string.Format("{0}={1}",Name,Value);
    }

    #region IDisposable implementation
    bool IsDisposed = false;
    public void Dispose ()
    {
      if (Writer!=null) {
        try {
          Writer.Close();
        } catch {
        }
        try {
          Writer.Dispose();
        } catch {
        } finally {
          Writer = null;
        }
      }
      if (Stream!=null) {
        try {
          Stream.Close();
        } catch {
        }
        try {
          Stream.Dispose();
        } catch {
        } finally {
          Writer = null;
        }
      }
      IsDisposed = true;
    }
    #endregion

  }

}

