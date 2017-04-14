namespace Avend.API.Services.Exceptions
{
    public class RecurlyServiceDuplicateRequestException : RecurlyServiceException
    {
        public RecurlyServiceDuplicateRequestException (string objectName, string message)
            : base(objectName, message)
        {
        }
    }
}