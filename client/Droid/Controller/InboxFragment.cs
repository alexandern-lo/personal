using Android.OS;
using Android.Views;
using Android.Widget;
using StudioMobile;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Droid.Views;
using Java.Lang;
using LiveOakApp.Models;
using LiveOakApp.Resources;

namespace LiveOakApp.Droid.Controller
{
	public class InboxFragment : CustomFragment
	{

        InboxViewModel model;
        InboxView view;

        public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
            model = new InboxViewModel();
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
            view = new InboxView(inflater.Context);

            RegisterForContextMenu(view.InboxList);

            Bindings.Adapter(view.InboxList, view.GetInboxAdapter(model.InboxItems));
            Bindings.Property(model.LoadInboxItemsCommand, _ => _.IsRunning)
                    .UpdateTarget((running) => 
            {
                view.IsShowingProgressBar = running.Value;
            });
            view.InboxListRefresher.Refresh += async (sender, e) =>
            {
                view.InboxListRefresher.Refreshing = false;
                try
                {
                    await model.LoadInboxItemsCommand.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    LOG.Error(ex.MessageForHuman());
                }
            };
            Bindings.Property(model, _ => _.LastUpdateTime)
                    .Convert<string>((time) => 
                                     string.Format(L10n.Localize("LastUpdatedFormat", "Updated at {time}"), ServiceLocator.Instance.DateTimeService.TimeToDisplayString(time, Context)))
                    .To(view.LastUpdated.TextProperty());

            model.LoadInboxItemsCommand.Execute();
			return view;
		}

        const int MENU_DELETE_ID = 0;

        public override void OnCreateContextMenu(IContextMenu menu, View sourceView, IContextMenuContextMenuInfo menuInfo)
        {
            if (sourceView.Id != view.InboxList.Id)
            {
                base.OnCreateContextMenu(menu, sourceView, menuInfo);
                return;
            }
            menu.Add(Menu.None, MENU_DELETE_ID, Menu.None, L10n.Localize("Delete", "Delete"));
        }
        public override bool OnContextItemSelected(IMenuItem item)
        {
            if(item.ItemId != MENU_DELETE_ID)
                return base.OnContextItemSelected(item);
            var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            model.InboxItems.RemoveAt(info.Position);
            return true;
        }

	}
}

