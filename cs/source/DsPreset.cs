//
// DrumSynthFile.cs
//
// Author:
//       tfwxo <tfwroble@gmail.com>
//
// Copyright (c) 2016 tfwxo
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

/* tfwxo * 1/19/2016 * 2:10 AM */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using on.drumsynth2;

namespace on.dsformat
{
  public interface IDsPreset
  {
    /// <summary>
    /// Active (generator) envelope.
    /// Primarily useful to a UI such as Control.SelectedIndex.
    /// </summary>
    int SelectedEnvelopeIndex { get; set; }

    /// this value is stored to our pattern and used internally by
    /// DsGenerator audio mixing.
    float Pan { get; set; }

    GeneralCfg    General    { get; }
    ToneCfg       Tone       { get; }
    OvertonesCfg  Overtones  { get; }
    NoiseCfg      Noise      { get; }
    NoiseBandCfg  NoiseBand  { get; }
    NoiseBand2Cfg NoiseBand2 { get; }
    DistortionCfg Distortion { get; }

    PointF[] ParseEnvelope2(int i);

    string FilePath { get; set; }

		/// <summary>
		/// returns version no.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		int Load(string fileName);
    
    void Save(string filePath);
    
    /// <summary>
    /// see if a checkbox should be checked
    /// for the particular generator id.
    /// this method isn't used (or shouldn't be)
    /// </summary>
    bool IsOn(int secID);
    
    /// <summary>
    /// see if an envelope is on
    /// </summary>
    bool IsOnE(int secID);
  }
  public class DsPreset:IDsPreset {

    public int SelectedEnvelopeIndex {
      get { return envelopeIndex; }
      set { envelopeIndex = value.Contain (0, 6); }
    } int envelopeIndex = 0;

    public float Pan { get; set; } = 0; // -100 to 100
    
    readonly IniReader INI = new IniReader();
    
    public string FilePath { get; set; }
    
    internal const string SecTone       = "Tone";       // 1
    internal const string SecNoise      = "Noise";      // 2
    internal const string SecOvertones  = "Overtones";  // 3, 4
    internal const string SecNoiseBand  = "NoiseBand";  // 5
    internal const string SecNoiseBand2 = "NoiseBand2"; // 
    internal const string SecDistortion = "Distortion"; // 
    internal const string SecGeneral    = "General";    // 

    public GeneralCfg    General    { get; set; } = new GeneralCfg();
    public ToneCfg       Tone       { get; set; } = new ToneCfg();
    public OvertonesCfg  Overtones  { get; set; } = new OvertonesCfg();
    public NoiseCfg      Noise      { get; set; } = new NoiseCfg();
    public NoiseBandCfg  NoiseBand  { get; set; } = new NoiseBandCfg();
    public NoiseBand2Cfg NoiseBand2 { get; set; } = new NoiseBand2Cfg();
    public DistortionCfg Distortion { get; set; } = new DistortionCfg();

    public PointF[] ParseEnvelope2(int i)
    {
      if (i > 6) return null;
      string keyValue = INI.GetChars(dsStr.SecKeys[i]);
      if (keyValue== null)
      {
        Debug.Print("Error loading {0} {1}", dsStr.SecKeys[i].A, dsStr.SecKeys[i].B);
        if (i==6) return new PointF[] { new PointF(0, 100), new PointF(100, 0) };
        return new PointF[] { new PointF(0, 100), new PointF(442000, 100), new PointF(442000, 0) };
      }
      var points = new List<PointF>();
      var keyValues = keyValue.Split(' ');
      for (int n = 0; n < keyValues.Length && n < DrumSynthFloat.ENV_MAX_COUNT; n++)
      {
        var vn = keyValues[n].Split(',');
        var p = new FloatPoint(float.Parse(vn[0]), float.Parse(vn[1]));
        points.Add(p);
      }

      return points.ToArray();
    }
    
    public int Load(string inputFile)
    {
      FilePath = inputFile;
      DsFileVars();
      return DsCheckVersion();
    }
    
    public bool IsOnE(int secID)
    {
      switch(secID)
      {
      case 0: return Tone.BOn;
      case 1: return Noise.BOn;
      case 2: 
      case 3: return Overtones.BOn;
      case 4: return NoiseBand.BOn;
      case 5: return NoiseBand2.BOn;
      case 6: return General.BFilter;
      case 7: 
      default: return false;
      }
    }
    
    public bool IsOn(int secID)
    {
      switch(secID)
      {
      case 0: return Tone.BOn;
      case 1: return Noise.BOn;
      case 2:
      case 3: return Overtones.BOn;
      case 4: return NoiseBand.BOn;
      case 5: return NoiseBand2.BOn;
      case 6: return Distortion.BOn;
      case 7: return !(General.Level == 0 && General.Tuning.Equals(0) && (General.Stretch.Equals(0) || General.Stretch.Equals(100.0f)));
      default: return false;
      }
    }

    //FloatPoint[] ParseEnvelope(string keyValue)
    //{
    //  var points = new FloatPoint[32];
    //  var keyValues = keyValue.Split(' ');
    //  for (int i = 0; i < keyValues.Length && i < 32; i++)
    //  {
    //    var vn = keyValues[i].Split(',');
    //    points[i] = new FloatPoint(float.Parse(vn[0]), float.Parse(vn[0]));
    //  }
    //  return points;
    //  // envpts[env, 0, keyValues.Length] = -1;
    //  // envData[env, MAX] = envpts[env, 0, keyValues.Length - 1];
    //}
    int DsCheckVersion()
    {
      //try to read version
      var cmp000 = string.Compare(General.Version.Substring(0,9), "DrumSynth", StringComparison.InvariantCulture);
      var cmp001 = General.Version[11];
      var cmp002 = cmp001 != '1' && cmp001 != '2';
      if(cmp000 != 0) { return 2;} //input fail
      if(cmp002) { return 1;} //version fail
      return 0;
    }
    
    void DsFileVars()
    {
      INI.Load(FilePath);
      General.Version     = INI.GetChars(SecGeneral,"Version"); // must be present
      General.Comment     = INI.GetChars(SecGeneral,"Comment"," \0");
      General.Stretch     = INI.GetFloat(SecGeneral, "Stretch",100);
      General.Level       = INI.GetInt32(SecGeneral,"Level",  0);
      General.Tuning      = INI.GetFloat(SecGeneral,"Tuning",0.0f);
      General.Filter      = INI.GetInt32(SecGeneral,"Filter",0);
      General.Resonance   = INI.GetInt32(SecGeneral,"Resonance",0);
      General.HighPass    = INI.GetInt32(SecGeneral,"HighPass",0);
      General.FilterEnv   = INI.GetChars(SecGeneral,"FilterEnv","0,100 442000,100 442000,0");

      Tone.On             = INI.GetInt32(SecTone,"On",0);
      Tone.Level          = INI.GetInt32(SecTone,"Level",128);
      Tone.F1             = INI.GetFloat(SecTone,"F1",200.0f);
      Tone.F2             = INI.GetFloat(SecTone,"F2",120.0f);
      Tone.Droop          = INI.GetInt32(SecTone,"Droop",0);
      Tone.Phase          = INI.GetFloat(SecTone,"Phase",90.0f); //degrees>radians
      Tone.Envelope       = INI.GetChars(SecTone,"Envelope","0,0 100,0");

      Noise.On            = INI.GetInt32(SecNoise,"On",0);
      Noise.Level         = INI.GetInt32(SecNoise,"Level",0);
      Noise.Slope         = INI.GetInt32(SecNoise,"Slope",0);
      Noise.FixedSeq      = INI.GetInt32(SecNoise,"FixedSeq",0);
      Noise.Envelope      = INI.GetChars(SecNoise,"Envelope","0,0 100,0");

      Overtones.On        = INI.GetInt32(SecOvertones,"On",0);
      Overtones.Level     = INI.GetInt32(SecOvertones,"Level",128);
      Overtones.Method    = INI.GetInt32(SecOvertones,"Method",2);
      Overtones.F1        = INI.GetFloat(SecOvertones,"F1",200.0f);
      Overtones.F2        = INI.GetFloat(SecOvertones,"F2",200.0f);
      Overtones.Wave1     = INI.GetInt32(SecOvertones,"Wave1",0);
      Overtones.Wave2     = INI.GetInt32(SecOvertones,"Wave2",0);
      Overtones.Param     = INI.GetInt32(SecOvertones,"Param",50);
      Overtones.Filter    = INI.GetInt32(SecOvertones,"Filter",0);
      Overtones.Track1    = INI.GetInt32(SecOvertones,"Track1",0);
      Overtones.Track2    = INI.GetInt32(SecOvertones,"Track2",0);
      Overtones.Envelope1 = INI.GetChars(SecOvertones,"Envelope1","0,0 100,0");
      Overtones.Envelope2 = INI.GetChars(SecOvertones,"Envelope2","0,0 100,0");

      NoiseBand.On        = INI.GetInt32(SecNoiseBand,"On",0);
      NoiseBand.Level     = INI.GetInt32(SecNoiseBand,"Level",128);
      NoiseBand.F         = INI.GetFloat(SecNoiseBand,"F",1000.0f);
      NoiseBand.dF        = INI.GetInt32(SecNoiseBand,"dF",50);
      NoiseBand.Envelope  = INI.GetChars(SecNoiseBand,"Envelope","0,0 100,0");

      NoiseBand2.On       = INI.GetInt32(SecNoiseBand2,"On",0);
      NoiseBand2.Level    = INI.GetInt32(SecNoiseBand2,"Level",128);
      NoiseBand2.F        = INI.GetFloat(SecNoiseBand2,"F",1000.0f);
      NoiseBand2.dF       = INI.GetInt32(SecNoiseBand2,"dF",50);
      NoiseBand2.Envelope = INI.GetChars(SecNoiseBand2,"Envelope","0,0 100,0");

      Distortion.On       = INI.GetInt32(SecDistortion,"On",0);
      Distortion.Rate     = INI.GetInt32(SecDistortion,"Rate",0);
      Distortion.Bits     = INI.GetInt32(SecDistortion,"Bits",0);
      Distortion.Clipping = INI.GetInt32(SecDistortion,"Clipping",0);
    }
    string KeyStr(string Name, object Value)
    {
      return string.Format("{0}={1}",Name,Value);
    }
    
    public void Save(string filePath)
    {
      using (var writer = new IniWriter(filePath))
      {
        writer.Write("[General]");
        writer.Write("Version","DrumSynth v2.0#");
        if (!string.IsNullOrEmpty(General.Comment)) writer.Write("Comment",General.Comment);
        writer.Write("Tuning",General.Tuning);
        writer.Write("Stretch",General.Stretch);
        writer.Write("Level",General.Level);
        writer.Write("Filter",General.Filter);
        writer.Write("HighPass",General.HighPass);
        writer.Write("Resonance",General.Resonance);
        writer.Write("FilterEnv",General.FilterEnv);
        writer.Write();
        
        if (true)//IsOn(0)
        {
          writer.Write("[Tone]");
          if (Tone.BOn) writer.Write("On",Tone.On);
          writer.Write("Level",Tone.Level);
          writer.Write("F1",Tone.F1);
          writer.Write("F2",Tone.F2);
          writer.Write("Droop",Tone.Droop);
          writer.Write("Phase",Tone.Phase);
          writer.Write("Envelope",Tone.Envelope);
          writer.Write();
        }
        
        writer.Write("[Noise]");
        //if (Noise.BOn) writer.WriteLine("On",Noise.On));
        writer.Write("On",Noise.On);
        writer.Write("Level",Noise.Level);
        writer.Write("Slope",Noise.Slope);
        writer.Write("Envelope",Noise.Envelope);
        writer.Write("FixedSeq",Noise.FixedSeq);
        writer.Write();
        
        writer.Write("[Overtones]");
        writer.Write("On",Overtones.On);
        writer.Write("Level",Overtones.Level);
        writer.Write("F1",Overtones.F1);
        writer.Write("F2",Overtones.F2);
        writer.Write("Method",Overtones.Method);
        writer.Write("Param",Overtones.Param);
        writer.Write("Envelope1",Overtones.Envelope1);
        writer.Write("Envelope2",Overtones.Envelope2);
        writer.Write("Wave1",Overtones.Wave1);
        writer.Write("Track1",Overtones.Track1);
        writer.Write("Wave2",Overtones.Wave2);
        writer.Write("Track2",Overtones.Track2);
        writer.Write("Filter",Overtones.Filter);
        writer.Write();
        
        writer.Write("[NoiseBand]");
        writer.Write("On",NoiseBand.On);
        writer.Write("Level",NoiseBand.Level);
        writer.Write("F",NoiseBand.F);
        writer.Write("dF",NoiseBand.dF);
        writer.Write("Envelope",NoiseBand.Envelope);
        writer.Write();
        
        writer.Write("[NoiseBand2]");
        writer.Write("On",NoiseBand2.On);
        writer.Write("Level",NoiseBand2.Level);
        writer.Write("F",NoiseBand2.F);
        writer.Write("dF",NoiseBand2.dF);
        writer.Write("Envelope",NoiseBand2.Envelope);
        writer.Write();
        
        writer.Write("[Distortion]");
        writer.Write("On",Distortion.On);
        writer.Write("Clipping",Distortion.Clipping);
        writer.Write("Bits",Distortion.Bits);
        writer.Write("Rate",Distortion.Rate);
        writer.Write();
      }
    }
  }

  public class GeneralCfg
  {
    public string    Version   { get; set; } = "DrumSynth v2.0"; // DrumSynth v2.0
    public string    Comment   { get; set; } = null; // 
    public float     Tuning    { get; set; } = 0.0f;
    public float     Stretch   { get; set; } = 0.0f;
    public int       Level     { get; set; } = 0; // -20 to 20,1,3
    [System.Xml.Serialization.XmlIgnore]
    public bool      BFilter   { get { return Filter == 1; } set { Filter = value ? 1 : 0; } }
    public int       Filter    { get; set; } = 0; // bool (0 to 1)
    [System.Xml.Serialization.XmlIgnore]
    public bool      BHighPass { get { return HighPass == 1; } set { HighPass = value ? 1 : 0; } }
    public int       HighPass  { get; set; } = 0; // bool (0 to 1)
    public int       Resonance { get; set; } = 0; // int  (0 to 99:1,10)
    public string    FilterEnv { get; set; } = "0,0 100,0";
  }
  public class ToneCfg
  {
    [System.Xml.Serialization.XmlIgnore]
    public bool      BOn       { get { return On == 1; } set { On = value ? 1 : 0; } }
    public int       On        { get; set; } = 1; // bool (0 to 1)
    public int       Level     { get; set; } = 128; // (1 to 181:1,9:128)
    public float     F1        { get; set; } = 200;
    public float     F2        { get; set; } = 120;
    public int       Droop     { get; set; } = 0; // int (0 to 100:1,10)
    public float     Phase     { get; set; } = 0.0f;
    public string    Envelope  { get; set; } = "0,0 100,0";
  }
  public class NoiseCfg
  {
    [System.Xml.Serialization.XmlIgnore]
    public bool      BOn       { get { return On == 1; } set { On = value ? 1 : 0; } }
    public int       On        { get; set; } = 1; // b
    public int       Level     { get; set; } = 128; // (1 to 181:1,9)
    public int       Slope     { get; set; } = 0; // (-100 to 100:0,10)
    public string    Envelope  { get; set; } = "0,0 100,0";
    public bool      BFixedSeq { get { return FixedSeq == 1; } set { FixedSeq = value ? 1 : 0; } }
    public int       FixedSeq  { get; set; } = 0; // b
  }
  public class OvertonesCfg
  {
    public int       On        { get; set; } = 0; // b
    [System.Xml.Serialization.XmlIgnore]
    public bool      BOn       { get { return On == 1; } set { On = value ? 1 : 0; } }
    public int       Level     { get; set; } = 128; // (1 to 181:1,9)
    public float     F1        { get; set; } = 316.0f; // 
    public int       Wave1     { get; set; } = 0; // (0 to 4: sin, sin^2, tri, saw, square)
    public int       Track1    { get; set; } = 0; // b
    public bool      BTrack1   { get { return Track1 == 1; } set { Track1 = value ? 1 : 0; } }
    public float     F2        { get; set; } = 630.0f; // 
    public int       Wave2     { get; set; } = 0; // (0 to 4: sin, sin^2, tri, saw, square)
    public int       Track2    { get; set; } = 0; // b
    [System.Xml.Serialization.XmlIgnore]
    public bool      BTrack2   { get { return Track2 == 1; } set { Track2 = value ? 1 : 0; } }
    public int       Method    { get; set; } = 0; // (0 to 3: A + B, A .B freq mod, A x B ring mod, Cymbal Tones)
    public int       Param     { get; set; } = 0; // there are ranges for this value...
    public string    Envelope1 { get; set; } = "0,0 100,0";
    public string    Envelope2 { get; set; } = "0,0 100,0";
    [System.Xml.Serialization.XmlIgnore]
    public bool      BFilter   { get { return Filter == 1; } set { Filter = value ? 1 : 0; } }
    public int       Filter    { get; set; } = 0; // b
  }
  public class NoiseBandCfg
  {
    [System.Xml.Serialization.XmlIgnore]
    public bool      BOn       { get { return On == 1; } set { On = value ? 1 : 0; } }
    public int       On        { get; set; } = 0; // b
    public int       Level     { get; set; } = 128; // (1 to 181:1,9)
    public float     F         { get; set; } = 1200.0f; // 
    public int       dF        { get; set; } = 40; // depth? (0 to 100)
    public string    Envelope  { get; set; } = "0,0 100,0";
  }
  public class NoiseBand2Cfg
  {
    [System.Xml.Serialization.XmlIgnore]
    public bool      BOn       { get { return On == 1; } set { On = value ? 1 : 0; } }
    public int       On        { get; set; } = 0; // b
    public int       Level     { get; set; } = 128; // (1 to 181:1,9)
    public float     F         { get; set; } = 3100.0f; // 
    public int       dF        { get; set; } = 40; // depth? (0 to 100)
    public string    Envelope  { get; set; } = "0,0 100,0";
  }
  public class DistortionCfg
  {
    [System.Xml.Serialization.XmlIgnore]
    public bool      BOn      { get { return On == 1; } set { On = value ? 1 : 0; } }
    public int       On       { get; set; } = 0; // b
    public int       Clipping { get; set; } = 0; // (0 to 60: 1, 3)
    public int       Bits     { get; set; } = 0; // (0 to 7)
    public int       Rate     { get; set; } = 0; // (0 to 7)
  }
  static class dsStr
  {
    readonly internal static int[] rates=
    {
      8000,
      8096,
      11025,
      16000,
      22050,
      32000,
      44100,
      48000,
      88200,
      96000,
      176400,
      192000,
      352800,
      384000,
    };
    readonly internal static string[] bd =  {
      "[none]",
      "14 bit",
      "12 bit",
      "10 bit",
      " 8 bit",
      " 6 bit",
      " 4 bit",
      " 2 bit"
    };

    readonly internal static string[] df =  {
      "[none]",
      "11 kHz",
      " 7 kHz",
      " 5 kHz",
      " 4 kHz",
      " 3 kHz",
      " 2 kHz",
      " 1 kHz"
    };

    readonly internal static string[] wav =  {
      "sin",
      "sin²",
      "tri",
      "saw",
      "square"
    };

    readonly internal static string[] opm =  {
      "A + B (additive)",
      "A . B (freq modulation)",
      "A x B (ring modulation)",
      "A...B (cymbal tones)",
    };

    readonly internal static string[] opp =  {
      "Mix",
      "Depth",
      "Depth",
      "Reason.",
      "Mix",
    };

    readonly internal static string[] key =  {
      "Tone",
      "Noise",
      "OtA",
      "OtB",
      "Noise Band 1",
      "Noise Band 2",
      "Filter Cutoff"
    };

    readonly internal static string[,] otl =  {
      {
        "Overtone A",
        "Overtone B"
      },
      {
        "Overtone level",
        "Overtone modulation depth"
      },
      {
        "Overtone level",
        "Overtone modulation depth"
      },
      {
        "Overtone A",
        "Overtone frequency range"
      },
      {
        "Overtone A",
        "Overtone B"
      },
    };
    internal static readonly IniReader.Pair[] SecKeys = {
      new IniReader.Pair(DsPreset.SecTone,"Envelope"),
      new IniReader.Pair(DsPreset.SecNoise,"Envelope"),
      new IniReader.Pair(DsPreset.SecOvertones,"Envelope1"),
      new IniReader.Pair(DsPreset.SecOvertones,"Envelope2"),
      new IniReader.Pair(DsPreset.SecNoiseBand,"Envelope"),
      new IniReader.Pair(DsPreset.SecNoiseBand2,"Envelope"),
      new IniReader.Pair(DsPreset.SecGeneral,"FilterEnv"),
    };
  }
}
