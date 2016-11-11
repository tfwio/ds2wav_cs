/* tfwxo * 1/19/2016 * 2:10 AM */
using System;
using System.Drawing;
#if USEFORMS
using System.Windows.Forms;
#endif

namespace System
{
  static public class xtant
  {
    #if USEFORMS
    #region Control Has Key and/or Mouse Automation

    static public bool HasCtl (this Control control)
    {
      return (Control.ModifierKeys & Keys.Control) == Keys.Control;
    }

    static public bool HasShiftKey (this Control c)
    {
      return HasMK (Keys.Shift);
    }

    static bool HasMK (Keys k)
    {
      return HasMK (null, k);
    }

    static bool HasMK (Control c, Keys k)
    {
      return (Keys)(int)(Control.ModifierKeys & k) == k;
    }
    static public bool Flagged(this Keys keys, Keys k)
    {
      return (Keys)(int)(keys & k) == k;
    }

    static public bool HasMouL (this Control c)
    {
      return HasMau (c, MouseButtons.Left);
    }

    static public bool HasMouR (this Control c)
    {
      return HasMau (c, MouseButtons.Right);
    }

    static public bool HasMouM (this Control c)
    {
      return HasMau (c, MouseButtons.Middle);
    }

    static bool HasMau (Control c, MouseButtons k)
    {
      return (MouseButtons)(Control.MouseButtons & k) == k;
    }

    #endregion
    #region Trackbar Translations (not used)

    static public void TranslateTxt (this TrackBar s, Label l, string f)
    {
      s.ValueChanged += (eo, es) => {
        l.Text = string.Format (f, s.Value);
      };
    }

    static public void TranslateDb (this TrackBar s, Label l)
    {
      s.ValueChanged += (eo, es) => {
        l.Text = s.Value.ToDB ();
      };
    }

    static public void TranslateDroop (this TrackBar s, Label l)
    {
      s.ValueChanged += (eo, es) => {
        l.Text = s.Value.ToDroop ();
      };
    }

    static public void TranslateSlope (this TrackBar s, Label l)
    {
      s.ValueChanged += (eo, es) => {
        l.Text = s.Value.ToSlope ();
      };
    }

    #endregion
    #region Windows.Forms DataBindings

    static public void BindText (this Control c, object o, string Key)
    {
      c.DataBindings.Clear ();
      c.DataBindings.Add ("Text", o, Key);
    }

    static public void BindVal (this Control c, object o, string Key)
    {
      c.DataBindings.Clear ();
      c.DataBindings.Add ("Value", o, Key);
    }

    //static public void BindCmb(this ComboBox c, object o, string Key)
    //{
    //  c.DataBindings.Clear();
    //  c.DataBindings.Add("SelectedValue", o, Key);
    //}

    static public void BindNdx (this Control c, object o, string Key)
    {
      c.DataBindings.Clear ();
      c.DataBindings.Add ("SelectedIndex", o, Key);
    }

    static public void BindCk (this Control c, object o, string Key)
    {
      c.DataBindings.Clear ();
      c.DataBindings.Add ("Checked", o, Key);
    }

    #endregion
    static public void TranslateNdx (this ComboBox s, Label l, Control ck1, Control ck2, Control ck3, Control ck4, params string[] range)
    {
      s.SelectedIndexChanged += (eo, es) => {
        l.Text = range [s.SelectedIndex];
        ck1.Enabled = ck2.Enabled = ck3.Enabled = ck4.Enabled = s.SelectedIndex < 3;
      };
    }
    #endif
    
    #region String Conversions

    static public string fsec (this float n)
    {
      //var mynum = n < 1 ? n * 1000 : n; // c#6
      // return n < 1 ? $"{mynum:N1} ms \u21E2" : $"{mynum:N1} s \u21E2";
      return string.Format (n < 1 ? "{0:N1} ms \u21E2" : "{0:N1} s \u21E2", n < 1 ? n * 1000 : n);
    }

    /// 32767ths -> dB
    static public string ToDB (this int value)
    {
      return ToDB ((float)value);
    }

    static public string ToDB (this float value)
    {
      if (value < 1) return "Mute";
      var v = 20.0d * Math.Log10 (Math.Pow (value, 2) / 32767f);
      return string.Format ("{0:n1} dB", v);
    }

    static public string ToDroop (this int v)
    {
      if (v <  2) return "Linear";
      if (v < 25) return "Slow";
      if (v < 66) return "Exp";
      return "Fast";
    }

    static public string ToSlope (this int v)
    {
      if (v < -80) return "Red";
      if (v < -20) return "Pink";
      if (v < 20) return "White";
      if (v < 80) return "Azure";
      return "Blue";
    }

    #endregion

    #region SAMPLE/PIXEL STRING TRANSLATIONS

    static public string Px2SpString (this FloatPoint point, FloatRect cli, FloatPoint tr, int PS, int Fs)
    {
      return Px2SpString ((PointF)point, cli, tr, PS, Fs);
    }

    static public string Px2SpString (this PointF point, FloatRect cli, FloatPoint tr, int Ps, int Fs)
    {
      var p = point.Px2Smp (cli, tr, Ps, Fs);
      return string.Format ("{0:N0} smp, {1:N1}", p.X, p.Y.ToDB ());
    }

    #endregion

    #region SAMPLE VS PIXEL TRANSLATIONS & TRANSFORMATIONS

    /// Ps is pixels per sample (second?)
    static public FloatPoint Smp2Px (this FloatPoint smp, FloatRect cli, FloatPoint tr, int Ps, int Fs = 44100, int yfactor = 100)
    {
      return smp
        .ZoomSmp(cli,tr,Ps,Fs,yfactor)
        .AddY(cli.Height)
        + cli.Location
        + tr;
    }
    static public FloatPoint OffsetPx (this FloatPoint px, FloatRect cli, FloatPoint tr)
    {
      return new FloatPoint (px.X - cli.X - tr.X, px.Y - cli.Height - cli.Top - tr.Y);
    }

    static public FloatPoint ZoomSmp (this FloatPoint px, FloatRect cli, FloatPoint tr, int Ps, int Fs = 44100, int yfactor = 100)
    {
      return new FloatPoint (
        px.X / Fs * Ps,
        px.Y / yfactor * cli.Height * -1);
    }
    /// px 2 smp
    static public FloatPoint ZoomPx (this FloatPoint px, FloatRect cli, FloatPoint tr, int Ps, int Fs = 44100, int yfactor = 100)
    {
      return new FloatPoint (
        px.X / Ps * Fs,
        px.Y / cli.Height * yfactor * -1);
    }

    static public FloatPoint Px2Smp (this PointF px, FloatRect cli, FloatPoint tr, int Ps, int Fs = 44100, int yfactor = 100)
    {
      return ((FloatPoint)px).Px2Smp (cli, tr, Ps, Fs, yfactor);
    }
    static public FloatPoint Px2Smp (this FloatPoint px, FloatRect cli, FloatPoint tr, int Ps, int Fs = 44100, int yfactor = 100)
    {
      return px.OffsetPx (cli, tr).ZoomPx (cli, tr, Ps, Fs, yfactor);
    }

    #endregion


    #region GRAPHICS PAINT HELPERS (RECT WITH OUTLINE)

    static public void Rect (this Graphics g, Brush b, Pen p, FloatPoint pt, int c)
    {
      g.FillRectangle (b, pt.X, pt.Y, c, c);
      g.DrawRectangle (p, pt.X, pt.Y, c, c);
    }

    static public void Rect (this Graphics g, Brush b, Pen p, FloatRect rect)
    {
      g.FillRectangle (b, rect);
      g.DrawRectangle (p, rect);
    }

    #endregion
  }
}
namespace System { using System.Reflection;
  
  static class ___x___
  {
    /// <summary>
    /// Uses reflection to get the field value from an object.
    /// </summary>
    /// <param name="instance">The instance object.</param>
    /// <param name="fieldName">The field's name which is to be fetched.</param>
    /// <returns>The field value from the object.</returns>
    //-  hacked from
    //-  zpt: http://stackoverflow.com/questions/3303126/how-to-get-the-value-of-private-field-in-c
    internal static int Field32(this object instance, string fieldName)
    {
      Type         type = instance.GetType();
      BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
      FieldInfo    field = type.GetField(fieldName, bindFlags);
      return (int) field.GetValue(instance);
    }
  }
}


