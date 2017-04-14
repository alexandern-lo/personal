using Android.Content;
using Android.Util;
using Android.Widget;
using StudioMobile;
using LiveOakApp.Droid.CustomViews;
using LiveOakApp.Models.ViewModels;
using Android.Views;

namespace LiveOakApp.Droid.Views
{
	public class AttendeeDetailsView : FrameLayout
	{
		public AttendeeDetailsView(Context context) :
			base(context)
		{
			Initialize();
		}

		public AttendeeDetailsView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public AttendeeDetailsView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

		public CircleRemoteImageView AttendeeAvatar { get; private set; }
		public TextView AttendeeName { get; private set; }
		public TextView AttendeeTitle { get; private set; }
		public TextView AttendeeCompany { get; private set; }

        public ExpandedListView InfoList { get; private set; }

		public ImageButton LeadButton { get; private set; }

		void Initialize()
		{
			Inflate(Context, Resource.Layout.AttendeeDetailsLayout, this);

			AttendeeAvatar = FindViewById<CircleRemoteImageView>(Resource.Id.attendee_avatar);

			AttendeeName = FindViewById<TextView>(Resource.Id.attendee_name);
			AttendeeTitle = FindViewById<TextView>(Resource.Id.attendee_title);
			AttendeeCompany = FindViewById<TextView>(Resource.Id.attendee_company);

            InfoList = FindViewById<ExpandedListView>(Resource.Id.info_list);

			LeadButton = FindViewById<ImageButton>(Resource.Id.leadButton);

		}

        public ObservableAdapter<AttendeeInfoItemViewModel> GetInfoListAdapter(ObservableList<AttendeeInfoItemViewModel> infos)
        {
            return infos.GetAdapter(GetInfoItemView);
        }

        public static View GetInfoItemView(int position, AttendeeInfoItemViewModel info, View convertView, View parent)
        {
            var view = convertView != null ? (AttendeeInfoItemView)convertView : new AttendeeInfoItemView(parent.Context);
            view.InfoTitle.Text = info.Key;
            view.InfoValue.Text = info.Value;

            view.InfoValue.AutoLinkMask = 0;

            if (info.Type == AttendeeDetailsViewModel.InfoType.Email)
                view.InfoValue.AutoLinkMask = Android.Text.Util.MatchOptions.EmailAddresses;
            else if (info.Type == AttendeeDetailsViewModel.InfoType.Phone)
                view.InfoValue.AutoLinkMask = Android.Text.Util.MatchOptions.PhoneNumbers;

            return view;
        }

	}
}

