using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class LeadQuestionAnswerDTO
    {
        [DataMember(Name = "lead_question_answer_uid")]
        public string UID { get; set; }

        [DataMember(Name = "question_uid")]
        public string QuestionUID { get; set; }

        [DataMember(Name = "answer_uid")]
        public string AnswerUID { get; set; }

        [DataMember(Name = "question_text")]
        public string QuestionName { get; set; }

        [DataMember(Name = "answer_text")]
        public string AnswerName { get; set; }
    }
}
