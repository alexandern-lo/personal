using System;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Skin
{
	public class LabelSkinAttribute : DecoratorAttribute
	{
		public LabelSkinAttribute(params string[] name) : base(typeof(LabelSkin), name)
		{
		}
	}

	public static class LabelSkin
	{
		public static void ContentText(UILabel label)
		{
			label.BackgroundColor = UIColor.Red;
			label.Font = UIFont.BoldSystemFontOfSize(20);
		}

		public static void XSmallWhiteLabel(UILabel label)
		{
			label.Font = Fonts.xSmallRegular;
			label.TextColor = UIColor.White;
		}

		public static void SmallLabel(UILabel label)
		{
			label.Font = Fonts.SmallRegular;
		}

		public static void NormalWhiteLabel(UILabel label)
		{
			label.Font = Fonts.NormalRegular;
			label.TextColor = UIColor.White;
		}

        public static void NormalRegularBlackLabel(UILabel label)
        {
            label.Font = Fonts.NormalRegular;
            label.TextColor = UIColor.Black;
        }

		public static void LargeLightWhiteLabel(UILabel label)
		{
			label.Font = Fonts.LargeLight;
			label.TextColor = UIColor.White;
		}

        public static void LargeRegularBlackLabel(UILabel label)
        {
            label.Font = Fonts.LargeRegular;
            label.TextColor = UIColor.Black;
        }

        public static void LargeBoldBlackLabel(UILabel label)
        {
            label.Font = Fonts.LargeBold;
            label.TextColor = UIColor.Black;
        }

        public static void xxxLargeRegularBlackLabel(UILabel label)
        {
            label.Font = Fonts.xxxLargeRegular;
            label.TextColor = UIColor.Black;
        }

        public static void xLargeBoldBlackLabel(UILabel label)
        {
            label.Font = Fonts.xLargeBold;
            label.TextColor = UIColor.Black;
        }

		public static void NormalRegularWhiteLabel(UILabel label)
		{
			label.Font = Fonts.NormalRegular;
			label.TextColor = UIColor.White;
		}

        public static void SmallRegularGrayLabel(UILabel label)
        {
            label.Font = Fonts.SmallRegular;
            label.TextColor = Colors.GrayTextColor;
        }

        public static void SmallRegularBlackLabel(UILabel label)
        {
            label.Font = Fonts.SmallRegular;
            label.TextColor = UIColor.Black;
        }

		public static void xSmallRegularBlackLabel(UILabel label)
		{
			label.Font = Fonts.xSmallRegular;
			label.TextColor = UIColor.Black;
		}

        public static void xxSmallRegularBlackLabel(UILabel label)
        {
            label.Font = Fonts.xxSmallRegular;
            label.TextColor = UIColor.Black;
        }

        public static void xSmallBoldBlackLabel(UILabel label)
        {
            label.Font = Fonts.xSmallBold;
            label.TextColor = UIColor.Black;
        }

        public static void xxSmallBoldBlackLabel(UILabel label)
        {
            label.Font = Fonts.xxSmallBold;
            label.TextColor = UIColor.Black;
        }

        public static void xSmallRegularGrayLabel(UILabel label)
        {
            label.Font = Fonts.xSmallRegular;
            label.TextColor = Colors.GrayTextColor;
        }

        public static void xxSmallRegularGrayLabel(UILabel label)
        {
            label.Font = Fonts.xxSmallRegular;
            label.TextColor = Colors.GrayTextColor;
        }

		public static void NormalSemiboldBlackLabel(UILabel label)
		{
			label.Font = Fonts.NormalSemibold;
			label.TextColor = UIColor.Black;
		}

        public static void NormalRegularGrayLabel(UILabel label)
        {
            label.Font = Fonts.NormalRegular;
            label.TextColor = Colors.GrayTextColor;
        }

        public static void NormalSemiboldGrayLabel(UILabel label)
        {
            label.Font = Fonts.NormalSemibold;
            label.TextColor = Colors.GrayTextColor;
        }

        public static void SmallBoldBlackLabel(UILabel label)
        {
            label.Font = Fonts.SmallBold;
            label.TextColor = UIColor.Black;
        }

        public static void LiveOakMenuLabel(UILabel label)
        {
            label.TextColor = new UIColor(0.365f, 0.624f, 0.988f, 1.0f);
            label.Font = UIFont.FromName("AvenirNext-Bold", 26f);
        }

        public static void UserNameLabel(UILabel label)
        {
            label.TextColor = UIColor.White;
            label.Font = Fonts.NormalRegular;
        }

        public static void ProfileInfoLabel(UILabel label)
        {
            label.TextColor = UIColor.White.ColorWithAlpha(0.4f);
            label.Font = Fonts.xSmallRegular;
            label.Text = L10n.Localize("MenuProfileHint", "View and edit your profile");
        }

        public static void EventCellTitle(UILabel label)
        {
            label.TextColor = UIColor.Black;
            label.Font = Fonts.NormalRegular;
        }

        public static void EventDetailsTypeLabel(UILabel label)
        {
            label.TextAlignment = UITextAlignment.Center;
            label.Font = Fonts.xLargeRegular;
            label.TextColor = UIColor.Black;
            label.LineBreakMode = UILineBreakMode.TailTruncation;
        }

        public static void EventDetailsIndustryLabel(UILabel label)
        {
            label.TextAlignment = UITextAlignment.Center;
            label.Font = Fonts.NormalRegular;
            label.TextColor = UIColor.Black;
            label.LineBreakMode = UILineBreakMode.TailTruncation;
        }

        public static void EventDetailsRecurringLabel(UILabel label)
        {
            label.TextAlignment = UITextAlignment.Center;
            label.Font = Fonts.NormalRegular;
            label.TextColor = Colors.GrayTextColor;
        }

        public static void LeadRecentActivityEventLabel(UILabel label)
        {
            label.Font = Fonts.xSmallRegular;
            label.TextColor = new UIColor(0.5f, 0.5f, 0.5f, 1.0f);
        }

        public static void LeadRecentActivityEventTitle(UILabel label)
        {
            label.Font = Fonts.xSmallBold;
            label.TextColor = new UIColor(0.5f, 0.5f, 0.5f, 1.0f);
        }

		#region PersonCell
		public static void PersonCellFullNameLabel(UILabel label)
		{
			label.Font = Fonts.NormalRegular;
            label.TextColor = UIColor.Black;
		}

		public static void PersonCellCompanyLabel(UILabel label)
		{
			label.Font = Fonts.xSmallRegular;
			label.TextColor = new UIColor(0.5f, 0.5f, 0.5f, 1.0f);
		}
		#endregion

		#region AttendeeDetails
		public static void AttendeeDetailsFullNameLabel(UILabel label)
		{
			label.Font = Fonts.xLargeBold;
			label.TextColor = Colors.DarkGray;
			label.TextAlignment = UITextAlignment.Center;
		}

		public static void AttendeeDetailsPositionLabel(UILabel label)
		{
			label.Font = Fonts.NormalRegular;
			label.TextColor = Colors.DarkGray;
			label.TextAlignment = UITextAlignment.Center;
		}

		public static void AttendeeDetailsCompanyLabel(UILabel label)
		{
			label.Font = Fonts.NormalRegular;
			label.TextColor = new UIColor(0.6f, 0.6f, 0.6f, 1.0f);
			label.TextAlignment = UITextAlignment.Center;
		}
        #endregion

        #region LeadDetails

        public static void LeadEmailLabel(UILabel label)
        {
            label.Text = L10n.Localize("LeadEmailLabel", "E-mail");
            label.Font = Fonts.xSmallRegular;
            label.TextColor = new UIColor(0.6f, 0.6f, 0.6f, 1.0f);
            label.TextAlignment = UITextAlignment.Center;
        }

        public static void LeadPhoneLabel(UILabel label)
        {
            label.Text = L10n.Localize("LeadPhoneLabel","Phone Number");
            label.Font = Fonts.xSmallRegular;
            label.TextColor = new UIColor(0.6f, 0.6f, 0.6f, 1.0f);
            label.TextAlignment = UITextAlignment.Center;
        }

        public static void NotesLabel(UILabel label)
        {
            label.Text = L10n.Localize("LeadNotesLabel", "My notes");
            label.Font = Fonts.xSmallRegular;
            label.TextColor = new UIColor(0.6f, 0.6f, 0.6f, 1.0f);
            label.TextAlignment = UITextAlignment.Center;
        }

        public static void LocationTitleLabel(UILabel label)
        {
            label.Text = L10n.Localize("LeadEntryLocationLabel", "First Entry Location");
            label.Font = Fonts.xSmallRegular;
            label.TextColor = new UIColor(0.6f, 0.6f, 0.6f, 1.0f);
            label.TextAlignment = UITextAlignment.Center;
        }

        public static void LocationLabel(UILabel label)
        {
            label.Font = Fonts.xLargeRegular;
            label.TextColor = new UIColor(0.6f, 0.6f, 0.6f, 1.0f);
            label.TextAlignment = UITextAlignment.Center;
            label.Lines = 2;
        }
            
        #endregion

        #region EventAgenda

        public static void AgendaItemDescriptionLabel(UILabel label)
        {
            label.Font = Fonts.SmallRegular;
            label.TextColor = Colors.GrayTextColor;
            label.TextAlignment = UITextAlignment.Left;
            label.LineBreakMode = UILineBreakMode.TailTruncation;
            label.Lines = 0;
        }

        public static void AgendaItemTitleLabel(UILabel label)
        {
            label.Font = Fonts.NormalSemibold;
            label.TextColor = UIColor.Black;
            label.TextAlignment = UITextAlignment.Left;
        }

        public static void AgendaItemTimeLabel(UILabel label)
        {
            label.Font = Fonts.SmallSemibold;
            label.TextColor = Colors.MainBlueColor;
            label.TextAlignment = UITextAlignment.Left;
        }

        public static void AgendaLocationLabel(UILabel label)
        {
            label.Font = Fonts.SmallLight;
            label.TextColor = UIColor.Black;
            label.TextAlignment = UITextAlignment.Right;
        }

        public static void AgendaDateLabel(UILabel label)
        {
            label.Font = Fonts.NormalLight;
            label.TextColor = UIColor.White;
            label.TextAlignment = UITextAlignment.Center;
        }

        #endregion
    }
}