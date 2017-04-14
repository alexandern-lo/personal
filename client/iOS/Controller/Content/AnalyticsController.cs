using LiveOakApp.iOS.View.Content;
using LiveOakApp.Resources;
using StudioMobile;

namespace LiveOakApp.iOS.Controller.Content
{
	public class AnalyticsController : MenuContentController<AnalyticsView>
	{
		public AnalyticsController(SlideController slideController) : base(slideController)
		{
			Title = L10n.Localize("MenuAnalytics", "Analytics");
		}
	}
}

