using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Avend.API.Infrastructure.Logging;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;

using Microsoft.Extensions.Logging;

using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class AgendaItemsWriter
    {
        public Validator Validator { get; }
        public ILogger<AgendaItemsWriter> Logger { get; }

        private readonly UserContext userContext;
        private readonly AvendDbContext db;
        private readonly AgendaItemsRepository repo;

        private EventRecord eventRecord;
        private EventAgendaItem agendaItem;

        public AgendaItemsWriter(AvendDbContext db, UserContext userContext)
        {
            this.userContext = userContext;
            this.db = db;

            repo = new AgendaItemsRepository(db);

            Validator = new Validator();
            Logger = AvendLog.CreateLogger<AgendaItemsWriter>();
        }

        public void FindEventRecord(Guid eventUid)
        {
            var eventsRepository = new EventsRepository(db)
            {
                Scope = userContext.AvailableEvents()
            };

            eventRecord = eventsRepository.FindEventByUid(eventUid);

            Validator.CheckValue(eventRecord, "event_uid").NotNull(onError: AvendErrors.NotFound);
            Validator.CheckValue(eventRecord.Type, "event_uid").EqualsTo("conference", "Cannot get agenda items for non-conference events");
        }

        public EventAgendaItem ConstructAgendaItem()
        {
            Assert.State(eventRecord).NotNull();
            Assert.State(agendaItem).IsNull();
            Assert.State(userContext.UserUid).NotNull();
            Assert.State(userContext.Role == UserRole.SuperAdmin).IsTrue();

            agendaItem = repo.CreateNew(eventRecord);

            return agendaItem;
        }

        public EventAgendaItem FindAgendaItem(Guid agendaItemUid)
        {
            Assert.State(eventRecord).NotNull();
            Assert.State(agendaItem).IsNull();
            Assert.State(userContext.UserUid).NotNull();
            Assert.State(userContext.Role == UserRole.SuperAdmin).IsTrue();

            agendaItem = repo.FindByUid(eventRecord, agendaItemUid);

            Validator.CheckValue(agendaItem, "agenda_item_uid", AvendErrors.NotFound).NotNull();

            return agendaItem;
        }

        public void ApplyAndValidateChanges(EventAgendaItemDTO dto)
        {
            Assert.State(eventRecord).NotNull();
            Assert.State(agendaItem).NotNull();
            Assert.State(userContext.UserUid).NotNull();
            Assert.State(userContext.Role == UserRole.SuperAdmin).IsTrue();

            dto.ApplyChangesToModel(agendaItem);

            Validator.CheckValue(agendaItem.StartTime < agendaItem.EndTime, "start_time").IsTrue("start_time should be less than end_time");
        }

        public EventAgendaItem DeleteRecord(Guid agendaItemUid)
        {
            Assert.State(eventRecord).NotNull();
            Assert.State(agendaItem).IsNull();
            Assert.State(userContext.UserUid).NotNull();
            Assert.State(userContext.Role == UserRole.SuperAdmin).IsTrue();

            agendaItem = repo.FindByUid(eventRecord, agendaItemUid);

            Validator.CheckValue(agendaItem, "agenda_item_uid", AvendErrors.NotFound).NotNull();

            repo.Delete(agendaItem);

            return agendaItem;
        }

        public async Task<long> DeleteRecords(IEnumerable<Guid> agendaItemUids)
        {
            Assert.State(eventRecord).NotNull();
            Assert.State(agendaItem).IsNull();
            Assert.State(userContext.UserUid).NotNull();
            Assert.State(userContext.Role == UserRole.SuperAdmin).IsTrue();

            var result = await repo.DeleteByUids(eventRecord.Uid, agendaItemUids);

            return result;
        }
    }
}