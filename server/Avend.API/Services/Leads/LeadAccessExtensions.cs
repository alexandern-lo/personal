using System;
using System.Linq.Expressions;

using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;

using Qoden.Validation;

namespace Avend.API.Services.Leads
{
    public static class LeadAccessExtensions
    {
        public static Expression<Func<LeadRecord, bool>> OwnLeads(this UserContext userContext)
        {
            return lead => !lead.Deleted && lead.UserUid == userContext.UserUid;
        }

        public static Expression<Func<LeadRecord, bool>> AvailableLeads(this UserContext userContext)
        {
            switch (userContext.Role)
            {
                case UserRole.SeatUser:
                    return userContext.OwnLeads();

                case UserRole.Admin:
                    return lead => lead.SubscriptionId == userContext.TenantId;

                case UserRole.SuperAdmin:
                    return lead => true;
            }

            return lead => false;
        }

        public static void AssertAccess(this UserContext user, LeadRecord leadRecord)
        {
            var check = user.AvailableLeads();
            var checker = check.Compile();
            
            Check.Value(checker(leadRecord), onError: AvendErrors.Forbidden("Access denied"))
                .IsTrue();
        }
    }
}