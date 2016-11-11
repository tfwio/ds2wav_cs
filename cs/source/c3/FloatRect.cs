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
/* THIS CALSS HAS NOT YET FULLY BEEN TESTED */
using System;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace System.Drawing
{
  #region “ FloatRect ”
  public class FloatRect
  {
    public bool Contains(FloatPoint point)
    {
      return point >= this.Location && point <= this.BottomRight;
    }
    public bool Contains(FloatRect point)
    {
      return point.Location >= this.Location && point.BottomRight <= this.BottomRight;
    }
    public FloatRect Move(float amount)
    {
      var rect     = Clone();
      rect.Y      += amount;
      rect.X      += amount;
      return rect;
    }
//    public FloatRect MoveX(float amount)
//    {
//      var rect     = Clone();
//      rect.X      += amount;
//      return rect;
//    }
//    public FloatRect MoveY(float amount)
//    {
//      var rect     = Clone();
//      rect.Y      += amount;
//      return rect;
//    }
    public FloatRect Shrink(float amount/*, bool floor=true*/)
    {
      var rect     = Clone();
      rect.Width  -= (amount);
      rect.Height -= (amount);
      return rect;
    }
    
    /// <summary>
    /// returns a floored copy (EG: Math.Floor(...))
    /// </summary>
    public FloatRect Floored { get { return Floor(this); } }
    static public FloatRect Floor(FloatRect source) { return new FloatRect(source.Location.Floored,source.Size.Floored); }
    static public FloatRect Round(FloatRect source, int ith) { return new FloatRect(source.Location.Rounded,source.Size.Rounded); }
    static public FloatRect Zero { get { return new FloatRect(0,0,0,0); } }
    ////////////////////////////////////////////////////////////////////////
    [XmlIgnore] public FloatPoint Location { get { return ztl_LOC; } set { ztl_LOC = value; } }
    [XmlIgnore] public FloatPoint Size = FloatPoint.Empty;
    ////////////////////////////////////////////////////////////////////////
    /// <summary> (read only) </summary>
    [XmlAttribute] public float Width  { get { return Size.X; } set { Size.X = value; } }
    [XmlAttribute] public float Height { get { return Size.Y; } set { Size.Y = value; } }
    ////////////////////////////////////////////////////////////////////////
    [XmlIgnore] public FloatPoint TopLeft      { get { return ztl_TL; } }
    [XmlIgnore] public FloatPoint TopRight     { get { return ztl_TR; } }
    [XmlIgnore] public FloatPoint Center       { get { return ztl_CE; } }
    [XmlIgnore] public FloatPoint BottomRight  { get { return ztl_BR; } }
    [XmlIgnore] public FloatPoint BottomLeft   { get { return ztl_BL; } }
    ////////////////////////////////////////////////////////////////////////
    public bool Zero_At_TopLeft = true;
    ////////////////////////////////////////////////////////////////////////
    FloatPoint ztl_LOC = FloatPoint.Empty;
    FloatPoint ztl_TL { get { return new FloatPoint(0,0); } }
    FloatPoint ztl_TR { get { return new FloatPoint(Width,0); } }
    FloatPoint ztl_BR { get { return Location+Size; } }
    FloatPoint ztl_BL { get { return Location+new FloatPoint(0,Height); } }
    FloatPoint ztl_CE { get { return Size * 0.5f; } }
    float ztl_T { get { return Location.Y; } }
    float ztl_B { get { return Location.Y+Size.Y; } }
    float ztl_L { get { return Location.X; } set { Location.X = value; } }
    float ztl_R { get { return (Location+Size).X; } }
    ////////////////////////////////////////////////////////////////////////
    [XmlAttribute] public float X   { get { return Location.X; } set { Location.X=value; } }
    [XmlAttribute] public float Y   { get { return Location.Y; } set { Location.Y=value; } }
    ////////////////////////////////////////////////////////////////////////
    [XmlIgnore] public float Top    { get { return ztl_T; } }
    [XmlIgnore] public float Bottom { get { return ztl_B; } }
    [XmlIgnore] public float Left   { get { return ztl_L; } set { ztl_L = value; } }
    [XmlIgnore] public float Right  { get { return ztl_R; } }
    ////////////////////////////////////////////////////////////////////////
    //	operator
    #region Standard +,-,*,/,++,--
    static public FloatRect operator +(FloatRect a, FloatRect b) { return new FloatRect(a.X+b.X,a.Y+b.Y,a.Width+b.Width,a.Height+b.Height); }
    static public FloatRect operator -(FloatRect a, FloatRect b){ return new FloatRect(a.X-b.X,a.Y-b.Y,a.Width-b.Width,a.Height-b.Height); }
    static public FloatRect operator /(FloatRect a, FloatRect b){ return new FloatRect(a.X/b.X,a.Y/b.Y,a.Width/b.Width,a.Height/b.Height); }
    static public FloatRect operator *(FloatRect a, FloatRect b){ return new FloatRect(a.X*b.X,a.Y*b.Y,a.Width*b.Width,a.Height*b.Height); }
    static public FloatRect operator %(FloatRect a, FloatRect b){ return new FloatRect(a.X%b.X,a.Y%b.Y,a.Width%b.Width,a.Height%b.Height); }
    static public FloatRect operator ++(FloatRect a) { return new FloatRect(a.X++,a.Y++,a.Width++,a.Height++); }
    static public FloatRect operator --(FloatRect a) { return new FloatRect(a.X--,a.Y--,a.Width--,a.Height--); }
    #endregion
    
    #region implicit operator Point,PointF
    /// <summary>
    /// We use rounding before int conversion
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    static public implicit operator Rectangle(FloatRect a){ return new Rectangle((int)Math.Round(a.X,0),(int)Math.Round(a.Y,0),(int)Math.Round(a.Width,0),(int)Math.Round(a.Height,0)); }
    
    static public implicit operator RectangleF(FloatRect a){ return new RectangleF(a.X,a.Y,a.Width,a.Height); }
    static public implicit operator Padding(FloatRect a){ return new Padding((int)a.X,(int)a.Y,(int)a.Width,(int)a.Height); }
    static public implicit operator FloatRect(Rectangle a){ return new FloatRect(a.X,a.Y,a.Right,a.Bottom); }
    static public implicit operator FloatRect(RectangleF a){ return new FloatRect(a.X,a.Y,a.Right,a.Bottom); }
    #endregion
    
    public FloatRect Clone(){ return new FloatRect(Location.X,Location.Y,Size.X,Size.Y); }
    ///	static FromControl Methods (relative to the control)
    static public FloatRect FromClientInfo(FloatPoint ClientSize, Padding pad){ return new FloatRect(FloatPoint.GetPaddingTopLeft(pad),ClientSize-FloatPoint.GetPaddingOffset(pad)); }
    ///	static FromControl Methods (relative to the control)
    static public FloatRect FromControl(Control ctl, bool usepadding){ return FromControl(ctl,(usepadding)?ctl.Padding:Padding.Empty); }
    ///	static FromControl Methods (relative to the control)
    static public FloatRect FromControl(Control ctl, Padding pad){ return new FloatRect(FloatPoint.GetPaddingTopLeft(pad),FloatPoint.GetClientSize(ctl)-FloatPoint.GetPaddingOffset(pad)); }
    /// <para>• p.Top,p.Right,p.Bottom,p.Left</para>
    static public FloatRect FromPadding(Padding p) { return new FloatRect(p.Left,p.Top,p.Right,p.Bottom); }
    
    /// <summary>
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    static public FloatRect ApplyPadding(FloatRect rect, Padding padding)
    {
      var rectangle = rect.Clone();
      rectangle.Location += new FloatPoint(padding.Left,padding.Top);
      rectangle.Size.X -= padding.Horizontal;
      rectangle.Size.Y -= padding.Vertical;
      return rectangle;
    }
    //
    public FloatRect() {}
    public FloatRect(float x, float y, float width, float height) { Location = new FloatPoint(x,y); Size = new FloatPoint(width,height); }
    public FloatRect(int x, int y, int width, int height) : this((float)x,(float)y,(float)width,(float)height) {}
    public FloatRect(FloatPoint L, FloatPoint S) : this(L.X,L.Y,S.X,S.Y) {}
    public FloatRect(Rectangle R) : this(R.X,R.Y,R.Width,R.Height) { }
    public FloatRect(float num) : this(num,num,num,num) { }
    public FloatRect(PointF Loc, SizeF Siz) : this(Loc.X,Loc.Y,Siz.Width,Siz.Height) {}
    
    
    
    public override bool Equals(object obj)
    {
      return obj.ToString()==ToString();
    }
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
    public override string ToString()
    {
      return string.Format("FloatRect: X:{0},Y:{1},Width:{2},Height:{3}", X,Y,Width,Height);
    }
  }
  #endregion
}
