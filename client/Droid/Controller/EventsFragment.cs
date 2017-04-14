
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.ViewModels;
using StudioMobile;
using LiveOakApp.Resources;
using LiveOakApp.Droid.CustomViews.Adapters;
using System;
using LiveOakApp.Models;
using System.Runtime.InteropServices.ComTypes;

namespace LiveOakApp.Droid.Controller
{
    public class EventsFragment : CustomFragment
	{
		private EventsView view;
		private EventsViewModel model;

        public Command EventSelectedCallbackCommand { get; set; }

        public Command EventSelectedCommand { get; private set; }
		public Command OpenLeadScreenCommand { get; private set; }
		public Command OpenEventDetailsCommand { get; private set; }

        private bool isCreatedForResult = false;

        public static EventsFragment CreateForResult(Command onEventSelectedCallback)
        {
            var fragment = Create();

            fragment.EventSelectedCallbackCommand = onEventSelectedCallback;
            fragment.isCreatedForResult = true;
            fragment.Title = L10n.Localize("SelectEventTitle", "Select event");

            return fragment;
        }

        public static EventsFragment Create()
        {
            var fragment = new EventsFragment();

            fragment.Title = L10n.Localize("EventsNavigationBarTitle", "Events");

            return fragment;
        }

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
            model = new EventsViewModel(isCreatedForResult);

			OpenLeadScreenCommand = new Command
			{
				Action = OpenLeadScreenAction
			};

			OpenEventDetailsCommand = new Command
			{
				Action = OpenEventDetailsAction
			};

            EventSelectedCommand = new Command
            {
                Action = OnEventSelected
            };
        }

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{

			view = new EventsView(Activity);

            var groupedAdapter = view.GetSectionsAdapter(model.Sections);
            Bindings.Adapter(view.EventList, groupedAdapter);

            if (isCreatedForResult)
            {
                view.IsInShowMode = false;
            } 
            else
            {
                view.IsInShowMode = true;
                Bindings.Command(OpenLeadScreenCommand)
                    .To(view.LeadButton.ClickTarget());
            }

			Bindings.Command(EventSelectedCommand)
                    .ParameterConverter(position => groupedAdapter[(int)position])
                    .To(view.EventList.ItemClickTarget());

            Bindings.Property(model.LoadEventsCommand, _ => _.IsRunning)
			        .UpdateTarget((running) => 
			{
                view.EventListRefresher.Enabled = !running.Value;
                view.IsShowingProgressBar = running.Value;
			});

            view.EventListRefresher.Refresh += async (sender, e) =>
            {
                view.EventListRefresher.Refreshing = false;
                try
                {
                    await model.LoadEventsCommand.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    LOG.Error(ex.MessageForHuman());
                }
            };

            model.LoadEventsCommand.Execute();

            return view;
		}

        void OnEventSelected(object clickedEvent)
        {
            if(EventSelectedCallbackCommand != null)
            {
                EventSelectedCallbackCommand.Execute(clickedEvent);
                FragmentManager.PopBackStack();
            }
            else 
            {
                OpenEventDetailsAction(clickedEvent); 
            }
        }

		void OpenLeadScreenAction(object args)
		{
			FragmentManager.BeginTransaction()
						   .Replace(Resource.Id.fragment_container, new LeadFragment())
			               .AddToBackStack("lead-screen")
						   .Commit();
		}

        void OpenEventDetailsAction(object clickedEvent)
        {
			var @event = (EventViewModel)clickedEvent;
			FragmentManager.BeginTransaction()
			               .Replace(Resource.Id.fragment_container, EventDetailsFragment.CreateForEvent(@event))
						   .AddToBackStack("event-details")
			               .Commit();
        }
	}
}
