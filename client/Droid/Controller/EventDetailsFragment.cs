using System;
using System.Globalization;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;

namespace LiveOakApp.Droid.Controller
{
    public class EventDetailsFragment : CustomFragment
    {
        const string EXTRA_EVENT = "EXTRA_EVENT";

        EventDetailsView view;
        EventDetailsViewModel model;

        public static Fragment CreateForEvent(EventViewModel @event)
        {
            var args = new Bundle();

            args.PutString(EXTRA_EVENT, @event.EventToJson());

            var fragment = new EventDetailsFragment();
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var _event = EventViewModel.JsonToEvent(Arguments.GetString(EXTRA_EVENT));

            model = new EventDetailsViewModel(_event);

            OpenUrlCommand = new Command
            {
                Action = OpenUrlAction
            };
            OpenAttendeesCommand = new Command
            {
                Action = OpenEventAttendees
            };
            OpenEventAgendaCommand = new Command
            {
                Action = OpenEventAgenda
            };
            AddExpenseCommand = new Command
            {
                Action = AddExpenseAction
            };
            OpenMapCommand = new Command
            {
                Action = OpenMapAction,
                CanExecute = (o) => model.Location != null && model.Location.Trim() != string.Empty,
            };

            Title = model.Event.Name;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = new EventDetailsView(inflater.Context);

            addExpenseDialog = CreateAddExpenseDialog();
            parseExpenseErrorDialog = CreateExpenseErrorDialog();

            model.LoadTotalExpensesCommand.Execute();

            Bindings.Property(model, _ => _.Type)
                    .To(view.EventType.TextProperty());
            Bindings.Property(model, _ => _.Industry)
                    .To(view.EventIndustry.TextProperty());
            Bindings.Property(model, _ => _.Date)
                    .To(view.EventDate.TextProperty());
            Bindings.Property(model, _ => _.Location)
                    .To(view.EventLocation.TextProperty());
            Bindings.Property(model, _ => _.WebsiteUrlString)
                    .To(view.EventUrl.TextProperty());
            Bindings.Property(model.TotalExpenses, _ => _.Amount)
                    .Convert((amount) => model.TotalExpenses.GetCurrencySymbol() + (int)amount)
                    .To(view.EventExpenses.TextProperty());

            Bindings.Property(model, _ => _.EventLogo)
                    .To(view.EventLogo.ImageProperty());

            Bindings.Command(AddExpenseCommand)
                    .To(view.AddExpensesButton.ClickTarget());
            Bindings.Command(OpenUrlCommand)
                    .To(view.EventUrl.ClickTarget());
            Bindings.Command(OpenAttendeesCommand)
                    .To(view.EventAttendees.ClickTarget());
            Bindings.Command(OpenEventAgendaCommand)
                    .To(view.EventAgenda.ClickTarget());
            Bindings.Command(OpenMapCommand)
                    .To(view.EventLocation.ClickTarget());

            Bindings.Property(model, _ => _.IsReccuring)
                    .UpdateTarget((reccuring) => 
            {
                if (reccuring.Value)
                    view.ShowReccuringStatus();
                else
                    view.HideReccuringStatus();
            });
            Bindings.Property(model, _ => _.TypeEnum)
                    .UpdateTarget((type) => 
            {
                if (type.Value == EventViewModel.EventType.Conference)
                    view.ShowAgendaAndAttendeesButtons();
                else
                    view.HideAgendaAndAttendeesButtons();
            });

            return view;
        }

        #region Actions

        Command AddExpenseCommand { get; set; }
        void AddExpenseAction(object arg) 
        {
            try
            {
                addExpenseDialog.Show();
            }
            catch (ObjectDisposedException ex)
            {
                addExpenseDialog = CreateAddExpenseDialog();
                addExpenseDialog.Show();
            }
        }

        Command OpenUrlCommand { get; set; }

        void OpenUrlAction(object param)
        {
            var url = model.WebsiteUri.ToAndroidUri();
            if (url != null)
            {
                var intent = new Intent(Intent.ActionView, url);
                Activity.StartActivity(intent);
            }
        }

        Command OpenAttendeesCommand { get; set; }

        void OpenEventAttendees(object param)
        {
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container, AttendeesFragment.CreateForEvent(model.Event))
                           .AddToBackStack("attendees-screen")
                           .Commit();
        }

        Command OpenEventAgendaCommand { get; set; }

        void OpenEventAgenda(object param)
        {
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container, AgendaFragment.Create(model.Event))
                           .AddToBackStack("agenda-screen")
                           .Commit();
        }

        Command OpenMapCommand { get; set; }
        void OpenMapAction(object arg) 
        {
            var location = Uri.EscapeUriString(model.MapsQuery);
            var mapUri = Android.Net.Uri.Parse("geo:0,0?q=" + location);
            var mapIntent = new Intent(Intent.ActionView, mapUri);
            mapIntent.SetPackage("com.google.android.apps.maps");
            if (Context.PackageManager.ResolveActivity(mapIntent, Android.Content.PM.PackageInfoFlags.MatchAll) != null)
                Activity.StartActivity(mapIntent);
            else
                Toast.MakeText(Context, L10n.Localize("MapError", "Map is unavailable"), ToastLength.Short);
        }

        #endregion

        #region Dialogs

        AlertDialog addExpenseDialog;
        AlertDialog CreateAddExpenseDialog()
        {
            var editDescriptionView = new EditText(Context);
            editDescriptionView.Hint = L10n.Localize("AddExpenseDescriptionTextFieldPlaceholder", "Description");
            var editExpenseView = new EditText(Context);
            editExpenseView.Hint = L10n.Localize("AddExpenseTextFieldPlaceholder", "Expense value");
            editExpenseView.InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagDecimal;
            var verticalLayout = new LinearLayout(Context);
            verticalLayout.Orientation = Orientation.Vertical;
            verticalLayout.AddView(editExpenseView);
            verticalLayout.AddView(editDescriptionView);
            return new AlertDialog.Builder(Context)
                                  .SetTitle(L10n.Localize("AddExpenseAlertTitle", "New expense"))
                                  .SetView(verticalLayout)
                                  .SetPositiveButton(L10n.Localize("Add", "Add"), (sender, e) =>
            {
                addExpenseDialog.Dismiss();
                decimal newExpense;
                if (Decimal.TryParse(editExpenseView.Text, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out newExpense) && newExpense >= 0)
                    model.AddExpense(newExpense, editDescriptionView.Text);
                else
                {
                    try
                    {
                        parseExpenseErrorDialog.Show();
                    }
                    catch (ObjectDisposedException)
                    {
                        parseExpenseErrorDialog = CreateExpenseErrorDialog();
                        parseExpenseErrorDialog.Show();
                    }
                }
            })
                                  .SetNegativeButton(L10n.Localize("Cancel", "Cancel"), (sender, e) =>
            {
                addExpenseDialog.Dismiss();
            })
                                  .Create();
        }
        AlertDialog parseExpenseErrorDialog;
        AlertDialog CreateExpenseErrorDialog()
        {
            return new AlertDialog.Builder(Context)
                                  .SetTitle(L10n.Localize("AddExpenseErrorAlert", "Input valid expense value"))
                                  .SetPositiveButton(L10n.Localize("Ok", "OK"), (sender, e) => parseExpenseErrorDialog.Dismiss())
                                  .Create();
        }
        #endregion
    }
}
