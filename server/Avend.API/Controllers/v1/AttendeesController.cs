using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Helpers;
using Avend.API.Infrastructure.Responses;
using Avend.API.Infrastructure.SearchExtensions;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Qoden.Validation;
using Swashbuckle.SwaggerGen.Annotations;

namespace Avend.API.Controllers.v1
{
    [Route("api/v1/events/{event_uid}/attendees")]
    [Authorize]
    public class AttendeesController : BaseController
    {
        private static readonly Dictionary<string, string> SortDictionary = new Dictionary<string, string>
        {
            //{ "tenant", QueryableExtensions.PropertyName<AttendeeRecord>(e => e.TenantUid) },
            {"first_name", QueryableExtensions.PropertyName<AttendeeRecord>(e => e.FirstName)},
            {"last_name", QueryableExtensions.PropertyName<AttendeeRecord>(e => e.LastName)},
            {"company_name", QueryableExtensions.PropertyName<AttendeeRecord>(e => e.Company)},
            {"email", QueryableExtensions.PropertyName<AttendeeRecord>(e => e.FirstName)},
//            { "created_at", QueryableExtensions.PropertyName<AttendeeRecord>(e => e.CreatedAt) },
        };

        private AttendeesService _service;

        public AttendeesController(DbContextOptions<AvendDbContext> userOptions,
            DbContextOptions<AvendDbContext> options, ILogger<AttendeesController> logger, AttendeesService service)
            : base(options)
        {
            Assert.Argument(service, nameof(service)).NotNull();
            _service = service;
        }

        /// <summary>
        /// Import attendees from CSV file.
        /// </summary>
        /// <param name="eventUid">Uid identifying the event</param>
#pragma warning disable 1584,1711,1572,1581,1580
        /// <param name="attendees">attendees csv file, max 2Mb</param>
#pragma warning restore 1584,1711,1572,1581,1580
        [HttpPost("import")]
        [SwaggerOperation("ImportFromCsv")]
        [ProducesResponseType(typeof(ImportReport), 200)]
        public async Task<IActionResult> ImportFromCsv([FromRoute(Name = "event_uid")] Guid eventUid)
        {
            var files = Request.Form.Files;
            var file = files.GetFile("attendees");
            var errors = await _service.ImportFromCsv(eventUid, file);
            if (errors.InvalidAttendees.Count == 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(errors);
            }
        }

        protected static IQueryable<AttendeeRecord> ConstructRecordsListQuery(
            IQueryable<AttendeeRecord> dbTable,
            int? pageNumber,
            int? pageSize,
            string sortPropertyName,
            bool isAscending
        )
        {
            IQueryable<AttendeeRecord> queryAttendees = dbTable;

            if (sortPropertyName != null)
            {
                queryAttendees = queryAttendees.OrderBy(sortPropertyName, isAscending);
            }

            if (pageNumber != null && pageNumber > 0 && pageSize != null && pageSize > 0)
                queryAttendees = queryAttendees.Skip(pageNumber.Value * pageSize.Value);

            if (pageSize != null && pageSize > 0)
                queryAttendees = queryAttendees.Take(pageSize.Value);

            return queryAttendees;
        }

        /// <summary>
        /// Returns event attendees by uid, for given event uid.
        /// </summary>
        /// <remarks>Returns event attendees by uid, for given event uid.</remarks>
        /// <param name="eventUid">Uid of the event to get attendee for</param>
        /// <param name="attendeeUid">Uid of the attendees to retrieve</param>
        /// <response code="200">An array of event attendees wrapped in success response object</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet("{attendee_uid}")]
        [SwaggerOperation("GetEventAttendeeByUid")]
        [ProducesResponseType(typeof(OkListResponse<AttendeeDto>), 200)]
        public IActionResult GetEventAttendeeByUid(
            [FromRoute(Name = "event_uid")] Guid? eventUid,
            [FromRoute(Name = "attendee_uid")] Guid? attendeeUid)
        {
            if (!UserUid.HasValue)
                return UnauthorizedWithCodeAndBody(401, ErrorResponse.GenerateInvalidUser("user_uid"));

            if (eventUid == null)
            {
                return NotFound(ErrorResponse.GenerateRequiredParameter(typeof(EventRecord), "event_uid"));
            }

            if (attendeeUid == null)
            {
                return NotFound(ErrorResponse.GenerateRequiredParameter(typeof(AttendeeRecord), "attendee_uid"));
            }

            Logger.LogInformation($@"Trying to get event attendee for UID '{attendeeUid}'");

            using (var db = GetDatabaseService())
            {
                var eventId = (from events in db.EventsTable
                    where events.Uid == eventUid
                    select events.Id).FirstOrDefault();

                if (eventId == 0)
                {
                    return NotFound(ErrorResponse.GenerateNotFound(typeof(EventRecord), eventUid.Value, "event_uid"));
                }

                var eventAttendeesWithCategoryValues =
                    db.Attendees.Include(attendeeObj => attendeeObj.Values)
                        .ThenInclude(valueObj => valueObj.AttendeeCategoryOption)
                        .ThenInclude(valueObj => valueObj.AttendeeCategory);

                var query = (from recEventAttendee in eventAttendeesWithCategoryValues
                    where recEventAttendee.EventId == eventId
                          && recEventAttendee.Uid == attendeeUid
                    select recEventAttendee);

                var eventAttendee = query.FirstOrDefault();

                if (eventAttendee == null)
                {
                    return
                        NotFound(ErrorResponse.GenerateNotFound(typeof(AttendeeRecord), attendeeUid.Value,
                            "attendee_uid"));
                }

                var eventAttendeeDto = AttendeeDto.From(eventAttendee, eventUid);

                var responseObj = new OkResponse<AttendeeDto>()
                {
                    Data = eventAttendeeDto,
                };

                return Ok(responseObj);
            }
        }

        /// <summary>
        /// Returns filtered list of event attendees for given event uid.
        /// </summary>
        /// <remarks>Returns filtered list of event attendees according to filter and other parameters\n</remarks>
        /// <param name="eventUid">Uid of the event to get attendees for</param>
        /// <param name="filter">A string to filter event attendees by. Currently is filtering event names using StartsWith criteria</param>
        /// <response code="200">An array of event attendees wrapped in success response object</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [SwaggerOperation("GetEventAttendeesList")]
        [ProducesResponseType(typeof(OkListResponse<AttendeeDto>), 200)]
        public IActionResult GetEventAttendeesList([FromRoute(Name = "event_uid")] Guid? eventUid,
            [FromQuery] string filter = "")
        {
            Check.Value(eventUid, "event_uid", AvendErrors.NotFound)
                .NotNull();

            using (var db = GetDatabaseService())
            {
                var eventId = db.EventsTable
                    .Where(events => events.Uid == eventUid).Select(events => events.Id)
                    .FirstOrDefault();

                Check.Value(eventId, "event_uid", AvendErrors.NotFound)
                    .NotEqualsTo(0);

                var attendees = db.Attendees
                    .Include(x => x.Values)
                    .ThenInclude(v => v.AttendeeCategoryOption)
                    .ThenInclude(o => o.AttendeeCategory)
                    .Include(x => x.Values)
                    .ThenInclude(v => v.Category)
                    .Where(x => x.EventId == eventId && (x.FirstName + " " + x.LastName).Contains(filter));

                var responseObj = new OkListResponse<AttendeeDto>()
                {
                    Data = attendees
                        .Select(x => AttendeeDto.From(x, eventUid))
                        .ToList(),
                    TotalFilteredRecords = attendees.Count(),
                };

                return Ok(responseObj);
            }
        }

        /// <summary>
        /// Returns filtered list of event attendees for given event uid.
        /// </summary>
        /// <remarks>Returns filtered list of event attendees according to filter and other parameters\n</remarks>
        /// <param name="eventUid">Uid of the event to get attendees for</param>
        /// <param name="filter">Category data</param>
        /// <param name="pageNumber"></param>
        /// <param name="recordsPerPage"></param>
        /// <param name="sortField"></param>
        /// <param name="sortOrder"></param>
        /// <response code="200">An array of event attendees wrapped in success response object</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost("filter")]
        [SwaggerOperation("GetEventAttendeesList")]
        [ProducesResponseType(typeof(OkListResponse<AttendeeDto>), 200)]
        public IActionResult GetEventAttendeesFilteredList(
            [FromRoute(Name = "event_uid")] Guid eventUid,
            [FromBody] AttendeesFilterRequestDTO filter,
            [FromQuery(Name = "page")] int pageNumber = 0,
            [FromQuery(Name = "per_page")] int recordsPerPage = 25,
            [FromQuery(Name = "sort_field")] string sortField = "first_name",
            [FromQuery(Name = "sort_order")] string sortOrder = "asc"
        )
        {
            if (filter == null)
            {
                filter = new AttendeesFilterRequestDTO();
            }

            Logger.LogDebug("Filter attendees with {filter}", filter);

            using (var db = GetDatabaseService())
            {
                var eventId = db.EventsTable
                    .Where(events => events.Uid == eventUid)
                    .Select(events => events.Id)
                    .FirstOrDefault();

                Check.Value(eventId, "event_uid", AvendErrors.NotFound)
                    .NotEqualsTo(0);

                var searchQuery = new SearchQueryParams(
                    filter.Query,
                    sortField,
                    sortOrder,
                    pageNumber,
                    recordsPerPage);
                searchQuery.ApplyDefaultSortOrder<AttendeeRecord>();
                searchQuery.Validate().Throw();

                var search = DefaultSearch.Start(searchQuery, db.Attendees);
                // ReSharper disable AccessToDisposedClosure
                search.Filter(attendees =>
                {
                    attendees = attendees.Where(x => x.EventId == eventId);
                    if (filter.Categories != null)
                    {
                        var requestedOptions = filter.Categories.SelectMany(_ => _.Values).ToList();
                        var filterOptions = db.AttendeeCategoryOptions
                            .Where(x => requestedOptions.Contains(x.Uid))
                            .Select(
                                x =>
                                    new
                                    {
                                        CategoryUid = x.AttendeeCategory.Uid,
                                        CategoryId = x.AttendeeCategory.Id,
                                        OptionUid = x.Uid,
                                        OptionId = x.Id
                                    })
                            .ToList()
                            .Where(x =>
                            {
                                //Sanity check - filter out selected values which are not present in filter
                                return filter.Categories
                                    .Any(_ => _.Uid == x.CategoryUid && _.Values.Contains(x.OptionUid));
                            })
                            .ToList();

                        //map from Category Uuid to list of options id
                        foreach (var category in filter.Categories)
                        {
                            var categoryOptions = filterOptions
                                .Where(x => x.CategoryUid == category.Uid)
                                .ToList();
                            if (categoryOptions.Count > 0)
                            {
                                var categoryId = categoryOptions[0].CategoryId;
                                var validOptions = categoryOptions.Select(x => new long?(x.OptionId)).ToList();
                                attendees = attendees
                                    .Where(x => x.Values
                                        .Any(v => v.Category.Id == categoryId &&
                                                  validOptions.Contains(v.CategoryOptionId)));
                            }
                        }
                    }
                    return attendees;
                });
                // ReSharper restore AccessToDisposedClosure
                var result = search.Paginate((a) => AttendeeDto.From(a, eventUid));
                return Ok(OkResponse.FromSearchResult(result));

//                try
//                {
//                                            
//                    var filteredEventAttendeesQuery = await GetFilteredWithCategorySubqueriesEventAttendeesQuery(db, eventId, filter);
//                    
//                    Logger.LogError($"Filtered attendees query preparation took {stopWatch.ElapsedMilliseconds}ms");
//
//                    var filteredEventAttendeesCount =
//                        ConstructRecordsListQuery(filteredEventAttendeesQuery, null, null, null, true).Count();
//
//                    //var filteredEventAttendeesCount = filteredEventAttendeesQuery.Count();
//
//                    Logger.LogError($"Filtered attendees after count request took {stopWatch.ElapsedMilliseconds}ms");
//
//                    filteredEventAttendeesQuery = ConstructRecordsListQuery(filteredEventAttendeesQuery, pageNumber,
//                        recordsPerPage, SortDictionary[sortField], sortOrder.ToLower() != "desc");
//
//                    var eventAttendeesList = filteredEventAttendeesQuery.ToList();
//
//                    Logger.LogError(
//                        $"Filtered attendees after list materialization took {stopWatch.ElapsedMilliseconds}ms");
//
//                    var eventAttendeeDtos =
//                        eventAttendeesList.Select(record => AttendeeDto.From(record, eventUid)).ToList();
//
//                    Logger.LogError($"Filtered attendees after DTO conversion took {stopWatch.ElapsedMilliseconds}ms");
//
//                    var responseObj = new OkListResponse<AttendeeDto>()
//                    {
//                        Data = eventAttendeeDtos,
//                        TotalFilteredRecords = filteredEventAttendeesCount,
//                    };
//
//                    return Ok(responseObj);
//                }
//                catch (Exception ex)
//                {
//                    Logger.LogCritical($"AttendeesController - Unexpected issue: {ex.Message}");
//
//                    Console.WriteLine(ex);
//
//                    return BadRequest(new ErrorResponse(new List<Error>()
//                    {
//                        new Error()
//                        {
//                            Code = "exception",
//                            Fields = new List<string>(),
//                            Message = JsonConvert.SerializeObject(ex),
//                        }
//                    }));
//                }
//                finally
//                {
//                    stopWatch.Stop();
//                    Logger.LogError($"Filtered attendees processing took {stopWatch.ElapsedMilliseconds}ms");
//                }
            }
        }

        // ReSharper disable once UnusedMember.Local
        private static IQueryable<AttendeeRecord> GetFilteredEventAttendeesNoCategoriesQuery(AvendDbContext db,
            long eventId, AttendeesFilterRequestDTO attendeeFilter)
        {
            var eventAttendeesWithAllData = db.Attendees
                .Include(attendeeObj => attendeeObj.Values)
                .ThenInclude(valueObj => valueObj.AttendeeCategoryOption)
                .ThenInclude(valueObj => valueObj.AttendeeCategory);

            return from eventAttendee in eventAttendeesWithAllData
                where eventAttendee.EventId == eventId
                      && (eventAttendee.FirstName.StartsWith(attendeeFilter.Query ?? string.Empty)
                          || eventAttendee.LastName.StartsWith(attendeeFilter.Query ?? string.Empty)
                          || eventAttendee.Company.StartsWith(attendeeFilter.Query ?? string.Empty)
                      )
                select eventAttendee;
        }

        // ReSharper disable once UnusedMember.Local
        private static IQueryable<AttendeeRecord> GetFilteredEventAttendeesQuery(AvendDbContext db, long eventId,
            AttendeesFilterRequestDTO attendeeFilter)
        {
            var eventAttendeesWithAllData = db.Attendees
                .Include(attendeeObj => attendeeObj.Values)
                .ThenInclude(valueObj => valueObj.AttendeeCategoryOption)
                .ThenInclude(valueObj => valueObj.AttendeeCategory);

            return from eventAttendee in eventAttendeesWithAllData
                where eventAttendee.EventId == eventId
                      && (eventAttendee.FirstName.StartsWith(attendeeFilter.Query ?? string.Empty)
                          || eventAttendee.LastName.StartsWith(attendeeFilter.Query ?? string.Empty)
                          || eventAttendee.Company.StartsWith(attendeeFilter.Query ?? string.Empty)
                      )
                      && (from categoryValue in eventAttendee.Values
                          where categoryValue.AttendeeId == eventAttendee.Id
                                &&
                                attendeeFilter.Categories.SelectMany(x => x.Values)
                                    .Contains(categoryValue.AttendeeCategoryOption == null
                                        ? categoryValue.AttendeeCategoryOption.Uid
                                        : Guid.Empty)
                          select categoryValue).Any()
                select eventAttendee;
        }

        // ReSharper disable once UnusedMember.Local
        private async Task<IQueryable<AttendeeRecord>> GetFilteredWithCategorySubqueriesEventAttendeesLinq(
            AvendDbContext db, long eventId, AttendeesFilterRequestDTO attendeeFilter)
        {
            var query = attendeeFilter.Query ?? string.Empty;

            var categoryUidsMappingQuery = from attendeeCategory in db.AttendeeCategories
                where attendeeFilter.Categories
                    .Select(filterCategoryValues => filterCategoryValues.Uid)
                    .Contains(attendeeCategory.Uid)
                select new {attendeeCategory.Id, attendeeCategory.Uid};

            Logger.LogWarning("The SQL for mapping the property Uids to Ids " +
                              JsonConvert.SerializeObject(categoryUidsMappingQuery.ToList()));

            var categoriesMapping = new Dictionary<Guid, long>();

            await categoryUidsMappingQuery.ForEachAsync(record => categoriesMapping.Add(record.Uid, record.Id));

            var eventAttendeesWithCategoryValues = db.Attendees
                .Include(attendeeObj => attendeeObj.Values);

            //  SQL request idea:
            //  q = SELECT FROM event_attendees AS ea WHERE 
            //    foreach (var filterCategoryValues in AttendeesFilterRequestDTO.Categories)
            //      q .= (?AND?) ea.attendee_id IN (
            //          q .= SELECT attendee_id FROM attendee_category_values WHERE 
            //          and ea.category_id=<filterCategoriesMappingtoIds[filterCategoryValues.Uid]> 
            //          and ea.option_id in (<filterOptionsMappingtoIds[filterCategoryValues.Uid)])

            return from eventAttendee in eventAttendeesWithCategoryValues
                where eventAttendee.EventId == eventId
                      && (eventAttendee.FirstName.StartsWith(query)
                          || eventAttendee.LastName.StartsWith(query)
                          || eventAttendee.Company.StartsWith(query)
                      )
                      && attendeeFilter.Categories.TrueForAll(categoryFilterData =>
                          (from categoryValue in eventAttendee.Values
                              where categoryValue.AttendeeId == eventAttendee.Id
                                    && categoryValue.CategoryId == categoriesMapping[categoryFilterData.Uid]
                                    && categoryFilterData.Values.Contains(categoryValue.AttendeeCategoryOption.Uid)
                              select categoryValue).Any())
                select eventAttendee;
        }

        // ReSharper disable once UnusedMember.Local
        private async Task<IQueryable<AttendeeRecord>> GetFilteredWithCategorySubqueriesEventAttendeesQuery(
            AvendDbContext db, long eventId, AttendeesFilterRequestDTO attendeeFilter)
        {
            var categoryUids = attendeeFilter.Categories.Select(filterCategoryValues => filterCategoryValues.Uid);
            var categoryUidsMappingQuery = from attendeeCategory in db.AttendeeCategories
                where categoryUids.Contains(attendeeCategory.Uid)
                select new {attendeeCategory.Id, attendeeCategory.Uid};

            Logger.LogWarning("The SQL for mapping the property Uids to Ids\n" +
                              JsonConvert.SerializeObject(categoryUidsMappingQuery.ToList(), Formatting.Indented));

            var categoriesMapping = new Dictionary<Guid, long>();

            var optionUids = new List<Guid>();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Logger.LogError(
                $"Start tracking time of GetFilteredWithCategorySubqueriesEventAttendeesQuery:{stopwatch.ElapsedMilliseconds}");

            attendeeFilter.Categories.ForEach(record => optionUids.AddRange(record.Values));

            Logger.LogError($"Time elapsed by preparing property value UIDs is:{stopwatch.ElapsedMilliseconds}");

            await categoryUidsMappingQuery.ForEachAsync(record => categoriesMapping.Add(record.Uid, record.Id));

            Logger.LogError($"Time elapsed by querying property IDs is:{stopwatch.ElapsedMilliseconds}");

            var categoryIds = categoriesMapping.Values;
            var optionUidsMappingQuery = (from attendeeOption in db.AttendeeCategoryOptions
                    where categoryIds.Contains(attendeeOption.CategoryId)
                          && optionUids.Contains(attendeeOption.Uid)
                    select new {attendeeOption.Id, attendeeOption.Uid})
                .Distinct();

            var optionIds = new List<long?>();
            var optionMappings = new Dictionary<Guid, long>();

            Logger.LogError(
                $"Time elapsed by preparing query for property option data is:{stopwatch.ElapsedMilliseconds}");

            try
            {
                await optionUidsMappingQuery.ForEachAsync(record =>
                {
                    optionIds.Add(record.Id);
                    optionMappings[record.Uid] = record.Id;
                });

                Logger.LogError(
                    $"Time elapsed by querying for property option data [count={optionMappings.Count}] is:{stopwatch.ElapsedMilliseconds}");
            }
            catch (Exception ex)
            {
                Logger.LogCritical($"AttendeesController - Unexpected issue: {ex.Message}");

                Console.WriteLine(ex);
            }

            var eventAttendeesWithCategoryValues = db.Attendees
                    .Include(attendeeObj => attendeeObj.Values)
                    .ThenInclude(categoryValueObj => categoryValueObj.AttendeeCategoryOption)
                    .ThenInclude(categoryOptionObj => categoryOptionObj.AttendeeCategory)
                ;

            var attendeesForEventQuery = eventAttendeesWithCategoryValues.Where(
                eventAttendee => eventAttendee.EventId == eventId
            );

            if (!string.IsNullOrWhiteSpace(attendeeFilter.Query))
            {
                attendeesForEventQuery = attendeesForEventQuery.Where(
                    eventAttendee => eventAttendee.FirstName.StartsWith(attendeeFilter.Query)
                                     ||
                                     eventAttendee.LastName.StartsWith(attendeeFilter.Query)
                                     ||
                                     eventAttendee.Company.StartsWith(attendeeFilter.Query)
                );
            }

            var attendeesQuery = attendeesForEventQuery;

            attendeeFilter.Categories.ForEach(categoryFilterData =>
            {
                var categoryId = categoriesMapping[categoryFilterData.Uid];

                attendeesQuery = attendeesQuery.Where(eventAttendee => (
                        from categoryValue in eventAttendee.Values
                        where categoryValue.AttendeeId == eventAttendee.Id
                              && categoryValue.CategoryId == categoryId
                              && optionIds.Contains(categoryValue.CategoryOptionId)
                        select categoryValue).Any()
                );
            });

            Logger.LogError($"Time elapsed by preparing the final query is:{stopwatch.ElapsedMilliseconds}");

            return attendeesQuery;
        }

        /// <summary>
        /// Adds a new event attendee to the given event
        /// </summary>
        /// <remarks>Adds a new event attendee based on parameters. Returns uid for newly created event attendee.</remarks>
        /// <param name="eventUid">Uid of the event to assign attendee to</param>
        /// <param name="newAttendeeDto">Event attendee object to be added to the database</param>
        /// <response code="200">New record was created successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost]
        [SwaggerOperation("CreateEventAttendee")]
        [ProducesResponseType(typeof(OkResponse<Guid>), 200)]
        public async Task<IActionResult> CreateEventAttendee(
            [FromRoute(Name = "event_uid")] Guid? eventUid,
            [FromBody] AttendeeDto newAttendeeDto)
        {
            if (!UserUid.HasValue)
                return UnauthorizedWithCodeAndBody(401, ErrorResponse.GenerateInvalidUser("user_uid"));

            if (eventUid == null)
            {
                return NotFound(ErrorResponse.GenerateRequiredParameter(typeof(EventRecord), "event_uid"));
            }

            using (var db = GetDatabaseService())
            {
                var eventRecord = (from recEventRecord in db.EventsTable
                    where recEventRecord.Uid == eventUid
                    select recEventRecord).FirstOrDefault();

                if (eventRecord == null)
                {
                    return NotFound(ErrorResponse.GenerateNotFound(typeof(EventRecord), eventUid.Value, "event_uid"));
                }

                var attendeeObj = new AttendeeRecord()
                {
                    Uid = Guid.NewGuid(),
                    EventId = eventRecord.Id,
                };

                newAttendeeDto.UpdateEventAttendee(attendeeObj);

                ErrorResponse errorResponse = null;

                foreach (var recordCat in newAttendeeDto.CategoryValues)
                {
                    var attendeeCategory = (from recAttendeeCategory in db.AttendeeCategories
                        where recAttendeeCategory.Uid == recordCat.CategoryUid
                              && recAttendeeCategory.EventId == eventRecord.Id
                        select recAttendeeCategory).FirstOrDefault();

                    if (attendeeCategory == null)
                    {
                        errorResponse = ErrorResponse.GenerateNotFound(typeof(AttendeeCategoryRecord), recordCat.CategoryUid,
                            "category_uid");
                        continue;
                    }

                    var attendeeCategoryOption = (from recCategoryOption in db.AttendeeCategoryOptions
                        where recCategoryOption.Uid == recordCat.OptionUid
                              && recCategoryOption.CategoryId == attendeeCategory.Id
                        select recCategoryOption).FirstOrDefault();

                    if (attendeeCategoryOption == null)
                    {
                        errorResponse = ErrorResponse.GenerateNotFound(typeof(AttendeeCategoryRecord), recordCat.CategoryUid,
                            "category_uid");
                        continue;
                    }

                    var attendeeCategoryValue = new AttendeeCategoryValue
                    {
                        CategoryId = attendeeCategory.Id,
                        CategoryOptionId = attendeeCategoryOption.Id,
                    };

                    attendeeObj.Values.Add(attendeeCategoryValue);
                }

                if (errorResponse != null)
                {
                    return NotFound(errorResponse);
                }

                db.Attendees.Add(attendeeObj);

                await db.SaveChangesAsync();

                //db.Attendees.Co

                var responseObj = new OkResponse<Guid>()
                {
                    Data = attendeeObj.Uid,
                };

                return Ok(responseObj);
            }
        }

        /// <summary>
        /// Updates given event attendee in the given event
        /// </summary>
        /// <remarks>Updates event attendee based on parameters. Returns updated record.</remarks>
        /// <param name="eventUid">Uid of the event to assign attendee to</param>
        /// <param name="attendeeUid"></param>
        /// <param name="dto">Event attendee object to be updated in the database</param>
        /// <response code="200">Record was updated successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPut("{attendee_uid}")]
        [SwaggerOperation("UpdateEventAttendee")]
        [ProducesResponseType(typeof(OkResponse<AttendeeRecord>), 200)]
        public async Task<IActionResult> UpdateEventAttendee(
            [FromRoute(Name = "event_uid")] Guid? eventUid,
            [FromRoute(Name = "attendee_uid")] Guid? attendeeUid,
            [FromBody] AttendeeDto dto)
        {
            if (!UserUid.HasValue)
                return UnauthorizedWithCodeAndBody(401, ErrorResponse.GenerateInvalidUser("user_uid"));

            if (eventUid == null)
            {
                return NotFound(ErrorResponse.GenerateRequiredParameter(typeof(EventRecord), "event_uid"));
            }

            if (!attendeeUid.HasValue)
                return
                    BadRequest(ErrorResponse.GenerateInvalidParameter(typeof(AttendeeRecord), "",
                        "Cannot parse GUID from value", "attendeeUid"));

            if (dto == null)
                return
                    BadRequest(ErrorResponse.GenerateInvalidParameter(typeof(AttendeeRecord), attendeeUid.Value,
                        "Invalid updated event attendee record", "attendee"));

            if (dto.Uid != attendeeUid)
                return
                    BadRequest(ErrorResponse.GenerateInvalidParameter(typeof(AttendeeRecord), attendeeUid.Value,
                        $"Uid of the attendee cannot be updated (new value {dto.Uid})",
                        "attendee_uid"));

            using (var db = GetDatabaseService())
            {
                var @event = db.EventsTable.FirstOrDefault(x => x.Uid == eventUid);
                if (@event == null)
                {
                    return NotFound(ErrorResponse.GenerateNotFound(typeof(EventRecord), eventUid.Value, "event_uid"));
                }

                var attendee = db.Attendees
                    .Include(x => x.Values)
                    .ThenInclude(x => x.AttendeeCategoryOption)
                    .ThenInclude(x => x.AttendeeCategory)
                    .FirstOrDefault(x => x.Uid == attendeeUid);

                if (attendee == null)
                {
                    return
                        NotFound(ErrorResponse.GenerateNotFound(typeof(AttendeeRecord), attendeeUid.Value,
                            "attendee_uid"));
                }

                foreach (var propertyValue in attendee.Values)
                {
                    db.AttendeeCategoryValues.Remove(propertyValue);
                }
                attendee.Values = new List<AttendeeCategoryValue>();
                dto.UpdateEventAttendee(attendee);

                ErrorResponse errorResponse = null;
                foreach (var value in dto.CategoryValues)
                {
                    var property = db.AttendeeCategories
                        .FirstOrDefault(x => x.Uid == value.CategoryUid && x.EventId == @event.Id);
                    if (property == null)
                    {
                        errorResponse = ErrorResponse.GenerateNotFound(typeof(AttendeeCategoryRecord), value.CategoryUid,
                            "category_uid");
                        break;
                    }
                    var propertyValue = db.AttendeeCategoryOptions
                        .FirstOrDefault(x => x.Uid == value.OptionUid && x.CategoryId == property.Id);
                    if (propertyValue == null)
                    {
                        errorResponse = ErrorResponse.GenerateNotFound(typeof(AttendeeCategoryRecord), value.CategoryUid,
                            "category_uid");
                        break;
                    }
                    attendee.Values.Add(new AttendeeCategoryValue
                    {
                        CategoryId = property.Id,
                        CategoryOptionId = propertyValue.Id,
                    });
                }

                if (errorResponse != null)
                {
                    return NotFound(errorResponse);
                }

                await db.SaveChangesAsync();

                var responseObj = new OkResponse<Guid>()
                {
                    Data = attendee.Uid,
                };

                return Ok(responseObj);
            }
        }

        /// <summary>
        /// Deletes event attendee by its uid
        /// </summary>
        /// <remarks>Returns true if successfull.\n</remarks>
        /// <param name="attendeeUidStr">Uid identifying the event attendee category </param>
        /// <response code="200">Record was deleted successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpDelete("{uid}")]
        [SwaggerOperation("DeleteEventAttendee")]
        [ProducesResponseType(typeof(OkResponse<string>), 200)]
        public async Task<IActionResult> DeleteEventAttendee([FromRoute(Name = "uid")] string attendeeUidStr)
        {
            if (!UserUid.HasValue)
                return UnauthorizedWithCodeAndBody(401, ErrorResponse.GenerateInvalidUser("user_uid"));

            Guid attendeeUid;
            var parseRes = Guid.TryParse(attendeeUidStr, out attendeeUid);

            if (!parseRes)
                return
                    BadRequest(ErrorResponse.GenerateInvalidParameter(typeof(AttendeeRecord), attendeeUidStr,
                        "Cannot parse GUID from value", "attendee_uid"));

            using (var db = GetDatabaseService())
            {
                var eventAttendeesWithCategoryValues =
                    db.Attendees.Include(attendeeObj => attendeeObj.Values)
                        .ThenInclude(valueObj => valueObj.AttendeeCategoryOption)
                        .ThenInclude(valueObj => valueObj.AttendeeCategory);

                var queryAttendees = from recAttendee in eventAttendeesWithCategoryValues
                    where recAttendee.Uid == attendeeUid
                    select recAttendee;

                var eventAttendee = queryAttendees.FirstOrDefault();

                if (eventAttendee == null)
                {
                    return NotFound(ErrorResponse.GenerateNotFound(typeof(AttendeeRecord), attendeeUid, "attendee_uid"));
                }

                foreach (var attendeeCategoryValue in eventAttendee.Values)
                {
                    db.AttendeeCategoryValues.Remove(attendeeCategoryValue);
                }

                await db.SaveChangesAsync();

                db.Attendees.Remove(eventAttendee);

                await db.SaveChangesAsync();

                var responseObj = new OkResponse<Guid>()
                {
                    Data = eventAttendee.Uid,
                };

                return Ok(responseObj);
            }
        }

        /// <summary>
        /// Generate required number of event attendees
        /// </summary>
        /// <remarks>Returns report indicating the number of added records\n</remarks>
        /// <param name="eventUid">Uid of the event to generate attendees for</param>
        /// <param name="count">Total number of event attendees to be added to the database</param>
        /// <response code="200">A report string</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost("massgenerate")]
        [SwaggerOperation("MassGenerateEventAttendees")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> MassGenerateEventAttendees([FromRoute(Name = "event_uid")] Guid? eventUid,
            [FromQuery] int count)
        {
            if (eventUid == null)
            {
                return NotFound(ErrorResponse.GenerateRequiredParameter(typeof(EventRecord), "event_uid"));
            }

            if (count <= 0)
                count = 1;

            var random = new Random();

            // ReSharper disable once UnusedVariable
            var next = random.Next();

            using (var db = GetDatabaseService())
            {
                var eventRecord = (from recEventRecord in db.EventsTable
                    where recEventRecord.Uid == eventUid
                    select recEventRecord).FirstOrDefault();

                if (eventRecord == null)
                {
                    return NotFound(ErrorResponse.GenerateNotFound(typeof(EventRecord), eventUid.Value, "event_uid"));
                }

                var faker = FakerHelper.AttendeesFaker(db, random, eventRecord.Id, eventRecord);

                var eventAttendees = faker.Generate(count);

                Logger.LogCritical("Generated attendees as folows:\n" + JsonConvert.SerializeObject(eventAttendees,
                                       new JsonSerializerSettings()
                                       {
                                           Formatting = Formatting.Indented,
                                           ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                           PreserveReferencesHandling = PreserveReferencesHandling.None,
                                       }));

                db.Attendees.AddRange(eventAttendees);

                await db.SaveChangesAsync();

                return Ok("Added " + count + " event attendee" + (count > 1 ? "s" : ""));
            }
        }

        /// <summary>
        /// Generate required number of event attendees
        /// </summary>
        /// <remarks>Returns report indicating the number of added records\n</remarks>
        /// <param name="eventUid">Uid of the event the attendee belongs to</param>
        /// <param name="attendeeUid">Uid of the attendee to generate categories for</param>
        /// <param name="count">Total number of event attendees to be added to the database</param>
        /// <response code="200">A report string</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost("{attendee_uid}/generate_value")]
        [SwaggerOperation("GenerateValue")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> GenerateValue(
            [FromRoute(Name = "event_uid")] Guid? eventUid,
            [FromRoute(Name = "attendee_uid")] Guid? attendeeUid,
            [FromQuery] int count)
        {
            if (!eventUid.HasValue)
            {
                return NotFound(ErrorResponse.GenerateRequiredParameter(typeof(EventRecord), "event_uid"));
            }

            if (!attendeeUid.HasValue)
            {
                return NotFound(ErrorResponse.GenerateRequiredParameter(typeof(EventRecord), "attendeeUid"));
            }

            if (count <= 0)
                count = 1;

            var random = new Random();

            // ReSharper disable once UnusedVariable
            var next = random.Next();

            using (var db = GetDatabaseService())
            {
                var @event = db.EventsTable
                    .Include(x => x.AttendeeCategories)
                    .ThenInclude(x => x.Options)
                    .FirstOrDefault(recEventRecord => recEventRecord.Uid == eventUid);

                if (@event == null)
                {
                    return NotFound(ErrorResponse.GenerateNotFound(typeof(EventRecord), eventUid.Value, "event_uid"));
                }

                var attendee = db.Attendees
                    .Include(x => x.Values)
                    .ThenInclude(x => x.AttendeeCategoryOption)
                    .ThenInclude(x => x.AttendeeCategory)
                    .FirstOrDefault(x => x.EventId == @event.Id && x.Uid == attendeeUid);

                if (attendee == null)
                {
                    return
                        NotFound(ErrorResponse.GenerateNotFound(typeof(AttendeeRecord), attendeeUid.Value,
                            "attendee_uid"));
                }

                var added = 0;
                while (added < count)
                {
                    Logger.LogCritical(
                        $"Trying to add value for property, available categories: {@event.AttendeeCategories.Count}");

                    var attendeeCategory =
                        @event.AttendeeCategories.Skip(random.Next(@event.AttendeeCategories.Count))
                            .Take(1)
                            .FirstOrDefault();

                    Logger.LogCritical("Trying to add value for category:\n" +
                                       JsonConvert.SerializeObject(attendeeCategory,
                                           new JsonSerializerSettings()
                                           {
                                               Formatting = Formatting.Indented,
                                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                               PreserveReferencesHandling = PreserveReferencesHandling.None,
                                           }));

                    if (attendeeCategory == null)
                        continue;

                    if (attendee.Values.Count ==
                        @event.AttendeeCategories.Sum(record => record.Options.Count))
                    {
                        Logger.LogCritical(
                            $"Trying to add value for category, but attendee already has all possible category values: {attendee.Values.Count}");

                        break;
                    }

                    if (
                        attendee.Values.Count(record => record.CategoryId == attendeeCategory.Id) ==
                        attendeeCategory.Options.Count)
                    {
                        Logger.LogCritical(
                            $"Trying to add value for category, but attendee already has all possible values for this category:\n{JsonConvert.SerializeObject(attendeeCategory, new JsonSerializerSettings() {Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore})}");

                        continue;
                    }

                    var attendeeCategoryValue = FakerHelper.AssignAttendeeCategoryValue(db, random, attendeeCategory,
                        attendee);

                    Logger.LogCritical(
                        $"Trying to add value for category, generated data:\n{JsonConvert.SerializeObject(attendeeCategoryValue, new JsonSerializerSettings() {Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore})}");

                    if (
                        !attendee.Values.Exists(
                            value => value.CategoryOptionId == attendeeCategoryValue.CategoryOptionId))
                    {
                        attendee.Values.Add(attendeeCategoryValue);

                        added++;
                    }
                }

                Logger.LogCritical("Generated attendee with categories as folows:\n" +
                                   JsonConvert.SerializeObject(attendee,
                                       new JsonSerializerSettings()
                                       {
                                           Formatting = Formatting.Indented,
                                           ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                           PreserveReferencesHandling = PreserveReferencesHandling.None,
                                       }));

                await db.SaveChangesAsync();

                return Ok("Added " + added + " event attendee category value" + (added > 1 ? "s" : ""));
            }
        }
    }
}