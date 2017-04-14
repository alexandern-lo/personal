using System;
using AVFoundation;
using Foundation;
using LiveOakApp.iOS.View;
using StudioMobile;
using CoreGraphics;
using UIKit;
using LiveOakApp.Resources;

namespace LiveOakApp.iOS
{
    public class CardScanerController : CustomController<CardScannerView>
    {
        AVCaptureSession CaptureSession { get; set; }
        AVCaptureStillImageOutput CaptureOutput { get; set; }
        Action<UIImage> OnCardScanned;

        public CardScanerController(Action<UIImage> onCardScanned, Action onCancelled) : base()
        {
            Title = L10n.Localize("CardScannerTitle", "Card scanner");
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (sender, e) => onCancelled());
            OnCardScanned = onCardScanned;
            CameraShotCommand = new Command()
            {
                Action = CameraShotAction
            };

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupCamera();
            Bindings.Command(CameraShotCommand).To(View.CameraShotButton.ClickTarget());
        }

        void SetupCamera()
        {
            if (!this.CheckCameraPermissions()) return;
            CaptureSession = new AVCaptureSession();
            CaptureSession.SessionPreset = AVCaptureSession.PresetPhoto;
            AVCaptureDevice captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
            NSError captureError;
            AVCaptureDeviceInput captureDeviceInput = new AVCaptureDeviceInput(captureDevice, out captureError);
            if (captureError == null)
                CaptureSession.AddInput(captureDeviceInput);
            CaptureOutput = new AVCaptureStillImageOutput();
            CaptureSession.AddOutput(CaptureOutput);

            AVCaptureVideoPreviewLayer previewLayer = new AVCaptureVideoPreviewLayer(CaptureSession);
            previewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
            View.SetPreviewLayer(previewLayer);
            CaptureSession.StartRunning();
        }


        Command CameraShotCommand { get; set; }
        void CameraShotAction(object arg)
        {
            if (!this.CheckCameraPermissions()) return;
            View.CameraShotButton.Enabled = false;
            CaptureOutput.CaptureStillImageAsynchronously(CaptureOutput.Connections[0], (imageDataSampleBuffer, error) =>
            {
                CaptureSession.StopRunning();
                NSData photoData = AVCaptureStillImageOutput.JpegStillToNSData(imageDataSampleBuffer);
                UIImage photo = new UIImage(photoData, 1);

                var photoCardHeight = photo.Size.Height * View.cardHeightRatio;
                var photoCardWidth = photo.Size.Width * View.cardWidthRatio;
                var cropRect = new CGRect((photo.Size.Height - photoCardHeight) / 2, (photo.Size.Width - photoCardWidth) / 2, photoCardHeight, photoCardWidth);
                photo = new UIImage(photo.CGImage.WithImageInRect(cropRect), 1, UIImageOrientation.Right);

                OnCardScanned(photo);
                View.CameraShotButton.Enabled = true;
                CaptureSession.StartRunning();
            });
        }
    }
}
