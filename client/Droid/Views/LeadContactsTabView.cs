using System;
using System.Collections.Generic;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading.Work;
using LiveOakApp.Droid.CustomViews;
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Models.Services;
using LiveOakApp.Models.ViewModels;
using StudioMobile;
using Android.Support.V4.Content;

namespace LiveOakApp.Droid.Views
{
    public class LeadContactsTabView : FrameLayout
	{
		public LeadContactsTabView(Context context) :
			base(context)
		{
			Initialize();
		}

		public LeadContactsTabView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public LeadContactsTabView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

        public TextView EventView { get; private set; }
		public EditText NameEdit { get; private set; }
        public View NameUnderline { get; private set; }
		public EditText SurnameEdit { get; private set; }
        public View SurnameUnderline { get; private set; }
		public EditText CompanyEdit { get; private set; }
        public View CompanyUnderline { get; private set; }
		public EditText TitleEdit { get; private set; }
        public View TitleUnderline { get; private set; }
		public EditText AddressEdit { get; private set; }
        public View AddressUnderline { get; private set; }
		public EditText CityEdit { get; private set; }
        public View CityUnderline { get; private set; }
		public EditText StateEdit { get; private set; }
        public View StateUnderline { get; private set; }
        public Spinner StateSpinner { get; private set; }
        public EditText ZIPEdit { get; private set; }
        public View ZIPUnderline { get; private set; }
        public Spinner CountrySpinner { get; private set; }
        public View CountryUnderline { get; private set; }
		public EditText CompanyURLEdit { get; private set; }
        public View CompanyURLUnderline { get; private set; }
		public EditText NotesEdit { get; private set; }
        public View NotesUnderline { get; private set; }
		public TextView FirstEntryText { get; private set; }
		public ExpandedListView EmailsList { get; private set; }
		public ExpandedListView PhonesList { get; private set; }
		public Button AddPhotoButton { get; private set; }
        public Button AddBusinessCardButton { get; private set; }
		public Button SendResourcesButton { get; private set; }
        public CustomImageView PhotoView { get; private set; }
        public ImageButton AddEmailButton { get; private set; }
        public ImageButton AddPhoneButton { get; private set; }
        public TextView EventErrorView { get; private set; }
        public View EventUnderline { get; private set; }
        public ImageView BusinessCardFrontThumb { get; private set; }
        public ImageView BusinessCardBackThumb { get; private set; }
        public ImageView CRMLogo { get; private set; }

		void Initialize()
		{
			Inflate(Context, Resource.Layout.LeadContactsTabLayout, this);
			EmailsList = FindViewById<ExpandedListView>(Resource.Id.emails_holder);
			AddEmailButton = CreateButtonForList(Context);
			EmailsList.AddFooterView(AddEmailButton);
			PhonesList = FindViewById<ExpandedListView>(Resource.Id.phones_holder);
			AddPhoneButton = CreateButtonForList(Context);
			PhonesList.AddFooterView(AddPhoneButton);
            EventView = FindViewById<TextView>(Resource.Id.current_event);
			NameEdit = FindViewById<EditText>(Resource.Id.name_edit);
            NameUnderline = FindViewById(Resource.Id.name_underline);
			SurnameEdit = FindViewById<EditText>(Resource.Id.surname_edit);
            SurnameUnderline = FindViewById(Resource.Id.surname_underline);
			CompanyEdit = FindViewById<EditText>(Resource.Id.company_edit);
            CompanyUnderline = FindViewById(Resource.Id.company_underline);
			TitleEdit = FindViewById<EditText>(Resource.Id.title_edit);
            TitleUnderline = FindViewById(Resource.Id.title_underline);
			AddressEdit = FindViewById<EditText>(Resource.Id.address_edit);
            AddressUnderline = FindViewById(Resource.Id.address_underline);
			CityEdit = FindViewById<EditText>(Resource.Id.city_edit);
            CityUnderline = FindViewById(Resource.Id.city_underline);
			ZIPEdit = FindViewById<EditText>(Resource.Id.zip_edit);
            ZIPUnderline = FindViewById(Resource.Id.zip_underline);
			StateEdit = FindViewById<EditText>(Resource.Id.state_edit);
            StateUnderline = FindViewById(Resource.Id.state_underline);
            StateSpinner = FindViewById<Spinner>(Resource.Id.state_spinner);
            CountrySpinner = FindViewById<Spinner>(Resource.Id.country_edit);
            CountryUnderline = FindViewById(Resource.Id.country_underline);
			CompanyURLEdit = FindViewById<EditText>(Resource.Id.company_url_edit);
            CompanyURLUnderline = FindViewById(Resource.Id.company_url_underline);
			NotesEdit = FindViewById<EditText>(Resource.Id.notes_edit);
            NotesUnderline = FindViewById(Resource.Id.notes_underline);
			FirstEntryText = FindViewById<TextView>(Resource.Id.first_location_content);
			AddPhotoButton = FindViewById<Button>(Resource.Id.add_photo_button);
			SendResourcesButton = FindViewById<Button>(Resource.Id.send_resources_button);
            PhotoView = FindViewById<CustomImageView>(Resource.Id.photo_view);
            EventErrorView = FindViewById<TextView>(Resource.Id.event_error);
            EventUnderline = FindViewById(Resource.Id.spinner_underline);
            AddBusinessCardButton = FindViewById<CustomButton>(Resource.Id.add_business_card_photo_button);
            BusinessCardFrontThumb = FindViewById<ImageView>(Resource.Id.front_card);
            BusinessCardBackThumb = FindViewById<ImageView>(Resource.Id.back_card);
            CRMLogo = FindViewById<ImageView>(Resource.Id.crm_logo);

            ChildrenBindingList = new WeakBindingList();

            this.Click += (sender, e) => 
            {
                CountrySpinner.ClearFocus();
                StateSpinner.ClearFocus();
            };
        }

        public void UpdateCRMLogo(CrmService.CrmType crmType, LeadDetailsViewModel.CrmExportStates exportState) 
        {
            switch(crmType)
            {
                case CrmService.CrmType.Dynamics365:
                    CRMLogo.Visibility = ViewStates.Visible;
                    if (exportState == LeadDetailsViewModel.CrmExportStates.NotExported)
                        CRMLogo.SetImageResource(Resource.Drawable.CRM_off);
                    else
                        CRMLogo.SetImageResource(Resource.Drawable.CRM_on);      
                break;
                case CrmService.CrmType.Salesforce:
                    CRMLogo.Visibility = ViewStates.Visible;
                    if(exportState == LeadDetailsViewModel.CrmExportStates.NotExported)
                        CRMLogo.SetImageResource(Resource.Drawable.sf_off); 
                    else
                        CRMLogo.SetImageResource(Resource.Drawable.sf_on);
                    break;
                case CrmService.CrmType.Other:
                    CRMLogo.Visibility = ViewStates.Gone;
                break;
            }
        }

        #region photo

        IScheduledWork photoLoadingWork;

        public void SetPhotoResource(FileResource photo)
        {
            if (PhotoView.ScheduledWork != null)
            {
                PhotoView.ScheduledWork.Cancel();
                PhotoView.ScheduledWork = null;
            }
            var localPath = photo?.AbsoluteLocalPath;
            var remoteUrl = photo?.RemoteUrl;
            bool hasPhoto = false;
            if (!string.IsNullOrEmpty(remoteUrl))
            {
                PhotoView.Visibility = ViewStates.Visible;
                AddPhotoButton.Visibility = ViewStates.Gone;
                PhotoView.LoadByUrl(remoteUrl);
                hasPhoto = true;
            }
            else if (!string.IsNullOrEmpty(localPath))
            {
                PhotoView.LoadByPath(localPath);
                hasPhoto = true;
            }
            if (hasPhoto)
            {
                PhotoView.Visibility = ViewStates.Visible;
                AddPhotoButton.Visibility = ViewStates.Gone;
                return;
            }
            PhotoView.Visibility = ViewStates.Gone;
            AddPhotoButton.Visibility = ViewStates.Visible;
        }

        public void SetCardFrontResource(FileResource resource)
        {
            bool haveFrontPhoto = !string.IsNullOrEmpty(resource.RelativeLocalPath) ||
                                         !string.IsNullOrEmpty(resource.RemoteUrl);
            BusinessCardFrontThumb.SetImageResource(haveFrontPhoto ? Resource.Drawable.bcard_on : Resource.Drawable.bcard_off);
        }

        public void SetCardBackResource(FileResource resource)
        {
            bool haveBackPhoto = !string.IsNullOrEmpty(resource.RelativeLocalPath) ||
                                        !string.IsNullOrEmpty(resource.RemoteUrl);
            BusinessCardBackThumb.SetImageResource(haveBackPhoto ? Resource.Drawable.bcard_on : Resource.Drawable.bcard_off);
        }

        #endregion 

        #region Adapters

        public WeakBindingList ChildrenBindingList { get; private set; }

        public ObservableAdapter<string> GetStatesSpinnerAdapter(List<string> states)
        {
            return states.GetSpinnerAdapter(GetStateItemView, GetStatesDropdownView);
        }

        public View GetStateItemView(int pos, string state, View convertView, View parent)
        {
            TextView view;
            if (convertView == null)
            {
                view = (TextView)LayoutInflater.From(Context).Inflate(Resource.Layout.SpinnerItemLayout, null);
            }
            else
            {
                view = (TextView)convertView;
            }
            view.Text = state;
            return view;
        }

        public View GetStatesDropdownView(int pos, string state, View convertView, View parent)
        {
            TextView view;
            if (convertView == null)
            {
                view = (TextView)LayoutInflater.From(Context).Inflate(Resource.Layout.SpinnerItemLayout, null);
            }
            else
            {
                view = (TextView)convertView;
            }
            view.Text = state;
            return view;
        }

        public ObservableAdapter<string> GetCountriesSpinnerAdapter(List<string> countries)
        {
            return new ObservableAdapter<string>
            {
                ViewFactory = GetCountryItemView,
                DropDownViewFactory = GetCountriesDropdownView,
                DataSource = countries
            };
        }

        View GetCountryItemView(int pos, string country, View convertView, View parent)
        {
            TextView view;
            if(convertView == null) 
                view = (TextView) LayoutInflater.From(Context).Inflate(Resource.Layout.SpinnerItemLayout, null);
            else
                view = (TextView)convertView;
            view.Text = country;
            return view;
        }

        View GetCountriesDropdownView(int pos, string country, View convertView, View parent)
        {
            TextView view;
            if (convertView == null)
                view = (TextView)LayoutInflater.From(Context).Inflate(Resource.Layout.SpinnerItemLayout, null);
            else
                view = (TextView)convertView;
            view.Text = country;
            return view;   
        }

        public Action OnEmailChangedAction { get; set; }
        public Action OnPhoneChangedAction { get; set; }

        public ObservableAdapter<LeadDetailsEmailViewModel> EmailsListAdapter(ObservableList<LeadDetailsEmailViewModel> emails)
		{
            return new ObservableAdapter<LeadDetailsEmailViewModel>
            {
                DataSource = emails,
                ViewFactory = GetEmailItemView
            };
		}

		public View GetEmailItemView(int position, LeadDetailsEmailViewModel emailViewModel, View convertView, View parent)
		{
            var view = (EmailView)convertView;
            if (view == null)
            {
                view = new EmailView(parent.Context);
                view.EmailStringChanged = OnEmailChangedAction;
                ChildrenBindingList.Add(view.Bindings);
            }
            view.ViewModel = emailViewModel;
			return view;
		}

        public ObservableAdapter<LeadDetailsPhoneViewModel> PhonesListAdapter(ObservableList<LeadDetailsPhoneViewModel> phones)
		{
            return new ObservableAdapter<LeadDetailsPhoneViewModel>
            {
                DataSource = phones,
                ViewFactory = GetPhoneItemView
            };
		}

        public View GetPhoneItemView(int position, LeadDetailsPhoneViewModel phoneViewModel, View convertView, View parent)
		{
            var view = (PhoneView)convertView;
            if (view == null)
            {
                view = new PhoneView(parent.Context);
                view.PhoneStringChanged = OnPhoneChangedAction;
                ChildrenBindingList.Add(view.Bindings);
            }
            view.ViewModel = phoneViewModel;
            return view;
		}

        ImageButton CreateButtonForList(Context context)
		{
            var button = new ImageButton(context); 

            var size = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 20, context.Resources.DisplayMetrics);
            var pad = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 4, context.Resources.DisplayMetrics);

            button.SetImageResource(Resource.Drawable.leads_plus);
            button.SetBackgroundResource(Android.Resource.Color.Transparent);
			var lp = new ExpandedListView.LayoutParams(size+pad, size+pad*2);
			lp.AlignRight = true;
			button.LayoutParameters = lp;
			button.Clickable = true;

			button.SetPadding(pad, pad, 0, pad);

			return button;
		}

        #endregion

        #region Highlighting

        bool isEventHighlighted;
        public bool IsEventHighlighted
        {
            get
            {
                return isEventHighlighted;
            }
            set
            {
                isEventHighlighted = value;
                if (isEventHighlighted)
                    EventUnderline.SetBackgroundColor(Color.Red);
                else
                    EventUnderline.SetBackgroundColor(new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray)));
            }
        }
        bool isNameHighlighted;
        public bool IsNameHighlighted
        {
            get
            {
                return isNameHighlighted;
            }
            set
            {
                isNameHighlighted = value;
                if (isNameHighlighted)
                    NameUnderline.SetBackgroundColor(Color.Red);
                else
                    NameUnderline.SetBackgroundColor(new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray)));
            }
        }
        bool isSurnameHighlighted;
        public bool IsSurnameHighlighted
        {
            get
            {
                return isSurnameHighlighted;
            }
            set
            {
                isSurnameHighlighted = value;
                if (isSurnameHighlighted)
                    SurnameUnderline.SetBackgroundColor(Color.Red);
                else
                    SurnameUnderline.SetBackgroundColor(new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray)));
            }
        }
        bool isCompanyHighlighted;
        public bool IsCompanyHighlighted
        {
            get
            {
                return isCompanyHighlighted;
            }
            set
            {
                isCompanyHighlighted = value;
                if (isCompanyHighlighted)
                    CompanyUnderline.SetBackgroundColor(Color.Red);
                else
                    CompanyUnderline.SetBackgroundColor(new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray)));
            }
        }
        bool isTitleHighlighted;
        public bool IsTitleHighlighted
        {
            get
            {
                return isTitleHighlighted;
            }
            set
            {
                isTitleHighlighted = value;
                if (isTitleHighlighted)
                    TitleUnderline.SetBackgroundColor(Color.Red);
                else
                    TitleUnderline.SetBackgroundColor(new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray)));
            }
        }
        bool isAddressHighlighted;
        public bool IsAddressHighlighted
        {
            get
            {
                return isAddressHighlighted;
            }
            set
            {
                isAddressHighlighted = value;
                if (isAddressHighlighted)
                    AddressUnderline.SetBackgroundColor(Color.Red);
                else
                    AddressUnderline.SetBackgroundColor(new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray)));
            }
        }
        bool isCityHighlighted;
        public bool IsCityHighlighted
        {
            get
            {
                return isCityHighlighted;
            }
            set
            {
                isCityHighlighted = value;
                if (isCityHighlighted)
                    CityUnderline.SetBackgroundColor(Color.Red);
                else
                    CityUnderline.SetBackgroundColor(new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray)));
            }
        }
        bool isStateHighlighted;
        public bool IsStateHighlighted
        {
            get
            {
                return isStateHighlighted;
            }
            set
            {
                isStateHighlighted = value;
                if (isStateHighlighted)
                    StateUnderline.SetBackgroundColor(Color.Red);
                else
                    StateUnderline.SetBackgroundColor(new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray)));
            }
        }
        bool isZIPHighlighted;
        public bool IsZIPHighlighted
        {
            get
            {
                return isZIPHighlighted;
            }
            set
            {
                isZIPHighlighted = value;
                if (isZIPHighlighted)
                    ZIPUnderline.SetBackgroundColor(Color.Red);
                else
                    ZIPUnderline.SetBackgroundColor(new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray)));
            }
        }
        bool isCountryHighlighted;
        public bool IsCountryHighlighted
        {
            get
            {
                return isCountryHighlighted;
            }
            set
            {
                isCountryHighlighted = value;
                if (isCountryHighlighted)
                    CountryUnderline.SetBackgroundColor(Color.Red);
                else
                    CountryUnderline.SetBackgroundColor(new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray)));
            }
        }
        bool isCompanyURLHighlighted;
        public bool IsCompanyURLHighlighted
        {
            get
            {
                return isCompanyURLHighlighted;
            }
            set
            {
                isCompanyURLHighlighted = value;
                if (isCompanyURLHighlighted)
                    CompanyURLUnderline.SetBackgroundColor(Color.Red);
                else
                    CompanyURLUnderline.SetBackgroundColor(new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray)));
            }
        }
        bool isNotesHighlighted;
        public bool IsNotesHighlighted
        {
            get
            {
                return isNotesHighlighted;
            }
            set
            {
                isNotesHighlighted = value;
                if (isNotesHighlighted)
                    NotesUnderline.SetBackgroundColor(Color.Red);
                else
                    NotesUnderline.SetBackgroundColor(new Color(ContextCompat.GetColor(Context, Resource.Color.primary_gray)));
            }
        }

        #endregion

        bool isStateSpinnerVisible;
        public bool IsStateSpinnerVisible
        {
            get
            {
                return isStateSpinnerVisible;
            }
            set
            {
                isStateSpinnerVisible = value;
                if(isStateSpinnerVisible) 
                {
                    StateSpinner.Visibility = ViewStates.Visible;
                    StateEdit.Visibility = ViewStates.Invisible;
                }
                else
                {
                    StateSpinner.Visibility = ViewStates.Invisible;
                    StateEdit.Visibility = ViewStates.Visible;
                }
            }
        }

        bool isAnimationRunning = false;
        bool isErrorDisplayed = false;

        public void SetEventErrorAnimationEnabled(bool enabled)
        {
            if (enabled && !isAnimationRunning && !isErrorDisplayed)
            {
                ((ScrollView)GetChildAt(0)).ScrollTo(0, 0);
                isAnimationRunning = true;
                EventErrorView.Visibility = ViewStates.Visible;
                EventUnderline.Visibility = ViewStates.Visible;

                var errorColorAnimator = ValueAnimator.OfObject(new ArgbEvaluator(), Color.White.ToArgb(), Color.Red.ToArgb());
                errorColorAnimator.SetDuration(500);
                errorColorAnimator.Update += (sender, e) =>
                {
                    var color = new Color((int)e.Animation.AnimatedValue);
                    EventErrorView.SetTextColor(color);
                };

                errorColorAnimator.AnimationEnd += (sender, e) =>
                {
                    isAnimationRunning = false;
                    isErrorDisplayed = true;
                };

                var lineColorAnimator = ValueAnimator.OfObject(new ArgbEvaluator(), Color.Black.ToArgb(), Color.Red.ToArgb());
                lineColorAnimator.SetDuration(500);
                lineColorAnimator.Update += (sender, e) =>
                {
                    var color = new Color((int)e.Animation.AnimatedValue);
                    EventUnderline.SetBackgroundColor(color);
                };

                errorColorAnimator.Start();
                lineColorAnimator.Start();
            }
            else if (!isAnimationRunning && isErrorDisplayed)
            {
                isAnimationRunning = true;
                var errorColorAnimator = ValueAnimator.OfObject(new ArgbEvaluator(), Color.Red.ToArgb(), Color.White.ToArgb());
                errorColorAnimator.SetDuration(500);
                errorColorAnimator.Update += (sender, e) =>
                {
                    var color = new Color((int)e.Animation.AnimatedValue);
                    EventErrorView.SetTextColor(color);
                };

                errorColorAnimator.AnimationEnd += (sender, e) =>
                {
                    EventErrorView.Visibility = ViewStates.Invisible;
                    isAnimationRunning = false;
                    isErrorDisplayed = false;
                };

                var lineColorAnimator = ValueAnimator.OfObject(new ArgbEvaluator(), Color.Red.ToArgb(), Color.Black.ToArgb());
                lineColorAnimator.SetDuration(500);
                lineColorAnimator.Update += (sender, e) =>
                {
                    var color = new Color((int)e.Animation.AnimatedValue);
                    EventUnderline.SetBackgroundColor(color);
                };
                lineColorAnimator.AnimationEnd += (sender, e) =>
                {
                    EventUnderline.Visibility = ViewStates.Invisible;
                };
                errorColorAnimator.Start();
                lineColorAnimator.Start();
            }
            else if (!enabled)
            {
                EventErrorView.Visibility = ViewStates.Invisible;
                EventUnderline.Visibility = ViewStates.Invisible;
            }
        }
	}
}

