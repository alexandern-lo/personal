using System;
using System.Linq;
using System.Threading.Tasks;

using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services;

using Microsoft.EntityFrameworkCore;

using Qoden.Validation;

namespace Avend.API.BL
{
    /// <summary>
    /// Special class for record that reference a meeting event.
    /// 
    /// It requres AvendDbContext to make DB queries for events.
    /// </summary>
    /// <typeparam name="TRecord">DB Model class (referencing the event with EventId property) this writer is using to create/update/delete.</typeparam>
    /// <typeparam name="TDto">DTO class (referencing the event with EventUid property) this writer is using to get data from.</typeparam>
    public class BaseEventRelatedWriter<TRecord, TDto> : BaseWriter<TRecord, TDto>
        where TRecord : class, new()
        where TDto : class, IEventBasedDto, new()
    {
        public long? EventId { get; protected set; }

        protected BaseEventRelatedWriter(AvendDbContext avendDb) : base (avendDb)
        {
        }

        /// <summary>
        /// Returns event object for given event UID and using given IQueryable to query event from.
        /// </summary>
        /// 
        /// <param name="eventUid">Uid of the event to retrieve.</param>
        /// <param name="eventsTable">IQueryable to query event from.</param>
        /// 
        /// <returns>EventRecord object populated according to eventsTable provided.</returns>
        protected async Task<EventRecord> GetEventByUid(Guid? eventUid, IQueryable<EventRecord> eventsTable)
        {
            var eventRecord = await (from recEvent in eventsTable
                                 where recEvent.Uid == eventUid
                                 select recEvent).FirstOrDefaultAsync();

            return eventRecord;
        }

        /// <summary>
        /// Returns event Id for given event UID and using given IQueryable to query event from.
        /// </summary>
        /// 
        /// <param name="eventUid">Uid of the event to retrieve ID for.</param>
        /// <param name="eventsTable">IQueryable to query event from.</param>
        /// 
        /// <returns>Id of the event or zero if it's not found.</returns>
        protected async Task<long> GetEventIdByUid(Guid? eventUid, IQueryable<EventRecord> eventsTable)
        {
            var eventId = await (from recEvent in eventsTable
                                 where recEvent.Uid == eventUid
                                 select recEvent.Id).FirstOrDefaultAsync();

            return eventId;
        }

        /// <summary>
        /// Sets EventId from the request body using GetEventIdByUid method defined above.
        /// </summary>
        /// 
        /// <returns>True if successful.</returns>
        public async Task<bool> SetEventIdFromRequestBody()
        {
            Assert.State(RequestBody, "request body").NotNull("Request body is not valid");
            Assert.State(RequestBody.EventUid, "event_uid").NotNull("Event UID cannot be null");

            EventId = await GetEventIdByUid(RequestBody.EventUid, GetUserAccessibleEventsTable());

            Validator.CheckValue(EventId, "event_uid").NotNull("Event is not found");
            Validator.CheckValue(EventId, "event_uid").ParameterNotEqualsTo(0, typeof(EventRecord), RequestBody.EventUid?.ToString() ?? "null uid", "Event is not found");

            return EventId.HasValue && EventId > 0;
        }

        /// <summary>
        /// Narrows the events table for the given user.
        /// </summary>
        /// 
        /// <returns>True if successful.</returns>
        private IQueryable<EventRecord> GetUserAccessibleEventsTable()
        {
/*
            return AvendDbContext.EventsTable.Where(
                record => record.Type == Event.EventTypeConference
                );

*/
            return AvendDbContext.EventsTable;
        }

        /// <summary>
        /// Implemented additional validation that EventUid is present in the request body DTO.
        /// </summary>
        /// 
        /// <returns>True if successful.</returns>
        public override bool ValidateRequestBody()
        {
            if (!base.ValidateRequestBody())
                return false;

            Validator.CheckValue(RequestBody.EventUid, "event_uid").NotNull("Event UID cannot be empty");

            return Validator.IsValid;
        }
    }
}
