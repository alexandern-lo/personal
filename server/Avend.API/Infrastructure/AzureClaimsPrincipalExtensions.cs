using System;
using System.Linq;
using System.Security.Claims;
using Qoden.Validation;

namespace Avend.API.Infrastructure
{
    public static class AzureClaimsPrincipalExtensions
    {
        public static Guid AzureOid(this ClaimsPrincipal principal)
        {
            Assert.Argument(principal, nameof(principal)).NotNull();
            var guidStr = principal.Claims.FirstOrDefault(claim =>
                claim.Type == "oid" ||
                claim.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier"
            );
            if (guidStr?.Value == null) return Guid.Empty;

            Guid result;
            if (!Guid.TryParse(guidStr.Value, out result)) return Guid.Empty;
            return result;
        }
    }
}