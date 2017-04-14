using System.Threading.Tasks;
using StudioMobile;
using LiveOakApp.Models.Services;

namespace LiveOakApp.Models.ViewModels
{
    public class StartupViewModel : DataContext
    {
        TermsOfUseService TermsOfUseService { get; set; }
        ProfileService ProfileService { get; set; }

        public StartupViewModel()
        {
            TermsOfUseService = ServiceLocator.Instance.TermsOfUseService;
            ProfileService = ServiceLocator.Instance.ProfileService;

            SetupCommand = new AsyncCommand
            {
                Action = SetupAction,
                CanExecute = CanExecuteSetupAction
            };
        }

        string progress;
        public string Progress
        {
            get { return progress; }
            private set
            {
                progress = value;
                LOG.Debug("startup progress: {0}", value);
                RaisePropertyChanged();
            }
        }

        public AsyncCommand SetupCommand { get; private set; }

        async Task SetupAction(object arg)
        {
            await ProfileService.LoadProfileFromCacheIfNeeded();
            await TermsOfUseService.LoadTermsFromCacheIfNeeded();
        }

        bool CanExecuteSetupAction(object arg)
        {
            return !SetupCommand.IsRunning;
        }
    }
}
