using System;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using StudioMobile;
using LiveOakApp.Resources;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Droid.Views;
using ZXing.Mobile;
using LiveOakApp.Models.Data.Entities;
using Android.Views.InputMethods;
using LiveOakApp.Models;
using Android.Support.V7.App;

namespace LiveOakApp.Droid.Controller
{
	public class RecentFragment : CustomFragment
	{

        public static RecentFragment Create() 
        {
            var fragment = new RecentFragment();
            return fragment;
        }

        private RecentActivityViewModel viewModel;
        private RecentActivityView view;

        bool IsFragmentVisible { get; set; } = false;

        Card ScannedCard { get; set; }

        public RecentFragment()
        {
            Title = L10n.Localize("MenuRecentActivity", "Recent activity");
            viewModel = new RecentActivityViewModel();
        }

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SelectLeadCommand = new Command
            {
                Action = OpenLeadDetailsAction
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

            viewModel.LoadRecentActivityCommand.Execute();
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
            view = new RecentActivityView(Context);
            return view;
		}

        public override void OnViewCreated(View v, Bundle savedInstanceState)
        {
            base.OnViewCreated(v, savedInstanceState);

            Bindings.Adapter(view.LeadList, view.LeadActivityListAdapter(viewModel.RecentActivityItems));

            Bindings.Property(viewModel.LoadRecentActivityCommand, _ => _.IsRunning)
                    .UpdateTarget((running) =>
            {
                view.LeadActivityRefresher.Enabled = !running.Value;
                view.IsShowingProgressBar = running.Value;
                updateErrorView();
                
            });

            Bindings.Command(SelectLeadCommand)
                    .To(view.LeadList.ItemClickTarget())
                    .ParameterConverter((pos) => viewModel.RecentActivityItems[(int)pos]);

            Bindings.Command(CreateLeadCommand)
                    .To(view.ManualEntryButton.ClickTarget());

            Bindings.Command(ScanQRCodeCommand)
                    .To(view.QrBarCardScanButton.ClickTarget());

            Bindings.Command(ScanBusinessCardCommand)
                    .To(view.BusinessCardScanButton.ClickTarget());

            Bindings.Property(viewModel.LoadRecentActivityCommand, _ => _.Error)
                    .Convert((ex) => ex.MessageForHuman())
                    .To(view.ErrorView.MessageProperty)
                    .AfterTargetUpdate((a, b) =>
            {
                if (viewModel.LoadRecentActivityCommand.Error != null)
                    view.ShowError();
                else
                    view.HideError();
            });

            Bindings.Property(viewModel.RecentActivityItems, _ => _.Count)
                    .UpdateTarget((count) =>
            {
                updateErrorView();
            });

            view.LeadActivityRefresher.Refresh += async (sender, e) =>
            {
                view.LeadActivityRefresher.Refreshing = false;
                try
                {
                    await viewModel.LoadRecentActivityCommand.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    LOG.Error(ex.MessageForHuman());
                }
            };

        }

        private void updateErrorView()
        {
            if (viewModel.LoadRecentActivityCommand.IsRunning)
                view.HideError();
            else if (viewModel.RecentActivityItems.Count == 0)
            {
                view.ErrorView.Message = L10n.Localize("NoRecentActivityLabel", "No recent activity");
                view.ShowError();
            }
                
                
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
        }

        public override void OnPause()
        {
            base.OnPause();
            IsFragmentVisible = false;
        }

        #region Select Lead

        public Command SelectLeadCommand { get; private set; }

        void OpenLeadDetailsAction(object clickedLead)
        {
            var lead = (LeadRecentActivityViewModel)clickedLead;
            if (LeadActionType.Deleted.Equals(lead.PerformedAction))
                return;
            //todo implement
            LOG.Debug("Open lead with id " + lead.LeadUid);
            return;
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
            try
            {
                MobileBarcodeScanner.Initialize(Activity.Application);
                var scanner = new MobileBarcodeScanner();
                scanner.CancelButtonText = L10n.Localize("Cancel", "Cancel");
                scanner.FlashButtonText = L10n.Localize("Flash", "Flash");
                var scanResult = await scanner.Scan();
                var vCardParser = new VCardParserViewModel(scanResult);
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
            }
            catch (Exception error)
            {
                LOG.Error("Failed to parse QR code", error);
                Toast.MakeText(Context, L10n.Localize("InvalidQRCodeMessage", "Invalid QR code") + ": " + error.Message, ToastLength.Long).Show();
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

        #endregion
	}
}

