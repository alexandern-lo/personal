using StudioMobile;
using UIKit;

namespace StudioMobile
{
	public class NavigationBar : CustomView 
	{
		[View]
		public UIButton Left { get; private set; }
		[View]
		public UIButton Right { get; private set; }
		[View]
		public UILabel Title { get; private set; }

		UIView titleView;
		public UIView TitleView {
			get { 
				return titleView ?? Title;
			}
			set { 
				if (titleView != null)
					titleView.RemoveFromSuperview ();
				titleView = value;
				if (titleView != null) {
					Title.Hidden = true;
					AddSubview (titleView);
				} else {
					Title.Hidden = false;
				}
			}
		}
		
		protected override void CreateView ()
		{
			var width = UIScreen.MainScreen.ApplicationFrame.Width;
			var height = 44 + UIApplication.SharedApplication.StatusBarFrame.Height;
			Bounds = new CoreGraphics.CGRect (0, 0, width, height);
			base.CreateView ();
			LayoutMargins = new UIEdgeInsets (0, 0, 5, 0);
			Title.TextAlignment = UITextAlignment.Center;
		}

		public override void LayoutSubviews ()
		{
			var top = UIApplication.SharedApplication.StatusBarFrame.Height + LayoutMargins.Top;
			Left.SizeToFit ();
			Right.SizeToFit ();
			base.LayoutSubviews ();
			Left.Frame = this.LayoutBox ()
				.Left (LayoutMargins.Left).CenterVertically (top/2).Height (Left).Width (Left);
			Right.Frame = this.LayoutBox ()
				.Right (LayoutMargins.Right).CenterVertically (top/2).Height (Right).Width (Right);
			TitleView.Frame = this.LayoutBox ()
				.After (Left, 2).Before (Right, 2).Top (top).Bottom (LayoutMargins.Bottom);
		}
	}
}

