using StudioMobile;
using UIKit;

namespace StudioMobile
{
	public class NavigationView<View> : CustomView where View : UIView, new()
	{
		[View]
		public NavigationBar NavBar { get; protected set; }

		[View]
		public View Body { get; protected set; }

		protected override void CreateView ()
		{
			base.CreateView ();
			BackgroundColor = UIColor.Clear;
		}

		public override void LayoutSubviews()
		{
			NavBar.Frame = this.LayoutBox ()
				.Top (0).Left (0).Right (0).Height (NavBar);
			Body.Frame = this.LayoutBox ()
				.Below (NavBar, 0).Left (0).Right (0).Bottom (0);
		}
	}
	
}
