using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

namespace Avend.API.Model
{
    [Table("event_question_answers")]
    public class AnswerChoiceRecord: BaseRecord, IDeletable
    {
        [Key]
        [Column("id")]
        public override long Id { get; set; }

        [Column("uid", TypeName = "UniqueIdentifier")]
        public override Guid Uid { get; set; }

        [Column("question_id")]
        public long EventQuestionId { get; set; }

        [ForeignKey("EventQuestionId")]
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public EventQuestionRecord Question { get; set; }

        [Column("position")]
        public int Position { get; set; }

        [Column("text")]
        public string Text { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }
    }
}
