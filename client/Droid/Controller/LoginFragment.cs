using Android.OS;
using Android.Views;
using StudioMobile;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Models;
using LiveOakApp.Droid.Services;

namespace LiveOakApp.Droid.Controller
{
    public class LoginFragment : CustomFragment
    {
        LoginViewModel ViewModel;
        LoginView view;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ViewModel = new LoginViewModel(Activity);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.LoginFragment, null);
            view = v.FindViewById<LoginView>(Resource.Id.login_view);

            Bindings.Command(ViewModel.LoginCommand)
                    .To(view.LoginButton.ClickTarget())
                    .WhenFinished((t, c) =>
                    {
                        NavigateNext();
                        ViewModel.LaunchLoginFlowAutomaticallyIfNeeded();
                    });

            Bindings.Property(ViewModel.LoginCommand, _ => _.IsRunning)
                    .UpdateTarget((s) => { view.LoginRunning = s.Value; });

            Bindings.Property(ViewModel.LoginCommand, _ => _.Error)
                    .Convert(_ => _?.MessageForHuman())
                    .To(view.ErrorTextView.TextProperty());

            Bindings.Command(ViewModel.ResetPasswordCommand)
                    .To(view.ResetPasswordTextView.ClickTarget())
                    .WhenFinished((t, c) => NavigateNext());

            Bindings.Property(ViewModel.ResetPasswordCommand, _ => _.IsRunning)
                    .UpdateTarget((s) => { view.ResetPasswordRunning = s.Value; });

            Bindings.Property(ViewModel.ResetPasswordCommand, _ => _.Error)
                    .Convert(_ => _?.MessageForHuman())
                    .To(view.ResetPasswordErrorTextView.TextProperty());

            NavigateNext();
            return view;
        }

        public override void OnStart()
        {
            base.OnStart();
            ViewModel.LaunchLoginFlowAutomaticallyIfNeeded();
        }

        void NavigateNext()
        {
            DroidNavigationManager.Instance.NavigateToRequiredStateIfNeeded();
        }
    }
}
