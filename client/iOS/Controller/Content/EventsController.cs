using StudioMobile;
using LiveOakApp.iOS.View.Content;
using LiveOakApp.Models.Data;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using System;
using UIKit;

namespace LiveOakApp.iOS.Controller.Content
{
	public class EventsController : MenuContentController<EventsView>
	{
        public Command SelectEventDetailsCommand { get; private set; }
        public Action<EventViewModel> OnEventSelectAction { get; private set; }
        public Command CreateLeadCommand { get; private set; }
        private bool isEventSelectionMode;

        public EventsController(Action<EventViewModel> onEventChosen) // constructor for Event choosing mode
        {
            Initialize();
            Title = L10n.Localize("SelectEventTitle", "Select Event");
            isEventSelectionMode = true;
            OnEventSelectAction = onEventChosen;
            ViewModel = new EventsViewModel(isEventSelectionMode);
        }

        public EventsController(SlideController slideController) : base(slideController)
        {
            Initialize();
            Title = L10n.Localize("MenuEvents", "Events");
            isEventSelectionMode = false;
            OnEventSelectAction = (EventViewModel obj) =>
            {
                var eventDetailsController = new EventDetailsController(obj);
                NavigationController.PushViewController(eventDetailsController, true);
            };
            ViewModel = new EventsViewModel();
        }

        private void Initialize()
        {
            SelectEventDetailsCommand = new Command()
            {
                Action = SelectEvent
            };
            CreateLeadCommand = new Command()
            {
                Action = CreateLeadAction
            };
        }

        public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            View.IsEventSelectionMode = isEventSelectionMode;
            if (isEventSelectionMode)
            {
                NavigationItem.LeftBarButtonItem = null;
            }
            var sectionsBinding = View.GetSectionsBinding(ViewModel.Sections);
            Bindings.Add(sectionsBinding);
            Bindings.Command(SelectEventDetailsCommand).To(sectionsBinding.ItemSelectedTarget());

            Bindings.Property(ViewModel.LoadEventsCommand, _ => _.IsRunning)
                    .UpdateTarget((s) =>
                    {
                        View.EventsFetchRunning = s.Value;
                        View.RefreshControl.Subviews[0].Subviews[0].Hidden = s.Value;
                    });
            Bindings.Command(CreateLeadCommand).To(View.AddButton.ClickTarget());
            ViewModel.LoadEventsCommand.Execute();

            View.RefreshControl.AddTarget((sender, e) =>
            {
                View.RefreshControl.EndRefreshing();
                ViewModel.LoadEventsCommand.Execute();
            }, UIControlEvent.ValueChanged);
        }

		void SelectEvent(object parameter)
		{
            var eventData = (EventViewModel)parameter;
            OnEventSelectAction(eventData);
		}

        void CreateLeadAction(object parameter)
        {
            LeadDetailsController leadDetailsController = new LeadDetailsController(() => NavigationController.PopToViewController(this, true));
            NavigationController.PushViewController(leadDetailsController, true);
        }

		private EventsViewModel ViewModel;
	}
}

