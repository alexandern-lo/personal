using System;
using System.Linq;
using System.Threading.Tasks;

using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Events.NetworkDTO;

using Microsoft.EntityFrameworkCore;

using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class AgendaItemsService
    {
        public readonly DbContextOptions<AvendDbContext> DbOptions;
        public readonly UserContext UserContext;

        public AgendaItemsService(DbContextOptions<AvendDbContext> dbOptions, UserContext userContext)
        {
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();
            Assert.Argument(userContext, nameof(userContext)).NotNull();

            this.DbOptions = dbOptions;
            this.UserContext = userContext;
        }

        public SearchResult<EventAgendaItemDTO> FindAndPaginate(Guid? eventUid, string dateStr, SearchQueryParams searchParams)
        {
            Check.Value(eventUid, "event_uid", AvendErrors.NotFound).NotNull();
            Check.Value(searchParams, nameof(searchParams)).NotNull();

            DateTime? date = null;

            if (!string.IsNullOrWhiteSpace(dateStr))
            {
                DateTime parsedDate;
                var dateParseResult = DateTime.TryParse(dateStr, out parsedDate);

                Check.Value(dateParseResult, "date").IsTrue("Date is not valid or doesn't have a valid format");

                date = parsedDate;
            }

            using (var db = new AvendDbContext(DbOptions))
            {
                var eventsRepo = new EventsRepository(db)
                {
                    Scope = UserContext.AvailableEvents()
                };
                var eventRecord = eventsRepo.FindEventByUid(eventUid.Value);

                Check.Value(eventRecord, "event_uid", AvendErrors.NotFound).NotNull();
                Check.Value(eventRecord.Type, "event_uid").EqualsTo("conference", "Cannot get agenda items for non-conference events");

                var agendaRepo = new AgendaItemsRepository(db);

                var search = agendaRepo.SearchWithFilter(eventRecord, date, searchParams);

                return search.Paginate(record => EventAgendaItemDTO.From(record, eventUid.Value))
                    .WithFilter("date", date);
            }
        }

        public EventAgendaItemDTO FindByUid(Guid? eventUid, Guid? agendaItemUid)
        {
            Check.Value(eventUid, "event_uid", AvendErrors.NotFound).NotNull();
            Check.Value(agendaItemUid, "agenda_item_uid", AvendErrors.NotFound).NotNull();

            using (var db = new AvendDbContext(DbOptions))
            {
                var eventsRepo = new EventsRepository(db)
                {
                    Scope = UserContext.AvailableEvents()
                };

                var eventRecord = eventsRepo.FindEventByUid(eventUid.Value);

                Check.Value(eventRecord, "event_uid", AvendErrors.NotFound).NotNull();
                Check.Value(eventRecord.Type, "event_uid").EqualsTo("conference", "Cannot get agenda items for non-conference events");

                var agendaRepo = new AgendaItemsRepository(db);
                var agendaItem = agendaRepo.FindByUid(eventRecord, agendaItemUid.Value);

                Check.Value(agendaItem, "agenda_item_uid", AvendErrors.NotFound).NotNull();

                return EventAgendaItemDTO.From(agendaItem, eventUid.Value);
            }
        }

        public async Task<EventAgendaItemDTO> CreateRecord(Guid? eventUid, EventAgendaItemDTO newAgendaItemDTO)
        {
            Check.Value(eventUid, "event_uid", AvendErrors.NotFound).NotNull();
            Check.Value(UserContext.UserUid, "user_uid", AvendErrors.Forbidden).NotNull();
            Check.Value(UserContext.Role, "user_uid", AvendErrors.Forbidden).EqualsTo(UserRole.SuperAdmin, "You're not allowed to add agenda items");

            using (var db = new AvendDbContext(DbOptions))
            {
                var agendaWriter = new AgendaItemsWriter(db, UserContext);

                agendaWriter.FindEventRecord(eventUid.Value);
                agendaWriter.Validator.Throw();

                var agendaItem = agendaWriter.ConstructAgendaItem();
                agendaWriter.Validator.Throw();

                agendaWriter.ApplyAndValidateChanges(newAgendaItemDTO);
                agendaWriter.Validator.Throw();

                await db.SaveChangesAsync();

                return EventAgendaItemDTO.From(agendaItem, eventUid.Value);
            }
        }

        public async Task<EventAgendaItemDTO> UpdateRecord(Guid? eventUid, Guid? agendaItemUid, EventAgendaItemDTO dto)
        {
            Check.Value(eventUid, "event_uid", AvendErrors.NotFound).NotNull();
            Check.Value(agendaItemUid, "agenda_item_uid", AvendErrors.NotFound).NotNull();
            Check.Value(UserContext.Role, "user_uid", AvendErrors.Forbidden).EqualsTo(UserRole.SuperAdmin, "You're not allowed to edit agenda items");

            using (var db = new AvendDbContext(DbOptions))
            {
                var agendaWriter = new AgendaItemsWriter(db, UserContext);

                agendaWriter.FindEventRecord(eventUid.Value);
                agendaWriter.Validator.Throw();

                var agendaItem = agendaWriter.FindAgendaItem(agendaItemUid.Value);
                agendaWriter.Validator.Throw();

                agendaWriter.ApplyAndValidateChanges(dto);
                agendaWriter.Validator.Throw();

                await db.SaveChangesAsync();

                return EventAgendaItemDTO.From(agendaItem, eventUid.Value);
            }
        }

        public async Task<EventAgendaItemDTO> DeleteRecord(Guid? eventUid, Guid? agendaItemUid)
        {
            Check.Value(eventUid, "event_uid", AvendErrors.NotFound).NotNull();
            Check.Value(agendaItemUid, "agenda_item_uid", AvendErrors.NotFound).NotNull();
            Check.Value(UserContext.Role, "user_uid", AvendErrors.Forbidden).EqualsTo(UserRole.SuperAdmin, "You're not allowed to delete agenda items");

            using (var db = new AvendDbContext(DbOptions))
            {
                var agendaWriter = new AgendaItemsWriter(db, UserContext);

                agendaWriter.FindEventRecord(eventUid.Value);
                agendaWriter.Validator.Throw();

                var agendaItem = agendaWriter.DeleteRecord(agendaItemUid.Value);
                agendaWriter.Validator.Throw();

                await db.SaveChangesAsync();

                return EventAgendaItemDTO.From(agendaItem, eventUid.Value);
            }
        }

        public async Task<long> MassDeleteRecords(string eventUidStr, EventAgendaItemMassDeleteRequestDto massDeleteRequestDto)
        {
            Check.Value(UserContext.Role, "user_uid", AvendErrors.Forbidden).EqualsTo(UserRole.SuperAdmin, "You're not allowed to delete agenda items");

            Guid eventUid;

            Check.Value(eventUidStr, "event_uid", AvendErrors.NotFound).NotNull();
            var eventUidIsValid = Guid.TryParse(eventUidStr, out eventUid);
            Check.Value(eventUidIsValid, "event_uid").IsTrue("Event uid in request route is not a valid GUID value");

            Check.Value(massDeleteRequestDto, "lead_uids", AvendErrors.InvalidParameter).NotNull();
            Check.Value(massDeleteRequestDto.Uids, "lead_uids", AvendErrors.InvalidParameter).NotNull();

            var leadUids = massDeleteRequestDto.Uids
                .Where(record => record.HasValue)
                .Select(record => record.Value);

            using (var db = new AvendDbContext(DbOptions))
            {
                var agendaWriter = new AgendaItemsWriter(db, UserContext);

                agendaWriter.FindEventRecord(eventUid);
                agendaWriter.Validator.Throw();

                var result = await agendaWriter.DeleteRecords(leadUids);
                agendaWriter.Validator.Throw();

                await db.SaveChangesAsync();

                return result;
            }
        }
    }
}