using Android.OS;
using Android.Support.V7.App;
using SL4N;

namespace StudioMobile
{
    public class CustomActivity : AppCompatActivity
    {
        ILogger _log;
        protected ILogger LOG { get { return _log ?? (_log = LoggerFactory.GetLogger(GetType().Name)); } }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            LOG.Info("OnCreate");
            base.OnCreate(savedInstanceState);
            Bindings = new BindingList();
        }

        protected override void OnResume()
        {
            LOG.Info("OnResume");
            base.OnResume();
            Bindings.Bind();
            Bindings.UpdateTarget();
        }

        protected override void OnPause()
        {
            LOG.Info("OnPause");
            base.OnPause();
            Bindings.Unbind();
        }

        protected override void OnDestroy()
        {
            LOG.Info("OnDestroy");
            base.OnDestroy();
        }

        protected override void Dispose(bool disposing)
        {
            LOG.Info("Dispose");
            base.Dispose(disposing);
        }

        public BindingList Bindings { get; private set; }
    }
}
