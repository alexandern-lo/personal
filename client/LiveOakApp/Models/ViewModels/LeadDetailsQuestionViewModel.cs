using System.Linq;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;
using System.Collections.Generic;
using System.Collections;

namespace LiveOakApp.Models.ViewModels
{
    public class LeadDetailsQuestionViewModel : DataContext
    {
        Field<ObservableList<LeadDetailsAnswerViewModel>> _checkedAnswers;

        public readonly ObservableList<LeadDetailsAnswerViewModel> Answers = new ObservableList<LeadDetailsAnswerViewModel>();

        readonly LeadDetailsViewModel ParentViewModel;
        public string QuestionUID { get; }
        public string Title { get; }

        public LeadDetailsQuestionViewModel(LeadDetailsViewModel parentViewModel, EventQuestionDTO questionDTO, List<LeadQuestionAnswerDTO> questionAnswersDTO)
        {
            ParentViewModel = parentViewModel;
            QuestionUID = questionDTO.UID;
            Title = questionDTO.Content;
            _checkedAnswers = Value<ObservableList<LeadDetailsAnswerViewModel>>(new ObservableList<LeadDetailsAnswerViewModel>());

            var newAnswers = questionDTO.Answers.OrderBy(_ => _.Position).Select(_ => new LeadDetailsAnswerViewModel(_));
            Answers.Reset(newAnswers);

            if (questionAnswersDTO != null)
            {
                questionAnswersDTO.ForEach((answer) =>
                {
                    var selectedAnswer = Answers.FirstOrDefault(_ => _.AnswerUID == answer.AnswerUID);
                    if (selectedAnswer != null)
                        CheckedAnswers.Add(selectedAnswer);
                });

            }

            SaveAnswer = new Command
            {
                Action = SaveAnswerAction
            };
        }

        public ObservableList<LeadDetailsAnswerViewModel> CheckedAnswers
        {
            get
            {
                return _checkedAnswers.Value;
            }
            set
            {
                _checkedAnswers.SetValue(value);
                ParentViewModel.PerformSave();
                RaisePropertyChanged();
            }
        }

        public List<LeadQuestionAnswerDTO> LeadQuestionAnswersDTO
        {
            get
            {
                if (CheckedAnswers == null || CheckedAnswers.IsNullOrEmpty()) return null;
                var answers = new List<LeadQuestionAnswerDTO>();

                foreach (LeadDetailsAnswerViewModel answer in CheckedAnswers)
                {
                    answers.Add(new LeadQuestionAnswerDTO()
                    {
                        QuestionUID = QuestionUID,
                        QuestionName = Title,
                        AnswerUID = answer.AnswerUID,
                        AnswerName = answer.Answer
                    });
                }
                return answers;
            }
        }

        public Command SaveAnswer { get; private set; }
        void SaveAnswerAction(object obj)
        {
            var answers = (IEnumerable<LeadDetailsAnswerViewModel>)obj;
            if (answers == null || answers.Count() == 0)
                CheckedAnswers.Clear();
            else
                CheckedAnswers.Reset(answers);
            ParentViewModel.PerformSave();
        }
    }
}
