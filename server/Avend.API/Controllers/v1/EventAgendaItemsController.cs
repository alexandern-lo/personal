using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Avend.API.Helpers;
using Avend.API.Infrastructure.Responses;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Events;
using Avend.API.Services.Events.NetworkDTO;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Qoden.Validation;
using Swashbuckle.SwaggerGen.Annotations;
using Error = Avend.API.Infrastructure.Responses.Error;

namespace Avend.API.Controllers.v1
{
    /// <summary>
    /// Handles operations on event agenda items. 
    /// </summary>
    [Route("api/v1/events/{event_uid}/agenda_items")]
    [Authorize]
    public class EventAgendaItemsController : CrudController<EventAgendaItem, EventAgendaItemDTO>
    {
        private static readonly Dictionary<string, string> SortDictionary = new Dictionary<string, string>
        {
            {"name", QueryableExtensions.PropertyName<EventAgendaItem>(e => e.Name)},
            {"date", QueryableExtensions.PropertyName<EventAgendaItem>(e => e.Date)},
            {"location", QueryableExtensions.PropertyName<EventAgendaItem>(e => e.Location)},
            {"created_at", QueryableExtensions.PropertyName<EventAgendaItem>(e => e.CreatedAt)},
        };

        public AgendaItemsService AgendaItemsService;

        public EventAgendaItemsController(
            DbContextOptions<AvendDbContext> options,
            AgendaItemsService agendaItemsService
            )
            : base(options, SortDictionary)
        {
            Assert.Argument(options, nameof(options)).NotNull();
            Assert.Argument(agendaItemsService, nameof(agendaItemsService)).NotNull();

            AgendaItemsService = agendaItemsService;
        }

        [NonAction]
        protected override IQueryable<EventAgendaItem> GetEntityTableWithDependents(AvendDbContext db)
        {
            return db.EventAgendaItemsTable.Include(record => record.EventRecord);
        }

        /// <summary>
        /// Returns event agenda items by uid, for given event uid.
        /// </summary>
        /// 
        /// <remarks>Returns event agenda items by uid, for given event uid.</remarks>
        /// 
        /// <param name="eventUid">Uid of the event to get agenda item for</param>
        /// <param name="agendaItemUid">Uid of the agenda items to retrieve</param>
        /// 
        /// <response code="200">An array of event agenda items wrapped in success response object</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet("{agenda_item_uid}")]
        [SwaggerOperation("GetRecordByUid")]
        [ProducesResponseType(typeof(OkListResponse<EventAgendaItemDTO>), 200)]
        public IActionResult GetRecordByUid(
            [FromRoute(Name = "event_uid")] Guid? eventUid,
            [FromRoute(Name = "agenda_item_uid")] Guid? agendaItemUid)
        {
            var agendaItemDto = AgendaItemsService.FindByUid(eventUid, agendaItemUid);

            var responseObj = new OkResponse<EventAgendaItemDTO>()
            {
                Data = agendaItemDto,
            };

            return Ok(responseObj);

        }

        /// <summary>
        /// Returns filtered list of event agenda items for given event uid.
        /// </summary>
        /// 
        /// <remarks>Returns filtered list of event agenda items according to filter and other parameters\n</remarks>
        /// 
        /// <param name="eventUid">Uid of the event to get agenda items for</param>
        /// <param name="obsoleFilter">A string to filter event agenda items by. Currently is filtering event names using StartsWith criteria</param>
        /// 
        /// <response code="200">An array of event agenda items wrapped in success response object</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [SwaggerOperation("GetRecordsList")]
        [ProducesResponseType(typeof(OkListResponse<EventAgendaItemDTO>), 200)]
        public IActionResult GetRecordsList(
            [FromRoute(Name = "event_uid")] Guid? eventUid,
            [FromQuery(Name = "filter")] string obsoleFilter,
            [FromQuery(Name = "q")] string q,
            [FromQuery(Name = "date")] string dateStr,
            [FromQuery(Name = "page")] int pageNumber = 0,
            [FromQuery(Name = "per_page")] int recordsPerPage = 25,
            [FromQuery(Name = "sort_order")] string sortOrder = null,
            [FromQuery(Name = "sort_field")] string sortField = "start_time_ticks"
        )
        {
            if (q == null)
                q = obsoleFilter;

            if (sortField == "start_time")
                sortField = "start_time_ticks";
            if (sortField == "end_time")
                sortField = "end_time_ticks";

            Logger.LogInformation(
                $@"Trying to get all event agenda items with names starting with '{q}', no categories filtering");

            var queryParams = new SearchQueryParams(q, sortField, sortOrder, pageNumber, recordsPerPage);

            var searchResult = AgendaItemsService.FindAndPaginate(eventUid, dateStr, queryParams);

            return Ok(OkResponse.FromSearchResult(searchResult));
        }

        /// <summary>
        /// Returns filtered list of event agenda items for given event uid.
        /// </summary>
        /// 
        /// <remarks>Returns filtered list of event agenda items according to filter and other parameters\n</remarks>
        /// 
        /// <param name="eventUid">Uid of the event to get agenda items for</param>
        /// <param name="queryMethod">Optional query method attribute for the sake of testing</param>
        /// <param name="pageNumber"></param>
        /// <param name="recordsPerPage"></param>
        /// <param name="sortField"></param>
        /// <param name="sortOrder"></param>
        /// 
        /// <response code="200">An array of event agenda items wrapped in success response object</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost("agenda_filter")]
        [SwaggerOperation("GetEventAgendaItemsFilteredList")]
        [ProducesResponseType(typeof(OkListResponse<EventAgendaItemDTO>), 200)]
        public async Task<IActionResult> GetEventAgendaItemsFilteredList(
            [FromRoute(Name = "event_uid")] Guid? eventUid,
            [FromQuery] string queryMethod,
            [FromQuery(Name = "page")] int? pageNumber,
            [FromQuery(Name = "per_page")] int? recordsPerPage,
            [FromQuery(Name = "sort_field")] string sortField = "first_name",
            [FromQuery(Name = "sort_order")] string sortOrder = "asc"
        )
        {
            Logger.LogInformation($@"Trying to get all event agenda items using a category values filter");

            if (!UserUid.HasValue)
                return UnauthorizedWithCodeAndBody(401, ErrorResponse.GenerateInvalidUser("user_uid"));

            if (eventUid == null)
            {
                return NotFound(ErrorResponse.GenerateRequiredParameter(typeof(EventRecord), "event_uid"));
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            using (var db = GetDatabaseService())
            {
                var eventId = await GetEventIdFromUid(eventUid, db.EventsTable);

                if (eventId == 0)
                {
                    return NotFound(ErrorResponse.GenerateNotFound(typeof(EventRecord), eventUid.Value, "event_uid"));
                }

                try
                {
                    IQueryable<EventAgendaItem> agendaItemsWithDetails = db.EventAgendaItemsTable;

                    Logger.LogError($"Filtered agenda items query preparation took {stopWatch.ElapsedMilliseconds}ms");

                    var filteredEventAgendaItemsCount =
                        ConstructRecordsListQuery(agendaItemsWithDetails, null, null, null, true).Count();

                    //var filteredEventAgendaItemsCount = filteredEventAgendaItemsQuery.Count();

                    Logger.LogError($"Filtered agenda items after count request took {stopWatch.ElapsedMilliseconds}ms");

                    var filteredEventAgendaItemsQuery = ConstructRecordsListQuery(agendaItemsWithDetails, pageNumber,
                        recordsPerPage, SortDictionary[sortField], sortOrder.ToLower() != "desc");

                    var eventAgendaItemsList = filteredEventAgendaItemsQuery.ToList();

                    Logger.LogError(
                        $"Filtered agenda items after list materialization took {stopWatch.ElapsedMilliseconds}ms");

                    var eventAgendaItemDtos =
                        eventAgendaItemsList.Select(record => EventAgendaItemDTO.From(record, eventUid.Value)).ToList();

                    Logger.LogError($"Filtered agenda items after DTO conversion took {stopWatch.ElapsedMilliseconds}ms");

                    var responseObj = new OkListResponse<EventAgendaItemDTO>()
                    {
                        Data = eventAgendaItemDtos,
                        TotalFilteredRecords = filteredEventAgendaItemsCount,
                    };

                    return Ok(responseObj);
                }
                catch (Exception ex)
                {
                    Logger.LogCritical($"EventAgendaItemsController - Unexpected issue: {ex.Message}");

                    Console.WriteLine(ex);

                    return BadRequest(new ErrorResponse(new List<Error>()
                    {
                        new Error()
                        {
                            Code = "exception",
                            Fields = new List<string>(),
                            Message = JsonConvert.SerializeObject(ex),
                        }
                    }));
                }
                finally
                {
                    stopWatch.Stop();
                    Logger.LogError($"Filtered agenda items processing took {stopWatch.ElapsedMilliseconds}ms");
                }
            }
        }

        /// <summary>
        /// Adds a new event agenda item to the given event
        /// </summary>
        /// 
        /// <remarks>Adds a new event agenda item based on parameters. Returns uid for newly created event agenda item.</remarks>
        /// 
        /// <param name="eventUid">Uid of the event to assign agenda item to</param>
        /// <param name="newEventAgendaItemDto">Event agenda item object to be added to the database</param>
        /// 
        /// <response code="200">New record was created successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost]
        [SwaggerOperation("CreateRecord")]
        [ProducesResponseType(typeof(OkResponse<Guid>), 200)]
        public async Task<IActionResult> CreateRecord(
            [FromRoute(Name = "event_uid")] Guid? eventUid,
            [FromBody] EventAgendaItemDTO newEventAgendaItemDto)
        {
            var eventAgendaItemDTO = await AgendaItemsService.CreateRecord(eventUid, newEventAgendaItemDto);

            return Ok(OkResponse.WithData(eventAgendaItemDTO.Uid));
        }

        /// <summary>
        /// Updates given event agenda item in the given event
        /// </summary>
        /// 
        /// <remarks>Updates event agenda item based on parameters. Returns updated record.</remarks>
        /// 
        /// <param name="eventUid">Uid of the event to assign agenda item to</param>
        /// <param name="agendaItemUid">Uid of the agenda item to be updated</param>
        /// <param name="updatedEventAgendaItemDto">Event agenda item object to be updated in the database</param>
        /// 
        /// <response code="200">Record was updated successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPut("{agenda_item_uid}")]
        [SwaggerOperation("UpdateRecord")]
        [ProducesResponseType(typeof(OkResponse<EventAgendaItem>), 200)]
        public async Task<IActionResult> UpdateRecord(
            [FromRoute(Name = "event_uid")] Guid? eventUid,
            [FromRoute(Name = "agenda_item_uid")] Guid? agendaItemUid, [FromBody] EventAgendaItemDTO updatedEventAgendaItemDto)
        {
            var eventAgendaItemDTO = await AgendaItemsService.UpdateRecord(eventUid, agendaItemUid, updatedEventAgendaItemDto);

            return Ok(OkResponse.WithData(eventAgendaItemDTO.Uid));
        }

        /// <summary>
        /// Deletes event agenda item by its uid
        /// </summary>
        /// 
        /// <remarks>Returns UID of the deleted agenda item if successfull.\n</remarks>
        /// 
        /// <param name="eventUidStr">Uid identifying the event to delete agenda items from.</param>
        /// <param name="agendaItemUidStr">Uid identifying the event agenda item category </param>
        /// 
        /// <response code="200">Record was deleted successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpDelete("{agenda_item_uid}")]
        [SwaggerOperation("DeleteRecord")]
        [ProducesResponseType(typeof(OkResponse<Guid>), 200)]
        public async Task<IActionResult> DeleteRecord(
            [FromRoute(Name = "event_uid")] Guid? eventUidStr,
            [FromRoute(Name = "agenda_item_uid")] Guid? agendaItemUidStr)
        {
            var eventAgendaItemDTO = await AgendaItemsService.DeleteRecord(eventUidStr, agendaItemUidStr);

            return Ok(OkResponse.WithData(eventAgendaItemDTO.Uid));
        }

        /// <summary>
        /// Deletes several event agenda items with UIDs passed in body
        /// </summary>
        /// 
        /// <remarks>Returns count of deleted records if successfull.\n</remarks>
        /// 
        /// <param name="eventUidStr">Uid identifying the event to delete agenda items from.</param>
        /// <param name="massDeleteRequestDto">DTO containing Uids of all agenda items to delete.</param>
        /// 
        /// <response code="200">Records were deleted successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost("delete")]
        [SwaggerOperation("MassDeleteRecords")]
        [ProducesResponseType(typeof(OkResponse<long>), 200)]
        public async Task<IActionResult> MassDeleteRecords(
            [FromRoute(Name = "event_uid")] string eventUidStr,
            [FromBody] EventAgendaItemMassDeleteRequestDto massDeleteRequestDto)
        {
            var deletedCount = await AgendaItemsService.MassDeleteRecords(eventUidStr, massDeleteRequestDto);

            return Ok(OkResponse.WithData(deletedCount));
        }

        /// <summary>
        /// Generate required number of event agenda items
        /// </summary>
        /// 
        /// <remarks>Returns report indicating the number of added records\n</remarks>
        /// 
        /// <param name="eventUidStr">Uid of the event to generate agenda items for</param>
        /// <param name="count">Total number of event agenda items to be added to the database</param>
        /// 
        /// <response code="200">A report string</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost("/api/v1/events/{event_uid}/agenda_items/massgenerate")]
        [SwaggerOperation("MassGenerateEventAgendaItems")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> MassGenerateEventAgendaItems(
            [FromRoute(Name = "event_uid")] Guid? eventUidStr,
            [FromQuery] int count)
        {
            if (eventUidStr == null)
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
                var meetingEvent = (from recMeetingEvent in db.EventsTable
                    where recMeetingEvent.Uid == eventUidStr
                    select recMeetingEvent).FirstOrDefault();

                if (meetingEvent == null)
                {
                    return NotFound(ErrorResponse.GenerateNotFound(typeof(EventRecord), eventUidStr.Value, "event_uid"));
                }

                var faker = FakerHelper.AgendaItemsFaker(db, random, meetingEvent.Id, meetingEvent);

                var eventAgendaItems = faker.Generate(count);

                Logger.LogCritical("Generated agenda items as folows:\n" + JsonConvert.SerializeObject(eventAgendaItems,
                                       new JsonSerializerSettings()
                                       {
                                           Formatting = Formatting.Indented,
                                           ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                           PreserveReferencesHandling = PreserveReferencesHandling.None,
                                       }));

                db.EventAgendaItemsTable.AddRange(eventAgendaItems);

                await db.SaveChangesAsync();

                return Ok("Added " + count + " event agenda item" + (count > 1 ? "s" : ""));
            }
        }
    }
}