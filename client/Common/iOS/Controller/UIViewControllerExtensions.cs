using UIKit;

namespace StudioMobile
{
    public static class UIViewControllerExtensions
    {
        public static void ShowInfoAlert(this UIViewController controller, string title, string message, string cancelButtonTitle)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create(cancelButtonTitle, UIAlertActionStyle.Cancel, null));
            controller.PresentViewController(alert, true, null);
        }
    }
}
