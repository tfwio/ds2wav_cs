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
using System.Drawing.Utilities.SuperOld;
using System.Xml.Serialization;

namespace System.Drawing
{
	//
	public class DPoint
	{
		static public DPoint Empty { get { return new DPoint(0D); } }
		static public DPoint One { get { return new DPoint(1D); } }
		
		double _X=0f, _Y=0f;
		[XmlAttribute] public double X { get { return _X; } set { _X = value; } }
		[XmlAttribute] public double Y { get { return _Y; } set { _Y = value; } }
		
		#region Properties
		[XmlIgnore] public double Bigger { get { return (X >= Y)? X: Y; } }
		[XmlIgnore] public bool IsLand { get { return X >= Y; } }
		/// <summary>zerod’</summary>
		[XmlIgnore] public double Slope { get  { return Math.Sqrt(Math.Pow(X,2)+Math.Pow(Y,2)); }  }
		#endregion
		#region Static Methods
		static public DPoint FlattenPoint(DPoint _pin, bool roundUp)
		{
			DPoint newP = _pin.Clone();
			if (newP.X==newP.Y) return newP;
			if (_pin.X > _pin.Y) { if (roundUp) newP.Y = newP.X; else newP.X = newP.Y; }
			else { if (!roundUp) newP.Y = newP.X; else newP.X = newP.Y; }
			return newP;
		}
		static public DPoint FlattenPoint(DPoint _pin) { return FlattenPoint(_pin,false); }
		/// <summary>same as FlattenPoint overload without boolean</summary>
		static public DPoint FlattenDown(DPoint _pin) { return FlattenPoint(_pin); }
		static public DPoint FlattenUp(DPoint _pin) { return FlattenPoint(_pin,true); }
		#endregion
		#region Helpers “Obsolete?”
		//public double Slope() { return Hypotenuse; }
		//public double Sine { get { return Y/Hypotenuse; } }
		//public double Cosine { get { return X/Hypotenuse; } }
		//public double Tangent { get { return Y/X; } }
		//public double SlopeRatio(XPoint cmp) { return Slope()/cmp.Slope); }
		/// <summary>Returns a new flattened point</summary>
		public DPoint Flat(bool roundUp) { return FlattenPoint(this,roundUp); }
		/// <summary>Flattens the calling point</summary>
		public void Flatten(bool roundUp) { DPoint f = Flat(roundUp); this.X = f.X; this.Y = f.Y; f = null; }
		/// <summary>use Flat or flatten calls.</summary>
		public DPoint ScaleTo(DPoint point)
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
		public DPoint GetRation(DPoint dst)
		{
			return dst/this;
		}
		public DPoint GetScaledRation(DPoint dst)
		{
			return this*(dst/this);
		}
		public double Dimension() { return X*Y; }
		#endregion
		#region Help
		public DPoint Translate(DPoint offset, DPoint zoom)
		{
			return (this+offset)*zoom;
		}
		public DPoint Translate(double offset, double zoom)
		{
			return (this+new DPoint(offset*zoom));
		}
		#endregion
		#region Maths
		public bool IsXG(DPoint P) { return X>P.X; }
		public bool IsYG(DPoint P) { return Y>P.Y; }
		public bool IsXL(DPoint P) { return X<P.X; }
		public bool IsYL(DPoint P) { return Y<P.Y; }
		
		public bool IsLEq(DPoint p) { return (X<=p.X) && (Y<=p.Y); }
		public bool IsGEq(DPoint p) { return (X>=p.X) && (Y>=p.Y); }
		
		public bool IsXGEq(DPoint P) { return IsXG(P)&IsXG(P); }
		public bool IsYGEq(DPoint P) { return IsYG(P)&IsYG(P); }
		public bool IsXLEq(DPoint P) { return IsXG(P)&IsXG(P); }
		public bool IsYLEq(DPoint P) { return IsYG(P)&IsYG(P); }
		public bool IsXEq(DPoint P) { return X==P.X; }
		public bool IsYEq(DPoint P) { return Y==P.Y; }
		
		public DPoint Multiply(params DPoint[] P) {
			if (P.Length==0) throw new ArgumentException("there is no data!");
			if (P.Length==1) return new DPoint(X,Y)*P[0];
			DPoint NewPoint = new DPoint(X,Y)*P[0];
			for (int i = 1; i < P.Length; i++)
			{
				NewPoint *= P[i];
			}
			return NewPoint;
		}
		public DPoint Multiply(params float[] P) {
			if (P.Length==0) throw new ArgumentException("there is no data!");
			if (P.Length==1) return new DPoint(X,Y)*P[0];
			DPoint NewPoint = new DPoint(X,Y)*P[0];
			for (int i = 1; i < P.Length; i++)
			{
				NewPoint *= P[i];
			}
			return NewPoint;
		}
		public DPoint Divide(params DPoint[] P)
		{
			if (P.Length==0) throw new ArgumentException("there is no data!");
			if (P.Length==1) return new DPoint(X,Y)/P[0];
			DPoint NewPoint = new DPoint(X,Y)/P[0];
			for (int i = 1; i < P.Length; i++)
			{
				NewPoint /= P[i];
			}
			return NewPoint;
		}
		public DPoint MulX(params DPoint[] P)
		{
			DPoint PBase = Clone();
			foreach (DPoint RefPoint in P) PBase.X *= RefPoint.X;
			return PBase;
		}
		public DPoint MulY(params DPoint[] P)
		{
			DPoint PBase = Clone();
			foreach (DPoint RefPoint in P) PBase.Y *= RefPoint.Y;
			return PBase;
		}
		public DPoint DivX(params DPoint[] P)
		{
			DPoint PBase = Clone();
			foreach (DPoint RefPoint in P) PBase.X /= RefPoint.X;
			return PBase;
		}
		public DPoint DivY(params DPoint[] P)
		{
			DPoint PBase = Clone();
			foreach (DPoint RefPoint in P) PBase.Y /= RefPoint.Y;
			return PBase;
		}
		public DPoint AddX(params DPoint[] P)
		{
			DPoint PBase = Clone();
			foreach (DPoint RefPoint in P) PBase.X += RefPoint.X;
			return PBase;
		}
		public DPoint AddY(params DPoint[] P)
		{
			DPoint PBase = Clone();
			foreach (DPoint RefPoint in P) PBase.Y += RefPoint.Y;
			return PBase;
		}
		public DPoint AddY(params int[] P)
		{
			DPoint PBase = Clone();
			foreach (int RefPoint in P) PBase.Y += RefPoint;
			return PBase;
		}
		public DPoint NegX(params DPoint[] P)
		{
			DPoint PBase = Clone();
			foreach (DPoint RefPoint in P) PBase.X -= RefPoint.X;
			return PBase;
		}
		public DPoint NegX(params int[] P)
		{
			DPoint PBase = Clone();
			foreach (int Ref in P) PBase.X -= Ref;
			return PBase;
		}
		public DPoint NegY(params DPoint[] P)
		{
			DPoint PBase = Clone();
			foreach (DPoint RefPoint in P) PBase.Y -= RefPoint.Y;
			return PBase;
		}
		public DPoint NegY(params int[] P)
		{
			DPoint PBase = Clone();
			foreach (int Ref in P) PBase.Y -= Ref;
			return PBase;
		}
		#endregion
		#region Static Methods
		static public DPoint Average(params DPoint[] xp)
		{
			DPoint p = new DPoint(0);
			foreach (DPoint pt in xp) p += pt;
			return p/xp.Length;
		}
		static public DPoint GetClientSize(Control ctl) { return ctl.ClientSize; }
		static public DPoint GetPaddingTopLeft(Padding pad) { return new DPoint(pad.Left,pad.Top); }
		static public DPoint GetPaddingOffset(Padding pad) { return new DPoint(pad.Left+pad.Right,pad.Top+pad.Bottom); }
		// =======================================================
		static public DPoint Angus(float offset, float ration, float phase) { return new DPoint(-Math.Sin(cvtf(ration,offset+phase)),Math.Cos(cvtf(ration,offset+phase))); }
		static public DPoint Angus(float offset, float ration) { return Angus(offset,ration,0.0f); }
		static float cvtf(float S, float I){ return (float)((Math.PI*2)*(I/S)); }
		// =======================================================
		/// • AutoScale — multiplies agains largest point in “dest / source”
		static public DPoint Fit(DPoint dest, DPoint source)
		{
			return Fit(dest,source,scaleFlags.autoScale);
		}
		/// • AutoScale — Multiplies against largest source size: ( ( source.X | source.Y ) * ( dest / source.X | source.Y ) )<br/>•
		/// ScaleWidth ( dest * source.X )
		static public DPoint Fit(DPoint dest, DPoint source, scaleFlags sf)
		{
			DPoint HX = dest/source;
			if (sf== scaleFlags.autoScale) return (HX.Y > HX.X) ? source*HX.X : source * HX.Y;
			else return (sf== scaleFlags.sWidth) ? source*HX.X : source*HX.Y;
		}
		
		#endregion
		#region Operators
		static public DPoint operator +(DPoint a, DPoint b){ return new DPoint(a.X+b.X,a.Y+b.Y); }
		static public DPoint operator +(DPoint a, Point b){ return new DPoint(a.X+b.X,a.Y+b.Y); }
		static public DPoint operator +(DPoint a, int b){ return new DPoint(a.X+b,a.Y+b); }
		static public DPoint operator +(DPoint a, float b){ return new DPoint(a.X+b,a.Y+b); }
		static public DPoint operator +(DPoint a, double b){ return new DPoint(a.X+b,a.Y+b); }
		static public DPoint operator -(DPoint a){ return new DPoint(-a.X,-a.Y); }
		static public DPoint operator -(DPoint a, DPoint b){ return new DPoint(a.X-b.X,a.Y-b.Y); }
		static public DPoint operator -(DPoint a, Point b){ return new DPoint(a.X-b.X,a.Y-b.Y); }
		static public DPoint operator -(DPoint a, int b){ return new DPoint(a.X-b,a.Y-b); }
		static public DPoint operator -(DPoint a, float b){ return new DPoint(a.X-b,a.Y-b); }
		static public DPoint operator -(DPoint a, double b){ return new DPoint(a.X-b,a.Y-b); }
		static public DPoint operator /(DPoint a, DPoint b){ return new DPoint(a.X/b.X,a.Y/b.Y); }
		static public DPoint operator /(DPoint a, Point b){ return new DPoint(a.X/b.X,a.Y/b.Y); }
		static public DPoint operator /(DPoint a, int b){ return new DPoint(a.X/b,a.Y/b); }
		static public DPoint operator /(DPoint a, float b){ return new DPoint(a.X/b,a.Y/b); }
		static public DPoint operator /(DPoint a, double b){ return new DPoint(a.X/b,a.Y/b); }
		static public DPoint operator *(DPoint a, DPoint b){ return new DPoint(a.X*b.X,a.Y*b.Y); }
		static public DPoint operator *(DPoint a, Point b){ return new DPoint(a.X*b.X,a.Y*b.Y); }
		static public DPoint operator *(DPoint a, int b){ return new DPoint(a.X*b,a.Y*b); }
		static public DPoint operator *(DPoint a, float b){ return new DPoint(a.X*b,a.Y*b); }
		static public DPoint operator *(DPoint a, double b){ return new DPoint(a.X*(float)b,a.Y*(float)b); }
		static public DPoint operator %(DPoint a, DPoint b){ return new DPoint(a.X%b.X,a.Y%b.Y); }
		static public DPoint operator %(DPoint a, Point b){ return new DPoint(a.X%b.X,a.Y%b.Y); }
		static public DPoint operator %(DPoint a, int b){ return new DPoint(a.X % b,a.Y % b); }
		static public DPoint operator %(DPoint a, float b){ return new DPoint(a.X % b,a.Y % b); }
		static public DPoint operator %(DPoint a, double b){ return new DPoint(a.X % b,a.Y % b); }
		static public DPoint operator ++(DPoint a){ return new DPoint(a.X++,a.Y++); }
		static public DPoint operator --(DPoint a){ return new DPoint(a.X--,a.Y--); }
		static public bool operator >(DPoint a,DPoint b){ return ((a.X>b.X) & (a.Y>b.Y)); }
		static public bool operator <(DPoint a,DPoint b){ return ((a.X<b.X) & (a.Y<b.Y)); }
		#endregion
		#region Operators Implicit
		static public implicit operator Point(DPoint a){ return new Point((int)a.X,(int)a.Y); }
		static public implicit operator PointF(DPoint a){ return new PointF((float)a.X,(float)a.Y); }
		static public implicit operator Size(DPoint a){ return new Size((int)a.X,(int)a.Y); }
		static public implicit operator SizeF(DPoint a){ return new SizeF((float)a.X,(float)a.Y); }
		static public implicit operator DPoint(Size s){ return new DPoint(s); }
		static public implicit operator DPoint(SizeF s){ return new DPoint(s); }
		static public implicit operator DPoint(Point s){ return new DPoint(s); }
		static public implicit operator DPoint(PointF s){ return new DPoint(s); }
		#endregion
		
		public DPoint(){ }
		public DPoint(double x, double y){ _X = x; _Y = y; }
		public DPoint(int value) : this(value,value) {  }
		public DPoint(long value) : this(value,value) {  }
		public DPoint(float value) : this((double)value,(double)value) {  }
		public DPoint(double value) : this(value,value) {  }
		public DPoint(FloatPoint value) : this(value.X,value.Y) {  }
		public DPoint(Point P){ _X = P.X; _Y = P.Y; }
		public DPoint(PointF P){ _X = P.X; _Y = P.Y; }
		public DPoint(Size P){ _X = P.Width; _Y = P.Height; }
		public DPoint(SizeF P){ _X = P.Width; _Y = P.Height; }
		
		#region Object
		public DPoint Clone(){ return new DPoint(X,Y); }
		public void CopyPoint(DPoint inPoint) { X=inPoint.X; Y=inPoint.Y; }
		public override string ToString() { return String.Format("XPoint:X:{0},Y:{1}",X,Y); }
		#endregion
		
	}

}
