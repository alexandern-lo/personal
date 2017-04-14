using System;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Controllers.v1;
using Avend.API.Infrastructure;
using Avend.API.Infrastructure.Logging;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Qoden.Validation;

namespace Avend.API.Middleware
{
    /// <summary>
    /// Authorize subscription member request and set instance UserContext into HttpContext
    /// </summary>
    public class SubscriptionRequirementHandler : AuthorizationHandler<SubscriptionRequirement>
    {
        private readonly ILogger _logger = AvendLog.CreateLogger(nameof(SubscriptionRequirementHandler));
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DbContextOptions<AvendDbContext> _dbOptions;

        public SubscriptionRequirementHandler(IHttpContextAccessor httpContextAccessor,
            DbContextOptions<AvendDbContext> dbOptions)
        {
            Assert.Argument(httpContextAccessor, nameof(httpContextAccessor)).NotNull();
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();
            _httpContextAccessor = httpContextAccessor;
            _dbOptions = dbOptions;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            SubscriptionRequirement requirement)
        {
            var userUid = context.User.AzureOid();
            var httpCtx = _httpContextAccessor.HttpContext;
            var userContext = httpCtx.GetUserContext() ?? new UserContext(_dbOptions);

            if (!userContext.IsLoaded)
            {
                if (userUid != Guid.Empty)
                {
                    await userContext.LoadUser(userUid, context.User);
                }
                else
                {
                    await userContext.LoadAnonymous();
                }
            }

            if (requirement.Roles.Contains(userContext.Role) || userContext.Role == UserRole.SuperAdmin)
            {
                _logger.LogTrace("Auth requirement fullfilled for '{role}' - {userUid}.", userContext.Role, userUid);
                httpCtx.SetUserContext(userContext);
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogTrace("Auth requirement NOT fullfilled for '{role}' - {userUid}", userContext.Role, userUid);
            }
            if (userContext.Role != UserRole.SuperAdmin)
            {
                //abort request processing if there are any errors in UserContext
                userContext.Errors.Throw();
            }
        }
    }
}