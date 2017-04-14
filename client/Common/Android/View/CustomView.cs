using System;
using System.Collections.Generic;
using System.Drawing;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace StudioMobile
{
	public class CustomView : ViewGroup
	{
		List<object> builtObjects;

		public CustomView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
			Initialize();
		}

		public CustomView(Context context) : base(context)
		{
			Initialize();
		}

		public CustomView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			Initialize();
		}

		public CustomView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
			Initialize();
		}

		void Initialize()
		{
			CreateView();
		}

		protected virtual void CreateView()
		{
			builtObjects = ViewBuilder.Build(this);
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			OnLayout(changed, new Rectangle(l, t, r - l, b - t));
		}

		protected virtual void OnLayout(bool changed, Rectangle rect)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Cleanup.List(builtObjects);
			}
			base.Dispose(disposing);
		}
	}
}

