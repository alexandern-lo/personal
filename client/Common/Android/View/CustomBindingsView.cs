using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using SL4N;

namespace StudioMobile
{
    /// <summary>
    /// View with BindingList to use with nested ViewModels.
    /// Only use it for complicated forms with lists to allow two-way bindings.
    /// Table with cells with multiple buttons should be made with EventListSource.
    /// </summary>
    public class CustomBindingsView : FrameLayout
    {
        protected static readonly ILogger LOG = LoggerFactory.GetLogger<CustomBindingsView>();
        
        public CustomBindingsView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize();
        }

        public CustomBindingsView(Context context) : base(context)
        {
            Initialize();
        }

        public CustomBindingsView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public CustomBindingsView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Initialize();
        }

        void Initialize()
        {
            LOG.Info("{0} view created", this.GetType());
            Bindings = new BindingList();
        }

        public void ResetBingings(BindingList bindings)
        {
            System.Diagnostics.Debug.Assert(Bindings.Count == 0, "Bindings must be empty");
            Bindings = bindings;
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            LOG.Info("{0} window attached", this.GetType());
            Bindings.Bind();
            Bindings.UpdateTarget();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            LOG.Info("{0} window detached", this.GetType());
            Bindings.Unbind();
        }

        protected override void Dispose(bool disposing)
        {
			LOG.Info("{0} view disposed", this.GetType());
            base.Dispose(disposing);
        }

        public BindingList Bindings { get; private set; }
    }
}
