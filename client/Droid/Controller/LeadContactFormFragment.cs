
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Java.IO;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using System.Linq;
using LiveOakApp.Models;
using LiveOakApp.Droid.Utils;

[assembly: UsesPermission(Name = "android.permission.WRITE_EXTERNAL_STORAGE")]
[assembly: UsesPermission(Name = "android.permission.CAMERA")]


namespace LiveOakApp.Droid.Controller
{
    public class LeadContactFormFragment : CustomFragment
    {

        LeadContactsTabView view;
        LeadDetailsViewModel model;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            TakePhotoCommand = new Command
            {
                Action = TakePhoto
            };

            ShowEventSelectorCommand = new Command
            {
                Action = ShowEventSelectorAction
            };

            ShowEmailSelectorCommand = new Command
            {
                Action = ShowEmailSelectorAction,
                CanExecute = CheckSendResourcesEnabled
            };

            AddBusinessCardPhoto = new Command
            {
                Action = TakeBusinessCardPhoto
            };

            ShowFrontBusinessCardCommand = new Command
            {
                Action = ShowFrontBusinessCardAction
            };

            ShowBackBusinessCardCommand = new Command
            {
                Action = ShowBackBusinessCardAction
            };

            model = ((LeadFragment)ParentFragment).ViewModel;

            var action = (Arguments ?? Bundle.Empty).GetInt(LeadFragment.ACTION_CODE, 0);
            switch (action)
            {
                case 1: //SCAN_BUSINESS_CARD_ACTION
                    TakeBusinessCardPhoto(null);
                    break;
            }
        }

        IPropertyBinding stateToEditBinding;
        IPropertyBinding stateToSpinnerBinding;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = new LeadContactsTabView(inflater.Context);

            Bindings.Property(model, _ => _.LeadName)
                    .To(view.NameEdit.TextProperty());
            Bindings.Property(model, _ => _.LeadSurname)
                    .To(view.SurnameEdit.TextProperty());
            Bindings.Property(model, _ => _.LeadCompany)
                    .To(view.CompanyEdit.TextProperty());
            Bindings.Property(model, _ => _.LeadAddress)
                    .To(view.AddressEdit.TextProperty());
            Bindings.Property(model, _ => _.LeadZip)
                    .To(view.ZIPEdit.TextProperty());
            stateToEditBinding = Bindings.Property(model, _ => _.LeadState)
                                             .To(view.StateEdit.TextProperty())
                                             .Binding;
            Bindings.Property(model, _ => _.LeadTitle)
                    .To(view.TitleEdit.TextProperty());
            Bindings.Property(model, _ => _.LeadCompanyURL)
                    .To(view.CompanyURLEdit.TextProperty());
            Bindings.Property(model, _ => _.LeadNotes)
                    .To(view.NotesEdit.TextProperty());
            Bindings.Property(model, _ => _.LeadFirstEntryLocationDescription)
                    .To(view.FirstEntryText.TextProperty());
            Bindings.Property(model, _ => _.LeadCity)
                    .To(view.CityEdit.TextProperty());

            Bindings.Adapter(view.StateSpinner, view.GetStatesSpinnerAdapter(model.StatesList.ToList()));
            stateToSpinnerBinding = Bindings.Property(model, _ => _.LeadState)
                                                .To(view.StateSpinner.Adapter().SelectedItemProperty<string>())
                                                .Binding;

            Bindings.Property(model, _ => _.IsStatesPickerEnabled)
                    .UpdateTarget((isStateSpinnerEnabled) =>
            {
                view.IsStateSpinnerVisible = isStateSpinnerEnabled.Value;
                if (isStateSpinnerEnabled.Value)
                {
                    stateToSpinnerBinding.Bind();
                    stateToEditBinding.Unbind();
                }
                else
                {
                    stateToSpinnerBinding.Unbind();
                    stateToEditBinding.Bind();
                }
            });

            Bindings.Adapter(view.CountrySpinner, view.GetCountriesSpinnerAdapter(model.CountriesList.ToList()));
            Bindings.Property(model, _ => _.LeadCountry)
                    .To(view.CountrySpinner.Adapter().SelectedItemProperty<string>());

            Bindings.Property(model, _ => _.IsLeadNameValid)
                    .UpdateTarget((isValid) => view.IsNameHighlighted = !isValid.Value);
            Bindings.Property(model, _ => _.IsLeadSurnameValid)
                    .UpdateTarget((isValid) => view.IsSurnameHighlighted = !isValid.Value);
            Bindings.Property(model, _ => _.IsLeadCompanyValid)
                    .UpdateTarget((isValid) => view.IsCompanyHighlighted = !isValid.Value);
            Bindings.Property(model, _ => _.IsLeadCompanyURLValid)
                    .UpdateTarget((isValid) => view.IsCompanyURLHighlighted = !isValid.Value);
            Bindings.Property(model, _ => _.IsLeadTitleValid)
                    .UpdateTarget((isValid) => view.IsTitleHighlighted = !isValid.Value);
            Bindings.Property(model, _ => _.IsLeadAddressValid)
                    .UpdateTarget((isValid) => view.IsAddressHighlighted = !isValid.Value);
            Bindings.Property(model, _ => _.IsLeadCityValid)
                    .UpdateTarget((isValid) => view.IsCityHighlighted = !isValid.Value);
            Bindings.Property(model, _ => _.IsLeadStateValid)
                    .UpdateTarget((isValid) => view.IsStateHighlighted = !isValid.Value);
            Bindings.Property(model, _ => _.IsLeadZipValid)
                    .UpdateTarget((isValid) => view.IsZIPHighlighted = !isValid.Value);
            Bindings.Property(model, _ => _.IsLeadCountryValid)
                    .UpdateTarget((isValid) => view.IsCountryHighlighted = !isValid.Value);
            Bindings.Property(model, _ => _.IsLeadNotesValid)
                    .UpdateTarget((isValid) => view.IsNotesHighlighted = !isValid.Value);

            Bindings.Property(model, _ => _.LeadCardFrontResource)
                    .UpdateTarget((source) =>
            {
                view.SetCardFrontResource(source.Value);
            });
            Bindings.Property(model, _ => _.LeadCardBackResource)
                    .UpdateTarget((source) =>
            {
                view.SetCardBackResource(source.Value);
            });
            Bindings.Property(model, _ => _.LeadPhotoResource)
                    .UpdateTarget((source) =>
            {
                view.SetPhotoResource(source.Value);
            });

            view.OnEmailChangedAction = RaiseSendResourcesAndCreateEmailEnabledChanged;
            Bindings.Adapter(view.EmailsList, view.EmailsListAdapter(model.EmailViewModels));
            view.OnPhoneChangedAction = model.CreatePhone.RaiseCanExecuteChanged;
            Bindings.Adapter(view.PhonesList, view.PhonesListAdapter(model.PhoneViewModels));

            model.CreateEmail.CanExecuteChanged += (sender, e) =>
            {
                if (model.CreateEmail.CanExecute(null))
                    view.AddEmailButton.Visibility = ViewStates.Visible;
                else
                    view.AddEmailButton.Visibility = ViewStates.Gone;
            };
            model.CreatePhone.CanExecuteChanged += (sender, e) =>
            {
                if (model.CreatePhone.CanExecute(null))
                    view.AddPhoneButton.Visibility = ViewStates.Visible;
                else
                    view.AddPhoneButton.Visibility = ViewStates.Gone;

            };

            Bindings.Command(model.CreateEmail)
                    .To(view.AddEmailButton.ClickTarget());
            Bindings.Command(model.CreatePhone)
                    .To(view.AddPhoneButton.ClickTarget());
            Bindings.Command(TakePhotoCommand)
                    .To(view.AddPhotoButton.ClickTarget());
            Bindings.Command(TakePhotoCommand)
                    .To(view.PhotoView.ClickTarget());
            Bindings.Command(AddBusinessCardPhoto)
                    .To(view.AddBusinessCardButton.ClickTarget());
            Bindings.Command(ShowFrontBusinessCardCommand)
                    .To(view.BusinessCardFrontThumb.ClickTarget());
            Bindings.Command(ShowBackBusinessCardCommand)
                    .To(view.BusinessCardBackThumb.ClickTarget());

            Bindings.Add(view.ChildrenBindingList);

            Bindings.Property(model, _ => _.Event)
                    .Convert<string>((_) => _ != null ? _.Name : L10n.Localize("NoEvent", "No event"))
                    .To(view.EventView.TextProperty());
            Bindings.Property(model, _ => _.IsNoEventSelectedErrorShown)
                    .UpdateTarget((isErrorShown) =>
            {
                view.SetEventErrorAnimationEnabled(isErrorShown.Value);
            });

            Bindings.Property(model.EmailViewModels, _ => _.Count).UpdateTarget((s) =>
            {
                ShowEmailSelectorCommand.RaiseCanExecuteChanged();
            });

            Bindings.Command(ShowEventSelectorCommand)
                    .To(view.EventView.ClickTarget());
            Bindings.Command(ShowEmailSelectorCommand)
                    .To(view.SendResourcesButton.ClickTarget());

            AlertDialog progressDialog = null;
            Bindings.Command(model.ExportLeadToCrmCommand)
                    .To(view.CRMLogo.ClickTarget())
                    .WhenStarted((view, command) =>
            {
                progressDialog = ShowProgressDialog();
            })
                    .WhenFinished((view, command) =>
            {
                progressDialog.Dismiss();
                ShowExportResult(command.Error);
            });

            Bindings.Property(model, _ => _.CurrentCrmType)
                    .UpdateTarget((source) => view.UpdateCRMLogo(source.Value, model.CrmExportState));
            Bindings.Property(model, _ => _.CrmExportState)
                    .UpdateTarget((source) => view.UpdateCRMLogo(model.CurrentCrmType, source.Value));

            return view;
        }

        #region Photos

        public Command TakePhotoCommand { get; private set; }
        public Command AddBusinessCardPhoto { get; private set; }

        const int REQUEST_IMAGE_CAPTURE = 1;
        const int PERMISSION_REQUEST_IMAGE_CAPTURE = 11;
        const int REQUEST_BUSINESS_CARD_FRONT_CAPTURE = 2;
        const int PERMISSION_REQUEST_BUSINESS_CARD_CAPTURE = 23;
        const int REQUEST_BUSINESS_CARD_BACK_CAPTURE = 3;

        const ActivityFlags uriPermissionFlags = ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantReadUriPermission;

        void TakePhoto(object args)
        {
            if(!PermissionsUtils.HasCameraAndStoragePermission(Context)) 
            {
                PermissionsUtils.RequestCameraAndStoragePermission(Activity, PERMISSION_REQUEST_IMAGE_CAPTURE);
                return;
            }
            var intent = CreateIntent(REQUEST_IMAGE_CAPTURE);
            if (intent == null)
            {
                Toast.MakeText(Context, L10n.Localize("UnableToTakePhotoError", "Unable to take photo"), ToastLength.Short).Show();
                return;
            }
            ParentFragment.StartActivityForResult(intent, REQUEST_IMAGE_CAPTURE);
        }

        void TakeBusinessCardPhoto(object args)
        {
            if (!PermissionsUtils.HasCameraAndStoragePermission(Context))
            {
                PermissionsUtils.RequestCameraAndStoragePermission(Activity, PERMISSION_REQUEST_BUSINESS_CARD_CAPTURE);
                return;
            }

            bool haveBackPhoto = !string.IsNullOrEmpty(model.LeadCardBackResource.RelativeLocalPath) ||
                                       !string.IsNullOrEmpty(model.LeadCardBackResource.RemoteUrl);
            bool haveFrontPhoto = !string.IsNullOrEmpty(model.LeadCardFrontResource.RelativeLocalPath) ||
                                         !string.IsNullOrEmpty(model.LeadCardFrontResource.RemoteUrl);

            var intent = new Intent(MediaStore.ActionImageCapture);
            if (intent.ResolveActivity(Activity.PackageManager) == null)
            {
                Toast.MakeText(Context, L10n.Localize("UnableToTakePhotoError", "Unable to take photo"), ToastLength.Short).Show();
                return;
            }

            if (!haveBackPhoto && !haveFrontPhoto)
            {
                ParentFragment.StartActivityForResult(CreateIntent(REQUEST_BUSINESS_CARD_FRONT_CAPTURE), REQUEST_BUSINESS_CARD_FRONT_CAPTURE);
            }
            else
            {
                var actions = new string[]
                {
                    haveFrontPhoto ?
                        L10n.Localize("ReplaceFrontCardLabel", "Replace front photo") :
                        L10n.Localize("AddFrontCardLabel", "Add front photo"),
                    haveBackPhoto ?
                        L10n.Localize("ReplaceBackCardLabel", "Replace back photo") :
                        L10n.Localize("AddBackCardLabel", "Add back photo")
                };
                AlertDialog.Builder builder = new AlertDialog.Builder(Context);
                builder.SetItems(actions, (sender, e) =>
                {
                    switch (e.Which)
                    {
                        case 0:
                            ParentFragment.StartActivityForResult(CreateIntent(REQUEST_BUSINESS_CARD_FRONT_CAPTURE), REQUEST_BUSINESS_CARD_FRONT_CAPTURE);
                            break;
                        case 1:
                            ParentFragment.StartActivityForResult(CreateIntent(REQUEST_BUSINESS_CARD_BACK_CAPTURE), REQUEST_BUSINESS_CARD_BACK_CAPTURE);
                            break;
                    }

                });
                builder.Create().Show();
            }
        }

        Intent CreateIntent(int requestCode)
        {
            var intents = new List<Intent>();
            var takePictureIntent = new Intent(MediaStore.ActionImageCapture);
            if (takePictureIntent.ResolveActivity(Activity.PackageManager) != null)
            {
                var photoFile = CreateImageFile(requestCode);
                if (photoFile != null)
                {
                    var photoURI = FileProvider.GetUriForFile(this.Context,
                                                          Context.PackageName + ".fileprovider",
                                                          photoFile);
                    takePictureIntent.PutExtra(MediaStore.ExtraOutput, photoURI);

                    var resInfoList = Context.PackageManager.QueryIntentActivities(takePictureIntent, PackageInfoFlags.MatchDefaultOnly);
                    foreach (var resolveInfo in resInfoList)
                    {
                        var packageName = resolveInfo.ActivityInfo.PackageName;
                        Context.GrantUriPermission(packageName, photoURI, uriPermissionFlags);
                    }

                    AddIntentsToList(Context, intents, takePictureIntent);

                    var pickPhotoIntent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
                    AddIntentsToList(Context, intents, pickPhotoIntent);

                    var chooserIntent = Intent.CreateChooser(intents[0], L10n.Localize("TakePhotoAlertAction", "Take a photo"));
                    intents.RemoveAt(0);
                    chooserIntent.PutExtra(Intent.ExtraInitialIntents, intents.ToArray());

                    var leadFragment = ParentFragment as LeadFragment;
                    switch (requestCode)
                    {
                        case REQUEST_IMAGE_CAPTURE:
                            leadFragment.LeadPhotoUri = photoURI;
                            leadFragment.LeadPhotoFilePath = photoFile.AbsolutePath;
                            break;
                        case REQUEST_BUSINESS_CARD_BACK_CAPTURE:
                            leadFragment.CardBackPhotoUri = photoURI;
                            leadFragment.CardBackPhotoFilePath = photoFile.AbsolutePath;
                            break;
                        case REQUEST_BUSINESS_CARD_FRONT_CAPTURE:
                            leadFragment.CardFrontPhotoUri = photoURI;
                            leadFragment.CardFrontPhotoFilePath = photoFile.AbsolutePath;
                            break;
                    }
                    return chooserIntent;
                }
            }
            return null;
        }

        Java.IO.File CreateImageFile(int requestCode)
        {
            // Create an image file name
            var timeStamp = DateTime.Now.Ticks.ToString();
            var imageFileName = string.Format("JPEG_{0}_{1}", timeStamp, requestCode);
            var storageDir = Activity.CacheDir;
            var image = Java.IO.File.CreateTempFile(
                imageFileName,
                ".jpg",
                storageDir
            );

            return image;
        }

        async Task ResizeAndSave(int requestCode, Java.IO.File photoFile)
        {
            var image = await Image.LoadFromStream(new FileStream(photoFile.AbsolutePath, FileMode.Open, FileAccess.Read));
            var scaledImage = await image.AspectFitInSize(model.MaxImageDimention, model.MaxImageDimention);
            photoFile.Delete();
            await scaledImage.Write(new FileStream(photoFile.AbsolutePath, FileMode.OpenOrCreate));
            switch (requestCode)
            {
                case REQUEST_IMAGE_CAPTURE:
                    model.ReplacePhoto(photoFile.AbsolutePath);
                    System.IO.File.Delete(photoFile.AbsolutePath);
                    break;
                case REQUEST_BUSINESS_CARD_FRONT_CAPTURE:
                    model.ReplaceBusinessCardFront(photoFile.AbsolutePath);
                    System.IO.File.Delete(photoFile.AbsolutePath);
                    break;
                case REQUEST_BUSINESS_CARD_BACK_CAPTURE:
                    model.ReplaceBusinessCardBack(photoFile.AbsolutePath);
                    System.IO.File.Delete(photoFile.AbsolutePath);
                    break;
            }
        }

        public void HandleActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (resultCode != (int)Result.Ok)
                return;
            
            var leadFragment = ParentFragment as LeadFragment;

            Android.Net.Uri photoURI = null;
            Java.IO.File photoFile = null;

            switch (requestCode) 
            {
                case REQUEST_IMAGE_CAPTURE:
                    photoURI = leadFragment.LeadPhotoUri;
                    photoFile = new Java.IO.File(leadFragment.LeadPhotoFilePath);
                break;
                case REQUEST_BUSINESS_CARD_BACK_CAPTURE:
                    photoURI = leadFragment.CardBackPhotoUri;
                    photoFile = new Java.IO.File(leadFragment.CardBackPhotoFilePath);
                break;
                case REQUEST_BUSINESS_CARD_FRONT_CAPTURE:
                    photoURI = leadFragment.CardFrontPhotoUri;
                    photoFile = new Java.IO.File(leadFragment.CardFrontPhotoFilePath);
                break;
            }

            Context.RevokeUriPermission(photoURI, uriPermissionFlags);

            if (photoFile.Length() == 0)
            {
                var inputStream = Context.ContentResolver.OpenInputStream(data.Data);
                var outputStream = new FileOutputStream(photoFile);
                var buffer = new byte[1024];
                var readBytes = 0;
                do
                {
                    readBytes = inputStream.Read(buffer, 0, buffer.Length);
                    outputStream.Write(buffer);
                } while (readBytes > 0);

                inputStream.Close();
                outputStream.Close();
            }
            ResizeAndSave(requestCode, photoFile).Ignore();
        }

        static List<Intent> AddIntentsToList(Context context, List<Intent> list, Intent intent)
        {
            var resInfoList = context.PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);

            foreach (var resolveInfo in resInfoList)
            {
                var packageName = resolveInfo.ActivityInfo.PackageName;
                var targetedIntent = new Intent(intent);
                targetedIntent.SetPackage(packageName);
                list.Add(targetedIntent);
            }
            return list;
        }

        #endregion

        bool CheckSendResourcesEnabled(object arg)
        {
            foreach (LeadDetailsEmailViewModel emailViewModel in model.EmailViewModels)
                if (emailViewModel.IsEmailValid && !string.IsNullOrWhiteSpace(emailViewModel.Email))
                    return true;
            return false;
        }

        void RaiseSendResourcesAndCreateEmailEnabledChanged()
        {
            model.CreateEmail.RaiseCanExecuteChanged();
            ShowEmailSelectorCommand.RaiseCanExecuteChanged();
        }

        #region Actions

        public Command ShowEventSelectorCommand { get; private set; }
        void ShowEventSelectorAction(object arg)
        {
            UiUtil.hideKeyboard(view);
            model.IsNoEventSelectedErrorShown = false;
            if (model.HasAnsweredQuestions)
            {
                var confirmationDialog = new ClearQualifyDialogFragment();
                confirmationDialog.PositiveCallback = (sender, e) =>
                {
                    confirmationDialog.Dismiss();
                    ShowEventSelectorFragment();
                };
                confirmationDialog.Show(FragmentManager, "confirmation-dialog");
            }
            else
            {
                ShowEventSelectorFragment();
            }
        }
        void ShowEventSelectorFragment()
        {
            var fragment = EventsFragment.CreateForResult(model.SetEventCommand);

            ParentFragment.FragmentManager.BeginTransaction()
                          .Replace(Resource.Id.fragment_container, fragment)
                          .AddToBackStack("events-choser-screen")
                          .Commit();
        }

        public Command ShowEmailSelectorCommand { get; private set; }
        void ShowEmailSelectorAction(object arg)
        {
            string[] emails = new string[model.EmailViewModels.Count];
            for (var i = 0; i < emails.Length; i++)
            {
                emails[i] = model.EmailViewModels[i].Email;
            }

            var emailSelectorDialog = EmailSelectorDialogFragment.CreateWithEmails(emails, (email) =>
            {
                var resourcesFragment = MyResourcesFragment.CreateForEmail(email);
                ParentFragment.FragmentManager.BeginTransaction()
                              .Replace(Resource.Id.fragment_container, resourcesFragment)
                              .AddToBackStack("resources-screen")
                              .Commit();
            });

            emailSelectorDialog.Show(FragmentManager, "email-selector-dialog");
        }

        Command ShowFrontBusinessCardCommand { get; set; }
        void ShowFrontBusinessCardAction(object arg)
        {
            bool haveFrontPhoto = !string.IsNullOrEmpty(model.LeadCardFrontResource.RelativeLocalPath) ||
                                         !string.IsNullOrEmpty(model.LeadCardFrontResource.RemoteUrl);
            if (haveFrontPhoto)
                ShowBusinessCard(L10n.Localize("FrontBusinessCardLabel", "Front business card"), model.LeadCardFrontResource);
        }

        Command ShowBackBusinessCardCommand { get; set; }
        void ShowBackBusinessCardAction(object arg)
        {
            bool haveBackPhoto = !string.IsNullOrEmpty(model.LeadCardBackResource.RelativeLocalPath) ||
                                       !string.IsNullOrEmpty(model.LeadCardBackResource.RemoteUrl);
            if (haveBackPhoto)
                ShowBusinessCard(L10n.Localize("BackBusinessCardLabel", "Back business card"), model.LeadCardBackResource);
        }

        void ShowBusinessCard(string title, FileResource res)
        {
            var viewer = BusinessCardViewerDialog.Create(title, res);
            viewer.Show(FragmentManager, null);
        }

        #endregion

        #region Dialogs

        void ShowExportResult(Exception error)
        {
            if (error == null && model.CrmExportState == LeadDetailsViewModel.CrmExportStates.Exported)
                ShowCRMExportedDialog();
            else if (model.CrmExportState == LeadDetailsViewModel.CrmExportStates.ExportScheduled)
                ShowCRMExportScheduledDialog();
            else // CRM returned error (i.e. missing fields)
                ShowCRMExportErrorDialog(error.MessageForHuman());
        }

        AlertDialog ShowProgressDialog()
        {
            var dialog = new AlertDialog.Builder(Context)
                           .SetTitle(L10n.Localize("ExportingMessage", "Exporting ") + model.FullName)
                           .SetView(new ProgressBar(Context))
                           .SetCancelable(false)
                           .Create();
            dialog.Show();
            return dialog;
        }

        void ShowCRMExportedDialog()
        {
            new AlertDialog.Builder(Context)
                           .SetTitle(model.FullName)
                           .SetMessage(L10n.Localize("ExportSuccessMessage", "successfully exported to ") + model.CurrentCrmType)
                           .SetPositiveButton(L10n.Localize("Ok", "OK"), (sender, e) =>
            {
                ((AlertDialog)sender).Dismiss();
            })
                           .Create()
                           .Show();
        }

        void ShowCRMExportScheduledDialog()
        {
            new AlertDialog.Builder(Context)
                           .SetTitle(model.FullName)
                           .SetMessage(L10n.Localize("ExportScheduledMessage", "will be exported to CRM when internet connection is restored"))
                           .SetPositiveButton(L10n.Localize("Ok", "OK"), (sender, e) =>
            {
                ((AlertDialog)sender).Dismiss();
            })
                           .Create()
                           .Show();
        }

        void ShowCRMExportErrorDialog(string error)
        {
            new AlertDialog.Builder(Context)
                           .SetTitle(model.FullName)
                           .SetMessage(error ?? L10n.Localize("ExportFailedMessage", "Failed to export lead"))
                           .SetPositiveButton(L10n.Localize("Ok", "OK"), (sender, e) =>
            {
                ((AlertDialog)sender).Dismiss();
            })
                           .Create()
                           .Show();
        }

        void ShowCameraPermissionDeniedDialog() 
        {
            new AlertDialog.Builder(Context)
                                       .SetTitle(L10n.Localize("CameraPermission", "Camera permission"))
                                       .SetMessage(L10n.Localize("CameraPermissionMessage", "Permission is needed to take photos"))
                                       .SetPositiveButton(L10n.Localize("Ok", "ok"), (sender, e) => { })
                                       .Create()
                                       .Show();
        }

        #endregion

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch(requestCode) 
            {
                case PERMISSION_REQUEST_IMAGE_CAPTURE:
                    if (grantResults[0] == Permission.Granted)
                        TakePhotoCommand.Execute();
                    else
                        ShowCameraPermissionDeniedDialog();
                    break;
                case PERMISSION_REQUEST_BUSINESS_CARD_CAPTURE:
                    if (grantResults[0] == Permission.Granted)
                        AddBusinessCardPhoto.Execute();
                    else
                        ShowCameraPermissionDeniedDialog();
                    break;
            }
        }
    }
}

