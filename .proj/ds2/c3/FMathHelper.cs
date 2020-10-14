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
  /// before most floating point numbers were converted to float.
  /// </summary>
  static class FloatMath
  {
    const short default_lim=32000;
    public const int RAND_MAX = int.MaxValue;
    static Random randy { get; set; } = new Random(1);
    static public float rand() { return rand(RAND_MAX); }
    static public float rand(int min, int max) { return (float)(randy.Next(min,max)); }
    static public float rand(int max) { return (float)(randy.Next(max)); }
    static public float sin(float value) { return (float)Math.Sin(value); }
    static public float cos(float value) { return (float)Math.Cos(value); }
    static public float fabs(float value) { return Math.Abs(value); }
    static public float fmod(float a, float b) { return a % b; }
    static public float pow(float x, float y) { return (float)Math.Pow(x,y); }
    /// <summary>
    /// returns e to the specified power
    /// </summary>
    /// <param name="value">Value.</param>
    static public float exp(float value) { return (float)Math.Exp(value); }
    static public float sqrt(float value) { return (float) Math.Sqrt(value); }

    static public short lim(this float input, float min, float max)
    {
      return (short)(input <= min ? min : (input >= max ? max : input));
    }
    static public short lim(float input)
    {
      #if OLIMIT
      return (short)(input.lim(-default_lim,default_lim));
      // return Convert.ToInt16(input <= short.MinValue ? short.MinValue : (input >= short.MaxValue ? short.MaxValue : input));
      #elif OMOD
      return Convert.ToInt16(input >=0 ? input % (float)short.MaxValue : input % (float)short.MinValue);
      #else
      return (short)input;
      #endif
      
    }
  }
}



