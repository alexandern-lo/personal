﻿using Android.OS;
using Android.Views;
using Android.Widget;
using StudioMobile;

namespace LiveOakApp.Droid.Controller
{
	public class AnalyticsFragment : CustomFragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			View v = inflater.Inflate (Resource.Layout.PlaceholderFragment, container, false);
			var textView = v.FindViewById<TextView> (Resource.Id.fragment_title);
			textView.Text = "Analytics Fragment";
			return v;
		}
	}
}

