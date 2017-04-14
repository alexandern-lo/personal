using System;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using LiveOakApp.Models.ViewModels;
using StudioMobile;

namespace LiveOakApp.Droid.Views
{
	public class EmailView : CustomBindingsView
	{
		public EmailView(Context context) :
			base(context)
		{
			Initialize();
		}

		public EmailView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public EmailView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

        LeadDetailsEmailViewModel viewModel;
        public LeadDetailsEmailViewModel ViewModel {
            get {
                return viewModel;
            }
            set
            {
                Bindings.Clear();

                viewModel = value;

                Bindings.Property(ViewModel, _ => _.Email)
                        .To(EmailEdit.TextProperty());
                Bindings.Property(ViewModel, _ => _.Email)
                        .UpdateTarget((email) => EmailStringChanged());

                Bindings.Adapter(EmailTypeSpinner, EmailSpinnerAdapter(ViewModel.EmailTypes));

                Bindings.Property(ViewModel, _ => _.TypeString)
                        .To(EmailTypeSpinner.Adapter().SelectedItemProperty<string>());

                Bindings.Command(ViewModel.RemoveEmailCommand)
                        .To(DeleteButton.ClickTarget());

                Bindings.Property(ViewModel, _ => _.IsEmailValid)
                        .UpdateTarget((isValid) => 
                {
                    if (isValid.Value)
                        ResetHighlight();
                    else
                        HighlightEmail();
                });
            }
        }

        public Action EmailStringChanged { get; set; }

        public ImageButton DeleteButton { get; private set; }
		public EditText EmailEdit { get; private set; }
		public Spinner EmailTypeSpinner { get; private set; }
        public View EmailUnderline { get; private set; }

		void Initialize()
		{
			Inflate(Context, Resource.Layout.EmailLayout, this);

            DeleteButton = FindViewById<ImageButton>(Resource.Id.delete_button);

			EmailEdit = FindViewById<EditText>(Resource.Id.email_edit);
            EmailEdit.RequestFocus();

			EmailTypeSpinner = FindViewById<Spinner>(Resource.Id.email_type);
            EmailUnderline = FindViewById(Resource.Id.email_underline);

            EmailStringChanged = () => { };
		}

        public void HighlightEmail()
        {
            EmailUnderline.SetBackgroundColor(Color.Red);
        }

        public void ResetHighlight() 
        {
            var gray = new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray));
            EmailUnderline.SetBackgroundColor(gray);
        }

        public ObservableAdapter<string> EmailSpinnerAdapter(ObservableList<string> emailTypes)
        {
            return emailTypes.GetSpinnerAdapter(GetEmailTypeItemView, GetEmailsDropDownView);
        }

        public static View GetEmailTypeItemView(int position, string emailType, View convertView, View parent)
		{
			var view = (TextView)convertView ?? ((TextView)LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SpinnerItem, null));
			view.Text = emailType;
			return view;
		}

		public static View GetEmailsDropDownView(int position, string emailType, View convertView, View parent)
		{
			var view = (TextView)convertView ?? ((TextView)LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SpinnerItem, null));
			view.SetBackgroundResource(Resource.Color.primary_light);
			view.Text = emailType;
			return view;
		}
	}
}

