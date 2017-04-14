using System;
using CoreGraphics;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.ViewModels;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Cells
{
    public class DashboardEventExpensesCell : CustomBindingsTableViewCell
    {
        public const string DefaultCellIdentifier = "DashboardEventExpensesCell";

        [View(0)]
        [LabelSkin("xSmallRegularBlackLabel")]
        public UILabel EventNameLabel { get; private set; }

        [View(1)]
        [LabelSkin("SmallRegularGrayLabel")]
        public UILabel SpentTitleLabel { get; private set; }

        [View(1)]
        [LabelSkin("SmallRegularBlackLabel")]
        public UILabel SpentValueLabel { get; private set; }

        [View(2)]
        public UIProgressView ProgressView { get; private set; }

        [View(3)]
        [LabelSkin("LargeRegularBlackLabel")]
        public UILabel CostPerLeadLabel { get; private set; }

        public DashboardEventExpensesCell(string cellId = DashboardEventExpensesCell.DefaultCellIdentifier) : base(UIKit.UITableViewCellStyle.Default, cellId)
        {
            SpentTitleLabel.Text = "Spent";
            ProgressView.ProgressTintColor = Colors.MainGrayColor;
            ProgressView.TrackTintColor = Colors.LightGray;
            ProgressView.Layer.MasksToBounds = true;
            ProgressView.ClipsToBounds = true;
            ProgressView.Layer.CornerRadius = 4;
        }

        public void SetupCell(DashboardEventViewModel dashboardEvent, Func<decimal> getMaxEventExpenses)
        {
            EventNameLabel.Text = dashboardEvent.Name;
            SpentValueLabel.Text = dashboardEvent.TotalExpenses.GetCurrencySymbol() + dashboardEvent.TotalExpenses.Amount.ToShortNumber();
            ProgressView.SetProgress(1, false);
            var maxEventExpenses = getMaxEventExpenses();
            if (maxEventExpenses != 0)
                ProgressView.SetProgress((float)dashboardEvent.TotalExpenses.Amount / (float)maxEventExpenses, false);
            if (dashboardEvent.LeadsCount == 0) CostPerLeadLabel.Text = dashboardEvent.TotalExpenses.GetCurrencySymbol() + 0;
            else CostPerLeadLabel.Text = dashboardEvent.TotalExpenses.GetCurrencySymbol() +
                    ((decimal)dashboardEvent.TotalExpenses.Amount / dashboardEvent.LeadsCount).ToShortNumber();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            EventNameLabel.SizeToFit();
            SpentTitleLabel.SizeToFit();
            SpentValueLabel.SizeToFit();
            CostPerLeadLabel.SizeToFit();

            var pw = (float)this.Bounds.Width;

            ProgressView.Transform = CGAffineTransform.MakeScale(1.0f, 1.0f);
            ProgressView.Frame = this.LayoutBox()
                .Bottom(15)
                .Height(ProgressView.Bounds.Height)
                .Left(13)
                .Width(pw * 0.78f);
            ProgressView.Transform = CGAffineTransform.MakeScale(1.0f, 3.3f);

            SpentValueLabel.Frame = this.LayoutBox()
                .Right(ProgressView, 2)
                .Above(ProgressView, 3)
                .Height(SpentValueLabel.Bounds.Height)
                .Width(SpentValueLabel.Bounds.Width);

            SpentTitleLabel.Frame = this.LayoutBox()
                .Before(SpentValueLabel, 4)
                .Above(ProgressView, 3)
                .Height(SpentTitleLabel.Bounds.Height)
                .Width(SpentTitleLabel.Bounds.Width);

            EventNameLabel.Frame = this.LayoutBox()
                .Left(15)
                .Before(SpentTitleLabel, 0)
                .Height(EventNameLabel.Font.LineHeight)
                .Above(ProgressView, 5);

            CostPerLeadLabel.Frame = this.LayoutBox()
                .Right(15)
                .CenterVertically()
                .Width(CostPerLeadLabel.Bounds.Width)
                .Height(CostPerLeadLabel.Bounds.Height);
        }
    }
}
