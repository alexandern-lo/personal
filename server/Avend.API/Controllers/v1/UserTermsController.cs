using System;
using System.Linq;
using System.Threading.Tasks;

using Avend.API.Infrastructure.Responses;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Swashbuckle.SwaggerGen.Annotations;

namespace Avend.API.Controllers.v1
{
    /// <summary>
    /// User terms controller. 
    /// Allows to get the latest terms by user and accept them.
    /// Also allows admin to review latest terms and update them.
    /// </summary>
    [Authorize]
    public class UserTermsController : BaseController
    {
        public UserTermsController(DbContextOptions<AvendDbContext> options) :
            base(options)
        {
        }

        /// <summary>
        /// Last terms for current user wiht acception indication.
        /// </summary>
        /// 
        /// <remarks>Returns TermsDTO\n</remarks>
        /// 
        /// <response code="200">Latest terms for the current user are retrieved successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [Route("api/v1/users/terms/latest")]
        [SwaggerOperation("GetLatestTermsForUser")]
        [ProducesResponseType(typeof(OkResponse<TermsDTO>), 200)]
        public IActionResult GetLatestTermsForUser()
        {
            if (!UserUid.HasValue)
                return NotFound(ErrorResponse.GenerateInvalidUser("user_uid"));

            Logger.LogInformation("Retrieving latest terms for user with UID: " + UserUid);

            using (var db = GetDatabaseService())
            {

                var termsQuery = from terms in db.TermsTable
                                 orderby terms.ReleaseDate descending
                                 select terms;

                var termsObj = termsQuery.FirstOrDefault();

                if (termsObj == null)
                {
                    var responseObjNoTerms = new OkResponse<TermsDTO>()
                    {
                        Data = null,
                    };

                    return Ok(responseObjNoTerms);
                }

                var termsAcceptanceQuery = from termsAcceptance in db.TermsAcceptancesTable
                                           where
                                               termsAcceptance.UserUid == (UserUid ?? Guid.Empty) &&
                                               termsAcceptance.TermsId == termsObj.Id
                                           select termsAcceptance;

                DateTime? acceptedAt = null;
                var acceptanceRecord = termsAcceptanceQuery.FirstOrDefault();
                if (acceptanceRecord != null)
                {
                    acceptedAt = acceptanceRecord.AcceptedAt;
                }

                var responseObj = new OkResponse<TermsDTO>()
                {
                    Data = TermsDTO.From(termsObj, acceptedAt),
                };

                return Ok(responseObj);
            }
        }

        /// <summary>
        /// Returns latest version of the terms present in the system to be edited by superadmins.
        /// </summary>
        /// 
        /// <response code="200">Latest version of the terms present in the system is retrieved successfully</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [Route("api/v1/terms/latest")]
        [SwaggerOperation("GetAdminLatestTerms")]
        [ProducesResponseType(typeof(OkResponse<TermsDTO>), 200)]
        public IActionResult GetAdminLatestTerms()
        {
            if (!UserUid.HasValue)
                return NotFound(ErrorResponse.GenerateInvalidUser("user_uid"));

            using (var db = GetDatabaseService())
            {

                var query = from terms in db.TermsTable
                            orderby terms.ReleaseDate descending
                            select terms;

                var termsObj = query.FirstOrDefault();

                var responseObj = new OkResponse<TermsDTO>()
                {
                    Data = TermsDTO.From(termsObj, null),
                };

                return Ok(responseObj);
            }
        }

        /// <summary>
        /// Terms acception endpoint.
        /// </summary>
        /// 
        /// <remarks>Tries to submit acceptance of the terms with given GUID by the current user into database.</remarks>
        /// 
        /// <param name="termsUid">UID of terms that were accepted by user</param>
        /// <response code="200">Empty success response</response>
        /// <response code="400">Terms with given UID are already accepted</response>
        /// <response code="404">Terms with given UID are not found</response>
        [HttpPost]
        [Route("api/v1/users/terms/{termsUid}/accept")]
        [SwaggerOperation("AcceptTerms")]
        [ProducesResponseType(typeof(OkResponseEmpty), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> AcceptTerms([FromRoute] Guid termsUid)
        {
            if (!UserUid.HasValue)
                return NotFound(ErrorResponse.GenerateInvalidUser("user_uid"));

            Logger.LogInformation("Accepting terms " + termsUid + " for user with UID: " + UserUid);

            using (var db = GetDatabaseService())
            {

                var query = from terms in db.TermsTable
                            where terms.Uid == termsUid
                            select terms;

                var termsObj = query.FirstOrDefault();

                if (termsObj == null)
                {
                    var responseObjTermsNotFound = ErrorResponse.GenerateNotFound(typeof(Terms), termsUid, "terms_uid");

                    return NotFound(responseObjTermsNotFound);
                }

                var termsAcceptanceQuery = from termsAcceptance in db.TermsAcceptancesTable
                                           where
                                               termsAcceptance.UserUid == (UserUid ?? Guid.Empty) &&
                                               termsAcceptance.TermsId == termsObj.Id
                                           select termsAcceptance;

                var acceptanceObj = termsAcceptanceQuery.FirstOrDefault();
                if (acceptanceObj != null)
                {
                    var responseObjAlreadyAccepted = ErrorResponse.GenerateTermsAlreadyAccepted(termsUid, "terms_uid");

                    return BadRequest(responseObjAlreadyAccepted);
                }

                db.TermsAcceptancesTable.Add(new TermsAcceptance()
                {
                    UserUid = UserUid ?? Guid.Empty,
                    TermsId = termsObj.Id,
                    AcceptedAt = DateTime.UtcNow,
                });

                await db.SaveChangesAsync();

                var responseObj = new OkResponseEmpty();

                return Ok(responseObj);
            }
        }

        /// <summary>
        /// Terms removal - FOR TESTING ONLY!
        /// <br/>
        /// <b>TODO:</b> Remove when go live in production
        /// </summary>
        /// 
        /// <remarks>Tries to remove the acceptance of the terms with given GUID by the current user into database.</remarks>
        /// 
        /// <param name="termsUid">UID of terms that were accepted by user</param>
        /// 
        /// <response code="200">Empty success response</response>
        /// <response code="400">Terms with given UID are not accepted</response>
        /// <response code="404">Terms with given UID are not found</response>
        [HttpDelete]
        [Route("api/v1/users/terms/{termsUid}/reset")]
        [SwaggerOperation("ResetTerms")]
        [ProducesResponseType(typeof(OkResponseEmpty), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> ResetTerms([FromRoute] Guid termsUid)
        {
            if (!UserUid.HasValue)
                return NotFound(ErrorResponse.GenerateInvalidUser("user_uid"));

            using (var db = GetDatabaseService())
            {

                var query = from terms in db.TermsTable
                            where terms.Uid == termsUid
                            select terms;

                var termsObj = query.FirstOrDefault();

                if (termsObj == null)
                {
                    var responseObjTermsNotFound = ErrorResponse.GenerateNotFound(typeof(Terms), termsUid, "terms_uid");

                    return NotFound(responseObjTermsNotFound);
                }

                var termsAcceptanceQuery = from termsAcceptance in db.TermsAcceptancesTable
                                           where
                                               termsAcceptance.UserUid == (UserUid ?? Guid.Empty) &&
                                               termsAcceptance.TermsId == termsObj.Id
                                           select termsAcceptance;

                var acceptanceObj = termsAcceptanceQuery.FirstOrDefault();
                if (acceptanceObj == null)
                {
                    var responseObjAlreadyAccepted = ErrorResponse.GenerateInvalidParameter(typeof(Terms), termsUid,
                        "terms are not accepted", "terms_uid");

                    return BadRequest(responseObjAlreadyAccepted);
                }

                db.TermsAcceptancesTable.Remove(acceptanceObj);

                await db.SaveChangesAsync();

                var responseObj = new OkResponseEmpty();

                return Ok(responseObj);
            }
        }
    }
}