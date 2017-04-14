using UIKit;
using StudioMobile;

namespace StudioMobile
{
	public class AccessoryButton : ClickableView
	{
		[View]
		public UILabel TitleLabel { get; private set; }
		[View]
		public UILabel AccessoryLabel { get; private set; }

		protected override void CreateView ()
		{
			base.CreateView ();
			LayoutMargins = new UIEdgeInsets (5, 0, 0, 0);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			TitleLabel.SizeToFit ();
			TitleLabel.Frame = this.LayoutBox ()
				.Left (LayoutMargins.Left).Width (TitleLabel).Height (TitleLabel).CenterVertically ();
			AccessoryLabel.SizeToFit ();
			AccessoryLabel.Frame = this.LayoutBox ()
				.Right (LayoutMargins.Right).Width (AccessoryLabel).Height (AccessoryLabel).CenterVertically ();
		}

		public void SetTitleText(string text)
		{
			TitleLabel.Text = text;
			SetNeedsLayout ();
		}
	}
	
}
