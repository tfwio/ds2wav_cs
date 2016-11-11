/* tfwxo * 1/19/2016 * 2:10 AM */
//#define OMOD
//#define OLIMIT
//#define OIGNORE
#define OLIMIT
using System;
namespace on.drumsynth2
{
  /// <summary>
  /// This MathHelper was written for the first phase of translating ds2wav,
  /// before most floating point numbers were converted to double.
  /// </summary>
  static class MathHelper
  {
    const short default_lim=32000;
    static public string ByteCount(this long leng)
    {
      double len=leng;
      // http://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
      string[] sizes = { "B", "K", "M", "G" };
      int order = 0;
      while (len >= 1024 && order + 1 < sizes.Length) { order++; len = len/1024; }
      return String.Format("{0:N1}{1}", len, sizes[order]);
    }
    public const int RAND_MAX = int.MaxValue;
    static Random randy { get { return randy2; } set { randy2 = value; } }
    static Random randy2 = new Random(1);
    static public double rand() { return rand(RAND_MAX); }
    static public double rand(int min, int max) { return Convert.ToSingle(randy.Next(min,max)); }
    static public double rand(int max) { return Convert.ToSingle(randy.Next(max)); }
    static public double sin(double value) { return (double)Math.Sin(value); }
    static public double cos(double value) { return (double)Math.Cos(value); }
    static public double fabs(double value) { return Math.Abs(value); }
    static public double fmod(double a, double b) { return a % b; }
    static public double pow(double x, double y) { return (double)Math.Pow(x,y); }
    static public double exp(double value) { return (double)Math.Exp(value); }
    static public double sqrt(double value) { return (double) Math.Sqrt(value); }

    static public short lim(this short input, short min, short max)
    {
      return /*Convert.ToInt16*/(input <= min ? min : (input >= max ? max : input));
    }
    static public short lim(this double input, double min, double max)
    {
      return Convert.ToInt16(input <= min ? min : (input >= max ? max : input));
    }
    static public short lim(double input)
    {
      #if OLIMIT
      return Convert.ToInt16(input.lim(-default_lim,default_lim));
      // return Convert.ToInt16(input <= short.MinValue ? short.MinValue : (input >= short.MaxValue ? short.MaxValue : input));
      #elif OMOD
      return Convert.ToInt16(input >=0 ? input % (double)short.MaxValue : input % (double)short.MinValue);
      #else
      return (short)input;
      #endif
      
    }
  }
}

