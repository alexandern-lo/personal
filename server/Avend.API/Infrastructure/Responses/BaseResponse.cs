using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Avend.API.Infrastructure.Responses
{
    /// <summary>
    /// Base class for API responses. 
    /// It introduced Success property that is required for any API response.
    /// Can only be instantiated by descendants.
    /// </summary>
    [DataContract]
    public class BaseResponse
    {
        protected BaseResponse(bool success)
        {
            Success = success;
        }

        /// <summary>
        /// Gets or Sets Success status
        /// </summary>
        [DataMember(Name = "success", Order = 0)]
        public bool Success { get; private set; }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}