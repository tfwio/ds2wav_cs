/*
 * User: xo
 * Date: 10/18/2016
 * Time: 7:15 PM
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
      Console.WriteLine("DrumSynth (sharp) 20161111");
      Console.WriteLine("---------");
      Console.WriteLine();
    }
    static void Footer()
    {
      //nsole.WriteLine("DrumSynth (sharp) {rel-date}; {rel-commit}");
      Console.WriteLine();
      Console.WriteLine("m.d.a. DrumSynth is Copyright 2007 Paul Kellet");
      Console.WriteLine();
    }
    static void Usage()
    {
      Console.WriteLine("Usage:");
      Console.WriteLine();
      Console.WriteLine("  ds2wav [path]");
      Console.WriteLine();
      Console.WriteLine("    path = *.ds program, or a directory containing several *.ds files.");
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

    // rewritten since generation from a directory path didn't work when I copy/pasted the
    // old code commented in here.
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
      /*
      string inputfile = null;
      string outputfile = null;
      string ext = null;
      bool haserror = false;
      var d2w = new ds2wav();
      // d2w.do_ds2wav(inputfile,outputfile);
      if (args.Length==2)
      {
        Console.WriteLine("Two parameters are required"); // really?
        Console.WriteLine("[1] DS-file.ds, [2] output.wav");
        inputfile = args[0];
        outputfile = args[1];
      }
      else if (args.Length==1)
      {
        inputfile = args[0];
        
        if (System.IO.Directory.Exists(inputfile))
        {
          var ofile = string.Empty;
          var files = Directory.GetFiles(inputfile,"*.ds");
          var filec = (int)0;
          var filel = files.Length;
          var errors = new List<KeyValuePair<int,string>>();
          if (filel >0)
            foreach (var ds in files)
          {
            var fname = Path.GetFileName(ds);
            try {
              d2w.do_ds2wav(ds,DrumSynthFloat.GetOutFile(ds, 44100));
              Console.WriteLine(
                "Got {0} of {1}: {2}",
                filec+1,
                filel,
                fname
               );
            }
            catch {
              haserror = true;
              errors.Add(new KeyValuePair<int, string>(filec+1,fname));
              Console.WriteLine("Error writing: {0}",Path.GetFileNameWithoutExtension(fname));
            }
            filec++;
          }
          if (haserror)
          {
            Console.WriteLine("had issues with...");
            foreach (var r in errors)
            {
              Console.WriteLine("  [{0}] {1}", r.Key, r.Value);
            }
          }
          Console.WriteLine("------------------------------------");
          Console.WriteLine("(press a key to exit)");
          if (haserror) Console.ReadKey();
          return;
        }
        else
        {
          if (!DrumSynthFloat.IsExtension(inputfile))
          {
            Console.WriteLine("EXITING: No DS file detected.");
            Console.WriteLine("(press a key to continue)");
            Console.ReadKey();
            return;
          }
          outputfile = DrumSynthFloat.GetOutFile(inputfile, 44100);
          //;(can't remember why I commented all of this)
          try
          {
          d2w.do_ds2wav(inputfile,outputfile);
          }
          catch (Exception err)
          {
            haserror = true;
            throw err;
            Console.WriteLine("FAILED: {0}",err);
          }
          if (haserror) {
            Console.WriteLine("(press a key to continue)");
            Console.ReadKey();
            return;
          }
        }
        
      }
      else
      {
        Console.WriteLine("One or two parameters are required");
        Console.WriteLine("[1] DS-file.ds, (optional) [2] output.wav");
        Console.WriteLine("(press a key to continue)");
        Console.ReadKey();
        return;
      }
      
      ext = Path.GetExtension(inputfile);
      if (ext.ToLower() != ".ds")
      {
        Console.WriteLine("EXITING: No DS file detected.");
        Console.WriteLine("(press a key to continue)");
        Console.ReadKey();
        return;
      }
      ext = Path.GetExtension(outputfile);
      if (ext.ToLower() != ".wav")
      {
        Console.WriteLine("EXITING: No output-file detected.");
        Console.WriteLine("(press a key to continue)");
        Console.ReadKey();
        return;
      }
      try {
        int result = d2w.do_ds2wav(inputfile,outputfile);
        switch (result) // do we support these still?
        {
            case 0: Console.WriteLine("OK!"); break;
            case 1: Console.WriteLine("Version FAIL!"); Console.ReadKey(); break;
            case 2: Console.WriteLine("Input FAIL!"); Console.ReadKey(); break;
        }
      }
      catch (Exception error) {
        Console.WriteLine("FAILED: Some error occured");
        Console.WriteLine("{0}",error);
        Console.WriteLine("(press a key to continue)");
        Console.ReadKey();
      }
      */
    }
  }
}