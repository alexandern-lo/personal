using Foundation;
using UIKit;
using SL4N;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models;
using LiveOakApp.iOS.Services;
using HockeyApp.iOS;

namespace LiveOakApp.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public static AppDelegate Shared { get { return (AppDelegate)UIApplication.SharedApplication.Delegate; } }

        readonly ILogger LOG = LoggerFactory.GetLogger<AppDelegate>();
        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
#if !DEBUG
            var hockey = BITHockeyManager.SharedHockeyManager;
            hockey.Configure("4c8529b7834046a694ee90501132cabc");
            hockey.CrashManager.CrashManagerStatus = BITCrashManagerStatus.AutoSend;
            hockey.StartManager();
#endif

            Window = new UIWindow();

            CommonSkin.SetupAppearance();

            LOG.Info("App Started");

            IOSNavigationManager.Instance.NavigateToRequiredStateIfNeeded();

            Window.MakeKeyAndVisible();
            ServiceLocator.Instance.IsAppInForeground = true;
            return true;
        }

        public override void ReceiveMemoryWarning(UIApplication application)
        {
            ServiceLocator.Instance.DropInMemoryCaches();
        }

        public override void DidEnterBackground(UIApplication application)
        {
            ServiceLocator.Instance.IsAppInForeground = false;
            ServiceLocator.Instance.FlushCaches();
        }

        public override void WillEnterForeground(UIApplication application)
        {
            ServiceLocator.Instance.IsAppInForeground = true;
        }

        public override void WillTerminate(UIApplication application)
        {
            ServiceLocator.Instance.Terminate();
        }
    }
}
