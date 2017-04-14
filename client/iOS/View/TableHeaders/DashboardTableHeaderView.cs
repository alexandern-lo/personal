using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.ViewModels;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.TableHeaders
{
    public class DashboardTableHeaderView : CustomView
    {
        [View(0)]
        [LabelSkin("xSmallRegularGrayLabel")]
        public UILabel LastPeriodCountTitleLabel { get; private set; }

        [View(1)]
        [LabelSkin("xxxLargeRegularBlackLabel")]
        public UILabel LastPeriodCountValueLabel { get; private set; }

        [View(2)]
        [LabelSkin("xSmallBoldBlackLabel")]
        public UILabel AllTimeCountValueLabel { get; private set; }

        [View(3)]
        [LabelSkin("xSmallRegularBlackLabel")]
        public UILabel AllTimeCountTitleLabel { get; private set; }

        [View(4)]
        [LabelSkin("xSmallRegularBlackLabel")]
        public UILabel LeadsGraphTitleLabel { get; private set; }

        [View(5)]
        [LabelSkin("xSmallRegularBlackLabel")]
        public UILabel GoalsGraphTitleLabel { get; private set; }

        [View(6)]
        public UIView GraphView { get; private set; }

        [View(7)]
        [LabelSkin("NormalRegularBlackLabel")]
        public UILabel LeadsGraphValueLabel { get; private set; }

        [View(8)]
        [LabelSkin("NormalRegularBlackLabel")]
        public UILabel GoalsGraphValueLabel { get; private set; }

        [View(9)]
        public UIImageView GraphSeparatorImageView { get; private set; }

        [View(10)]
        public UIView BottomSeparatorView { get; private set; }

        int lastPeriodLeadsPercentage = 100;
        bool needsDrawGraph = false;

        public void SetupHeader(LeadsStatisticsViewModel leadsStatistics)
        {
            LastPeriodCountValueLabel.Text = leadsStatistics.LastPeriodCount.ToString();
            AllTimeCountValueLabel.Text = leadsStatistics.AllTimeCount.ToString();

            if (leadsStatistics.LastPeriodGoal != 0)
                lastPeriodLeadsPercentage = (int)Math.Truncate((double)leadsStatistics.LastPeriodCount / (double)leadsStatistics.LastPeriodGoal * 100);
            lastPeriodLeadsPercentage = Math.Min(lastPeriodLeadsPercentage, 100);
            lastPeriodLeadsPercentage = Math.Max(lastPeriodLeadsPercentage, 0);
            LeadsGraphValueLabel.Text = lastPeriodLeadsPercentage + "%";
            GoalsGraphValueLabel.Text = (100 - lastPeriodLeadsPercentage) + "%";

            needsDrawGraph = true;
            SetNeedsLayout();
        }

        protected override void CreateView()
        {
            base.CreateView();
            LastPeriodCountTitleLabel.Text = "Leads (30 days)";
            AllTimeCountTitleLabel.Text = "all time";
            LeadsGraphTitleLabel.Text = "Leads";
            GoalsGraphTitleLabel.Text = "Goals";
            GraphSeparatorImageView.Image = UIImage.FromBundle("greyline.png");
            BottomSeparatorView.BackgroundColor = Colors.DefaultTableViewBackgroundColor;
            LastPeriodCountValueLabel.TextColor = Colors.MainGrayColor;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            LastPeriodCountTitleLabel.SizeToFit();
            LastPeriodCountValueLabel.SizeToFit();
            AllTimeCountValueLabel.SizeToFit();
            AllTimeCountTitleLabel.SizeToFit();
            LeadsGraphTitleLabel.SizeToFit();
            GoalsGraphTitleLabel.SizeToFit();
            LeadsGraphValueLabel.SizeToFit();
            GoalsGraphValueLabel.SizeToFit();
            GraphSeparatorImageView.SizeToFit();

            LastPeriodCountTitleLabel.Frame = this.LayoutBox()
                .Top(this.Bounds.Height * 0.27f)
                .Left(25)
                .Width(LastPeriodCountTitleLabel.Bounds.Width)
                .Height(LastPeriodCountTitleLabel.Bounds.Height);

            LastPeriodCountValueLabel.Frame = this.LayoutBox()
                .Below(LastPeriodCountTitleLabel, -13)
                .Left(25)
                .Width(LastPeriodCountValueLabel.Bounds.Width)
                .Height(LastPeriodCountValueLabel.Bounds.Height);

            AllTimeCountValueLabel.Frame = this.LayoutBox()
                .Below(LastPeriodCountValueLabel, -10)
                .Left(25)
                .Width(AllTimeCountValueLabel.Bounds.Width)
                .Height(AllTimeCountValueLabel.Bounds.Height);

            AllTimeCountTitleLabel.Frame = this.LayoutBox()
                .Below(LastPeriodCountValueLabel, -10)
                .After(AllTimeCountValueLabel, 3)
                .Width(AllTimeCountTitleLabel.Bounds.Width)
                .Height(AllTimeCountTitleLabel.Bounds.Height);

            GraphView.Frame = this.LayoutBox()
                .Right(40)
                .CenterVertically(-7.5f)
                .Height(this.Bounds.Height * 0.65f)
                .Width(this.Bounds.Height * 0.65f);

            LeadsGraphTitleLabel.Frame = this.LayoutBox()
                .Top(GraphView, -12)
                .Before(GraphView, -GraphView.Bounds.Width * 0.11f)
                .Width(LeadsGraphTitleLabel.Bounds.Width)
                .Height(LeadsGraphTitleLabel.Bounds.Height);

            GoalsGraphTitleLabel.Frame = this.LayoutBox()
                .Bottom(GraphView, -12)
                .After(GraphView, -GraphView.Bounds.Width * 0.11f)
                .Width(GoalsGraphTitleLabel.Bounds.Width)
                .Height(GoalsGraphTitleLabel.Bounds.Height);

            GraphSeparatorImageView.Frame = this.LayoutBox()
                .CenterVertically(GraphView)
                .CenterHorizontally(GraphView)
                .Width(GraphSeparatorImageView.Bounds.Width)
                .Height(GraphSeparatorImageView.Bounds.Height);

            LeadsGraphValueLabel.Frame = this.LayoutBox()
                .Before(GraphSeparatorImageView, 6)
                .CenterVertically(GraphView)
                .Width(LeadsGraphValueLabel.Bounds.Width)
                .Height(LeadsGraphValueLabel.Bounds.Height);

            GoalsGraphValueLabel.Frame = this.LayoutBox()
                .After(GraphSeparatorImageView, 6)
                .CenterVertically(GraphView)
                .Width(GoalsGraphValueLabel.Bounds.Width)
                .Height(GoalsGraphValueLabel.Bounds.Height);

            BottomSeparatorView.Frame = this.LayoutBox()
                .Bottom(0)
                .Height(15)
                .Left(0)
                .Right(0);

            if (needsDrawGraph)
            {
                DrawGraph(new CGPoint(GraphView.Frame.Width / 2, GraphView.Frame.Width / 2), GraphView.Frame.Width / 2, lastPeriodLeadsPercentage);
                needsDrawGraph = false;
            }
        }

        void DrawGraph(CGPoint _graphCenter, nfloat _graphRadius, int leadsPercentage)
        {
            GraphView.Layer.Sublayers = null;
            var graphCenter = _graphCenter;
            var graphRadius = _graphRadius;

            var leadsStartAngle = (nfloat)(-0.5 * Math.PI) - (nfloat)0.03;
            var goalsStartAngle = (nfloat)(-0.5 * Math.PI) + (nfloat)0.03;
            var leadsAngle = (nfloat)(-0.5 * Math.PI - 2 * Math.PI * (leadsPercentage) / 100) + (nfloat)0.03;
            var goalsAngle = (nfloat)(-0.5 * Math.PI + 2 * Math.PI * (100 - leadsPercentage) / 100) - (nfloat)0.03;

            if (lastPeriodLeadsPercentage == 100)
            {
                var leadsArc = new CAShapeLayer();
                leadsArc.Path = UIBezierPath.FromArc(graphCenter, graphRadius, (nfloat)(-0.5 * Math.PI), (nfloat)(-2.5 * Math.PI), false).CGPath;
                leadsArc.StrokeColor = Colors.MainGrayColor.CGColor;
                leadsArc.FillColor = UIColor.Clear.CGColor;
                leadsArc.LineWidth = 20;
                GraphView.Layer.AddSublayer(leadsArc);

                CABasicAnimation animateStrokeEnd = CABasicAnimation.FromKeyPath("strokeEnd");
                animateStrokeEnd.Duration = 1;
                animateStrokeEnd.From = NSNumber.FromInt32(0);
                animateStrokeEnd.To = NSNumber.FromInt32(1);
                leadsArc.AddAnimation(animateStrokeEnd, "strokeEnd");
            }
            else if (lastPeriodLeadsPercentage == 0)
            {
                var goalsArc = new CAShapeLayer();
                goalsArc.Path = UIBezierPath.FromArc(graphCenter, graphRadius, (nfloat)(-0.5 * Math.PI), (nfloat)(1.5 * Math.PI), true).CGPath;
                goalsArc.StrokeColor = Colors.MainBlueColor.CGColor;
                goalsArc.FillColor = UIColor.Clear.CGColor;
                goalsArc.LineWidth = 20;
                GraphView.Layer.AddSublayer(goalsArc);

                CABasicAnimation animateStrokeEnd = CABasicAnimation.FromKeyPath("strokeEnd");
                animateStrokeEnd.Duration = 1;
                animateStrokeEnd.From = NSNumber.FromInt32(0);
                animateStrokeEnd.To = NSNumber.FromInt32(1);
                goalsArc.AddAnimation(animateStrokeEnd, "strokeEnd");
            }
            else{
                var leadsArc = new CAShapeLayer();
                leadsArc.Path = UIBezierPath.FromArc(graphCenter, graphRadius, leadsStartAngle, leadsAngle, false).CGPath;
                leadsArc.StrokeColor = Colors.MainGrayColor.CGColor;
                leadsArc.FillColor = UIColor.Clear.CGColor;
                leadsArc.LineWidth = 20;
                GraphView.Layer.AddSublayer(leadsArc);
            
                var goalsArc = new CAShapeLayer();
                goalsArc.Path = UIBezierPath.FromArc(graphCenter, graphRadius, goalsStartAngle, goalsAngle, true).CGPath;
                goalsArc.StrokeColor = Colors.MainBlueColor.CGColor;
                goalsArc.FillColor = UIColor.Clear.CGColor;
                goalsArc.LineWidth = 20;
                GraphView.Layer.AddSublayer(goalsArc);

                CABasicAnimation animateStrokeEnd = CABasicAnimation.FromKeyPath("strokeEnd");
                animateStrokeEnd.Duration = 1;
                animateStrokeEnd.From = NSNumber.FromInt32(0);
                animateStrokeEnd.To = NSNumber.FromInt32(1);
                goalsArc.AddAnimation(animateStrokeEnd, "strokeEnd");
                leadsArc.AddAnimation(animateStrokeEnd, "strokeEnd");
            }
        }
    }
}
