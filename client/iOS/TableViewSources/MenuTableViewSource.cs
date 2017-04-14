using System;
using System.Collections.Generic;
using Foundation;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.Resources;
using UIKit;

namespace LiveOakApp.iOS.TableSources
{
    public enum MainMenuItemType
    {
        Dashboard,
        Leads,
        Events,
        RecentActivity,
        MyResources,
        Inbox,
        Profile,
        Support
    }

    public class MainMenuItem
    {
        public string Title { get; private set; }
        public MainMenuItemType Type { get; private set; }
        public UIImage Image { get; private set; }
        public UIImage ActiveImage { get; private set; }
        public object Info { get; private set; }

        public MainMenuItem(string title, MainMenuItemType type, UIImage image, UIImage activeImage, object info)
        {
            Title = title;
            Type = type;
            Image = image;
            ActiveImage = activeImage;
            Info = info;
        }
    }

    public class MenuTableViewSource : UITableViewSource
    {
        public event Action<MainMenuItem> MenuItemSelected;
        private List<MainMenuItem> FirstSectionMenuItems { get; set; }
        private List<MainMenuItem> SecondSectionMenuItems { get; set; }
        private List<MainMenuItem> ThirdSectionMenuItems { get; set; }
        private const float DefaultCellHeight = 70f;
        private const float DefaultSectionSpacingsHeight = 20f;

        public MenuTableViewSource()
        {
            FirstSectionMenuItems = new List<MainMenuItem>();
            SecondSectionMenuItems = new List<MainMenuItem>();
            ThirdSectionMenuItems = new List<MainMenuItem>();

            FirstSectionMenuItems.Add(new MainMenuItem(L10n.Localize("MenuDashboard", "Dashboard"),
                                           MainMenuItemType.Dashboard, UIImage.FromBundle("menu_dash"), UIImage.FromBundle("menu_dash_active"), null));
            FirstSectionMenuItems.Add(new MainMenuItem(L10n.Localize("MenuLeads", "Leads"),
                                           MainMenuItemType.Leads, UIImage.FromBundle("menu_leads"), UIImage.FromBundle("menu_leads_active"), null));
            FirstSectionMenuItems.Add(new MainMenuItem(L10n.Localize("MenuEvents", "Events"),
                                           MainMenuItemType.Events, UIImage.FromBundle("menu_events"), UIImage.FromBundle("menu_events_active"), null));
            FirstSectionMenuItems.Add(new MainMenuItem(L10n.Localize("MenuRecentActivity", "Recent activity"),
                                           MainMenuItemType.RecentActivity, UIImage.FromBundle("menu_recentactivity"), UIImage.FromBundle("menu_recentactivity_active"), null));
            /*
             * remove inbox section for a while, it could be restored soon
            SecondSectionMenuItems.Add(new MainMenuItem(L10n.Localize("MenuInbox", "Inbox"),
                                           MainMenuItemType.Inbox, UIImage.FromBundle("menu_inbox"), UIImage.FromBundle("menu_inbox_active"), null));
            */
            SecondSectionMenuItems.Add(new MainMenuItem(L10n.Localize("MenuMyResources", "My resources"),
                                           MainMenuItemType.MyResources, UIImage.FromBundle("menu_resources"), UIImage.FromBundle("menu_resources_active"), null));
            ThirdSectionMenuItems.Add(new MainMenuItem(L10n.Localize("MenuSupport", "Support"),
                                           MainMenuItemType.Support, UIImage.FromBundle("menu_support"), UIImage.FromBundle("menu_support_active"), null));
        }

        private MainMenuItem MenuItemAtIndexPath(NSIndexPath path)
        {
            switch (path.Section)
            {
                case 0:
                    return FirstSectionMenuItems[path.Row];

                case 1:
                    return SecondSectionMenuItems[path.Row];

                case 2:
                    return ThirdSectionMenuItems[path.Row];

                default:
                    return null;
            }
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return DefaultCellHeight;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(MenuItemCell.DefaultCellIdentifier) as MenuItemCell;
            if (cell == null)
                cell = new MenuItemCell(MenuItemCell.DefaultCellIdentifier);

            MainMenuItem currentMainMenuItem = MenuItemAtIndexPath(indexPath);
            cell.SetupCell(currentMainMenuItem);
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            RowSelectedHandler(indexPath);
        }

        public void RowSelectedHandler(NSIndexPath indexPath)
        {
            MainMenuItem currentMainMenuItem = MenuItemAtIndexPath(indexPath);
            MenuItemSelected.Invoke(currentMainMenuItem);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            switch (section)
            {
                case 0:
                    return FirstSectionMenuItems.Count;

                case 1:
                    return SecondSectionMenuItems.Count;

                case 2:
                    return ThirdSectionMenuItems.Count;

                default:
                    return 0;
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 3;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            if (section == 0) return 0;
            else return DefaultSectionSpacingsHeight;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIView sectionHeader = new UIView();
            sectionHeader.BackgroundColor = UIColor.FromWhiteAlpha(1, 0.3f);
            return sectionHeader;
        }

    }
}