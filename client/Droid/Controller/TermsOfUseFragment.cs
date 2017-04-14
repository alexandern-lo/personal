using Android.OS;
using Android.Views;
using StudioMobile;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models;
using LiveOakApp.Droid.Services;
using System.Runtime.InteropServices;

namespace LiveOakApp.Droid.Controller
{
    public class TermsOfUseFragment : CustomFragment
    {
        TermsOfUseViewModel ViewModel;
        TermsOfUseView view;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ViewModel = new TermsOfUseViewModel();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.TermsOfUseFragment, null);
            view = v.FindViewById<TermsOfUseView>(Resource.Id.terms_of_use);

            Bindings.Property(ViewModel, _ => _.Terms)
                    .To(view.TermsTextView.TextProperty());

            Bindings.Property(ViewModel, _ => _.IsAccepted)
                    .UpdateTarget(_ => NavigateNext());

            Bindings.Property(ViewModel.LoadTermsCommand, _ => _.IsRunningWithoutData)
                    .UpdateTarget(_ => view.LoadTermsRunning = _.Value);

            Bindings.Command(ViewModel.AcceptCommand)
                    .To(view.AcceptButton.ClickTarget())
                    .WhenFinished((t, c) => NavigateNext());

            Bindings.Property(ViewModel.AcceptCommand, _ => _.IsRunning)
                    .UpdateTarget(_ => view.AcceptRunning = _.Value);

            Bindings.Property(ViewModel.AcceptCommand, _ => _.Error)
                    .Convert(_ => _?.MessageForHuman())
                    .To(view.ErrorTextView.TextProperty());

            Bindings.Property(ViewModel.LoadTermsCommand, _ => _.Error)
                    .Convert(_ => _?.MessageForHuman())
                    .To(view.ErrorTextView.TextProperty());

            Bindings.Property(ViewModel, _ => _.HasTermsErrorOccured)
                    .UpdateTarget((arg) =>
            {
                if (arg.Value)
                {
                    view.ShowErrorMessage();
                    view.ShowErrorButton();
                }
                else if(!ViewModel.HasAcceptErrorOccured)
                {
                    view.HideError();
                }
            });

            Bindings.Property(ViewModel, _ => _.HasAcceptErrorOccured)
                    .UpdateTarget((arg) =>
            {
                if (arg.Value)
                    view.ShowErrorMessage();
                else if(!ViewModel.HasTermsErrorOccured)
                    view.HideError();
            });

            Bindings.Command(ViewModel.DeclineCommand)
                    .To(view.DeclineButton.ClickTarget())
                    .WhenFinished((t, c) => NavigateNext());

            Bindings.Command(ViewModel.LoadTermsCommand)
                    .To(view.ReloadButton.ClickTarget());

            ViewModel.LoadTermsCommand.Execute();

            return view;
        }

        void NavigateNext()
        {
            DroidNavigationManager.Instance.NavigateToRequiredStateIfNeeded();
        }
    }
}
