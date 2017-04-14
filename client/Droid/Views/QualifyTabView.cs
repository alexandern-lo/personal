using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using StudioMobile;
using LiveOakApp.Models.ViewModels;
using Android.Support.V4.View;
using Android.Graphics;
using LiveOakApp.Resources;
using System;
using LiveOakApp.Models.Services;
using LiveOakApp.Models;
using Android.Support.V4.Content;
using Android.Text;

namespace LiveOakApp.Droid.Views
{
    public class QualifyTabView : CustomBindingsView, ViewPager.IOnPageChangeListener
    {
        public QualifyTabView(Context context) :
            base(context)
        {
            Initialize();
        }

        public QualifyTabView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public QualifyTabView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        public LinearLayout TitlesRow { get; private set; }
        public ViewPager QuestionsPager { get; private set; }
        public ViewPager StatePager { get; private set; }

        public EditText NotesView { get; private set; }

        public View TitlesRowHolder { get; private set; }

        private View FloatingView { get; set; }

        private FontService fontService;

        private bool isAnimationSet;

        #region QuestionsCount property

        private int questionsCount;
        public int QuestionsCount
        {
            get
            {
                return questionsCount;
            }
            set
            {
                questionsCount = value;

                SetupTitlesRow();

                if (!isAnimationSet)
                    SetupFloatingView();

                if (QuestionsCountChange != null)
                {
                    QuestionsCountChange(this, new QuestionsCountChangeEventArgs
                    {
                        QuestionsCount = value
                    });
                }
            }
        }

        public event EventHandler<QuestionsCountChangeEventArgs> QuestionsCountChange;

        public class QuestionsCountChangeEventArgs : EventArgs
        {
            public int QuestionsCount { get; set; }
        }

        public static readonly RuntimeEvent QuestionsCountChangeEvent = new RuntimeEvent(typeof(QualifyTabView), "QuestionsCountChange");
        public static readonly IPropertyBindingStrategy QuestionsCountChangeBinding = new EventHandlerBindingStrategy<QuestionsCountChangeEventArgs>(QuestionsCountChangeEvent);

        public IProperty<int> QuestionsCountProperty()
        {
            return this.GetProperty(_ => _.QuestionsCount, QuestionsCountChangeBinding);
        }

        #endregion

        private void SetupTitlesRow()
        {
            TitlesRow.RemoveAllViews();

            var lp = GetLayoutParamsForTitlesRow();

            for (var i = 0; i < questionsCount; i++)
            {
                var textView = new TextView(Context);

                textView.Text = (i+1).ToString();

                textView.Gravity = GravityFlags.Center;

                textView.SetTextColor(Color.Black);
                textView.SetTextSize(ComplexUnitType.Sp, 16);
                textView.SetTypeface(fontService.GetFont(Context, FontService.OpenSansRegular), TypefaceStyle.Normal);

                var index = i;

                textView.Click += (sender, e) =>
                {
                    QuestionsPager.SetCurrentItem(index, true);
                };

                //textView.LongClick += (sender, e) =>
                //{
                //        // TODO: Show question popup;
                //};

                TitlesRow.AddView(textView, lp);
            }
            Bindings.UpdateTarget();

            var imageView = new ImageView(Context);

            var imagePadding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 8, Context.Resources.DisplayMetrics);
            imageView.SetPadding(imagePadding, imagePadding, imagePadding, imagePadding);

            imageView.SetImageResource(Resource.Drawable.lead_notes);

            var blueColor = new Color(ContextCompat.GetColor(Context, Resource.Color.primary_blue));

            imageView.SetColorFilter(blueColor);

            TitlesRow.AddView(imageView, lp);

            imageView.Click += (sender, e) =>
            {
                QuestionsPager.SetCurrentItem(QuestionsCount, true);
            };

            // initial 'bolding'
            var firstTextView = TitlesRow.GetChildAt(0) as TextView;
            if (firstTextView != null)
            {
                firstTextView.SetTypeface(fontService.GetFont(Context, FontService.OpenSansBold), TypefaceStyle.Normal);
            }
        }

        private void SetupFloatingView()
        {
            QuestionsPager.PageMargin = QuestionsPager.PaddingLeft;
            QuestionsPager.SetClipToPadding(false);

            var titleItemWidth =
               (Context.Resources.DisplayMetrics.WidthPixels - (TitlesRow.PaddingLeft + TitlesRow.PaddingRight)) / TitlesRow.ChildCount;

            var floatingViewPadding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 6.0f, Context.Resources.DisplayMetrics);

            FloatingView.LayoutParameters.Width = titleItemWidth - floatingViewPadding * 2;

            FloatingView.ViewTreeObserver.GlobalLayout += (sender, e) =>
            {
                FloatingView.SetX(TitlesRow.GetChildAt(QuestionsPager.CurrentItem).GetX() + floatingViewPadding);
                FloatingView.SetY(TitlesRow.GetY());
            };

            QuestionsPager.PageScrolled += (sender, e) =>
            {
                var pos = e.Position;
                if (TitlesRow.ChildCount > 1 && pos < TitlesRow.ChildCount-1)
                {
                    var offset = e.PositionOffset;

                    var X0 = TitlesRow.GetChildAt(pos).GetX() + floatingViewPadding;
                    var X1 = TitlesRow.GetChildAt(pos + 1).GetX() + floatingViewPadding;

                    if (X0 >= TitlesRow.GetX() + TitlesRow.PaddingLeft)
                    {
                        FloatingView.SetX(X0 + (X1 - X0) * offset);
                    }
                }
            };

            QuestionsPager.PageSelected += (sender, e) =>
            {
                var pos = e.Position;

                FloatingView.SetX(TitlesRow.GetChildAt(pos).GetX() + floatingViewPadding);

                for (var i = 0; i < TitlesRow.ChildCount; i++)
                {
                    var textView = TitlesRow.GetChildAt(i) as TextView;

                    if (textView != null)
                    {
                        textView.SetTypeface(fontService.GetFont(Context, FontService.OpenSansRegular), TypefaceStyle.Normal);
                    }
                }
                var currentTextView = TitlesRow.GetChildAt(pos) as TextView;

                if (currentTextView != null)
                {
                    currentTextView.SetTypeface(fontService.GetFont(Context, FontService.OpenSansBold), TypefaceStyle.Normal);
                }
                UiUtil.hideKeyboard(NotesView);
            };

            isAnimationSet = true;
        }

        private void SetupNotesView()
        {
            NotesView = new EditText(Context);
            NotesView.SetBackgroundResource(Resource.Drawable.round_white);
            NotesView.SetTextColor(Color.Black);
            NotesView.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(180) });
            NotesView.Gravity = GravityFlags.Left | GravityFlags.Top;
            NotesView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        }

        private void SetupStatePager()
        {
            StatePager.OffscreenPageLimit = 3;
            var pageInterval = StatePager.PaddingLeft;
            StatePager.PageMargin = pageInterval / 2;
            StatePager.SetClipToPadding(false);
            StatePager.AddOnPageChangeListener(this);
        }

        private ViewGroup.LayoutParams GetLayoutParamsForTitlesRow()
        {
            var titleItemHeight = TypedValue.ApplyDimension(ComplexUnitType.Dip, 50.0f, Context.Resources.DisplayMetrics);
            var lp = new LinearLayout.LayoutParams(0, (int)titleItemHeight);
            lp.Weight = 1;
            return lp;
        }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.QualifyTabLayout, this);

            fontService = ServiceLocator.Instance.FontService;

            FloatingView = FindViewById(Resource.Id.floating_view);

            TitlesRow = FindViewById<LinearLayout>(Resource.Id.titles_row);
            QuestionsPager = FindViewById<ViewPager>(Resource.Id.questionnaire_pager);
            StatePager = FindViewById<ViewPager>(Resource.Id.state_pager);

            TitlesRowHolder = FindViewById(Resource.Id.titles_row_holder);

            SetupStatePager();

            SetupNotesView();
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            //do nothing
        }

        public void OnPageScrollStateChanged(int state)
        {
            if (state == ViewPager.ScrollStateIdle)
            {
                StatePager.Adapter.NotifyDataSetChanged();
            }
        }

        public void OnPageSelected(int position)
        {
            //do nothing
        }

        public CustomPagerAdapter<LeadDetailsQuestionViewModel> GetQuestionsAdapter(ObservableList<LeadDetailsQuestionViewModel> questions)
        {
            var adapter = questions.GetPagerAdapter((position) =>
            {
                var questionVM = questions[position];

                var view = new QuestionView(Context);

                view.ViewModel = questionVM;

                return view;
            });

            adapter.RightView = NotesView;

            for (var i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                var index = i;

                Bindings.Property(question, _ => _.CheckedAnswers)
                        .UpdateTarget((arg) => 
                { 
                    var textView = TitlesRow.GetChildAt(index) as TextView;
                    if(textView != null)
                        textView.SetTextColor(arg.Value.Count > 0 ? Color.Gray : Color.Black);
                });
            }

            return adapter;
        }

        public CustomPagerAdapter<LeadDetailsViewModel.LeadStates> GetStatesAdapter(ObservableList<LeadDetailsViewModel.LeadStates> states)
        {
            return states.GetPagerAdapter((position) =>
            {
                var state = states[position];

                var view = new QualifyStateView(Context);
                int selectedBackgroundResource = 0;
                int normalBackgroundResource = Resource.Drawable.qualify_state_unselect;

                switch (state)
                {
                    case LeadDetailsViewModel.LeadStates.Cold: 
                        view.StateTitle.Text = L10n.Localize("QualifyStateCold", "Cold"); 
                        view.StateIcon.SetImageResource(Resource.Drawable.lead_cold);
                        selectedBackgroundResource = Resource.Drawable.qualify_state_cold;
                    break;
                    case LeadDetailsViewModel.LeadStates.Warm: 
                        view.StateTitle.Text = L10n.Localize("QualifyStateWarm", "Warm"); 
                        view.StateIcon.SetImageResource(Resource.Drawable.lead_warm);
                        selectedBackgroundResource = Resource.Drawable.qualify_state_warm;
                    break;
                    case LeadDetailsViewModel.LeadStates.Hot: 
                        view.StateTitle.Text = L10n.Localize("QualifyStateHot", "Hot"); 
                        view.StateIcon.SetImageResource(Resource.Drawable.lead_hot);
                        selectedBackgroundResource = Resource.Drawable.qualify_state_hot;
                    break;
                }

                if (position == StatePager.CurrentItem)
                {
                    view.SetBackgroundResource(selectedBackgroundResource);
                }
                else {
                    view.SetBackgroundResource(normalBackgroundResource);
                }
                return view;
            });
        }
    }
}

