using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Avend.API.Helpers;
using Avend.API.Infrastructure.Logging;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Qoden.Validation;

namespace Avend.API.Services.Events
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventRange
    {
        [EnumMember(Value = "all")] All,
        [EnumMember(Value = "ongoing")] Ongoing,
        [EnumMember(Value = "upcoming")] Upcoming,
        [EnumMember(Value = "past")] Past
    }

    [DataContract]
    public enum EventScope
    {
        [EnumMember(Value = "available")] Avaialble,
        [EnumMember(Value = "selectable")] Selectable,
    }

    public class EventsService
    {
        public EventsService(DbContextOptions<AvendDbContext> dbOptions,
            UserContext userContext)
        {
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();
            Assert.Argument(userContext, nameof(userContext)).NotNull();

            DbContextOptions = dbOptions;
            UserContext = userContext;
            Logger = AvendLog.CreateLogger(nameof(EventsService));
        }

        public ILogger Logger { get; }
        public UserContext UserContext { get; }
        public DbContextOptions<AvendDbContext> DbContextOptions { get; }

        public async Task<SearchResult<EventDto>> Find(
            SearchQueryParams searchQuery, 
            string eventType, 
            Guid? tenant, 
            EventRange range, 
            string industry, 
            DateTime? startAfter, 
            DateTime? endsBefore, 
            EventScope scope, 
            Guid? user)
        {
            Assert.Argument(searchQuery, nameof(searchQuery)).NotNull();

            using (var db = new AvendDbContext(DbContextOptions))
            {
                var repo = new EventsRepository(db);
                var principal = await UserContext.GetSubordinate(user);
                Check.Value(principal, "user", AvendErrors.NotFound).NotNull("{Key} not found");

                switch (scope)
                {
                    case EventScope.Avaialble:
                        repo.Scope = principal.AvailableEvents();
                        break;
                    case EventScope.Selectable:
                        repo.Scope = principal.SelectableEvents();
                        break;
                    default:
                        throw new ArgumentException("scope");
                }

                var tz = UserContext.TimeZone;
                var search = repo.Search(searchQuery);

                search.Filter(collection =>
                {
                    if (startAfter != null)
                    {
                        var start = TimeZoneInfo.ConvertTime(startAfter.Value.ToUniversalTime(), TimeZoneInfo.Utc, tz);
                        start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                        collection = collection.Where(x => x.StartDate >= start);
                    }

                    if (endsBefore != null)
                    {
                        var ends = TimeZoneInfo.ConvertTime(endsBefore.Value.ToUniversalTime(), TimeZoneInfo.Utc, tz);
                        ends = new DateTime(ends.Year, ends.Month, ends.Day, 0, 0, 0);
                        ends = ends.Add(TimeSpan.FromHours(24));
                        collection = collection.Where(x => x.EndDate <= ends);
                    }

                    if (startAfter == null && endsBefore == null)
                    {
                        var localNow = TimeZoneInfo.ConvertTime(DateTime.Now.ToUniversalTime(), TimeZoneInfo.Utc, tz);
                        switch (range)
                        {
                            case EventRange.Ongoing:
                                collection =
                                    collection.Where(
                                        x => (x.StartDate <= localNow && x.EndDate >= localNow) || x.Recurring);
                                break;
                            case EventRange.Past:
                                collection = collection.Where(x => x.EndDate < localNow);
                                break;
                            case EventRange.Upcoming:
                                collection = collection.Where(x => x.StartDate > localNow);
                                break;
                        }
                    }

                    if (eventType != null)
                    {
                        collection = collection.Where(x => x.Type == eventType);
                    }

                    if (tenant.HasValue)
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        var subscriptions = new SubscriptionRepository(db);
                        var subscriptionRecord = subscriptions.FindSubscriptionByUid(tenant.Value);
                        if (subscriptionRecord != null)
                        {
                            collection = collection.Where(x => x.SubscriptionId == subscriptionRecord.Id);
                        }
                        else
                        {
                            collection = Enumerable.Empty<EventRecord>().AsQueryable();
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(industry))
                    {
                        collection = collection.Where(x => x.Industry == industry);
                    }
                    return collection;
                });

                return (await search.PaginateAsync(EventDto.From))
                    .WithFilter("event_type", eventType)
                    .WithFilter("tenant", tenant)
                    .WithFilter("range", range)
                    .WithFilter("industry", industry)
                    .WithFilter("start_after", startAfter)
                    .WithFilter("end_before", endsBefore);
            }
        }

        public EventDto GetEvent(Guid eventUid)
        {
            using (var leadsDb = new AvendDbContext(DbContextOptions))
            {
                var repo = new EventsRepository(leadsDb) {Scope = UserContext.AvailableEvents()};
                var @event = repo.FindEventByUid(eventUid);
                Check.Value(@event, "event", AvendErrors.NotFound).NotNull();
                return EventDto.From(@event);
            }
        }

        public SearchResult<MeetingEventUidAndNameDTO> GetSummary()
        {
            using (var leadsDb = new AvendDbContext(DbContextOptions))
            {
                var repo = new EventsRepository(leadsDb) {Scope = UserContext.AvailableEvents()};
                var search = repo.GetSummary();
                var dtos = search.Select(x => new MeetingEventUidAndNameDTO
                    {
                        Uid = x.Uid,
                        Type = x.Type,
                        Name = x.Name,
                        StartDate = x.StartDate,
                        City = x.City,
                        State = x.State,
                        QuestionsCount = x.Questions.Count(),
                    })
                    .ToList();
                return new SearchResult<MeetingEventUidAndNameDTO>
                {
                    Data = dtos,
                    Total = dtos.Count
                };
            }
        }

        public async Task<EventDto> Create(EventDto dto)
        {
            using (var leadsDb = new AvendDbContext(DbContextOptions))
            {
                var repo = new EventsRepository(leadsDb);
                var @event = new Event(repo, UserContext);
                @event.Create();
                @event.Update(dto);
                @event.Validator.Throw();

                await leadsDb.SaveChangesAsync();

                return EventDto.From(@event.Data);
            }
        }

        public async Task<EventDto> Update(Guid eventUid, EventDto dto)
        {
            Check.Value(dto, "event").NotNull();

            using (var db = new AvendDbContext(DbContextOptions))
            {
                var repo = new EventsRepository(db);
                var @event = new Event(repo, UserContext);
                @event.Find(eventUid);
                @event.Update(dto);
                @event.Validator.Throw();

                await db.SaveChangesAsync();

                return EventDto.From(@event.Data);
            }
        }

        public async Task Delete(Guid eventUid)
        {
            using (var leadsDb = new AvendDbContext(DbContextOptions))
            {
                var repo = new EventsRepository(leadsDb);
                var @event = new Event(repo, UserContext);
                @event.Find(eventUid);
                @event.Delete();

                await leadsDb.SaveChangesAsync();
            }
        }


        public async Task MassDelete(Guid[] eventUids)
        {
            using (var db = new AvendDbContext(DbContextOptions))
            {
                var events = new EventsRepository(db) {Scope = UserContext.OwnEvents()};
                foreach (var e in events.FindEventByUids(eventUids))
                {
                    events.DeleteEvent(e);
                }
                await db.SaveChangesAsync();
            }
        }

        public async Task<IList<EventRecord>> MassGenerate(int count, DateTime? eventStartDate, DateTime? eventEndDate,
            int minCategories,
            int cntAttendees)
        {
            if (count <= 0)
                count = 1;
            var minQuestions = 2;
            if (minCategories < 0)
                minCategories = 0;
            if (minCategories >= 5)
                minCategories = 4;
            if (cntAttendees < 0)
                cntAttendees = 0;
            var random = new Random();
            // ReSharper disable once UnusedVariable
            var next = random.Next();
            var optionsFaker = FakerHelper.AttendeeCategoryOptionsFaker();
            var categoriesFaker = FakerHelper.AttendeeCategoriesFaker(random, optionsFaker);
            var answersFaker = FakerHelper.EventQuestionAnswerFaker();
            var questionsFaker = FakerHelper.EventQuestionFaker(UserContext.UserId.GetValueOrDefault(), random, answersFaker);
            var eventsFaker = FakerHelper.EventsFaker(random, categoriesFaker, questionsFaker, eventStartDate,
                eventEndDate, minCategories, minQuestions);
            using (var db = new AvendDbContext(DbContextOptions))
            {
                var meetingEvents = eventsFaker.Generate(count);
                var meetingEventsList = meetingEvents as IList<EventRecord> ?? meetingEvents.ToList();
                Logger.LogDebug("Generated events list as folows:\n" + JsonConvert.SerializeObject(meetingEventsList,
                                    new JsonSerializerSettings
                                    {
                                        Formatting = Formatting.Indented,
                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                        PreserveReferencesHandling = PreserveReferencesHandling.None,
                                        DefaultValueHandling = DefaultValueHandling.Ignore,
                                    }));

                db.EventsTable.AddRange(meetingEventsList);
                var eventAttendees = new List<AttendeeRecord>();
                foreach (var meetingEvent in meetingEventsList)
                {
                    eventAttendees.AddRange(
                        FakerHelper.AttendeesFaker(db, random, 0, meetingEvent).Generate(cntAttendees).ToList());
                }
                Logger.LogDebug("Generated attendees list for those events as folows:\n" +
                                JsonConvert.SerializeObject(eventAttendees,
                                    new JsonSerializerSettings
                                    {
                                        Formatting = Formatting.Indented,
                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                    }));
                db.Attendees.AddRange(eventAttendees);
                await db.SaveChangesAsync();
                return meetingEventsList;
            }
        }

        public async Task<MoneyDto> GetEventExpenses(Guid eventUid)
        {
            using (var db = new AvendDbContext(DbContextOptions))
            {
                var eventsRepository = new EventsRepository(db) {Scope = UserContext.AvailableEvents()};
                var eventRecord = eventsRepository.FindEventByUid(eventUid);

                Check.Value(eventRecord, "event_uid", AvendErrors.NotFound).NotNull();

                var expenses = new EventUserExpensesRepository(db);
                return await expenses.GetTotalEventExpensesAmountForUserAndEvent(UserContext.UserUid, eventRecord.Id);
            }
        }
    }
}