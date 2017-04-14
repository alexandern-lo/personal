using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SE.Emilsjolander.Stickylistheaders;
using StudioMobile;
using LiveOakApp.Models.ViewModels;

namespace LiveOakApp.Droid.CustomViews.Adapters
{
    public class StickyHeadersListViewAdapter<GT, T> : GroupedObservableAdapter<GT, T>, IStickyListHeadersAdapter 
        where GT : IList<T>, INotifyCollectionChanged
    {
        public long GetHeaderId(int position)
        {
            GT groupData;
            GetItemAtPosition(position, out groupData);
            return groupData.GetHashCode();
        }

        public View GetHeaderView(int position, View convertView, ViewGroup parent)
        {
            if (HeaderViewFactory == null)
                return convertView;
            
            GT groupData;
            GetItemAtPosition(position, out groupData);

            return HeaderViewFactory(position, groupData, convertView, parent);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            GT groupData;
            var item = GetItemAtPosition(position, out groupData);
            if (item is T && ViewFactory != null)
            {
                return ViewFactory(position, groupData, (T)item, convertView, parent);
            }
            return convertView;
        }
    }
    public static class StickyHeadersCollectionAdapterExtension
    {     
        public static StickyHeadersListViewAdapter<GT, T> GetStickyHeadersAdapter<GT, T>(this IList<GT> collection,
                                                         Func<int, GT, T, View, ViewGroup, View> viewFactory,
                                                         Func<int, GT, View, ViewGroup, View> headerViewFactory)
            where GT : IList<T>, INotifyCollectionChanged
        {
            return new StickyHeadersListViewAdapter<GT, T>
            {
                DataSource = collection,
                ViewFactory = viewFactory,
                HeaderViewFactory = headerViewFactory
            };
        }
    }

    public static class StickyHeadersListViewBindingsExtensions
    {
        
        public static BindingList Adapter<T>(this BindingList bindings, StickyListHeadersListView view, T adapter)
        where T : IStickyListHeadersAdapter
        {
            view.Adapter = adapter;
            if (adapter is IBinding)
            {
                bindings.Add((IBinding)adapter);
            }
            return bindings;
        }

        public static readonly RuntimeEvent ItemClickEvent = new RuntimeEvent(typeof(StickyListHeadersListView), "ItemClick");
        public static EventHandlerSource ItemClickTarget(this StickyListHeadersListView view)
        {
            return new EventHandlerSource(ItemClickEvent, view)
            {
                ParameterExtractor = (sender, args) => ((StickyListHeadersListView.ItemClickEventArgs)args).P2 // position
            };
        }


        [Preserve]
        static void LinkerTrick()
        {
            new StickyListHeadersListView(null).ItemClick += (o, a) => { };
        }
    }
}

