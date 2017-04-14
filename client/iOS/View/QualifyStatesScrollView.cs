using System;
using UIKit;
using StudioMobile;
using CoreGraphics;

namespace LiveOakApp.iOS.View
{
    public class QualifyStatesScrollView : UIScrollView
    {
        public LeadQualifyStateView StateViewCold { get; private set; } = LeadQualifyStateView.LeadQualityColdStateView();
        public LeadQualifyStateView StateViewWarm { get; private set; } = LeadQualifyStateView.LeadQualityWarmStateView();
        public LeadQualifyStateView StateViewHot { get; private set; } = LeadQualifyStateView.LeadQualityHotStateView();

        public QualifyStatesScrollView()
        {
            AddSubview(StateViewCold);
            AddSubview(StateViewWarm);
            AddSubview(StateViewHot);
            PagingEnabled = true;
            Layer.MasksToBounds = false;
            ClipsToBounds = false;
            ShowsVerticalScrollIndicator = false;
            ShowsHorizontalScrollIndicator = false;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            StateViewCold.Frame = this.LayoutBox()
                .CenterVertically()
                .Left(14)
                .Width(Bounds.Width - 14)
                .Height(Bounds.Height * 0.555f);

            StateViewWarm.Frame = this.LayoutBox()
                .After(StateViewCold, 14)
                .CenterVertically()
                .Width(Bounds.Width - 14)
                .Height(Bounds.Height * 0.555f);

            StateViewHot.Frame = this.LayoutBox()
                .After(StateViewWarm, 14)
                .CenterVertically()
                .Width(Bounds.Width - 14)
                .Height(Bounds.Height * 0.555f);

            ContentSize = new CGSize(StateViewHot.Frame.GetMaxX(), Bounds.Height);
        }
    }
}

