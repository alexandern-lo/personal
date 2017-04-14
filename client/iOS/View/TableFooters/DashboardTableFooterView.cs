using System;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.TableFooters
{
    public class DashboardTableFooterView : CustomView
    {
        [View(0)]
        public UIView TopSeparatorView { get; private set; }

        [View(1)]
        [LabelSkin("xSmallRegularBlackLabel")]
        public UILabel ThisYearExpensesTitleLabel { get; private set; }

        [View(2)]
        [LabelSkin("xSmallBoldBlackLabel")]
        public UILabel ThisYearExpensesValueLabel { get; private set; }

        [View(3)]
        [LabelSkin("xSmallRegularBlackLabel")]
        public UILabel AverageCostPerLeadTitleLabel { get; private set; }

        [View(4)]
        [LabelSkin("xSmallBoldBlackLabel")]
        public UILabel AverageCostPerLeadValueLabel { get; private set; }

        [View(5)]
        [ButtonSkin("AddExpenseButton")]
        public UIButton AddExpenseButton { get; private set; }

        [View(6)]
        public UIView BottomSeparatorView { get; private set; }

        public DashboardTableFooterView()
        {
            TopSeparatorView.BackgroundColor = Colors.GraySeparatorColor;
            ThisYearExpensesTitleLabel.Text = L10n.Localize("YearlyExpensesToDate", "Yearly expenses to date");
            AverageCostPerLeadTitleLabel.Text = L10n.Localize("AverageCostPerLead", "Average cost per lead");
            BottomSeparatorView.BackgroundColor = Colors.DefaultTableViewBackgroundColor;
        }

        public void SetupFooter(LeadsStatisticsViewModel leadsStatistics)
        {
            ThisYearExpensesValueLabel.Text = leadsStatistics.ThisYearExpenses.GetCurrencySymbol() + leadsStatistics.ThisYearExpenses.Amount.ToShortNumber();
            AverageCostPerLeadValueLabel.Text = leadsStatistics.ThisYearCostPerLead.GetCurrencySymbol() + leadsStatistics.ThisYearCostPerLead.Amount.ToShortNumber();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            ThisYearExpensesTitleLabel.SizeToFit();
            ThisYearExpensesValueLabel.SizeToFit();
            AverageCostPerLeadTitleLabel.SizeToFit();
            AverageCostPerLeadValueLabel.SizeToFit();
            AddExpenseButton.SizeToFit();

            TopSeparatorView.Frame = this.LayoutBox()
                .Top(0)
                .Height(1)
                .Left(15)
                .Right(15);

            ThisYearExpensesTitleLabel.Frame = this.LayoutBox()
                .CenterVertically(-17.5f)
                .Left(15)
                .Width(ThisYearExpensesTitleLabel.Bounds.Width)
                .Height(ThisYearExpensesTitleLabel.Bounds.Height);

            AverageCostPerLeadTitleLabel.Frame = this.LayoutBox()
                .CenterVertically(2.5f)
                .Left(15)
                .Width(AverageCostPerLeadTitleLabel.Bounds.Width)
                .Height(AverageCostPerLeadTitleLabel.Bounds.Height);

            AddExpenseButton.Frame = this.LayoutBox()
                .Right(10)
                .CenterVertically(-7.5f)
                .Width(this.Bounds.Width / 3)
                .Height(40);

            ThisYearExpensesValueLabel.Frame = this.LayoutBox()
                .CenterVertically(-17.5f)
                .Before(AddExpenseButton, 12)
                .Width(ThisYearExpensesValueLabel.Bounds.Width)
                .Height(ThisYearExpensesValueLabel.Bounds.Height);

            AverageCostPerLeadValueLabel.Frame = this.LayoutBox()
                .CenterVertically(2.5f)
                .Before(AddExpenseButton, 12)
                .Width(AverageCostPerLeadValueLabel.Bounds.Width)
                .Height(AverageCostPerLeadValueLabel.Bounds.Height);

            BottomSeparatorView.Frame = this.LayoutBox()
                .Bottom(0)
                .Left(0)
                .Right(0)
                .Height(15);
        }

        public static float FooterHeight { get { return 78f; } }
    }
}
