using System;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;

namespace StudioMobile
{
	public static class TextViewBindings
	{
		public static readonly RuntimeEvent TextChangedEvent = new RuntimeEvent (typeof (TextView), "TextChanged");
		public static readonly IPropertyBindingStrategy TextChangedBinding = new EventHandlerBindingStrategy<TextChangedEventArgs> (TextChangedEvent);

		public static IProperty<string> TextProperty (this TextView view)
		{
            return view.GetProperty(_ => _.Text, TextChangedBinding);
		}

        [Preserve]
        static void LinkerTrick()
        {
            var t = new TextView(null);
            t.TextChanged += (o, a) => { };
            t.EditorAction += (sender, e) => { };
        }

        public static readonly RuntimeEvent EditorActionEvent = new RuntimeEvent(typeof(TextView), "EditorAction");

        public static EventHandlerSource<T> EditorActionTarget<T>(this T view)
            where T : TextView
        {
            return new EventHandlerSource<T>(EditorActionEvent, view)
            {
                ParameterExtractor = (sender, args) => { return ((TextView.EditorActionEventArgs)args).ActionId; } // TODO: pass both args
            };
        }
    }

	public static class CompoundButtonBindings
	{
		public static readonly RuntimeEvent CheckedChangeEvent = new RuntimeEvent(typeof(CompoundButton), "CheckedChange");
		public static readonly IPropertyBindingStrategy CheckedChangeBinding = new EventHandlerBindingStrategy<CompoundButton.CheckedChangeEventArgs>(CheckedChangeEvent);

		public static IProperty<bool> CheckedProperty(this CompoundButton view)
		{
			return view.GetProperty(_ => _.Checked, CheckedChangeBinding);
		}

		[Preserve]
		static void LinkerTrick()
		{
			new Switch(null).CheckedChange += (o, a) => { }; // switch extends CompoundButton
		}

	}

	public static class RemoteImageViewBindings
	{
		public static readonly RuntimeEvent ImageChangedEvent = new RuntimeEvent(typeof(RemoteImageView), "ImageChanged");
		public static readonly IPropertyBindingStrategy ImageChangedBinding = new EventHandlerBindingStrategy<EventArgs>(ImageChangedEvent);

		public static IProperty<RemoteImage> ImageProperty(this RemoteImageView view) 
		{
            return view.GetProperty(_ => _.Image, ImageChangedBinding);
		}

        [Preserve]
        static void LinkerTrick()
        {
            new RemoteImageView(null).ImageChanged += (o, a) => { };
        }
    }

	public static class ViewBindings
	{
		public static readonly RuntimeEvent ClickEvent = new RuntimeEvent(typeof(View), "Click");
		public static EventHandlerSource<T> ClickTarget<T>(this T view)
			where T : View
		{
			return new EventHandlerSource<T>(ClickEvent, view)
			{
				SetEnabledAction = SetViewEnabled
			};
		}

		static void SetViewEnabled(View view, bool enabled)
		{
			view.Enabled = enabled;
		}

        [Preserve]
        static void LinkerTrick()
        {
            new View(null).Click += (o, a) => { };
        }
    }

    public static class ViewPagerBindings
    {
        public static BindingList Adapter<T>(this BindingList bindings, ViewPager viewPager, T adapter)
            where T : PagerAdapter
        {
            viewPager.Adapter = adapter;
            if (adapter is IBinding)
            {
                bindings.Add((IBinding)adapter);
            }
            return bindings;
        }

        public static readonly RuntimeEvent PageSelectedEvent = new RuntimeEvent(typeof(ViewPager), "PageSelected");
        public static readonly IPropertyBindingStrategy PageSelectedEventBinding = new EventHandlerBindingStrategy<ViewPager.PageSelectedEventArgs>(PageSelectedEvent);

        public static IProperty<int> SelectedPagePositionProperty(this ViewPager viewPager)
        {
            return viewPager.GetProperty(_ => _.CurrentItem, PageSelectedEventBinding);
        }

    }

	public static class AdapterViewBindings
	{
		/// <summary>
		/// Set adapter into view and then add it as a binding if it support IBinding interface. Usually this is done 
		/// to make adapter subscribe/unsubscribe to model together will all other bindings.
		/// </summary>
		/// <param name="bindings">Binding list</param>
		/// <param name="view">Adapter owner view</param>
		/// <param name="adapter">Adapter instance</param>
		/// <typeparam name="T">Adapter type</typeparam>
		public static BindingList Adapter<T> (this BindingList bindings, AdapterView<T> view, T adapter)
		where T : IAdapter
		{
			view.Adapter = adapter;
			if (adapter is IBinding) {
				bindings.Add ((IBinding)adapter);
			}
			return bindings;
		}

        public static readonly RuntimeEvent ItemSelectedEvent = new RuntimeEvent (typeof (AdapterView), "ItemSelected");
		public static readonly IPropertyBindingStrategy ItemSelectedEventBinding = new EventHandlerBindingStrategy<AdapterView.ItemSelectedEventArgs> (ItemSelectedEvent);

        public static EventHandlerSource<T> ItemSelectedTarget<T>(this T view)
			where T : AdapterView	{
			return new EventHandlerSource<T>(ItemSelectedEvent, view)
			{
				SetEnabledAction = SetViewEnabled,
                ParameterExtractor = (sender, args) => ((AdapterView.ItemSelectedEventArgs)args).Position
			};
		}

        public static readonly RuntimeEvent ItemClickEvent = new RuntimeEvent(typeof(AdapterView), "ItemClick");
        public static EventHandlerSource<T> ItemClickTarget<T>(this T view)
            where T : AdapterView   {
            return new EventHandlerSource<T>(ItemClickEvent, view)
            {
                SetEnabledAction = SetViewEnabled,
                ParameterExtractor = (sender, args) => ((AdapterView.ItemClickEventArgs)args).Position
            };
        } 
		static void SetViewEnabled(AdapterView view, bool enabled)
		{
			view.Enabled = enabled;
		}

		public static AdapterBinding<T> Adapter<T> (this AdapterView<T> view) where T : IAdapter
		{
			return new AdapterBinding<T> (view);
		}

		/// <summary>
		/// Helper class to make Adapter related properties bindings.
		/// </summary>
		public class AdapterBinding<T> where T : IAdapter
		{
			public AdapterBinding (AdapterView<T> view)
			{
				View = view;
			}

			public AdapterView<T> View { get; set; }

			/// <summary>
			/// Binding for SelectedItemProperty. It listen to 'ItemSelected' event, extracts
			/// selected object from Adapter and update source.
			/// NOTE: this logic requires Adapter to be subtype of Xamarin BaseAdapter. Otherwise there is no easy 
			/// way to get .NET object from Java Adapter.
			/// </summary>
			/// <returns>Property ready for binding</returns>
			/// <typeparam name="U">Type of adapter contents</typeparam>
			public IProperty<U> SelectedItemProperty<U> ()
			{
                var property = View.GetProperty(_ => _.SelectedItemPosition, ItemSelectedEventBinding, setter: (view, pos) => view.SetSelection(pos));
                Func<int, U> posToItem = (pos) => {
					var adapter = View.Adapter as BaseAdapter<U>;
					if (adapter == null) {
						throw new InvalidOperationException ("This type of Adapter is not supported. Use adapter derived from BaseAdapter<T> or use custom binding");
					}
					return adapter [pos];
				};
                Func<U, int> itemToPos = null;
                var reverseAdapter = View.Adapter as IReverseAdapter<U>;
                if (reverseAdapter != null)
                {
                    itemToPos = (item) =>
                    {
                        return reverseAdapter.GetPosition(item);
                    };
                }
                return property.Convert(posToItem, itemToPos);
			}
		}

        [Preserve]
        static void LinkerTrick()
        {
            new ListView(null).ItemSelected += (o, a) => { };
            new ListView(null).ItemClick += (o, a) => { };
        }
    }

}
