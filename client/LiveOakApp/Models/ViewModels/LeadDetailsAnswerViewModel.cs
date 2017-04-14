using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.Models.ViewModels
{
    public class LeadDetailsAnswerViewModel : DataContext
    {
        public string AnswerUID { get; }

        public string Answer { get; }

        public LeadDetailsAnswerViewModel(EventQuestionAnswerDTO answer)
        {
            AnswerUID = answer.UID;
            Answer = answer.Content;
        }
    }
}
