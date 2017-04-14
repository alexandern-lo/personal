using System.Collections.Generic;
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
    /// 
    /// </summary>
    [Authorize]
    [Route("api/v1/events/{event_uid}/expenses")]
    public class EventUserExpensesController : BaseAuthenticatedController
    {
        protected EventUserExpensesService ExpensesService { get; }

        public EventUserExpensesController(
            EventUserExpensesService expensesService,
            ILogger<EventUserExpensesController> logger
            )
        {
            ExpensesService = expensesService;
        }

        /// <summary>
        /// Returns list of user expense records for the given event uid for the current user.
        /// </summary>
        /// 
        /// <remarks>Returns list of <see cref="EventUserExpenseDto"/> on successful processing.</remarks>
        /// 
        /// <response code="200">A success response containing list of <see cref="EventUserExpenseDto"/></response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [SwaggerOperation("GetRecordsList")]
        [ProducesResponseType(typeof(OkResponse<List<EventUserExpenseDto>>), 200)]
        public IActionResult GetRecordsList([FromRoute(Name = "event_uid")] string eventUidStr)
        {
            Logger.LogInformation("Trying to get list of own user expenses for event: " + eventUidStr);

            var result = ExpensesService.FindUserExpensesForCurrentUserByEventUid(eventUidStr);

            // ReSharper disable once PossibleInvalidOperationException
            var response = new OkResponse<List<EventUserExpenseDto>>()
            {
                Data = result,
            };

            return Ok(response);
        }

        /// <summary>
        /// Creates a new user expenses value for the chosen event.
        /// Sets value for the current user if no user_uid is passed over in dto.
        /// 
        /// Tenant admin can add expenses for another user by passing his user_uid in dto.
        /// </summary>
        /// 
        /// <remarks>Returns success response with DTO for created record on successful processing.</remarks>
        /// 
        /// <param name="eventUidStr">Uid of the event to add expense for</param>
        /// <param name="dto">DTO with data of the expense to be added</param>
        /// 
        /// <response code="200">DTO for the freshly added expense record</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost]
        [SwaggerOperation("CreateNewRecord")]
        [ProducesResponseType(typeof(OkResponse<EventUserExpenseDto>), 200)]
        public async Task<IActionResult> CreateNewRecord(
            [FromRoute(Name = "event_uid")] string eventUidStr, 
            [FromBody] EventUserExpenseDto dto
            )
        {
            Logger.LogInformation("Trying to create new user expense record:\n" + JsonConvert.SerializeObject(dto, Formatting.Indented));

            var createdDto = await ExpensesService.CreateUserExpense(eventUidStr, dto);

            // ReSharper disable once PossibleInvalidOperationException
            var response = new OkResponse<EventUserExpenseDto>()
            {
                Data = createdDto,
            };

            return Ok(response);
        }

        /// <summary>
        /// Updates user expenses value with given uid.
        /// Sets value for the current user if no user_uid is passed over in dto.
        /// 
        /// Tenant admin can add expenses for another user by passing his user_uid in dto.
        /// </summary>
        /// 
        /// <remarks>Returns success response with DTO for updated record on successful processing.</remarks>
        /// 
        /// <param name="eventUidStr">Uid of the event to add expense for</param>
        /// <param name="dto">DTO with data of the expense to be added</param>
        /// 
        /// <response code="200">DTO for the freshly added expense record</response>
        /// <response code="500">Unexpected error</response>
        [HttpPut("{event_user_expense_uid}")]
        [SwaggerOperation("UpdateRecord")]
        [ProducesResponseType(typeof(OkResponse<EventUserExpenseDto>), 200)]
        public async Task<IActionResult> UpdateRecord(
            [FromRoute(Name = "event_uid")] string eventUidStr,
            [FromBody] EventUserExpenseDto dto
            )
        {
            Logger.LogInformation("Trying to create new user expense record:\n" + JsonConvert.SerializeObject(dto, Formatting.Indented));

            var updatedDto = await ExpensesService.UpdateUserExpense(eventUidStr, dto);

            var response = new OkResponse<EventUserExpenseDto>()
            {
                Data = updatedDto,
            };

            return Ok(response);
        }
    }
}