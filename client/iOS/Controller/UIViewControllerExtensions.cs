using System;
using AVFoundation;
using Foundation;
using LiveOakApp.Resources;
using Photos;
using UIKit;

namespace LiveOakApp.iOS
{
    public static class UIViewControllerExtensions
    {
        public static bool CheckCameraPermissions(this UIViewController controller)
        {
            bool isCameraAccess = AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video).Result;
            if (!isCameraAccess)
            {
                var alert = UIAlertController.Create(L10n.Localize("NoCameraAccessTitle", "No camera access"),
                                                     L10n.Localize("NoCameraAccessMessage", "Please go to Settings and enable the camera for this app to use this feature."),
                                                     UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create(L10n.Localize("SettingsLabel", "Settings"), UIAlertActionStyle.Default, (obj) =>
                                                     UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(UIApplication.OpenSettingsUrlString))));
                alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
                controller.PresentViewController(alert, true, null);
            }
            return isCameraAccess;
        }


        public static bool CheckPhotosPermissions(this UIViewController controller)
        {
            bool isPhotosAccess = PHPhotoLibrary.RequestAuthorizationAsync().Result == PHAuthorizationStatus.Authorized;
            if (!isPhotosAccess)
            {
                var alert = UIAlertController.Create(L10n.Localize("NoPhotosAccessTitle", "No Photos access"),
                                                     L10n.Localize("NoPhotosAccessMessage", "Please go to Settings and enable the Photos for this app to use this feature."),
                                                     UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create(L10n.Localize("SettingsLabel", "Settings"), UIAlertActionStyle.Default, (obj) =>
                                                     UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(UIApplication.OpenSettingsUrlString))));
                alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
                controller.PresentViewController(alert, true, null);
            }
            return isPhotosAccess;
        }
    }
}
