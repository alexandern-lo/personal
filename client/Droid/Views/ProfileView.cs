using Android.Content;
using Android.Util;
using Android.Widget;

using StudioMobile;

namespace LiveOakApp.Droid.Views
{
	public class ProfileView : FrameLayout
	{
		public ProfileView(Context context) :
			base(context)
		{
			Initialize();
		}

		public ProfileView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public ProfileView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

		public RemoteImageView ProfilePhoto
		{
			get; private set;
		}

		public TextView ProfileName
		{
			get; private set;
		}

		void Initialize()
		{
			Inflate(Context, Resource.Layout.ProfileLayout, this);

			ProfilePhoto = FindViewById<RemoteImageView>(Resource.Id.profile_photo);

			ProfileName = FindViewById<TextView>(Resource.Id.profile_name);

		}
	}
}

