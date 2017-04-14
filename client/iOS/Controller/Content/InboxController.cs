using LiveOakApp.iOS.View.Content;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.Controller.Content
{
    public class InboxController : MenuContentController<InboxView>
    {
        InboxViewModel ViewModel = new InboxViewModel();
        public InboxController(SlideController slideController) : base(slideController)
        {
            Title = L10n.Localize("MenuInbox", "Inbox");
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var tableBinding = View.GetInboxItemsBinding(ViewModel.InboxItems);
            Bindings.Add(tableBinding);
            Bindings.Property(ViewModel.LoadInboxItemsCommand, _ => _.IsRunning).UpdateTarget((s) => View.ResourcesFetchRunning = s.Value);
            View.RefreshControl.AddTarget((sender, e) =>
            {
                View.RefreshControl.EndRefreshing();
                ViewModel.LoadInboxItemsCommand.Execute();
            }, UIControlEvent.ValueChanged);

            ViewModel.LoadInboxItemsCommand.Execute();
        }
    }
}
