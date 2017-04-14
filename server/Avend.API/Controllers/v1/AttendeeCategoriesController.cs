using System;
using System.Threading.Tasks;

using Avend.API.Infrastructure.Responses;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Events;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Qoden.Validation;

using Swashbuckle.SwaggerGen.Annotations;

namespace Avend.API.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/v1/events/{event_uid}/attendee_categories")]
    [Authorize]
    public class AttendeeCategoriesController : BaseController
    {
        private readonly AttendeeCategoriesService _service;

        public AttendeeCategoriesController(AttendeeCategoriesService service, DbContextOptions<AvendDbContext> options)
            : base(options)
        {
            Assert.Argument(service, nameof(service));
            _service = service;
        }

        /// <summary>
        /// Returns event attendee categories array
        /// </summary>
        /// <remarks>Returns filtered and sorted array of event categories according to filter and other parameters\n</remarks>
        /// <param name="eventUid">Event Guid parsed as string to properlyreport on parsing error.</param>
        /// <param name="perPage">hoe many records per page</param>
        /// <param name="page">page number</param>
        /// <param name="sortField">sort field, default 'name'</param>
        /// <param name="sortOrder">sort order, default 'asc'</param>
        /// <response code="200">An array of event attendee categories with their option values</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [SwaggerOperation("GetAttendeeCategories")]
        [ProducesResponseType(typeof(OkListResponse<AttendeeCategoryDto>), 200)]
        public IActionResult GetAttendeeCategories(
            [FromRoute(Name = "event_uid")] Guid eventUid,
            [FromQuery(Name = "page")] int page = 0,
            [FromQuery(Name = "per_page")] int perPage = 25,
            [FromQuery(Name = "sort_field")] string sortField = null,
            [FromQuery(Name = "sort_order")] string sortOrder = null)
        {
            var searchParams = new SearchQueryParams(null, sortField, sortOrder, page, perPage);
            var events = _service.Find(eventUid, searchParams);
            return Ok(OkResponse.FromSearchResult(events));
        }

        /// <summary>
        /// Retrieve event attendee category with given uid
        /// </summary>
        /// <remarks>Returns event attendee category record or error if the event attendee category is not found or is not accessible\n</remarks>
        /// <param name="eventUid">Guid identifying the event</param>
        /// <param name="categoryUid">Guid identifying the event attendee category</param>
        /// <response code="200">Event attendee category with given uid was found</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet("{category_uid}")]
        [SwaggerOperation("GetAttendeeCategoryByUID")]
        [ProducesResponseType(typeof(OkResponse<AttendeeCategoryDto>), 200)]
        public async Task<IActionResult> GetAttendeeCategoryByUid(
            [FromRoute(Name = "event_uid")] Guid eventUid,
            [FromRoute(Name = "category_uid")] Guid categoryUid)
        {
            var category = await _service.FindByUid(eventUid, categoryUid);
            return Ok(OkResponse.WithData(category));
        }

        /// <summary>
        /// Adds a new event attendee category
        /// </summary>
        /// <remarks>Adds a new event attendee category based on parameters. Returns uid for newly created event attendee category.</remarks>
        /// <param name="eventUid">Guid identifying the event</param>
        /// <param name="newAttendeeCategoryDto">Event attendee category object to be added to the database</param>
        /// <response code="200">New record was created successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost]
        [SwaggerOperation("CreateAttendeeCategory")]
        [ProducesResponseType(typeof(OkResponse<Guid>), 200)]
        public async Task<IActionResult> CreateAttendeeCategory(
            [FromRoute(Name = "event_uid")] Guid eventUid,
            [FromBody] AttendeeCategoryDto newAttendeeCategoryDto)
        {
            var result = await _service.Create(eventUid, newAttendeeCategoryDto);
            return Ok(OkResponse.WithData(result));
        }

        /// <summary>
        /// Updates the event attendee category identified by uid with new data
        /// </summary>
        /// <remarks>Updates the event attendee category based on parameters. Returns true on success.</remarks>
        /// <param name="eventUid">Guid identifying the event</param>
        /// <param name="categoryUid">Guid identifying the event attendee category</param>
        /// <param name="updatedCategoryDto">Updated event attendee category data</param>
        /// <response code="200">Record was updated successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPut("{category_uid}")]
        [SwaggerOperation("UpdateAttendeeCategory")]
        [ProducesResponseType(typeof(OkResponse<string>), 200)]
        public async Task<IActionResult> UpdateAttendeeCategory(
            [FromRoute(Name = "event_uid")] Guid eventUid,
            [FromRoute(Name = "category_uid")] Guid categoryUid,
            [FromBody] AttendeeCategoryDto updatedCategoryDto)
        {
            var result = await _service.Update(eventUid,  categoryUid, updatedCategoryDto);
            return Ok(OkResponse.WithData(result));
        }

        /// <summary>
        /// Deletes event attendee category by its uid
        /// </summary>
        /// <remarks>Returns true if successfull.\n</remarks>
        /// 
        /// <param name="eventUid">Uid identifying the event</param>
        /// <param name="uids">Uid identifying the event attendee category</param>
        /// 
        /// <response code="200">Record was deleted successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost("delete")]
        [SwaggerOperation("DeleteAttendeeCategory")]
        [ProducesResponseType(typeof(OkResponse<string>), 200)]
        public async Task<IActionResult> DeleteAttendeeCategory(
            [FromRoute(Name = "event_uid")] Guid eventUid,
            [FromBody] Guid [] uids)
        {
            var result = await _service.MassDelete(eventUid,  uids);
            return Ok(OkResponse.WithData(result));
        }
    }
}