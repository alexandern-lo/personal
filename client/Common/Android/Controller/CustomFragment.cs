using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using SL4N;

namespace StudioMobile
{
	public class CustomFragment : Fragment
	{
        ILogger _log;
        protected ILogger LOG { get { return _log ?? (_log = LoggerFactory.GetLogger(GetType().Name)); } }

        public string Title { get; protected set; }

        public override void OnCreate(Bundle savedInstanceState)
		{
			LOG.Info("OnCreate");
			base.OnCreate(savedInstanceState);
			Bindings = new BindingList();
		}

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			LOG.Info("OnViewCreated");
			base.OnViewCreated(view, savedInstanceState);

		}

		public override void OnStart()
		{
			base.OnStart();
            Bindings.Bind();
			Bindings.UpdateTarget();
		}

		public override void OnStop()
		{
			base.OnStop();
			Bindings.Unbind();
		}

		public override void OnDestroyView()
		{
			LOG.Info("OnDestroyView");
			base.OnDestroyView();
			Bindings.Unbind();
		}

		public override void OnDestroy()
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
