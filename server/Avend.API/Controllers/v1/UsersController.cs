using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Infrastructure;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.SwaggerGen.Annotations;
using Qoden.Validation;
using Avend.API.Infrastructure.Responses;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Services.Crm;
using Avend.API.Services.Subscriptions;

namespace Avend.API.Controllers.v1
{
    /// <summary>
    /// Users controller. 
    /// Allows to get the user profile and subscription details.
    /// </summary>
    [Authorize]
    [Route("api/v1/users")]
    public class UsersController : BaseController
    {
        public string[] AllowedPlanNamesFreeTrial { get; } = {
            "individual",
            "corporate",
        };

        public UsersController(
            DbContextOptions<AvendDbContext> options,
            UserCrmDtoBuilder userCrmConfigBuilder,
            ProfileService profileService,
            UsersManagementService usersManagement,
            SubscriptionsService subscriptions,
            ILogger<UsersController> logger
        ) :
            base(options)
        {
            Assert.Argument(userCrmConfigBuilder, nameof(userCrmConfigBuilder)).NotNull();
            Assert.Argument(profileService, nameof(profileService)).NotNull();
            Assert.Argument(usersManagement, nameof(usersManagement)).NotNull();

            UserCrmConfigBuilder = userCrmConfigBuilder;
            UsersManagementService = usersManagement;
            ProfileService = profileService;
            Subscriptions = subscriptions;
        }

        public UserContext UserContext { get; set; }
        public SubscriptionsService Subscriptions { get; set; }
        public ProfileService ProfileService { get; }
        public UsersManagementService UsersManagementService { get; }
        public UserCrmDtoBuilder UserCrmConfigBuilder { get; }

        [HttpGet]
        [SwaggerOperation("GetSubscriptionMembers")]
        [ProducesResponseType(typeof(OkResponseEmpty), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [Authorize(Policy = Startup.SubscriptionAdminPolicy)]
        public OkListResponse<SubscriptionMemberDto> Get(
            [FromQuery(Name = "q")] string filter = null,
            [FromQuery(Name = "page")] int pageNumber = 0,
            [FromQuery(Name = "per_page")] int recordsPerPage = 25,
            [FromQuery(Name = "sort_field")] string sortField = null,
            [FromQuery(Name = "sort_order")] string sortOrder = "asc",
            [FromQuery(Name = "status")] string status = null,
            [FromQuery(Name = "role")] string role = null,
            [FromQuery(Name = "tenant")] Guid? tenant = null)
        {
            var searchQueryParams = new SearchQueryParams(filter, sortField, sortOrder, pageNumber, recordsPerPage);

            var list = Subscriptions.FindUsers(
                searchQueryParams, 
                role: EnumFilter.Parse<SubscriptionMemberRole>(role),
                status: EnumFilter.Parse<SubscriptionMemberStatus>(status),
                tenantUid: tenant);
            return OkResponse.FromSearchResult(list);
        }

        [HttpPut]
        [Route("grant_admin")]
        [SwaggerOperation("GrantAdmin")]
        [ProducesResponseType(typeof(OkResponse<SubscriptionMemberDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [Authorize(Policy = Startup.SubscriptionAdminPolicy)]
        public Task<OkResponse<SubscriptionMemberDto>> GrantAdmin([FromBody] Guid userUid)
        {
            return GrantRevokeAdmin(userUid, true);
        }

        [HttpPut]
        [Route("revoke_admin")]
        [SwaggerOperation("RevokeAdmin")]
        [ProducesResponseType(typeof(OkResponse<SubscriptionMemberDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [Authorize(Policy = Startup.SubscriptionAdminPolicy)]
        public Task<OkResponse<SubscriptionMemberDto>> RevokeAdmin([FromBody] Guid userUid)
        {
            return GrantRevokeAdmin(userUid, false);
        }

        private async Task<OkResponse<SubscriptionMemberDto>> GrantRevokeAdmin(Guid userUid, bool admin)
        {            
            return OkResponse.WithData(await Subscriptions.UpdateMemberRole(userUid, admin));
        }

        [HttpPut]
        [Route("enable")]
        [SwaggerOperation("EnableUser")]
        [ProducesResponseType(typeof(OkResponse<SubscriptionMemberDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [Authorize(Policy = Startup.SubscriptionAdminPolicy)]
        public Task<OkResponse<SubscriptionMemberDto>> EnableUser([FromBody] Guid userUid)
        {
            return EnableDisableUser(userUid, true);
        }

        [HttpPut]
        [Route("disable")]
        [SwaggerOperation("DisableUser")]
        [ProducesResponseType(typeof(OkResponse<SubscriptionMemberDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [Authorize(Policy = Startup.SubscriptionAdminPolicy)]
        public Task<OkResponse<SubscriptionMemberDto>> DisableUser([FromBody] Guid userUid)
        {
            return EnableDisableUser(userUid, false);
        }

        private async Task<OkResponse<SubscriptionMemberDto>> EnableDisableUser(Guid userUid, bool status)
        {
            return OkResponse.WithData(await Subscriptions.UpdateMemberStatus(userUid, status));
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
        [Route("profile")]
        [SwaggerOperation("GetProfile")]
        [ProducesResponseType(typeof(OkResponse<UserProfileDto>), 200)]
        public async Task<IActionResult> GetProfile()
        {
            Logger.LogInformation("Retrieving profile for user with UID: " + UserUid);
            var profile = await ProfileService.GetProfile();
            var responseObj = new OkResponse<UserProfileDto>
            {
                Data = profile,
            };

            return Ok(responseObj);
        }

        /// <summary>
        /// Completely remove subscription record.
        /// </summary>
        /// <remarks>Returns empty success reply</remarks>
        /// <response code="200">Empty Success response</response>
        /// <response code="404">Subscription not found</response>        
        [HttpDelete]
        [Route("subscription")]
        [SwaggerOperation("RemoveSubscription")]
        [ProducesResponseType(typeof(OkResponseEmpty), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> RemoveSubscription()
        {
            if (!UserUid.HasValue)
                return NotFound(ErrorResponse.GenerateInvalidUser("user_uid"));

            using (var db = GetDatabaseService())
            {
                var subscriptionQuery = from subscription in db.SubscriptionsTable
                    where
                    subscription.RecurlyAccountUid == UserUid
                    orderby
                    subscription.Status == SubscriptionStatus.Active descending,
                    subscription.CreatedAt descending
                    select subscription;

                var subscriptionObj = subscriptionQuery.FirstOrDefault();

                if (subscriptionObj == null)
                {
                    return
                        NotFound(ErrorResponse.GenerateNotFound(typeof(SubscriptionRecord), "NO-SUBSCRIPTION",
                            "user_subscription_uid"));
                }

                db.SubscriptionsTable.Remove(subscriptionObj);

                await db.SaveChangesAsync();

                var responseObj = new OkResponseEmpty();

                return Ok(responseObj);
            }
        }

        /// <summary>
        /// Tries to register a free trial subscription for this user.
        /// </summary>
        /// <param name="type"></param>
        /// <remarks>Returns empty success reply</remarks>
        /// <response code="200">Empty Success response</response>
        /// <response code="204">Empty Success response</response>
        /// <response code="404">Subscription is not there</response>        
        [HttpPost]
        [Route("subscriptions/free/{planType}")]
        [SwaggerOperation("TryFreeSubscription")]
        [ProducesResponseType(typeof(OkResponse<DateTime>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> TryFreeSubscription([FromRoute(Name = "planType")] string type)
        {
            if (!UserUid.HasValue)
                return NotFound(ErrorResponse.GenerateInvalidUser("user_uid"));

            type = type.ToLower();

            if (!AllowedPlanNamesFreeTrial.Contains(type))
                return
                    NotFound(ErrorResponse.GenerateInvalidParameter(typeof(SubscriptionRecord), type,
                        "Invalid name of Free Trial plan", "planType"));

            using (var db = GetDatabaseService())
            {
                var subscriptionQuery = from subscription in db.SubscriptionsTable
                    where
                    subscription.RecurlyAccountUid == UserUid
                    orderby
                    subscription.Status == SubscriptionStatus.Active descending,
                    subscription.CreatedAt descending
                    select subscription;

                var subscriptionObj = subscriptionQuery.FirstOrDefault();

                if (subscriptionObj != null)
                {
                    if (subscriptionObj.Type == type)
                        return NoContent();

                    return
                        BadRequest(ErrorResponse.GenerateInvalidParameter(typeof(SubscriptionRecord), subscriptionObj.Uid,
                            "User already had a subscription and cannot apply for a free trial", "user_uid"));
                }

                Debug.Assert(UserUid != null, "UserUid != null");

                var freeTrialSubscription = new SubscriptionRecord()
                {
                    RecurlyAccountUid = UserUid.Value,
                    Service = SubscriptionServiceType.Free,
                    Type = type,
                    MaximumUsersCount = type == "corporate" ? 100 : 1,
                    AdditionalData = "Free trial",
                    Status = SubscriptionStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMonths(1),
                };

                db.SubscriptionsTable.Add(freeTrialSubscription);

                await db.SaveChangesAsync();

                var responseObj = new OkResponse<DateTime>()
                {
                    Data = freeTrialSubscription.ExpiresAt
                };

                return Ok(responseObj);
            }
        }
    }
}