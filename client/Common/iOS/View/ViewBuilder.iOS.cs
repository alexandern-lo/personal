using System;
using UIKit;

namespace StudioMobile
{
	public partial class ViewBuilder
	{
		static readonly Type[] NoArguments = { };

		public static void AddSubview(UIView parent, UIView child)
		{
			parent.AddSubview(child);
		}

		public static void RemoveSubview(UIView parent, UIView child)
		{
			child.RemoveFromSuperview();
		}

		public static UIView CreateView(UIView parent, Type outletType)
		{
			var constructor = outletType.GetConstructor(NoArguments);
			if (constructor == null)
			{
				LOG.Error("Cannot find default constructor for {0}. Please check type has default constructor and is referenced somewhere in your code to ensure linker does not remove it. See LinkerHack class for example how to trick linker.", outletType);
			}
			return outletType.GetConstructor(NoArguments).Invoke(NoArguments) as UIView;
		}
	}

	public static class LinkerHack
	{
		public static object[] ToMakeSureLinkerDoesNotRemoveDefaultConstructorOfUIKitControls()
		{
			return new object[] {
				new UIView(),
				new UIControl(),
				new UIButton(),
				new UIDatePicker(),
				new UIPageControl(),
				new UISegmentedControl(),
				new UIImageView(),
				new UITextField(),
				new UISlider(),
				new UIStepper(),
				new UISwitch(),
				new UITableView(),
				new UILabel(),
				new UITabBar(),
				new UIProgressView(),
				new UIWebView(),
				new UIActionSheet(),
				new UIActivityIndicatorView(),
				new UINavigationBar(),
				new UIPickerView(),
				new UIScrollView(),
				new UISearchBar(),
				new UITextView(),
				new UIToolbar()
			};
		}
	}
}

