
using System;
using Android.Animation;
using Android.Content;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using LiveOakApp.Models.ViewModels;
using StudioMobile;

namespace LiveOakApp.Droid.Views
{
    public class ResourcesView : FrameLayout
    {
        public ResourcesView(Context context) :
            base(context)
        {
            Initialize();
        }

        public ResourcesView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public ResourcesView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        public SwipeRefreshLayout ResourcesListRefresher { get; private set; }
        public ListView ResourcesList { get; private set; }
        public TextView SendButton { get; private set; }
        public View ProgressBar { get; private set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.MyResourcesFragment, this);
            ResourcesListRefresher = FindViewById<SwipeRefreshLayout>(Resource.Id.resources_list_refresher);
            ResourcesList = FindViewById<ListView>(Resource.Id.resourcesList);
            SendButton = FindViewById<TextView>(Resource.Id.send_resources_button);
            ProgressBar = FindViewById(Resource.Id.progressBar);
            SendButton.Enabled = false;
        }

        public ObservableAdapter<ResourceViewModel> ResourcesAdapter(ObservableList<ResourceViewModel> resources)
        {
            return resources.GetAdapter(GetResourceItemView);
        }

        public Action ResourceChangedAction { get; set; }

        View GetResourceItemView(int position, ResourceViewModel resource, View convertView, ViewGroup parent)
        {
            var view = (ResourceItemView)convertView ?? new ResourceItemView(parent.Context);
            view.ResourceChanged = ResourceChangedAction;
            view.ResourceItem = resource;
            return view;
        }

        bool isShowingProgressBar;
        public bool IsShowingProgressBar
        {
            get
            {
                return isShowingProgressBar;
            }
            set
            {
                isShowingProgressBar = value;

                if (isShowingProgressBar)
                {
                    if (ViewCompat.IsLaidOut(this))
                    {
                        animateShowProgressBar();
                    }
                    else
                    {
                        ProgressBar.Visibility = ViewStates.Visible;
                        ResourcesList.SetY(TypedValue.ApplyDimension(ComplexUnitType.Dip, 50, Context.Resources.DisplayMetrics));
                    }
                }
                else if (ViewCompat.IsLaidOut(this))
                {
                    animateHideProgressBar();
                }
            }
        }

        void animateShowProgressBar()
        {
            var animator = CreateEventsScroll(0, ProgressBar.Height);
            animator.AnimationStart += (sender, e) => ProgressBar.Visibility = ViewStates.Visible;
            animator.Start();
        }

        void animateHideProgressBar()
        {
            var animator = CreateEventsScroll(ProgressBar.Height, 0);
            animator.AnimationEnd += (sender, e) => ProgressBar.Visibility = ViewStates.Invisible;
            animator.Start();
        }

        ValueAnimator CreateEventsScroll(int y0, int y1)
        {
            var animator = ValueAnimator.OfInt(y0, y1);
            animator.SetDuration(500);
            animator.Update += (sender, e) => ResourcesList.SetY((int)e.Animation.AnimatedValue);
            return animator;
        }
    }
}
