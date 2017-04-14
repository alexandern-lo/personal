using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Identity.Client;
using LiveOakApp.Resources;
using LiveOakApp.Models.Services;
using System.Net;

namespace LiveOakApp.Models
{
    public static class ExceptionExtensions
    {
        public static string MessageForHuman(this Exception exception)
        {
            // TODO: write better messages
            var msalError = exception as MsalException;
            if (msalError != null)
            {
                if (msalError.IsMsalUserDontExistException())
                {
                    return L10n.Localize("MSALNoUserPleaseSignUp", "This account doesn't exist, please go to Avend web portal to create an account.");
                }
                if (msalError.IsMsalCancelledException())
                {
                    return null;
                }
            }
            var multiple = exception as MultipleException;
            if (multiple != null)
            {
                return multiple.Exceptions.FirstOrDefault()?.MessageForHuman()
                               ?? multiple.InnerException?.MessageForHuman()
                               ?? exception.Message;
            }
            var apiException = exception as ApiException;
            if (apiException != null)
            {
                return apiException.InnerException?.MessageForHuman()
                               ?? exception.Message;
            }
            var webException = exception as WebException;
            if (webException == null) 
                webException = exception?.InnerException as WebException;
            if (webException != null)
            {
                return L10n.Localize("NetworkError", "Network error");
            }
            return exception?.Message;
        }

        public static bool IsMsalUserDontExistException(this MsalException msalError)
        {
            return msalError.Message?.Contains("User does not exist") == true;
        }

        public static bool IsMsalCancelledException(this MsalException msalError)
        {
            return msalError.Message?.Contains("User canceled authentication") == true;
        }

        public static List<Exception> AllExceptions(this Exception exception)
        {
            var list = new List<Exception>();
            list.Add(exception);
            var multiple = exception as MultipleException;
            if (multiple?.Exceptions?.Any() == true)
            {
                list.AddRange(multiple.Exceptions);
            }
            return list;
        }
    }

    public class ApiException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ApiException() { }

        public ApiException(string message) : base(message) { }

        public ApiException(string message, Exception inner) : base(message, inner) { }
    }

    public class MultipleException : ApiException
    {
        public MultipleException() { }

        public MultipleException(string message) : base(message) { }

        public MultipleException(string message, Exception inner) : base(message, inner) { }

        public List<Exception> Exceptions { get; private set; }

        public MultipleException(List<Exception> exceptions) { Exceptions = exceptions; }

        public MultipleException(string message, List<Exception> exceptions) : base(message) { Exceptions = exceptions; }

        public MultipleException(string message, Exception inner, List<Exception> exceptions) : base(message, inner) { Exceptions = exceptions; }
    }

    public class AuthException : ApiException
    {
        public AuthException() { }

        public AuthException(string message) : base(message) { }

        public AuthException(string message, Exception inner) : base(message, inner) { }
    }

    public class ApiRecordNotFound : ApiException
    {
        public ApiRecordNotFound() { }

        public ApiRecordNotFound(string message) : base(message) { }

        public ApiRecordNotFound(string message, Exception inner) : base(message, inner) { }
    }

    public class ApiRoutingError : ApiException
    {
        public ApiRoutingError() { }

        public ApiRoutingError(string message) : base(message) { }

        public ApiRoutingError(string message, Exception inner) : base(message, inner) { }
    }

    public class ApiServerError : ApiException
    {
        public ApiServerError() { }

        public ApiServerError(string message) : base(message) { }

        public ApiServerError(string message, Exception inner) : base(message, inner) { }
    }

    #region ApiServerErrors

    public class AccessDeniedError : ApiServerError
    {
        public AccessDeniedError() { }

        public AccessDeniedError(string message) : base(message) { }

        public AccessDeniedError(string message, Exception inner) : base(message, inner) { }
    }

    public class SubscriptionExpiredError : ApiServerError
    {
        public SubscriptionExpiredError() { }

        public SubscriptionExpiredError(string message) : base(message) { }

        public SubscriptionExpiredError(string message, Exception inner) : base(message, inner) { }
    }

    public class ServerHasNewerLeadError : ApiServerError
    {
        public DateTime? ServerUpdatedAt { get; }

        public ServerHasNewerLeadError(DateTime? serverUpdatedAt)
        {
            ServerUpdatedAt = serverUpdatedAt;
        }
    }

    public class EntityNotFoundError : ApiServerError
    {
        public EntityNotFoundError() { }

        public EntityNotFoundError(string message) : base(message) { }

        public EntityNotFoundError(string message, Exception inner) : base(message, inner) { }
    }

    public class LeadNotFoundError : ApiServerError
    {
        public LeadNotFoundError() { }

        public LeadNotFoundError(string message) : base(message) { }

        public LeadNotFoundError(string message, Exception inner) : base(message, inner) { }
    }

    public class EventNotFoundError : ApiServerError
    {
        public EventNotFoundError() { }

        public EventNotFoundError(string message) : base(message) { }

        public EventNotFoundError(string message, Exception inner) : base(message, inner) { }
    }

    public class LeadCRMExportFailedError : ApiServerError
    {
        public LeadCRMExportFailedError() { }

        public LeadCRMExportFailedError(string message) : base(message) { }

        public LeadCRMExportFailedError(string message, Exception inner) : base(message, inner) { }
    }

    #endregion
}
