using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

namespace Avend.API.Model
{
    [Table("lead_question_answers")]
    public class LeadQuestionAnswer: BaseRecord
    {
        [Key]
        [Column("lead_question_answer_id")]
        public override long Id { get; set; }

        [Column("lead_question_answer_uid", TypeName = "UniqueIdentifier")]
        public override Guid Uid { get; set; }

        [Column("event_question_id")]
        public long EventQuestionId { get; set; }

        [Column("event_question_uid")]
        public Guid? EventQuestionUid { get; set; }

        [ForeignKey("EventQuestionId")]
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public EventQuestionRecord EventQuestion { get; set; }

        [Column("event_answer_id")]
        public long EventAnswerId { get; set; }

        [Column("event_answer_uid")]
        public Guid? EventAnswerUid { get; set; }

        [ForeignKey("EventAnswerId")]
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public AnswerChoiceRecord Answer { get; set; }

        [Column("lead_id")]
        public long LeadId { get; set; }

        [ForeignKey("LeadId")]
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public LeadRecord LeadRecord { get; set; }

        [Column("question_text")]
        public string QuestionText { get; set; }

        [Column("answer_text")]
        public string AnswerText { get; set; }
    }
}
