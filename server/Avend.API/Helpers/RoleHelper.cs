using Avend.API.Model;
using Avend.API.Model.NetworkDTO;

namespace Avend.API.Helpers
{
    public static class RoleHelper
    {
        public static UserRole FromSubscriptionRole(SubscriptionMemberRole role)
        {
            if (role == SubscriptionMemberRole.Admin) return UserRole.Admin;
            if (role == SubscriptionMemberRole.User) return UserRole.SeatUser;
            if (role == SubscriptionMemberRole.SuperAdmin) return UserRole.SuperAdmin;
            return UserRole.Anonymous;
        }
    }
}
