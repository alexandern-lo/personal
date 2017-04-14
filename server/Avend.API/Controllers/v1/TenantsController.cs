using Avend.API.Infrastructure.Logging;
using Avend.API.Infrastructure.Responses;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Subscriptions;
using Avend.API.Services.Subscriptions.NetworkDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Swashbuckle.SwaggerGen.Annotations;

namespace Avend.API.Controllers.v1
{
    /// <summary>
    /// Tenants controller. 
    /// By now can only return tenants for superadmin users.
    /// </summary>
    [Authorize(Policy = Startup.AdmininsPolicy)]
    [Route("api/v1/tenants")]
    public class TenantsController : Controller
    {
        private ILogger<TenantsController> Logger;
        public SubscriptionsService SubscriptionsService { get; set; }

        public TenantsController(SubscriptionsService subscriptionsService)
        {
            SubscriptionsService = subscriptionsService;

            Logger = AvendLog.CreateLogger<TenantsController>();
        }

        /// <summary>
        /// Returns paginated list of tenants, filtered and sorted according to query parameters.
        /// </summary>
        /// 
        /// <remarks>Returns list of <see cref="TenantDto"/> objects.\n</remarks>
        /// 
        /// <response code="200">Succesfully constructed list</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not authorized</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [SwaggerOperation("GetRecords")]
        [ProducesResponseType(typeof(OkListResponse<TenantDto>), 200)]
        public OkListResponse<TenantDto> GetRecords(
            [FromQuery(Name = "q")] string filter = null,
            [FromQuery(Name = "page")] int pageNumber = 0,
            [FromQuery(Name = "per_page")] int recordsPerPage = 25,
            [FromQuery(Name = "sort_field")] string sortField = "company_name",
            [FromQuery(Name = "sort_order")] string sortOrder = "asc"
            )
        {
            Logger.LogInformation($"Retrieving tenants list filtered by '{filter}', elements {pageNumber * recordsPerPage}-{(pageNumber+1) * recordsPerPage} sorted by: {sortField} {sortOrder}");

            var searchQueryParams = new SearchQueryParams(filter, sortField, sortOrder, pageNumber, recordsPerPage);

            var tenants = SubscriptionsService.FindSubscriptions(searchQueryParams);

            return OkResponse.FromSearchResult(tenants);
        }
    }
}