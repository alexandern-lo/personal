using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Services;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Qoden.Validation;

namespace Avend.API.BL
{
    /// <summary>
    /// Base Writer Request Processor class for CRUD tasks. 
    /// 
    /// Requires record class and corresponding DTO class.
    /// 
    /// Relies upon authentication performed by BaseRequestProcessor.
    /// 
    /// Authorization is to be considered separately for each particular request.
    /// 
    /// It is not intended to be used directly.
    /// </summary>
    /// 
    /// <typeparam name="TRecord">DB Model class this writer is using to create/update/delete.</typeparam>
    /// <typeparam name="TDto">DTO class this writer is using to get data from.</typeparam>
    public class BaseWriter<TRecord, TDto> : BaseRequestProcessor<TDto>
        where TRecord : class, new()
        where TDto : class, new()
    {
        public TDto RequestBody { get; private set; }
        public TRecord PreparedRecord { get; protected set; }

        /// <summary>
        /// Protected constructor. This class is not intended to be used directly.
        /// </summary>
        /// 
        /// <param name="avendDbContext">AvendDbContext required to perform subscription validation.</param>
        /// <param name="logger">Logger to be used.</param>
        protected BaseWriter(
            AvendDbContext avendDbContext
            ) : base(avendDbContext)
        {
        }

        /// <summary>
        /// Sets the request body DTO to be used for record creation / updating.
        /// </summary>
        /// 
        /// <param name="dto">DTO object to set as a body.</param>
        /// 
        /// <returns>True if successful.</returns>
        public bool SetRequestBody(TDto dto)
        {
            RequestBody = dto;

            var recordName = EntityName;

            Logger.LogTrace($"RequestBody of type {recordName} is set to:\n" + JsonConvert.SerializeObject(dto, Formatting.Indented));

            Validator.CheckValue(RequestBody, recordName).ParameterNotNull(typeof(TDto), "null", "Cannot parse the data from the request body");

            return Validator.IsValid;
        }

        /// <summary>
        /// Validates the request body. 
        /// 
        /// Intended to be overriden in child classes. Only checks that the body is not null.
        /// </summary>
        /// 
        /// <returns>True if successful.</returns>
        public virtual bool ValidateRequestBody()
        {
            Validator.CheckValue(RequestBody, "request body").NotNull();

            return Validator.IsValid;
        }

    }
}
