using System;
using System.Runtime.Serialization;
using Avend.API.Services.Subscriptions;
using Avend.API.Services.Subscriptions.NetworkDTO;

namespace Avend.API.Model.NetworkDTO
{
    /// <summary>
    /// Network data object for resource entity.
    /// </summary>
    [DataContract(Name = "resource")]
    public class ResourceDto
    {
        /// <summary>
        /// Unique identifier representing the specific resource.
        /// </summary>
        /// <value>Unique identifier representing the specific resource.</value>
        [DataMember(Name = "resource_uid")]
        public Guid? Uid { get; set; }

        /// <summary>
        /// Unique identifier of the event the resource belongs to.
        /// </summary>
        /// <value>Unique identifier of the event the resource belongs to.</value>
        [DataMember(Name = "event_uid")]
        public Guid? EventUid { get; set; }

        /// <summary>
        /// Resource name
        /// </summary>
        /// <value>Resource name</value>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Resource description
        /// </summary>
        /// <value>Resource description</value>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Resource MIME type
        /// </summary>
        /// <value>Resource MIME type</value>
        [DataMember(Name = "type")]
        public string MimeType { get; set; }

        /// <summary>
        /// Resource URL
        /// </summary>
        /// <value>Resource URL</value>
        [DataMember(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Date and time of the record's creation.
        /// </summary>
        /// <value>Date and time of the record's creation.</value>
        [DataMember(Name = "created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time of the record's last update.
        /// </summary>
        /// <value>Date and time of the record's last update.</value>
        [DataMember(Name = "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [DataMember(Name = "user")]
        public SubscriptionMemberDto User { get; set; }

        [DataMember(Name = "tenant")]
        public TenantDto Tenant { get; set; }

        public void UpdateResource(Resource obj)
        {
            if (Name != null)
                obj.Name = Name;

            if (Description != null)
                obj.Description = Description;

            if (MimeType != null)
                obj.MimeType = MimeType;

            if (Url != null)
                obj.Url = Url;
        }

        /// <summary>
        /// Convert persisitent Resource entity to ResourceDto.
        /// </summary>
        public static ResourceDto From(Resource obj)
        {
            return From(obj, null, null);
        }

        /// <summary>
        /// Use this version to create ResourceDto from fresh resource which might not have correct user and tenant links.
        /// User and tenant are still taken from model object if possible.
        /// </summary>
        public static ResourceDto From(Resource obj, SubscriptionMemberDto user, TenantDto tenant)
        {
            if (obj.User != null) user = SubscriptionMemberDto.From(obj.User);
            if (obj.User?.Subscription != null) tenant = TenantDto.From(obj.User.Subscription);

            var dto = new ResourceDto
            {
                Uid = obj.Uid,
                MimeType = obj.MimeType,
                Name = obj.Name,
                Description = obj.Description,
                Url = obj.Url,
                User = user,
                Tenant = tenant,
                CreatedAt = obj.CreatedAt,
                UpdatedAt = obj.UpdatedAt,
            };

            if (obj.Event != null)
            {
                dto.EventUid = obj.Event.Uid;
            }

            return dto;
        }
    }
}