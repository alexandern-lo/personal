using System;

namespace Avend.API.Services.Exceptions
{
    public class RecurlyServiceException : Exception
    {
        public string ObjectName { get; private set; }

        public RecurlyServiceException(string objectName, string message)
            : base(message)
        {
            ObjectName = objectName;
        }
    }
}