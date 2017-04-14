using System;
using CoreGraphics;
using Foundation;
using LiveOakApp.Resources;
using StudioMobile;
using StudioMobile.FontAwesome;
using UIKit;

namespace LiveOakApp.iOS.View.Skin
{
    public class ButtonSkinAttribute : DecoratorAttribute
    {
        public ButtonSkinAttribute(params string[] name) : base(typeof(ButtonSkin), name)
        {
        }
    }

    public static class ButtonSkin
    {
        public static void MenuButton(UIButton button)
        {
            button.SetTitleColor(UIColor.Blue, UIControlState.Normal);
            button.SetTitleColor(UIColor.Gray, UIControlState.Selected | UIControlState.Highlighted);
        }

        public static void FilterButton(UIButton button)
        {
            button.SetIcon(FontAwesome.Glass.Icon(20));
        }

        public static void LoginButton(UIButton button)
        {
            button.SetTitleColor(UIColor.White, UIControlState.Normal);
            button.SetTitleColor(UIColor.White.ColorWithAlpha(0.5f), UIControlState.Highlighted);
            button.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
            button.BackgroundColor = Colors.MainBlueColor;
            button.TitleLabel.Font = Fonts.xLargeBold;
            button.Layer.CornerRadius = 5.0f;
        }

        public static void AgreeButton(UIButton button)
        {
            button.Layer.CornerRadius = 5;

            button.SetTitleColor(UIColor.White.ColorWithAlpha(1.0f), UIControlState.Normal);
            button.SetTitleColor(UIColor.White.ColorWithAlpha(0.5f), UIControlState.Highlighted);
            button.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
            button.BackgroundColor = Colors.MainBlueColor;
            button.TitleLabel.Font = Fonts.LargeBold;
        }

        public static void DisagreeButton(UIButton button)
        {
            button.Layer.BorderWidth = 1;
            button.Layer.CornerRadius = 5;
            button.Layer.BorderColor = new UIColor(0.4f, 0.4f, 0.4f, 1.0f).CGColor;

            button.SetTitleColor(UIColor.White.ColorWithAlpha(1.0f), UIControlState.Normal);
            button.SetTitleColor(UIColor.White.ColorWithAlpha(0.5f), UIControlState.Highlighted);
            button.BackgroundColor = UIColor.Clear;
            button.TitleLabel.Font = Fonts.LargeBold;
        }

        public static void ForgotPasswordButton(UIButton button)
        {
            button.BackgroundColor = UIColor.Clear;
            button.Font = Fonts.NormalRegular;
            var underlineAttr = new UIStringAttributes { UnderlineStyle = NSUnderlineStyle.Single, ForegroundColor = Colors.LightGray };
            button.SetAttributedTitle(new NSAttributedString(L10n.Localize("ForgotPasswordButtonTitle", "Forgot password?"), underlineAttr), UIControlState.Normal);
        }

        public static void SmallWhiteTextButton(UIButton button)
        {
            button.SetTitleColor(UIColor.White, UIControlState.Normal);
            button.SetTitleColor(UIColor.White.ColorWithAlpha(0.5f), UIControlState.Highlighted);
            button.BackgroundColor = UIColor.Clear;
            button.TitleLabel.Font = Fonts.SmallRegular;
        }

        #region SubscriptionExpired

        public static void SubscriptionExpiredLogoutButton(UIButton button)
        {
            button.Layer.BorderWidth = 1;
            button.Layer.CornerRadius = 5;
            button.Layer.BorderColor = new UIColor(0.4f, 0.4f, 0.4f, 1.0f).CGColor;

            button.SetTitleColor(UIColor.White.ColorWithAlpha(1.0f), UIControlState.Normal);
            button.SetTitleColor(UIColor.White.ColorWithAlpha(0.5f), UIControlState.Highlighted);
            button.BackgroundColor = UIColor.Clear;
            button.TitleLabel.Font = Fonts.LargeBold;

            button.SetTitle(L10n.Localize("SubscriptionExpiredLogout", "Logout"), UIControlState.Normal);
        }

        public static void SubscriptionExpiredRecheckButton(UIButton button)
        {
            button.Layer.CornerRadius = 5;

            button.SetTitleColor(UIColor.White.ColorWithAlpha(1.0f), UIControlState.Normal);
            button.SetTitleColor(UIColor.White.ColorWithAlpha(0.5f), UIControlState.Highlighted);
            button.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
            button.BackgroundColor = Colors.MainBlueColor;
            button.TitleLabel.Font = Fonts.LargeBold;

            button.SetTitle(L10n.Localize("SubscriptionExpiredRecheck", "Check again"), UIControlState.Normal);
        }

        public static void SubscriptionRenewButton(UIButton button)
        {
            button.Layer.CornerRadius = 5;
            button.SetTitleColor(UIColor.White.ColorWithAlpha(1.0f), UIControlState.Normal);
            button.SetTitleColor(UIColor.White.ColorWithAlpha(0.5f), UIControlState.Highlighted);
            button.BackgroundColor = Colors.MainBlueColor;
            button.TitleLabel.Font = Fonts.NormalSemibold;
            button.SetTitle(L10n.Localize("RenewSubscriptionLabel", "Renew subscription"), UIControlState.Normal);
        }

        #endregion

        public static void SkipButton(UIButton button)
        {
            button.SetTitleColor(UIColor.White, UIControlState.Normal);
            button.SetTitleColor(UIColor.White.ColorWithAlpha(0.5f), UIControlState.Highlighted);
            button.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
            button.BackgroundColor = Colors.MainBlueColor;
            button.TitleLabel.Font = Fonts.xLargeBold;
            button.Layer.CornerRadius = 5.0f;
            button.SetTitle(L10n.Localize("SkipButton", "Skip"), UIControlState.Normal);
        }

        public static void LogoutButton(UIButton button)
        {
            button.SetImage(UIImage.FromBundle("menu_logout"), UIControlState.Normal);
        }

        public static void AddLeadButton(HighlightButton button)
        {
            button.TitleLabel.Text = String.Empty;
            button.SetImage(UIImage.FromBundle("icon_addlead"), UIControlState.Normal);
            button.ImageEdgeInsets = new UIEdgeInsets(14, 16, 12, 14);
            button.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            button.Layer.CornerRadius = 10.0f;
            button.NormalColor = Colors.DarkGray;
            button.PressedColor = Colors.DarkGray.ColorWithAlpha(0.5f);
        }

        public static void AddExpenseButton(UIButton button)
        {
            button.BackgroundColor = Colors.MainBlueColor;
            button.SetTitle(L10n.Localize("AddExpense", "+Add expense"), UIControlState.Normal);
            button.SetTitleColor(UIColor.White, UIControlState.Normal);
            button.Font = Fonts.SmallSemibold;
            button.Layer.CornerRadius = 5;
        }

        public static void EventDetailsWebLinkButton(UIButton button)
        {
            button.Font = Fonts.LargeRegular;
            button.SetTitleColor(UIColor.Black, UIControlState.Normal);
            button.BackgroundColor = UIColor.Clear;
            button.LineBreakMode = UILineBreakMode.TailTruncation;
        }

        public static void LeadCellChooseButton(UIButton button)
        {
            button.BackgroundColor = UIColor.Clear;
            button.TitleLabel.Font = Fonts.SmallSemibold;
            button.SetTitleColor(Colors.DarkGray, UIControlState.Normal);
        }

        public static void ResourcesButton(UIButton button)
        {
            button.SetTitleColor(UIColor.White.ColorWithAlpha(1.0f), UIControlState.Normal);
            button.SetTitleColor(UIColor.White.ColorWithAlpha(0.5f), UIControlState.Highlighted);
            button.BackgroundColor = Colors.MainBlueColor;
            button.TitleLabel.Font = Fonts.xLargeSemibold;
            button.Layer.CornerRadius = 5.0f;
            button.SetImage(UIImage.FromBundle("leads_sendresources_icon"), UIControlState.Normal);
            var title = L10n.Localize("SendResourcesButton", "Send Resources");
            button.SetTitle("  " + title, UIControlState.Normal);
        }

        public static void LeadSendResourcesButton(UIButton button)
        {
            ResourcesButton(button);
            button.TitleLabel.Font = Fonts.SmallSemibold;
        }

        public static void ReloadButton(UIButton button)
        {
            button.SetTitleColor(UIColor.White.ColorWithAlpha(1.0f), UIControlState.Normal);
            button.SetTitleColor(UIColor.White.ColorWithAlpha(0.5f), UIControlState.Highlighted);
            button.BackgroundColor = Colors.MainBlueColor;
            button.TitleLabel.Font = Fonts.LargeSemibold;
            button.Layer.CornerRadius = 5.0f;
            button.SetTitle(L10n.Localize("ReloadButton", "Reload"), UIControlState.Normal);
        }

        public static void AddAvatarButton(UIButton button)
        {
            button.SetTitle(L10n.Localize("AddPhotoButton", "  Add photo"), UIControlState.Normal);
            button.SetImage(UIImage.FromBundle("leads_plus_small"), UIControlState.Normal);
            button.BackgroundColor = Colors.MainBlueColor;
            button.Layer.CornerRadius = 5;
            button.ContentEdgeInsets = new UIEdgeInsets(5, 15, 5, 15);
            button.TitleLabel.Font = Fonts.SmallSemibold;
            button.SetTitleColor(UIColor.White, UIControlState.Normal);
        }

        public static void AddBusinessCardButton(UIButton button)
        {
            button.SetTitle("  " + L10n.Localize("Add", "Add"), UIControlState.Normal);
            button.SetImage(UIImage.FromBundle("leads_plus_small"), UIControlState.Normal);
            button.BackgroundColor = Colors.MainBlueColor;
            button.Layer.CornerRadius = 5;
            button.ContentEdgeInsets = new UIEdgeInsets(3.5f, 8, 3.5f, 8);
            button.TitleLabel.Font = Fonts.SmallSemibold;
            button.SetTitleColor(UIColor.White, UIControlState.Normal);
        }
        #region AttendeesList

        public static void PersonsListFilterButton(UIButton button)
        {
            button.BackgroundColor = Colors.DarkGray;
            button.SetImage(UIImage.FromBundle("attendees_filter"), UIControlState.Normal);
            button.Layer.CornerRadius = 3.0f;
        }

        public static void PersonsListEventButton(UIButton button)
        {
            button.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            button.TitleLabel.Font = Fonts.LargeSemibold;
            button.TitleLabel.LineBreakMode = UILineBreakMode.TailTruncation;
            button.SetTitleColor(Colors.DarkGray, UIControlState.Normal);
            button.SetTitleColor(Colors.DarkGray.ColorWithAlpha(0.5f), UIControlState.Highlighted);
        }

        public static void QuestionNumberButton(UIButton button)
        {
            button.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
            button.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            button.TitleLabel.Font = Fonts.xLargeRegular;
            button.BackgroundColor = UIColor.Clear;
            button.SetTitleColor(Colors.DarkGray, UIControlState.Normal);
            button.SetTitleColor(Colors.DarkGray.ColorWithAlpha(0.5f), UIControlState.Highlighted);
        }

        public static void NotesButton(UIButton button)
        {
            button.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
            button.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            button.SetImage(UIImage.FromBundle("qualify_notes"), UIControlState.Normal);
            button.BackgroundColor = UIColor.Clear;
        }

        #endregion

        #region AttendeesFilter

        public static void AttendeesFilterResetButton(UIButton button)
        {
            button.SetTitleColor(UIColor.White.ColorWithAlpha(1.0f), UIControlState.Normal);
            button.SetTitleColor(UIColor.White.ColorWithAlpha(0.5f), UIControlState.Highlighted);
            button.BackgroundColor = Colors.MainBlueColor;
            button.TitleLabel.Font = Fonts.xLargeSemibold;
        }

        #endregion
    }
}
