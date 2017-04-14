using System;

namespace Avend.API.Model.NetworkDTO
{
    public interface IEventBasedDto
    {
        Guid? EventUid { get; set; }
    }
}