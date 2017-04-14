using System;

namespace Avend.API.Services.Exceptions
{
    public class DuplicateRecordCreationException : Exception
    {
        public Guid RecordUid { get; }
        public Guid ClientsideUid { get; }

        public DuplicateRecordCreationException(Guid clientsideUid, Guid recordUid, string message)
            : base(message)
        {
            ClientsideUid = clientsideUid;
            RecordUid = recordUid;
        }
    }
}