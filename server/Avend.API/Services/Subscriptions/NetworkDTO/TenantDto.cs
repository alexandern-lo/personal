using System;
using System.Runtime.Serialization;

using Avend.API.Model;

namespace Avend.API.Services.Subscriptions.NetworkDTO
{
    /// <summary>
    /// Contains basic data on all tenants in the system.
    /// </summary>
    [DataContract]
    public class TenantDto
    {
        /// <summary>
        /// Uid of the subscription/tenant in system database.
        /// </summary>
        /// <value>Uid of the subscription/tenant in system database.</value>
        [DataMember(Name = "tenant_uid")]
        public Guid? Uid { get; set; }

        /// <summary>
        /// Company name of the tenant/subscription.
        /// </summary>
        /// <value>Company name of the tenant/subscription.</value>
        [DataMember(Name = "company_name")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Constructs proper TenantDto object based on SubscriptionRecord object passed over.
        /// </summary>
        /// <param name="obj">Object to construct DTO from</param>
        /// <returns>Properly populated TenantDto object</returns>
        public static TenantDto From(SubscriptionRecord obj)
        {
            var dto = new TenantDto()
            {
                Uid = obj.Uid,
                CompanyName = obj.Name,
            };

            return dto;
        }
    }
}