using System.Security.Claims;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Microsoft.AspNetCore.Authorization;
using Qoden.Validation;

namespace Avend.API.Middleware
{
    /// <summary>
    /// Store subscription member required roles.
    /// </summary>
    public class SubscriptionRequirement : IAuthorizationRequirement
    {
        public SubscriptionRequirement(params UserRole[] roles)
        {
            Assert.Argument(roles, nameof(roles)).NotNull();
            Roles = roles;
        }

        public UserRole[] Roles { get; }
    }
}