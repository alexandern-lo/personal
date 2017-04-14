using System;
using UIKit;
using CoreGraphics;
using System.Drawing;
using System.Linq;

namespace StudioMobile
{
	public class StackPanelView : UIView
	{
		StackLayout layout = new StackLayout();

		public StackPanelView ()
		{
		}

		public StackPanelView (Foundation.NSCoder coder) : base (coder)
		{
		}
		

		public StackPanelView (Foundation.NSObjectFlag t) : base (t)
		{
		}
		

		public StackPanelView (IntPtr handle) : base (handle)
		{
		}
		

		public StackPanelView (CGRect frame) : base (frame)
		{
		}

		public override void AddSubview(UIView view)
		{
			layout.Add (view);
			base.AddSubview (view);
		}

		public void AddSubview (UIView view, float size)
		{
			layout.Add (view, size);
			base.AddSubview (view);
		}

		public void AddSubview (UIView view, float size, UIEdgeInsets margins)
		{
			layout.Add (view, size, margins);
			base.AddSubview (view);
		}

		public void AddSubview (StackLayoutInfo info)
		{
			layout.Add (info);
			base.AddSubview (info.View);
		}

		public override void WillRemoveSubview (UIView uiview)
		{
			base.WillRemoveSubview (uiview);
			layout.Remove (uiview);
		}

		public override void LayoutSubviews ()
		{
			layout.Layout (this);
		}

		public LayoutDirection Direction {
			get { return layout.Direction; }
			set {
				layout.Direction = value;
				SetNeedsLayout ();
			}
		}

		public override CGSize SizeThatFits (CGSize size)
		{
			var bounds = new RectangleF (PointF.Empty, (SizeF)size);
			if (layout.Direction == LayoutDirection.TopBottom || layout.Direction == LayoutDirection.BottomTop) {
				var height = layout.Measure (bounds).DefaultIfEmpty().Max(p => p.Frame.Bottom);
				return new CGSize (size.Width, height);
			} else {
				var width = layout.Measure (bounds).DefaultIfEmpty().Max(p => p.Frame.Right);
				return new CGSize (width, size.Height);
			}
		}
	}
}

