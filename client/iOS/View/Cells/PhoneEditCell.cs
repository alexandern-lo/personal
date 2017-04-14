using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UIKit;
using StudioMobile;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.ViewModels;
using Foundation;

namespace LiveOakApp.iOS.View.Cells
{
    public class PhoneEditCell : CustomBindingsTableViewCell, IUITextFieldDelegate
    {
        public const string DefaultCellIdentifier = "PhoneEditCell";
        public AsyncCommand PhoneTypeChooseCommand { get; private set; }
        Func<List<string>, Task<string>> OnPhoneTypeButtonClick { get; set; }
        Action PhoneStringChanged { get; set; }
        public Func<UIView,Task> AfterRemoveFunc { get; set; }

        [View(0)]
        [ButtonSkin("LeadCellChooseButton")]
        public UIButton PhoneTypeButton { get; private set; }

        [View(1)]
        public UIView SeparatorView { get; private set; }

        [View(2)]
        public UITextField PhoneTextField { get; private set; }

        [View(3)]
        public UIButton CellRemoveButton { get; private set; }

        LeadDetailsPhoneViewModel phone;
        public LeadDetailsPhoneViewModel Phone
        {
            get { return phone; }
            private set
            {
                phone = value;
                Bindings.Clear();
                Bindings.Unbind();
                Bindings.Command(Phone.RemovePhoneCommand).To(CellRemoveButton.ClickTarget())
                        .AfterExecute((target, command) => AfterRemoveFunc(this).Ignore());
                Bindings.Property(Phone, _ => _.TypeString).UpdateTarget((source) => PhoneTypeButton.SetTitle(source.Value, UIControlState.Normal));
                Bindings.Property(Phone, _ => _.Phone).To(PhoneTextField.TextProperty());
                Bindings.Property(Phone, _ => _.Phone).UpdateTarget((source) => PhoneStringChanged());
                Bindings.Command(PhoneTypeChooseCommand).To(PhoneTypeButton.ClickTarget());
                Bindings.Bind();
                Bindings.UpdateTarget();
            }
        }

        public PhoneEditCell(string cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = UIColor.Clear;
            SelectedBackgroundView = new UIView();
            SelectionStyle = UITableViewCellSelectionStyle.None;
            LayoutMargins = UIEdgeInsets.Zero;
            PreservesSuperviewLayoutMargins = false;
            SeparatorView.BackgroundColor = UIColor.Black;
            PhoneTextField.Delegate = this;

            PhoneTextField.Font = Fonts.LargeRegular;
            PhoneTextField.TextColor = Colors.DarkGray;
            CellRemoveButton.SetImage(UIImage.FromBundle("leads_minus"), UIControlState.Normal);
            PhoneTextField.KeyboardType = UIKeyboardType.PhonePad;
            PhoneTextField.AutocorrectionType = UITextAutocorrectionType.No;

            PhoneTypeChooseCommand = new AsyncCommand()
            {
                Action = PhoneTypeChoose
            };
            PhoneStringChanged = () => { }; // stub when cell had not setup yet
        }

        public void SetupCell(LeadDetailsPhoneViewModel phone, Func<List<string>, Task<string>> onPhoneTypeButtonClick, Func<UIView,Task> afterRemoveFunc, Action phoneStringChanged)
        {
            Phone = phone;
            PhoneTypeButton.SetTitle(Phone.TypeString, UIControlState.Normal);
            PhoneTextField.Text = Phone.Phone;
            OnPhoneTypeButtonClick = onPhoneTypeButtonClick;
            PhoneStringChanged = phoneStringChanged;
            AfterRemoveFunc = afterRemoveFunc;
        }

        public async Task PhoneTypeChoose(object param)
        {
            string phoneType = await OnPhoneTypeButtonClick(new List<string>(Phone.PhoneTypes));
            if (phoneType != null) Phone.TypeString = phoneType;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CellRemoveButton.SizeToFit();
            var removeButtonWidth = CellRemoveButton.Bounds.Width;

            PhoneTypeButton.Frame = this.LayoutBox()
                .Left(0)
                .Width(75)
                .Top(0)
                .Bottom(0);

            SeparatorView.Frame = this.LayoutBox()
                .After(PhoneTypeButton, 0)
                .Width(1)
                .Height(Bounds.Height / 2)
                .CenterVertically();

            CellRemoveButton.Frame = this.LayoutBox()
                .Right(5)
                .Top(0)
                .Bottom(0)
                .Width(42);

            PhoneTextField.Frame = this.LayoutBox()
                .After(SeparatorView, 15)
                .Before(CellRemoveButton, 0)
                .Top(0)
                .Bottom(0);
        }

        public static float RowHeight
        {
            get { return 42.0f; }
        }

        #region IUITextFieldDelegate

        [Export("textFieldDidEndEditing:")]
        public void EditingEnded(UITextField textField)
        {
            textField.Text = textField.Text.Trim();
        }
        #endregion
    }
}

