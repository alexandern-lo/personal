using System;
using System.Collections.Generic;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Content
{
    public class LeadQualifyView : CustomView, IUIScrollViewDelegate, IUITextViewDelegate
    {
        [View(0)]
        [CommonSkin("QualifyCurrentQuestionBackground")]
        public UIView CurrentQuestionBackground { get; private set; }

        [View(6)]
        [ButtonSkin("NotesButton")]
        public UIButton NotesButton { get; private set; }

        [View(7)]
        public UIScrollView QuestionsScrollView { get; private set; }

        [View(8)]
        public QualifyStatesScrollView StatesScrollView { get; private set; } = new QualifyStatesScrollView();

        public event EventHandler CurrentQuestionChanged;
        int currentQuestion;
        public int CurrentQuestion
        {
            get { return currentQuestion; }
            set
            {
                if (value != currentQuestion)
                {
                    currentQuestion = value;
                    if (CurrentQuestionChanged != null)
                    {
                        CurrentQuestionChanged(this, EventArgs.Empty);
                    }
                    int buttonIndex = 0;
                    foreach (UIButton questionNumberButton in QuestionNumberButtons)
                    {
                        if (buttonIndex == value) QuestionNumberButtons[buttonIndex].Font = Fonts.xLargeBold;
                        else QuestionNumberButtons[buttonIndex].Font = Fonts.xLargeRegular;
                        buttonIndex++;
                    }
                    RefreshQuestionsOfset();
                }
            }
        }
        public event EventHandler CurrentStateChanged;
        int currentState;
        public int CurrentState
        {
            get { return currentState; }
            set
            {
                if (value != currentState)
                {
                    currentState = value;
                    if (CurrentStateChanged != null)
                    {
                        CurrentStateChanged(this, EventArgs.Empty);
                    }
                    RefreshStatesOfsetAndShadowColor();
                }
            }
        }

        private bool alreadyLayouted = false;

        [CommonSkin("QualifyNotesTextView")]
        public UITextView NotesTextView { get; private set; } = new UITextView();
        public UIView ContainerForNotes { get; private set; } = new UIView();

        public List<LeadQualifyQuestionView> QuestionViews { get; private set; } = new List<LeadQualifyQuestionView>();
        public List<UIButton> QuestionNumberButtons { get; private set; } = new List<UIButton>();
        private int questionViewsCount;

        public int QuestionViewsCount
        {
            get
            {
                return questionViewsCount;
            }
            set
            {
                questionViewsCount = value;
                for (int i = 0; i < questionViewsCount; i++)
                {
                    var questionView = new LeadQualifyQuestionView();
                    QuestionViews.Add(questionView);
                    QuestionsScrollView.AddSubview(questionView);

                    var questionNumberButton = new UIButton();
                    questionNumberButton.Tag = i;
                    ButtonSkin.QuestionNumberButton(questionNumberButton);
                    questionNumberButton.SetTitle(i + 1 + "", UIControlState.Normal);
                    QuestionNumberButtons.Add(questionNumberButton);
                    AddSubview(questionNumberButton);

                    NotesButton.Tag = value;
                }
            }
        }
        KeyboardScroller scroller;

        protected override void CreateView()
        {
            base.CreateView();
            BackgroundColor = new UIColor(0.95f, 0.95f, 0.95f, 1f);

            QuestionsScrollView.Delegate = this;
            QuestionsScrollView.BackgroundColor = new UIColor(0.95f, 0.95f, 0.95f, 1f);
            QuestionsScrollView.PagingEnabled = true;
            QuestionsScrollView.ShowsHorizontalScrollIndicator = false;

            StatesScrollView.Delegate = this;
            scroller = new KeyboardScroller()
            {
                ScrollView = NotesTextView
            };
            ContainerForNotes.AddSubview(NotesTextView);
            QuestionsScrollView.AddSubview(ContainerForNotes);
            Layer.MasksToBounds = true;
            ClipsToBounds = true;
            NotesTextView.Delegate = this;
            RefreshStatesOfsetAndShadowColor();
        }

        public void RefreshQuestionsOfset()
        {
            if (alreadyLayouted)
            {
                QuestionsScrollView.SetContentOffset(new CGPoint(QuestionsScrollView.Bounds.Width * CurrentQuestion, 0), false);
                RefreshCurrentQuestionBackgroundPosition();
            }
        }

        public void RefreshStatesOfsetAndShadowColor()
        {
            switch (CurrentState)
            {
                case 0:
                    StatesScrollView.StateViewCold.SetSelected(true);
                    StatesScrollView.StateViewWarm.SetSelected(false);
                    StatesScrollView.StateViewHot.SetSelected(false);
                    break;
                case 1:
                    StatesScrollView.StateViewCold.SetSelected(false);
                    StatesScrollView.StateViewWarm.SetSelected(true);
                    StatesScrollView.StateViewHot.SetSelected(false);
                    break;
                case 2:
                    StatesScrollView.StateViewCold.SetSelected(false);
                    StatesScrollView.StateViewWarm.SetSelected(false);
                    StatesScrollView.StateViewHot.SetSelected(true);
                    break;
            }
            if (alreadyLayouted)
            {
                StatesScrollView.SetContentOffset(new CGPoint(StatesScrollView.Bounds.Width * CurrentState, 0), false);
            }
        }

        public void RefreshCurrentQuestionBackgroundPosition()
        {
            var pagePosition = QuestionsScrollView.ContentOffset.X / QuestionsScrollView.Bounds.Width;
            var view = CurrentQuestionBackground;
            var leftMargin = 25;
            var marginBetweenNumberButtons = questionViewsCount > 0 ? 50 / questionViewsCount : 0;

            CGRect frame = view.Frame;
            frame.X = leftMargin + pagePosition * view.Frame.Width + pagePosition * marginBetweenNumberButtons;
            view.Frame = frame;
        }

        [Export("scrollViewDidScroll:")]
        public void Scrolled(UIScrollView scrollView)
        {
            if (scrollView == QuestionsScrollView)
            {
                RefreshCurrentQuestionBackgroundPosition();
                NotesTextView.ResignFirstResponder();

                if ((scrollView.ContentOffset.X / scrollView.Bounds.Width) % 1 != 0) return;
                int questionNumber = (int)(scrollView.ContentOffset.X / scrollView.Bounds.Width);
                if (CurrentQuestion != questionNumber)
                    CurrentQuestion = questionNumber;
            }
            else if (scrollView == StatesScrollView)
            {
                double meanPage = scrollView.ContentOffset.X / scrollView.Bounds.Width;
                if (meanPage % 1 > 0.05 && meanPage % 1 < 0.95) return;
                var stateNumber = (int)Math.Round(scrollView.ContentOffset.X / scrollView.Bounds.Width);
                CurrentState = stateNumber;
            }

        }

        [Export("textViewDidEndEditing:")]
        public void EditingEnded(UITextView textView)
        {
            textView.Text = textView.Text.Trim();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            nfloat questionNumberButtonWidth;
            int marginBetweenNumberButtons;
            if (questionViewsCount > 0)
            {
                questionNumberButtonWidth = (Frame.Width - 100) / (questionViewsCount + 1);
                marginBetweenNumberButtons = 50 / questionViewsCount;
            }
            else
            {
                questionNumberButtonWidth = (Frame.Width - 100) / 5;
                marginBetweenNumberButtons = 0;
            }
            int questionNumberIndex = 0;
            foreach (UIButton questionNumberButton in QuestionNumberButtons)
            {
                if (questionNumberIndex == 0)
                {
                    questionNumberButton.Frame = this.LayoutBox()
                        .Top(13)
                        .Left(25)
                        .Width(questionNumberButtonWidth)
                        .Height(45);
                }
                else {
                    questionNumberButton.Frame = this.LayoutBox()
                        .Top(13)
                        .After(QuestionNumberButtons[questionNumberIndex - 1], marginBetweenNumberButtons)
                        .Width(questionNumberButtonWidth)
                        .Height(45);
                }
                questionNumberIndex++;
            }
            if (questionViewsCount == 0)
            {
                NotesButton.Frame = this.LayoutBox()
                    .Top(13)
                    .Left(25)
                    .Width(questionNumberButtonWidth)
                    .Height(45);
            }
            else {
                NotesButton.Frame = this.LayoutBox()
                    .Top(13)
                    .After(QuestionNumberButtons[QuestionNumberButtons.Count - 1], marginBetweenNumberButtons)
                    .Width(questionNumberButtonWidth)
                    .Height(45);
            }
            CurrentQuestionBackground.Frame = this.LayoutBox()
                .Top(13)
                .Left(CurrentQuestionBackground.Frame.X)
                .Width(questionNumberButtonWidth)
                .Height(45);

            StatesScrollView.Frame = this.LayoutBox()
                .Bottom(0)
                .Height(90)
                .Width(Bounds.Width * 0.88f)
                .CenterHorizontally(-7);

            QuestionsScrollView.Frame = this.LayoutBox()
                .Below(NotesButton, 0)
                .Above(StatesScrollView, 0)
                .Left(0)
                .Right(0);

            int questionViewsIndex = 0;
            foreach (LeadQualifyQuestionView questionView in QuestionViews)
            {
                questionView.Frame = new CGRect(QuestionsScrollView.Bounds.Width * questionViewsIndex,
                                                0,
                                                QuestionsScrollView.Bounds.Width,
                                                QuestionsScrollView.Bounds.Height);
                questionViewsIndex++;
            }

            ContainerForNotes.Frame = new CGRect(QuestionsScrollView.Bounds.Width * questionViewsIndex,
                                                0,
                                                QuestionsScrollView.Bounds.Width,
                                                QuestionsScrollView.Bounds.Height);
            NotesTextView.Frame = ContainerForNotes.LayoutBox()
                .Left(15)
                .Top(0)
                .Right(15)
                .Bottom(0);
            NotesTextView.ContentSize = new CGSize(NotesTextView.ContentSize.Width, NotesTextView.ContentSize.Height > NotesTextView.Bounds.Height ? 
                                                   NotesTextView.ContentSize.Height : NotesTextView.Bounds.Height + 1);
            QuestionsScrollView.ContentSize = new CGSize(ContainerForNotes.Frame.GetMaxX(),
                                                         QuestionsScrollView.Bounds.Height);

            QuestionsScrollView.SetContentOffset(new CGPoint(QuestionsScrollView.Bounds.Width * CurrentQuestion, 0), false);
            StatesScrollView.SetContentOffset(new CGPoint(StatesScrollView.Bounds.Width * CurrentState, 0), false);
            RefreshCurrentQuestionBackgroundPosition();
            alreadyLayouted = true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                scroller.Dispose();
            }
        }
    }

    public static class LeadQualifyViewProperties
    {
        public static readonly RuntimeEvent CurrentQuestionChangedEvent = new RuntimeEvent(typeof(LeadQualifyView), "CurrentQuestionChanged");
        public static readonly IPropertyBindingStrategy CurrentQuestionChangedBinding = new EventHandlerBindingStrategy(CurrentQuestionChangedEvent);

        public static readonly RuntimeEvent CurrentStateChangedEvent = new RuntimeEvent(typeof(LeadQualifyView), "CurrentStateChanged");
        public static readonly IPropertyBindingStrategy CurrentStateChangedBinding = new EventHandlerBindingStrategy(CurrentStateChangedEvent);

        public static IProperty<int> CurrentQuestionProperty(this LeadQualifyView leadQualifyView)
        {
            return leadQualifyView.GetProperty(_ => _.CurrentQuestion, CurrentQuestionChangedBinding);
        }

        public static IProperty<int> CurrentStateProperty(this LeadQualifyView leadQualifyView)
        {
            return leadQualifyView.GetProperty(_ => _.CurrentState, CurrentStateChangedBinding);
        }

        [Preserve]
        static void LinkerTrick()
        {
            new LeadQualifyView().CurrentQuestionChanged += (o, a) => { };
            new LeadQualifyView().CurrentStateChanged += (o, a) => { };
        }
    }
}

