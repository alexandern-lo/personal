using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using Avend.API.Infrastructure.Logging;
using Avend.API.Infrastructure.Responses;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Dashboard;
using Avend.API.Services.Dashboard.NetworkDTO;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Qoden.Validation;

using Swashbuckle.SwaggerGen.Annotations;

namespace Avend.API.Controllers.v1
{
    /// <summary>
    /// Endpoints for providing aggregated data to be displayed on dashboard pages / screens.
    /// </summary>
    [Authorize]
    [Route("api/v1/dashboard")]
    public class DashboardController : Controller
    {
        private ILogger<DashboardController> Logger;

        protected DashboardService DashboardService { get; }
        protected SuperadminDashboardService SuperadminDashboardService { get; }

        // ReSharper disable once SuggestBaseTypeForParameter
        /// <summary>
        /// Default constructor ready for Dependency Injection.
        /// </summary>
        /// 
        /// <param name="service">Dashboard service to be used.</param>
        /// <param name="superadminDashboardService">Superadmin dasboard service to be used.</param>
        public DashboardController(
            DashboardService service, 
            SuperadminDashboardService superadminDashboardService
            )
        {
            Assert.Argument(service).NotNull();
            Assert.Argument(superadminDashboardService).NotNull();

            DashboardService = service;
            SuperadminDashboardService = superadminDashboardService;

            Logger = AvendLog.CreateLogger<DashboardController>();
        }

        /// <summary>
        /// Returns dashboard object containing all data for seat user's dashboard.
        /// </summary>
        /// 
        /// <remarks>Returns specialized <see cref="DashboardDTO"/> wrapping the record lists and statistical data.</remarks>
        /// 
        /// <response code="200">An OkResponse object with specialized DashboardDTO as data, wrapping the event and resource data records lists as well as statistical data</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet("")]
        [SwaggerOperation("GetDashboard")]
        [ProducesResponseType(typeof(OkResponse<DashboardDTO>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetDashboard(
            [FromQuery(Name = "events_limit")] int? eventsLimit,
            [FromQuery(Name = "events_sort_field")] string eventsSortField,
            [FromQuery(Name = "events_sort_order")] string eventsSortOrder,
            [FromQuery(Name = "resources_limit")] int? resourcesLimit,
            [FromQuery(Name = "resources_sort_field")] string resourcesSortField,
            [FromQuery(Name = "resources_sort_order")] string resourcesSortOrder
            )
        {
            Logger.LogInformation(@"Trying to get dashboard");

            if (!resourcesLimit.HasValue || resourcesLimit <= 0)
                resourcesLimit = 5;
            if (!eventsLimit.HasValue || eventsLimit <= 0)
                eventsLimit = 5;

            // ReSharper disable once PossibleInvalidOperationException
            var response = new OkResponse<DashboardDTO>
            {
                Data = DashboardService.ProduceDashboardForUser(
                    eventsLimit.Value, eventsSortField, eventsSortOrder,
                    resourcesLimit.Value, resourcesSortField, resourcesSortOrder
                    ),
            };

            return Ok(response);
        }

        /// <summary>
        /// Returns history of leads acquired (daily for the given number of days in the past from today).
        /// </summary>
        /// 
        /// <remarks>Returns list of <see cref="DateIndexedTupleDto&lt;T&gt;" /> records ordered by date.</remarks>
        /// 
        /// <response code="200">An OkResponse object with list of DateIndexedTupleDTO&lt;decimal&gt; forming historical values for number of acquired leads per day</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet("leads_history/daily")]
        [SwaggerOperation("GetLeadsHistoryDaily")]
        [ProducesResponseType(typeof(OkResponse<List<DateIndexedTupleDto<decimal>>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public OkObjectResult GetLeadsHistoryDaily([FromQuery(Name = "limit")] int limit = 7)
        {
            Logger.LogInformation(@"Trying to get leads history, daily data");

            // ReSharper disable once PossibleInvalidOperationException
            var response = new OkResponse<IEnumerable<DateIndexedTupleDto<decimal>>>
            {
                Data = DashboardService.ProduceLeadsCountDailyHistoryForUser(limit),
            };

            return Ok(response);
        }

        /// <summary>
        /// Returns stats for each user in the tenant/subscription on total number of acquired leads and leads goal for the given events.
        /// </summary>
        /// 
        /// <remarks>Returns list of <see cref="UserTotalExpensesDto" /> records ordered by first and last name.</remarks>
        /// 
        /// <response code="200">An OkResponse object with list of total number of acquired leads per user.</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not authorized</response>
        /// <response code="500">Unexpected error</response>
        [Authorize(Policy = Startup.SubscriptionAdminPolicy)]
        [HttpPost("users/leads_goals")]
        [SwaggerOperation("GetTenantStatsForLeadsGoalsByUser")]
        [ProducesResponseType(typeof(OkResponse<List<UserTotalExpensesDto>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<OkObjectResult> GetTenantStatsForLeadsGoalsByUser(
            [FromBody] FilterByEventsRequestDTO eventsFilter
            )
        {
            Logger.LogInformation(@"Trying to get leads goals tenant stats grouped by user");

            var response = new OkResponse<IEnumerable<UserTotalLeadsGoalDto>>
            {
                Data = await DashboardService.GetUserGoalsFilteredByEvents(eventsFilter),
            };

            return Ok(response);
        }

        /// <summary>
        /// Returns stats for each user in the tenant/subscription on total expenses for given events.
        /// </summary>
        /// 
        /// <remarks>Returns list of <see cref="DateIndexedTupleDto&lt;T&gt;" /> records ordered by date.</remarks>
        /// 
        /// <response code="200">An OkResponse object with list total expenses by user</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not authorized</response>
        /// <response code="500">Unexpected error</response>
        [Authorize(Policy = Startup.SubscriptionAdminPolicy)]
        [HttpPost("users/expenses")]
        [SwaggerOperation("GetTenantStatsForExpensesByUser")]
        [ProducesResponseType(typeof(OkResponse<List<DateIndexedTupleDto<decimal>>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<OkObjectResult> GetTenantStatsForExpensesByUser(
            [FromBody] FilterByEventsRequestDTO eventsFilter
            )
        {
            Logger.LogInformation(@"Trying to get expenses tenant stats grouped by user");

            var response = new OkResponse<UserTotalExpensesListDto>
            {
                Data = await DashboardService.GetUserExpensesFilteredByEvents(eventsFilter),
            };

            return Ok(response);
        }

        /// <summary>
        /// Returns summary stats to be displayed on dashboard of superadmin users. 
        /// 
        /// Not available for lower access level users.
        /// </summary>
        /// 
        /// <remarks>Returns <see cref="SuperadminDashboardDto" /> for superadmin users.</remarks>
        /// 
        /// <response code="200">An OkResponse object with <see cref="SuperadminDashboardDto" /> data</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not authorized</response>
        /// <response code="500">Unexpected error</response>
        [Authorize(Policy = Startup.AdmininsPolicy)]
        [HttpGet("superadmin/summary")]
        [SwaggerOperation("GetSuperadminDashboardSummary")]
        [ProducesResponseType(typeof(OkResponse<SuperadminDashboardDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<OkObjectResult> GetSuperadminSummary()
        {
            Logger.LogInformation(@"Trying to gather data for Superadmin's dashboard summary widget");

            var dto = await SuperadminDashboardService.GetSummary();

            var response = new OkResponse<SuperadminDashboardDto>
            {
                Data = dto,
            };

            Logger.LogInformation(@"Finished gathering data for Superadmin's dashboard summary widget");

            return Ok(response);
        }

        /// <summary>
        /// Returns new users daily history to be displayed on dashboard of superadmin users. 
        /// 
        /// Not available for lower access level users.
        /// </summary>
        /// 
        /// <remarks>Returns list of <see cref="DateIndexedTupleDto{decimal}" /> for superadmin users.</remarks>
        /// 
        /// <response code="200">An OkResponse object with list of <see cref="DateIndexedTupleDto{decimal}" /> data</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not authorized</response>
        /// <response code="500">Unexpected error</response>
        [Authorize(Policy = Startup.AdmininsPolicy)]
        [HttpGet("superadmin/history/new_users/daily")]
        [SwaggerOperation("GetSuperadminNewUsersHistoryDaily")]
        [ProducesResponseType(typeof(OkResponse<DateIndexedTupleDto<decimal>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<OkObjectResult> GetSuperadminNewUsersHistoryDaily([FromQuery(Name = "limit")] int days = 7)
        {
            Logger.LogInformation(@"Trying to gather data for Superadmin's dashboard daily history on new users");

            var dto = await SuperadminDashboardService.GetNewUsersHistoryDaily(days);

            var response = new OkResponse<IEnumerable<DateIndexedTupleDto<decimal>>>
            {
                Data = dto,
            };

            Logger.LogInformation(@"Finished gathering data for Superadmin's dashboard daily history on new users");

            return Ok(response);
        }

        /// <summary>
        /// Returns new subscriptions daily history to be displayed on dashboard of superadmin users. 
        /// Can display data for trial or paid subscriptions depending on type parameter.
        /// 
        /// Not available for lower access level users.
        /// </summary>
        /// 
        /// <remarks>Returns list of <see cref="DateIndexedTupleDto{decimal}" /> for superadmin users.</remarks>
        /// 
        /// <response code="200">An OkResponse object with list of <see cref="DateIndexedTupleDto{decimal}" /> data</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not authorized</response>
        /// <response code="500">Unexpected error</response>
        [Authorize(Policy = Startup.AdmininsPolicy)]
        [HttpGet("superadmin/history/new_subscriptions/daily")]
        [SwaggerOperation("GetSuperadminNewSubscriptionsHistoryDaily")]
        [ProducesResponseType(typeof(OkResponse<DateIndexedTupleDto<decimal>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<OkObjectResult> GetSuperadminNewSubscriptionsHistoryDaily([FromQuery(Name = "type")] string type = "paid", [FromQuery(Name = "limit")] int days = 7)
        {
            Logger.LogInformation(@"Trying to gather data for Superadmin's dashboard daily history on new {0} subscriptions for {1} days", new object[] {type, days});

            var dto = await SuperadminDashboardService.GetNewSubscriptionsHistoryDaily(type, days);

            var response = new OkResponse<IEnumerable<DateIndexedTupleDto<decimal>>>
            {
                Data = dto,
            };

            Logger.LogInformation(@"Finished gathering data for Superadmin's dashboard daily history on new {0} subscriptions for {1} days", new object[] { type, days });

            return Ok(response);
        }

        /// <summary>
        /// Returns average leads per month history to be displayed on dashboard of superadmin users. 
        /// 
        /// Not available for lower access level users.
        /// </summary>
        /// 
        /// <remarks>Returns list of <see cref="DateIndexedTupleDto{decimal}" /> for superadmin users.</remarks>
        /// 
        /// <response code="200">An OkResponse object with list of <see cref="DateIndexedTupleDto{decimal}" /> data</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not authorized</response>
        /// <response code="500">Unexpected error</response>
        [Authorize(Policy = Startup.AdmininsPolicy)]
        [HttpGet("superadmin/history/average_leads/monthly")]
        [SwaggerOperation("GetSuperadminAverageLeadsHistoryMonthly")]
        [ProducesResponseType(typeof(OkResponse<DateIndexedTupleDto<decimal>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<OkObjectResult> GetSuperadminAverageLeadsHistoryMonthly([FromQuery(Name = "limit")] int months = 3)
        {
            Logger.LogInformation(@"Trying to gather data for Superadmin's dashboard monthly history on average leads for {0} months", months);

            var dto = await SuperadminDashboardService.GetAverageLeadsHistoryMonthly(months);

            var response = new OkResponse<IEnumerable<DateIndexedTupleDto<decimal>>>
            {
                Data = dto,
            };

            Logger.LogInformation(@"Finished gathering data for Superadmin's dashboard monthly history on average leads for {0} months", months);

            return Ok(response);
        }

        /// <summary>
        /// Returns average events per month history to be displayed on dashboard of superadmin users. 
        /// Can display data for trial or paid subscriptions depending on type parameter.
        /// 
        /// Not available for lower access level users.
        /// </summary>
        /// 
        /// <remarks>Returns list of <see cref="DateIndexedTupleDto{decimal}" /> for superadmin users.</remarks>
        /// 
        /// <response code="200">An OkResponse object with list of <see cref="DateIndexedTupleDto{decimal}" /> data</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not authorized</response>
        /// <response code="500">Unexpected error</response>
        [Authorize(Policy = Startup.AdmininsPolicy)]
        [HttpGet("superadmin/history/average_events/monthly")]
        [SwaggerOperation("GetSuperadminAverageEventsHistoryMonthly")]
        [ProducesResponseType(typeof(OkResponse<DateIndexedTupleDto<decimal>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<OkObjectResult> GetSuperadminAverageEventsHistoryMonthly([FromQuery(Name = "type")] string type = "all", [FromQuery(Name = "limit")] [DefaultValue(3)] int months = 3)
        {
            Logger.LogInformation(@"Trying to gather data for Superadmin's dashboard monthly history on average events [{1}] for {0} months", months, type);

            var dto = await SuperadminDashboardService.GetAverageEventsHistoryMonthly(type, months);

            var response = new OkResponse<IEnumerable<DateIndexedTupleDto<decimal>>>
            {
                Data = dto,
            };

            Logger.LogInformation(@"Finished gathering data for Superadmin's dashboard monthly history on average events [{1}] for {0} months", months, type);

            return Ok(response);
        }
    }
}