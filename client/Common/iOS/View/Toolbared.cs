using System;
using UIKit;

namespace StudioMobile
{
	public class Toolbared<T> : StackPanelView where T : UIView
	{
		T content;

		public Toolbared() : this(null)
		{
		}

		public Toolbared(T content)
		{
			Direction = LayoutDirection.BottomTop;
			Toolbar = new UIToolbar();
			Toolbar.SizeToFit();
			Content = content; // this also add Toolbar as last subview
		}

		public T Content
		{
			get { return content; }
			set
			{
				this.SetView(ref content, value, (o, s) => { s.SizeToFit(); o.AddSubview(s); });
				AddSubview(Toolbar);
			}
		}

		public UIToolbar Toolbar
		{
			get;
			private set;
		}
	}
	
}
