using System;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View
{
	public class TermsView : CustomView
	{
		[View(0)]
        [CommonSkin("BackgroundImageView")]
		public UIImageView BackgroundImage { get; private set; }

		[View(1)]
		[ButtonSkin("AgreeButton")]
		public UIButton AgreeButton { get; private set; }

		[View(2)]
		[ButtonSkin("DisagreeButton")]
		public UIButton DisagreeButton { get; private set; }

		[View(3)]
		[CommonSkin("SmallWhiteTextView")]
		public UITextView TermsTextView { get; private set; }

        [View(4)]
        [CommonSkin("ErrorLabel")]
        public UILabel ErrorLabel { get; private set; }

        [View(5)]
        public CustomErrorView ErrorView { get; private set; }

        [View(6)]
        public ActivityDialogView ActivityView { get; private set; }

        public String ErrorMessageText
        {
            set
            {
                ErrorView.ErrorMessageLabel.Text = value;
                SetNeedsLayout();
            }
        }

		protected override void CreateView()
		{
			base.CreateView();
			TermsTextView.TextColor = UIColor.White;

			AgreeButton.SetTitle(L10n.Localize("TermsAccept", "Accept"), UIControlState.Normal);
			DisagreeButton.SetTitle(L10n.Localize("TermsDecline", "Decline"), UIControlState.Normal);
			TermsTextView.Editable = false;
			TermsTextView.BackgroundColor = UIColor.Clear;
            ErrorView.BackgroundColor = Colors.ErrorBackgroundColorDark;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			var margin = 20;
			var buttonSpacing = 5;
			var buttonWidth = this.Bounds.Width / 2 - margin - buttonSpacing;


			BackgroundImage.Frame = Bounds;

			AgreeButton.Frame = this.LayoutBox()
				.Height(50)
				.Width(buttonWidth)
				.Bottom(margin)
				.Right(margin);
			
			DisagreeButton.Frame = this.LayoutBox()
				.Height(50)
				.Width(buttonWidth)
				.Bottom(margin)
				.Left(margin);

            ErrorLabel.Frame = this.LayoutBox()
                .Height(20)
                .Width(this.Bounds.Width)
                .Above(AgreeButton, 10);

			TermsTextView.Frame = this.LayoutBox()
				.Top(55)
				.Left(margin)
				.Right(margin)
				.Above(AgreeButton, 40);

            ErrorView.SizeToFit();
            ErrorView.Frame = this.LayoutBox()
                .Height(ErrorView.Bounds.Height)
                .CenterVertically(-30)
                .Left(15)
                .Right(15);

            ActivityView.Frame = this.Bounds;
		}

        bool loadTermsRunning;
        public bool LoadTermsRunning
        {
            get { return loadTermsRunning; }
            set {
                loadTermsRunning = value;
                RefreshActivityIndicator();
            }
        }

        bool acceptRunning;
        public bool AcceptRunning
        {
            get { return acceptRunning; }
            set {
                acceptRunning = value;
                RefreshActivityIndicator();
            }
        }

        void RefreshActivityIndicator()
        {
            var showActivity = loadTermsRunning || acceptRunning;
            ActivityView.Hidden = !showActivity;
        }
	}
}

