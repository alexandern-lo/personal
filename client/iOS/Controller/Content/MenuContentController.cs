using System;
using Foundation;
using LiveOakApp.iOS.TableSources;
using LiveOakApp.iOS.View;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.Controller.Content
{
    public class MenuContentController<T> : CustomController<T> where T : UIView, new()
    {
        public MenuContentController()
        {
            CommonInit();
        }

        public MenuContentController(SlideController slideController)
        {
            Slider = slideController;
            CommonInit();
            MenuBarButtonItem = CommonSkin.MenuBarButtonItem;
            NavigationItem.LeftBarButtonItem = MenuBarButtonItem;
        }

        void CommonInit()
        {
            NavigationItem.BackBarButtonItem = CommonSkin.BackBarButtonItem;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (MenuBarButtonItem != null)
            {
                MenuBarButtonItem.Clicked += MenuBarButtonItem_Clicked;
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            if (MenuBarButtonItem != null)
            {
                MenuBarButtonItem.Clicked -= MenuBarButtonItem_Clicked;
            }
        }

        public void MenuBarButtonItem_Clicked(object sender, EventArgs e)
        {
            Slider.PresentMenu(SlideLayoutLocation.Left);
        }

        public void ChooseMenuItem(MainMenuItemType menuItemType)
        {
            switch (menuItemType)
            {
                case MainMenuItemType.Dashboard: SelectMenuRow(NSIndexPath.FromRowSection(0, 0)); break;
                case MainMenuItemType.Leads: SelectMenuRow(NSIndexPath.FromRowSection(1, 0)); break;
                case MainMenuItemType.Events: SelectMenuRow(NSIndexPath.FromRowSection(2, 0)); break;
                case MainMenuItemType.RecentActivity: SelectMenuRow(NSIndexPath.FromRowSection(3, 0)); break;
                //case MainMenuItemType.Inbox: SelectMenuRow(NSIndexPath.FromRowSection(0, 1)); break;
                case MainMenuItemType.MyResources: SelectMenuRow(NSIndexPath.FromRowSection(0, 1)); break;
                case MainMenuItemType.Support: SelectMenuRow(NSIndexPath.FromRowSection(0, 2)); break;
                default: break;
            }
        }

        void SelectMenuRow(NSIndexPath indexPath)
        {
            ((MenuView)(Slider.LeftController.View)).MenuTableView.SelectRow(indexPath, false, UITableViewScrollPosition.None);
            ((MenuTableViewSource)(((MenuView)(Slider.LeftController.View)).MenuTableView.Source)).RowSelectedHandler(indexPath);
        }

        UIBarButtonItem MenuBarButtonItem { get; set; }
        SlideController Slider { get; set; }
    }
}
