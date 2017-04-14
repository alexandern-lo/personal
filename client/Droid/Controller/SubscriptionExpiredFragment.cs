using Android.OS;
using Android.Views;
using StudioMobile;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Droid.Views;
using LiveOakApp.Droid.Services;

namespace LiveOakApp.Droid.Controller
{
    public class SubscriptionExpiredFragment : CustomFragment
    {
        SubscriptionExpiredViewModel ViewModel;
        SubscriptionExpiredView view;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ViewModel = new SubscriptionExpiredViewModel();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.SubscriptionExpiredFragment, null);
            view = v.FindViewById<SubscriptionExpiredView>(Resource.Id.subscription_expired_view);

            Bindings.Property(ViewModel, _ => _.IsSubscriptionValid)
                    .UpdateTarget(_ => NavigateNext());

            Bindings.Command(ViewModel.RecheckCommand)
                    .To(view.RecheckButton.ClickTarget())
                    .WhenFinished((t, c) => NavigateNext());

            Bindings.Property(ViewModel.RecheckCommand, _ => _.IsRunning)
                    .UpdateTarget(_ => view.RecheckRunning = _.Value);

            Bindings.Command(ViewModel.LogoutCommand)
                    .To(view.LogoutButton.ClickTarget())
                    .WhenFinished((t, c) => NavigateNext());

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
            ViewModel.RecheckCommand.Execute();
        }

        void NavigateNext()
        {
            DroidNavigationManager.Instance.NavigateToRequiredStateIfNeeded();
        }
    }
}
