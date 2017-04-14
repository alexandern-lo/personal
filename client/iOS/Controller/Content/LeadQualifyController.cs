using Foundation;
using LiveOakApp.iOS.View;
using LiveOakApp.iOS.View.Content;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.ViewModels;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.Controller.Content
{
    public class LeadQualifyController : CustomController<LeadQualifyView>
    {
        LeadDetailsViewModel ViewModel { get; set; }

        public LeadQualifyController(LeadDetailsViewModel viewModel)
        {
            ViewModel = viewModel;

            AnswerClickCommand = new Command
            {
                Action = AnswerClickAction
            };
            QuestionNumberClickCommand = new Command
            {
                Action = QuestionNumberClickAction
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.QuestionViewsCount = ViewModel.Questions.Count;
            View.NotesTextView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.Interactive;

            int viewIndex = 0;
            foreach (LeadDetailsQuestionViewModel question in ViewModel.Questions)
            {
                var questionView = View.QuestionViews[viewIndex];
                questionView.Question = question;
                var tableBinding = questionView.GetTableBinding(question.Answers);
                Bindings.Add(tableBinding);
                Bindings.Property(question, _ => _.Title).To(questionView.QuestionTextView.TextProperty());
                Bindings.Command(question.SaveAnswer).To(tableBinding.ItemSelectedTarget());
                Bindings.Command(AnswerClickCommand).To(tableBinding.ItemSelectedTarget());
                Bindings.Property(question, _ => _.CheckedAnswers).UpdateTarget((source) =>
                {
                    if (source.Value.IsNullOrEmpty()) return;
                    foreach (LeadDetailsAnswerViewModel answer in source.Value)
                    {
                        NSIndexPath indexPath = tableBinding.FirstIndexPathForItem(answer);
                        if (indexPath == null) return;
                        questionView.QuestionTableView.SelectRow(indexPath, false, UITableViewScrollPosition.None);
                    }
                });
                View.QuestionsScrollView.AddSubview(questionView);

                if (!question.CheckedAnswers.IsNullOrEmpty())
                {
                    View.QuestionNumberButtons[viewIndex].SetTitleColor(Colors.LightGray, UIControlState.Normal);
                }
                else {
                    View.QuestionNumberButtons[viewIndex].SetTitleColor(Colors.DarkGray, UIControlState.Normal);
                }
                Bindings.Command(QuestionNumberClickCommand).To(View.QuestionNumberButtons[viewIndex].ClickTarget());
                viewIndex++;
            }
            Bindings.Command(QuestionNumberClickCommand).To(View.NotesButton.ClickTarget());
            Bindings.Property(ViewModel, _ => _.LeadNotes).To(View.NotesTextView.TextProperty());
            Bindings.Property(ViewModel, _ => _.CurrentQuestion).To(View.CurrentQuestionProperty());
            Bindings.Property(ViewModel, _ => _.CurrentState).To(View.CurrentStateProperty());
        }

        Command AnswerClickCommand { get; set; }
        void AnswerClickAction(object obj)
        {
            if (ViewModel.Questions[ViewModel.CurrentQuestion].CheckedAnswers.IsNullOrEmpty())
                View.QuestionNumberButtons[ViewModel.CurrentQuestion].SetTitleColor(Colors.DarkGray, UIControlState.Normal);
            else
                View.QuestionNumberButtons[ViewModel.CurrentQuestion].SetTitleColor(Colors.LightGray, UIControlState.Normal);

        }

        Command QuestionNumberClickCommand { get; set; }
        void QuestionNumberClickAction(object obj)
        {
            var questionButton = (UIButton)obj;
            ViewModel.CurrentQuestion = (int)questionButton.Tag;
        }
    }
}
