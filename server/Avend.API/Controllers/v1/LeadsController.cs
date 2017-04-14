using System;
using System.Net;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Responses;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Model;
using Avend.API.Services;
using Avend.API.Services.Exceptions;
using Avend.API.Services.Leads;
using Avend.API.Services.Leads.NetworkDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Swashbuckle.SwaggerGen.Annotations;

namespace Avend.API.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/v1/leads")]
    [Authorize]
    public class LeadsController : BaseController
    {
        public LeadsCrudService LeadsCrudService { get; }
        public LeadsExportToFileService LeadsExportToFileService { get; }
        public LeadsExportToCrmService LeadsExportToCrmService { get; }

        public ILogger logger { get; }

        public LeadsController(
            DbContextOptions<AvendDbContext> options,
            UsersManagementService usersManagementService,
            LeadsCrudService leadsCrudService,
            LeadsExportToFileService leadExportToFileService,
            LeadsExportToCrmService leadsExportToCrmService
        )
            : base(options)
        {
            LeadsCrudService = leadsCrudService;
            LeadsExportToFileService = leadExportToFileService;
            LeadsExportToCrmService = leadsExportToCrmService;
        }

        /// <summary>
        /// Returns leads array
        /// </summary>
        /// 
        /// <remarks>Returns filtered and sorted array of leads according to filter and other parameters\n</remarks>
        /// 
        /// <param name="subscriptionUid">Tenant Uid to filter by. Only available to Super Admins</param>
        /// <param name="filter">Query string to be matched by first or last name of the lead record.</param>
        /// <param name="pageNumber">Number of page to retrieve</param>
        /// <param name="recordsPerPage">Page size</param>
        /// <param name="sortField">Field to sort by</param>
        /// <param name="sortOrder">Sort order</param>
        /// 
        /// <response code="200">An array of leads</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [SwaggerOperation("GetLeads")]
        [ProducesResponseType(typeof(OkListResponse<LeadDto>), 200)]
        public IActionResult GetLeads(
            [FromQuery(Name = "tenant")] Guid? subscriptionUid,
            [FromQuery(Name = "filter")] string filter,
            [FromQuery(Name = "page")] int? pageNumber,
            [FromQuery(Name = "per_page")] int? recordsPerPage,
            [FromQuery(Name = "sort_field")] string sortField = "first_name",
            [FromQuery(Name = "sort_order")] string sortOrder = null
        )
        {
            Logger.LogInformation($@"Trying to get all leads with names starting with '{filter}'");

            var leadDtos = LeadsCrudService.FindBy(new SearchQueryParams()
            {
                SortField = sortField,
                SortOrder = sortOrder,
                Filter = filter ?? "",
                PageNumber = pageNumber ?? 0,
                RecordsPerPage = recordsPerPage ?? 10,
            }, subscriptionUid);

            return Ok(OkResponse.FromSearchResult(leadDtos));
        }

        /// <summary>
        /// Returns leads array
        /// </summary>
        /// 
        /// <remarks>Returns filtered and sorted array of leads according to filter and other parameters\n</remarks>
        /// 
        /// <param name="filter">Query string to be matched by first or last name of the lead record.</param>
        /// <param name="pageNumber">Number of page to retrieve</param>
        /// <param name="recordsPerPage">Page size</param>
        /// <param name="sortField">Field to sort by</param>
        /// <param name="sortOrder">Sort order</param>
        /// 
        /// <response code="200">An array of leads</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet("own")]
        [SwaggerOperation("GetLeads")]
        [ProducesResponseType(typeof(OkListResponse<LeadDto>), 200)]
        public IActionResult GetOwnLeads(
            [FromQuery(Name = "filter")] string filter,
            [FromQuery(Name = "page")] int? pageNumber,
            [FromQuery(Name = "per_page")] int? recordsPerPage,
            [FromQuery(Name = "sort_field")] string sortField = "first_name",
            [FromQuery(Name = "sort_order")] string sortOrder = null
        )
        {
            Logger.LogInformation($@"Trying to get all leads with names starting with '{filter}'");

            var leadDtos = LeadsCrudService.FindBy(new SearchQueryParams()
            {
                SortField = sortField,
                SortOrder = sortOrder,
                Filter = filter ?? "",
                PageNumber = pageNumber ?? 0,
                RecordsPerPage = recordsPerPage ?? 10,
            }, null);

            return Ok(OkResponse.FromSearchResult(leadDtos));
        }

        /// <summary>
        /// Retrieve recent activity for leads
        /// </summary>
        /// <remarks>Returns lead record or error if the lead is not found or is not accessible\n</remarks>
        /// <response code="200">Lead with given uid was found</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet("recent_activity")]
        [SwaggerOperation("GetLeadsRecentActivity")]
        [ProducesResponseType(typeof(OkResponse<LeadDto>), 200)]
        public IActionResult GetLeadsRecentActivity([FromQuery(Name = "limit")] int? limit)
        {
            var leadDTO = LeadsCrudService.GetRecentActivity(limit ?? 10);

            return Ok(new OkListResponse<LeadRecentActivityDto>()
            {
                Data = leadDTO,
            });
        }

        /// <summary>
        /// Retrieve lead with given uid
        /// </summary>
        /// <remarks>Returns lead record or error if the lead is not found or is not accessible\n</remarks>
        /// <param name="leadUid">uid identifying the lead</param>
        /// <response code="200">Lead with given uid was found</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet("{leadUid}")]
        [SwaggerOperation("GetLeadByUID")]
        [ProducesResponseType(typeof(OkResponse<LeadDto>), 200)]
        public async Task<IActionResult> GetLeadByUid([FromRoute] Guid leadUid)
        {
            var leadDTO = await LeadsCrudService.GetLeadByUid(leadUid);

            return Ok(new OkResponse<LeadDto>()
            {
                Data = leadDTO,
            });
        }

        /// <summary>
        /// Create a new lead.
        /// </summary>
        /// <remarks>Adds a new lead based on parameters. Returns uid for newly created lead.</remarks>
        /// <param name="newLeadDto">Lead object to be added to the database</param>
        /// <response code="200">New record was created successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost]
        [SwaggerOperation("CreateLead")]
        [ProducesResponseType(typeof(OkResponse<string>), 200)]
        public async Task<IActionResult> CreateLead([FromBody] LeadDto newLeadDto)
        {
            try
            {
                var leadDto = await LeadsCrudService.CreateLeadFromDto(newLeadDto);
                return Ok(OkResponse.WithData(leadDto));
            }
            catch (DuplicateRecordCreationException ex)
            {
                return ResultWithCodeAndBody(HttpStatusCode.SeeOther, new OkResponse<Guid>()
                {
                    Data = ex.RecordUid,
                });
            }
        }

        /// <summary>
        /// Updates the lead identified by uid with new data
        /// </summary>
        /// <remarks>Updates the lead based on parameters. Returns true on success.</remarks>
        /// <param name="leadUid">uid identifying the lead</param>
        /// <param name="updatedLeadDto">Updated Lead data</param>
        /// <response code="200">Record was updated successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpPut("{lead_uid}")]
        [SwaggerOperation("UpdateLead")]
        [ProducesResponseType(typeof(OkResponse<LeadDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<IActionResult> UpdateLead(
            [FromRoute(Name = "lead_uid")] Guid leadUid,
            [FromBody] LeadDto updatedLeadDto)
        {
            var lead = await LeadsCrudService.Update(leadUid, updatedLeadDto);
            return Ok(OkResponse.WithData(lead));
        }

        /// <summary>
        /// Deletes lead by its uid
        /// </summary>
        /// <remarks>Returns true if successfull.\n</remarks>
        /// 
        /// <param name="leadUidStr">Uid identifying the lead</param>
        /// 
        /// <response code="200">Record was deleted successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpDelete("{leadUidStr}")]
        [SwaggerOperation("DeleteLead")]
        [ProducesResponseType(typeof(OkResponseEmpty), 200)]
        public async Task<IActionResult> DeleteLead([FromRoute] string leadUidStr)
        {
            await LeadsCrudService.DeleteLeadByUidString(leadUidStr);

            return Ok(new OkResponseEmpty());
        }

        /// <summary>
        /// Export all leads with given IDs to file
        /// </summary>
        /// <remarks>Returns the file instantly with a Content-Disposition header.\n</remarks>
        /// 
        /// <param name="leadExportRequestDto">List of lead uids</param>
        /// <param name="format">File format - could be csv or excel</param>
        /// 
        /// <response code="200">File contents for the exported leads.</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost("export/file")]
        [SwaggerOperation("ExportLeadsToFile")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> ExportLeadsToFile([FromBody] LeadsExportRequestDto leadExportRequestDto)
        {
            var exportData = await LeadsExportToFileService.PrepareExportData(leadExportRequestDto);
            var file = LeadsExportToFileService.ConstructExportFileName(leadExportRequestDto.Format);
            var val = new ContentDispositionHeaderValue("attachment")
            {
                FileName = file,
            };
            Response.Headers.Add("Content-Disposition", val.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            return File(exportData, LeadsExportToFileService.MimeMapping[leadExportRequestDto.Format]);
        }

        /// <summary>
        /// Export all leads with given IDs to default CRM
        /// </summary>
        /// <remarks>Updates the LeadsExportStatus for the given CRM. Returns the report on data export.\n</remarks>
        /// <param name="leadExportRequestDto">List of lead uids</param>
        /// <response code="200">Export log</response>
        /// <response code="500">Unexpected error</response>
        /// 
        /// <exception cref="CrmUnauthorizedException">Throws exception if CRM returns rejects refresh token and responds with 401/403</exception>
        [HttpPost("export/crm")]
        [SwaggerOperation("ExportLeadsToCrm")]
        [ProducesResponseType(typeof(OkResponse<LeadsExportReportDto>), 200)]
        public async Task<IActionResult> ExportLeadsToCrm([FromBody] LeadsExportRequestDto leadExportRequestDto)
        {
            var result = await LeadsExportToCrmService.ExportLeads(leadExportRequestDto);

            var responseObj = new OkResponse<LeadsExportReportDto>()
            {
                Data = result
            };

            return Ok(responseObj);
        }
    }
}