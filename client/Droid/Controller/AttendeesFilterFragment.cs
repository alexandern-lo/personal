using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using LiveOakApp.Droid.CustomViews.Adapters;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;

namespace LiveOakApp.Droid.Controller
{

    public class AttendeesFilterFragment : CustomFragment
    {
        private const string EXTRA_EVENT = "EXTRA_EVENT";

        public static Fragment CreateForEvent(EventViewModel @event)
        {
            var args = new Bundle();

            args.PutString(EXTRA_EVENT, @event.EventToJson());

            var fragment = new AttendeesFilterFragment();
            fragment.Arguments = args;

            return fragment;
        }

        AttendeeFiltersViewModel model;
        AttendeesFiltersView view;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var _event = EventViewModel.JsonToEvent(Arguments.GetString(EXTRA_EVENT));

            model = new AttendeeFiltersViewModel(_event);

            Title = L10n.Localize("CategoryFilterNavigationBarTitle", "Category filter"); 
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = new AttendeesFiltersView(inflater.Context);

            var actionBar = ((AppCompatActivity)Activity).SupportActionBar;
            if (actionBar != null)
            {
                actionBar.SetDisplayHomeAsUpEnabled(true);
                actionBar.SetHomeButtonEnabled(true);
                actionBar.SetHomeAsUpIndicator(Android.Resource.Drawable.IcMenuCloseClearCancel);
            }
            HasOptionsMenu = true;

            var groupedAdapter = view.GetSectionsAdapter(model.Sections);
            Bindings.Adapter(view.FiltersList, groupedAdapter);

            Bindings.Command(model.ToggleOptionCommand)
                    .ParameterConverter((pos) => groupedAdapter[(int)pos])
                    .To(view.SwitchListSource);
            
            Bindings.Command(model.ResetTogglesCommand)
                    .To(view.ResetButton.ClickTarget());
            
            return view;         
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            menu.Clear();

            Activity.MenuInflater.Inflate(Resource.Layout.FullscreenDialogMenuLayout, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;

            if (id == Resource.Id.action_save)
            {
                model.SaveChangesCommand.Execute();
                FragmentManager.PopBackStack();
                return true;
            }
            else if (id == Android.Resource.Id.Home)
            {
                FragmentManager.PopBackStack();
                return true;
            }

            return base.OnOptionsItemSelected(item);    
        }

        public override void OnDetach()
        {
            base.OnDetach();

            var actionBar = ((AppCompatActivity)Activity).SupportActionBar;
            if (actionBar != null)
            {
                actionBar.SetHomeAsUpIndicator(Resource.Drawable.navbar_arrow);
            }
        }

    }
}


