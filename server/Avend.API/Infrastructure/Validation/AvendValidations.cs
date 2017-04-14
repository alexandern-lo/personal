using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Infrastructure.Responses;
using Avend.API.Infrastructure.Validation;
using Qoden.Validation;
using Error = Qoden.Validation.Error;

namespace Avend.API.Infrastructure.Validation
{
    public static class AvendValidations
    {
        public const string ErrorCode = "Code";

        public static Check<string> IsShortText(this Check<string> check)
        {
            return check.NotEmpty().MinLength(2).MaxLength(200);
        }

        public static Check<SubscriptionRecord> SubscriptionIsValid(this Check<SubscriptionRecord> check)
        {
            if (check.Value == null)
            {
                var error = new Error(ErrorResponse.ErrorMessageSubscriptionAbsent)
                {
                    {ErrorCode, ErrorCodes.SubscriptionAbsent}
                };

                check.Fail(error);

                return check;
            }

            if (check.Value.ExpiresAt < DateTime.UtcNow)
            {
                var error = new Error(ErrorResponse.ErrorMessageSubscriptionExpired)
                {
                    {ErrorCode, ErrorCodes.SubscriptionExpired}
                };

                check.Fail(error);
            }

            return check;
        }

        public static Check<MoneyDto> MoneyIsValid(this Check<MoneyDto> check, string recordUid)
        {
            if (check.Value == null)
            {
                var message = ErrorResponse.GenerateInvalidParameterMessage(typeof(MoneyDto), recordUid,
                    "Money object cannot be null");
                check.Fail(new Error(message) { {ErrorCode, ErrorCodes.CodeInvalidParameter} });

                return check;
            }

            if (check.Value.Amount < 0)
            {
                var message = ErrorResponse.GenerateInvalidParameterMessage(typeof(MoneyDto), recordUid, "Amount cannot be less than zero");
                check.Fail(new Error(message) { {ErrorCode, ErrorCodes.CodeInvalidParameter} });
            }

            if (!check.Value.Currency.HasValue || check.Value.Currency.Value == CurrencyCode.Unknown)
            {
                var message = ErrorResponse.GenerateInvalidParameterMessage(typeof(MoneyDto), recordUid, "Currency is unknown");
                check.Fail(new Error(message) { {ErrorCode, ErrorCodes.CodeInvalidParameter} });
            }

            return check;
        }

        public static Check<T> ParameterNotNull<T>(this Check<T> check, Type entity, string recordUid, string details)
            where T : class
        {
            if (check.Value == null)
            {
                var message = ErrorResponse.GenerateInvalidParameterMessage(entity, recordUid, details);
                var error = new Error(message);
                error.ApiErrorCode(ErrorCodes.CodeInvalidUser);
                check.Fail(error);
            }
            return check;
        }

        public static Check<T?> ParameterHasValue<T>(this Check<T?> check, Type entity, string recordUid, string details)
            where T : struct
        {
            if (!check.Value.HasValue)
            {
                var message = ErrorResponse.GenerateInvalidParameterMessage(entity, recordUid, details);
                var error = new Error(message);
                error.ApiErrorCode(ErrorCodes.CodeInvalidUser);
                check.Fail(error);
            }
            return check;
        }

        public static Check<T> ParameterNotEqualsTo<T>(this Check<T> check, T val, Type entity, string recordUid,
            string details)
        {
            if (Equals(check.Value, val))
            {
                var message = ErrorResponse.GenerateInvalidParameterMessage(entity, recordUid, details);
                var error = new Error(message);
                error.ApiErrorCode(ErrorCodes.CodeInvalidUser);
                check.Fail(error);
            }
            return check;
        }

        public static Check<T> ParameterEqualsTo<T>(this Check<T> check, T val, Type entity, string recordUid,
            string details)
        {
            if (!Equals(check.Value, val))
            {
                var message = ErrorResponse.GenerateInvalidParameterMessage(entity, recordUid, details);
                var error = new Error(message);
                error.ApiErrorCode(ErrorCodes.CodeInvalidUser);
                check.Fail(error);
            }

            return check;
        }

        public static Check<T> ParameterGreaterThan<T>(this Check<T> check, T val, Type entity, string recordUid, string details)
            where T : IComparable, IComparable<T>
        {
            if (check.Value.CompareTo(val) <= 0)
            {
                var message = ErrorResponse.GenerateInvalidParameterMessage(entity, recordUid, details);
                check.Fail(new Error(message));
            }

            return check;
        }

        public static Check<T> ParameterGreaterOrEqualThan<T>(this Check<T> check, T val, Type entity, string recordUid, string details)
            where T : IComparable, IComparable<T>
        {
            if (check.Value.CompareTo(val) < 0)
            {
                var message = ErrorResponse.GenerateInvalidParameterMessage(entity, recordUid, details);
                check.Fail(new Error(message));
            }

            return check;
        }
        private static readonly Regex re = new Regex("(?<=[a-z0-9])[A-Z]", RegexOptions.Compiled);

        public static Responses.Error ToAvendError(this Error e)
        {
            string[] fields = e.IsValidationError() ? new[] {e.Key} : null;
            string code;
            if (e.ApiErrorCode() != null)
            {
                code = e.ApiErrorCode();
            }
            else if (e.ContainsKey("Validator"))
            {
                code = e["Validator"] as string;
            }
            else if (e.IsValidationError())
            {
                code = "validation_error";
            }
            else
            {
                code = "bad_request";
            }            
            code = re.Replace(code, m => "_" + m.Value).ToLowerInvariant();;

            var avendError = new Responses.Error(code, e.Message, fields);
            //this is clumsy but does transfer data from Qoden.Validation.Error to avend error
            foreach (var info in e.Info)
            {
                if (info.Key == "Validator" || info.Key == "Exception")
                {
                    //Exceptions cannot be serialized into JSON
                    continue;                    
                }
                if (avendError.AdditionalData == null)
                {
                    avendError.AdditionalData = new Dictionary<string, object>();
                }
                var key = info.Key;
                key = re.Replace(key, m => "_" + m.Value).ToLowerInvariant();
                avendError.AdditionalData[key] = info.Value;
            }
            return avendError;
        }

        public static ErrorResponse ToAvendErrorResponse(this IValidator validator)
        {
            var errors = validator.Errors.Select(e => e.ToAvendError()).ToList();
            return new ErrorResponse(errors);
        }

        public static ErrorResponse ToAvendErrorResponse(this MultipleErrorsException errorList)
        {
            var errors = errorList.Errors.Select(e => e.ToAvendError()).ToList();
            return new ErrorResponse(errors);
        }

        public static ErrorResponse ToAvendErrorResponse(this Error error)
        {
            return new ErrorResponse(new List<Responses.Error> { error.ToAvendError() });
        }
    }
}