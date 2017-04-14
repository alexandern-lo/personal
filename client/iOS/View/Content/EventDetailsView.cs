using System;
using Foundation;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Content
{
    public class EventDetailsView : CustomView
    {
        [View]
        [CommonSkin("AspectFitRemoteImageView")]
        public RemoteImageView EventIconRemoteImageView { get; private set; }

        [View]
        [LabelSkin("EventDetailsTypeLabel")]
        public UILabel EventTypeLabel { get; private set; }

        [View]
        [LabelSkin("EventDetailsIndustryLabel")]
        public UILabel EventIndustryLabel { get; private set; }

        [View]
        [ButtonSkin("EventDetailsWebLinkButton")]
        public UIButton WebSiteButton { get; private set; }

        [View]
        [CommonSkin("EventDetailsTimeCellView")]
        public CellView TimeCellView { get; private set; }

        [View]
        [LabelSkin("EventDetailsRecurringLabel")]
        public UILabel EventRecurringLabel { get; private set; }

        [View]
        [CommonSkin("EventDetailsLocationCellView")]
        public CellView LocationCellView { get; private set; }

        [View]
        public UIView ExpensesSeparatorView { get; private set; }

        [View]
        [LabelSkin("NormalRegularBlackLabel")]
        public UILabel ExpensesTitleLabel { get; private set; }

        [View]
        [LabelSkin("xLargeBoldBlackLabel")]
        public UILabel ExpensesLabel { get; private set; }

        [View]
        [ButtonSkin("AddExpenseButton")]
        public UIButton AddExpenseButton { get; private set; }

        [View]
        [CommonSkin("EventAttendeesCellView")]
        public CellView EventAttendeesCellView { get; private set; }

        [View]
        [CommonSkin("EventAgendaCellView")]
        public CellView EventAgendaCellView { get; private set; }

        protected override void CreateView()
        {
            base.CreateView();
            EventIconRemoteImageView.Placeholder = UIImage.FromBundle("edetails_icon");
            LocationCellView.LineBreakMode = UILineBreakMode.TailTruncation | UILineBreakMode.WordWrap;
            LocationCellView.TitleLabel.Lines = 2;

            ExpensesSeparatorView.BackgroundColor = Colors.GraySeparatorColor;
            ExpensesTitleLabel.Text = L10n.Localize("TotalExpensesToDate", "Total expenses to date:");
            EventRecurringLabel.Text = "Recurring";
        }

        public void SetWebLinkUrl(string url)
        {
            if (url == null)
            {
                WebSiteButton.SetTitle(null, UIControlState.Normal);
                WebSiteButton.SetTitle(null, UIControlState.Highlighted);
                return;
            }
            var attributeNormal = new NSAttributedString(url, new UIStringAttributes { UnderlineStyle = NSUnderlineStyle.Single });
            var attributeHighlight = new NSAttributedString(url, new UIStringAttributes { UnderlineStyle = NSUnderlineStyle.Single, ForegroundColor = UIColor.Black.ColorWithAlpha(0.5f) });
            WebSiteButton.SetAttributedTitle(attributeNormal, UIControlState.Normal);
            WebSiteButton.SetAttributedTitle(attributeHighlight, UIControlState.Highlighted);
        }

        public void SetAgendaAndAttendeesButtonHidden(bool isHidden)
        {
            EventAgendaCellView.Hidden = isHidden;
            EventAttendeesCellView.Hidden = isHidden;
            SetNeedsLayout();
        }

        public void SetRecurringLabelHidden(bool isHidden)
        {
            EventRecurringLabel.Hidden = isHidden;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var pH = Bounds.Height;
            var pW = Bounds.Width;
            var maxContentWidth = pW - 10.0f;

            EventTypeLabel.SizeToFit();
            EventIndustryLabel.SizeToFit();
            EventRecurringLabel.SizeToFit();
            WebSiteButton.SizeToFit();
            ExpensesTitleLabel.SizeToFit();
            ExpensesLabel.SizeToFit();
            AddExpenseButton.SizeToFit();

            EventIconRemoteImageView.Frame = this.LayoutBox()
                .Height(90)
                .Width(90)
                .CenterHorizontally()
                .Top(pH * 0.07f);

            EventTypeLabel.Frame = this.LayoutBox()
                .Height(EventTypeLabel.Font.LineHeight)
                .Left(5.0f)
                .Right(5.0f)
                .Below(EventIconRemoteImageView, pH * 0.025f);

            EventIndustryLabel.Frame = this.LayoutBox()
                .Height(EventIndustryLabel.Font.LineHeight)
                .Left(5.0f)
                .Right(5.0f)
                .Below(EventTypeLabel, pH * 0.001f);

            var timeCellWidth = TimeCellView.MinWidthForHeight(30f);
            TimeCellView.Frame = this.LayoutBox()
                .Width((timeCellWidth < maxContentWidth) ? timeCellWidth : maxContentWidth)
                .Height(30f)
                .Below(EventIndustryLabel, pH * 0.02f)
                .CenterHorizontally();

            EventRecurringLabel.Frame = this.LayoutBox()
                .Height(EventRecurringLabel.Bounds.Height)
                .Width(EventRecurringLabel.Bounds.Width)
                .CenterHorizontally()
                .Below(TimeCellView, pH * 0.001f);

            var locationCellWidth = LocationCellView.MinWidthForHeight(44f);
            LocationCellView.Frame = this.LayoutBox()
                .Width((locationCellWidth < maxContentWidth) ? locationCellWidth : maxContentWidth)
                .Height(44f)
                .Below(EventRecurringLabel, pH * 0.002f)
                .CenterHorizontally();

            WebSiteButton.Frame = this.LayoutBox()
                .Height(WebSiteButton.Bounds.Height)
                .Left(5.0f)
                .Right(5.0f)
                .Below(LocationCellView, pH * 0.003f);

            EventAgendaCellView.Frame = this.LayoutBox()
                .Height(50)
                .Left(0)
                .Right(0)
                .Bottom(0);

            EventAttendeesCellView.Frame = this.LayoutBox()
                .Height(50)
                .Left(0)
                .Right(0)
                .Above(EventAgendaCellView, 0);

            if (EventAgendaCellView.Hidden && EventAttendeesCellView.Hidden)
            {
                AddExpenseButton.Frame = this.LayoutBox()
                    .Right(15)
                    .Bottom(18)
                    .Width(this.Bounds.Width / 3)
                    .Height(40);

                ExpensesLabel.Frame = this.LayoutBox()
                    .Height(ExpensesLabel.Font.LineHeight)
                    .Before(AddExpenseButton, 5)
                    .Left(15)
                    .Bottom(18);
            }
            else {
                AddExpenseButton.Frame = this.LayoutBox()
                    .Right(15)
                    .Above(EventAttendeesCellView, 18)
                    .Width(this.Bounds.Width / 3)
                    .Height(40);

                ExpensesLabel.Frame = this.LayoutBox()
                    .Height(ExpensesLabel.Font.LineHeight)
                    .Before(AddExpenseButton, 5)
                    .Left(15)
                    .Above(EventAttendeesCellView, 12);
            }

            ExpensesTitleLabel.Frame = this.LayoutBox()
                .Height(ExpensesTitleLabel.Bounds.Height)
                .Width(ExpensesTitleLabel.Bounds.Width)
                .Left(15)
                .Above(ExpensesLabel, 2);

            ExpensesSeparatorView.Frame = this.LayoutBox()
                .Height(1)
                .Left(15)
                .Right(0)
                .Above(ExpensesTitleLabel, 8);
        }
    }
}

