using System;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View
{
	public class ForgotPasswordView : CustomView
	{
		[View]
		public UIWebView WebView { get; private set; }

		protected override void CreateView()
		{
			base.CreateView();
			WebView.LoadHtmlString("<html><head></head><body><H1 align='center'>This page will be implemented soon.</H1></body></html>", 
			                       new Foundation.NSUrl("http://vk.com"));
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			WebView.Frame = this.LayoutBox().Left(0).Right(0).Top(0).Bottom(0);
		}
	}
}

