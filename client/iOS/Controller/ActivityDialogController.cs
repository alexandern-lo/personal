using System;
using CoreAnimation;
using LiveOakApp.iOS.View;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.Controller
{
	public class ActivityDialogController : CustomController <ActivityDialogView>
	{
		public ActivityDialogController()
		{
			Setup(L10n.Localize("ActivityDialogTitle", "Loading..."));
		}

		public ActivityDialogController(string title)
		{
			Setup(title);
		}

		public void Setup(string title)
		{
			ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
			base.Title = title;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			View.TitleLabel.Text = this.Title;
		}

		public override string Title
		{
			get
			{
				return base.Title;
			}
			set
			{
				base.Title = value;
				if (View != null && View.TitleLabel != null)
				{
					View.TitleLabel.Text = value;
				}
			}
		}

		public ActivityDialogController Present(UIViewController parent, bool animated, Action completion = null)
		{
			if (!parent.IsViewLoaded || parent.View.Superview == null)
			{
				throw new Exception("The fromViewController view must reside in the container view upon initializing the transition context.");
			}
			if (animated)
			{
				SetupAnimation(parent);
			}
			parent.PresentViewController(this, false, completion);
			return this;
		}

		public ActivityDialogController Dismiss(bool animated, Action completion = null)
		{
			if (animated)
			{ 
				SetupAnimation(PresentingViewController);
			}
			PresentingViewController.DismissViewController(false, completion);
			return this;
		}

		private void SetupAnimation(UIViewController parent)
		{
			CATransition transition = CATransition.CreateAnimation();
			transition.Duration = 0.25f;
			transition.Type = CATransition.TransitionFade;
			parent.View.Window?.Layer.AddAnimation(transition, CALayer.Transition);
		}
	}
}

