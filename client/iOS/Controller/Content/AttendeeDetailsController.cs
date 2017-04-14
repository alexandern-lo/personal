using System;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Models.Data;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;
using LiveOakApp.iOS.Controller.Content;
using Foundation;

namespace LiveOakApp.iOS
{
    public class AttendeeDetailsController : CustomController<AttendeeDetailsView>
    {
        AttendeeDetailsViewModel ViewModel { get; set; }
        public Command LeadCreateCommand { get; private set; }
        public Command InfoItemClickCommand { get; private set; }

        public AttendeeDetailsController(AttendeeViewModel item, EventViewModel eventViewModel)
        {
            Title = L10n.Localize("AttendeeDetailsNavigationBarTitle", "Details");
            ViewModel = new AttendeeDetailsViewModel(item, eventViewModel);

            LeadCreateCommand = new Command()
            {
                Action = LeadCreateAction
            };

            InfoItemClickCommand = new Command()
            {
                Action = InfoItemClickAction
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationItem.BackBarButtonItem = CommonSkin.BackBarButtonItem;

            var infoItemsBinding = View.GetSectionsBinding(new ObservableList<ObservableList<AttendeeInfoItemViewModel>>() { ViewModel.AttendeeInfoList });
            Bindings.Command(InfoItemClickCommand).To(infoItemsBinding.ItemSelectedTarget());
            Bindings.Add(infoItemsBinding);

            Bindings.Property(ViewModel, _ => _.AttendeeAvatar).To(View.TableHeader.AvatarRemoteImageView.ImageProperty());
            Bindings.Property(ViewModel, _ => _.AttendeeName).To(View.SectionHeader.FullNameLabel.TextProperty());
            Bindings.Property(ViewModel, _ => _.AttendeeTitle).To(View.SectionHeader.PositionLabel.TextProperty());
            Bindings.Property(ViewModel, _ => _.AttendeeCompany).To(View.SectionHeader.CompanyLabel.TextProperty());
            Bindings.Command(LeadCreateCommand).To(View.AddButton.ClickTarget());
        }

        void LeadCreateAction(object obj)
        {
            LeadDetailsController leadDetailsController = new LeadDetailsController(ViewModel.AttendeeViewModel, ViewModel.Event, () => NavigationController.PopToViewController(this, true));
            NavigationController.PushViewController(leadDetailsController, true);
        }

        void InfoItemClickAction(object obj)
        {
            AttendeeInfoItemViewModel infoItem = (AttendeeInfoItemViewModel)obj;
            if (infoItem.Type == AttendeeDetailsViewModel.InfoType.Phone)
            {
                NSUrl url = NSUrl.FromString("tel:" + infoItem.Value);
                if (url == null)
                {
                    var alert = UIAlertController.Create(L10n.Localize("WrongPhoneFormat", "Wrong phone number format"), null, UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
                    PresentViewController(alert, true, null);
                    return;
                }
                UIApplication.SharedApplication.OpenUrl(url);
            }
            if (infoItem.Type == AttendeeDetailsViewModel.InfoType.Email)
            {
                NSUrl url = NSUrl.FromString("mailto:" + infoItem.Value);
                if (url == null)
                {
                    var alert = UIAlertController.Create(L10n.Localize("WrongEmailFormat", "Wrong E-mail format"), null, UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
                    PresentViewController(alert, true, null);
                    return;
                }
                UIApplication.SharedApplication.OpenUrl(url);
            }
        }

    }
}

