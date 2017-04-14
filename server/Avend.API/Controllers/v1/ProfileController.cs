using System;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Responses;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qoden.Validation;
using Swashbuckle.SwaggerGen.Annotations;

namespace Avend.API.Controllers.v1
{
    [Authorize]
    [Route("api/v1/profile")]
    public class ProfileController : Controller
    {
        private readonly ProfileService _service;

        public ProfileController(ProfileService profileService)
        {
            Assert.Argument(profileService, nameof(profileService)).NotNull();
            _service = profileService;
        }
        /// <summary>
        /// Profile for current user indicating subscription status.
        /// </summary>
        /// 
        /// <remarks>Returns user profile containing user UID, last accepted terms and current subscription details</remarks>
        /// 
        /// <response code="200">Success response with UserProfileDTO as data</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [SwaggerOperation("GetProfile")]
        [ProducesResponseType(typeof(OkResponse<UserProfileDto>), 200)]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _service.GetProfile();
            var responseObj = new OkResponse<UserProfileDto>
            {
                Data = profile,
            };

            return Ok(responseObj);
        }

        [HttpPut("crm")]
        [SwaggerOperation("UpdateDefaultCrm")]
        [ProducesResponseType(typeof(OkResponse<UserProfileDto>), 200)]
        public async Task<OkResponse<UserCrmDto>> UpdateDefaultCrm([FromBody] Guid uid)
        {
            return OkResponse.WithData(await _service.UpdateDefaultCrm(uid));
        }
    }
}