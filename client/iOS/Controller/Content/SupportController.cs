using Foundation;
using LiveOakApp.iOS.View.Content;
using LiveOakApp.Resources;
using StudioMobile;

namespace LiveOakApp.iOS.Controller.Content
{
    public class SupportController : MenuContentController<SupportView>
    {
        public SupportController(SlideController slideController) : base(slideController)
        {
            Title = L10n.Localize("MenuSupport", "Dashboard");
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }
    }
}

