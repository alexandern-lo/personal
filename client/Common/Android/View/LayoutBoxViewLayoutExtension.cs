using System;

namespace StudioMobile
{
	public static class LayoutBoxViewLayoutExtension
	{
		public static void Layout(this Android.Views.View view, LayoutBox box)
		{
			view.Layout((int)box.LayoutLeft, (int)box.LayoutTop, (int)box.LayoutRight, (int)box.LayoutBottom);
		}
	}
}

