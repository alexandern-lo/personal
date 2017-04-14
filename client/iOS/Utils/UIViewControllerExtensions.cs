using System;
using UIKit;
using SL4N;
using LiveOakApp.Resources;

namespace LiveOakApp.iOS.Utils
{
    public static class UIViewControllerExtensions
    {
        public static void PresentErrorAlert(this UIViewController controller, string logMessage, Exception error)
        {
            if (error == null) return;
            var LOG = LoggerFactory.GetLogger(controller.GetType().Name);

            LOG.Warn(logMessage, error);

            var errorDialog = UIAlertController.Create(error.Message, null, UIAlertControllerStyle.Alert);
            errorDialog.AddAction(UIAlertAction.Create(L10n.Localize("Ok", "Ok"), UIAlertActionStyle.Cancel, null));
            controller.PresentViewController(errorDialog, true, null);
        }
    }
}
