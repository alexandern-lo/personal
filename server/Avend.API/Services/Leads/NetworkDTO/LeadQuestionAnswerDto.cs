using System;
using System.Runtime.Serialization;

using Avend.API.Model;

namespace Avend.API.Services.Leads.NetworkDTO
{
    [DataContract]
    public class LeadQuestionAnswerDto
    {
        [DataMember(Name = "lead_answer_uid")]
        public Guid? Uid { get; set; }

        [DataMember(Name = "question_uid")]
        public Guid? EventQuestionUid { get; set; }

        [DataMember(Name = "answer_uid")]
        public Guid? EventAnswerUid { get; set; }

        [DataMember(Name = "question_text")]
        public string QuestionText { get; set; }

        [DataMember(Name = "answer_text")]
        public string AnswerText { get; set; }

        public static LeadQuestionAnswerDto From(LeadQuestionAnswer obj)
        {
            var dto = new LeadQuestionAnswerDto()
            {
                Uid = obj.Uid,  
                EventQuestionUid = obj.EventQuestionUid,
                EventAnswerUid = obj.EventAnswerUid,
                QuestionText = obj.QuestionText,
                AnswerText = obj.AnswerText,
            };

            return dto;
        }
    }
}