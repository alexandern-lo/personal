using System;
using Foundation;
using UIKit;

namespace StudioMobile
{
    public class CustomBindingsTableViewCell : CustomTableViewCell
    {
        public CustomBindingsTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            Initialize();
        }

        public CustomBindingsTableViewCell()
        {
            Initialize();
        }

        public CustomBindingsTableViewCell(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        public CustomBindingsTableViewCell(NSObjectFlag t) : base(t)
        {
            Initialize();
        }

        public CustomBindingsTableViewCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public CustomBindingsTableViewCell(CoreGraphics.CGRect frame) : base(frame)
        {
            Initialize();
        }

        public CustomBindingsTableViewCell(UITableViewCellStyle style, NSString reuseIdentifier) : base(style, reuseIdentifier)
        {
            Initialize();
        }

        void Initialize()
        {
            Bindings = new BindingList();
        }

        public override void WillMoveToSuperview(UIView newSuperview)
        {
            base.WillMoveToSuperview(newSuperview);
            if (newSuperview != null)
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
