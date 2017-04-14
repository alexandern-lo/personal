
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace LiveOakApp.Droid.Views
{
    public class AgendaHeaderView : FrameLayout
    {
        public AgendaHeaderView(Context context) :
            base(context)
        {
            Initialize();
        }

        public AgendaHeaderView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public AgendaHeaderView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.AgendaHeaderView, this);

            LocationView = FindViewById<TextView>(Resource.Id.location);
        }

        public TextView LocationView;
    }
}
