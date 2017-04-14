using System;
using LiveOakApp.iOS.View;
using SL4N;
using StudioMobile;

namespace LiveOakApp.iOS.Controller
{
	public class ForgotPasswordController : CustomController <ForgotPasswordView>
	{
		public EventHandler closeButtonPressed { get; set; }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.Title = "Find your account";
			this.NavigationItem.RightBarButtonItem = new UIKit.UIBarButtonItem("Close", UIKit.UIBarButtonItemStyle.Plain, closeButtonPressed);
		}
	}
}

