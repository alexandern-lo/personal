using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using LiveOakApp.Droid.Services;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using LiveOakApp.Models;

namespace LiveOakApp.Droid.Controller
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/icon", Theme = "@style/Theme.AppCompat.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class MainActivity : CustomActivity
    {
        public static void LaunchActivity(Activity previousActivity)
        {
            previousActivity.Finish();
            var intent = new Intent(previousActivity, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            previousActivity.StartActivity(intent);
        }

        DrawerLayout drawerLayout;
        MainMenuDrawerToggle drawerToggle;

        MainMenuView view;

        MainMenuViewModel model;

        public MainMenuViewModel ViewModel
        {
            get
            {
                return model;
            }
        }

        #region Initialization

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DroidNavigationManager.Instance.Activity = this;

            model = new MainMenuViewModel();

            SelectItemCommand = new Command
            {
                Action = SelectItem
            };

            SetContentView(Resource.Layout.MainActivity);

            view = FindViewById<MainMenuView>(Resource.Id.drawer_menu);

            SetupBindings(view, model);

            SetupActionBar();

            SetupDrawerLayout();

            SetupBackstackChangeListener();

            if (savedInstanceState == null)
            {
                view.SelectedItem = MainMenuViewModel.MainMenuItem.Dashboard;
                SelectItem(MainMenuViewModel.MainMenuItem.Dashboard);
            }

            ServiceLocator.Instance.MessagingCenter.Subscribe(LeadsExportReportDTO.SHOW_EXPORT_REPORT_EVENT_NAME, (sender, e) => ShowExportResultsDialog(e.UserInfo));
        }

        void SetupBackstackChangeListener()
        {
            SupportFragmentManager.BackStackChanged += (sender, e) =>
            {
                if (SupportFragmentManager.BackStackEntryCount > 0)
                {
                    drawerToggle.DrawerIndicatorEnabled = false;
                    drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
                }
                else
                {
                    drawerToggle.DrawerIndicatorEnabled = true;
                    drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
                }
                var fragment = SupportFragmentManager.FindFragmentById(Resource.Id.fragment_container) as CustomFragment;
                if (fragment != null)
                {
                    SupportActionBar.Title = fragment.Title;
                }
                else
                {
                    SupportActionBar.Title = Title;
                }

                var imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(Window.DecorView.ApplicationWindowToken, HideSoftInputFlags.None);
            };
        }

        void SetupBindings(MainMenuView view, MainMenuViewModel model)
        {
            Bindings.Command(SelectItemCommand)
                    .To(view.ItemSelectedTarget());
            /*
            Bindings.Property(model, _ => _.InboxCount)
                    .Convert((arg) => arg.ToString())
                    .To(view.InboxCount.TextProperty());
            */
        }

        void SetupActionBar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            toolbar.OverflowIcon = ContextCompat.GetDrawable(this, Resource.Drawable.navbar_dots);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.navbar_arrow);

            SupportActionBar.Title = Title;
        }

        void SetupDrawerLayout()
        {
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawerLayout.SetDrawerShadow(Resource.Drawable.drawer_shadow, GravityCompat.Start);
            drawerToggle = new MainMenuDrawerToggle(this, drawerLayout, Resource.String.open_drawer, Resource.String.close_drawer);
            drawerLayout.AddDrawerListener(drawerToggle);
        }

        #endregion

        #region Actions

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }

        public override void OnBackPressed()
        {
            if (SupportFragmentManager.BackStackEntryCount == 0 && !drawerLayout.IsDrawerOpen(view))
            {
                drawerLayout.OpenDrawer(view);
            }
            else if (SupportFragmentManager.FindFragmentById(Resource.Id.fragment_container) is LeadFragment)
            {
                var leadFragment = (LeadFragment)SupportFragmentManager.FindFragmentById(Resource.Id.fragment_container);
                if (!leadFragment.OnBackPressed())
                    base.OnBackPressed();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            drawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            drawerToggle.SyncState();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (drawerToggle.OnOptionsItemSelected(item))
            {
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        void OnMainMenuItemSelected(MainMenuViewModel.MainMenuItem item)
        {
            SelectItem(item);
        }

        public Command SelectItemCommand { get; private set; }
        void SelectItem(object argPosition)
        {
            var position = (MainMenuViewModel.MainMenuItem)argPosition;
            CustomFragment fragment = null;
            switch (position)
            {
                case MainMenuViewModel.MainMenuItem.Dashboard:
                    fragment = DashboardFragment.Create();
                    break;
                case MainMenuViewModel.MainMenuItem.Leads:
                    fragment = LeadsFragment.Create();
                    break;
                case MainMenuViewModel.MainMenuItem.Events:
                    fragment = EventsFragment.Create();
                    break;
                case MainMenuViewModel.MainMenuItem.Analytics:
                    fragment = new AnalyticsFragment();
                    break;
                case MainMenuViewModel.MainMenuItem.RecentActivity:
                    fragment = RecentFragment.Create();
                    break;
                case MainMenuViewModel.MainMenuItem.MyResources:
                    fragment = MyResourcesFragment.Create();
                    break;
                case MainMenuViewModel.MainMenuItem.Inbox:
                    fragment = new InboxFragment();
                    break;
                case MainMenuViewModel.MainMenuItem.Profile:
                    fragment = new ProfileFragment();
                    break;
                case MainMenuViewModel.MainMenuItem.Support:
                    fragment = SupportFragment.Create();
                    break;
            }
            if (fragment != null)
                ChangeFragment(fragment);
        }

        void ShowExportResultsDialog(IDictionary resultInfo)
        {
            var resultsDictionary = (Dictionary<string, IEnumerable>)resultInfo;
            if (resultsDictionary == null) return;
            string exportResultString = "";
            string successExportsString = "";
            string failedExportsString = "";

            ((List<LeadExportSuccessDetails>)resultsDictionary[LeadsExportReportDTO.EXPORT_CREATED_LIST_NAME]).ForEach((obj) =>
            {
                if (!string.IsNullOrWhiteSpace(obj.FirstName + " " + obj.LastName))
                    successExportsString += string.Format("{0} {1}", obj.FirstName, obj.LastName);
                else
                    successExportsString += obj.Email;
                successExportsString += "\n";
            });

            ((List<LeadExportSuccessDetails>)resultsDictionary[LeadsExportReportDTO.EXPORT_UPDATED_LIST_NAME]).ForEach((obj) =>
            {
                if (!string.IsNullOrWhiteSpace(obj.FirstName + " " + obj.LastName))
                    successExportsString += string.Format("{0} {1}", obj.FirstName, obj.LastName);
                else
                    successExportsString += obj.Email;
                successExportsString += "\n";
            });

            ((List<LeadExportFailureDetails>)resultsDictionary[LeadsExportReportDTO.EXPORT_FAILED_LIST_NAME]).ForEach((obj) =>
            {
                if (!string.IsNullOrWhiteSpace(obj.FirstName + " " + obj.LastName))
                    failedExportsString += string.Format("{0} {1}", obj.FirstName, obj.LastName);
                else
                    failedExportsString += obj.Email;
                failedExportsString += "\n" + obj.Reason + "\n\n";
            });

            if (!string.IsNullOrWhiteSpace(successExportsString))
                exportResultString += L10n.Localize("SuccessExportLeadsTitle", "Succesfully exported leads:") + "\n" + successExportsString;
            if (!string.IsNullOrWhiteSpace(failedExportsString))
            {
                if (!string.IsNullOrWhiteSpace(exportResultString)) exportResultString += "\n";
                exportResultString += L10n.Localize("FailedExportLeadsTitle", "Failed to export leads:") + "\n" + failedExportsString;
            }

            exportResultString.Trim();
            exportResultString += "\n\n" + L10n.Localize("PleaseRefreshLeadsList", "Please refresh your leads list to view changes");

            new Android.Support.V7.App.AlertDialog.Builder(this)
                       .SetTitle(L10n.Localize("ExportResultTitle", "Export result"))
                       .SetMessage(exportResultString)
                       .SetPositiveButton(L10n.Localize("Ok", "Ok"), (sender, e) => { })
                       .Create()
                       .Show();
        }

        #endregion

        public void ChangeFragment(CustomFragment newFragment)
        {
            if (CurrentFocus != null)
                UiUtil.hideKeyboard(CurrentFocus);
            EnsureFragmentTabIsHighlighted(newFragment);
            SupportFragmentManager.PopBackStack(null, 1); // 1 is "inclusive"
            SupportFragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container, newFragment)
                           .Commit();
            drawerLayout.CloseDrawers();
            SupportActionBar.Title = newFragment.Title;
        }

        void EnsureFragmentTabIsHighlighted(CustomFragment fragment)
        {
            if (fragment is DashboardFragment)
                view.SelectedItem = MainMenuViewModel.MainMenuItem.Dashboard;
            if (fragment is LeadsFragment)
                view.SelectedItem = MainMenuViewModel.MainMenuItem.Leads;
            if (fragment is EventsFragment)
                view.SelectedItem = MainMenuViewModel.MainMenuItem.Events;
            if (fragment is InboxFragment)
                view.SelectedItem = MainMenuViewModel.MainMenuItem.Inbox;
            if (fragment is MyResourcesFragment)
                view.SelectedItem = MainMenuViewModel.MainMenuItem.MyResources;
            if (fragment is SupportFragment)
                view.SelectedItem = MainMenuViewModel.MainMenuItem.Support;
            if (fragment is RecentFragment)
                view.SelectedItem = MainMenuViewModel.MainMenuItem.RecentActivity;
        }

        internal class MainMenuDrawerToggle : ActionBarDrawerToggle
        {
            readonly MainActivity owner;

            public MainMenuDrawerToggle(MainActivity activity, DrawerLayout layout, int openRes, int closeRes)
                : base(activity, layout, openRes, closeRes)
            {
                owner = activity;
            }

            public override void OnDrawerClosed(View drawerView)
            {
                owner.InvalidateOptionsMenu();
            }

            public override void OnDrawerOpened(View drawerView)
            {
                if (owner.CurrentFocus != null)
                    UiUtil.hideKeyboard(owner.CurrentFocus);
                owner.InvalidateOptionsMenu();
                owner.ShowLeadOverwriteDialogIfNeeded().Ignore();
            }
        }



        #region Lead overwrite dialog

        async Task ShowLeadOverwriteDialogIfNeeded()
        {
            var lead = await ViewModel.FindFirstLeadForOverwrite(null);
            if (lead == null) return;

            new Android.Support.V7.App.AlertDialog.Builder(this)
                       .SetTitle(lead.DialogTitle)
                       .SetMessage(lead.DialogMessage)
                       .SetPositiveButton(lead.OverwriteActionName, (sender, e) =>
            {
                lead.OverwiteLeadChanges().Ignore();
            })
                       .SetNegativeButton(lead.DiscardActionName, (sender, e) =>
            {
                lead.DiscardLeadChanges().Ignore();
            })
                       .SetNeutralButton(lead.CancelActionName, (sender, e) => { })
                       .SetIcon(Android.Resource.Drawable.IcDialogAlert)
                       .Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            SupportFragmentManager.FindFragmentById(Resource.Id.fragment_container)?.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #endregion
    }
}
