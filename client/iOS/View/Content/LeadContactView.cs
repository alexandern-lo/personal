using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;
using LiveOakApp.Models.Data.Entities;
using FFImageLoading;
using FFImageLoading.Work;
using FFImageLoading.Transformations;
using System.Linq;
using LiveOakApp.Models.Services;

namespace LiveOakApp.iOS.View.Content
{
    public class LeadContactView : CustomView, IUITextFieldDelegate
    {

        [View(0)]
        public KeyValueSelectButton EventSelectButton { get; private set; }

        [View(0)]
        [CommonSkin("ErrorLabel")]
        public UILabel EventErrorLabel { get; private set; }

        [View(1)]
        [CommonSkin("LeadInfoInput")]
        public KeyValueInfoInput FirstNameInput { get; private set; }

        [View]
        public UIView FirstNameUnderlineView { get; private set; }

        [View]
        public UIButton CrmExportButton { get; private set; }

        [View(2)]
        public UIButton AvatarPreview { get; private set; }

        public UIActivityIndicatorView AvatarIndicatorView { get; private set; } = new UIActivityIndicatorView();

        [View(3)]
        public UIButton AvatarIndicatorButton { get; private set; }

        [View(3)]
        [ButtonSkin("AddAvatarButton")]
        public UIButton AddAvatarButton { get; private set; }

        [View(4)]
        [CommonSkin("LeadInfoInput")]
        public KeyValueInfoInput LastNameInput { get; private set; }

        [View(5)]
        [CommonSkin("LeadInfoInput")]
        public KeyValueInfoInput CompanyInput { get; private set; }

        [View(5)]
        public UIView BusinessCardSeparator { get; private set; }

        [View(5)]
        [LabelSkin("xSmallRegularGrayLabel")]
        public UILabel BusinessCardLabel { get; private set; }

        [View(5)]
        public UIButton FrontBusinessCardButton { get; private set; }

        [View(5)]
        public UIButton BackBusinessCardButton { get; private set; }

        [View(5)]
        [ButtonSkin("AddBusinessCardButton")]
        public UIButton AddBusinessCardButton { get; private set; }

        [View(5)]
        public UIView BusinessCardBottomSeparator { get; private set; }

        [View(6)]
        [CommonSkin("LeadInfoInput")]
        public KeyValueInfoInput TitleInput { get; private set; }

        [View(7)]
        public UIImageView EmailImageView { get; private set; }

        [View(8)]
        [LabelSkin("LeadEmailLabel")]
        public UILabel EmailLabel { get; private set; }

        [View(9)]
        public UITableView EmailsTableView { get; private set; }

        [View(10)]
        public UIButton AddEmailInputButton { get; private set; }

        [View(11)]
        [ButtonSkin("LeadSendResourcesButton")]
        public UIButton SendResourcesButton { get; private set; }

        [View(12)]
        [LabelSkin("LeadPhoneLabel")]
        public UILabel PhoneLabel { get; private set; }

        [View(13)]
        public UITableView PhonesTableView { get; private set; }

        [View(14)]
        public UIButton AddPhoneInputButton { get; private set; }

        [View(15)]
        public UIImageView AddressImageView { get; private set; }

        [View(16)]
        [CommonSkin("LeadInfoInput")]
        public KeyValueInfoInput AddressInput { get; private set; }

        [View(17)]
        [CommonSkin("LeadInfoInput")]
        public KeyValueInfoInput CityInput { get; private set; }

        [View(18)]
        [CommonSkin("LeadInfoInput")]
        public KeyValueInfoInput StateInput { get; private set; }
        public UIPickerView StatePicker { get; private set; } = new UIPickerView();

        [View(19)]
        [CommonSkin("LeadInfoInput")]
        public KeyValueInfoInput ZipInput { get; private set; }

        [View(20)]
        [CommonSkin("LeadInfoInput")]
        public KeyValueInfoInput CountryInput { get; private set; }
        public UIPickerView CountryPicker { get; private set; } = new UIPickerView();

        [View(21)]
        public UIImageView CompanyUrlImageView { get; private set; }

        [View(22)]
        [CommonSkin("LeadInfoInput")]
        public KeyValueInfoInput CompanyUrlInput { get; private set; }

        [View(23)]
        public UIImageView NotesImageView { get; private set; }

        [View(24)]
        [LabelSkin("NotesLabel")]
        public UILabel NotesLabel { get; private set; }

        [View(25)]
        public UITextView NotesTextView { get; private set; }

        [View(26)]
        public UIView NotesSeparatorView { get; private set; }

        [View(27)]
        [LabelSkin("LocationTitleLabel")]
        public UILabel LocationTitleLabel { get; private set; }

        [View(28)]
        [LabelSkin("LocationLabel")]
        public UILabel LocationLabel { get; private set; }

        protected override void CreateView()
        {
            base.CreateView();
            EventSelectButton.KeyValueInfoView.KeyLabel.Text = L10n.Localize("EventTitle", "Event");
            EventSelectButton.KeyValueInfoView.ValueLabel.Text = " ";
            FirstNameInput.KeyLabel.Text = L10n.Localize("FirstNameTitle", "First name");
            LastNameInput.KeyLabel.Text = L10n.Localize("SurnameTitle", "Last name");
            CompanyInput.KeyLabel.Text = L10n.Localize("CompanyTitle", "Company");
            TitleInput.KeyLabel.Text = L10n.Localize("JobTitle", "Title");
            AddressInput.KeyLabel.Text = L10n.Localize("AddressTitle", "Address");
            CityInput.KeyLabel.Text = L10n.Localize("CityTitle", "City");
            StateInput.KeyLabel.Text = L10n.Localize("StateTitle", "State/Province");
            ZipInput.KeyLabel.Text = L10n.Localize("ZipTitle", "ZIP Code");
            CountryInput.KeyLabel.Text = L10n.Localize("CountryTitle", "Country");
            CompanyUrlInput.KeyLabel.Text = L10n.Localize("CompanyURLTitle", "Company URL");
            FirstNameInput.ValueTextField.Delegate = this;
            LastNameInput.ValueTextField.Delegate = this;
            CompanyInput.ValueTextField.Delegate = this;
            TitleInput.ValueTextField.Delegate = this;
            AddressInput.ValueTextField.Delegate = this;
            CityInput.ValueTextField.Delegate = this;
            StateInput.ValueTextField.Delegate = this;
            ZipInput.ValueTextField.Delegate = this;
            CountryInput.ValueTextField.Delegate = this;
            CompanyUrlInput.ValueTextField.Delegate = this;
            NotesTextView.Font = Fonts.LargeRegular;
            NotesTextView.TextColor = Colors.DarkGray;
            NotesTextView.ScrollEnabled = false;
            NotesTextView.Delegate = new DynamicHeightTextViewDelegate();
            NotesSeparatorView.BackgroundColor = new UIColor(0.902f, 0.902f, 0.902f, 1.0f);
            FirstNameUnderlineView.BackgroundColor = Colors.GraySeparatorColor;
            EventErrorLabel.Text = L10n.Localize("NoEventSelectedError", "No event selected");
            EventErrorLabel.TextColor = Colors.Red;
            EventErrorLabel.Alpha = 0;
            AvatarIndicatorView.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
            AvatarIndicatorView.Hidden = true;
            AvatarIndicatorView.UserInteractionEnabled = false;
            AvatarIndicatorButton.AddSubview(AvatarIndicatorView);

            EmailImageView.Image = UIImage.FromBundle("lead_contacts");
            AddressImageView.Image = UIImage.FromBundle("lead_address");
            CompanyUrlImageView.Image = UIImage.FromBundle("lead_url");
            NotesImageView.Image = UIImage.FromBundle("lead_notes");
            EmailsTableView.RowHeight = EmailEditCell.RowHeight;
            EmailsTableView.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            EmailsTableView.ScrollEnabled = false;
            PhonesTableView.RowHeight = PhoneEditCell.RowHeight;
            PhonesTableView.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            PhonesTableView.ScrollEnabled = false;
            AddEmailInputButton.SetImage(UIImage.FromBundle("leads_plus"), UIControlState.Normal);
            AddPhoneInputButton.SetImage(UIImage.FromBundle("leads_plus"), UIControlState.Normal);
            AvatarPreview.Layer.MasksToBounds = true;

            BusinessCardSeparator.BackgroundColor = Colors.GraySeparatorColor;
            BusinessCardBottomSeparator.BackgroundColor = Colors.GraySeparatorColor;
            BusinessCardLabel.Text = L10n.Localize("BusinessCardLabel", "Business card");
            CountryInput.ValueTextField.InputView = CountryPicker;
        }

        public void ShowEventErrorAnimated(bool show)
        {
            if (show == true) Animate(0.7, () =>
            {
                EventErrorLabel.Alpha = 1;
                EventSelectButton.KeyValueInfoView.SeparatorView.BackgroundColor = Colors.Red;
            });
            else Animate(0.7, () =>
            {
                EventErrorLabel.Alpha = 0;
                EventSelectButton.KeyValueInfoView.SeparatorView.BackgroundColor = new UIColor(0.902f, 0.902f, 0.902f, 1.0f);
            });
        }

        public void SetSendResourcesEnabled(bool enabled)
        {
            SendResourcesButton.UserInteractionEnabled = enabled;
            if (enabled) SendResourcesButton.BackgroundColor = Colors.MainBlueColor;
            else SendResourcesButton.BackgroundColor = Colors.LightGray;
        }

        public void UpdateCrmButton(CrmService.CrmType type, LeadDetailsViewModel.CrmExportStates state)
        {
            // TODO: add images from design
            if (type == CrmService.CrmType.Other) CrmExportButton.Hidden = true;
            else if (type == CrmService.CrmType.Dynamics365)
            {
                if (state == LeadDetailsViewModel.CrmExportStates.NotExported)
                    CrmExportButton.SetImage(UIImage.FromBundle("CRM_off"), UIControlState.Normal);
                else
                    CrmExportButton.SetImage(UIImage.FromBundle("CRM_on"), UIControlState.Normal);
            }
            else if (type == CrmService.CrmType.Salesforce)
            {
                if (state == LeadDetailsViewModel.CrmExportStates.NotExported)
                    CrmExportButton.SetImage(UIImage.FromBundle("sf_off"), UIControlState.Normal);
                else
                    CrmExportButton.SetImage(UIImage.FromBundle("sf_on"), UIControlState.Normal);
            }
        }

        IScheduledWork photoLoadingWork;

        public void SetPhotoResource(FileResource photo)
        {
            AvatarIndicatorButton.Hidden = false;
            AvatarIndicatorView.StartAnimating();

            if (photoLoadingWork != null)
            {
                photoLoadingWork.Cancel();
                photoLoadingWork = null;
            }
            var localPath = photo?.AbsoluteLocalPath;
            var remoteUrl = photo?.RemoteUrl;
            TaskParameter loadingSource = null;
            if (!String.IsNullOrWhiteSpace(photo.RemoteUrl))
            {
                loadingSource = ImageService.Instance.LoadUrl(remoteUrl);
            }
            else if (!String.IsNullOrWhiteSpace(localPath))
            {
                loadingSource = ImageService.Instance.LoadFile(localPath);
            }
            if (loadingSource != null)
            {
                photoLoadingWork = loadingSource
                        .DownSample((int)AvatarPreview.Bounds.Width, (int)AvatarPreview.Bounds.Height)
                        .Transform(new CircleTransformation())
                        .Finish((obj) =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                AvatarIndicatorButton.Hidden = true;
                                AvatarIndicatorView.StopAnimating();
                            });
                        })
                        .Into(AvatarPreview);
                AddAvatarButton.Hidden = true;
                return;
            }
            AddAvatarButton.Hidden = false;
            AvatarIndicatorButton.Hidden = true;
            AvatarIndicatorView.StopAnimating();
        }

        public bool NeedsCellBecomeFirstResponder { get; set; } = false;
        public PlainUITableViewBinding<LeadDetailsEmailViewModel> GetEmailInputsBinding(ObservableList<LeadDetailsEmailViewModel> emailItems, Func<List<string>, Task<string>> onEmailTypeButtonClick, Action emailStringChanged)
        {
            var tableViewBinding = new PlainUITableViewBinding<LeadDetailsEmailViewModel>
            {
                DataSource = emailItems,
                CellFactory = (UITableView tableView, LeadDetailsEmailViewModel item, int index) =>
                {
                    var cell = tableView.DequeueReusableCell(EmailEditCell.DefaultCellIdentifier) as EmailEditCell;
                    if (cell == null) cell = new EmailEditCell(EmailEditCell.DefaultCellIdentifier);
                    cell.EmailTextField.Delegate = this;
                    cell.SetupCell(item, onEmailTypeButtonClick, SetNeedsLayoutAsync, emailStringChanged);
                    return cell;
                },
                WillDisplayCell = (tableView, cell, indexPath) =>
                {
                    if (!NeedsCellBecomeFirstResponder) return;
                    ((EmailEditCell)cell).EmailTextField.BecomeFirstResponder();
                },
                TableView = EmailsTableView
            };
            tableViewBinding.DeleteAnimation = UITableViewRowAnimation.None;
            tableViewBinding.AddAnimation = UITableViewRowAnimation.None;
            return tableViewBinding;
        }

        public PlainUITableViewBinding<LeadDetailsPhoneViewModel> GetPhoneInputsBinding(ObservableList<LeadDetailsPhoneViewModel> phoneItems, Func<List<string>, Task<string>> onPhoneTypeButtonClick, Action phoneStringChanged)
        {
            var tableViewBinding = new PlainUITableViewBinding<LeadDetailsPhoneViewModel>
            {
                DataSource = phoneItems,
                CellFactory = (UITableView tableView, LeadDetailsPhoneViewModel item, int index) =>
                {
                    var cell = tableView.DequeueReusableCell(PhoneEditCell.DefaultCellIdentifier) as PhoneEditCell;
                    if (cell == null) cell = new PhoneEditCell(PhoneEditCell.DefaultCellIdentifier);
                    cell.PhoneTextField.Delegate = this;
                    cell.SetupCell(item, onPhoneTypeButtonClick, SetNeedsLayoutAsync, phoneStringChanged);
                    return cell;
                },
                WillDisplayCell = (tableView, cell, indexPath) =>
                {
                    if (!NeedsCellBecomeFirstResponder) return;
                    ((PhoneEditCell)cell).PhoneTextField.BecomeFirstResponder();
                },
                TableView = PhonesTableView,
                DeleteAnimation = UITableViewRowAnimation.None,
                AddAnimation = UITableViewRowAnimation.None
            };
            return tableViewBinding;
        }

        public void SetFrontBusinessCardButtonEnabled(bool enabled)
        {
            FrontBusinessCardButton.UserInteractionEnabled = enabled;
            if (enabled) FrontBusinessCardButton.SetImage(UIImage.FromBundle("bcard_on"), UIControlState.Normal);
            else FrontBusinessCardButton.SetImage(UIImage.FromBundle("bcard_off"), UIControlState.Normal);
        }

        public void SetBackBusinessCardButtonEnabled(bool enabled)
        {
            BackBusinessCardButton.UserInteractionEnabled = enabled;
            if (enabled) BackBusinessCardButton.SetImage(UIImage.FromBundle("bcard_on"), UIControlState.Normal);
            else BackBusinessCardButton.SetImage(UIImage.FromBundle("bcard_off"), UIControlState.Normal);
        }

        public void SetNeedsLayoutAsyncNoWait()
        {
            SetNeedsLayoutAsync(null).Ignore();
        }

        async Task SetNeedsLayoutAsync(UIView fromView)
        {
            await Task.Yield();
            SetNeedsLayout();
            if (fromView != null) fromView.EndEditing(true);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            AddAvatarButton.SizeToFit();
            EmailImageView.SizeToFit();
            EmailLabel.SizeToFit();
            AddEmailInputButton.SizeToFit();
            PhoneLabel.SizeToFit();
            AddPhoneInputButton.SizeToFit();
            AddressImageView.SizeToFit();
            CompanyUrlImageView.SizeToFit();
            NotesImageView.SizeToFit();
            NotesLabel.SizeToFit();
            NotesTextView.SizeToFit();
            LocationTitleLabel.SizeToFit();
            LocationLabel.SizeToFit();
            EventErrorLabel.SizeToFit();
            BusinessCardLabel.SizeToFit();
            FrontBusinessCardButton.SizeToFit();
            BackBusinessCardButton.SizeToFit();
            AddBusinessCardButton.SizeToFit();
            CrmExportButton.SizeToFit();

            EventSelectButton.Frame = this.LayoutBox()
                .Top(7)
                .Left(15)
                .Right(0)
                .Height(60);

            EventErrorLabel.Frame = this.LayoutBox()
                .Below(EventSelectButton, 0)
                .Left(15)
                .Width(EventErrorLabel.Bounds.Width)
                .Height(EventErrorLabel.Bounds.Height);

            AddAvatarButton.Frame = this.LayoutBox()
                .Below(EventSelectButton, 97)
                .Right(15)
                .Width(AddAvatarButton.Bounds.Width)
                .Height(AddAvatarButton.Bounds.Height);

            FirstNameInput.Frame = this.LayoutBox()
                .Below(EventSelectButton, 78)
                .Left(15)
                .Before(AddAvatarButton, 5)
                .Height(60);

            FirstNameUnderlineView.Frame = this.LayoutBox()
                .Below(FirstNameInput, -1)
                .Left(15)
                .Right(0)
                .Height(1);

            CrmExportButton.Frame = this.LayoutBox()
                .Above(AddAvatarButton, 30)
                .Right(15)
                .Width(CrmExportButton.Bounds.Width)
                .Height(CrmExportButton.Bounds.Height);

            AvatarPreview.Frame = this.LayoutBox()
                .Above(FirstNameUnderlineView, 5)
                .Right(25)
                .Width(36)
                .Height(36);
            AvatarPreview.Layer.CornerRadius = 18;

            AvatarIndicatorButton.Frame = this.LayoutBox()
               .Above(FirstNameUnderlineView, 5)
               .Right(25)
               .Width(36)
               .Height(36);
            AvatarIndicatorView.Frame = AvatarIndicatorButton.Bounds;

            LastNameInput.Frame = this.LayoutBox()
                .Below(FirstNameInput, 6)
                .Left(15)
                .Right(0)
                .Height(60);

            var businessCardSectionWidth = BusinessCardLabel.Bounds.Width + AddBusinessCardButton.Bounds.Width + 23;
            CompanyInput.Frame = this.LayoutBox()
                .Below(LastNameInput, 6)
                .Left(15)
                .Right(businessCardSectionWidth)
                .Height(60);

            AddBusinessCardButton.Frame = this.LayoutBox()
                .Right(15)
                .Bottom(CompanyInput, 9)
                .Width(AddBusinessCardButton.Bounds.Width)
                .Height(AddBusinessCardButton.Bounds.Height);

            BusinessCardLabel.Frame = this.LayoutBox()
                .Before(AddBusinessCardButton, 1)
                .Above(AddBusinessCardButton, 1)
                .Width(BusinessCardLabel.Bounds.Width)
                .Height(BusinessCardLabel.Bounds.Height);

            BusinessCardSeparator.Frame = this.LayoutBox()
                .Before(BusinessCardLabel, 7)
                .Top(CompanyInput, 6)
                .Width(1)
                .Height(44);

            BusinessCardBottomSeparator.Frame = this.LayoutBox()
                .Bottom(CompanyInput, 0)
                .Left(BusinessCardSeparator, 0)
                .Right(0)
                .Height(1);

            FrontBusinessCardButton.Frame = this.LayoutBox()
                .Left(BusinessCardLabel, 0)
                .CenterVertically(AddBusinessCardButton)
                .Width(FrontBusinessCardButton.Bounds.Width)
                .Height(FrontBusinessCardButton.Bounds.Height);

            BackBusinessCardButton.Frame = this.LayoutBox()
                .After(FrontBusinessCardButton, 8)
                .CenterVertically(AddBusinessCardButton)
                .Width(BackBusinessCardButton.Bounds.Width)
                .Height(BackBusinessCardButton.Bounds.Height);

            TitleInput.Frame = this.LayoutBox()
                .Below(CompanyInput, 6)
                .Left(15)
                .Right(0)
                .Height(60);

            EmailImageView.Frame = this.LayoutBox()
                .Below(TitleInput, 18)
                .Left(15)
                .Width(EmailImageView.Bounds.Width)
                .Height(EmailImageView.Bounds.Height);

            EmailLabel.Frame = this.LayoutBox()
                .Below(EmailImageView, 8)
                .Left(15)
                .Width(EmailLabel.Bounds.Width)
                .Height(EmailLabel.Bounds.Height);

            EmailsTableView.ContentInset = new UIEdgeInsets(1, 0, 0, 0);
            EmailsTableView.Frame = this.LayoutBox()
                .Below(EmailLabel, 0)
                .Left(15)
                .Right(0)
                .Height(EmailsTableView.ContentSize.Height + 1);

            AddEmailInputButton.Frame = this.LayoutBox()
                .Below(EmailsTableView, 5)
                .Right(5)
                .Height(42)
                .Width(42);

            SendResourcesButton.Frame = this.LayoutBox()
                .Below(AddEmailInputButton, 5)
                .CenterHorizontally()
                .Height(40)
                .Width(165);

            PhoneLabel.Frame = this.LayoutBox()
                .Below(SendResourcesButton, 15)
                .Left(15)
                .Width(PhoneLabel.Bounds.Width)
                .Height(PhoneLabel.Bounds.Height);

            PhonesTableView.ContentInset = new UIEdgeInsets(1, 0, 0, 0);
            PhonesTableView.Frame = this.LayoutBox()
                .Below(PhoneLabel, 0)
                .Left(15)
                .Right(0)
                .Height(PhonesTableView.ContentSize.Height + 1);

            AddPhoneInputButton.Frame = this.LayoutBox()
                .Below(PhonesTableView, 5)
                .Right(5)
                .Height(42)
                .Width(42);

            AddressImageView.Frame = this.LayoutBox()
                .Below(AddPhoneInputButton, 10)
                .Left(15)
                .Width(AddressImageView.Bounds.Width)
                .Height(AddressImageView.Bounds.Height);

            AddressInput.Frame = this.LayoutBox()
                .Below(AddressImageView, 1)
                .Left(15)
                .Right(0)
                .Height(60);

            CityInput.Frame = this.LayoutBox()
                .Below(AddressInput, 6)
                .Left(15)
                .Width(Bounds.Width * 0.5f - 15)
                .Height(60);

            StateInput.Frame = this.LayoutBox()
                .Below(AddressInput, 6)
                .After(CityInput, 15)
                .Right(0)
                .Height(60);

            ZipInput.Frame = this.LayoutBox()
                .Below(CityInput, 6)
                .Left(15)
                .Width(Bounds.Width * 0.3f - 15)
                .Height(60);

            CountryInput.Frame = this.LayoutBox()
                .Below(StateInput, 6)
                .After(ZipInput, 15)
                .Right(0)
                .Height(60);

            CompanyUrlImageView.Frame = this.LayoutBox()
                .Below(ZipInput, 17)
                .Left(15)
                .Width(CompanyUrlImageView.Bounds.Width)
                .Height(CompanyUrlImageView.Bounds.Height);

            CompanyUrlInput.Frame = this.LayoutBox()
                .Below(CompanyUrlImageView, 0)
                .Left(15)
                .Right(0)
                .Height(60);

            NotesImageView.Frame = this.LayoutBox()
                .Below(CompanyUrlInput, 16)
                .Left(15)
                .Width(NotesImageView.Bounds.Width)
                .Height(NotesImageView.Bounds.Height);

            NotesLabel.Frame = this.LayoutBox()
                .Below(NotesImageView, 3)
                .Left(15)
                .Width(NotesLabel.Bounds.Width)
                .Height(NotesLabel.Bounds.Height);

            NotesTextView.Frame = this.LayoutBox()
                .Below(NotesLabel, 0)
                .Left(15)
                .Right(0)
                .Height((float)Math.Max(NotesTextView.Bounds.Height, NotesTextView.IntrinsicContentSize.Height));

            NotesSeparatorView.Frame = this.LayoutBox()
                .Below(NotesTextView, 0)
                .Height(1)
                .Left(15)
                .Right(0);

            LocationTitleLabel.Frame = this.LayoutBox()
                .Below(NotesSeparatorView, 25)
                .Left(15)
                .Width(LocationTitleLabel.Bounds.Width)
                .Height(LocationTitleLabel.Bounds.Height);

            LocationLabel.Frame = this.LayoutBox()
                .Below(LocationTitleLabel, 5)
                .Left(15)
                .Right(15)
                .Width(Bounds.Width - 30)
                .Height(LocationLabel.Font.LineHeight * 2);
        }

        public override CGSize SizeThatFits(CGSize size)
        {
            LayoutSubviews();
            nfloat height;
            height = LocationLabel.Frame.GetMaxY();
            height += 17;

            return new CGSize(size.Width, height);
        }

        #region IUITextFieldDelegate

        [Export("textFieldShouldReturn:")]
        public bool ShouldReturn(UITextField textField)
        {
            var topFields = new List<UIView>() { FirstNameInput.ValueTextField, LastNameInput.ValueTextField, CompanyInput.ValueTextField, TitleInput.ValueTextField };
            var bottomFields = new List<UIView>() { AddressInput.ValueTextField, CityInput.ValueTextField, StateInput.ValueTextField, ZipInput.ValueTextField, CountryInput.ValueTextField,
                CompanyUrlInput.ValueTextField, NotesTextView };
            var emailFields = EmailsTableView.VisibleCells.Select(_ => ((EmailEditCell)_).EmailTextField);
            var phoneFields = PhonesTableView.VisibleCells.Select(_ => ((PhoneEditCell)_).PhoneTextField);
            var fields = topFields.Concat(emailFields).Concat(phoneFields).Concat(bottomFields).ToList();

            int index = fields.FindIndex((obj) => obj == textField);
            if (index == -1) return true;
            else if (index + 1 >= fields.Count) ResignFirstResponder();
            else fields[index + 1].BecomeFirstResponder();

            return true;
        }

        [Export("textFieldDidEndEditing:")]
        public void EditingEnded(UITextField textField)
        {
            textField.Text = textField.Text.Trim();
        }

        #endregion
    }

    public class DynamicHeightTextViewDelegate : UITextViewDelegate
    {

        public override void Changed(UITextView textView)
        {
            var width = textView.Bounds.Width;
            var newSize = textView.SizeThatFits(new CGSize(width, 10000));
            var newFrame = textView.Frame;
            newFrame.Size = new CGSize((float)Math.Max(newSize.Width, width), newSize.Height);
            textView.Frame = newFrame;
        }

        public override void EditingEnded(UITextView textView)
        {
            textView.Text = textView.Text.Trim();
            var width = textView.Bounds.Width;
            var frame = textView.Frame;
            frame.Size = textView.SizeThatFits(new CGSize(width, 10000));
            textView.Frame = frame;
        }
    }
}

