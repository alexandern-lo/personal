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
    public class BaseRecord
    {
        /// <summary>
        /// Bigint identifier representing the specific record.
        /// </summary>
        /// <value>Bigint identifier representing the specific record.</value>
        [Key]
        public virtual long Id { get; set; }

        /// <summary>
        /// Unique identifier representing the specific record.
        /// </summary>
        /// <value>Unique identifier representing the specific record.</value>
        public virtual Guid Uid { get; set; }

        /// <summary>
        /// Date and time of the record creation.
        /// </summary>
        /// <value>Date and time of the record creation.</value>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time of last record update.
        /// </summary>
        /// <value>Date and time of last record update. Can be blank.</value>
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            });
        }
    }
}