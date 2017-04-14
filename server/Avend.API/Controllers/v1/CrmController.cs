using System;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Responses;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Crm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qoden.Validation;
using Swashbuckle.SwaggerGen.Annotations;

namespace Avend.API.Controllers.v1
{
    [Authorize]
    [Route("api/v1/crm")]
    public class CrmController : Controller
    {
        private readonly CrmConfigurationService _service;

        public CrmController(CrmConfigurationService service)
        {
            Assert.Argument(service, nameof(service)).NotNull();
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("GetCrm")]
        [ProducesResponseType(typeof(OkListResponse<UserCrmDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<OkListResponse<UserCrmDto>> Get(
            [FromQuery(Name = "page")] int pageNumber = 0,
            [FromQuery(Name = "per_page")] int recordsPerPage = 25,
            [FromQuery(Name = "sort_field")] string sortField = "name",
            [FromQuery(Name = "sort_order")] string sortOrder = "asc")
        {
            var searchQuery = new SearchQueryParams(null, sortField, sortOrder, pageNumber, recordsPerPage);
            var list = await _service.Search(searchQuery);
            return OkResponse.FromSearchResult(list);
        }


        /// <summary>
        /// Retrieves the configuration of CRM with given UID.
        /// </summary>
        /// 
        /// <remarks>Returns user configuration DTO on success.</remarks>
        /// 
        /// <response code="200">Success response</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [Route("{uid}")]
        [SwaggerOperation("GetCrmConfigurationByUid")]
        [ProducesResponseType(typeof(OkResponse<UserCrmDto>), 200)]
        public async Task<OkResponse<UserCrmDto>> GetCrmConfigurationByUid(
            [FromRoute] Guid uid)
        {
            return OkResponse.WithData(await _service.FindByUid(uid));
        }

        /// <summary>
        /// Creates a new configuration for CRM with given UID.
        /// </summary>
        /// 
        /// <remarks>
        /// Returns object with the created CRM configuration.
        /// 
        /// 'dynamics_365_url' is only valid if 'crm_system_uid' is set to Dynamics365 CRM. If present 'dynamics_365_url' should be URL with 'http' or 'https' prefix.
        /// </remarks>
        /// 
        /// <response code="200">Success response</response>
        /// <response code="400">Invalid crm configuration given</response>
        /// <response code="404">Crm system referenced by crm configuration not found</response>
        [HttpPost]
        [SwaggerOperation("CreateCrmConfiguration")]
        [ProducesResponseType(typeof(OkResponse<UserCrmDto>), 200)]
        public async Task<OkResponse<UserCrmDto>> CreateCrmConfiguration([FromBody] UserCrmDto crmDto)
        {
            return OkResponse.WithData(await _service.Create(crmDto));            
        }

        /// <summary>
        /// Updates the configuration of CRM with given UID.
        /// </summary>
        /// 
        /// <remarks>
        /// Returns server date and time of the update moment.
        /// 
        /// 'dynamics_365_url' is only valid if 'crm_system_uid' is set to Dynamics365 CRM. If present 'dynamics_365_url' should be URL with 'http' or 'https' prefix.
        /// </remarks>
        /// 
        /// <response code="200">Success response</response>
        /// <response code="400">Invalid 'crmConfigurationDTO' given</response>
        /// <response code="500">Unexpected error</response>
        [HttpPut("{uid}")]
        [SwaggerOperation("UpdateCrmConfiguration")]
        [ProducesResponseType(typeof(OkResponse<UserCrmDto>), 200)]
        public async Task<OkResponse<UserCrmDto>> UpdateCrmConfiguration(
            [FromRoute] Guid uid,
            [FromBody] UserCrmDto crmDto)
        {
            return OkResponse.WithData(await _service.Update(uid, crmDto));
        }

        /// <summary>
        /// Providers the Oauth Grant Code for the CRM with given UID.
        /// <br /><br />
        /// Upon receiving it we need to request both the access and refresh tokens
        /// and store them into configuration record for future use.
        /// </summary>
        /// 
        /// <remarks>Returns server date and time of the update moment.</remarks>
        /// 
        /// <response code="200">Success response</response>        
        [HttpPut("{uid}/grant_code")]
        [SwaggerOperation("UpdateCrmGrantCode")]
        [ProducesResponseType(typeof(OkResponse<UserCrmDto>), 200)]
        public async Task<OkResponse<UserCrmDto>> UpdateCrmGrantCode(
            [FromRoute] Guid uid,
            [FromBody] UserCrmTokenDto userCrmTokenDto)
        {
            return OkResponse.WithData(await _service.UpdateGrantCode(uid, userCrmTokenDto));            
        }

        /// <summary>
        /// Deletes user configuration for CRM with given UID.
        /// </summary>
        /// 
        /// <remarks>Returns empty response indicating request outcome.</remarks>
        /// 
        /// <response code="200">Success response</response>        
        [HttpDelete("{uid}")]
        [SwaggerOperation("DeleteCrmConfiguration")]
        [ProducesResponseType(typeof(OkResponse<UserCrmDto>), 200)]
        public async Task<OkResponse<UserCrmDto>> DeleteCrmConfiguration([FromRoute] Guid uid)
        {
            return OkResponse.WithData(await _service.Delete(uid));
        }

    }
}