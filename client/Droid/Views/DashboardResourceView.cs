using Android.Content;
using Android.Util;
using Android.Widget;
using LiveOakApp.Models.ViewModels;
using StudioMobile;

namespace LiveOakApp.Droid.Views
{
    public class DashboardResourceView : CustomBindingsView
    {
        public DashboardResourceView(Context context) :
            base(context)
        {
            Initialize();
        }

        public DashboardResourceView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public DashboardResourceView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        DashboardResourceViewModel _viewModel;
        public DashboardResourceViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
            set
            {
                Bindings.Clear();
                _viewModel = value;

                Bindings.Property(ViewModel, _ => _.Name)
                        .To(Name.TextProperty());
                Bindings.Property(ViewModel, _ => _.OpenedCount)
                        .Convert((number) => number.ToString())
                        .To(Opened.TextProperty());
                Bindings.Property(ViewModel, _ => _.SentCount)
                        .Convert((number) => number.ToString())
                        .To(Sent.TextProperty());
                Bindings.Property(ViewModel, _ => _.ResourceTypeImageName)
                        .UpdateTarget((type) =>
                {
                    Icon.SetImageResource(IconIdByType(type.Value));
                });
            }
        }

        static int IconIdByType(string type)
        {
            switch (type)
            {
                case "resources_pdf":
                    return Resource.Drawable.resources_pdf;
                case "resources_ppt":
                    return Resource.Drawable.resources_ppt;
                case "resources_xls":
                    return Resource.Drawable.resources_xls;
                case "resources_doc":
                    return Resource.Drawable.resources_doc;
                case "resources_link":
                    return Resource.Drawable.resources_link;
                case "resources_archive":
                    return Resource.Drawable.resources_archive;
                case "resources_image":
                    return Resource.Drawable.resources_image;
                default:
                    return Resource.Drawable.resources_unknown;
            }
        }

        public ImageView Icon { get; private set; }
        public TextView Name { get; private set; }
        public TextView Sent { get; private set; }
        public TextView Opened { get; private set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.DashboardResourceItemLayout, this);
            Icon = FindViewById<ImageView>(Resource.Id.resource_icon);
            Name = FindViewById<TextView>(Resource.Id.resource_name);
            Sent = FindViewById<TextView>(Resource.Id.resource_sent);
            Opened = FindViewById<TextView>(Resource.Id.resource_opened);
        }
    }
}
