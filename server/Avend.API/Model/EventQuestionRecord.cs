using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avend.API.Model
{
    [Table("event_questions")]
    public class EventQuestionRecord: BaseRecord, IDeletable
    {
        public EventQuestionRecord()
        {
            Choices = new List<AnswerChoiceRecord>();
        }

        [Key]
        [Column("id")]
        public override long Id { get; set; }

        [Column("uid", TypeName = "UniqueIdentifier")]
        public override Guid Uid { get; set; }

        [Column("event_id")]
        public long EventId { get; set; }

        [ForeignKey("EventId")]
        public EventRecord Event { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public SubscriptionMember User { get; set; }

        [Column("position")]
        public int Position { get; set; }

        [Column("text")]
        public string Text { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }

        public List<AnswerChoiceRecord> Choices { get; set; }
    }
}
