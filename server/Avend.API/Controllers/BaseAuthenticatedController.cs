using System;
using System.Linq;

using Avend.API.Infrastructure.Logging;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Avend.API.Controllers
{
    /// <summary>
    /// Base controller for application endpoints that uses.
    /// </summary>
    public class BaseAuthenticatedController : Controller
    {
        protected ILogger Logger { get; }

        public BaseAuthenticatedController()
        {
            Logger = AvendLog.CreateLogger(GetType().Name);
            Logger.LogInformation("Controller " + GetType().FullName + " was instantiated");
            tenantUid = null;
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
    }
}