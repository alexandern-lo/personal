using System;
using StudioMobile;
using UIKit;
using CoreAnimation;
using LiveOakApp.iOS.View.Skin;

namespace LiveOakApp.iOS.View
{
    public class CardScannerView : CustomView
    {
        [View(0)]
        public UIView CameraPreview { get; private set; }

        [View(1)]
        public UIView LeftBlur { get; private set; }
        [View(1)]
        public UIView RightBlur { get; private set; }
        [View(1)]
        public UIView TopBlur { get; private set; }
        [View(1)]
        public UIView BottomBlur { get; private set; }

        [View(2)]
        public UIButton CameraShotButton { get; private set; }

        public CALayer PreviewLayer = new CALayer();

        public float cardHeightRatio = 0.4f;
        public float cardWidthRatio = 0.85f;

        protected override void CreateView()
        {
            base.CreateView();
            BackgroundColor = Colors.MainGrayColor;
            LeftBlur.BackgroundColor = UIColor.FromWhiteAlpha(1, 0.4f);
            RightBlur.BackgroundColor = UIColor.FromWhiteAlpha(1, 0.4f);
            TopBlur.BackgroundColor = UIColor.FromWhiteAlpha(1, 0.4f);
            BottomBlur.BackgroundColor = UIColor.FromWhiteAlpha(1, 0.4f);
            CameraShotButton.SetImage(UIImage.FromBundle("button_camera.png"), UIControlState.Normal);
            CameraShotButton.SetImage(UIImage.FromBundle("button_camera_pressed.png"), UIControlState.Highlighted);
        }

        public void SetPreviewLayer(CALayer layer)
        {
            PreviewLayer = layer;
            CameraPreview.Layer.AddSublayer(PreviewLayer);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            CameraShotButton.SizeToFit();

            CameraPreview.Frame = this.LayoutBox()
                .Top(0)
                .Left(0)
                .Width(this.Bounds.Width)
                .Height(this.Bounds.Width * 4 / 3);
            PreviewLayer.Frame = CameraPreview.Bounds;

            var cardVerticalInset = (CameraPreview.Bounds.Height - CameraPreview.Bounds.Height * cardHeightRatio) / 2;
            var cardHorizontalInset = (CameraPreview.Bounds.Width - CameraPreview.Bounds.Width * cardWidthRatio) / 2;

            TopBlur.Frame = this.LayoutBox()
                .Left(CameraPreview, 0)
                .Right(CameraPreview, 0)
                .Top(CameraPreview, 0)
                .Height(cardVerticalInset);

            BottomBlur.Frame = this.LayoutBox()
                .Left(CameraPreview, 0)
                .Right(CameraPreview, 0)
                .Bottom(CameraPreview, 0)
                .Height(cardVerticalInset);

            LeftBlur.Frame = this.LayoutBox()
                .Left(CameraPreview, 0)
                .Top(CameraPreview, cardVerticalInset)
                .Bottom(CameraPreview, cardVerticalInset)
                .Width(cardHorizontalInset);

            RightBlur.Frame = this.LayoutBox()
                .Right(CameraPreview, 0)
                .Top(CameraPreview, cardVerticalInset)
                .Bottom(CameraPreview, cardVerticalInset)
                .Width(cardHorizontalInset);

            CameraShotButton.Frame = this.LayoutBox()
                .CenterHorizontally()
                .Below(CameraPreview, (Bounds.Height - CameraPreview.Bounds.Height) / 2 - CameraShotButton.Bounds.Height / 2)
                .Width(CameraShotButton.Bounds.Width)
                .Height(CameraShotButton.Bounds.Height);

        }
    }
}
