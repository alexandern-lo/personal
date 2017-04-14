using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using System;
using System.Threading.Tasks;
using ServiceStack;
using ZXing.Mobile;
using StudioMobile;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using LiveOakApp.Models;
using LiveOakApp.Models.Data.Entities;
using Android.Support.V7.App;

namespace LiveOakApp.Droid.Controller
{
    public class AttendeesFragment : CustomFragment
    {
        const string EXTRA_EVENT = "EXTRA_EVENT";

        bool IsFragmentVisible { get; set; } = false;

        Card ScannedCard { get; set; }

        public static Fragment CreateForEvent(EventViewModel @event)
        {
            var args = new Bundle();

            args.PutString(EXTRA_EVENT, @event.EventToJson());

            var fragment = new AttendeesFragment();
            fragment.Arguments = args;

            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var _event = EventViewModel.JsonToEvent(Arguments.GetString(EXTRA_EVENT));

            model = new AttendeesViewModel(_event);

            OpenAttendeeDetailsCommand = new Command
            {
                Action = OpenAttendeeDetailsAction
            };

            OpenFilterScreenCommand = new Command
            {
                Action = OpenFilterScreenAction
            };

            CreateLeadCommand = new Command
            {
                Action = CreateLeadAction
            };

            ScanQRCodeCommand = new AsyncCommand
            {
                Action = ScanQRCodeAction
            };

            ScanBusinessCardCommand = new Command
            {
                Action = ScanBusinessCardAction
            };

            SelectEventCommand = new Command
            {
                Action = SelectEventAction
            };

            RunSearchCommand = new Command
            {
                Action = RunSearchAction
            };

            model.LoadEventsCommand.Execute();

            Title = L10n.Localize("AttendeesNavigationBarTitle", "Attendees");
        }

        AttendeesViewModel model;
        AttendeesView view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = new AttendeesView(inflater.Context);

            Bindings.Adapter(view.AttendeesList, view.PersonListAdapter(model.Persons));

            Bindings.Property(model, _ => _.SearchText)
                    .To(view.SearchText.TextProperty());

            Bindings.Property(model.LoadEventsCommand, _ => _.IsRunning)
                    .UpdateTarget((running) =>
            {
                view.EventsLoading = running.Value;
            });

            Bindings.Property(model.SearchCommand, _ => _.IsRunning)
                    .UpdateTarget((running) =>
            {
                view.AttendeesListRefresher.Enabled = !running.Value;
                view.AttendeesLoading = running.Value;
            });

            Bindings.Property(model, _ => _.Event)
                    .Convert((_) => _ != null ? _.Name : L10n.Localize("NoEvent", "No event"))
                    .To(view.EventView.TextProperty());

            Bindings.Command(OpenAttendeeDetailsCommand)
                    .To(view.AttendeesList.ItemClickTarget())
                    .ParameterConverter((pos) => model.Persons[(int)pos]);

            Bindings.Command(OpenFilterScreenCommand)
                    .To(view.FilterButton.ClickTarget());

            Bindings.Command(CreateLeadCommand)
                    .To(view.ManualEntryButton.ClickTarget());

            Bindings.Command(ScanQRCodeCommand)
                    .To(view.QrBarCardScanButton.ClickTarget());

            Bindings.Command(ScanBusinessCardCommand)
                    .To(view.BusinessCardScanButton.ClickTarget());

            Bindings.Command(SelectEventCommand)
                    .To(view.EventView.ClickTarget());

            view.OnAttendeeAtIndexWillBeShown = (index) => model.LoadNextPageIfNeeded(index);

            Bindings.Property(model.LoadNextPageCommand, _ => _.IsRunning)
                    .UpdateTarget((isRunning) => view.SetFooterProgressBarEnabled(isRunning.Value));

            Bindings.Command(RunSearchCommand)
                    .ParameterConverter((actionId) => { return actionId.Equals(ImeAction.Search); })
                    .To(view.SearchText.EditorActionTarget());

            Bindings.Property(model.SearchCommand, _ => _.Error)
                    .Convert((ex) => ex.MessageForHuman())
                    .To(view.ErrorMessage.MessageProperty)
                    .AfterTargetUpdate((a, b) =>
            {
                if (model.SearchCommand.Error != null)
                    view.ShowErrorWithButton();
                else
                    view.HideError();
            });

            Bindings.Property(model.SearchCommand, _ => _.IsRunning)
                    .UpdateTarget((isRunning) =>
            {
                if (isRunning.Value)
                    view.HideError();
            });

            Bindings.Property(model.Persons, _ => _.Count)
                    .UpdateTarget((count) =>
            {
                if (count.Value > 0)
                {
                    view.HideError();
                    return;
                }
                view.ErrorMessage.Message = model.AreFiltersEnabled || !model.SearchText.IsEmpty() ? L10n.Localize("NoMatchesLabel", "No matches found") : L10n.Localize("NoAttendeesLabel", "No attendees");
                view.ShowError();
            });

            Bindings.Command(model.SearchCommand)
                    .To(view.ErrorMessage.ActionButtonClickTarget);

            view.AttendeesListRefresher.Refresh += async (sender, e) =>
            {
                view.AttendeesListRefresher.Refreshing = false;
                try
                {
                    await model.SearchCommand.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    LOG.Error(ex.MessageForHuman());
                }
            };

			return view;
		}

        public override void OnResume()
        {
            base.OnResume();
            IsFragmentVisible = true;
            if (ScannedCard != null)
            {
                var card = ScannedCard;
                ScannedCard = null;
                CreateLeadFromCard(card);
            }
            model.ExecuteSearchIfNeeded();
        }

        public override void OnPause()
        {
            base.OnPause();
            IsFragmentVisible = false;
        }

        #region Create Lead

        public Command CreateLeadCommand { get; private set; }

        void CreateLeadAction(object args)
        {
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container, new LeadFragment())
                           .AddToBackStack("lead-screen")
                           .Commit();
        }

        #endregion

        #region Scan QR Code

        public AsyncCommand ScanQRCodeCommand { get; private set; }

        async Task ScanQRCodeAction(object args)
        {

            AlertDialog progressDialog = null;
            try
            {
                MobileBarcodeScanner.Initialize(Activity.Application);
                var scanner = new MobileBarcodeScanner();
                scanner.CancelButtonText = L10n.Localize("Cancel", "Cancel");
                scanner.FlashButtonText = L10n.Localize("Flash", "Flash");
                var scanResult = await scanner.Scan();
                var vCardParser = new VCardParserViewModel(scanResult);
                if (!vCardParser.IsUri)
                {
                    var card = vCardParser.Parse();
                    if (card == null)
                    {
                        return;
                    }
                    if (!IsFragmentVisible)
                    {
                        ScannedCard = card;
                        return;
                    }
                    CreateLeadFromCard(card);
                    return;
                }


                progressDialog = new AlertDialog.Builder(Context)
                                                    .SetTitle(L10n.Localize("DownloadingVCardMessage", "Downloading vCard file.."))
                                                    .SetNeutralButton(L10n.Localize("Cancel", "Cancel"), (sender, e) => { ScanQRCodeCommand.CancelCommand.Execute(); })
                                                    .SetView(new ProgressBar(Context))
                                                    .SetCancelable(false)
                                                    .Create();
                progressDialog.Show();

                var vCardByteArray = await model.TryDownloadVCard(vCardParser.ScanningResult.Text.TryParseWebsiteUri()?.AbsoluteUri, ScanQRCodeCommand.Token);
                var vCardString = System.Text.Encoding.UTF8.GetString(vCardByteArray);
                var downloadedCard = vCardParser.Parse(vCardString);

                progressDialog.Cancel();

                if (downloadedCard == null)
                {
                    return;
                }
                if (!IsFragmentVisible)
                {
                    ScannedCard = downloadedCard;
                    return;
                }
                CreateLeadFromCard(downloadedCard);
            }
            catch (Exception error)
            {
                LOG.Error("Failed to parse QR code", error);
                if (error is TaskCanceledException) return;
                if (progressDialog != null && progressDialog.IsShowing)
                {
                    progressDialog.Cancel();
                }
                var errorText = error.MessageForHuman();
                AlertDialog dialog = new AlertDialog.Builder(Context)
                                                    .SetTitle(L10n.Localize("InvalidQRCodeMessage", "Invalid QR code"))
                                                    .SetPositiveButton(L10n.Localize("Cancel", "Cancel"), (sender, e) => { })
                                                    .SetMessage(errorText)
                                                    .Show();
            }
        }

        void CreateLeadFromCard(Card card)
        {
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container, LeadFragment.CreateFromCard(card, model.Event))
                           .AddToBackStack("lead-screen")
                           .Commit();
        }

        #endregion

        #region Scan Business Card

        public Command ScanBusinessCardCommand { get; private set; }

        void ScanBusinessCardAction(object args)
        {
            string[] actions = new string[]
                {
                    L10n.Localize("CreateNewLead", "Create new lead"),
                    L10n.Localize("AddToExistingLead", "Add to existing lead")
                };
            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            builder.SetItems(actions, (sender, e) =>
            {
                switch (e.Which)
                {
                    case 0:
                        createLeadAndAttach();
                        break;
                    case 1:
                        selectLeadAndAttach();
                        break;
                }

            });
            builder.Create().Show();
        }

        void createLeadAndAttach()
        {
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container,
                                    new LeadFragment.Builder()
                                        .Action(LeadFragment.SCAN_BUSINESS_CARD_ACTION)
                                        .Build()
                                   )
                           .AddToBackStack("lead-screen")
                           .Commit();
        }

        void selectLeadAndAttach()
        {
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container,
                                    LeadsFragment.CreateToAttachBusinessCard()
                                   )
                           .AddToBackStack(null)
                           .Commit();
        }

        #endregion

        #region Select Attendee

        public Command OpenAttendeeDetailsCommand { get; private set; }

        void OpenAttendeeDetailsAction(object clickedAttendee)
        {
            var attendee = (AttendeeViewModel)clickedAttendee;
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container, AttendeesDetailsFragment.CreateForAttendee(attendee, model.Event))
                           .AddToBackStack("attendee-screen")
                           .Commit();
        }

        #endregion

        #region Open Filters

        public Command OpenFilterScreenCommand { get; private set; }

        void OpenFilterScreenAction(object args)
        {
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container, AttendeesFilterFragment.CreateForEvent(model.Event))
                           .AddToBackStack("filter-screen")
                           .Commit();
        }

        #endregion

        #region Select Event

        public Command SelectEventCommand { get; private set; }

        void SelectEventAction(object arg)
        {
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container, EventsFragment.CreateForResult(model.SetEventCommand))
                           .AddToBackStack("event-selector")
                           .Commit();
        }

        #endregion

        #region Run Search

        public Command RunSearchCommand { get; private set; }

        void RunSearchAction(object isActionImeSearch)
        {
            if ((bool)isActionImeSearch)
            {
                model.SearchCommand.ExecuteWithoutDelay(null).Ignore();
                view.SearchText.ClearFocus();
                var imm = (InputMethodManager)Context.GetSystemService(Android.Content.Context.InputMethodService);
                imm.HideSoftInputFromWindow(view.SearchText.WindowToken, HideSoftInputFlags.None);
            }
        }

        #endregion
    }
}
