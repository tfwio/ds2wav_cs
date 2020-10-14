/* oOo * 11/19/2007 : 8:00 AM */
using System;
namespace System
{
	static public class NumericExtensions
  {
	  public const int FactorialMax = 170;
	  
    /// <summary>
    /// Any number larger than 170 results in double.PosInfinity.
    /// </summary>
    static public double[] FactorialArray(this int nMax)
    {
      double[] output = new double[nMax];
      for (int i = 0; i < nMax; i++) {
        if (i == 0) output[i] = 1.0;
        else output[i] = output[i - 1] * i;
      }
      return output;
    }
    static public float Smallest(this float input, params float[] inputs) { return input.Conditional<float>( (a, b) => a < b ? a : b, inputs); }
    static public float Largest(this float input, params float[] inputs) { return input.Conditional<float>( (a, b) => a > b ? a : b, inputs); }
    public static T Conditional<T>(this T input, Func<T, T, T> condition, params T[] inputs)
    {
      var result = input;
      foreach (var i in inputs) result = condition(result, i);
      return result;
    }
    static public bool IsIn(this double number, double min, double max)
    {
      return (number >= min) && (number <= max);
    }
    static public bool IsIn(this float number, float min, float max)
    {
      return (number >= min) && (number <= max);
    }
    static public bool IsIn(this int number, int min, int max)
    {
      return (number >= min) && (number <= max);
    }
	  static public int Contain(this int number, int? min, int? max)
	  {
	    if (min.HasValue && number < min) return min.Value;
	    return max.HasValue && number > max.Value ? max.Value : number;
	  }
	  static public int Contain(this int number, float? min, float? max)
	  {
	    if (min.HasValue && number <= min) return Convert.ToInt32(min.Value);
	    return max.HasValue && number > max.Value ? Convert.ToInt32(max.Value) : number;
	  }
	  static public uint Contain(this uint number, uint? min, uint? max)
	  {
      if (min.HasValue && number <= min) return min.Value;
      return max.HasValue && number > max.Value ? max.Value : number;
	  }
	  static public float Contain(this float number, float? min, float? max)
	  {
      if (min.HasValue && number <= min) return min.Value;
      return max.HasValue && number > max.Value ? max.Value : number;;
	  }
	  static public double Contain(this double number, double? min, double? max)
	  {
	    if (min.HasValue && number <= min) return min.Value;
	    return max.HasValue && number > max.Value ? max.Value : number;
	  }
	}
}

