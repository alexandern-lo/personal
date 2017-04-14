using Android.Content;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using LiveOakApp.Droid.CustomViews;
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using LiveOakApp.Models;

namespace LiveOakApp.Droid.Views
{
    public class LeadActitvityItemView : LinearLayout
    {

        LeadRecentActivityViewModel lead;

        public CustomImageView PictureImage { get; private set; }
        public TextView NameText { get; private set; }
        public TextView TitleText { get; private set; }
        public TextView TimeText { get; private set; }
        public TextView TypeText { get; private set; }

        public LeadActitvityItemView(Context context) :
            base(context)
        {
            Initialize();
        }

        public LeadActitvityItemView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public LeadActitvityItemView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            SetBackgroundResource(Resource.Drawable.selector_white_to_light);

            SetGravity(GravityFlags.CenterVertical);

            Inflate(Context, Resource.Layout.LeadActivityItemView, this);
            Orientation = Orientation.Horizontal;

            PictureImage = FindViewById<CustomImageView>(Resource.Id.person_item_picture);
            NameText = FindViewById<TextView>(Resource.Id.activity_item_name);
            TitleText = FindViewById<TextView>(Resource.Id.activity_item_title);
            TimeText = FindViewById<TextView>(Resource.Id.activity_action_time);
            TypeText = FindViewById<TextView>(Resource.Id.activity_action_type);
        }

        public LeadRecentActivityViewModel Lead
        {
            get { return lead; }
            set
            {
                lead = value;
                NameText.Text = lead.FullName;
                #pragma warning disable 0618
                TitleText.TextFormatted = Html.FromHtml("<b>" + L10n.Localize("RecentActivityItemEventTitle", "Event:") + "</b> " + lead.EventName);
                #pragma warning restore 0618
                TypeText.Text = lead.PerformedAction.ToString();
                TimeText.Text = lead.GetPerformedAtFormatted(Context);
                SetPhotoResource(lead.PhotoResource);
                if (LeadActionType.Deleted.Equals(lead.PerformedAction))
                {
                    SetBackgroundColor(Android.Graphics.Color.White);
                }
                else {
                    SetBackgroundResource(Resource.Drawable.selector_white_to_light);
                }
            }
        }

        void SetPhotoResource(FileResource photo)
        {
            if (PictureImage.ScheduledWork != null)
            {
                PictureImage.ScheduledWork.Cancel();
                PictureImage.ScheduledWork = null;
            }
            var localPath = photo?.AbsoluteLocalPath;
            var remoteUrl = photo?.RemoteUrl;
            if (remoteUrl != null && remoteUrl.Length != 0)
            {
                PictureImage.LoadByUrl(remoteUrl);
            }
            else if (localPath != null && localPath.Length != 0)
            {
                PictureImage.LoadByPath(localPath);
            }
            else
            {
                PictureImage.SetImageResource(Resource.Drawable.lead_placeholder);
            }
        }

        public void Recycle()
        {
            if (PictureImage.ScheduledWork != null)
            {
                PictureImage.ScheduledWork.Cancel();
                PictureImage.ScheduledWork = null;
            }
            PictureImage.SetImageDrawable(null);
        }
    }
}
