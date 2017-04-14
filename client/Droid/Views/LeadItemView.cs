using Android.Content;
using Android.Util;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Droid.CustomViews;
using System.Threading.Tasks;

namespace LiveOakApp.Droid.Views
{
    public class LeadItemView : LinearLayout
    {
        LeadViewModel person;

        public CustomImageView PictureImage { get; private set; }
        public TextView NameText { get; private set; }
        public TextView TitleText { get; private set; }

        public LeadItemView(Context context) : base(context)
        {
            Initialize();
        }

        void Initialize()
        {
            SetBackgroundResource(Resource.Drawable.selector_white_to_light);

            var padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 4, Context.Resources.DisplayMetrics);
            SetPadding(padding, padding, padding, padding);

            SetGravity(Android.Views.GravityFlags.CenterVertical);

            Inflate(Context, Resource.Layout.PersonItem, this);
            Orientation = Orientation.Horizontal;

            PictureImage = FindViewById<CustomImageView>(Resource.Id.person_item_picture);
            NameText = FindViewById<TextView>(Resource.Id.person_item_name);
            TitleText = FindViewById<TextView>(Resource.Id.person_item_title);
        }

        public LeadViewModel Person
        {
            get { return person; }
            set
            {
                person = value;
                NameText.Text = person.FullName;
                TitleText.Text = person.JobInfo;
                SetPhotoResource(person.PhotoResource);
            }
        }

        void SetPhotoResource(FileResource photo)
        {
            if (PictureImage.ScheduledWork != null)
            {
                Task.Run(() => PictureImage.ScheduledWork.Cancel());
                PictureImage.ScheduledWork = null;
            }
            var localPath = photo?.AbsoluteLocalPath;
            var remoteUrl = photo?.RemoteUrl;
            if (!string.IsNullOrEmpty(remoteUrl))
                PictureImage.LoadByUrl(remoteUrl);
            else if (!string.IsNullOrEmpty(localPath))
                PictureImage.LoadByPath(localPath);
            else
                PictureImage.SetImageResource(Resource.Drawable.lead_placeholder);
        }

        public void Recycle()
        {
            if (PictureImage.ScheduledWork != null)
            {
                Task.Run(() => PictureImage.ScheduledWork.Cancel());
                PictureImage.ScheduledWork = null;
            }
            PictureImage.SetImageDrawable(null);
        }
    }
}
