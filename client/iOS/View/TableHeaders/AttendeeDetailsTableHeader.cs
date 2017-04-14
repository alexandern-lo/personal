using System;
using CoreGraphics;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.TableHeaders
{
    public class AttendeeDetailsTableHeader : CustomView
    {
        [View(0)]
        public RemoteImageView AvatarRemoteImageView { get; private set; }

        public AttendeeDetailsTableHeader()
        {
        }

        protected override void CreateView()
        {
            base.CreateView();
            AvatarRemoteImageView.Layer.MasksToBounds = true;
            AvatarRemoteImageView.Placeholder = UIImage.FromBundle("user-default-image");
            this.Frame = new CGRect(0, 0, 1, 220);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var avatarDiameter = 150;
            AvatarRemoteImageView.Layer.CornerRadius = avatarDiameter / 2.0f;
            AvatarRemoteImageView.Frame = this.LayoutBox()
                .Top(40)
                .CenterHorizontally()
                .Width(avatarDiameter)
                .Height(avatarDiameter);
        }
    }
}

