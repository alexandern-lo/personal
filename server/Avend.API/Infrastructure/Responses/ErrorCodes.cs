namespace Avend.API.Infrastructure.Responses
{
    public static class ErrorCodes
    {
        public static readonly string Unauthorized = "access_denied";
        public static readonly string Forbidden = "forbidden";
        public static readonly string SubscriptionAbsent = "subscription_absent";
        public static readonly string SubscriptionExpired = "subscription_expired";
        public static readonly string SubscriptionMembersViolation = "subscription_members_error";
        public static readonly string CodeNotFound = "not_found";
        public static readonly string CodeInvalidUser = "invalid_user";
        public static readonly string CodeInvalidParameter = "invalid_parameter";
        public static readonly string CodeTermsAlreadyAccepted = "already_accepted";
        public static readonly string CodeRejectedOldData = "rejected_old_data";
    }
}