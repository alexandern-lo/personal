using System;
using System.Globalization;
using Foundation;
using CoreGraphics;
using LiveOakApp.iOS.View;
using LiveOakApp.iOS.View.Content;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.Controller.Content
{
    public class AgendaController : CustomController<AgendaView>
    {
        readonly AgendaViewModel ViewModel;

        public AgendaController(EventViewModel @event)
        {
            Title = L10n.Localize("AgendaNavigationBarTitle", "Event agenda");
            ViewModel = new AgendaViewModel(@event);

            AgendaSelectCommand = new Command
            {
                Action = AgendaSelectAction
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationItem.BackBarButtonItem = CommonSkin.BackBarButtonItem;

            var sectionsBinding = View.GetSectionsBinding(ViewModel.CurrentAgendaSections, TableHeaderClickAction);
            Bindings.Command(AgendaSelectCommand).To(sectionsBinding.ItemSelectedTarget());
            Bindings.Add(sectionsBinding);
            Bindings.Property(ViewModel.LoadAgendaCommand, _ => _.IsRunning).UpdateTarget((s) =>
            {
                View.FetchRunning = s.Value;
                if (!s.Value) View.SetupDateButtons(ViewModel.AgendaDates, DateClickAction);
                View.RefreshControl.Subviews[0].Subviews[0].Hidden = s.Value;
            });
            Bindings.Property(ViewModel.LoadAgendaCommand, _ => _.IsRunning)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());
            Bindings.Property(ViewModel.LoadAgendaCommand, _ => _.Error)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());
            Bindings.Property(ViewModel.CurrentAgendaSections, _ => _.Count)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());
            Bindings.Property(ViewModel, _ => _.CurrentAgendaDate).To(View.CurrentDateProperty());
            ViewModel.ExecuteLoadIfNeeded().Ignore();

            View.RefreshControl.AddTarget((sender, e) =>
            {
                View.RefreshControl.EndRefreshing();
                ViewModel.LoadAgendaCommand.Execute();
            }, UIControlEvent.ValueChanged);
        }

        public void UpdateErrorAndWarningViews()
        {
            bool isError = ViewModel.LoadAgendaCommand.Error != null;
            bool isEmptyList = ViewModel.CurrentAgendaSections.Count == 0;
            bool fetchRunning = View.FetchRunning;

            View.ErrorView.Hidden = !isError || fetchRunning;
            View.MessageView.Hidden = isError || !isEmptyList || fetchRunning;

            if (isError)
            {
                View.ErrorView.ErrorMessageLabel.Text = ViewModel.LoadAgendaCommand.Error.MessageForHuman();
                View.ErrorView.SizeToFit();
            }
            else if (isEmptyList)
            {
                View.MessageView.MessageLabel.Text = L10n.Localize("NoAgendaLabel", "No agenda");
                View.MessageView.SizeToFit();
            }
        }

        Command AgendaSelectCommand { get; set; }
        void AgendaSelectAction(object obj)
        {
            var agentaItem = (AgendaItemViewModel)obj;
            NSUrl urlToOpen = agentaItem.DetailsUrl.TryParseWebsiteUri()?.ToNSUrl();
            if (urlToOpen == null) return;
            UIApplication.SharedApplication.OpenUrl(urlToOpen);
        }

        void DateClickAction(object obj)
        {
            var dateButton = (AgendaDateButton)obj;
            var index = View.AgendaDateButtons.IndexOf(dateButton);
            ViewModel.CurrentAgendaDate = ViewModel.AgendaDates[index].Date;
        }

        void TableHeaderClickAction(string url)
        {
            NSUrl urlToOpen = url.TryParseWebsiteUri()?.ToNSUrl();
            if (urlToOpen == null) return;
            UIApplication.SharedApplication.OpenUrl(urlToOpen);
        }
    }
}
