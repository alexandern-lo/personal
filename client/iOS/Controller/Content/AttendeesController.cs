using System;
using System.Threading.Tasks;
using UIKit;
using Foundation;
using ZXing.Mobile;
using StudioMobile;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.iOS.View.Content;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Resources;
using LiveOakApp.Models;
using LiveOakApp.Models.Data.Entities;
using AVFoundation;

namespace LiveOakApp.iOS.Controller.Content
{
    public class AttendeesController : CustomController<AttendeesView>, IUITextFieldDelegate, IUIImagePickerControllerDelegate
    {
        readonly AttendeesViewModel ViewModel;

        UIImagePickerController BusinessCardImagePickerController = new UIImagePickerController();

        public AttendeesController(EventViewModel eventItem)
        {
            Title = L10n.Localize("AttendeesNavigationBarTitle", "Attendees");
            ViewModel = new AttendeesViewModel(eventItem);
            ViewModel.LoadEventsCommand.Execute();

            OpenFilterCommand = new Command
            {
                Action = OpenFilterAction
            };
            OpenEventsSelectionCommand = new Command
            {
                Action = OpenEventsSelectionAction
            };
            OpenAttendeeCommand = new Command
            {
                Action = OpenAttendeeAction
            };

            CreateLeadCommand = new Command
            {
                Action = CreateLeadAction
            };

            OpenQRBarcodeScannerCommand = new AsyncCommand
            {
                Action = OpenQRBarcodeScannerAction
            };
            AddBusinessCardCommand = new Command
            {
                Action = AddBusinessCardAction
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationItem.BackBarButtonItem = CommonSkin.BackBarButtonItem;

            BusinessCardImagePickerController.Delegate = this;
            View.MainPartView.SearchTextField.Delegate = this;
            var sectionsBinding = View.GetPersonsBinding(ViewModel.Persons);
            Bindings.Property(ViewModel, _ => _.Event).UpdateTarget(e => { View.EventButtonTitle = e.Value.Name; });
            Bindings.Property(ViewModel, _ => _.SearchText).To(View.MainPartView.SearchTextField.TextProperty());
            Bindings.Property(ViewModel.LoadEventsCommand, _ => _.IsRunning).UpdateTarget((s) => View.FetchRunning = s.Value);
            Bindings.Property(ViewModel.SearchCommand, _ => _.IsRunning).UpdateTarget((s) => { View.SearchRunning = s.Value; });
            Bindings.Property(ViewModel.LoadNextPageCommand, _ => _.IsRunning).UpdateTarget((s) => View.MainPartView.SlideUpFooter(s.Value, true));
            Bindings.Command(OpenAttendeeCommand).To(sectionsBinding.ItemSelectedTarget());
            Bindings.Command(ViewModel.SearchCommand).To(View.ErrorView.ReloadButton.ClickTarget());
            Bindings.Command(OpenFilterCommand).To(View.MainPartView.FilterButton.ClickTarget());
            Bindings.Command(OpenEventsSelectionCommand).To(View.MainPartView.SelectEventButton.ClickTarget());
            Bindings.Command(OpenQRBarcodeScannerCommand).To(View.BarScanTabBarButton.ClickTarget());
            Bindings.Command(AddBusinessCardCommand).To(View.CardScanTabBarButton.ClickTarget());
            Bindings.Command(CreateLeadCommand).To(View.ManualEntryTabBarButton.ClickTarget());
            Bindings.Add(sectionsBinding);

            Bindings.Property(ViewModel.SearchCommand, _ => _.IsRunning)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());
            Bindings.Property(ViewModel.SearchCommand, _ => _.Error)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());
            Bindings.Property(ViewModel.Persons, _ => _.Count)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());

            View.OnAttendeeAtIndexWillBeShown = (index) => ViewModel.LoadNextPageIfNeeded(index);

            View.MainPartView.RefreshControl.AddTarget((sender, e) =>
           {
               View.MainPartView.RefreshControl.EndRefreshing();
               ViewModel.SearchCommand.Execute();
           }, UIControlEvent.ValueChanged);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ViewModel.ExecuteSearchIfNeeded();
        }

        public void UpdateErrorAndWarningViews()
        {
            bool isError = ViewModel.SearchCommand.Error != null;
            bool isEmptyList = ViewModel.Persons.Count == 0;
            bool fetchRunning = View.SearchRunning;

            View.ErrorView.Hidden = !isError || fetchRunning;
            View.MessageView.Hidden = isError || !isEmptyList || fetchRunning;

            if (isError)
            {
                View.ErrorView.ErrorMessageLabel.Text = ViewModel.SearchCommand.Error.MessageForHuman();
                View.ErrorView.SizeToFit();
            }
            else if (isEmptyList)
            {
                if (View.MainPartView.SearchTextField.HasText)
                    View.MessageView.MessageLabel.Text = L10n.Localize("NoMatchesLabel", "No matches found");

                else
                    View.MessageView.MessageLabel.Text = L10n.Localize("NoAttendeesLabel", "No attendees");
                View.MessageView.SizeToFit();
            }
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

        #region Commands
        AsyncCommand OpenQRBarcodeScannerCommand { get; set; }
        async Task OpenQRBarcodeScannerAction(object obj)
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
                    if (card != null) CreateLead(card, ViewModel.Event);
                    return;
                }
                loadingAlert = UIAlertController.Create(L10n.Localize("DownloadingVCardMessage", "Downloading vCard file.."), "\n\n", UIAlertControllerStyle.Alert);
                UIActivityIndicatorView indicator = new UIActivityIndicatorView(loadingAlert.View.Bounds);
                indicator.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                indicator.Color = UIColor.Black;
                indicator.UserInteractionEnabled = false;
                indicator.StartAnimating();
                loadingAlert.View.AddSubview(indicator);
                loadingAlert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, (obj1) => OpenQRBarcodeScannerCommand.CancelCommand.Execute()));
                PresentViewController(loadingAlert, true, null);

                var vCardByteArray = await ViewModel.TryDownloadVCard(vCardParser.ScanningResult.Text.TryParseWebsiteUri()?.AbsoluteUri, OpenQRBarcodeScannerCommand.Token);
                var vCardString = System.Text.Encoding.UTF8.GetString(vCardByteArray);
                var downloadedCard = vCardParser.Parse(vCardString);
                loadingAlert.DismissViewController(true, null);
                if (downloadedCard != null) CreateLead(downloadedCard, ViewModel.Event);
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

        void CreateLead()
        {
            var leadDetailsController = new LeadDetailsController(() => NavigationController.PopToViewController(this, true));
            NavigationController.PushViewController(leadDetailsController, true);
        }

        void CreateLead(Card card, EventViewModel eventViewModel)
        {
            var leadDetailsController = new LeadDetailsController(card, eventViewModel, () => NavigationController.PopToViewController(this, true));
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

        Command OpenAttendeeCommand { get; set; }
        void OpenAttendeeAction(object arg)
        {
            var item = (AttendeeViewModel)arg;
            var controller = new AttendeeDetailsController(item, ViewModel.Event);
            NavigationController.PushViewController(controller, true);
        }

        Command OpenEventsSelectionCommand { get; set; }
        void OpenEventsSelectionAction(object obj)
        {
            var eventsController = new EventsController((EventViewModel eventArg) =>
            {
                ViewModel.Event = eventArg;
                NavigationController.PopViewController(true);
            });
            NavigationController.PushViewController(eventsController, true);
        }

        Command OpenFilterCommand { get; set; }
        void OpenFilterAction(object obj)
        {
            var controller = new AttendeesFilterController(ViewModel.Event, (changed) =>
            {
                DismissViewController(true, null);
            });
            var navController = new UINavigationController(controller);
            PresentViewController(navController, true, null);
        }

        #endregion
    }
}
