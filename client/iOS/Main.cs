using LiveOakApp.Models;
using SL4N;
using UIKit;
using ServiceStack;

namespace LiveOakApp.iOS
{
	public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // Configure ServiceStack Client
            IosPclExportClient.Configure();

            //trigger service locator init
			var instance = ServiceLocator.Instance;
			LoggerFactory.GetLogger<Application>().Info("Application starting");

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
