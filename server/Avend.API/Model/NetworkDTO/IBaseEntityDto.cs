using System;

namespace Avend.API.Model.NetworkDTO
{
    /// <summary>
    /// Base interface for DTOs that expose Uid, UserUid and SubscriptionUid.
    /// </summary>
    public interface IBaseEntityDto
    {
        Guid? Uid { get; set; }
        Guid? UserUid { get; set; }
        Guid? SubscriptionUid { get; set; }
    }
}