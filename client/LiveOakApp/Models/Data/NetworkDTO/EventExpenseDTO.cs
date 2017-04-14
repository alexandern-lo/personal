using System;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract(Name = "event_user_expense")]
    public class EventExpenseDTO
    {
        [DataMember(Name = "event_user_expense_uid")]
        public Guid? Uid { get; set; }

        [DataMember(Name = "event_uid", IsRequired = true)]
        public Guid? EventUid { get; set; }

        [DataMember(Name = "expense", IsRequired = true)]
        public MoneyDTO Expense { get; set; }

        [DataMember(Name = "comments")]
        public string Comments { get; set; }
    }
}
