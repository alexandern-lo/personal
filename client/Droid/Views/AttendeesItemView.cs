using Android.Content;
using Android.Util;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using LiveOakApp.Droid.CustomViews;
using LiveOakApp.Models.ViewModels;

namespace LiveOakApp.Droid.Views
{
	public class AttendeesItemView : LinearLayout
	{
		AttendeeViewModel person;

		public CustomImageView PictureImage { get; private set; }
		public TextView NameText { get; private set; }
		public TextView TitleText { get; private set; }

		public AttendeesItemView(Context context) : base(context)
		{
			Initialize();
		}

		void Initialize()
		{
			SetBackgroundResource(Resource.Drawable.selector_white_to_light);

			var padding = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 4, Context.Resources.DisplayMetrics);
			SetPadding(padding, padding, padding, padding);

			SetGravity(Android.Views.GravityFlags.CenterVertical);

			Inflate(Context, Resource.Layout.PersonItem, this);
			Orientation = Orientation.Horizontal;

            PictureImage = FindViewById<CustomImageView>(Resource.Id.person_item_picture);
			NameText = FindViewById<TextView>(Resource.Id.person_item_name);
			TitleText = FindViewById<TextView>(Resource.Id.person_item_title);
		}

		public AttendeeViewModel Person
		{
			get { return person; }
			set
			{
				person = value;
                NameText.Text = person.FullName;
                TitleText.Text = person.Company;
                var imageLoadingWork = PictureImage.ScheduledWork;
                var url = person.AvatarUrl;
                if (string.IsNullOrEmpty(url))
                {
                    PictureImage.SetImageResource(Resource.Drawable.placeholder);
                }
                else {
                    if (imageLoadingWork != null) imageLoadingWork.Cancel();
                    PictureImage.LoadByUrl(person.AvatarUrl);
                }
			}
		}

        public void Recycle()
        {
            var imageLoadingWork = PictureImage.ScheduledWork;
            if (imageLoadingWork != null) imageLoadingWork.Cancel();
            PictureImage.SetImageDrawable(null);
        }
	}
}
