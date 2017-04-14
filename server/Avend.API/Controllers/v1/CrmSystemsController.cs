using System.Collections.Generic;
using System.Linq;
using Avend.API.Infrastructure;
using Avend.API.Infrastructure.Responses;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.SwaggerGen.Annotations;
using Qoden.Validation;

namespace Avend.API.Controllers.v1
{
    /// <summary>
    /// User terms controller. 
    /// Allows to get the latest terms by user and accept them.
    /// Also allows admin to review latest terms and update them.
    /// </summary>
    [Authorize]
    public class CrmSystemsController : BaseController
    {
        public AppSettingsCrm AppSettingsCrm { get; }

        public UsersManagementService UsersManagementService { get; }

        public CrmSystemsController(
            DbContextOptions<AvendDbContext> options,
            AppSettingsCrm appSettingsCrm,
            UsersManagementService usersManagement,
            ILogger<UserTermsController> logger
            ) :
            base(options)
        {
            Assert.Argument(usersManagement, nameof(usersManagement)).NotNull();
            AppSettingsCrm = appSettingsCrm;

            UsersManagementService = usersManagement;
        }

        /// <summary>
        /// Last terms for current user wiht acception indication.
        /// </summary>
        /// <remarks>Returns TermsDTO\n</remarks>
        /// <response code="200">An array of events</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [Route("api/v1/crm_systems")]
        [SwaggerOperation("GetCrmSystems")]
        [ProducesResponseType(typeof(OkResponse<CrmSystemDTO>), 200)]
        public IActionResult GetCrmSystems()
        {
            if (!UserUid.HasValue)
                return NotFound(ErrorResponse.GenerateInvalidUser("user_uid"));

            Logger.LogInformation("Retrieving crm systems for user with UID: " + UserUid);

            using (var db = GetDatabaseService())
            {
                var crmSystemsQuery = from crmSystemRec in db.CrmSystemsTable
                                 orderby crmSystemRec.Name descending
                                 select crmSystemRec;

                var crmSystemsDtos = new List<CrmSystemDTO>();

                foreach (var crmSystem in crmSystemsQuery)
                {
                    crmSystemsDtos.Add(CrmSystemDTO.From(crmSystem));
                }

                var responseObj = new OkResponse<List<CrmSystemDTO>>()
                {
                    Data = crmSystemsDtos,
                };

                return Ok(responseObj);
            }
        }
    }
}