using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using LiveOakApp.Models.Services;

namespace LiveOakApp.Models.Data.NetworkDTO
{

    [DataContract]
    public class ResourceDTO
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Unique identifier representing the specific resource.
        /// </summary>
        /// <value>Unique identifier representing the specific resource.</value>
        [DataMember(Name = "resource_uid")]
        public Guid? Uid { get; set; }

        /// <summary>
        /// Unique identifier representing the current user.
        /// </summary>
        /// <value>Unique identifier representing the current user.</value>
        [DataMember(Name = "user_uid")]
        public Guid? UserUid { get; set; }

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
    }
}
