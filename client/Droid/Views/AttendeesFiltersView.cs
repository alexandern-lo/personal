using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Kyleduo.Switchbutton;
using LiveOakApp.Droid.CustomViews.Adapters;
using LiveOakApp.Models.ViewModels;
using SE.Emilsjolander.Stickylistheaders;
using StudioMobile;
using Android.Graphics;
using Android.Support.V4.Content;

namespace LiveOakApp.Droid.Views
{
	public class AttendeesFiltersView : FrameLayout
	{
		public AttendeesFiltersView(Context context) :
			base(context)
		{
			Initialize();
		}

		public AttendeesFiltersView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public AttendeesFiltersView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

		public StickyListHeadersListView FiltersList { get; private set; }
        public Button ResetButton { get; private set; }

        public EventListSource<SwitchButton> SwitchListSource { get; private set; }

		void Initialize()
		{
			Inflate(Context, Resource.Layout.AttendeesFiltersLayout, this);

			FiltersList = FindViewById<StickyListHeadersListView>(Resource.Id.categoriesList);
            ResetButton = FindViewById<Button>(Resource.Id.resetButton);

            ResetButton.Click += (sender, e) => FiltersList.InvalidateViews();

			SwitchListSource = new EventListSource<SwitchButton>(ViewBindings.ClickEvent)
			{
				ParameterExtractor = (sender, args) => FiltersList.GetPositionForView((View) sender)
			};
		}

		public StickyHeadersListViewAdapter<AttendeeFiltersViewModel.Section, AttendeeFiltersViewModel.OptionToggleViewModel> GetSectionsAdapter(ObservableList<AttendeeFiltersViewModel.Section> sections)
		{
			return sections.GetStickyHeadersAdapter<AttendeeFiltersViewModel.Section, AttendeeFiltersViewModel.OptionToggleViewModel>(GetFilterItemView, GetSectionHeaderView);
		}

		public View GetFilterItemView(int position, AttendeeFiltersViewModel.Section section, AttendeeFiltersViewModel.OptionToggleViewModel data, View convertView, View parent)
		{
			FilterItemView view;
			if (convertView == null)
			{
				view = new FilterItemView(parent.Context);
				SwitchListSource.Listen(view.Switch, view);
			}
			else
			{
				view = (FilterItemView)convertView;
			}

			view.OptionName.Text = data.OptionName;

            view.Switch.SetCheckedImmediately(data.IsSelected);

			return view;
		}

		public static View GetSectionHeaderView(int position, AttendeeFiltersViewModel.Section section, View convertView, View parent)
		{
			TextView view;

			if (convertView == null)
			{
				view = new TextView(parent.Context);

				var padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 8, parent.Context.Resources.DisplayMetrics);

				view.SetPadding(padding, padding, padding, padding);

				view.SetTextSize(ComplexUnitType.Sp, 20);

                var dark_color = new Color(ContextCompat.GetColor(parent.Context, Resource.Color.primary_dark));
				view.SetTextColor(dark_color);

                view.SetBackgroundResource(Resource.Color.primary_light);

				view.Clickable = true; // not clickable
			}
			else
			{
				view = (TextView)convertView;
			}

			view.Text = section.Header;

			return view;
		}
	}
}

