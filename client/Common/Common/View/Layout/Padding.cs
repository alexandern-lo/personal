using System;

namespace StudioMobile
{
	public struct Padding
	{
		public float Left;
		public float Top;
		public float Right;
		public float Bottom;

		public Padding(float l, float t, float r, float b)
		{
			Left = l;
			Top = t;
			Right = r;
			Bottom = b;
		}

		public static readonly Padding Empty = new Padding(0, 0, 0, 0);

		#if __IOS__
		public static implicit operator UIKit.UIEdgeInsets (Padding p)
		{
			return new UIKit.UIEdgeInsets (p.Top, p.Left, p.Bottom, p.Right);
		}
		#endif
	}
}

