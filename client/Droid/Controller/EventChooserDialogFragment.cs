using Android.Support.V4.App;
using LiveOakApp.Resources;
using System;
using LiveOakApp.Models.ViewModels;

namespace LiveOakApp.Droid.Controller
{
    public class EventChooserDialogFragment : DialogFragment
    {

        public Action<EventViewModel> EventSetter { get; set; }
        public Action LaunchEventSelector { get; set; }

        public override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle(StyleNormal, Resource.Style.CustomDialog);
        }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.EventChooserDialogLayout, container);

            view.FindViewById(Resource.Id.any_event_button).Click += (sender, e) =>
            {
                EventSetter(EventViewModel.CreateAnyEvent());
                Dismiss();
            };

            view.FindViewById(Resource.Id.select_event_button).Click += (sender, e) =>
            {
                LaunchEventSelector();
                Dismiss();
            };

            Dialog.SetTitle(L10n.Localize("SelectEventTitle","Select Event"));

            return view;
        }

    }
}

