using System.Threading.Tasks;
using Avend.API.Infrastructure.Responses;
using Avend.API.Infrastructure.Validation;
using Microsoft.AspNetCore.Http;
using Qoden.Validation;
using Error = Qoden.Validation.Error;

namespace Avend.API.Middleware
{
    public class ErrorExceptionConverter : ExceptionConverter<ErrorException>
    {
        protected override async Task Convert(ErrorException e, HttpContext context)
        {
            var response = context.Response;
            response.StatusCode = ErrorStatusCode(e.Error);
            await WriteBody(response, e.Error.ToAvendErrorResponse());
        }

        public static int ErrorStatusCode(Error e)
        {
            var code = e.ApiErrorCode();
            if (Equals(code, ErrorCodes.CodeNotFound))
            {
                return 404;
            }
            if (Equals(code, ErrorCodes.CodeRejectedOldData))
            {
                return 409;
            }

            if (Equals(code, ErrorCodes.Unauthorized))
            {
                return 401;
            }
            if (Equals(code, ErrorCodes.Forbidden))
            {
                return 403;
            }

            return 400;
        }
    }
}