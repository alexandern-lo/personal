using Android.Content;
using Android.Support.V4.View;
using Android.Util;
using Android.Widget;
using Android.Views;
using Android.Animation;
using Android.Support.V4.Widget;
using StudioMobile;
using LiveOakApp.Models.ViewModels;

namespace LiveOakApp.Droid.Views
{
    public class InboxView : FrameLayout
    {
        public InboxView(Context context) :
            base(context)
        {
            Initialize();
        }

        public InboxView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public InboxView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        #region Progress bar

        public View ProgressBar { get; private set; }

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
                        ContentView.SetY(TypedValue.ApplyDimension(ComplexUnitType.Dip, 50, Context.Resources.DisplayMetrics));
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
            var animator = CreateContentScroll(0, ProgressBar.Height);
            animator.AnimationStart += (sender, e) => ProgressBar.Visibility = ViewStates.Visible;
            animator.Start();
        }

        void animateHideProgressBar()
        {
            var animator = CreateContentScroll(ProgressBar.Height, 0);
            animator.AnimationEnd += (sender, e) => ProgressBar.Visibility = ViewStates.Invisible;
            animator.Start();
        }

        ValueAnimator CreateContentScroll(int y0, int y1)
        {
            var animator = ValueAnimator.OfInt(y0, y1);
            animator.SetDuration(500);
            animator.Update += (sender, e) => ContentView.SetY((int)e.Animation.AnimatedValue);
            return animator;
        }

        #endregion

        public SwipeRefreshLayout InboxListRefresher { get; private set; }
        public ListView InboxList { get; private set; }
        public View ContentView { get; private set; }
        public TextView LastUpdated { get; private set; }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.InboxLayout, this);
            InboxListRefresher = FindViewById<SwipeRefreshLayout>(Resource.Id.inbox_list_refresher);
            ProgressBar = FindViewById(Resource.Id.progressBar);
            InboxList = FindViewById<ListView>(Resource.Id.inbox_list);
            LastUpdated = FindViewById<TextView>(Resource.Id.last_update);
            ContentView = FindViewById(Resource.Id.content_view);

            ChildrenBindingList = new WeakBindingList();
        }

        public WeakBindingList ChildrenBindingList { get; private set; }

        public ObservableAdapter<InboxItemViewModel> GetInboxAdapter(ObservableList<InboxItemViewModel> inboxItems) 
        {
            return inboxItems.GetAdapter(GetInboxItemView);
        }

        public View GetInboxItemView(int position, InboxItemViewModel viewModel, View convertView, View parent)
        {
            var view = convertView as InboxItemView;
            if(view == null)
            {
                view = new InboxItemView(parent.Context);
                view.LongClickable = true;
                var bindings = new BindingList();
                view.ResetBingings(bindings);
                ChildrenBindingList.Add(bindings);
            }
            view.ViewModel = viewModel;
            view.Bindings.UpdateTarget();
            return view;
        }
    }
}
