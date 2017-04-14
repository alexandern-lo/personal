
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
using LiveOakApp.Models;
using LiveOakApp.Models.ViewModels;

namespace LiveOakApp.Droid.Views
{
    public class AgendaItemView : FrameLayout
    {
        public AgendaItemView(Context context) :
            base(context)
        {
            Initialize();
        }

        public AgendaItemView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public AgendaItemView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.AgendaItemView, this);

            titleView = FindViewById<TextView>(Resource.Id.agenda_title);
            descriptionView = FindViewById<TextView>(Resource.Id.agenda_desc);
            timeView = FindViewById<TextView>(Resource.Id.agenda_time);

        }

        TextView titleView;
        TextView descriptionView;
        TextView timeView;

        private AgendaItemViewModel model;
        public AgendaItemViewModel ViewModel 
        {
            get { return model; }
            set {
                model = value;
                titleView.Text = model.Name;
                descriptionView.Text = model.Description;
                timeView.Text = ServiceLocator.Instance.DateTimeService.TimeToDisplayString(new DateTime(model.StartTime.Ticks), Context) + " \u2014 " + ServiceLocator.Instance.DateTimeService.TimeToDisplayString(new DateTime(model.EndTime.Ticks), Context);
            }
        }
    }
}
