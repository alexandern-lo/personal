using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Avend.API.Model
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CurrencyCode
    {
        [EnumMember(Value = "unknown")]
        Unknown = 0,

        [EnumMember(Value = "usd")]
        USD = 1,

        [EnumMember(Value = "eur")]
        EUR = 2,
    }

    [Table("event_user_expenses")]
    public class EventUserExpenseRecord : BaseUserDependentRecord
    {
        [Key]
        [Column("event_user_expense_id")]
        public override long Id { get; set; }

        [Column("event_user_expense_uid", TypeName = "UniqueIdentifier")]
        public override Guid Uid { get; set; }
        
        [Column("event_id")]
        public long EventId { get; set; }

        [ForeignKey("EventId")]
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public EventRecord EventRecord { get; set; }

        [Column("spent_at")]
        public DateTime SpentAt { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("currency")]
        public CurrencyCode Currency { get; set; }

        [Column("comments")]
        public string Comments { get; set; }
    }
}
