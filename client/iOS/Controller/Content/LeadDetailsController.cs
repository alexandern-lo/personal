using System;
using System.Threading.Tasks;
using UIKit;
using StudioMobile;
using CoreGraphics;
using LiveOakApp.iOS.View.Content;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Resources;

namespace LiveOakApp.iOS.Controller.Content
{
    public class LeadDetailsController : CustomController<LeadDetailsView>, IUIImagePickerControllerDelegate
    {
        public LeadDetailsViewModel ViewModel { get; set; }

        EventHandler OnActionsMenuClicked { get; set; }
        Action OnFinishedEditing { get; set; }
        UIImage businessCardPhotoToAdd { get; set; }

        public LeadDetailsController(UIImage businessCardPhoto, Action cancelAction) // new lead
        {
            ViewModel = new LeadDetailsViewModel();
            Initialize(cancelAction);
            businessCardPhotoToAdd = businessCardPhoto;
        }

        public LeadDetailsController(LeadViewModel lead, UIImage businessCardPhoto, Action cancelAction) // new lead
        {
            ViewModel = new LeadDetailsViewModel(lead.Lead);
            Initialize(cancelAction);
            Title = L10n.Localize("OldLeadControllerTitle", "Lead");
            businessCardPhotoToAdd = businessCardPhoto;
        }

        public LeadDetailsController(Action cancelAction) // new lead
        {
            ViewModel = new LeadDetailsViewModel();
            Initialize(cancelAction);
        }

        public LeadDetailsController(Card card, EventViewModel eventViewModel, Action cancelAction) // create lead from vCard
        {
            ViewModel = new LeadDetailsViewModel(card, eventViewModel);
            Initialize(cancelAction);
        }

        public LeadDetailsController(LeadViewModel lead, Action onFinishedEditing) // edit existing lead
        {
            ViewModel = new LeadDetailsViewModel(lead.Lead);
            Initialize(onFinishedEditing);
            Title = L10n.Localize("OldLeadControllerTitle", "Lead");
        }

        public LeadDetailsController(AttendeeViewModel attendee, EventViewModel eventViewModel, Action onFinishedEditing) // create lead from attendee
        {
            ViewModel = new LeadDetailsViewModel(attendee, eventViewModel);
            Initialize(onFinishedEditing);
        }

        void Initialize(Action onFinishedEditing)
        {
            Title = L10n.Localize("NewLeadControllerTitle", "New Lead");
            OnActionsMenuClicked += ShowActionsMenu;
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("navbar_dots"), UIBarButtonItemStyle.Plain, OnActionsMenuClicked);

            OpenContactTabCommand = new Command()
            {
                Action = OpenContactTabAction
            };
            OpenQualifyTabCommand = new Command()
            {
                Action = OpenQualifyTabAction
            };
            OnFinishedEditing = onFinishedEditing;
            TabController = new LeadContactController(ViewModel);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var backNavigationButton = new UIBarButtonItem( L10n.Localize("Back", "Back"), UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                if (ViewModel.HasValidFields)
                {
                    OnFinishedEditing();
                    return;
                }

                var alert = UIAlertController.Create(L10n.Localize("LeadChangesWillNotBeSavedTitle", "Confirm exit"),
                                                               L10n.Localize("LeadChangesWillNotBeSaved", "Missing required fields. Changes will not be saved."),
                                                               UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create(L10n.Localize("Ok", "Ok"), UIAlertActionStyle.Destructive, (arg) => OnFinishedEditing()));
                alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
                PresentViewController(alert, true, null);
            });
            NavigationItem.LeftBarButtonItem = backNavigationButton;
            Bindings.Command(OpenContactTabCommand).To(View.TopBarContactButton.ClickTarget());
            Bindings.Command(OpenQualifyTabCommand).To(View.TopBarQualifyButton.ClickTarget());
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (businessCardPhotoToAdd != null)
            {
                if (TabController is LeadContactController)
                    ((LeadContactController)TabController).CheckCardPhotoAndAddToLead(businessCardPhotoToAdd);
                businessCardPhotoToAdd = null;
            }
            ViewModel.StartDetectingLocationIfNeeded(() =>
            {
                var lm = new LocationManager();
                lm.Native.DistanceFilter = 50;
                return lm;
            }, () => new AddressGeocoder());
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            ViewModel.StopDetectingLocation();
        }

        UIViewController tabController;
        UIViewController TabController
        {
            get { return tabController; }
            set
            {
                if (tabController != null)
                {
                    tabController.WillMoveToParentViewController(null);
                    tabController.View.RemoveFromSuperview();
                    tabController.RemoveFromParentViewController();
                }
                tabController = value;
                if (tabController != null)
                {
                    AddChildViewController(tabController);
                    View.SetChildView(TabController.View);
                    tabController.DidMoveToParentViewController(this);
                }
                View.TopBarContactButton.SetActive(tabController is LeadContactController);
                View.TopBarQualifyButton.SetActive(tabController is LeadQualifyController);
            }
        }

        #region Commands

        public Command OpenContactTabCommand { get; private set; }
        void OpenContactTabAction(object obj)
        {
            if (TabController is LeadContactController) return;
            TabController = new LeadContactController(ViewModel);
        }

        public Command OpenQualifyTabCommand { get; private set; }
        void OpenQualifyTabAction(object obj)
        {
            if (TabController is LeadQualifyController) return;
            if (!ViewModel.EnsureCanSelectQualifyTab())
            {
                ((LeadContactController)tabController).View.EndEditing(true);
                ((LeadContactController)tabController).View.ContentScrollView.SetContentOffset(CGPoint.Empty, true);
                return;
            }
            TabController = new LeadQualifyController(ViewModel);
        }

        #endregion

        void ShowActionsMenu(object sender, EventArgs e)
        {
            UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);
            alert.AddAction(UIAlertAction.Create(L10n.Localize("Delete", "Delete"), UIAlertActionStyle.Destructive, (obj) =>
            {
                PerformDelete().Ignore();
            }));
            alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }

        async Task PerformDelete()
        {
            await ViewModel.DeleteLead.ExecuteAsync();
            OnFinishedEditing();
        }
    }
}
