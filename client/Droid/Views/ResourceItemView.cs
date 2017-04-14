using System;
using Android.Content;
using Android.Util;
using Android.Widget;
using StudioMobile;
using LiveOakApp.Models.ViewModels;

namespace LiveOakApp.Droid.Views
{
    public class ResourceItemView : CustomBindingsView
    {
        public ResourceItemView(Context context) :
            base(context)
        {
            Initialize();
        }

        public ResourceItemView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public ResourceItemView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        ResourceViewModel ViewModel;

        TextView titleView;
        ImageView iconView;
        TextView descriptionView;
        public CheckBox SelectedView;

        void Initialize()
        {
            Inflate(Context, Resource.Layout.ResourceItem, this);
            titleView = FindViewById<TextView>(Resource.Id.resource_title);
            iconView = FindViewById<ImageView>(Resource.Id.resource_type);
            descriptionView = FindViewById<TextView>(Resource.Id.resource_desc);
            SelectedView = FindViewById<CheckBox>(Resource.Id.resource_selected);

            SelectedView.CheckedChange += (sender, e) => 
            {
                if (ViewModel != null)
                    ViewModel.Selected = e.IsChecked;
                if (ResourceChanged != null)
                    ResourceChanged();
            };
        }

        public ResourceViewModel ResourceItem 
        {
            get { return ViewModel; }
            set
            {
                ViewModel = value;
                titleView.Text = ViewModel.Title;
                iconView.SetImageResource(IconIdByType(ViewModel.ResourceTypeImageName));
                descriptionView.Text = ViewModel.Description;
                SelectedView.Checked = ViewModel.Selected;
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

        public Action ResourceChanged { get; set; }
    }
}
