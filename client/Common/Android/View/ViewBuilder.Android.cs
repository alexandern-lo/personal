using System;
using Android.Content;
using Android.Views;

namespace StudioMobile
{
	public partial class ViewBuilder
	{
		static readonly Type[] ContructorArgs = { typeof(Context) };

		public static void AddSubview(ViewGroup parent, View child)
		{
			parent.AddView(child);
		}

		public static void RemoveSubview(ViewGroup parent, View child)
		{
			parent.RemoveView(child);
		}

		public static View CreateView(View parent, Type outletType)
		{
			var constructor = outletType.GetConstructor(ContructorArgs);
			if (constructor == null)
			{
				LOG.Error("Cannot find constructor with 'Context' for {0}. Please check type has constructor with one argument of type Android.Content.Context and is referenced somewhere in your code to ensure linker does not remove it. See LinkerHack class for example how to trick linker.", outletType);
			}
			return outletType.GetConstructor(ContructorArgs).Invoke(new object[] { parent.Context }) as View;
		}
	}

	public static class LinkerHack
	{
		public static object[] ToMakeSureLinkerDoesNotRemoveDefaultConstructorOfUIKitControls()
		{
			return new object[] {
			};
		}
	}
}

