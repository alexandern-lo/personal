using System;
using System.Threading.Tasks;
using StudioMobile;
using System.Collections.Generic;

namespace LiveOakApp.Models.ViewModels
{
    public class InboxViewModel : DataContext
    {
        public ObservableList<InboxItemViewModel> InboxItems { get; private set; } = new ObservableList<InboxItemViewModel>();

        Field<DateTime> _lastUpdateTime;

        public DateTime LastUpdateTime 
        {
            get
            {
                return _lastUpdateTime.Value;
            }
            set 
            {
                _lastUpdateTime.SetValue(value);
            }
        }

        public InboxViewModel()
        {
            LoadInboxItemsCommand = new AsyncCommand()
            {
                Action = LoadInboxItemsAction,
                CanExecute = CanExecuteLoadInboxItems
            };
            // TODO: change when back-end will be ready.
            _lastUpdateTime = Value(DateTime.Now);
        }

        bool CanExecuteLoadInboxItems(object arg)
        {
            return !LoadInboxItemsCommand.IsRunning;
        }

        public AsyncCommand LoadInboxItemsCommand { get; private set; }
        async Task LoadInboxItemsAction(object parameter)
        {
            await Task.Delay(1500);
            LastUpdateTime = DateTime.Now;
            var inboxItems = new List<InboxItemViewModel>();
            inboxItems.Add(new InboxItemViewModel()
            {
                Type = InboxItemViewModel.NotificationType.Announcement,
                Title = "Inbox item for new Announcement",
                Message = "Long message for current day inbox item #1!",
                ReceivedTime = DateTime.Today,
                IsRead = true
            });

            for (int i = 1; i < 8; i++)
            {
                inboxItems.Add(new InboxItemViewModel()
                {
                    Type = InboxItemViewModel.NotificationType.Poll,
                    Title = "Inbox item #" + i,
                    Message = "Message #" + i + " for inbox item",
                    ReceivedTime = new DateTime(2017, 12, i, 11, 24, 0),
                    IsRead = false
                });
            }
            InboxItems.Reset(inboxItems);
        }

    }
}
