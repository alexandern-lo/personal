using System;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using LiveOakApp.Droid.CustomViews.Adapters;
using SE.Emilsjolander.Stickylistheaders;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models;
using LiveOakApp.Models.Services;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using Android.Support.V7.App;

namespace LiveOakApp.Droid.Controller
{
    public class AgendaFragment : CustomFragment
	{

        AgendaViewModel ViewModel;
        AgendaView  AgendaView;

        private const string EXTRA_EVENT = "EXTRA_EVENT";

        public static CustomFragment Create(EventViewModel eventViewModel)
        {
            var args = new Bundle();
            args.PutString(EXTRA_EVENT, eventViewModel.EventToJson());
            var fragment = new AgendaFragment();
            fragment.Arguments = args;
            return fragment;
        }

        public AgendaFragment()
        {
            Title = L10n.Localize("AgendaNavigationBarTitle", "Event agenda");
            AgendaSelectCommand = new Command
            {
                Action = AgendaSelectAction
            };
        }

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
            var _event = EventViewModel.JsonToEvent(Arguments.GetString(EXTRA_EVENT));
            ViewModel = new AgendaViewModel(_event);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
            AgendaView = new AgendaView(container.Context);
            return AgendaView;
		}

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var groupedAdapter = AgendaView.GetSectionsAdapter(ViewModel.CurrentAgendaSections);
            Bindings.Adapter(AgendaView.AgendaList, groupedAdapter);
            Bindings.Command(AgendaSelectCommand)
                    .ParameterConverter(position => groupedAdapter[(int)position])
                    .To(AgendaView.AgendaList.ItemClickTarget());
            Bindings.Property(ViewModel.LoadAgendaCommand, _ => _.IsRunning)
                    .UpdateTarget((s) =>
            {
                AgendaView.IsShowingProgressBar = s.Value;
                AgendaView.AgendaRefresher.Enabled = !s.Value;
            });
            Bindings.Property(ViewModel.LoadAgendaCommand, _ => _.IsRunning)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());
            Bindings.Property(ViewModel.LoadAgendaCommand, _ => _.Error)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());
            Bindings.Property(ViewModel.CurrentAgendaSections, _ => _.Count)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());
            AgendaView.HeaderClickAction = HeaderClickAction;
            AgendaView.DateClickAction = DateSelectAction;
            AgendaView.AgendaRefresher.Refresh += async (sender, e) =>
            {
                AgendaView.AgendaRefresher.Refreshing = false;
                try
                {
                    await ViewModel.LoadAgendaCommand.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    LOG.Error(ex.MessageForHuman());
                }
            };
            LoadAgendaAndSetup();
        }

        async void LoadAgendaAndSetup()
        {
            await ViewModel.ExecuteLoadIfNeeded();
            AgendaView.SetupDateButtons(ViewModel.AgendaDates, ViewModel.CurrentAgendaDate);
        }

        Command AgendaSelectCommand { get; set; }
        void AgendaSelectAction(object obj)
        {
            var agentaItem = (AgendaItemViewModel)obj;
            Android.Net.Uri urlToOpen = agentaItem.DetailsUrl.TryParseWebsiteUri()?.ToAndroidUri();
            if (urlToOpen == null) return;
            var intent = new Intent(Intent.ActionView, urlToOpen);
            Context.StartActivity(intent);
        }

        void DateSelectAction(int position)
        {
            ViewModel.CurrentAgendaDate = ViewModel.AgendaDates[position].Date;
        }

        void HeaderClickAction(int position)
        {
            Android.Net.Uri urlToOpen = ViewModel.CurrentAgendaSections[position].LocationUrl.TryParseWebsiteUri()?.ToAndroidUri();
            if (urlToOpen == null) return;
            var intent = new Intent(Intent.ActionView, urlToOpen);
            Context.StartActivity(intent);
        }

        public void UpdateErrorAndWarningViews()
        {
            bool isError = ViewModel.LoadAgendaCommand.Error != null;
            bool isEmptyList = ViewModel.CurrentAgendaSections.Count == 0;
            bool fetchRunning = AgendaView.IsShowingProgressBar;

            if (fetchRunning)
                return;

            if (isError)
            {
                new AlertDialog.Builder(Context)
                    .SetMessage(ViewModel.LoadAgendaCommand.Error.MessageForHuman())
                    .Show();
            }
            else if (isEmptyList)
            {
                AgendaView.MessageView.Visibility = ViewStates.Visible;
                AgendaView.MessageView.Text = L10n.Localize("NoAgendaLabel", "No agenda");
            }
        }
    }
}

