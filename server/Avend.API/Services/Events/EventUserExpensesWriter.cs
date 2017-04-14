using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using Avend.API.BL;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Services.Events.NetworkDTO;

using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class EventUserExpensesWriter : BaseEventRelatedWriter<EventUserExpenseRecord, EventUserExpenseDto>
    {
        public EventUserExpensesWriter(AvendDbContext avendDb) : base(avendDb)
        {
        }

        public override bool ValidateRequestBody()
        {
            if (!base.ValidateRequestBody())
                return false;

            Validator.CheckValue(RequestBody.Expense, "expense").MoneyIsValid(RequestBody.Uid?.ToString() ?? "new record object");

            return Validator.IsValid;
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public bool MakeNewRecord()
        {
            Assert.State(UserUid, "user_uid").NotNull();

            Assert.State(RequestBody, "request body").NotNull();
            Assert.State(RequestBody.Expense, "request body.expense").NotNull("'expense' field should have a valid value");
            Assert.State(RequestBody.Expense.Currency, "request body.expense.currency").NotNull("'expense.currency' field should have a valid value");

            Assert.State(EventId, "event_uid").NotNull("Event UID is not found");
            Assert.State(EventId, "event_uid").ParameterNotEqualsTo(0, typeof(EventRecord), RequestBody.EventUid.ToString(), "Event UID is not found");

            if (Validator.HasErrors)
                return false;

            PreparedRecord = new EventUserExpenseRecord()
            {
                Uid = Guid.NewGuid(),
                UserUid = UserUid.Value,
                TenantUid = TenantUid.Value,

                EventId = EventId.Value,
                Amount = RequestBody.Expense.Amount,
                Currency = RequestBody.Expense.Currency.Value,
                Comments = RequestBody.Comments,
                SpentAt = DateTime.UtcNow,
            };

            return true;
        }

        public bool LoadRecordAndApplyChanges()
        {
            Assert.State(UserUid, "user_uid").NotNull();
            Assert.State(RequestBody, "request body").NotNull();
            Assert.State(EventId, "event_uid").NotNull("Event UID is not found");

            if (Validator.HasErrors)
            {
                return false;
            }

            PreparedRecord = AvendDbContext.EventUserExpensesTable.FirstOrDefault(
                record => record.Uid == RequestBody.Uid
                          && record.UserUid == UserUid
                          && record.EventId == EventId);

            Assert.State(PreparedRecord, "event_uid").NotNull("Event for user expense record is not found");
            Assert.State(PreparedRecord.EventId, "event_uid").EqualsTo(EventId.Value, "Event for user expense record is not found");

            if (Validator.HasErrors)
                return false;

            RequestBody.ApplyChangesToModel(PreparedRecord);

            return PreparedRecord != null;
        }

        public async Task<bool> SaveChanges()
        {
            if (PreparedRecord.Id == 0)
                AvendDbContext.EventUserExpensesTable.Add(PreparedRecord);
            else
                AvendDbContext.EventUserExpensesTable.Update(PreparedRecord);

            try
            {
                await AvendDbContext.SaveChangesAsync();

                RequestBody.Uid = PreparedRecord.Uid;
            }
            catch (Exception ex)
            {
                Validator.Add(new Error("Database exception: " + ex.Message) {{"Exception", ex}});
            }

            return Validator.IsValid;
        }
    }
}