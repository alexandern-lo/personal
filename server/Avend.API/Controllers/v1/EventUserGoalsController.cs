using System;
using System.Threading.Tasks;

using Avend.API.Infrastructure.Responses;
using Avend.API.Services.Events;
using Avend.API.Services.Events.NetworkDTO;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Swashbuckle.SwaggerGen.Annotations;

namespace Avend.API.Controllers.v1
{
    /// <summary>
    /// Controller responsible for operations on user goals.
    /// 
    /// It's very special because we have only a single goals record per each combination of 
    /// event and user, and thus UID of the record itself is required only on very special occasions.
    /// </summary>
    [Authorize]
    [Route("api/v1/events/{event_uid}/goals")]
    public class EventUserGoalsController : BaseAuthenticatedController
    {
        protected EventUserGoalsService Service { get; }

        public EventUserGoalsController(
            EventUserGoalsService service
            )
        {
            Service = service;
        }

        /// <summary>
        /// Returns user goals record for the given event uid.
        /// </summary>
        /// 
        /// <remarks>Returns <see cref="EventUserGoalsDto"/> on successful processing.</remarks>
        /// 
        /// <response code="200">A success response containing <see cref="EventUserGoalsDto"/></response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [SwaggerOperation("GetRecord")]
        [ProducesResponseType(typeof(OkResponse<EventUserGoalsDto>), 200)]
        public IActionResult GetRecord([FromRoute(Name = "event_uid")] string eventUidStr)
        {
            Logger.LogInformation("Trying to get user goals record for event: " + eventUidStr);

            var result = Service.FindUserGoalsByEventUid(eventUidStr);

            // ReSharper disable once PossibleInvalidOperationException
            var response = new OkResponse<EventUserGoalsDto>()
            {
                Data = result,
            };

            return Ok(response);
        }

        /// <summary>
        /// Returns user goals record for the given event uid and user uid.
        /// Access rights apply. Superadmin sees everyone's records.
        /// </summary>
        /// 
        /// <remarks>Returns <see cref="EventUserGoalsDto"/> on successful processing.</remarks>
        /// 
        /// <response code="200">A success response containing <see cref="EventUserGoalsDto"/></response>
        /// <response code="500">Unexpected error</response>
        [HttpGet("{user_uid}")]
        [SwaggerOperation("GetRecordByUserUid")]
        [ProducesResponseType(typeof(OkResponse<EventUserGoalsDto>), 200)]
        public IActionResult GetRecordByUserUid([FromRoute(Name = "event_uid")] string eventUidStr, [FromRoute(Name = "user_uid")] string userUidStr)
        {
            Logger.LogInformation("Trying to get user goals record for event: " + eventUidStr);

            var result = Service.FindUserGoalsByEventUidAndUserUid(eventUidStr, userUidStr);

            // ReSharper disable once PossibleInvalidOperationException
            var response = new OkResponse<EventUserGoalsDto>()
            {
                Data = result,
            };

            return Ok(response);
        }

        /// <summary>
        /// Returns dashboard object containing all data for seat user dashboard.
        /// </summary>
        /// 
        /// <remarks>Returns success response on successful processing.</remarks>
        /// 
        /// <response code="200">A empty response indicating success</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost]
        [SwaggerOperation("CreateNewRecord")]
        [ProducesResponseType(typeof(OkResponse<Guid>), 200)]
        public async Task<IActionResult> CreateNewRecord([FromRoute(Name = "event_uid")] string eventUidStr, [FromBody] EventUserGoalsDto dto)
        {
            Logger.LogInformation("Trying to create (or probably update) new user goals record:\n" + JsonConvert.SerializeObject(dto, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            }));

            var result = await Service.CreateOrUpdateUserGoal(eventUidStr, dto);

            // ReSharper disable once PossibleInvalidOperationException
            var response = new OkResponse<Guid>()
            {
                Data = result.Uid.Value,
            };

            return Ok(response);
        }

        /// <summary>
        /// Returns dashboard object containing all data for seat user dashboard.
        /// </summary>
        /// 
        /// <remarks>Returns success response on successful processing.</remarks>
        /// 
        /// <response code="200">A empty response indicating success</response>
        /// <response code="500">Unexpected error</response>
        [HttpPut]
        [SwaggerOperation("UpdateRecord")]
        [ProducesResponseType(typeof(OkResponse<Guid>), 200)]
        public async Task<IActionResult> UpdateRecord([FromRoute(Name = "event_uid")] string eventUidStr, [FromBody] EventUserGoalsDto dto)
        {
            Logger.LogInformation("Trying to update user goals record:\n" + JsonConvert.SerializeObject(dto, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            }));

            var result = await Service.UpdateUserGoal(eventUidStr, dto);

            // ReSharper disable once PossibleInvalidOperationException
            var response = new OkResponse<Guid>()
            {
                Data = result.Uid.Value,
            };

            return Ok(response);
        }
    }
}