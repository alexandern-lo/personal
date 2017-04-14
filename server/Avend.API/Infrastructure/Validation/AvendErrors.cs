using System;
using Avend.API.Infrastructure.Responses;
using Error = Qoden.Validation.Error;

namespace Avend.API.Infrastructure.Validation
{
    public static class AvendErrors
    {
        public static void ApiErrorCode(this Error error, string code)
        {
            error.Add(AvendValidations.ErrorCode, code);
        }

        public static string ApiErrorCode(this Error error)
        {
            if (error.ContainsKey(AvendValidations.ErrorCode))
                return error[AvendValidations.ErrorCode] as string;
            return null;
        }

        public static bool IsValidationError(this Error e)
        {
            return e.ApiErrorCode() == null && !string.IsNullOrEmpty(e.Key);
        }

        public static void NotFound(this Error error)
        {
            ApiErrorCode(error, ErrorCodes.CodeNotFound);
        }

        public static void OldData(this Error error)
        {
            ApiErrorCode(error, ErrorCodes.CodeRejectedOldData);
        }

        public static void InvalidParameter(this Error error)
        {
            ApiErrorCode(error, ErrorCodes.CodeInvalidParameter);
        }

        public static void InvalidUser(this Error error)
        {
            ApiErrorCode(error, ErrorCodes.CodeInvalidUser);
        }

        public static void Forbidden(this Error error)
        {
            ApiErrorCode(error, ErrorCodes.Forbidden);
        }

        public static Action<Error> Forbidden(string message)
        {
            return e =>
            {
                e.MessageFormat = message;
                e.Forbidden();
            };
        }
    }
}