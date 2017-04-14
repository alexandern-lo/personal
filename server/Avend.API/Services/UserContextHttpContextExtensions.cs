using Microsoft.AspNetCore.Http;

namespace Avend.API.Services
{
    public static class UserContextHttpContextExtensions
    {
        public static UserContext GetUserContext(this HttpContext ctx)
        {
            return ctx.Items["UserContext"] as UserContext;
        }

        public static void SetUserContext(this HttpContext ctx, UserContext userContext)
        {
            ctx.Items["UserContext"] = userContext;
        }
    }
}