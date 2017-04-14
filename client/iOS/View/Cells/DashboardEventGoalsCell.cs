using System;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.ViewModels;
using StudioMobile;
using UIKit;
using CoreGraphics;

namespace LiveOakApp.iOS.View.Cells
{
    public class DashboardEventGoalsCell : CustomBindingsTableViewCell
    {
        public const string DefaultCellIdentifier = "DashboardEventGoalsCell";

        [View(0)]
        [LabelSkin("xSmallRegularBlackLabel")]
        public UILabel EventNameLabel { get; private set; }

        [View(1)]
        [LabelSkin("SmallRegularGrayLabel")]
        public UILabel GoalTitleLabel { get; private set; }

        [View(1)]
        [LabelSkin("SmallBoldBlackLabel")]
        public UILabel GoalValueLabel { get; private set; }

        [View(2)]
        public UIProgressView GoalProgressView { get; private set; }

        [View(3)]
        [LabelSkin("LargeBoldBlackLabel")]
        public UILabel LeadsCountLabel { get; private set; }

        [View(4)]
        public UIButton GoalEditingButton { get; private set; }

        public DashboardEventViewModel ViewModel { get; private set; }

        public DashboardEventGoalsCell(string cellId = DashboardEventGoalsCell.DefaultCellIdentifier) : base(UIKit.UITableViewCellStyle.Default, cellId)
        {
            GoalTitleLabel.Text = "Goal";
            GoalEditingButton.SetImage(UIImage.FromBundle("button_edit.png"), UIControlState.Normal);
            GoalProgressView.ProgressTintColor = Colors.MainGrayColor;
            GoalProgressView.TrackTintColor = Colors.LightGray;
            GoalProgressView.Layer.MasksToBounds = true;
            GoalProgressView.ClipsToBounds = true;
            GoalProgressView.Layer.CornerRadius = 4;
        }

        public void SetupCell(DashboardEventViewModel dashboardEvent, Action<DashboardEventViewModel> goalEditingButtonClickAction)
        {
            ViewModel = dashboardEvent;
            EventNameLabel.Text = dashboardEvent.Name;
            GoalValueLabel.Text = ((decimal)dashboardEvent.LeadsGoal).ToShortNumber();
            GoalEditingButton.RemoveTarget(null, null, UIControlEvent.TouchUpInside);
            GoalEditingButton.TouchUpInside += (sender, e) => goalEditingButtonClickAction(ViewModel);

            GoalProgressView.SetProgress(1, false);
            if (dashboardEvent.LeadsGoal != 0)
                GoalProgressView.SetProgress((float)dashboardEvent.LeadsCount / (float)dashboardEvent.LeadsGoal, false);
            LeadsCountLabel.Text = ((decimal)dashboardEvent.LeadsCount).ToShortNumber();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            EventNameLabel.SizeToFit();
            GoalTitleLabel.SizeToFit();
            GoalValueLabel.SizeToFit();
            GoalEditingButton.SizeToFit();
            LeadsCountLabel.SizeToFit();

            var pw = (float)this.Bounds.Width;

            GoalProgressView.Transform = CGAffineTransform.MakeScale(1.0f, 1.0f);
            GoalProgressView.Frame = this.LayoutBox()
                .Bottom(15)
                .Height(GoalProgressView.Bounds.Height)
                .Left(13)
                .Width(pw * 0.72f);
            GoalProgressView.Transform = CGAffineTransform.MakeScale(1.0f, 3.3f);

            GoalValueLabel.Frame = this.LayoutBox()
                .Right(GoalProgressView, 2)
                .Above(GoalProgressView, 3)
                .Height(GoalValueLabel.Bounds.Height)
                .Width(GoalValueLabel.Bounds.Width);

            GoalTitleLabel.Frame = this.LayoutBox()
                .Before(GoalValueLabel, 3)
                .Above(GoalProgressView, 3)
                .Height(GoalTitleLabel.Bounds.Height)
                .Width(GoalTitleLabel.Bounds.Width);

            EventNameLabel.Frame = this.LayoutBox()
                .Left(15)
                .Before(GoalTitleLabel, 0)
                .Height(EventNameLabel.Font.LineHeight)
                .Above(GoalProgressView, 5);

            GoalEditingButton.Frame = this.LayoutBox()
                .After(GoalProgressView, 0)
                .CenterVertically()
                .Height(44)
                .Width(44);

            LeadsCountLabel.Frame = this.LayoutBox()
                .CenterVertically()
                .Width(LeadsCountLabel.Bounds.Width)
                .Height(LeadsCountLabel.Bounds.Height)
                .Right(15);
        }
    }
}
