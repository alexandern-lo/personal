using StudioMobile;
using LiveOakApp.Models;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.iOS.View;
using LiveOakApp.iOS.Services;

namespace LiveOakApp.iOS.Controller
{
    public class TermsController : CustomController<TermsView>
    {
        TermsOfUseViewModel ViewModel;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationController.SetNavigationBarHidden(true, false);

            ViewModel = new TermsOfUseViewModel();

            Bindings.Property(ViewModel, _ => _.Terms)
                    .To(View.TermsTextView.TextProperty());

            Bindings.Property(ViewModel, _ => _.IsAccepted)
                    .UpdateTarget(_ => NavigateNext());

            Bindings.Property(ViewModel.LoadTermsCommand, _ => _.IsRunningWithoutData)
                    .UpdateTarget(_ => View.LoadTermsRunning = _.Value);

            Bindings.Command(ViewModel.AcceptCommand)
                    .To(View.AgreeButton.ClickTarget())
                    .WhenFinished((t, c) => NavigateNext());

            Bindings.Property(ViewModel.AcceptCommand, _ => _.IsRunning)
                    .UpdateTarget(_ => View.AcceptRunning = _.Value);

            Bindings.Property(ViewModel.AcceptCommand, _ => _.Error)
                    .Convert(_ => _?.MessageForHuman())
                    .To(View.ErrorLabel.TextProperty());

            Bindings.Command(ViewModel.DeclineCommand)
                    .To(View.DisagreeButton.ClickTarget())
                    .WhenFinished((t, c) => NavigateNext());

            Bindings.Property(ViewModel.LoadTermsCommand, _ => _.Error)
                    .UpdateTarget((source) => View.ErrorMessageText = source.Value.MessageForHuman());

            Bindings.Command(ViewModel.LoadTermsCommand)
                    .To(View.ErrorView.ReloadButton.ClickTarget());

            Bindings.Property(ViewModel, _ => _.HasTermsErrorOccured)
                    .Convert(_ => !_)
                    .To(View.ErrorView.HiddenProperty());

            ViewModel.LoadTermsCommand.Execute();
        }

        void NavigateNext()
        {
            IOSNavigationManager.Instance.NavigateToRequiredStateIfNeeded();
        }
    }
}
