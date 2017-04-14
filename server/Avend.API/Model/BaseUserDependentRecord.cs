using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

namespace Avend.API.Model
{
    /// <summary>
    /// Base record having following fields (columns):
    /// <list type="bullet">
    /// <item>Id [Bigint]</item>
    /// <item>Uid [Guid]</item>
    /// <item>CreatedAt [DateTime]</item>
    /// <item>UpdatedAt [DateTime]</item>
    /// </list>
    /// </summary>
    public class BaseUserDependentRecord : BaseRecord
    {
        /// <summary>
        /// Unique identifier of the user owning this record.
        /// </summary>
        /// <value>Unique identifier of the user owning this record.</value>
        [Column("user_uid", TypeName = "UniqueIdentifier")]
        public Guid? UserUid { get; set; }

        /// <summary>
        /// Unique identifier of the tenant whose user is owning this record.
        /// </summary>
        /// <value>Unique identifier of the tenant whose user is owning this record.</value>
        [Column("tenant_uid", TypeName = "UniqueIdentifier")]
        public Guid? TenantUid { get; set; }
    }
}