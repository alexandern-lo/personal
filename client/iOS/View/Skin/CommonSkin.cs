using CoreGraphics;
using UIKit;
using StudioMobile;
using StudioMobile.FontAwesome;
using LiveOakApp.iOS.View.TableHeaders;
using LiveOakApp.Resources;
using Foundation;

namespace LiveOakApp.iOS.View.Skin
{
    public class CommonSkinAttribute : DecoratorAttribute
    {
        public CommonSkinAttribute(params string[] name) : base(typeof(CommonSkin), name)
        {
        }
    }

    public static class CommonSkin
    {
        public static void SetupAppearance()
        {
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
            var barButtonItem = UIBarButtonItem.AppearanceWhenContainedIn(typeof(CustomController<>));
            barButtonItem.TintColor = UIColor.White;
            var navBar = UINavigationBar.Appearance;
            navBar.TintColor = UIColor.White;
            navBar.BarTintColor = Colors.MainGrayColor;
            navBar.Translucent = false;
            navBar.ShadowImage = new UIImage();
            navBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            navBar.TitleTextAttributes = new UIStringAttributes()
            {
                ForegroundColor = UIColor.White,
                Font = Fonts.xLargeSemibold
            };
        }

        public static void DefaultSwitch(UISwitch switchView)
        {
            var offColor = new UIColor(0.855f, 0.855f, 0.855f, 1f);
            switchView.BackgroundColor = offColor;
            switchView.TintColor = offColor;
            switchView.Layer.CornerRadius = 16.0f;
            switchView.OnTintColor = new UIColor(0.353f, 0.6157f, 1f, 1f);
        }

        public static void SmallWhiteTextView(UITextView textView)
        {
            textView.Font = Fonts.SmallRegular;
            textView.TextColor = UIColor.White;
        }

        public static void BackgroundImageView(UIImageView imageView)
        {
            imageView.Image = UIImage.FromBundle("background");
            imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
        }

        public static void TimeIconImageView(UIImageView imageView)
        {
            imageView.Image = UIImage.FromBundle("edetails_dates");
            imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
        }

        public static void LocationIconImageView(UIImageView imageView)
        {
            imageView.Image = UIImage.FromBundle("edetails_location");
            imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
        }

        public static void SubscriptionExpiredLabel(UILabel label)
        {
            label.Font = Fonts.xxLargeRegular;
            label.TextColor = Colors.LightGray;
            label.TextAlignment = UITextAlignment.Center;

            label.Text = L10n.Localize("SubscriptionExpiredLabel", "Subscription Expired");
        }

        public static void ErrorLabel(UILabel label)
        {
            label.Font = Fonts.NormalRegular;
            label.BackgroundColor = UIColor.Clear;
            label.TextColor = UIColor.Red;
            label.TextAlignment = UITextAlignment.Center;
        }

        public static void ErrorLabel(UITextView label)
        {
            label.Font = Fonts.LargeRegular;
            label.BackgroundColor = UIColor.Clear;
            label.TextColor = UIColor.Red;
            label.TextAlignment = UITextAlignment.Center;
            label.Editable = false;
            label.Selectable = true;
            label.WeakLinkTextAttributes = new NSDictionary(UIStringAttributeKey.ForegroundColor, UIColor.Red, 
                                                            UIStringAttributeKey.UnderlineStyle, NSUnderlineStyle.Single); 
        }

        public static void MessageLabel(UILabel label)
        {
            label.Font = Fonts.NormalRegular;
            label.TextColor = Colors.LightGray;
            label.TextAlignment = UITextAlignment.Center;
        }

        public static void AspectFitRemoteImageView(RemoteImageView remoteImageView)
        {
            remoteImageView.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            remoteImageView.BackgroundColor = UIColor.Clear;
        }

        public static void AvatarRemoteImageView(RemoteImageView remoteImageView)
        {
            var imageView = remoteImageView.ImageView;
            imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            remoteImageView.Layer.CornerRadius = 15;
            remoteImageView.Layer.MasksToBounds = true;
            remoteImageView.BackgroundColor = UIColor.Clear;
        }

        public static UIBarButtonItem MenuBarButtonItem
        {
            get
            {
                var menuBarButtonItem = new UIBarButtonItem();
                menuBarButtonItem.SetIcon(FontAwesome.Bars.Icon(25), UIColor.White);
                return menuBarButtonItem;
            }
        }

        public static UIBarButtonItem BackBarButtonItem
        {
            get
            {
                return new UIBarButtonItem(L10n.Localize("Back", "Back"), UIBarButtonItemStyle.Plain, null);
            }
        }

        #region Startup

        public static void StartupProgressLabel(UILabel label)
        {
            label.Font = Fonts.NormalLight;
            label.TextColor = UIColor.White;
            label.TextAlignment = UITextAlignment.Center;
        }

        #endregion

        #region Event Details
        public static void EventAttendeesCellView(CellView cellView)
        {
            DefaultCellViewSettings(cellView);
            cellView.SetTitle(L10n.Localize("EventAttendeesButtonTitle", "Event Attendees"), UIControlState.Normal);
            cellView.LeftIconImageView.Image = UIImage.FromBundle("edetails_attendees");
        }

        public static void EventAgendaCellView(CellView cellView)
        {
            DefaultCellViewSettings(cellView);
            cellView.SetTitle(L10n.Localize("EventAgendaButtonTitle", "Event Agenda"), UIControlState.Normal);
            cellView.LeftIconImageView.Image = UIImage.FromBundle("edetails_agenda");
        }

        public static void DefaultCellViewSettings(CellView cellView)
        {
            cellView.PressedColor = UIColor.Black.ColorWithAlpha(0.2f);
            cellView.RightIconImageView.Image = UIImage.FromBundle("right-arrow-icon"); // TODO: add svg image
            cellView.AddFixedWidth(CellViewWidthType.SeparatorMultiplier, 1f - 0.04f);
            cellView.AddFixedWidth(CellViewWidthType.LeftSegmentMultiplier, 0.176f);
            cellView.AddFixedWidth(CellViewWidthType.RightSegmentMultiplier, 0.09f);
            cellView.TitleLabelGravity = CellViewGravity.Left;
            cellView.SeparatorGravity = CellViewGravity.TopRight;
            cellView.TitleLabel.Font = Fonts.NormalRegular;
            cellView.EnableLeftIcon = true;
            cellView.EnableRightIcon = true;
            cellView.EnableSeparator = true;
        }

        public static void EventDetailsTimeCellView(CellView cellView)
        {
            cellView.TitleLabel.Font = Fonts.xLargeRegular;
            cellView.LeftIconSize = new CGSize(20f, 20f);
            cellView.LeftImageMargin = new UIEdgeInsets(0, 0, 0, 10f);
            cellView.LeftIconImageView.Image = UIImage.FromBundle("edetails_dates");
            cellView.TitleLabelGravity = CellViewGravity.Left;
        }

        public static void EventDetailsLocationCellView(CellView cellView)
        {
            cellView.TitleLabel.Font = Fonts.NormalRegular;
            cellView.LeftIconSize = new CGSize(20f, 20f);
            cellView.LeftImageMargin = new UIEdgeInsets(0, 0, 0, 10f);
            cellView.LeftIconImageView.Image = UIImage.FromBundle("edetails_location");
            cellView.TitleLabelGravity = CellViewGravity.Left;
        }

        public static void HappeningNowSectionHeaderView(SectionHeaderView headerView)
        {
            headerView.BackgroundColor = new UIColor(0.365f, 0.624f, 0.988f, 1.0f);
            headerView.TextLabel.TextColor = UIColor.White;
        }

        public static void RecentSectionHeaderView(SectionHeaderView headerView)
        {
            headerView.SeparatorView.Hidden = false;
        }
        #endregion

        #region AttendeesList
        public static void ManualEntryTabBarButton(CustomTabBarButton tabBarButton)
        {
            tabBarButton.SetTitle(L10n.Localize("ButtonManualEntry", "Manual entry"), UIControlState.Normal);
            tabBarButton.SetImage(UIImage.FromBundle("icon_addlead"), UIControlState.Normal);
            tabBarButton.TitleLabel.Font = Fonts.xxSmallRegular;
            tabBarButton.EnableSeparator = false;
        }

        public static void BarScanTabBarButton(CustomTabBarButton tabBarButton)
        {
            tabBarButton.SetTitle(L10n.Localize("ButtonQrBarScan", "QR / Bar scan"), UIControlState.Normal);
            tabBarButton.SetImage(UIImage.FromBundle("leads_qr"), UIControlState.Normal);
            tabBarButton.TitleLabel.Font = Fonts.xxSmallRegular;
            tabBarButton.EnableSeparator = true;
            tabBarButton.SeparatorGravity = CustomTabBarButton.SeparatorGravityType.Right;
        }

        public static void CardScanTabBarButton(CustomTabBarButton tabBarButton)
        {
            tabBarButton.SetTitle(L10n.Localize("ButtonBusinessCardScan", "Business card scan"), UIControlState.Normal);
            tabBarButton.SetImage(UIImage.FromBundle("leads_businesscard"), UIControlState.Normal);
            tabBarButton.TitleLabel.Font = Fonts.xxSmallRegular;
            tabBarButton.EnableSeparator = true;
            tabBarButton.SeparatorGravity = CustomTabBarButton.SeparatorGravityType.Right;
        }

        public static void BottomPanelBackgroundView(UIView backgroundView)
        {
            backgroundView.BackgroundColor = Colors.MainGrayColor;
        }

        public static void EventPanelBackgroundView(UIView backgroundView)
        {
            backgroundView.BackgroundColor = new UIColor(0.95f, 0.95f, 0.95f, 1f);
        }

        public static void SearchTextFieldBackgroundView(UIView backgroundView)
        {
            backgroundView.BackgroundColor = new UIColor(0.95f, 0.95f, 0.95f, 1f);
            backgroundView.Layer.CornerRadius = 3.0f;
        }

        public static void PersonsListSearchIcon(UIImageView icon)
        {
            icon.BackgroundColor = UIColor.Clear;
            icon.Image = UIImage.FromBundle("leads_search");
        }

        public static void PersonsListSearchTextField(UITextField textField)
        {
            textField.BackgroundColor = UIColor.Clear;
            textField.Font = Fonts.NormalLight;
        }

        public static void SelectEventIconImageView(UIImageView imageView)
        {
            imageView.Image = UIImage.FromBundle("arrow_down");
        }
        #endregion

        #region AttendeeDetails

        #endregion

        #region LeadView

        public static void topBarContactButton(CustomTopBarButton topBarButton)
        {
            topBarButton.SetTitle(L10n.Localize("TabContact", "Contact"), UIControlState.Normal);
            topBarButton.ImageViewSize = CGSize.Empty;
            topBarButton.TitleLabel.Font = Fonts.LargeRegular;
            topBarButton.EnableSeparator = true;
            topBarButton.BackgroundColor = Colors.MainGrayColor;
        }

        public static void topBarQualifyButton(CustomTopBarButton topBarButton)
        {
            topBarButton.SetTitle(L10n.Localize("TabQualify", "Qualify"), UIControlState.Normal);
            topBarButton.ImageViewSize = CGSize.Empty;
            topBarButton.TitleLabel.Font = Fonts.LargeRegular;
            topBarButton.EnableSeparator = false;
            topBarButton.BackgroundColor = Colors.MainGrayColor;
        }

        public static void LeadInfoInput(KeyValueInfoInput infoInput)
        {
            infoInput.ValueTextField.Font = Fonts.LargeRegular;
            infoInput.ValueTextField.ReturnKeyType = UIReturnKeyType.Next;
        }

        public static void QualifyCurrentQuestionBackground(UIView view)
        {
            view.BackgroundColor = UIColor.White;
        }

        public static void QualifyQuestionTextView(UITextView textView)
        {
            textView.Font = Fonts.NormalRegular;
            textView.ScrollEnabled = true;
            textView.Editable = false;
            textView.BackgroundColor = UIColor.White;
            textView.TextContainerInset = new UIEdgeInsets(12, 12, 12, 12);
            textView.Layer.CornerRadius = 6;
        }

        public static void QualifyNotesTextView(UITextView textView)
        {
            textView.Font = Fonts.NormalRegular;
            textView.ScrollEnabled = true;
            textView.BackgroundColor = UIColor.White;
            textView.TextContainerInset = new UIEdgeInsets(12, 12, 12, 12);
            textView.Layer.CornerRadius = 6;
        }

        #endregion

        #region Resources list


        #endregion
    }
}

