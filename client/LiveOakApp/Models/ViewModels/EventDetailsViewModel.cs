using System;
using System.Collections.Generic;
using System.Linq;
using StudioMobile;
using LiveOakApp.Models.Services;
using System.Threading.Tasks;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.Models.ViewModels
{
    public class EventDetailsViewModel : DataContext
    {
        DateTimeService DateTimeService;
        EventService EventService;

        public EventDetailsViewModel(EventViewModel @event)
        {
            Event = @event;
            DateTimeService = ServiceLocator.Instance.DateTimeService;
            EventService = ServiceLocator.Instance.EventService;
            TotalExpenses = new MoneyViewModel();
            LoadTotalExpensesCommand = new AsyncCommand()
            {
                Action = LoadTotalExpensesAction,
                CanExecute = CanExecuteLoadTotalExpenses
            };
        }

        bool CanExecuteLoadTotalExpenses(object arg)
        {
            return !LoadTotalExpensesCommand.IsRunning;
        }

        public EventViewModel Event { get; private set; }

        public EventViewModel.EventType TypeEnum
        {
            get
            {
                return Event.TypeEnum;
            }
        }

        public string Industry
        {
            get
            {
                return Event.Industry;
            }
        }

        public bool IsReccuring
        {
            get
            {
                return Event.IsReccuring;
            }
        }

        public string Title { get { return Event.Name; } }

        public string Date
        {
            get
            {
                var dateStrings = new List<DateTime?> { Event.StartDate, Event.EndDate }
                    .Where(d => d != null)
                    .Select(d => DateTimeService.DateToDisplayString(d)).ToList()
                    .Distinct();
                return string.Join(" - ", dateStrings);
            }
        }

        public string Location
        {
            get
            {
                if (Event.VenueName != null)
                    return Event.VenueName + "\n" + Address;
                return Address;
            }
        }

        private string Address
        {
            get
            {
                var locationParts = new List<string> { Event.Address, Event.ZipCode, Event.City, Event.State, Event.Country }
                    .Where(p => p != null);
                return string.Join(", ", locationParts);
            }
        }

        public string MapsQuery
        {
            get
            {
                if (Event.VenueName != null)
                    return Address + ", " + Event.VenueName;
                return Address;
            }
        }

        public string Type { get { return Event.Type; } }

        public string WebsiteUrlString { get { return Event.WebsiteUrl; } }

        public Uri WebsiteUri { get { return Event.WebsiteUrl.TryParseWebsiteUri(); } }

        public MoneyViewModel TotalExpenses { get; set; } = new MoneyViewModel(0, MoneyViewModel.MoneyCurrency.USD);
        public AsyncCommand LoadTotalExpensesCommand;
        async Task LoadTotalExpensesAction(object arg)
        {
            var expensesDTO = await EventService.GetTotalExpenses(Event.UID, LoadTotalExpensesCommand.Token);
            TotalExpenses.UpdateFromDTO(expensesDTO);
            RaisePropertyChanged(() => TotalExpenses);
        }

        RemoteImage eventLogo;
        public RemoteImage EventLogo
        {
            get
            {
                if (eventLogo == null)
                {
                    var imageUri = Event.LogoUrl.TryParseWebsiteUri();
                    if (imageUri != null)
                    {
                        eventLogo = new RemoteImage(imageUri);
                    }
                }
                return eventLogo;
            }
        }

        public void AddExpense(decimal newExpense, string description)
        {
            var expense = new EventExpenseDTO()
            {
                EventUid = new Guid(Event.UID),
                Expense = new MoneyViewModel(newExpense, TotalExpenses.Currency).MoneyDTO,
                Comments = description
            };
            EventService.AddExpense(expense).Ignore();
            TotalExpenses.Amount += newExpense;
            RaisePropertyChanged(() => TotalExpenses);
        }
    }
}
