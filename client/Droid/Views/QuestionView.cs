using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System;
using StudioMobile;
using LiveOakApp.Droid.CustomViews;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Models;
using LiveOakApp.Models.Services;

namespace LiveOakApp.Droid.Views
{
    public class QuestionView : CustomBindingsView
    {
        public QuestionView(Context context) :
            base(context)
        {
            Initialize();
        }

        public QuestionView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public QuestionView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        LeadDetailsQuestionViewModel viewModel;

        public LeadDetailsQuestionViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
            set
            {
                Bindings.Clear();

                viewModel = value;

                Bindings.Property(viewModel, _ => _.Title)
                        .To(QuestionTitle.TextProperty());

                Bindings.Adapter(AnswersList, GetAnswersAdapter(viewModel.Answers));

                Bindings.Property(viewModel, _ => _.CheckedAnswers)
                        .To(HighlightedItemsProperty());
            }
        }

        public CustomTextView QuestionTitle { get; private set; }
        public ListView AnswersList { get; private set; }

        #region HighlightedItemPosition property

        ObservableList<int> highlightedItemsPosition = new ObservableList<int>();
        public ObservableList<int> HighlightedItemsPosition
        {
            get
            {
                return highlightedItemsPosition;
            }
            set
            {
                highlightedItemsPosition = value;
                if (HighlightedItemsPositionChange != null)
                {
                    HighlightedItemsPositionChange(this, new HighlightedItemsPositionChangeEventArgs
                    {
                        HighlightedItemsPosition = highlightedItemsPosition
                    });
                }
            }
        }

        public event EventHandler<HighlightedItemsPositionChangeEventArgs> HighlightedItemsPositionChange;

        public class HighlightedItemsPositionChangeEventArgs : EventArgs
        {
            public ObservableList<int> HighlightedItemsPosition { get; set; }
        }

        public static readonly RuntimeEvent HighlightedItemsPositionChangeEvent = new RuntimeEvent(typeof(QuestionView), "HighlightedItemsPositionChange");
        public static readonly IPropertyBindingStrategy HighlightedItemsPositionChangeBinding = new EventHandlerBindingStrategy<HighlightedItemsPositionChangeEventArgs>(HighlightedItemsPositionChangeEvent);

        public IProperty<ObservableList<int>> HighlightedItemsPositionProperty()
        {
            return this.GetProperty(_ => _.HighlightedItemsPosition, HighlightedItemsPositionChangeBinding);
        }

        #endregion

        #region HighlightedItem property

        ObservableList<LeadDetailsAnswerViewModel> highlightedItems = new ObservableList<LeadDetailsAnswerViewModel>();
        public ObservableList<LeadDetailsAnswerViewModel> HighlightedItems
        {
            get
            {
                return highlightedItems;
            }
            set
            {
                highlightedItems = value;

                HighlightedItemsPosition = new ObservableList<int>();
                for (var i = 0; i < highlightedItems.Count; i++)
                {
                    HighlightedItemsPosition.Add((AnswersList.Adapter as IReverseAdapter<LeadDetailsAnswerViewModel>).GetPosition(highlightedItems[i]));
                }

                if (HighlightedItemsChange != null)
                {
                    HighlightedItemsChange(this, new HighlightedItemsChangeEventArgs
                    {
                        HighlightedItems = highlightedItems
                    });
                }
            }
        }

        public event EventHandler<HighlightedItemsChangeEventArgs> HighlightedItemsChange;

        public class HighlightedItemsChangeEventArgs : EventArgs
        {
            public ObservableList<LeadDetailsAnswerViewModel> HighlightedItems { get; set; }
        }

        public static readonly RuntimeEvent HighlightedItemsChangeEvent = new RuntimeEvent(typeof(QuestionView), "HighlightedItemsChange");
        public static readonly IPropertyBindingStrategy HighlightedItemsChangeBinding = new EventHandlerBindingStrategy<HighlightedItemsChangeEventArgs>(HighlightedItemsChangeEvent);

        public IProperty<ObservableList<LeadDetailsAnswerViewModel>> HighlightedItemsProperty()
        {
            return this.GetProperty(_ => _.HighlightedItems, HighlightedItemsChangeBinding);
        }

        #endregion

        void Initialize()
        {
            Inflate(Context, Resource.Layout.QuestionLayout, this);

            QuestionTitle = FindViewById<CustomTextView>(Resource.Id.question_title);
            AnswersList = FindViewById<ListView>(Resource.Id.answers_list);

            AnswersList.ItemClick += (sender, e) => 
            {
                var listview = e.Parent as ListView;
                var clickedPos = e.Position;
                var clickedView = e.View;

                var fontService = ServiceLocator.Instance.FontService;

                if (!HighlightedItemsPosition.Contains(clickedPos))
                    HighlightedItemsPosition.Add(clickedPos);
                else
                    HighlightedItemsPosition.Remove(clickedPos);
                
                var item = listview.Adapter.GetItem(clickedPos).Cast<LeadDetailsAnswerViewModel>();
                if (!HighlightedItems.Contains(item))
                    HighlightedItems.Add(item);
                else
                    HighlightedItems.Remove(item);
                if (HighlightedItemsChange != null)
                {
                    HighlightedItemsChange(this, new HighlightedItemsChangeEventArgs
                    {
                        HighlightedItems = highlightedItems
                    });
                }
                if (HighlightedItemsPositionChange != null)
                {
                    HighlightedItemsPositionChange(this, new HighlightedItemsPositionChangeEventArgs
                    {
                        HighlightedItemsPosition = highlightedItemsPosition
                    });
                }
                (listview.Adapter as BaseAdapter).NotifyDataSetChanged();
            };
        }

        ObservableAdapter<LeadDetailsAnswerViewModel> GetAnswersAdapter(ObservableList<LeadDetailsAnswerViewModel> answers)
        {
            return answers.GetAdapter(GetAnswerItemView);
        }

        View GetAnswerItemView(int position, LeadDetailsAnswerViewModel answer, View convertView, View parent)
        {
            var view = convertView == null ? new QualifyAnswerView(parent.Context) : (QualifyAnswerView)convertView;

            if (!HighlightedItemsPosition.Contains(position))
                SetupRegularView(view, ServiceLocator.Instance.FontService);
            else
                SetupSelectedView(view, ServiceLocator.Instance.FontService);
            
            view.AnswerTitle.Text = answer.Answer;

            return view;
        }

        void SetupRegularView(View view, FontService fontService)
        {
            view.SetBackgroundResource(Android.Resource.Color.White);
            var title = view.FindViewById<CustomTextView>(Resource.Id.answer_title);
            title.SetTextColor(Color.Black);

            var regularFont = fontService.GetFont(Context, FontService.OpenSansRegular);

            title.SetTypeface(regularFont, TypefaceStyle.Normal);
        }

        void SetupSelectedView(View view, FontService fontService)
        {
            view.SetBackgroundResource(Resource.Color.primary_blue);
            var answerTitle = view.FindViewById<CustomTextView>(Resource.Id.answer_title);

            var boldFont = fontService.GetFont(Context, FontService.OpenSansBold);
            answerTitle.SetTextColor(Color.White);
            answerTitle.SetTypeface(boldFont, TypefaceStyle.Normal);
        }
    }
}

