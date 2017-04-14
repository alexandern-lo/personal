using System;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using ZXing.Mobile;
using StudioMobile;
using LiveOakApp.iOS.View.Content;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Resources;
using System.IO;
using System.Net.Http;
using LiveOakApp.Models;
using CoreGraphics;
using AVFoundation;

namespace LiveOakApp.iOS.Controller.Content
{
    public class LeadsController : MenuContentController<LeadsView>, IUITextFieldDelegate, IUIImagePickerControllerDelegate
    {
        LeadsViewModel ViewModel { get; set; }

        Action<LeadViewModel> OnLeadSelectedAction { get; set; }
        UIImagePickerController BusinessCardImagePickerController = new UIImagePickerController();
        bool isLeadSelectionMode;

        public LeadsController(Action<LeadViewModel> onLeadSelected, Action onCancelled) // constructor for Lead choosing mode
        {
            Initialize();
            Title = L10n.Localize("LeadsSelectLead", "Select Lead");
            isLeadSelectionMode = true;
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (sender, e) => onCancelled());
            OnLeadSelectedAction = onLeadSelected;
        }

        public LeadsController(SlideController slideController) : base(slideController)
        {
            Initialize();
            Title = L10n.Localize("MenuLeads", "Leads");
            isLeadSelectionMode = false;
            NavigationItem.BackBarButtonItem = CommonSkin.BackBarButtonItem;
            OnLeadSelectedAction = (LeadViewModel lead) =>
            {
                var controller = new LeadDetailsController(lead, () => NavigationController.PopToViewController(this, true));
                NavigationController.PushViewController(controller, true);
            };
        }

        void Initialize()
        {
            ViewModel = new LeadsViewModel();
            SelectEventCommand = new Command
            {
                Action = OpenEventsAction
            };

            OpenLeadCommand = new Command
            {
                Action = OpenLeadAction
            };

            CreateLeadCommand = new Command
            {
                Action = CreateLeadAction
            };

            OpenBarScannerCommand = new AsyncCommand
            {
                Action = OpenBarScannerAction
            };

            AddBusinessCardCommand = new Command
            {
                Action = AddBusinessCardAction
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.IsPersonSelectionMode = isLeadSelectionMode;
            View.MainPartView.SearchTextField.Delegate = this;
            BusinessCardImagePickerController.Delegate = this;

            var leadsBinding = View.GetPersonsBinding(ViewModel.Leads);
            Bindings.Add(leadsBinding);
            Bindings.Property(ViewModel, _ => _.Event).UpdateTarget(e =>
            {
                if (e.Value == null) View.EventButtonTitle = L10n.Localize("NoEvent", "No Event");
                else View.EventButtonTitle = e.Value.Name;
            });
            Bindings.Property(ViewModel, _ => _.SearchText).To(View.MainPartView.SearchTextField.TextProperty());
            Bindings.Property(ViewModel.LoadLeadsCommand, _ => _.IsRunning).UpdateTarget((s) => View.FetchRunning = s.Value);
            Bindings.Command(OpenBarScannerCommand).To(View.BarScanTabBarButton.ClickTarget());
            Bindings.Command(AddBusinessCardCommand).To(View.CardScanTabBarButton.ClickTarget());
            Bindings.Command(OpenLeadCommand).To(leadsBinding.ItemSelectedTarget());
            Bindings.Command(SelectEventCommand).To(View.MainPartView.SelectEventButton.ClickTarget());
            Bindings.Command(CreateLeadCommand).To(View.ManualEntryTabBarButton.ClickTarget());
            ViewModel.LoadLeadsCommand.Execute();

            View.MainPartView.RefreshControl.AddTarget((sender, e) =>
            {
                View.MainPartView.RefreshControl.EndRefreshing();
                ViewModel.LoadLeadsCommand.Execute();
            }, UIControlEvent.ValueChanged);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ViewModel.GetLeads().Ignore();
        }

        #region TextFieldDelegate

        [Export("textFieldShouldReturn:")]
        public bool ShouldReturn(UITextField textField)
        {
            ViewModel.SearchCommand.Execute();
            View.EndEditing(true);
            return true;
        }

        #endregion

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
                loadingAlert.DismissViewController(true, () =>
                {
                    if (downloadedCard != null)
                        CreateLead(downloadedCard);
                });

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

        Command OpenLeadCommand { get; set; }
        void OpenLeadAction(object arg)
        {
            var lead = (LeadViewModel)arg;
            OnLeadSelectedAction(lead);
        }

        Command SelectEventCommand { get; set; }
        void OpenEventsAction(object obj)
        {
            var alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);
            alert.AddAction(UIAlertAction.Create(L10n.Localize("LeadsAnyEvent", "All Leads"), UIAlertActionStyle.Default, o =>
            {
                ViewModel.Event = EventViewModel.CreateAnyEvent();
            }));

            alert.AddAction(UIAlertAction.Create(L10n.Localize("SelectEventTitle", "Select Event"), UIAlertActionStyle.Default, o =>
            {
                var eventsController = new EventsController((EventViewModel eventArg) =>
                {
                    ViewModel.Event = eventArg;
                    NavigationController.PopViewController(true);
                });
                NavigationController.PushViewController(eventsController, true);
            }));

            alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }

        Command CreateLeadCommand { get; set; }
        void CreateLeadAction(object obj)
        {
            CreateLead();
        }

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
        #endregion
    }
}
