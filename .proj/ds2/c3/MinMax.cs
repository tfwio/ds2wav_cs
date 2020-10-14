using System;
namespace System
{
  public class MinMax<T> where T:struct
	{
		virtual public T Minimum {
			get;
			set;
		}

		virtual public T Maximum {
			get;
			set;
		}

		virtual public T Value {
			get;
			set;
		}
	}
}








