using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using LiveOakApp.Models.ViewModels;
using StudioMobile;

namespace LiveOakApp.Droid.Views
{
    public class MainMenuView : RelativeLayout
	{
		public MainMenuView (Context context) :
			base (context)
		{
			Initialize ();
		}

		public MainMenuView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public MainMenuView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

        #region Properties and events

        public View DashboardButton { get; private set; }
        public View LeadsButton { get; private set; }
        public View EventsButton { get; private set; }
        public View AnalyticsButton { get; private set; }
        public View ResourcesButton { get; private set; }
        //public View InboxButton { get; private set; }
        //public TextView InboxCount { get; private set; }
        public View SupportButton { get; private set; }
        public View ProfileView { get; private set; }
        public View RecentActivityButton { get; private set; }


        MainMenuViewModel.MainMenuItem _selectedItem;
        public MainMenuViewModel.MainMenuItem SelectedItem 
        { 
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem == value) return;
                _selectedItem = value;
                ResetViewsState();
                HighlightSelectedView();
                if (SelectedItemChange != null)
                {
                    SelectedItemChange(this, new ItemSelectedEventArgs
                    {
                        ItemType = value
                    });
                }
            }
        }

        public event EventHandler<ItemSelectedEventArgs> SelectedItemChange;

        public class ItemSelectedEventArgs : EventArgs
        {
            public MainMenuViewModel.MainMenuItem ItemType { get; set; }
        }

        public static readonly RuntimeEvent SelectedItemChangeEvent = new RuntimeEvent(typeof(MainMenuView), "SelectedItemChange");
        public static readonly IPropertyBindingStrategy SelectedItemChangedBinding = new EventHandlerBindingStrategy<ItemSelectedEventArgs>(SelectedItemChangeEvent);

        public IProperty<MainMenuViewModel.MainMenuItem> SelectedItemProperty()
        {
            return this.GetProperty(_ => _.SelectedItem, SelectedItemChangedBinding);
        } 

        public EventHandlerSource<MainMenuView> ItemSelectedTarget()
        {
            return new EventHandlerSource<MainMenuView>(SelectedItemChangeEvent, this)
            {
                ParameterExtractor = (sender, args) => ((ItemSelectedEventArgs)args).ItemType
            };
        }

        #endregion

        private List<MenuItemView> items;

        void Initialize()
        {
            Inflate(Context, Resource.Layout.MainMenu, this);
            SetGravity(GravityFlags.Start);

            items = new List<MenuItemView>();

            SetBackgroundResource(Resource.Drawable.login_bg);

            DashboardButton = FindViewById(Resource.Id.main_menu_dashboard_button);
            var dashboardItem = new MenuItemView
            {
                Type = MainMenuViewModel.MainMenuItem.Dashboard,
                Background = DashboardButton,
                Icon = FindViewById<ImageView>(Resource.Id.main_menu_dashboard_icon),
                Text = FindViewById<TextView>(Resource.Id.main_menu_dashboard_text)
            };
            items.Add(dashboardItem);
            DashboardButton.Click += (sender, e) => SelectedItem = dashboardItem.Type;

            LeadsButton = FindViewById(Resource.Id.main_menu_leads_button);
            var leadsItem = new MenuItemView
            {
                Type = MainMenuViewModel.MainMenuItem.Leads,
                Background = LeadsButton,
                Icon = FindViewById<ImageView>(Resource.Id.main_menu_leads_icon),
                Text = FindViewById<TextView>(Resource.Id.main_menu_leads_text)
            };
            items.Add(leadsItem);
            LeadsButton.Click += (sender, e) => SelectedItem = leadsItem.Type;

            EventsButton = FindViewById(Resource.Id.main_menu_events_button);
            var eventsItem = new MenuItemView
            {
                Type = MainMenuViewModel.MainMenuItem.Events,
                Background = EventsButton,
                Icon = FindViewById<ImageView>(Resource.Id.main_menu_events_icon),
                Text = FindViewById<TextView>(Resource.Id.main_menu_events_text)
            };
            items.Add(eventsItem);
            EventsButton.Click += (sender, e) => SelectedItem = eventsItem.Type;

            RecentActivityButton = FindViewById(Resource.Id.main_menu_recent_button);
            var recentActivityItem = new MenuItemView
            {
                Type = MainMenuViewModel.MainMenuItem.RecentActivity,
                Background = RecentActivityButton,
                Icon = FindViewById<ImageView>(Resource.Id.main_menu_recent_icon),
                Text = FindViewById<TextView>(Resource.Id.main_menu_recent_text)
            };
            items.Add(recentActivityItem);
            RecentActivityButton.Click += (sender, e) => SelectedItem = recentActivityItem.Type;

            /*
            InboxButton = FindViewById(Resource.Id.main_menu_inbox_button);
			InboxCount = FindViewById<TextView> (Resource.Id.main_menu_inbox_count);
            var inboxItem = new MenuItemView
            {
                Type = MainMenuViewModel.MainMenuItem.Inbox,
                Background = InboxButton,
                Icon = FindViewById<ImageView>(Resource.Id.main_menu_inbox_icon),
                Text = FindViewById<TextView>(Resource.Id.main_menu_inbox_text)
            };
            items.Add(inboxItem);
            InboxButton.Click += (sender, e) => SelectedItem = inboxItem.Type;
            */
            ResourcesButton = FindViewById(Resource.Id.main_menu_my_resources_button);
            var resourcesItem = new MenuItemView
            {
                Type = MainMenuViewModel.MainMenuItem.MyResources,
                Background = ResourcesButton,
                Icon = FindViewById<ImageView>(Resource.Id.main_menu_my_resources_icon),
                Text = FindViewById<TextView>(Resource.Id.main_menu_my_resources_text)
            };
            items.Add(resourcesItem);
            ResourcesButton.Click += (sender, e) => SelectedItem = resourcesItem.Type;

            SupportButton = FindViewById(Resource.Id.main_menu_support_button);
            var supportItem = new MenuItemView
            {
                Type = MainMenuViewModel.MainMenuItem.Support,
                Background = SupportButton,
                Icon = FindViewById<ImageView>(Resource.Id.main_menu_support_icon),
                Text = FindViewById<TextView>(Resource.Id.main_menu_support_text)
            };
            items.Add(supportItem);
            SupportButton.Click += (sender, e) => SelectedItem = supportItem.Type;

            ProfileView = FindViewById(Resource.Id.menuProfile);
            var profileItem = new MenuItemView
            {
                Type = MainMenuViewModel.MainMenuItem.Profile
            };
            items.Add(profileItem);
            ProfileView.Click += (sender, e) => SelectedItem = profileItem.Type;
        }

        void ResetViewsState()
        {
            var whiteColor = new Color(ContextCompat.GetColor(Context, Android.Resource.Color.White));

            foreach (var item in items)
            {
                item.Text?.SetTextColor(whiteColor);
                item.Icon?.ClearColorFilter();
                if (item.Background != null)
                    item.Background.SetBackgroundResource(Resource.Drawable.menu_selector);
            }
        }

        void HighlightSelectedView()
        {
            var blueColor = new Color(ContextCompat.GetColor(Context, Resource.Color.primary_blue));
            var darkTransparentColor = new Color(ContextCompat.GetColor(Context, Resource.Color.primary_transparent_dark));

            var selectedItem = items.Find((item) => item.Type.Equals(SelectedItem));

            if(selectedItem.Background != null)
                selectedItem.Background.Background = new ColorDrawable(darkTransparentColor);
            selectedItem.Text?.SetTextColor(blueColor);
            selectedItem.Icon?.SetColorFilter(blueColor);
        }

        class MenuItemView
        {
            public MainMenuViewModel.MainMenuItem Type { get; set; }
            public View Background { get; set; }
            public TextView Text { get; set; }
            public ImageView Icon { get; set; }
        }
	}
}

