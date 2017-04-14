using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using LiveOakApp.Models.ViewModels;
using StudioMobile;

namespace LiveOakApp.Droid.Views
{
    public class PhoneView : CustomBindingsView
	{
		public PhoneView(Context context) :
			base(context)
		{
			Initialize();
		}

		public PhoneView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public PhoneView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

        LeadDetailsPhoneViewModel viewModel;
        public LeadDetailsPhoneViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
            set
            {
                Bindings.Clear();

                viewModel = value;

                Bindings.Property(ViewModel, _ => _.Phone)
                        .To(PhoneEdit.TextProperty());
                Bindings.Property(ViewModel, _ => _.Phone)
                        .UpdateTarget((phone) => PhoneStringChanged());

                Bindings.Adapter(PhoneTypeSpinner, PhoneSpinnerAdapter(ViewModel.PhoneTypes));

                Bindings.Property(ViewModel, _ => _.TypeString)
                        .To(PhoneTypeSpinner.Adapter().SelectedItemProperty<string>());

                Bindings.Command(ViewModel.RemovePhoneCommand)
                        .To(DeleteButton.ClickTarget());
            }
        }


        public ImageButton DeleteButton { get; private set; }

		public EditText PhoneEdit { get; private set; }

		public Spinner PhoneTypeSpinner { get; private set; }

        public Action PhoneStringChanged { get; set; }

        void Initialize()
		{
			Inflate(Context, Resource.Layout.PhoneLayout, this);

            DeleteButton = FindViewById<ImageButton>(Resource.Id.delete_button);

			PhoneEdit = FindViewById<EditText>(Resource.Id.phone_edit);
            PhoneEdit.RequestFocus();

			PhoneTypeSpinner = FindViewById<Spinner>(Resource.Id.phone_type);
            PhoneStringChanged = () => { };
		}

        public ObservableAdapter<string> PhoneSpinnerAdapter(ObservableList<string> phoneTypes)
        {
            return phoneTypes.GetSpinnerAdapter(GetPhoneTypeItemView, GetPhonesDropDownView);
        }

		public static View GetPhoneTypeItemView(int position, string phoneType, View convertView, View parent)
		{
			var view = convertView != null ? (TextView)convertView :
				(TextView)LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SpinnerItem, null);
			view.Text = phoneType;
			return view;
		}

		public static View GetPhonesDropDownView(int position, string phoneType, View convertView, View parent)
		{
			var view = convertView != null ? (TextView)convertView :
				(TextView)LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SpinnerItem, null);
			view.SetBackgroundResource(Resource.Color.primary_light);
			view.Text = phoneType;
			return view;
		}
	}
}