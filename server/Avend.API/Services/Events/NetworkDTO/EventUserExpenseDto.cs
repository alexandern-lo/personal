using System;
using System.Runtime.Serialization;

using Avend.API.Model;
using Avend.API.Model.NetworkDTO;

namespace Avend.API.Services.Events.NetworkDTO
{
    [DataContract]
    public class EventUserExpenseDto : IEventBasedDto
    {
        [DataMember(Name = "event_user_expense_uid")]
        public Guid? Uid { get; set; }

        [DataMember(Name = "user_uid")]
        public Guid? UserUid { get; set; }

        [DataMember(Name = "event_uid", IsRequired = true)]
        public Guid? EventUid { get; set; }

        [DataMember(Name = "expense", IsRequired = true)]
        public MoneyDto Expense { get; set; }

        [DataMember(Name = "comments")]
        public string Comments { get; set; }

        public static EventUserExpenseDto From(EventUserExpenseRecord obj, Guid? eventUid)
        {
            var dto = new EventUserExpenseDto()
            {
                Uid = obj.Uid,

                UserUid = obj.UserUid,
                EventUid = eventUid,

                Comments = obj.Comments,
                Expense = new MoneyDto()
                {
                    Amount = obj.Amount,
                    Currency = obj.Currency,
                },
            };

            return dto;
        }

        public void ApplyChangesToModel(EventUserExpenseRecord eventUserExpenseRecord)
        {
            if (Comments != null)
            {
                eventUserExpenseRecord.Comments = Comments;
            }

            if (Expense != null)
            {
                eventUserExpenseRecord.Amount = Expense.Amount;

                eventUserExpenseRecord.Currency = Expense.Currency ?? CurrencyCode.USD;
            }
        }
    }
}
