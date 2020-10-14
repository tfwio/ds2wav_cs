#region User/License
// Copyright (c) 2005-2013 tfwroble
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion
/* oOo * 11/28/2007 : 5:29 PM */
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace System.Drawing
{
  /// <remarks>
  /// Only Add and Neg calls have Integer parameters when compared to the
  /// other parameter based calls (because they were most recently added).
  /// </remarks>
  public class FloatPoint : IComparable
  {
    #region limiters
    public FloatPoint Smallest(params FloatPoint[] inputs)
    {
      return this.Conditional<FloatPoint>( (a, b) => a.Slope < b.Slope ? a : b, inputs);
    }
    public FloatPoint Largest(params FloatPoint[] inputs)
    {
      return this.Conditional<FloatPoint>( (a, b) => a.Slope > b.Slope ? a : b, inputs);
    }
    public FloatPoint Limit(FloatPoint min, FloatPoint max)
    {
      var result = this.Clone();
      result.X = result.X.MinMax(min.X,max.X).ToSingle();
      result.Y = result.Y.MinMax(min.Y, max.Y).ToSingle();
      return result;
    }
    #endregion

    /// <summary>
    /// X &lt; input.X ? X : input.X, Y &lt; input.Y ? Y : input.Y
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public FloatPoint Nearest(FloatPoint input)
    {
      return new FloatPoint(X < input.X ? X : input.X, Y < input.Y ? Y : input.Y);
    }
    /// <summary>
    /// FloatPoint(X &gt; input.X ? X : input.X, Y &gt; input.Y ? Y : input.Y)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public FloatPoint Furthest(FloatPoint input)
    {
      return new FloatPoint(X > input.X ? X : input.X, Y > input.Y ? Y : input.Y);
    }
    /// Returnes a Floored point (copy)
    static public FloatPoint Floor(FloatPoint source)
    {
      return new FloatPoint(Math.Floor(source.X),Math.Floor(source.Y));
    }
    /// nearest int: digits
    static public FloatPoint Round(FloatPoint source, int digits)
    {
      return new FloatPoint(Math.Round(source.X,digits),Math.Round(source.Y,digits));
    }
    /// nearest int
    static public FloatPoint Round(FloatPoint source)
    {
      return Round(source,0);
    }
    public FloatPoint Floored { get { return Floor(this); } }
    public FloatPoint Rounded { get { return Round(this,0); } }
    
    static public FloatPoint Empty { get { return new FloatPoint(0f); } }
    static public FloatPoint One { get { return new FloatPoint(1f); } }
    static public FloatPoint RootComplex { get { return new FloatPoint(1.0f, 0.0f); } }
    
    
    float _X, _Y;
    [XmlAttribute] public float X { get { return _X; } set { _X = value; } }
    [XmlAttribute] public float Y { get { return _Y; } set { _Y = value; } }
    
    #region Properties
    [XmlIgnore] public float Bigger { get { return (X >= Y)? X: Y; } }
    [XmlIgnore] public bool IsLand { get { return X >= Y; } }
    /// <summary>zerod'</summary>
    [XmlIgnore] public double Slope { get  { return Math.Sqrt(Math.Pow(X,2)+Math.Pow(Y,2)); }  }
    #endregion
    #region Static Methods
    static public FloatPoint Average(params FloatPoint[] xp)
    {
      FloatPoint p = new FloatPoint(0);
      foreach (FloatPoint pt in xp) p += pt;
      return p/xp.Length;
    }
    static public FloatPoint FlattenPoint(FloatPoint _pin, bool roundUp)
    {
      FloatPoint newP = _pin.Clone();
      if (newP.X==newP.Y) return newP;
      if (_pin.X > _pin.Y) { if (roundUp) newP.Y = newP.X; else newP.X = newP.Y; }
      else { if (!roundUp) newP.Y = newP.X; else newP.X = newP.Y; }
      return newP;
    }
    static public FloatPoint FlattenPoint(FloatPoint _pin) { return FlattenPoint(_pin,false); }
    /// <summary>same as FlattenPoint overload without boolean</summary>
    static public FloatPoint FlattenDown(FloatPoint _pin) { return FlattenPoint(_pin); }
    static public FloatPoint FlattenUp(FloatPoint _pin) { return FlattenPoint(_pin,true); }
    
    static public FloatPoint GetClientSize(Control ctl) { return ctl.ClientSize; }
    
    static public FloatPoint GetPaddingTopLeft(Padding pad) { return new FloatPoint(pad.Left,pad.Top); }
    static public FloatPoint GetPaddingOffset(Padding pad)  { return  new FloatPoint(pad.Horizontal,pad.Vertical); }
    
    //
    static public FloatPoint Angus(float offset, float ration, float phase) { return new FloatPoint(-Math.Sin(cvtf(ration,offset+phase)),Math.Cos(cvtf(ration,offset+phase))); }
    static public FloatPoint Angus(float offset, float ration) { return Angus(offset,ration,0.0f); }
    static float cvtf(float S, float I){ return (float)((Math.PI*2)*(I/S)); }
    /// scales src to dest
    static public FloatPoint Fit(FloatPoint dest, FloatPoint source)
    {
      return Fit(dest, source, 0);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="source"></param>
    /// <param name="ScaleFlagBit"> 0x00 | (width) 0x01 | (height) 0x02 = 0x03 so you can check both width and height bits with a value of three.</param>
    /// <returns></returns>
    static public FloatPoint Fit(FloatPoint dest, FloatPoint source, int ScaleFlagBit)
    {
      FloatPoint HX = dest / source;
      if (ScaleFlagBit == 0) return Fit( dest , source , 1 );
      else return (ScaleFlagBit == 1) ? source * HX.X : source * HX.Y;
    }
    #region Obsolete
    /*		/// todo: replace this with something more accurate and faster
		/// use Boxed as the default scale flag
		static public HPoint Fit(Point dest, Point source, scaleFlags sf)
		{
			HPoint skaler = new HPoint();
			float tr=0;
			string omax,cmax;
			if (source.X >= source.Y) omax = "width"; else omax="height";
			if (dest.X >= dest.Y) cmax = "width"; else cmax="height";
			switch (cmax)
			{
				case "width": if (omax==cmax) sf = scaleFlags.sWidth; else sf = scaleFlags.sHeight; break;
				case "height": if (omax==cmax) sf = scaleFlags.sHeight; else sf = scaleFlags.sWidth; break;
			}
		skale:
			switch (sf)
			{
				case scaleFlags.sHeight:
					if ( source.Y > dest.Y ) { skaler.Y = dest.Y; tr = (float)((float)dest.Y / (float)source.Y); skaler.X = (int)Math.Round(source.X*tr); }
					else skaler = source; break;
				case scaleFlags.sWidth:
					if ( source.X > dest.X ) { skaler.X = dest.X; tr = (float)((float)dest.X / (float)source.X); skaler.Y = (int)Math.Round(source.Y*tr); }
					else skaler = source; break;
			}
			if ((skaler.X > dest.X) | (skaler.Y > dest.Y))
			{
				switch (sf)
				{
					case scaleFlags.sWidth: sf = scaleFlags.sHeight; goto skale;
					case scaleFlags.sHeight: sf = scaleFlags.sWidth; goto skale;
				}
			}
			return skaler;
		}*/
    #endregion
    #endregion
    #region Help
    //		public XPoint Translate(XPoint offset, XPoint zoom, bool ZoomAfterOffset, bool MultiplyOffset)
    //		{
    //			return (ZoomAfterOffset) ? (this+((MultiplyOffset) ? offset*zoom : offset ))*zoom : (this*zoom)+((MultiplyOffset) ? offset*zoom : offset );
    //		}
    /// <summary>
    /// Note that this is not a suggested optimal translation.
    /// The relation of most transforms should be provided via
    /// the context of the window or container.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="zoom"></param>
    /// <returns></returns>
    public FloatPoint Translate(FloatPoint translateOffset, FloatPoint translateZoom)
    {
      return (this+translateOffset)*translateZoom;
    }
    public FloatPoint Translate(double offset, double zoom)
    {
      return (this+new FloatPoint(offset))*new FloatPoint(zoom);
    }
    #endregion
    #region Helpers “Most of which are obsoltet”
    public FloatPoint GetRation(FloatPoint dst)
    {
      return dst/this;
    }
    public FloatPoint GetScaledRation(FloatPoint dst)
    {
      return this*(dst/this);
    }
    public float Dimension() { return X*Y; }
    /// <summary>Returns a new flattened point</summary>
    public FloatPoint Flat(bool roundUp) { return FlattenPoint(this,roundUp); }
    /// <summary>Flattens the calling point</summary>
    public void Flatten(bool roundUp) { FloatPoint f = Flat(roundUp); this.X = f.X; this.Y = f.Y; f = null; }
    /// <summary>use Flat or flatten calls.</summary>
    public FloatPoint ScaleTo(FloatPoint point)
    {
      if (point.X==X && point.Y==Y) throw new InvalidOperationException("you mucker");
      System.Windows.Forms.MessageBox.Show( string.Format("X: {1},Y: {0}",Y/point.Y,X/point.X) );
      if (X > point.Y)
      {
        #if CONSOLE && COR3
        Global.cstat(ConsoleColor.Red,"X is BIGGER");
        #else
        Debug.Print("X is BIGGER");
        #endif
      }
      #if CONSOLE && COR3
      else Global.cstat(ConsoleColor.Red,"X is SMALLER");
      #else
      else Debug.Print("X is SMALLER");
      #endif
      return this;
    }
    #endregion
    #region Operators
    
    static public FloatPoint operator +(FloatPoint a, FloatPoint b){ return new FloatPoint(a.X+b.X,a.Y+b.Y); }
    static public FloatPoint operator +(FloatPoint a, Point b){ return new FloatPoint(a.X+b.X,a.Y+b.Y); }
    static public FloatPoint operator +(FloatPoint a, int b){ return new FloatPoint(a.X+b,a.Y+b); }
    static public FloatPoint operator +(FloatPoint a, float b){ return new FloatPoint(a.X+b,a.Y+b); }
    static public FloatPoint operator +(FloatPoint a, double b){ return new FloatPoint(a.X+b,a.Y+b); }
    static public FloatPoint operator -(FloatPoint a){ return new FloatPoint(-a.X,-a.Y); }
    static public FloatPoint operator -(FloatPoint a, FloatPoint b){ return new FloatPoint(a.X-b.X,a.Y-b.Y); }
    static public FloatPoint operator -(FloatPoint a, Point b){ return new FloatPoint(a.X-b.X,a.Y-b.Y); }
    static public FloatPoint operator -(FloatPoint a, int b){ return new FloatPoint(a.X-b,a.Y-b); }
    static public FloatPoint operator -(FloatPoint a, float b){ return new FloatPoint(a.X-b,a.Y-b); }
    static public FloatPoint operator -(FloatPoint a, double b){ return new FloatPoint(a.X-b,a.Y-b); }
    static public FloatPoint operator /(FloatPoint a, FloatPoint b){ return new FloatPoint(a.X/b.X,a.Y/b.Y); }
    static public FloatPoint operator /(FloatPoint a, Point b){ return new FloatPoint(a.X/b.X,a.Y/b.Y); }
    static public FloatPoint operator /(FloatPoint a, int b){ return new FloatPoint(a.X/b,a.Y/b); }
    static public FloatPoint operator /(FloatPoint a, float b){ return new FloatPoint(a.X/b,a.Y/b); }
    static public FloatPoint operator /(FloatPoint a, double b){ return new FloatPoint(a.X/b,a.Y/b); }
    static public FloatPoint operator *(FloatPoint a, FloatPoint b){ return new FloatPoint(a.X*b.X,a.Y*b.Y); }
    static public FloatPoint operator *(FloatPoint a, Point b){ return new FloatPoint(a.X*b.X,a.Y*b.Y); }
    static public FloatPoint operator *(FloatPoint a, int b){ return new FloatPoint(a.X*b,a.Y*b); }
    static public FloatPoint operator *(FloatPoint a, float b){ return new FloatPoint(a.X*b,a.Y*b); }
    static public FloatPoint operator *(FloatPoint a, double b){ return new FloatPoint(a.X*(float)b,a.Y*(float)b); }
    static public FloatPoint operator %(FloatPoint a, FloatPoint b){ return new FloatPoint(a.X%b.X,a.Y%b.Y); }
    static public FloatPoint operator %(FloatPoint a, Point b){ return new FloatPoint(a.X%b.X,a.Y%b.Y); }
    
    static public FloatPoint operator ++(FloatPoint a){ return new FloatPoint(a.X++,a.Y++); }
    static public FloatPoint operator --(FloatPoint a){ return new FloatPoint(a.X--,a.Y--); }
    
    static public bool operator >(FloatPoint a,FloatPoint b){ return ((a.X>b.X) & (a.Y>b.Y)); }
    static public bool operator >=(FloatPoint a,FloatPoint b){ return ((a.X>=b.X) & (a.Y>=b.Y)); }
    static public bool operator <(FloatPoint a,FloatPoint b){ return ((a.X<b.X) & (a.Y<b.Y)); }
    static public bool operator <=(FloatPoint a,FloatPoint b){ return ((a.X<=b.X) & (a.Y<=b.Y)); }
    
    #endregion
    #region Operators Implicit
    static public implicit operator Point(FloatPoint a){ return new Point((int)a.X,(int)a.Y); }
    static public implicit operator PointF(FloatPoint a){ return new PointF(a.X,a.Y); }
    static public implicit operator Size(FloatPoint a){ return new Size((int)a.X,(int)a.Y); }
    static public implicit operator SizeF(FloatPoint a){ return new SizeF(a.X,a.Y); }
    static public implicit operator FloatPoint(Size s){ return new FloatPoint(s); }
    static public implicit operator FloatPoint(SizeF s){ return new FloatPoint(s); }
    static public implicit operator FloatPoint(Point s){ return new FloatPoint(s); }
    static public implicit operator FloatPoint(PointF s){ return new FloatPoint(s); }
    static public implicit operator FloatPoint(DPoint s){ return new FloatPoint(s); }
    static public implicit operator FloatPoint(double s){ return new FloatPoint(s, s); }
    static public implicit operator FloatPoint(float s){ return new FloatPoint(s, s); }
    static public implicit operator FloatPoint(int s){ return new FloatPoint(s, s); }

    //static public implicit operator FloatPoint(float s) { return new FloatPoint(s,s); }

    #endregion
    #region Maths
    public bool IsXG(FloatPoint P) { return X>P.X; }
    public bool IsYG(FloatPoint P) { return Y>P.Y; }
    public bool IsXL(FloatPoint P) { return X<P.X; }
    public bool IsYL(FloatPoint P) { return Y<P.Y; }
    
    public bool IsLEq(FloatPoint p) { return (X<=p.X) && (Y<=p.Y); }
    public bool IsGEq(FloatPoint p) { return (X>=p.X) && (Y>=p.Y); }
    
    public bool IsXGEq(FloatPoint P) { return IsXG(P)&IsXG(P); }
    public bool IsYGEq(FloatPoint P) { return IsYG(P)&IsYG(P); }
    public bool IsXLEq(FloatPoint P) { return IsXG(P)&IsXG(P); }
    public bool IsYLEq(FloatPoint P) { return IsYG(P)&IsYG(P); }
    public bool IsXEq(FloatPoint P) { return X==P.X; }
    public bool IsYEq(FloatPoint P) { return Y==P.Y; }
    
    public FloatPoint Multiply(params FloatPoint[] P) {
      if (P.Length==0) throw new ArgumentException("there is no data!");
      if (P.Length==1) return new FloatPoint(X,Y)*P[0];
      FloatPoint NewPoint = new FloatPoint(X,Y)*P[0];
      for (int i = 1; i < P.Length; i++)
      {
        NewPoint *= P[i];
      }
      return NewPoint;
    }
    public FloatPoint Multiply(params float[] P) {
      if (P.Length==0) throw new ArgumentException("there is no data!");
      if (P.Length==1) return new FloatPoint(X,Y)*P[0];
      FloatPoint NewPoint = new FloatPoint(X,Y)*P[0];
      for (int i = 1; i < P.Length; i++)
      {
        NewPoint *= P[i];
      }
      return NewPoint;
    }
    public FloatPoint Divide(params FloatPoint[] P)
    {
      if (P.Length==0) throw new ArgumentException("there is no data!");
      if (P.Length==1) return new FloatPoint(X,Y)/P[0];
      FloatPoint NewPoint = new FloatPoint(X,Y)/P[0];
      for (int i = 1; i < P.Length; i++)
      {
        NewPoint /= P[i];
      }
      return NewPoint;
    }
    
    public FloatPoint MulX(params FloatPoint[] P)
    {
      FloatPoint PBase = Clone();
      foreach (FloatPoint RefPoint in P) PBase.X *= RefPoint.X;
      return PBase;
    }
    public FloatPoint MulY(params FloatPoint[] P)
    {
      FloatPoint PBase = Clone();
      foreach (FloatPoint RefPoint in P) PBase.Y *= RefPoint.Y;
      return PBase;
    }
    public FloatPoint DivX(params FloatPoint[] P)
    {
      FloatPoint PBase = Clone();
      foreach (FloatPoint RefPoint in P) PBase.X /= RefPoint.X;
      return PBase;
    }
    public FloatPoint DivY(params FloatPoint[] P)
    {
      FloatPoint PBase = Clone();
      foreach (FloatPoint RefPoint in P) PBase.Y /= RefPoint.Y;
      return PBase;
    }
    public FloatPoint AddX(params FloatPoint[] P)
    {
      FloatPoint PBase = Clone();
      foreach (FloatPoint RefPoint in P) PBase.X += RefPoint.X;
      return PBase;
    }
    public FloatPoint AddX(params float[] P)
    {
      FloatPoint PBase = Clone();
      foreach (var RefPoint in P) PBase.X += RefPoint;
      return PBase;
    }
    public FloatPoint AddY(params FloatPoint[] P)
    {
      FloatPoint PBase = Clone();
      foreach (FloatPoint RefPoint in P) PBase.Y += RefPoint.Y;
      return PBase;
    }
    public FloatPoint AddY(params int[] P)
    {
      FloatPoint PBase = Clone();
      foreach (int RefPoint in P) PBase.Y += RefPoint;
      return PBase;
    }
    public FloatPoint AddY(params float[] P)
    {
      FloatPoint PBase = Clone();
      foreach (int RefPoint in P) PBase.Y += RefPoint;
      return PBase;
    }
    public FloatPoint NegX(params FloatPoint[] P)
    {
      FloatPoint PBase = Clone();
      foreach (FloatPoint RefPoint in P) PBase.X -= RefPoint.X;
      return PBase;
    }
    public FloatPoint NegX(params int[] P)
    {
      FloatPoint PBase = Clone();
      foreach (int Ref in P) PBase.X -= Ref;
      return PBase;
    }
    public FloatPoint NegY(params FloatPoint[] P)
    {
      FloatPoint PBase = Clone();
      foreach (FloatPoint RefPoint in P) PBase.Y -= RefPoint.Y;
      return PBase;
    }
    public FloatPoint NegY(params int[] P)
    {
      FloatPoint PBase = Clone();
      foreach (int Ref in P) PBase.Y -= Ref;
      return PBase;
    }
    #endregion
    
//		public FloatPoint() : this(0,0){  }
    public FloatPoint(){}
    public FloatPoint(FloatPoint y){ this._X = y.X; this._Y = y.Y; }
    public FloatPoint(float x, float y){ _X = x; _Y = y; }
    public FloatPoint(int value) : this(value,value) {  }
    public FloatPoint(float value) : this(value,value) {  }
    public FloatPoint(double value) : this(value,value) {  }
    public FloatPoint(double x, double y) : this((float)x,(float)y) {  }
    public FloatPoint(MouseEventArgs P) : this(P.X,P.Y) {}
    public FloatPoint(DPoint P) : this(P.X,P.Y) {}
    public FloatPoint(Point P){ _X = P.X; _Y = P.Y; }
    public FloatPoint(PointF P){ _X = P.X; _Y = P.Y; }
    public FloatPoint(Size P){ _X = P.Width; _Y = P.Height; }
    public FloatPoint(SizeF P){ _X = P.Width; _Y = P.Height; }

    #region Obj
    // Object ====================================
    public FloatPoint Clone(){ return (new FloatPoint(X,Y)); }
    public void CopyPoint(FloatPoint inPoint) { X=inPoint.X; Y=inPoint.Y; }
    public override string ToString() { return String.Format("{0}x{1}",X,Y); }
    #endregion

    #region IComparable
    int IComparable.CompareTo(object obj)
    {
      FloatPoint o = FloatPoint.Empty;
      if (!(obj is FloatPoint)) return 0;
      o = (FloatPoint) obj;
      if (o.X.Equals(0f) && o.Y.Equals(0f)) return 0;
      if (this < o) return -1;
      return this > o ? 1 : 0;
    }
    #endregion
  }
  
}
