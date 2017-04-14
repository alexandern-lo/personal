
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Android.Support.V4.View;
using Android.Views;

namespace StudioMobile
{
    public class CustomPagerAdapter<T> : PagerAdapter, IBinding
    {

        #region pager adapter

        public override void DestroyItem(Android.Views.ViewGroup container, int position, Java.Lang.Object objectValue)
        {
            container.RemoveView(objectValue as View);
        }

        public override float GetPageWidth(int position)
        {
            return ItemWidth;
        }

        public override Java.Lang.Object InstantiateItem(Android.Views.ViewGroup container, int position)
        {
            View item;
            if (LeftView != null && position == 0)
                item = LeftView;
            else if (RightView != null && position == Count - 1)
                item = RightView;
            else item = ViewFactory(LeftView == null ? position : position - 1);
            container.AddView(item);
            return item;
        }

        public override bool IsViewFromObject(Android.Views.View view, Java.Lang.Object objectValue)
        {
            return view == objectValue;
        }

        public override int Count
        {
            get
            {
                if (_list == null)
                    return 0;

                var count = _list.Count;

                if (LeftView != null)
                    count++;
                if (RightView != null)
                    count++;

                return count;
            }
        }

        public override int GetItemPosition(Java.Lang.Object objectValue)
        {
            return PositionNone; // it means position has changed
        }

        public float ItemWidth { get; set; }

        #endregion

        #region  IBinding 

        public Func<int, View> ViewFactory
        {
            get;
            set;
        }

        public bool Enabled
        {
            get;
            set;
        }

        public bool Bound
        {
            get;
            private set;
        }

        IList<T> _list;
        INotifyCollectionChanged _notifier;

        public IList<T> DataSource
        {
            get
            {
                return _list;
            }
            set
            {
                if (Bound) throw new InvalidOperationException("Cannot change DataSource when adapter is bound.");

                if (Equals(_list, value))
                {
                    return;
                }

                _list = value;
                _notifier = _list as INotifyCollectionChanged;
            }
        }

        void NotifierCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Enabled)
                NotifyDataSetChanged();
        }

        public void Bind()
        {
            if (Bound)
                return;
            Check.State(DataSource != null).IsTrue("Source is not set");

            if (_notifier != null)
            {
                _notifier.CollectionChanged += NotifierCollectionChanged;
            }
            Bound = true;
        }

        public void Unbind()
        {
            if (!Bound)
                return;
            if (_notifier != null)
            {
                _notifier.CollectionChanged -= NotifierCollectionChanged;
            }
            Bound = false;
        }

        public void UpdateTarget()
        {
            NotifyDataSetChanged();
        }

        public void UpdateSource()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region custom left and right views

        public View LeftView { get; set; }
        public View RightView { get; set; }

        #endregion

    }

    public static class CollectionsCustomPagerAdapterExtensions
    {
        
        public static CustomPagerAdapter<T> GetPagerAdapter<T>(this IList<T> collection, Func<int, View> viewFactory, float itemWidthScale = 1.0f)
        {
            return new CustomPagerAdapter<T>
            {
                DataSource = collection,
                ViewFactory = viewFactory,
                ItemWidth = itemWidthScale
            };
        }
    }
}

