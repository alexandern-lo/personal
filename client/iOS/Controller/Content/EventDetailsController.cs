using System;
using Foundation;
using UIKit;
using StudioMobile;
using LiveOakApp.iOS.View.Content;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using System.Globalization;

namespace LiveOakApp.iOS.Controller.Content
{
    public class EventDetailsController : CustomController<EventDetailsView>
    {
        EventDetailsViewModel ViewModel { get; set; }

        public EventDetailsController(EventViewModel eventData)
        {
            ViewModel = new EventDetailsViewModel(eventData);
            Title = ViewModel.Title;
            OpenUrlCommand = new Command()
            {
                Action = OpenUrlAction,
                CanExecute = CanExecuteOpenUrl
            };
            OpenAttendeesCommand = new Command()
            {
                Action = OpenAttendeesAction
            };
            OpenAgendaCommand = new Command()
            {
                Action = OpenAgendaAction
            };
            AddExpenseButtonClickCommand = new Command()
            {
                Action = AddExpenseButtonClickAction
            };
            OpenMapCommand = new Command()
            {
                Action = OpenMapAction,
                CanExecute = (o) => ViewModel.Location != null && ViewModel.Location.Trim() != string.Empty,
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ViewModel.LoadTotalExpensesCommand.Execute();
            NavigationItem.BackBarButtonItem = CommonSkin.BackBarButtonItem;

            Bindings.Property(ViewModel, _ => _.Type)
                    .To(View.EventTypeLabel.TextProperty());

            Bindings.Property(ViewModel, _ => _.TypeEnum)
                    .UpdateTarget((source) => View.SetAgendaAndAttendeesButtonHidden(source.Value == EventViewModel.EventType.Personal));

            Bindings.Property(ViewModel, _ => _.Industry)
                    .To(View.EventIndustryLabel.TextProperty());

            Bindings.Property(ViewModel, _ => _.IsReccuring)
                    .UpdateTarget(e => View.SetRecurringLabelHidden(!e.Value));

            Bindings.Property(ViewModel, _ => _.Date)
                    .UpdateTarget(e => { this.View.TimeCellView.SetTitle(e.Value, UIControlState.Normal); this.View.SetNeedsLayout(); });

            Bindings.Property(ViewModel, _ => _.Location)
                    .UpdateTarget(e => { this.View.LocationCellView.SetTitle(e.Value, UIControlState.Normal); this.View.SetNeedsLayout(); });
            Bindings.Command(OpenMapCommand).To(View.LocationCellView.ClickTarget());

            Bindings.Property(ViewModel, _ => _.WebsiteUrlString)
                    .UpdateTarget(e => { this.View.SetWebLinkUrl(e.Value); });

            Bindings.Command(OpenUrlCommand)
                    .To(View.WebSiteButton.ClickTarget());

            Bindings.Command(OpenAttendeesCommand)
                    .To(View.EventAttendeesCellView.ClickTarget());

            Bindings.Command(OpenAgendaCommand)
                    .To(View.EventAgendaCellView.ClickTarget());

            Bindings.Property(ViewModel, _ => _.EventLogo)
                    .To(View.EventIconRemoteImageView.ImageProperty());

            Bindings.Property(ViewModel, _ => _.TotalExpenses)
                    .Convert((arg) => arg.GetCurrencySymbol() + arg.Amount)
                    .To(View.ExpensesLabel.TextProperty());
            Bindings.Command(AddExpenseButtonClickCommand).To(View.AddExpenseButton.ClickTarget());
        }

        #region Actions

        Command OpenUrlCommand { get; set; }
        void OpenUrlAction(object param)
        {
            var url = ViewModel.WebsiteUri.ToNSUrl();
            if (url != null)
            {
                UIApplication.SharedApplication.OpenUrl(url);
            }
        }

        bool CanExecuteOpenUrl(object param)
        {
            return ViewModel.WebsiteUri != null;
        }

        Command OpenAttendeesCommand { get; set; }
        void OpenAttendeesAction(object param)
        {
            var controller = new AttendeesController(ViewModel.Event);
            NavigationController.PushViewController(controller, true);
        }

        Command OpenMapCommand { get; set; }
        void OpenMapAction(object obj)
        {
            var location = Uri.EscapeUriString(ViewModel.MapsQuery);
            var url = new NSUrl("http://maps.apple.com/?q=" + location);
            UIApplication.SharedApplication.OpenUrl(url);
        }

        Command OpenAgendaCommand { get; set; }
        void OpenAgendaAction(object param)
        {
            var controller = new AgendaController(ViewModel.Event);
            NavigationController.PushViewController(controller, true);
        }

        public Command AddExpenseButtonClickCommand { get; private set; }
        void AddExpenseButtonClickAction(object arg)
        {
            UIAlertController alert = UIAlertController.Create(L10n.Localize("AddExpenseAlertTitle", "New expense"), null, UIAlertControllerStyle.Alert);
            alert.AddTextField((UITextField textField) =>
            {
                textField.Placeholder = L10n.Localize("AddExpenseTextFieldPlaceholder", "Expense value");
                textField.KeyboardType = UIKeyboardType.DecimalPad;
            });
            alert.AddTextField((UITextField textField) => textField.Placeholder = L10n.Localize("AddExpenseDescriptionTextFieldPlaceholder", "Description"));

            UIAlertAction alertActionCancel = UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null);
            UIAlertAction alertActionAdd = UIAlertAction.Create(L10n.Localize("Add", "Add"), UIAlertActionStyle.Default, (alertAction) =>
             {
                 decimal newExpense;
                 if (Decimal.TryParse(alert.TextFields[0].Text, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out newExpense) && newExpense >= 0)
                     ViewModel.AddExpense(newExpense, alert.TextFields[1].Text);
                 else
                 {
                     alert.DismissViewController(true, () => { });
                     UIAlertController errorAlert = UIAlertController.Create(L10n.Localize("Error", "Error"), L10n.Localize("AddExpenseErrorAlert", "Input valid expense value"), UIAlertControllerStyle.Alert);
                     UIAlertAction alertActionOk = UIAlertAction.Create(L10n.Localize("Ok", "Ok"), UIAlertActionStyle.Cancel, null);
                     errorAlert.AddAction(alertActionOk);
                     PresentViewController(errorAlert, true, null);
                 }
             });
            alert.AddAction(alertActionCancel);
            alert.AddAction(alertActionAdd);
            PresentViewController(alert, true, null);
        }

        #endregion
    }
}
