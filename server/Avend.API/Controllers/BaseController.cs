using System;
using System.Linq;
using System.Net;
using Avend.API.Infrastructure;
using Avend.API.Infrastructure.Logging;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Qoden.Validation;

namespace Avend.API.Controllers
{
    /// <summary>
    /// Base controller for application endpoints that utilize database access.
    ///	Database options are passed via DI mechanism provided by AspNetCore.
    /// </summary>
    public class BaseController : Controller
    {
        private readonly DbContextOptions<AvendDbContext> _databaseOptions;

        protected ILogger Logger { get; }

        public BaseController(DbContextOptions<AvendDbContext> options)
        {
            _databaseOptions = options;

            Logger = AvendLog.CreateLogger(GetType().Name);
            Logger.LogInformation("Controller " + GetType().FullName + " was instantiated");

            tenantUid = null;
        }

        [NonAction]
        public AvendDbContext GetDatabaseService()
        {
            Logger.LogTrace(HttpContext.TraceIdentifier + ": Trying to get the new instance of SqlService");
            var db = Activator.CreateInstance(typeof(AvendDbContext), _databaseOptions) as AvendDbContext;
            Logger.LogTrace(HttpContext.TraceIdentifier + ": Succesfully got the new instance of SqlService");
            return db;
        }

        private SubscriptionRecord _subscription;

        public SubscriptionRecord Subscription
        {
            get
            {
                if (UserUid == null)
                    return null;

                if (_subscription == null)
                {
                    using (var db = GetDatabaseService())
                    {
                        var querySubscriptions = from subscription in db.SubscriptionsTable
                            where subscription.RecurlyAccountUid == _userUid.Value
                            orderby subscription.ExpiresAt descending
                            select subscription;

                        _subscription = querySubscriptions.FirstOrDefault();
                    }
                }

                return _subscription;
            }
        }

        private Guid? _userUid;

        public Guid? UserUid
        {
            get
            {
                if (_userUid == null)
                {
                    var userUidClaim = User.Claims.FirstOrDefault(claim =>
                        claim.Type == "oid" ||
                        claim.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier"
                    );

                    if (userUidClaim == null)
                    {
                        // "Cannot find UID claim for user"
                        // return BadRequest(ErrorResponse.GenerateInvalidUser("user"));
                    }
                    else
                        _userUid = Guid.Parse(userUidClaim.Value);
                }

                return _userUid;
            }
        }

        private Guid? tenantUid;

        public Guid? TenantUid
        {
            get
            {
                if (UserUid == null)
                    return null;

                return tenantUid;
            }
        }

        [NonAction]
        public virtual IActionResult UnauthorizedWithCodeAndBody(int code, object value)
        {
            return new ObjectResult(value)
            {
                StatusCode = code,
            };
        }

        [NonAction]
        public virtual IActionResult ResultWithCodeAndBody(int code, object value)
        {
            return new ObjectResult(value)
            {
                StatusCode = code,
            };
        }

        [NonAction]
        public virtual IActionResult ResultWithCodeAndBody(HttpStatusCode code, object value)
        {
            return new ObjectResult(value)
            {
                StatusCode = (int)code,
            };
        }

        [NonAction]
        public ActionResult ValidationError(IValidator errorList)
        {
            Assert.Argument(errorList.HasErrors, "errors").IsTrue();
            return BadRequest(errorList.ToAvendErrorResponse());
        }
    }
}