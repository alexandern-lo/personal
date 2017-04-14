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
    public class EmailEditCell : CustomBindingsTableViewCell, IUITextFieldDelegate
    {
        public const string DefaultCellIdentifier = "EmailEditCell";
        public AsyncCommand EmailTypeChooseCommand { get; private set; }
        Func<List<string>, Task<string>> OnEmailTypeButtonClick { get; set; }
        Action EmailStringChanged { get; set; }
        public Func<UIView, Task> AfterRemoveFunc { get; set; }

        [View(0)]
        [ButtonSkin("LeadCellChooseButton")]
        public UIButton EmailTypeButton { get; private set; }

        [View(1)]
        public UIView SeparatorView { get; private set; }

        [View(2)]
        public UITextField EmailTextField { get; private set; }

        [View(3)]
        public UIButton CellRemoveButton { get; private set; }

        [View(4)]
        public UIView ErrorUnderline { get; private set; }

        LeadDetailsEmailViewModel email;
        public LeadDetailsEmailViewModel Email
        {
            get { return email; }
            private set
            {
                email = value;
                Bindings.Clear();
                Bindings.Unbind();
                Bindings.Command(Email.RemoveEmailCommand).To(CellRemoveButton.ClickTarget())
                        .AfterExecute((target, command) => AfterRemoveFunc(this).Ignore());
                Bindings.Property(Email, _ => _.TypeString).UpdateTarget((source) => EmailTypeButton.SetTitle(source.Value, UIControlState.Normal));
                Bindings.Property(Email, _ => _.Email).To(EmailTextField.TextProperty());
                Bindings.Property(Email, _ => _.Email).UpdateTarget((source) => EmailStringChanged());
                Bindings.Command(EmailTypeChooseCommand).To(EmailTypeButton.ClickTarget());
                Bindings.Bind();
                Bindings.UpdateTarget();
            }
        }

        public EmailEditCell(string cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = UIColor.Clear;
            SelectedBackgroundView = new UIView();
            SelectionStyle = UITableViewCellSelectionStyle.None;
            LayoutMargins = UIEdgeInsets.Zero;
            PreservesSuperviewLayoutMargins = false;
            SeparatorView.BackgroundColor = UIColor.Black;

            EmailTextField.Font = Fonts.LargeRegular;
            EmailTextField.TextColor = Colors.DarkGray;
            CellRemoveButton.SetImage(UIImage.FromBundle("leads_minus"), UIControlState.Normal);
            EmailTextField.Delegate = this;
            EmailTextField.KeyboardType = UIKeyboardType.EmailAddress;
            EmailTextField.AutocorrectionType = UITextAutocorrectionType.No;
            EmailTextField.AutocapitalizationType = UITextAutocapitalizationType.None;
            ErrorUnderline.BackgroundColor = UIColor.Clear;

            EmailTypeChooseCommand = new AsyncCommand()
            {
                Action = EmailTypeChoose
            };
            EmailStringChanged = () => { }; // stub when cell had not setup yet
        }

        public void SetupCell(LeadDetailsEmailViewModel email, Func<List<string>, Task<string>> onEmailTypeButtonClick, Func<UIView, Task> afterRemoveFunc, Action emailStringChanged)
        {
            Email = email;
            EmailTypeButton.SetTitle(Email.TypeString, UIControlState.Normal);
            EmailTextField.Text = Email.Email;
            OnEmailTypeButtonClick = onEmailTypeButtonClick;
            EmailStringChanged = emailStringChanged;
            AfterRemoveFunc = afterRemoveFunc;
        }

        public async Task EmailTypeChoose(object param)
        {
            string emailType = await OnEmailTypeButtonClick(new List<string>(Email.EmailTypes));
            if (emailType != null) Email.TypeString = emailType;

        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CellRemoveButton.SizeToFit();

            EmailTypeButton.Frame = this.LayoutBox()
                .Left(0)
                .Width(75)
                .Top(0)
                .Bottom(0);

            SeparatorView.Frame = this.LayoutBox()
                .After(EmailTypeButton, 0)
                .Width(1)
                .Height(Bounds.Height / 2)
                .CenterVertically();

            CellRemoveButton.Frame = this.LayoutBox()
                .Right(5)
                .Top(0)
                .Bottom(0)
                .Width(42);

            EmailTextField.Frame = this.LayoutBox()
                .After(SeparatorView, 15)
                .Before(CellRemoveButton, 0)
                .Top(0)
                .Bottom(0);

            ErrorUnderline.Frame = this.LayoutBox()
                .Bottom(0)
                .Height(1)
                .Left(0)
                .Right(0);
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

