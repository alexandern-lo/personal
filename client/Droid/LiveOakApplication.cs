using System;
using Android.App;
using Android.Content;
using SL4N;
using LiveOakApp.Models;
using HockeyApp.Android;
using FFImageLoading;

[assembly: MetaData("com.google.android.maps.v2.API_KEY", Value = "AIzaSyCcwcnX2v1XcbIsfru96YvBybSuLMCKpDU")]

namespace LiveOakApp.Droid
{
    [Application]
    public class LiveOakApplication : Application
    {
        static readonly ILogger LOG;

        static LiveOakApplication()
        {
            //init service locator before everything else
            var i = ServiceLocator.Instance;
            LOG = LoggerFactory.GetLogger<LiveOakApplication>();
        }

        public LiveOakApplication(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
#if !DEBUG
            CrashManager.Register(this, "18083d70329541969faa6d7565053f13", new MyCrashManagerListener());
#endif
            LOG.Info("App started");
            ServiceLocator.Instance.IsAppInForeground = true;
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            ServiceLocator.Instance.DropInMemoryCaches();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            base.OnTrimMemory(level);
            if (level == TrimMemory.UiHidden)
            {
                ServiceLocator.Instance.FlushCaches();
            }
        }

        public override void OnTerminate()
        {
            ServiceLocator.Instance.Terminate();
            base.OnTerminate();
        }
    }

    class MyCrashManagerListener : CrashManagerListener
    {
        public override bool ShouldAutoUploadCrashes()
        {
            return true;
        }
    }
}
