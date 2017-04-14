using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using LiveOakApp.Droid.CustomViews;
using LiveOakApp.Models.Data.Entities;

namespace LiveOakApp.Droid.Controller
{
    public class BusinessCardViewerDialog : DialogFragment
    {
        private readonly static string TITLE_KEY = "title";
        private readonly static string LOCAL_PATH_KEY = "local";
        private readonly static string REMOTE_PATH_KEY = "remote";
        private readonly static double HEIGHT = 0.75;
        private readonly static double WIDTH = 0.75;

        public static BusinessCardViewerDialog Create(string title, FileResource res)
        {
            var fragment = new BusinessCardViewerDialog();
            var args = new Bundle();
            args.PutString(TITLE_KEY, title);
            args.PutString(LOCAL_PATH_KEY, res.RelativeLocalPath);
            args.PutString(REMOTE_PATH_KEY, res.RemoteUrl);
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnStart()
        {
            base.OnStart();
            DisplayMetrics metrics = new DisplayMetrics();
            Activity.WindowManager.DefaultDisplay.GetMetrics(metrics);
            int height = (int)(metrics.HeightPixels * HEIGHT);
            int width = (int)(metrics.WidthPixels * WIDTH);
            Dialog.Window.SetLayout(width, height);
        }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var title = Arguments.GetString(TITLE_KEY);
            var localPath = Arguments.GetString(LOCAL_PATH_KEY);
            var remotePath = Arguments.GetString(REMOTE_PATH_KEY);
            var resource = new FileResource(localPath, remotePath);

            CustomImageView cardView = (CustomImageView)View.Inflate(Context, Resource.Layout.BusinessCardView, null);
            cardView.LoadByResource(resource);

            var dialog = new AlertDialog.Builder(Context)
                .SetTitle(title)
                .SetView(cardView)
                .Create();

            return dialog;
        }
    }
}
