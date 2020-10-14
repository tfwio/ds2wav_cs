/*
 * User: xo
 * Date: 10/18/2016
 * Time: 7:15 PM
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace on.drumsynth2
{
  /// <summary>
  /// maybe a bit (rough) drafty, but it GITs the job done.
  /// </summary>
  class Program
  {
    static void Caption()
    {
      //nsole.WriteLine("DrumSynth (sharp) {rel-date}; {rel-commit}");
      Console.WriteLine("DrumSynth cs v0.1.1");
      Console.WriteLine("------------");
      Console.WriteLine();
    }
    static void Footer()
    {
      //nsole.WriteLine("DrumSynth (sharp) {rel-date}; {rel-commit}");
      Console.WriteLine();
      Console.WriteLine("M.D.A. DrumSynth - Copyright 2007 Paul Kellet");
      Console.WriteLine();
    }
    static void Usage()
    {
      Console.WriteLine("Use:");
      Console.WriteLine();
      Console.WriteLine("  ds2wav [ <directory> | <*.ds> ]");
      Console.WriteLine();
    }

    static bool GenDs(IList<FileInfo> list, int rate=44100)
    {
      bool hasError = false;
      int i = 0;
      foreach (var file in list)
      {
        hasError &= DrumSynthFloat.DsGenWaveform(file.FullName, rate);
        if (!hasError)
          Console.WriteLine("- {0} of {1} - {2}", ++i, list.Count, Path.GetFileNameWithoutExtension(file.Name));
        else
        {
          var dfg = Console.ForegroundColor; // reuse
          Console.Write("- {0} of {1} - {2}", ++i, list.Count, Path.GetFileNameWithoutExtension(file.Name));
          Console.ForegroundColor = ConsoleColor.Red;
          Console.Write(" FAIL\n");
          Console.ForegroundColor = dfg;
        }
      }
      return hasError;
    }

    static IList<FileInfo> EnumDsFiles(string path)
    {
      if (path == null) return null;
      if (Directory.Exists(path)) {
        var d = new DirectoryInfo(path);
        var result = new List<FileInfo>(d.GetFiles("*.ds"));
        result.Sort((a,b)=> string.Compare(a.FullName, b.FullName, StringComparison.Ordinal));
        return result;
      }
      if (File.Exists(path) && DrumSynthFloat.IsExtension(path))
      {
        return new List<FileInfo>(){ new FileInfo(path) };
      }
      return null;
    }

    public static void Main(string[] args)
    {

      Caption();

      if (args.Length>2 || args.Length==0)
      {
        Usage();
        return;
      }
      if (args.Length==2 && File.Exists(args[0]))
      {
        if (DrumSynthFloat.IsExtension(args[0]) && DrumSynthFloat.IsExtension(args[1],".wav"))
        {
          var ifile = new FileInfo(args[0]);
          var ofile = Path.GetFileName(args[1]) == args[1] ?
                          Path.Combine(
                            ifile.Directory.FullName,
                            Path.GetFileNameWithoutExtension(args[1])
                           ):
                          args[1];
          var ds2file = new DrumSynthFloat();
          ds2file.do_ds2wav(ifile.FullName, ofile);
          Console.WriteLine("Generating");
          return;
        }
      }

      GenDs(EnumDsFiles(args[0]));

      Footer();

      return;
    }
  }
}