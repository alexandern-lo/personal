using System;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using LiveOakApp.iOS.View.Content;
using LiveOakApp.Models;
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;
using ZXing.Mobile;

namespace LiveOakApp.iOS.Controller.Content
{
    public class RecentActivityController : MenuContentController<RecentActivityView>, IUIImagePickerControllerDelegate
    {
        RecentActivityViewModel ViewModel = new RecentActivityViewModel();

        UIImagePickerController BusinessCardImagePickerController = new UIImagePickerController();

        public RecentActivityController(SlideController slideController) : base(slideController)
        {
            Title = L10n.Localize("MenuRecentActivity", "Recent activity");
            CreateLeadCommand = new Command()
            {
                Action = CreateLeadAction
            };
            OpenBarScannerCommand = new AsyncCommand()
            {
                Action = OpenBarScannerAction
            };
            AddBusinessCardCommand = new Command()
            {
                Action = AddBusinessCardAction
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            BusinessCardImagePickerController.Delegate = this;

            var tableBinding = View.GetTableBinding(ViewModel.RecentActivityItems);
            Bindings.Add(tableBinding);
            Bindings.Property(ViewModel.LoadRecentActivityCommand, _ => _.IsRunning)
                    .UpdateTarget((s) => { View.FetchRunning = s.Value; });
            Bindings.Command(CreateLeadCommand).To(View.ManualEntryTabBarButton.ClickTarget());
            Bindings.Command(OpenBarScannerCommand).To(View.BarScanTabBarButton.ClickTarget());
            Bindings.Command(AddBusinessCardCommand).To(View.CardScanTabBarButton.ClickTarget());
            Bindings.Command(ViewModel.LoadRecentActivityCommand).To(View.ErrorView.ReloadButton.ClickTarget());
            ViewModel.LoadRecentActivityCommand.Execute();

            Bindings.Property(ViewModel.LoadRecentActivityCommand, _ => _.Error)
                    .Convert(_ => _?.MessageForHuman())
                    .To(View.ErrorView.ErrorMessageLabel.TextProperty());
            Bindings.Property(ViewModel.LoadRecentActivityCommand, _ => _.IsRunning)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());
            Bindings.Property(ViewModel.LoadRecentActivityCommand, _ => _.Error)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());
            Bindings.Property(ViewModel.RecentActivityItems, _ => _.Count)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());

            View.RefreshControl.AddTarget((sender, e) =>
            {
                View.RefreshControl.EndRefreshing();
                ViewModel.LoadRecentActivityCommand.Execute();
            }, UIControlEvent.ValueChanged);
        }

        public void UpdateErrorAndWarningViews()
        {
            bool isError = ViewModel.LoadRecentActivityCommand.Error != null;
            bool isEmptyList = ViewModel.RecentActivityItems.Count == 0;
            bool fetchRunning = View.FetchRunning;

            View.ErrorView.Hidden = !isError || fetchRunning;
            View.MessageView.Hidden = isError || !isEmptyList || fetchRunning;

            if (isError)
            {
                View.ErrorView.ErrorMessageLabel.Text = ViewModel.LoadRecentActivityCommand.Error.MessageForHuman();
                View.ErrorView.SizeToFit();
            }
            else if (isEmptyList)
            {
                View.MessageView.MessageLabel.Text = L10n.Localize("NoRecentActivityLabel", "No recent activity");
                View.MessageView.SizeToFit();
            }
        }

        #region Actions
        AsyncCommand OpenBarScannerCommand { get; set; }
        async Task OpenBarScannerAction(object obj)
        {
            if (!this.CheckCameraPermissions()) return;
            UIAlertController loadingAlert = null;
            try
            {
                var scanner = new MobileBarcodeScanner(this);
                scanner.CancelButtonText = L10n.Localize("Cancel", "Cancel");
                scanner.FlashButtonText = L10n.Localize("Flash", "Flash");
                var scanResult = await scanner.Scan();
                var vCardParser = new VCardParserViewModel(scanResult);
                if (!vCardParser.IsUri)
                {
                    var card = vCardParser.Parse();
                    if (card != null) CreateLead(card);
                    return;
                }
                loadingAlert = UIAlertController.Create(L10n.Localize("DownloadingVCardMessage", "Downloading vCard file.."), "\n\n", UIAlertControllerStyle.Alert);
                UIActivityIndicatorView indicator = new UIActivityIndicatorView(loadingAlert.View.Bounds);
                indicator.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                indicator.Color = UIColor.Black;
                indicator.UserInteractionEnabled = false;
                indicator.StartAnimating();
                loadingAlert.View.AddSubview(indicator);
                loadingAlert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, (obj1) => OpenBarScannerCommand.CancelCommand.Execute()));
                PresentViewController(loadingAlert, true, null);

                var vCardByteArray = await ViewModel.TryDownloadVCard(vCardParser.ScanningResult.Text.TryParseWebsiteUri()?.AbsoluteUri, OpenBarScannerCommand.Token);
                var vCardString = System.Text.Encoding.UTF8.GetString(vCardByteArray);
                var downloadedCard = vCardParser.Parse(vCardString);
                loadingAlert.DismissViewController(true, null);
                if (downloadedCard != null) CreateLead(downloadedCard);
            }
            catch (Exception ex)
            {
                LOG.Error("Failed to parse QR code", ex);
                if (ex is TaskCanceledException) return;
                var errorText = ex.MessageForHuman();
                var alert = UIAlertController.Create(L10n.Localize("InvalidQRCodeMessage", "Invalid QR code"), errorText, UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
                if (loadingAlert != null && !loadingAlert.IsBeingDismissed) loadingAlert.DismissViewController(true, () =>
                {
                    PresentViewController(alert, true, null);
                });
                else PresentViewController(alert, true, null);
            }
        }

        Command AddBusinessCardCommand { get; set; }
        void AddBusinessCardAction(object arg)
        {
            UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);
            alert.AddAction(UIAlertAction.Create(L10n.Localize("TakePhotoAlertAction", "Take a photo"), UIAlertActionStyle.Default, (obj1) =>
                 {
                     CardScanerController cardScanner = new CardScanerController((cardPhoto) =>
            {
                   DismissViewController(true, null);
                   CheckCardPhotoAndAddToLead(cardPhoto);
               }, () => DismissViewController(true, null));
                     var navController = new UINavigationController(cardScanner);
                     PresentViewController(navController, true, null);
                 }));
            alert.AddAction(UIAlertAction.Create(L10n.Localize("ChoosePhotoAlertAction", "Choose photo"), UIAlertActionStyle.Default, (obj1) =>
                 {
                     if (!this.CheckPhotosPermissions()) return;
                     BusinessCardImagePickerController.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                     PresentViewController(BusinessCardImagePickerController, true, null);
                 }));
            alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }

        void CheckCardPhotoAndAddToLead(UIImage cardPhoto)
        {
            var alert = UIAlertController.Create(L10n.Localize("NewBusinessCardPhotoLabel", "New business card photo"), null, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create(L10n.Localize("CreateNewLead", "Create new lead"), UIAlertActionStyle.Default, (obj) => CreateLead(cardPhoto)));
            alert.AddAction(UIAlertAction.Create(L10n.Localize("AddToExistingLead", "Add to existing lead"), UIAlertActionStyle.Default, (obj) =>
            {
                LeadsController chooseLeadController = null;
                chooseLeadController = new LeadsController((leadViewModel) =>
    {
                chooseLeadController.DismissViewController(true, null);
                AddBusinessCardToLead(leadViewModel, cardPhoto);
            }, () => chooseLeadController.DismissViewController(true, null));
                var navController = new UINavigationController(chooseLeadController);
                PresentViewController(navController, true, null);
            }));
            alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }

        [Export("imagePickerController:didFinishPickingImage:editingInfo:")]
        public void FinishedPickingImage(UIImagePickerController picker, UIImage image, NSDictionary editingInfo)
        {
            DismissViewController(true, null);
            CheckCardPhotoAndAddToLead(image);
        }

        Command CreateLeadCommand { get; set; }
        void CreateLeadAction(object obj)
        {
            CreateLead();
        }

        #endregion

        void CreateLead()
        {
            var leadDetailsController = new LeadDetailsController(() => NavigationController.PopToViewController(this, true));
            NavigationController.PushViewController(leadDetailsController, true);
        }

        void CreateLead(Card card)
        {
            var leadDetailsController = new LeadDetailsController(card, null, () => NavigationController.PopToViewController(this, true));
            NavigationController.PushViewController(leadDetailsController, true);
        }

        void CreateLead(UIImage businessCardPhoto)
        {
            var leadDetailsController = new LeadDetailsController(businessCardPhoto, () => NavigationController.PopToViewController(this, true));
            NavigationController.PushViewController(leadDetailsController, true);
        }

        void AddBusinessCardToLead(LeadViewModel leadViewModel, UIImage businessCardPhoto)
        {
            var leadDetailsController = new LeadDetailsController(leadViewModel, businessCardPhoto, () => NavigationController.PopToViewController(this, true));
            NavigationController.PushViewController(leadDetailsController, true);
        }
    }
}

