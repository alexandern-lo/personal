using System;
using Android.OS;
using Android.Views;
using StudioMobile;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Droid.Services;
using System.Threading.Tasks;

namespace LiveOakApp.Droid.Controller
{
    public class StartupFragment : CustomFragment
    {
        StartupViewModel ViewModel;
        StartupView view;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ViewModel = new StartupViewModel();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.StartupFragment, null);
            view = v.FindViewById<StartupView>(Resource.Id.startup_view);

            Bindings.Property(ViewModel, _ => _.Progress)
                    .To(view.ProgressTextView.TextProperty());

            PerformStartup().Ignore();
            return view;
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
            DroidNavigationManager.Instance.NavigateToRequiredStateIfNeeded();
        }
    }
}
