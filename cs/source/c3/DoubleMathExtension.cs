/* oOo * 11/19/2007 : 8:00 AM */
using System;
using System.Drawing;

//	static class ByteExtension {
//		/// <summary>
//		/// Format a string of bytes to space separated hex digits.
//		/// </summary>
//		/// <param name="input">byte array</param>
//		/// <returns>00 0F F0 ...</returns>
//		static public string StringifyHex(this byte[] input) {
//			return DspAudio.Midi.MidiReader.SmfStringFormatter.byteToString(input);
//		}
//	}

// Most of these extensions were put here to cater to audio channels
// in a audio process.

namespace System
{
	static public class FloatPointExtension
	{
		static public FloatPoint Floor(this FloatPoint point)
		{
			return new FloatPoint(Math.Floor(point.X), Math.Floor(point.Y));
		}
	}
	static public class FloatMathExtension
	{
		static public float Combine(float amp, params float[] inputs)
		{
			float output = 0;
			foreach (float input in inputs) output += (amp * input);
			return output;
		}
		static public float Accumilate(this float self, params float[] inputs)
		{
			float f = self;
			foreach (float input in inputs) f += input;
			return f;
    }
    static public float Minimum(this float input, float min)
    {
      if (input <= min) return min;
      return input;
    }
    /// this method seems to be used a bit too much.  lighten up.
    static public double Floor(this float input)
    {
      return Math.Floor(input);
    }
    /// this method seems to be used a bit too much.  lighten up.
    static public double FloorMinimum(this float input, float min)
    {
      return Math.Floor(Minimum(input, min));
    }
    static public double Maximum(this float input, double max)
    {
      return input > max ? max : input;
    }
    static public double FloorMaximum(this float input, double max)
    {
      return input.Maximum(max).Floor();
    }
    static public double MinMax(this float input, double min, double max)
    {
      return input.Maximum(max).Minimum(min);
    }
    static public double FloorMinMax(this float input, double min, double max)
    {
      return input.MinMax(min, max).Floor();
    }
  }
	static public class DoubleMathExtension
	{
		static public float ToSingle(this double value) { return Convert.ToSingle(value); }
		static public int ToInt32(this double value) { return Convert.ToInt32(value); }
		static public double Minimum(this double input, double min)
		{
			if (input <= min) return min;
			return input;
    }
    static public double Floor(this double input)
    {
      return Math.Floor(input);
    }
    /// this method seems to be used a bit too much.  lighten up.
    static public double FloorMinimum(this double input, double min)
    {
      return Math.Floor(Minimum(input, min));
    }
    static public double Maximum(this double input, double max)
		{
			return input > max ? max : input;
		}
		static public double FloorMaximum(this double input, double max)
		{
			return input.Maximum(max).Floor();
		}
		static public double MinMax(this double input, double min, double max)
		{
			return input.Minimum(min).Maximum(max);
		}
		static public double FloorMinMax(this double input, double min, double max)
		{
			return input.Minimum(min).Maximum(max).Floor();
		}
	}
}
