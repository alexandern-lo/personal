using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;

namespace Avend.API.Services.Resources
{
    public static class ResourcesAccess
    {
        public static Expression<Func<Resource, bool>> AvailableResources(this UserContext userContext, AvendDbContext db)
        {
            switch (userContext.Role)
            {
                case UserRole.SeatUser:
                    return x => x.UserUid == userContext.UserUid;
                case UserRole.Admin:
                {
                     var users = db.SubscriptionMembers
                    .Where(x => x.SubscriptionId == userContext.TenantId)
                    .Select(x => new Guid?(x.UserUid))
                    .ToList();
                return x => users.Contains(x.UserUid);
                }
                case UserRole.SuperAdmin:
                    return x => true;
                default:
                    return x => false;
            }
        }
    }
}
