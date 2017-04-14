using System;
using System.Runtime.CompilerServices;

#if __IOS__
using UIKit;
using View = UIKit.UIView;
using ContainerView = UIKit.UIView;
#endif

#if __ANDROID__
using View = Android.Views.View;
using ContainerView = Android.Views.ViewGroup;
#endif

namespace StudioMobile
{
	/// <summary>
	/// Helper class to implement Lazy view pattern. 
	/// View is lazy when it instantiated and added to view hierarchy only when specifically requested by user. 
	/// Typically this is implemented like this
	/// <code>
	/// public View MyLazyView {
	/// 	get { 
	/// 		if (myLazyView == null) {
	/// 			MyLazyView = new View(); 			
	/// 		}
	/// 	}
	/// 	set {
	/// 		if (value != myLazyView) {
	/// 			if (myLazyView != null) myLazyView.RemoveFromSuperview();
	/// 			if (value != null) this.AddSubview(value);
	/// 			myLazyView = value;
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// 
	/// With this helper class this reduced to
	/// <code>
	/// public View MyLazyView {
	/// 	get { 
	/// 		return this.GetLazyView(ref myLazyView);
	/// 	}
	/// 	set {
	/// 		this.SetLazyView(ref myLazyView, value);
	/// 	}
	/// }
	/// </code>
	/// </summary>
	public static class LazyView
	{
		public static T GetLazyView<T>(this ContainerView owner, 
		                               ref T view, 
		                               [CallerMemberName] string prop = "") 
			where T : View, new()
		{
			return owner.GetLazyView(ref view, () => new T(), prop);
		}

		public static T GetLazyView<T>(this ContainerView owner, 
		                               ref T view, 
		                               Func<T> factory, 
		                               [CallerMemberName] string prop = "") 
			where T : View
		{
			if (view == null)
			{
				var newView = factory();
				KeyValueCoding.Impl<ContainerView>().Set(owner, prop, newView);
				if (view != newView)
				{
					throw new InvalidOperationException(
						"Setter didn't populated lazy view field. "+
						"Please check LazyView documentation and follow suggested usage pattern/");
				}
			}
			return view;
		}

		public static void SetView<T>(this ContainerView owner, ref T view, T value) where T : View
		{
			owner.SetView(ref view, value, (o, v) => ViewBuilder.AddSubview(o, v));
		}

		public static void SetView<T>(this ContainerView owner, ref T view, T value, Action<ContainerView, T> combine) where T : View
		{
			if (value != view)
			{
				if (view != null) ViewBuilder.RemoveSubview(owner, view);
				if (value != null) combine(owner, value);
				view = value;
			}
		}
	}
	
}
