using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using LiveOakApp.Models.Data.Entities;
using Android.Support.V4.Content;
using Android.Graphics;
using System.Threading.Tasks;
using Android.Support.V7.App;
using LiveOakApp.Droid.Utils;
using Android.Content.PM;

[assembly: Android.App.UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.AccessCoarseLocation)]
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.Internet)]

namespace LiveOakApp.Droid.Controller
{
    public class LeadFragment : CustomFragment
	{

        public readonly static string ACTION_CODE = "action";
        public readonly static int SCAN_BUSINESS_CARD_ACTION = 1;

		NewLeadView view;
        LeadDetailsViewModel model;

        //dirty hack.
        public Android.Net.Uri LeadPhotoUri;
        public string LeadPhotoFilePath;
        public Android.Net.Uri CardBackPhotoUri;
        public string CardBackPhotoFilePath;
        public Android.Net.Uri CardFrontPhotoUri;
        public string CardFrontPhotoFilePath;
        //end of hack

        public LeadDetailsViewModel ViewModel 
        { 
            get
            {
                return model;
            } 
        }

        public static LeadFragment CreateWithLead(Lead lead)
        {
            var fragment = new LeadFragment();
            fragment.model = new LeadDetailsViewModel(lead);
            return fragment;
        }

        public static LeadFragment CreateFromAttendee(AttendeeViewModel attendee, EventViewModel @event)
        {
            var fragment = new LeadFragment();
            fragment.model = new LeadDetailsViewModel(attendee, @event);
            return fragment;
        }

        public static LeadFragment CreateFromCard(Card card, EventViewModel eventViewModel)
        {
            var fragment = new LeadFragment();
            fragment.model = new LeadDetailsViewModel(card, eventViewModel);
            return fragment;
        }

        int actionDeleteItemId;

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            var action_delete = menu.Add(L10n.Localize("Delete", "Delete"));
            action_delete.SetShowAsAction(ShowAsAction.Never);
            actionDeleteItemId = action_delete.ItemId;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId.Equals(actionDeleteItemId))
            {
                PerformDelete().Ignore();
                return true; // consume event
            }
            return base.OnOptionsItemSelected(item);
        }
        async Task PerformDelete()
        {
            await model.DeleteLead.ExecuteAsync();
            FragmentManager.PopBackStack();
        }

        public bool OnBackPressed()
        {
            if (model.HasValidFields)
                return false;
            try 
            {
                changesDiscardedDialog.Show();
            } catch (Exception e)
            {
                changesDiscardedDialog = CreateChangesDiscardedDialog();
                changesDiscardedDialog.Show();
            }
            return true;
        }

        EventHandler showDialogHandler;

        public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
            if(model == null)
                model = new LeadDetailsViewModel();
            Title = L10n.Localize("LeadNavigationBarTitle", "Lead");
        }


		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = new NewLeadView(inflater.Context);
            changesDiscardedDialog = CreateChangesDiscardedDialog();

			var tabHost = view.TabHost;
			var tabWidget = view.TabWidget;
			var tabContent = view.TabContent;

			tabHost.Setup(Activity, ChildFragmentManager, tabContent.Id);

            var contactTabSpec = tabHost.NewTabSpec("Contact");
            var contactTabView = new TabView(inflater.Context);

            contactTabView.Title.Text = L10n.Localize("TabContact", "Contact");

            contactTabSpec.SetIndicator(contactTabView);

            var qualifyTabSpec = tabHost.NewTabSpec("Qualify");

            var qualifyTabView = new TabView(inflater.Context);

            qualifyTabView.Title.Text = L10n.Localize("TabQualify", "Qualify");

            qualifyTabSpec.SetIndicator(qualifyTabView);

            tabHost.AddTab(contactTabSpec, Java.Lang.Class.FromType(typeof(LeadContactFormFragment)) , Arguments);
            tabHost.AddTab(qualifyTabSpec, Java.Lang.Class.FromType(typeof(LeadQualifyFormFragment)), null);

            { // init with right color
                var blue_color = new Color(ContextCompat.GetColor(Context, Resource.Color.primary_blue));
                var currentTabView = (TabView)tabHost.TabWidget.GetChildAt(tabHost.CurrentTab);
                currentTabView.Title.SetTextColor(blue_color);
            }

            tabWidget.GetChildAt(1).Touch += (sender, e) => 
            {
                e.Handled = !model.EnsureCanSelectQualifyTab();
            };

            tabHost.TabChanged += (object sender, TabHost.TabChangeEventArgs e) =>
            {
                var tabId = e.TabId;

                var white_color = new Color(ContextCompat.GetColor(Context, Android.Resource.Color.White));
                var blue_color = new Color(ContextCompat.GetColor(Context, Resource.Color.primary_blue));

                for (int i = 0; i < tabHost.TabWidget.ChildCount; i++)
                {
                    var tabView = (TabView) tabHost.TabWidget.GetChildAt(i);
                    tabView.Title.SetTextColor(white_color);
                }

                var currentTabView = (TabView) tabHost.TabWidget.GetChildAt(tabHost.CurrentTab);
                currentTabView.Title.SetTextColor(blue_color);
                UiUtil.hideKeyboard(tabHost);

            };

			return view;
		}

        public override void OnStart()
        {
            base.OnStart();
            if (PermissionsUtils.HasLocationPermission(Context))
                StartDetectingLocationIfNeeded();
            else
                PermissionsUtils.RequestLocationPermission(Activity);
        }

        public override void OnStop()
        {
            base.OnStop();
            ViewModel.StopDetectingLocation();
        }

        void StartDetectingLocationIfNeeded() 
        {
            ViewModel.StartDetectingLocationIfNeeded(() => new LocationManager(Context, 50),
                                                     () => new AddressGeocoder(Context, 5));
        }

		public override void OnActivityResult(int requestCode, int resultCode, Android.Content.Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			var currentFragment = ChildFragmentManager.FindFragmentById(view.TabContent.Id);
			var contactFormFragment = currentFragment as LeadContactFormFragment;
			if(contactFormFragment != null)
				contactFormFragment.HandleActivityResult(requestCode, resultCode, data);
		}

        #region Dialogs

        AlertDialog changesDiscardedDialog;
        AlertDialog CreateChangesDiscardedDialog()
        {
            return new AlertDialog.Builder(Context)
                                  .SetTitle(L10n.Localize("LeadChangesWillNotBeSavedTitle", "Confirm exit"))
                                  .SetMessage(L10n.Localize("LeadChangesWillNotBeSaved", "Missing required fields. Changes will not be saved"))
                                  .SetPositiveButton(L10n.Localize("Ok", "OK"), (sender, e) =>
            {
                changesDiscardedDialog.Dismiss();
                FragmentManager.PopBackStack();
            })
                                  .SetNegativeButton(L10n.Localize("Cancel", "Cancel"), (sender, e) =>
            {
                changesDiscardedDialog.Dismiss();
            })
                                  .Create();
        }

        void ShowLocationPermissionDeniedDialog()
        {
            new AlertDialog.Builder(Context)
                                       .SetTitle(L10n.Localize("LocationPermission", "Location permission"))
                                       .SetMessage(L10n.Localize("LocationPermissionMessage", "Location is needed to access first entry location"))
                                       .SetPositiveButton(L10n.Localize("Ok", "ok"), (sender, e) => { })
                                       .Create()
                                       .Show();
        }

        #endregion

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if(requestCode == PermissionsUtils.LOCATION_PERMISSION_REQUEST_CODE) 
            {
                if (grantResults[0] == Permission.Granted && grantResults[1] == Permission.Granted)
                    StartDetectingLocationIfNeeded();
                else
                    ShowLocationPermissionDeniedDialog();
            }
            ChildFragmentManager.FindFragmentById(view.TabContent.Id)?.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public class Builder
        {
            private readonly Bundle args;
            private LeadDetailsViewModel viewModel; //todo use lead id instead of view model;

            public Builder()
            {
                args = new Bundle();
            }

            public Builder Lead(Lead lead)
            {
                viewModel = new LeadDetailsViewModel(lead);
                return this;
            }

            public Builder Action(int action)
            {
                args.PutInt(ACTION_CODE, action);
                return this;
            }

            public Builder Card(Card card, EventViewModel eventViewModel)
            {
                viewModel = new LeadDetailsViewModel(card, eventViewModel);
                return this;
            }

            public Builder Attendee(AttendeeViewModel attendee, EventViewModel @event)
            {
                viewModel = new LeadDetailsViewModel(attendee, @event);
                return this;
            }

            public LeadFragment Build()
            {
                var fragment = new LeadFragment();
                fragment.model = viewModel;
                fragment.Arguments = args;
                return fragment;
            }
        }
    }
}
