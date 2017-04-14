using System.Linq;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Responses;
using Avend.API.Infrastructure.Validation;
using Microsoft.AspNetCore.Http;
using Qoden.Validation;
using Error = Qoden.Validation.Error;

namespace Avend.API.Middleware
{
    public class MultipleErrorsExceptionConverter : ExceptionConverter<MultipleErrorsException>
    {
        protected override async Task Convert(MultipleErrorsException e, HttpContext context)
        {
            var response = context.Response;
            var topError = e.Errors.OrderBy(ErrorScore).FirstOrDefault();
            response.StatusCode = ErrorExceptionConverter.ErrorStatusCode(topError);
            await WriteBody(response, e.ToAvendErrorResponse());
        }

        private int ErrorScore(Error arg)
        {
            var code = arg.ApiErrorCode();
            if (code == null)
            {
                //validation errors comes last
                return 0;
            }

            if (code == ErrorCodes.Forbidden || code == ErrorCodes.Unauthorized)
            {
                return 3;
            }

            if (code == ErrorCodes.CodeNotFound)
            {
                return 2;
            }
            
            return 1;
        }
    }
}