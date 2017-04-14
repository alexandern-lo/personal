using StudioMobile;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.iOS.View;
using LiveOakApp.iOS.Services;
using UIKit;
using LiveOakApp.Models.Services;

namespace LiveOakApp.iOS.Controller
{
    public class SubscriptionExpiredController : CustomController<SubscriptionExpiredView>
    {
        SubscriptionExpiredViewModel ViewModel;

        public SubscriptionExpiredController()
        {
            RenewSubscriptionCommand = new Command()
            {
                Action = RenewSubscriptionAction
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationController.SetNavigationBarHidden(true, false);

            ViewModel = new SubscriptionExpiredViewModel();

            Bindings.Property(ViewModel, _ => _.IsSubscriptionValid)
                    .UpdateTarget(_ => NavigateNext());

            Bindings.Command(ViewModel.RecheckCommand)
                    .To(View.RecheckButton.ClickTarget())
                    .WhenFinished((t, c) => NavigateNext());

            Bindings.Property(ViewModel.RecheckCommand, _ => _.IsRunning)
                    .UpdateTarget(_ => View.RecheckRunning = _.Value);

            Bindings.Command(ViewModel.LogoutCommand)
                    .To(View.LogoutButton.ClickTarget())
                    .WhenFinished((t, c) => NavigateNext());

            Bindings.Command(RenewSubscriptionCommand)
                    .To(View.RenewSubscriptionButton.ClickTarget());
        }

        public Command RenewSubscriptionCommand { get; private set; }
        void RenewSubscriptionAction(object param)
        {
            var subscriptionRenewUrl = ApplicationConfig.SubscriptionRenewUrl.TryParseWebsiteUri();
            if (UIApplication.SharedApplication.CanOpenUrl(subscriptionRenewUrl))
                UIApplication.SharedApplication.OpenUrl(subscriptionRenewUrl);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ViewModel.RecheckCommand.Execute();
        }

        void NavigateNext()
        {
            IOSNavigationManager.Instance.NavigateToRequiredStateIfNeeded();
        }
    }
}
