using System;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Content
{
    public class AgendaDateButton : CustomTopBarButton
    {
        Command DateClickCommand { get; set; }
        Action<object> DateClickAction { get; set; }

        public AgendaDateButton(Action<object> dateClickAction)
        {
            DateClickAction = dateClickAction;
            Initialize();
            Bindings.Command(DateClickCommand).To(this.ClickTarget());
        }

        void Initialize()
        {
            Bindings = new BindingList();
            DateClickCommand = new Command()
            {
                Action = DateClickAction
            };
        }

        public override void SetActive(bool active)
        {
            SelectionSeparatorView.Hidden = !active;
            if (active)
            {
                SetTitleColor(UIColor.White, UIControlState.Normal);
            }
            else {
                SetTitleColor(UIColor.Black, UIControlState.Normal);
            }
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            if (newsuper != null)
            {
                Bindings.Bind();
                Bindings.UpdateTarget();
            }
            else {
                Bindings.Unbind();
            }
        }

        public BindingList Bindings { get; private set; }

    }
}
