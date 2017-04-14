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
    public class EventUserGoalsWriter : BaseEventRelatedWriter<EventUserGoalsRecord, EventUserGoalsDto>
    {
        public EventUserGoalsWriter(AvendDbContext avendDb) : base(avendDb)
        {
        }

        public override bool ValidateRequestBody()
        {
            if (!base.ValidateRequestBody())
                return false;

            if (RequestBody.LeadsGoal.HasValue)
                Validator.CheckValue(RequestBody.LeadsGoal.Value, "leads_goal").ParameterGreaterOrEqualThan(0, typeof(EventUserGoalsRecord), RequestBody.Uid?.ToString() ?? "null", "Leads goal should be greater or equal to zero");

            return Validator.IsValid;
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public bool MakeNewRecord()
        {
            Assert.State(UserUid, "user_uid").NotNull();
            Assert.State(RequestBody, "request body").NotNull();
            Assert.State(EventId, "event_uid").NotNull("Event UID is not found");

            PreparedRecord = AvendDbContext.EventUserGoalsTable.FirstOrDefault(
                record => record.UserUid == UserUid
                          && record.EventId == EventId);

            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
            if (PreparedRecord == null)
            {
                PreparedRecord = new EventUserGoalsRecord()
                {
                    Uid = Guid.NewGuid(),

                    UserUid = UserUid.Value,
                    TenantUid = TenantUid.Value,
                    EventId = EventId.Value,
                };
            }

            RequestBody.ApplyChangesToModel(PreparedRecord);

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

            PreparedRecord = AvendDbContext.EventUserGoalsTable.FirstOrDefault(
                record => record.Uid == RequestBody.Uid
                          && record.UserUid == UserUid
                          && record.EventId == EventId);

            Assert.State(PreparedRecord, "event_uid").NotNull("Event user goals record is not found");

            if (Validator.HasErrors)
                return false;

            RequestBody.ApplyChangesToModel(PreparedRecord);

            return PreparedRecord != null;
        }

        public async Task<bool> SaveChanges()
        {
            if (PreparedRecord.Id == 0)
                AvendDbContext.EventUserGoalsTable.Add(PreparedRecord);
            else
                AvendDbContext.EventUserGoalsTable.Update(PreparedRecord);

            try
            {
                await AvendDbContext.SaveChangesAsync();

                RequestBody.Uid = PreparedRecord.Uid;
            }
            catch (Exception ex)
            {
                Validator.Add(new Error("Database exception: " + ex.Message) { {"Exception", ex} });
            }

            return Validator.IsValid;
        }
    }
}
