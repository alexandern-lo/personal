using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Widget;
using Android.Views.InputMethods;
using ZXing.Mobile;
using StudioMobile;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Models;
using Android.Content;

namespace LiveOakApp.Droid.Controller
{
    public class LeadsFragment : CustomFragment
    {
        const string MODE_KEY = "mode";
        const string EMAIL_BODY_KEY = "body";
        const string RESOURCE_UIDS_KEY = "uids";
        const int leadEmailMode = 1;
        const int businessCardMode = 2;
        const int GUID_SIZE = 16;

        LeadsViewModel model;
        LeadsView view;

        AlertDialog selectEventDialog;

        bool IsFragmentVisible { get; set; } = false;

        Card ScannedCard { get; set; }

        public static LeadsFragment Create()
        {
            var fragment = new LeadsFragment();
            return fragment;
        }

        public static CustomFragment CreateForLeadEmails(List<Guid> resourceUids, string emailBody)
        {
            var fragment = new LeadsFragment();
            var args = new Bundle();
            byte[] guid_bytes = new byte[resourceUids.Count * GUID_SIZE]; 
            for (var i = 0; i < guid_bytes.Length; i++)
                guid_bytes[i] = resourceUids[i / GUID_SIZE].ToByteArray()[i % GUID_SIZE];
            args.PutByteArray(RESOURCE_UIDS_KEY, guid_bytes);
            args.PutInt(MODE_KEY, leadEmailMode);
            args.PutString(EMAIL_BODY_KEY, emailBody);
            fragment.Arguments = args;
            return fragment;
        }

        public static CustomFragment CreateToAttachBusinessCard()
        {
            var fragment = new LeadsFragment();
            var args = new Bundle();
            args.PutInt(MODE_KEY, businessCardMode);
            fragment.Arguments = args;
            return fragment;
        }

        public LeadsFragment()
        {
            Title = L10n.Localize("LeadsNavigationBarTitle", "Leads");
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            model = new LeadsViewModel();

            if (Arguments?.GetInt(MODE_KEY) == leadEmailMode)
            {
                SelectLeadCommand = new Command
                {
                    Action = SendResourcesAction
                };
            }
            else if (Arguments?.GetInt(MODE_KEY) == businessCardMode)
            {
                SelectLeadCommand = new Command
                {
                    Action = OpenLeadDetailsAndAttach
                };
            }
            else
            {
                SelectLeadCommand = new Command
                {
                    Action = OpenLeadDetailsAction
                };
            }

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
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = new LeadsView(inflater.Context);

            selectEventDialog = new AlertDialog.Builder(Context)
                                    .SetItems(new string[]
            {
                L10n.Localize("LeadsAnyEvent", "All Leads"),
                L10n.Localize("SelectEventTitle", "Select Event"),
            }, (sender, e) =>
            {
                switch (e.Which)
                {
                    case 0:
                        model.Event = EventViewModel.CreateAnyEvent();
                        selectEventDialog.Dismiss();
                        break;
                    case 1:
                        FragmentManager.BeginTransaction()
                                       .Replace(Resource.Id.fragment_container, EventsFragment.CreateForResult(model.SetEventCommand))
                                       .AddToBackStack("event-selector")
                                       .Commit();
                        selectEventDialog.Dismiss();
                        break;
                }
            })
                                    .Create();

            Bindings.Adapter(view.LeadList, view.PersonListAdapter(model.Leads));

            Bindings.Property(model, _ => _.SearchText)
                    .To(view.SearchText.TextProperty());

            Bindings.Property(model, _ => _.Event)
                    .Convert((_) => _ != null ? _?.Name : L10n.Localize("NoEvent", "No event"))
                    .To(view.EventView.TextProperty());

            Bindings.Property(model.LoadEventsCommand, _ => _.IsRunning)
                    .UpdateTarget((running) =>
                    {
                        view.EventsLoading = running.Value;
                    });

            Bindings.Property(model.LoadLeadsCommand, _ => _.IsRunning)
                    .UpdateTarget((running) =>
            {
                view.LeadListRefresher.Enabled = !running.Value;
                view.LeadsLoading = running.Value;
            });

            Bindings.Command(SelectLeadCommand)
                    .To(view.LeadList.ItemClickTarget())
                    .ParameterConverter((pos) => model.Leads[(int)pos]);

            Bindings.Command(CreateLeadCommand)
                    .To(view.ManualEntryButton.ClickTarget());

            Bindings.Command(ScanQRCodeCommand)
                    .To(view.QrBarCardScanButton.ClickTarget());

            Bindings.Command(ScanBusinessCardCommand)
                    .To(view.BusinessCardScanButton.ClickTarget());

            Bindings.Command(SelectEventCommand)
                    .To(view.EventView.ClickTarget());

            Bindings.Command(RunSearchCommand)
                    .ParameterConverter((actionId) => { return actionId.Equals(ImeAction.Search); })
                    .To(view.SearchText.EditorActionTarget());

            view.LeadListRefresher.Refresh += async (sender, e) =>
            {
                view.LeadListRefresher.Refreshing = false;
                try
                {
                    await model.LoadLeadsCommand.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    LOG.Error(ex.MessageForHuman());
                }
            };

            model.LoadLeadsCommand.Execute();

            if (Arguments?.GetInt(MODE_KEY) == businessCardMode)
            {
                view.BottomPanel.Visibility = ViewStates.Gone;
            }

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
            model.GetLeads().Ignore();
        }

        public override void OnPause()
        {
            base.OnPause();
            IsFragmentVisible = false;
        }

        #region Select Lead

        public Command SelectLeadCommand { get; private set; }

        void SendResourcesAction(object args)
        {
            var lead = args as LeadViewModel;
            var recipientsList = new List<string>();
            foreach (LeadEmailViewModel viewModel in lead.Emails)
                recipientsList.Add(viewModel.Email);
            
            var guid_bytes = Arguments.GetByteArray(RESOURCE_UIDS_KEY);
            List<Guid> guids = new List<Guid>();
            var buffer = new byte[GUID_SIZE];
            for (var i = 0; i < guid_bytes.Length; i += GUID_SIZE)
            {
                Array.Copy(guid_bytes, i, buffer, 0, GUID_SIZE);
                guids.Add(new Guid(buffer));
            }
            model.TrackResourceSentCommand.Execute(guids);
            MyResourcesFragment.doShare(Context, recipientsList.ToArray(), Arguments.GetString(EMAIL_BODY_KEY));
        }

        void OpenLeadDetailsAction(object clickedLead)
        {
            var lead = (LeadViewModel)clickedLead;
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container, LeadFragment.CreateWithLead(lead.Lead))
                           .AddToBackStack("lead-screen")
                           .Commit();
        }

        #endregion

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
                           .Replace(Resource.Id.fragment_container, LeadFragment.CreateFromCard(card, null))
                           .AddToBackStack("lead-screen")
                           .Commit();
        }

        #endregion

        #region Scan Business Card

        public Command ScanBusinessCardCommand { get; private set; }

        void ScanBusinessCardAction(object args)
        {
            var actions = new string[]
                {
                    L10n.Localize("CreateNewLead", "Create new lead"),
                    L10n.Localize("AddToExistingLead", "Add to existing lead")
                };
            var builder = new AlertDialog.Builder(Context);
            builder.SetItems(actions, (sender, e) =>
            {
                switch (e.Which)
                {
                    case 0:
                        CreateLeadAndAttach();
                        break;
                    case 1:
                        SelectLeadAndAttach();
                        break;
                }

            });
            builder.Create().Show();
        }

        void CreateLeadAndAttach()
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

        void SelectLeadAndAttach()
        {
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container,
                                    LeadsFragment.CreateToAttachBusinessCard()
                                   )
                           .AddToBackStack(null)
                           .Commit();
        }

        void OpenLeadDetailsAndAttach(object clicked)
        {
            var lead = (LeadViewModel)clicked;
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container, 
                                    new LeadFragment.Builder()
                                        .Lead(lead.Lead)
                                        .Action(LeadFragment.SCAN_BUSINESS_CARD_ACTION)
                                        .Build()
                                   )
                           .AddToBackStack("lead-screen")
                           .Commit();
        }

        #endregion

        #region Select Event

        public Command SelectEventCommand { get; private set; }

        void SelectEventAction(object arg)
        {
            selectEventDialog.Show();
        }

        #endregion

        #region Run Search

        public Command RunSearchCommand { get; private set; }

        void RunSearchAction(object isActionImeSearch)
        {
            if ((bool)isActionImeSearch)
            {
                model.SearchCommand.Execute();
                view.SearchText.ClearFocus();
                var imm = (InputMethodManager)Context.GetSystemService(Android.Content.Context.InputMethodService);
                imm.HideSoftInputFromWindow(view.SearchText.WindowToken, HideSoftInputFlags.None);
            }
        }

        #endregion
    }
}
