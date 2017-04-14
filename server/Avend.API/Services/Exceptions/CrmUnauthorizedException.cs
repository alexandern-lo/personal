using System;

namespace Avend.API.Services.Exceptions
{
    public class CrmUnauthorizedException : Exception
    {
        public CrmUnauthorizedException(string message)
            : base(message)
        { }

        public CrmUnauthorizedException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}