using System;
using System.Linq;
using System.Linq.Expressions;
using Avend.API.Infrastructure.SearchExtensions;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Model;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class EventsRepository
    {
        public EventsRepository(AvendDbContext db)
        {
            Assert.Argument(db, nameof(db)).NotNull();
            Db = db;
        }

        public AvendDbContext Db { get; }

        /// <summary>
        /// Search for event with given params. Returned event has Owner and Subscription
        /// </summary>
        /// <param name="searchQuery">search query</param>
        /// <returns>search object</returns>
        public DefaultSearch<EventRecord> Search(SearchQueryParams searchQuery)
        {
            searchQuery.ApplyDefaultSortOrder<EventRecord>();
            searchQuery.Validate().Throw();

            var dbSet = Db.EventsTable
                .Include(x => x.Owner)
                .Include(eventObj => eventObj.Subscription)
                .Include(eventObj => eventObj.AttendeeCategories)
                .ThenInclude(categoryObj => categoryObj.Options)
                .Include(eventObj => eventObj.Questions)
                .ThenInclude(questionObj => questionObj.Choices)
                .NotDeleted()
                .Where(Scope);

            return DefaultSearch.Start(searchQuery, dbSet);
        }

        /// <summary>
        /// Find one event by given uid. Returned event has Subscription, AttendeeCategories with Options and Questions with Choices.
        /// </summary>
        /// <param name="eventUid">event uid</param>
        /// <returns>event record</returns>
        public EventRecord FindEventByUid(Guid eventUid)
        {
            return Db.EventsTable
                .NotDeleted()
                .Where(Scope)
                .Include(x => x.Owner)
                .Include(eventObj => eventObj.Subscription)
                .Include(eventObj => eventObj.AttendeeCategories)
                .ThenInclude(categoryObj => categoryObj.Options)
                .Include(eventObj => eventObj.Questions)
                .ThenInclude(questionObj => questionObj.Choices)
                .FirstOrDefault(x => x.Uid == eventUid);
        }

        /// <summary>
        /// Fetch events with given uids. Does not load any referenced records.
        /// </summary>
        /// <param name="eventUids">list of event uid</param>
        /// <returns>found event records</returns>
        public IQueryable<EventRecord> FindEventByUids(Guid[] eventUids)
        {
            return Db.EventsTable
                .NotDeleted()
                .Where(Scope)
                .Where(x => eventUids.Contains(x.Uid));
        }

        /// <summary>
        /// Query to filter events loaded by this repository
        /// </summary>
        public Expression<Func<EventRecord, bool>> Scope { get; set; } = e => true;

        /// <summary>
        /// Load event records with Questions ordered by start date.
        /// </summary>
        /// <returns>events queryable</returns>
        public IQueryable<EventRecord> GetSummary()
        {
            return Db.EventsTable
                .Include(x => x.Questions)
                .NotDeleted()
                .Where(Scope)
                .OrderByDescending(x => x.StartDate);
        }

        /// <summary>
        /// Create empty event
        /// </summary>
        /// <param name="userUid">owner id</param>
        /// <param name="subscription">owner susbcription if any</param>
        /// <returns>fresh event</returns>
        public EventRecord CreateEvent(Guid userUid, Guid? subscription)
        {
            var user = Db.SubscriptionMembers.FirstOrDefault(x => x.UserUid == userUid);
            var @event = new EventRecord
            {
                Uid = Guid.NewGuid(),
                Subscription =
                    subscription.HasValue ? Db.SubscriptionsTable.First(x => x.Uid == subscription) : null,
                Owner = user
            };
            if (@event.Subscription != null)
            {
                Db.Entry(@event.Subscription).State = EntityState.Unchanged;
            }
            Db.EventsTable.Add(@event);
            return @event;
        }

        /// <summary>
        /// Delete event (does not apply Scope predicate)
        /// </summary>
        /// <param name="event">event record</param>
        /// <param name="soft">perform soft or hard delete</param>
        public void DeleteEvent(EventRecord @event, bool soft = true)
        {
            Assert.Argument(@event, nameof(@event)).NotNull();

            if (soft)
            {
                @event.Deleted = true;
            }
            else
            {
                var categories = Db.AttendeeCategories
                    .Where(x => x.EventId == @event.Id);

                var categoryOptions = Db.AttendeeCategoryOptions
                    .Where(x => categories.Contains(x.AttendeeCategory));
                Db.RemoveRange(categoryOptions);

                var categoryValues = Db.AttendeeCategoryValues
                    .Where(x => categories.Contains(x.Category));
                Db.RemoveRange(categoryValues);

                var attendees = Db.Attendees
                    .Where(x => x.EventId == @event.Id);
                Db.RemoveRange(attendees);

                Db.RemoveRange(categories);

                var agenda = Db.EventAgendaItemsTable
                    .Where(x => x.EventId == @event.Id);
                Db.RemoveRange(agenda);

                Db.EventsTable.Remove(@event);
            }
        }
    }
}