using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Avend.API.Infrastructure.SearchExtensions;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Model;

using Microsoft.EntityFrameworkCore;

using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class AgendaItemsRepository
    {
        private AvendDbContext db;

        public AgendaItemsRepository(AvendDbContext db)
        {
            Assert.Argument(db, nameof(db)).NotNull();

            this.db = db;
        }

        public DefaultSearch<EventAgendaItem> SearchWithFilter(EventRecord eventRecord, DateTime? date, SearchQueryParams searchParams)
        {
            searchParams.ApplyDefaultSortOrder<EventAgendaItem>();
            searchParams.Validate().Throw();

            IQueryable<EventAgendaItem> recordsQuery = db.EventAgendaItemsTable.Include(record => record.EventRecord)
                .Where(record => record.EventId == eventRecord.Id);

            if (date.HasValue)
            {
                recordsQuery = recordsQuery.Where(record => record.Date.Date == date.Value.Date);
            }

            var search = DefaultSearch.Start(searchParams, recordsQuery);

            return search;
        }

        public EventAgendaItem FindByUid(EventRecord @event, Guid agendaItemUid)
        {
            return db.EventAgendaItemsTable
                .FirstOrDefault(x => x.EventId == @event.Id && x.Uid == agendaItemUid);
        }

        public EventAgendaItem CreateNew(EventRecord @event)
        {
            var entity = db.EventAgendaItemsTable.Add(new EventAgendaItem()
            {
                Uid = Guid.NewGuid(),
                EventRecord = @event,
            });

            return entity.Entity;
        }

        public void Delete(EventAgendaItem agendaItem)
        {
            db.EventAgendaItemsTable.Remove(agendaItem);
        }

        public Task<long> DeleteByUids(Guid eventUid, IEnumerable<Guid> agendaItemUids)
        {
/*
            var affected = await db.Database.ExecuteSqlCommandAsync(
                "DELETE FROM event_agenda_items WHERE event_uid=@event_uid AND event_agenda_item_uid IN (@uids_array)",
                parameters: new object[]
                {
                    new SqlParameter("event_uid", SqlDbType.UniqueIdentifier) { Value =  eventUid},
                    new SqlParameter("uids_array", SqlDbType.Variant) { Value = agendaItemUids.Select(x => x.ToString()).ToArray() }
                }
            );
*/

            var affected = db.EventAgendaItemsTable.Include(record => record.EventRecord)
                .Count(record => record.EventRecord.Uid == eventUid
                                 && agendaItemUids.Contains(record.Uid));

            db.EventAgendaItemsTable.RemoveRange(db.EventAgendaItemsTable.Include(record => record.EventRecord)
                .Where(record => record.EventRecord.Uid == eventUid
                                 && agendaItemUids.Contains(record.Uid)
                )
            );

            return Task.FromResult((long)affected);
        }
    }
}