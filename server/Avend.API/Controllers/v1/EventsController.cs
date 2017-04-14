using System;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Logging;
using Avend.API.Infrastructure.Responses;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Swashbuckle.SwaggerGen.Annotations;
using Qoden.Validation;

namespace Avend.API.Controllers.v1
{
    /// <summary>
    /// CRUD Controller for managing events.
    /// </summary>
    [Route("api/v1/events")]
    [Authorize]
    public class EventsController : Controller
    {
        public EventsController(EventsService eventsService)
        {
            Assert.Argument(eventsService, nameof(eventsService)).NotNull();

            Logger = AvendLog.CreateLogger(nameof(EventsController));
            EventsService = eventsService;
        }

        public ILogger Logger { get; }
        public EventsService EventsService { get; }

        /// <summary>
        /// Returns array of records with just uids and names for all events, sorted by name.
        /// </summary>
        /// 
        /// <remarks>Returns just a couple of fields for each event but allows to get the whole events list easily\n</remarks>
        /// 
        /// <response code="200">An array of event uids and names wrapped in success response object</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet("uids_and_names")]
        [SwaggerOperation("GetEventUidsAndNamesList")]
        [ProducesResponseType(typeof(OkListResponse<MeetingEventUidAndNameDTO>), 200)]
        public IActionResult GetEventUidsAndNamesList()
        {
            var events = EventsService.GetSummary();
            return Ok(OkResponse.FromSearchResult(events));
        }

        /// <summary>
        /// Returns filtered list of events.
        /// </summary>
        /// <remarks>Returns filtered list of events according to filter and other parameters\n</remarks>
        /// <param name="obsoleFilter">Don't use</param>
        /// <param name="q">A string to filter events by. Currently is filtering only event names using StartsWith criteria</param>
        /// <param name="eventType">Type of the events to return (local/global). If omitted, return all events</param>
        /// <param name="tenant"></param>
        /// <param name="pageNumber">Zero-based page number</param>
        /// <param name="recordsPerPage">Records per page</param>
        /// <param name="sortField">Field to sort by</param>
        /// <param name="sortOrder">Sorting order (asc/desc)</param>
        /// <param name="range"></param>
        /// <param name="industry">event industry</param>
        /// <param name="startAfter">leave events which start after given date</param>
        /// <param name="endsBefore">leave events which ends before gievn date</param>
        /// <param name="scope">Event scope: available - all events visible to user, selectable - events which can be as lead events</param>
        /// <param name="user">event owner, available to SA/TA only</param>
        /// <response code="200">An array of events wrapped in success response object</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [SwaggerOperation("GetEventsList")]
        [ProducesResponseType(typeof(OkListResponse<EventDto>), 200)]
        public async Task<IActionResult> GetEventsList(
            [FromQuery(Name = "filter")] string obsoleFilter,
            [FromQuery(Name = "q")] string q,
            [FromQuery(Name = "event_type")] string eventType,
            [FromQuery(Name = "tenant")] Guid? tenant,
            [FromQuery(Name = "page")] int pageNumber = 0,
            [FromQuery(Name = "per_page")] int recordsPerPage = 25,
            [FromQuery(Name = "sort_order")] string sortOrder = null,
            [FromQuery(Name = "sort_field")] string sortField = "name",
            [FromQuery(Name = "range")] EventRange range = EventRange.All,
            [FromQuery(Name = "industry")] string industry = null,
            [FromQuery(Name = "start_after")] DateTime? startAfter = null,
            [FromQuery(Name = "end_before")] DateTime? endsBefore = null,
            [FromQuery(Name = "scope")] EventScope? scope = null,
            [FromQuery(Name = "for_user")] Guid? user = null
        )
        {
            if (q == null) q = obsoleFilter;
            var searchParams = new SearchQueryParams(q, sortField, sortOrder, pageNumber, recordsPerPage);
            var events = await EventsService.Find(
                searchParams,
                eventType,
                tenant,
                range,
                industry,
                startAfter,
                endsBefore,
                scope.GetValueOrDefault(EventScope.Avaialble),
                user);

            return Ok(OkResponse.FromSearchResult(events));
        }

        /// <summary>
        /// Returns event with given UID.
        /// </summary>
        /// <remarks>Returns filtered list of events according to filter and other parameters\n</remarks>
        /// <param name="eventUid">Event UID</param>
        /// <response code="200">An array of events wrapped in success response object</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet("{eventUid}")]
        [SwaggerOperation("GetEventByUid")]
        [ProducesResponseType(typeof(OkResponse<EventDto>), 200)]
        public IActionResult GetEventByUid([FromRoute] Guid eventUid)
        {
            var meetingEventDto = EventsService.GetEvent(eventUid);
            var responseObj = new OkResponse<EventDto>()
            {
                Data = meetingEventDto,
            };

            return Ok(responseObj);
        }

        /// <summary>
        /// Returns total amount of event expenses for the current user.
        /// </summary>
        /// 
        /// <remarks>Returns <see cref="MoneyDto"/> if successfull.\n</remarks>
        /// 
        /// <param name="eventUid">Uid identifying the event</param>
        /// 
        /// <response code="200">Amount was retrieved succesfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet("{eventUid}/expenses_total")]
        [SwaggerOperation]
        [ProducesResponseType(typeof(OkResponse<MoneyDto>), 200)]
        public async Task<IActionResult> GetEventExpensesByEventUid([FromRoute] Guid eventUid)
        {
            var amount = await EventsService.GetEventExpenses(eventUid);

            var responseObj = new OkResponse<MoneyDto>()
            {
                Data = amount,
            };

            return Ok(responseObj);
        }

        /// <summary>
        /// Adds a new event
        /// </summary>
        /// <remarks>Adds a new event based on parameters. Returns uid for newly created event.</remarks>
        /// <param name="newEventDto">Event object to be added to the database</param>
        /// <response code="200">New record was created successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost]
        [SwaggerOperation("CreateEvent")]
        [ProducesResponseType(typeof(OkResponse<EventDto>), 200)]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto newEventDto)
        {
            newEventDto = await EventsService.Create(newEventDto);
            return Ok(OkResponse.WithData(newEventDto));
        }

        /// <summary>
        /// Updates main fields of the event
        /// </summary>
        /// <remarks>Updates the event based on parameters. Returns updated event.</remarks>
        /// <param name="eventUid">Uid of the event to be updated</param>
        /// <param name="updatedEventDto">Fields of the event object to be changed in the database</param>
        /// <response code="200">Record was updated successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPut("{eventUid}")]
        [SwaggerOperation("UpdateEvent")]
        [ProducesResponseType(typeof(OkResponse<EventDto>), 200)]
        public async Task<IActionResult> UpdateEvent(
            [FromRoute] Guid eventUid,
            [FromBody] EventDto updatedEventDto)
        {
            var @event = await EventsService.Update(eventUid, updatedEventDto);
            return Ok(OkResponse.WithData(@event));
        }

        /// <summary>
        /// Deletes event by its uid
        /// </summary>
        /// <remarks>Returns true if successfull.\n</remarks>
        /// <param name="eventUid">Uid identifying the event</param>
        /// <response code="200">Record was deleted successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpDelete("{eventUid}")]
        [SwaggerOperation("DeleteEvent")]
        [ProducesResponseType(typeof(OkResponse<Guid>), 200)]
        public async Task<IActionResult> DeleteEvent([FromRoute] Guid eventUid)
        {
            await EventsService.Delete(eventUid);
            return Ok(OkResponse.WithData(eventUid));
        }

        /// <summary>
        /// Mass deletes events
        /// </summary>
        /// <remarks>Returns true if successfull</remarks>
        /// <param name="eventUids">Uid identifying the event</param>
        /// <response code="200">Records was deleted successfully</response>
        [HttpPost("delete")]
        [SwaggerOperation("MassDeleteEvents")]
        [ProducesResponseType(typeof(OkResponse<Guid>), 200)]
        public async Task<IActionResult> MassDeleteEvents([FromBody] Guid[] eventUids)
        {
            await EventsService.MassDelete(eventUids);
            return Ok();
        }


        /// <summary>
        /// Generate required number of events
        /// </summary>
        /// <remarks>Returns report indicating the number of added records\n</remarks>
        /// <param name="count">Total number of events to be added to the database</param>
        /// <param name="eventStartDate">Optional date to assign as start (and possibly end) date for all events</param>
        /// <param name="eventEndDate">Optional date to assign as end date for all events</param>
        /// <param name="minCategories">Minimum number of categories to assign to each event</param>
        /// <param name="cntAttendees">Exact number of attendees to assign to each event</param>
        /// <response code="200">A report string</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost("massgenerate")]
        [SwaggerOperation("MassGenerateEvents")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<OkObjectResult> MassGenerateEvents(
            [FromQuery] int count,
            [FromQuery] DateTime? eventStartDate,
            [FromQuery] DateTime? eventEndDate,
            [FromQuery] int minCategories,
            [FromQuery] int cntAttendees
        )
        {
            Logger.LogCritical("Started mass generating events:");
            var meetingEventsList = await EventsService.MassGenerate(count, eventStartDate, eventEndDate, minCategories,
                cntAttendees);
            return
                Ok(
                    $"Added {count} event{(count > 1 ? "s" : "")}\n{meetingEventsList.Select(record => record.Uid.ToString()).Aggregate((x, y) => x + "\n" + y)}");
        }
    }
}