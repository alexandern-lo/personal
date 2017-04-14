using UIKit;
using StudioMobile;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Resources;

namespace LiveOakApp.iOS.View
{
	public class MenuView : CustomView
	{
		[View(0)]
		[CommonSkin("BackgroundImageView")]
		public UIImageView BackgroundImageView { get; private set; }

		[View(1)]
		[LabelSkin("LiveOakMenuLabel")]
		public UILabel MenuTitleLabel { get; private set; }

		[View(2)]
		public UITableView MenuTableView { get; private set; }

		[View(3)]
		public UIView ProfileInfoView { get; private set; }

		[View(4)]
		[LabelSkin("UserNameLabel")]
		public UILabel UserNameLabel { get; private set; }

		[View(5)]
		[CommonSkin("AvatarRemoteImageView")]
		public RemoteImageView AvatarImageView { get; private set; }

		[View(6)]
		[LabelSkin("ProfileInfoLabel")]
		public UILabel ProfileInfoLabel { get; private set; }

		[View(7)]
		public UIButton ViewProfileButton { get; private set; }

		[View(8)]
		[ButtonSkin("LogoutButton")]
		public UIButton LogoutButton { get; private set; }


		protected override void CreateView()
		{
			base.CreateView();

			MenuTableView.BackgroundColor = UIColor.Clear;
			MenuTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

			ProfileInfoView.BackgroundColor = Colors.MainGrayColor;

            MenuTitleLabel.Text = L10n.Localize("ApplicationTitle", "Avend");

            AvatarImageView.Placeholder = new Image(UIImage.FromBundle("user-default-image"));
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			BackgroundImageView.Frame = Bounds;

			MenuTitleLabel.SizeToFit();
			MenuTitleLabel.Frame = this.LayoutBox()
				.Height(MenuTitleLabel.Bounds.Height)
				.Top(25)
				.Left(18)
				.Right(10);

			ProfileInfoView.Frame = this.LayoutBox()
				.Height(50)
				.Left(0)
				.Right(0)
				.Bottom(0);

			ViewProfileButton.Frame = ProfileInfoView.Frame;

			LogoutButton.Frame = this.LayoutBox()
				.Height(30)
				.Width(30)
				.Right(10)
				.Bottom(10);

			MenuTableView.Frame = this.LayoutBox()
				.Left(0)
				.Right(0)
				.Below(MenuTitleLabel, 0)
				.Above(ProfileInfoView, 0);

			AvatarImageView.Frame = this.LayoutBox()
				.Width(30)
				.Height(30)
				.Left(15)
				.Bottom(10);

			UserNameLabel.SizeToFit();
			UserNameLabel.Frame = this.LayoutBox()
				.Height(UserNameLabel.Frame.Height)
				.After(AvatarImageView, 15)
				.Before(LogoutButton, 5)
				.Bottom(23);

			ProfileInfoLabel.SizeToFit();
			ProfileInfoLabel.Frame = this.LayoutBox()
				.Height(ProfileInfoLabel.Frame.Height)
				.After(AvatarImageView, 15)
				.Before(LogoutButton, 5)
				.Bottom(8);

		}
	}
}