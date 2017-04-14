
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;

namespace LiveOakApp.Droid.Controller
{
    public class AttendeesDetailsFragment : CustomFragment
	{

		private const string  EXTRA_ATTENDEE = "EXTRA_ATTENDEE";
        private const string EXTRA_EVENT = "EXTRA_EVENT";

        public static Fragment CreateForAttendee(AttendeeViewModel attendee, EventViewModel @event)
		{
			var fragment = new AttendeesDetailsFragment();

			var args = new Bundle();
            args.PutString(EXTRA_ATTENDEE, attendee.AttendeeToJson());
            args.PutString(EXTRA_EVENT, @event.EventToJson());

			fragment.Arguments = args;

			return fragment; 
		}

		public Command OpenLeadScreenCommand { get; private set; }

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			var attendee = AttendeeViewModel.JsonToAttendee(Arguments.GetString(EXTRA_ATTENDEE));
            var @event = EventViewModel.JsonToEvent(Arguments.GetString(EXTRA_EVENT));

			OpenLeadScreenCommand = new Command
			{
				Action = OpenLeadScreenAction
			};

			model = new AttendeeDetailsViewModel(attendee, @event);

            Title = L10n.Localize("AttendeeDetailsNavigationBarTitle", "Details");
		}

		AttendeeDetailsView view;
		AttendeeDetailsViewModel model;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = new AttendeeDetailsView(inflater.Context);

			Bindings.Property(model, _ => _.AttendeeName)
					.To(view.AttendeeName.TextProperty());
			Bindings.Property(model, _ => _.AttendeeTitle)
			        .To(view.AttendeeTitle.TextProperty());
			Bindings.Property(model, _ => _.AttendeeCompany)
			        .To(view.AttendeeCompany.TextProperty());

            Bindings.Adapter(view.InfoList, view.GetInfoListAdapter(model.AttendeeInfoList));

			Bindings.Property(model, _ => _.AttendeeAvatar)
			        .To(view.AttendeeAvatar.ImageProperty());

			Bindings.Command(OpenLeadScreenCommand)
			        .To(view.LeadButton.ClickTarget());

			return view;
		}

		private void OpenLeadScreenAction(object args)
		{
			FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container, LeadFragment.CreateFromAttendee(model.AttendeeViewModel, model.Event))
						   .AddToBackStack("lead-screen")
						   .Commit();
		}

	}
}

