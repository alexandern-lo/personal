using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;
using Foundation;
using StudioMobile;
using LiveOakApp.iOS.View.Content;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using AVFoundation;
using Photos;
using FFImageLoading.Work;
using LiveOakApp.Models.Data.Entities;
using FFImageLoading;
using CoreGraphics;
using LiveOakApp.Models;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.iOS.View.Skin;

namespace LiveOakApp.iOS.Controller.Content
{
    public class LeadContactController : CustomController<LeadContactScrollView>, IUIImagePickerControllerDelegate, IUIPickerViewDelegate, IUIPickerViewDataSource
    {
        LeadDetailsViewModel ViewModel { get; set; }
        UIAlertController CrmExportActivityAlert { get; set; }
        UIImagePickerController AvatarImagePickerController = new UIImagePickerController();
        UIImagePickerController BusinessCardImagePickerController = new UIImagePickerController();

        public LeadContactController(LeadDetailsViewModel viewModel)
        {
            ViewModel = viewModel;

            ChooseEventCommand = new Command
            {
                Action = EventChooseClickAction
            };

            ResourcesSendCommand = new Command
            {
                Action = SendResourcesButtonClickAction
            };

            AddPhotoCommand = new Command
            {
                Action = AddPhotoClickAction
            };

            ShowBackBusinessCardCommand = new Command
            {
                Action = ShowBackBusinessCardAction
            };

            ShowFrontBusinessCardCommand = new Command
            {
                Action = ShowFrontBusinessCardAction
            };

            AddBusinessCardCommand = new Command
            {
                Action = AddBusinessCardAction
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AvatarImagePickerController.Delegate = this;
            BusinessCardImagePickerController.Delegate = this;
            View.ContentScrollView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.Interactive;

            var emailInputsBinding = View.LeadContactView.GetEmailInputsBinding(ViewModel.EmailViewModels, OnEmailTypeButtonClickHandler, CheckSendResourcesAndAddEmailEnabled);
            Bindings.Add(emailInputsBinding);
            var phoneInputsBinding = View.LeadContactView.GetPhoneInputsBinding(ViewModel.PhoneViewModels, OnPhoneTypeButtonClickHandler, () => ViewModel.CreatePhone.RaiseCanExecuteChanged());
            Bindings.Add(phoneInputsBinding);
            Bindings.Command(ChooseEventCommand).To(View.LeadContactView.EventSelectButton.ClickTarget());
            Bindings.Property(ViewModel, _ => _.Event)
                    .UpdateTarget((s) =>
                    {
                        if (s.Value == null) return;
                        if (s.Value.Name != null) View.LeadContactView.EventSelectButton.KeyValueInfoView.ValueLabel.Text = s.Value.Name;
                    });
            Bindings.Property(ViewModel.EmailViewModels, _ => _.Count).UpdateTarget((s) => { CheckSendResourcesAndAddEmailEnabled(); View.SetNeedsLayout(); });
            Bindings.Property(ViewModel.PhoneViewModels, _ => _.Count).UpdateTarget((s) => View.SetNeedsLayout());
            Bindings.Command(ViewModel.CreateEmail).To(View.LeadContactView.AddEmailInputButton.ClickTarget())
                    .AfterExecute((target, command) => { View.LeadContactView.SetNeedsLayoutAsyncNoWait(); View.LeadContactView.NeedsCellBecomeFirstResponder = true; });
            Bindings.Command(ViewModel.CreatePhone).To(View.LeadContactView.AddPhoneInputButton.ClickTarget())
                    .AfterExecute((target, command) => { View.LeadContactView.SetNeedsLayoutAsyncNoWait(); View.LeadContactView.NeedsCellBecomeFirstResponder = true; });
            Bindings.Command(ResourcesSendCommand).To(View.LeadContactView.SendResourcesButton.ClickTarget());
            Bindings.Command(AddPhotoCommand).To(View.LeadContactView.AddAvatarButton.ClickTarget());
            Bindings.Command(AddPhotoCommand).To(View.LeadContactView.AvatarIndicatorButton.ClickTarget());
            Bindings.Command(AddPhotoCommand).To(View.LeadContactView.AvatarPreview.ClickTarget());
            Bindings.Property(ViewModel, _ => _.LeadPhotoResource).UpdateTarget((source) => View.LeadContactView.SetPhotoResource(source.Value));
            Bindings.Property(ViewModel, _ => _.LeadName).To(View.LeadContactView.FirstNameInput.ValueTextField.TextProperty());
            Bindings.Property(ViewModel, _ => _.LeadSurname).To(View.LeadContactView.LastNameInput.ValueTextField.TextProperty());
            Bindings.Property(ViewModel, _ => _.LeadCompany).To(View.LeadContactView.CompanyInput.ValueTextField.TextProperty());
            Bindings.Property(ViewModel, _ => _.LeadTitle).To(View.LeadContactView.TitleInput.ValueTextField.TextProperty());
            Bindings.Property(ViewModel, _ => _.LeadAddress).To(View.LeadContactView.AddressInput.ValueTextField.TextProperty());
            Bindings.Property(ViewModel, _ => _.LeadCity).To(View.LeadContactView.CityInput.ValueTextField.TextProperty());
            Bindings.Property(ViewModel, _ => _.LeadState).To(View.LeadContactView.StateInput.ValueTextField.TextProperty());
            Bindings.Property(ViewModel, _ => _.LeadState).UpdateTarget((source) =>
                    {
                        var currStateIndex = 0;
                        ViewModel.StatesList.Each((string obj, int index) => { if (obj.Equals(source.Value)) currStateIndex = index; });
                        View.LeadContactView.StatePicker.Select(currStateIndex, 0, false);
                    });
            Bindings.Property(ViewModel, _ => _.LeadZip).To(View.LeadContactView.ZipInput.ValueTextField.TextProperty());
            Bindings.Property(ViewModel, _ => _.LeadCountry).To(View.LeadContactView.CountryInput.ValueTextField.TextProperty());
            Bindings.Property(ViewModel, _ => _.LeadCountry).UpdateTarget((source) =>
                    {
                        var currCountryIndex = 0;
                        ViewModel.CountriesList.Each((string obj, int index) => { if (obj.Equals(source.Value)) currCountryIndex = index; });
                        View.LeadContactView.CountryPicker.Select(currCountryIndex, 0, false);
                    });
            Bindings.Property(ViewModel, _ => _.LeadCompanyURL).To(View.LeadContactView.CompanyUrlInput.ValueTextField.TextProperty());
            Bindings.Property(ViewModel, _ => _.LeadFirstEntryLocationDescription).To(View.LeadContactView.LocationLabel.TextProperty())
                    .AfterTargetUpdate((target, source) => View.LeadContactView.SetNeedsLayoutAsyncNoWait());
            Bindings.Property(ViewModel, _ => _.LeadNotes).To(View.LeadContactView.NotesTextView.TextProperty());
            Bindings.Property(ViewModel, _ => _.IsNoEventSelectedErrorShown).UpdateTarget((source) => View.LeadContactView.ShowEventErrorAnimated(source.Value));
            Bindings.Property(ViewModel, _ => _.LeadCardFrontResource).UpdateTarget((source) => View.LeadContactView.SetFrontBusinessCardButtonEnabled(!source.Value.isEmpty()));
            Bindings.Property(ViewModel, _ => _.LeadCardBackResource).UpdateTarget((source) => View.LeadContactView.SetBackBusinessCardButtonEnabled(!source.Value.isEmpty()));
            Bindings.Property(ViewModel, _ => _.IsStatesPickerEnabled).UpdateTarget((source) =>
            {
                if (source.Value)
                    View.LeadContactView.StateInput.ValueTextField.InputView = View.LeadContactView.StatePicker;
                else
                    View.LeadContactView.StateInput.ValueTextField.InputView = null;
            });
            Bindings.Command(ShowFrontBusinessCardCommand).To(View.LeadContactView.FrontBusinessCardButton.ClickTarget());
            Bindings.Command(ShowBackBusinessCardCommand).To(View.LeadContactView.BackBusinessCardButton.ClickTarget());
            Bindings.Command(AddBusinessCardCommand).To(View.LeadContactView.AddBusinessCardButton.ClickTarget());
            Bindings.Property(ViewModel, _ => _.HasNameOrEmail).UpdateTarget((source) => HighlightUnvalidNameOrEmailIfNeeded());

            Bindings.Command(ViewModel.ExportLeadToCrmCommand).BeforeExecute((target, command) =>
            {
                if (ViewModel.HasValidFields) return;
                this.ShowInfoAlert(L10n.Localize("Error", "Error"),
                                   L10n.Localize("LeadWillNotBeExportedMessage", "Missing required fields. Lead will not be exported"),
                                   L10n.Localize("Ok", "Ok"));
            }).To(View.LeadContactView.CrmExportButton.ClickTarget())
                    .WhenStarted((target, command) => ShowCrmExportProgress())
                    .WhenFinished((target, command) => ShowCrmExportResult(command.Error));
            Bindings.Property(ViewModel, _ => _.CurrentCrmType)
                    .UpdateTarget((source) => View.LeadContactView.UpdateCrmButton(source.Value, ViewModel.CrmExportState));
            Bindings.Property(ViewModel, _ => _.CrmExportState)
                    .UpdateTarget((source) => View.LeadContactView.UpdateCrmButton(ViewModel.CurrentCrmType, source.Value));


            Bindings.Property(ViewModel, _ => _.IsLeadEventValid)
                    .UpdateTarget((source) => View.LeadContactView.EventSelectButton.KeyValueInfoView.MakeValid(source.Value));
            Bindings.Property(ViewModel, _ => _.IsLeadSurnameValid)
                    .UpdateTarget((source) => View.LeadContactView.LastNameInput.MakeValid(source.Value));
            Bindings.Property(ViewModel, _ => _.IsLeadCompanyValid)
                    .UpdateTarget((source) => View.LeadContactView.CompanyInput.MakeValid(source.Value));
            Bindings.Property(ViewModel, _ => _.IsLeadTitleValid)
                    .UpdateTarget((source) => View.LeadContactView.TitleInput.MakeValid(source.Value));
            Bindings.Property(ViewModel, _ => _.IsLeadAddressValid)
                    .UpdateTarget((source) => View.LeadContactView.AddressInput.MakeValid(source.Value));
            Bindings.Property(ViewModel, _ => _.IsLeadCityValid)
                    .UpdateTarget((source) => View.LeadContactView.CityInput.MakeValid(source.Value));
            Bindings.Property(ViewModel, _ => _.IsLeadStateValid)
                    .UpdateTarget((source) => View.LeadContactView.StateInput.MakeValid(source.Value));
            Bindings.Property(ViewModel, _ => _.IsLeadZipValid)
                    .UpdateTarget((source) => View.LeadContactView.ZipInput.MakeValid(source.Value));
            Bindings.Property(ViewModel, _ => _.IsLeadCountryValid)
                    .UpdateTarget((source) => View.LeadContactView.CountryInput.MakeValid(source.Value));
            Bindings.Property(ViewModel, _ => _.IsLeadCompanyURLValid)
                    .UpdateTarget((source) => View.LeadContactView.CompanyUrlInput.MakeValid(source.Value));
            Bindings.Property(ViewModel, _ => _.IsLeadNotesValid)
                    .UpdateTarget((source) =>
                    {
                        if (source.Value)
                            View.LeadContactView.NotesSeparatorView.BackgroundColor = Colors.GraySeparatorColor;
                        else
                            View.LeadContactView.NotesSeparatorView.BackgroundColor = Colors.Red;
                    });

            View.LeadContactView.CountryPicker.Delegate = this;
            View.LeadContactView.CountryPicker.DataSource = this;
            View.LeadContactView.StatePicker.Delegate = this;
            View.LeadContactView.StatePicker.DataSource = this;
        }

        public Command ChooseEventCommand { get; private set; }
        void EventChooseClickAction(object obj)
        {
            if (ViewModel.Event == null)
            {
                PushEventChoosingViewController();
                return;
            }
            if (ViewModel.HasAnsweredQuestions)
            {
                var alert = UIAlertController.Create(L10n.Localize("ChangeEventWarningTitle", "Changing event"),
                                                                   L10n.Localize("ChangeEventWarningText", "Warning! This will delete all answers in the Qualify tab."),
                                                                   UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create(L10n.Localize("Ok", "Ok"), UIAlertActionStyle.Destructive, (arg) => PushEventChoosingViewController()));
                alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
                PresentViewController(alert, true, null);
            }
            else {
                PushEventChoosingViewController();
            }
        }

        void PushEventChoosingViewController()
        {
            EventsController eventsController = null;
            eventsController = new EventsController((EventViewModel eventDTO) =>
            {
                ViewModel.Event = eventDTO;
                eventsController.NavigationController.PopViewController(true);
            });
            NavigationController.PushViewController(eventsController, true);
        }

        Task<string> OnEmailTypeButtonClickHandler(List<string> emails)
        {
            var task = new TaskCompletionSource<string>();

            var alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);
            foreach (string email in emails)
            {
                alert.AddAction(UIAlertAction.Create(email, UIAlertActionStyle.Default, (obj) => { task.SetResult(email); }));
            }
            alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, (obj) => { task.SetResult(null); }));
            PresentViewController(alert, true, null);

            return task.Task;
        }

        Task<string> OnPhoneTypeButtonClickHandler(List<string> phones)
        {
            var task = new TaskCompletionSource<string>();

            var alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);
            foreach (string phone in phones)
            {
                alert.AddAction(UIAlertAction.Create(phone, UIAlertActionStyle.Default, (obj) => { task.SetResult(phone); }));
            }
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (obj) => { task.SetResult(null); }));
            PresentViewController(alert, true, null);

            return task.Task;
        }

        public Command ResourcesSendCommand { get; private set; }
        void SendResourcesButtonClickAction(object obj)
        {
            var alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);
            foreach (LeadDetailsEmailViewModel email in ViewModel.EmailViewModels)
            {
                if (string.IsNullOrWhiteSpace(email.Email)) continue;
                alert.AddAction(UIAlertAction.Create(email.Email, UIAlertActionStyle.Default, (obj1) =>
                {
                    var resourcesController = new MyResourcesController(email.Email);
                    var navController = new UINavigationController(resourcesController);
                    PresentViewController(navController, true, null);
                }));
            }
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }

        void CheckSendResourcesAndAddEmailEnabled()
        {
            ViewModel.CreateEmail.RaiseCanExecuteChanged();
            foreach (LeadDetailsEmailViewModel emailViewModel in ViewModel.EmailViewModels)
            {
                if (!string.IsNullOrWhiteSpace(emailViewModel.Email))
                {
                    View.LeadContactView.SetSendResourcesEnabled(true);
                    return;
                }
            }
            View.LeadContactView.SetSendResourcesEnabled(false);
        }

        void HighlightUnvalidNameOrEmailIfNeeded()
        {
            if (ViewModel.HasNameOrEmail)
            {
                View.LeadContactView.FirstNameInput.MakeValid(true);
                View.LeadContactView.FirstNameUnderlineView.BackgroundColor = Colors.GraySeparatorColor;
                View.LeadContactView.EmailsTableView.VisibleCells.Each((cell, index) => ((EmailEditCell)cell).ErrorUnderline.BackgroundColor = UIColor.Clear);
            }
            else {
                View.LeadContactView.FirstNameInput.MakeValid(false);
                View.LeadContactView.FirstNameUnderlineView.BackgroundColor = Colors.Red;
                View.LeadContactView.EmailsTableView.VisibleCells.Each((cell, index) => ((EmailEditCell)cell).ErrorUnderline.BackgroundColor = Colors.Red);
            }
        }

        void ShowCrmExportProgress()
        {
            if (CrmExportActivityAlert == null)
            {
                CrmExportActivityAlert = UIAlertController.Create(L10n.Localize("ExportingMessage", "Exporting ") + ViewModel.FullName, "\n\n\n\n", UIAlertControllerStyle.Alert);
                var indicator = new UIActivityIndicatorView();
                indicator.Frame = new CGRect(CrmExportActivityAlert.View.Bounds.X, CrmExportActivityAlert.View.Bounds.Y + 15,
                                             CrmExportActivityAlert.View.Bounds.Width, CrmExportActivityAlert.View.Bounds.Height);
                indicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
                indicator.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
                indicator.StartAnimating();
                CrmExportActivityAlert.View.AddSubview(indicator);
            }
            CrmExportActivityAlert.Title = L10n.Localize("ExportingMessage", "Exporting ") + ViewModel.FullName;
            PresentViewController(CrmExportActivityAlert, true, null);
        }

        void ShowCrmExportResult(Exception error)
        {
            DismissViewController(true, () =>
            {
                if (error != null)
                {
                    this.ShowInfoAlert(ViewModel.FullName, L10n.Localize("ExportFailedMessage", "Export to CRM failed: ") + error.MessageForHuman(), L10n.Localize("Ok", "Ok"));
                    return;
                }
                switch (ViewModel.CrmExportState)
                {
                    case LeadDetailsViewModel.CrmExportStates.Exported:
                        {
                            this.ShowInfoAlert(ViewModel.FullName, L10n.Localize("ExportSuccessMessage", "Successfully exported to ") + ViewModel.CurrentCrmType, L10n.Localize("Ok", "Ok"));
                            break;
                        }
                    case LeadDetailsViewModel.CrmExportStates.ExportScheduled:
                        {
                            this.ShowInfoAlert(ViewModel.FullName, L10n.Localize("ExportScheduledMessage", "will be exported to CRM when internet connection is restored"), L10n.Localize("Ok", "Ok"));
                            break;
                        }
                    case LeadDetailsViewModel.CrmExportStates.NotExported:
                        return;
                }
            });
        }

        public Command AddPhotoCommand { get; private set; }
        void AddPhotoClickAction(object obj)
        {
            UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);
            alert.AddAction(UIAlertAction.Create(L10n.Localize("TakePhotoAlertAction", "Take a photo"), UIAlertActionStyle.Default, (obj1) =>
                 {
                     if (!this.CheckCameraPermissions()) return;
                     AvatarImagePickerController.SourceType = UIImagePickerControllerSourceType.Camera;
                     PresentViewController(AvatarImagePickerController, true, null);
                 }));
            alert.AddAction(UIAlertAction.Create(L10n.Localize("ChoosePhotoAlertAction", "Choose photo"), UIAlertActionStyle.Default, (obj1) =>
                 {
                     if (!this.CheckPhotosPermissions()) return;
                     AvatarImagePickerController.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                     PresentViewController(AvatarImagePickerController, true, null);
                 }));
            alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }

        [Export("imagePickerController:didFinishPickingImage:editingInfo:")]
        public void FinishedPickingImage(UIImagePickerController picker, UIImage image, NSDictionary editingInfo)
        {
            DismissViewController(true, null);
            if (picker.Equals(AvatarImagePickerController))
            {
                SavePhoto(image).Ignore();
            }
            else if (picker.Equals(BusinessCardImagePickerController))
            {
                CheckCardPhotoAndAddToLead(image);
            }
        }

        public async Task ReplaceBusinessCardFront(UIImage image)
        {
            var photoPath = await ResizeAndSaveImage(image);
            if (photoPath == null) return;
            ViewModel.ReplaceBusinessCardFront(photoPath);
            RemoveTempImage(photoPath);
        }

        public async Task ReplaceBusinessCardBack(UIImage image)
        {
            var photoPath = await ResizeAndSaveImage(image);
            if (photoPath == null) return;
            ViewModel.ReplaceBusinessCardBack(photoPath);
            RemoveTempImage(photoPath);
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

        public void CheckCardPhotoAndAddToLead(UIImage businessCardPhoto)
        {

            var frontCardActionTitle = ViewModel.LeadCardFrontResource.isEmpty() ? L10n.Localize("AddFrontCardLabel", "Add Front") : L10n.Localize("ReplaceFrontCardLabel", "Replace Front");
            var backCardActionTitle = ViewModel.LeadCardBackResource.isEmpty() ? L10n.Localize("AddBackCardLabel", "Add Back") : L10n.Localize("ReplaceBackCardLabel", "Replace Back");

            var alert = UIAlertController.Create(L10n.Localize("NewBusinessCardPhotoLabel", "New business card photo"), "\n\n\n\n\n\n\n\n\n", UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create(frontCardActionTitle, UIAlertActionStyle.Default, (arg) => ReplaceBusinessCardFront(businessCardPhoto).Ignore()));
            alert.AddAction(UIAlertAction.Create(backCardActionTitle, UIAlertActionStyle.Default, (arg) => ReplaceBusinessCardBack(businessCardPhoto).Ignore()));
            alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));

            var photoView = new UIImageView(businessCardPhoto);
            photoView.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
            alert.View.AddSubview(photoView);
            photoView.ContentMode = UIViewContentMode.ScaleAspectFit;
            photoView.Frame = new CGRect(alert.View.Bounds.Width / 2 - businessCardImageWidth / 2, alert.View.Bounds.Height / 2 - businessCardImageHeight / 2 - 125,
                                                                 businessCardImageWidth, businessCardImageHeight);
            PresentViewController(alert, false, null);
        }

        Command ShowFrontBusinessCardCommand { get; set; }
        void ShowFrontBusinessCardAction(object arg)
        {
            if (!ViewModel.LeadCardFrontResource.isEmpty())
                ShowBusinessCard(L10n.Localize("FrontBusinessCardLabel", "Front business card"), ViewModel.LeadCardFrontResource);
        }

        Command ShowBackBusinessCardCommand { get; set; }
        void ShowBackBusinessCardAction(object arg)
        {
            if (!ViewModel.LeadCardBackResource.isEmpty())
                ShowBusinessCard(L10n.Localize("BackBusinessCardLabel", "Back business card"), ViewModel.LeadCardBackResource);
        }

        int businessCardImageWidth = 240;
        int businessCardImageHeight = 153;
        IScheduledWork businessCardLoadingWork;
        void ShowBusinessCard(string alertTitle, FileResource cardPhotoResource)
        {
            if (businessCardLoadingWork != null)
            {
                businessCardLoadingWork.Cancel();
                businessCardLoadingWork = null;
            }

            var localPath = cardPhotoResource.AbsoluteLocalPath;
            var remoteUrl = cardPhotoResource.RemoteUrl;
            TaskParameter loadingSource = null;
            if (!String.IsNullOrWhiteSpace(cardPhotoResource.RemoteUrl))
            {
                loadingSource = ImageService.Instance.LoadUrl(remoteUrl);
            }
            else if (!String.IsNullOrWhiteSpace(localPath))
            {
                loadingSource = ImageService.Instance.LoadFile(localPath);
            }
            if (loadingSource != null)
            {
                UIAlertController businessCardAlert = UIAlertController.Create(alertTitle, "\n\n\n\n\n\n\n\n\n", UIAlertControllerStyle.Alert);
                businessCardAlert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, (obj) =>
                {
                    businessCardLoadingWork.Cancel();
                    businessCardLoadingWork = null;
                }));
                UIActivityIndicatorView businessCardAlertIndicator = new UIActivityIndicatorView();
                businessCardAlertIndicator.Frame = new CGRect(businessCardAlert.View.Bounds.Width / 2 - businessCardImageWidth / 2, businessCardAlert.View.Bounds.Height / 2 - businessCardImageHeight / 2 - 25,
                                                                 businessCardImageWidth, businessCardImageHeight);
                businessCardAlertIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
                businessCardAlertIndicator.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
                businessCardAlert.View.AddSubview(businessCardAlertIndicator);
                UIImageView businessCardImageViewForAlert = new UIImageView();
                businessCardImageViewForAlert.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
                businessCardAlert.View.AddSubview(businessCardImageViewForAlert);
                businessCardImageViewForAlert.ContentMode = UIViewContentMode.ScaleAspectFit;
                businessCardImageViewForAlert.Frame = new CGRect(businessCardAlert.View.Bounds.Width / 2 - businessCardImageWidth / 2, businessCardAlert.View.Bounds.Height / 2 - businessCardImageHeight / 2 - 25,
                                                                 businessCardImageWidth, businessCardImageHeight);
                businessCardAlertIndicator.Hidden = false;
                businessCardAlertIndicator.StartAnimating();
                PresentViewController(businessCardAlert, true, null);

                businessCardLoadingWork = loadingSource
                        .DownSample(businessCardImageWidth, businessCardImageHeight)
                        .Finish((obj) =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                businessCardAlertIndicator.Hidden = true;
                                businessCardAlertIndicator.StopAnimating();
                                businessCardImageViewForAlert.LayoutSubviews();
                            });
                        })
                        .Into(businessCardImageViewForAlert);
            }
        }

        async Task SavePhoto(UIImage image)
        {
            var photoPath = await ResizeAndSaveImage(image);
            if (photoPath == null) return;
            ViewModel.ReplacePhoto(photoPath);
            RemoveTempImage(photoPath);
        }

        async Task<string> ResizeAndSaveImage(UIImage image)
        {
            var scaledImage = await new Image(image).AspectFitInSize(ViewModel.MaxImageDimention, ViewModel.MaxImageDimention);

            var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var tmpDirectory = System.IO.Path.Combine(documentsDirectory, "..", "tmp");
            var imagePath = System.IO.Path.Combine(tmpDirectory, Guid.NewGuid() + ".jpg");

            NSError err = null;
            if (!NSFileManager.DefaultManager.FileExists(tmpDirectory))
            {
                NSFileManager.DefaultManager.CreateDirectory(tmpDirectory, false, (NSFileAttributes)null, out err);
            }
            NSData imageData = scaledImage.Native.AsJPEG();

            if (imageData.Save(imagePath, NSDataWritingOptions.Atomic, out err))
            {
                return imagePath;
            }
            LOG.Error("failed to save temp image: {0} => {1}", imagePath, err);
            return null;
        }

        void RemoveTempImage(string imagePath)
        {
            NSError err = null;
            NSFileManager.DefaultManager.Remove(imagePath, out err);
            if (err != null)
            {
                LOG.Error("failed to remove temp image: {0} => {1}", imagePath, err);
            }
        }

        #region UIPickerView delegate/dataSoutce

        public nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            if (pickerView == View.LeadContactView.CountryPicker)
                return ViewModel.CountriesList.Length;
            else
                return ViewModel.StatesList.Length;
        }

        [Export("pickerView:titleForRow:forComponent:")]
        public string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            if (pickerView == View.LeadContactView.CountryPicker)
                return ViewModel.CountriesList[row];
            else
                return ViewModel.StatesList[row];
        }

        [Export("pickerView:didSelectRow:inComponent:")]
        public void Selected(UIPickerView pickerView, nint row, nint component)
        {
            if (pickerView == View.LeadContactView.CountryPicker)
                View.LeadContactView.CountryInput.ValueTextField.Text = ViewModel.CountriesList[row];
            else
                View.LeadContactView.StateInput.ValueTextField.Text = ViewModel.StatesList[row];
        }

        #endregion
    }
}

