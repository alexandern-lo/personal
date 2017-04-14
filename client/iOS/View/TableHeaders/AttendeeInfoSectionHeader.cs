using System;
using Foundation;
using UIKit;
using System.Drawing;
using CoreGraphics;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;

namespace LiveOakApp.iOS.View.TableHeaders
{
    [Register("AttendeeInfoSectionHeader")]
    public class AttendeeInfoSectionHeader : CustomView
    {
        [View(2)]
        [LabelSkin("AttendeeDetailsFullNameLabel")]
        public UILabel FullNameLabel { get; private set; }

        [View(3)]
        [LabelSkin("AttendeeDetailsPositionLabel")]
        public UILabel PositionLabel { get; private set; }

        [View(4)]
        [LabelSkin("AttendeeDetailsCompanyLabel")]
        public UILabel CompanyLabel { get; private set; }

        public AttendeeInfoSectionHeader()
        {

        }

       protected override void CreateView()
        {
            base.CreateView();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            FullNameLabel.SizeToFit();
            PositionLabel.SizeToFit();
            CompanyLabel.SizeToFit();
           
            FullNameLabel.Frame = this.LayoutBox()
                .Height(FullNameLabel.Bounds.Height)
                .Top(3)
                .Left(5.0f)
                .Right(5.0f);

            PositionLabel.Frame = this.LayoutBox()
                .Height(PositionLabel.Bounds.Height)
                .Below(FullNameLabel,10)
                .Left(5.0f)
                .Right(5.0f);

            CompanyLabel.Frame = this.LayoutBox()
                .Height(CompanyLabel.Bounds.Height)
                .Below(PositionLabel,10)
                .Left(5.0f)
                .Right(5.0f);
        }

        public static float HeaderHeight
        {
            get
            {
                return 100f;
            }
        }
    }
}

