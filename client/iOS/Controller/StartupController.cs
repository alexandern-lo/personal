using System;
using StudioMobile;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.iOS.View;
using LiveOakApp.iOS.Services;
using System.Threading.Tasks;

namespace LiveOakApp.iOS.Controller
{
    public class StartupController : CustomController<StartupView>
    {
        StartupViewModel ViewModel;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationController.SetNavigationBarHidden(true, false);

            ViewModel = new StartupViewModel();

            Bindings.Property(ViewModel, _ => _.Progress)
                    .To(View.ProgressLabel.TextProperty());

            PerformStartup().Ignore();
        }

        async Task PerformStartup()
        {
            try
            {
                await ViewModel.SetupCommand.ExecuteAsync(null);
            }
            catch (Exception error)
            {
                LOG.Warn("Startup failed", error);
            }
            NavigateNext();
        }

        void NavigateNext()
        {
            IOSNavigationManager.Instance.NavigateToRequiredStateIfNeeded();
        }
    }
}
