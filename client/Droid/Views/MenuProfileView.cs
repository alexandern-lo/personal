using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

using StudioMobile;

namespace LiveOakApp.Droid.Views
{
	public class MenuProfileView : FrameLayout
	{
		public MenuProfileView(Context context) :
			base(context)
		{
			Initialize();
		}

		public MenuProfileView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public MenuProfileView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

		public CircleRemoteImageView AvatarView
		{
			get; private set;
		}

		public TextView NameView
		{
			get; private set;
		}

        public View LogoutButton
        {
            get; private set;
        }

		void Initialize()
		{
			Inflate(Context, Resource.Layout.MenuProfileLayout, this);

			AvatarView = FindViewById<CircleRemoteImageView>(Resource.Id.user_avatar);

			NameView = FindViewById<TextView>(Resource.Id.user_name);

            LogoutButton = FindViewById(Resource.Id.sing_out_icon);
		}
	}
}

