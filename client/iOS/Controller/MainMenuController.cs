using System;
using System.Threading.Tasks;
using UIKit;
using Foundation;
using StudioMobile;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.iOS.TableSources;
using LiveOakApp.iOS.View;
using LiveOakApp.iOS.Controller.Content;
using LiveOakApp.iOS.Services;
using LiveOakApp.Resources;
using LiveOakApp.Models;
using System.Collections.Generic;
using System.Collections;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.iOS.Controller
{
    public class MainMenuController : CustomController<MenuView>, IUINavigationControllerDelegate
    {
        SlideController Slider { get; set; }
        MenuTableViewSource MenuSource { get; set; }
        MainMenuViewModel ViewModel { get; set; }

        bool FirstViewDidAppear = true;

        public static UIViewController CreateSliderController(CoreGraphics.CGRect frame)
        {
            var slider = new SlideController();
            slider.View.Frame = frame; // partial fix for presenting animation bug
            slider.LeftController = new MainMenuController(slider);
            slider.LeftWidth = (float)frame.Width * 0.816f;
            slider.LayoutView.Style = SlideTransitionStyle.Paginate;
            return slider;
        }

        public MainMenuController(SlideController slider)
        {
            Slider = slider;
            ViewModel = new MainMenuViewModel();
            ChangeContentController(MainMenuItemType.Dashboard);

            LogoutCommand = new Command()
            {
                Action = LogoutAction
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // TODO: use face db
            MenuSource = new MenuTableViewSource();
            View.MenuTableView.Source = MenuSource;

            Bindings.Command(LogoutCommand)
                    .To(View.LogoutButton.ClickTarget());
            Bindings.Property(ViewModel, _ => _.UserAvatar)
                    .To(View.AvatarImageView.ImageProperty());
            Bindings.Property(ViewModel, _ => _.UserFullName)
                    .To(View.UserNameLabel.TextProperty());

            MenuSource.MenuItemSelected += MenuItemSelected;
            View.ViewProfileButton.TouchUpInside += ViewProfileButton_TouchUpInside;
            ServiceLocator.Instance.MessagingCenter.Subscribe(LeadsExportReportDTO.SHOW_EXPORT_REPORT_EVENT_NAME, (sender, e) => ShowExportResultDialog(e.UserInfo));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            // TODO: fix double ViewWillAppear call in Slider (on creation and on menu show)

            if (View.MenuTableView.IndexPathForSelectedRow == null)
            {
                View.MenuTableView.SelectRow(NSIndexPath.FromRowSection(0, 0), false, UITableViewScrollPosition.Top);
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (!FirstViewDidAppear)
            {
                ShowLeadOverwriteDialogIfNeeded().Ignore();
            }
            FirstViewDidAppear = false;
        }

        private void MenuItemSelected(MainMenuItem menuId)
        {
            ChangeContentController(menuId.Type);
        }

        private void ChangeContentController(MainMenuItemType menuItemType)
        {
            var controller = ControllerForMenuItem(menuItemType);
            var navController = new UINavigationController(controller);

            navController.Delegate = this;
            Slider.ContentController = navController;
            Slider.DismissMenu();
        }

        public UIViewController ControllerForMenuItem(MainMenuItemType menuItemType)
        {
            UIViewController controller = null;
            switch (menuItemType)
            {
                case MainMenuItemType.Dashboard: controller = new DashboardController(Slider); break;
                case MainMenuItemType.Events: controller = new EventsController(Slider); break;
                case MainMenuItemType.RecentActivity: controller = new RecentActivityController(Slider); break;
                case MainMenuItemType.Inbox: controller = new InboxController(Slider); break;
                case MainMenuItemType.Leads: controller = new LeadsController(Slider); break;
                case MainMenuItemType.MyResources: controller = new MyResourcesController(Slider); break;
                case MainMenuItemType.Profile: controller = new ProfileController(Slider); break;
                case MainMenuItemType.Support: controller = new SupportController(Slider); break;
            }
            return controller;
        }

        #region Actions

        void ViewProfileButton_TouchUpInside(object sender, EventArgs e)
        {
            ChangeContentController(MainMenuItemType.Profile);
        }

        public Command LogoutCommand { get; private set; }
        void LogoutAction(object arg)
        {
            var alert = UIAlertController.Create(L10n.Localize("LogoutMessage", "Do you really want to logout?"), null, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
            alert.AddAction(UIAlertAction.Create(L10n.Localize("LogoutTitle", "Logout"), UIAlertActionStyle.Destructive, async (obj) =>
            {
                await ViewModel.LogoutCommand.ExecuteAsync();
                AfterLogout();
            }));
            PresentViewController(alert, true, null);
        }

        void AfterLogout()
        {
            IOSNavigationManager.Instance.NavigateToRequiredStateIfNeeded();
        }

        #endregion

        #region Lead overwrite dialog

        async Task ShowLeadOverwriteDialogIfNeeded()
        {
            var lead = await ViewModel.FindFirstLeadForOverwrite(null);
            if (lead == null) return;
            var alert = UIAlertController.Create(lead.DialogTitle, lead.DialogMessage, UIAlertControllerStyle.ActionSheet);
            Action<UIAlertAction> overwriteAction = (obj) =>
            {
                lead.OverwiteLeadChanges().Ignore();
            };
            alert.AddAction(UIAlertAction.Create(lead.OverwriteActionName, UIAlertActionStyle.Default, overwriteAction));
            Action<UIAlertAction> discardAction = (obj) =>
            {
                lead.DiscardLeadChanges().Ignore();
            };
            alert.AddAction(UIAlertAction.Create(lead.DiscardActionName, UIAlertActionStyle.Destructive, discardAction));
            alert.AddAction(UIAlertAction.Create(lead.CancelActionName, UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }

        #endregion

        #region Export result dialog

        void ShowExportResultDialog(IDictionary resultInfo)
        {
            var resultsDictionary = (Dictionary<string, IEnumerable>)resultInfo;
            if (resultsDictionary == null) return;
            string exportResultString = "";
            string successExportsString = "";
            string failedExportsString = "";

            ((List<LeadExportSuccessDetails>)resultsDictionary[LeadsExportReportDTO.EXPORT_CREATED_LIST_NAME]).ForEach((obj) =>
            {
                if (!String.IsNullOrWhiteSpace(obj.FirstName + " " + obj.LastName))
                    successExportsString += String.Format("{0} {1}", obj.FirstName, obj.LastName);
                else
                    successExportsString += obj.Email;
                successExportsString += "\n";
            });

            ((List<LeadExportSuccessDetails>)resultsDictionary[LeadsExportReportDTO.EXPORT_UPDATED_LIST_NAME]).ForEach((obj) =>
            {
                if (!String.IsNullOrWhiteSpace(obj.FirstName + " " + obj.LastName))
                    successExportsString += String.Format("{0} {1}", obj.FirstName, obj.LastName);
                else
                    successExportsString += obj.Email;
                successExportsString += "\n";
            });

            ((List<LeadExportFailureDetails>)resultsDictionary[LeadsExportReportDTO.EXPORT_FAILED_LIST_NAME]).ForEach((obj) =>
            {
                if (!String.IsNullOrWhiteSpace(obj.FirstName + " " + obj.LastName))
                    failedExportsString += String.Format("{0} {1}", obj.FirstName, obj.LastName);
                else
                    failedExportsString += obj.Email;
                failedExportsString += "\n" + obj.Reason + "\n\n";
            });

            if (!String.IsNullOrWhiteSpace(successExportsString))
                exportResultString += L10n.Localize("SuccessExportLeadsTitle", "Succesfully exported leads:") + "\n" + successExportsString;
            if (!String.IsNullOrWhiteSpace(failedExportsString))
            {
                if (!String.IsNullOrWhiteSpace(exportResultString)) exportResultString += "\n";
                exportResultString += L10n.Localize("FailedExportLeadsTitle", "Failed to export leads:") + "\n" + failedExportsString;
            }

            exportResultString.Trim();
            exportResultString += "\n\n" + L10n.Localize("PleaseRefreshLeadsList", "Please refresh your leads list to view changes");
            var alert = UIAlertController.Create(L10n.Localize("ExportResultTitle", "Export result"), exportResultString, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create(L10n.Localize("Ok", "Ok"), UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }

        #endregion

        #region IUINavigationControllerDelegate

        [Export("navigationController:didShowViewController:animated:")]
        public void DidShowViewController(UINavigationController navigationController, UIViewController viewController, bool animated)
        {
            Slider.GesturesEnabled = navigationController.ViewControllers.Length == 1;
        }

        #endregion
    }
}

