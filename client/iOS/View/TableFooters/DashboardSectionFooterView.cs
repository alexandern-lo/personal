using System;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.TableFooters
{
    public class DashboardSectionFooterView : CustomView
    {

        protected override void CreateView()
        {
            base.CreateView();
            BackgroundColor = Colors.DefaultTableViewBackgroundColor;
        }

        public static float FooterHeight { get { return 15f; } }
    }
}
