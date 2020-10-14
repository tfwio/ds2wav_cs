/* tfwxo * 1/19/2016 * 2:10 AM */
using System;
using System.Collections.Generic;
namespace on.drumsynth2
{
  public class FileInfoEx
  {
    public string Name { get { return System.IO.Path.GetFileNameWithoutExtension(info.Name); } }
    public string FullName { get { return info.FullName; } }
    public System.IO.DirectoryInfo Directory { get { return info.Directory; } }
    System.IO.FileInfo info;
    public FileInfoEx(System.IO.FileInfo fileName) { info = fileName; }
    public FileInfoEx(string fileName) { info = new System.IO.FileInfo(fileName); }
    static public implicit operator FileInfoEx(System.IO.FileInfo file) { return new FileInfoEx(file); }
    static public implicit operator System.IO.FileInfo(FileInfoEx file) { return file.info; }

    static public FileInfoEx[] Translate(params System.IO.FileInfo[] input) {
      var list = new List<FileInfoEx>();
      foreach (var i in input) list.Add(i);
      return list.ToArray();
    }
  }
}




