using System;
using System.Linq;
using Avend.API.Infrastructure;
using Avend.API.Infrastructure.Logging;
using Avend.API.Model;
using Avend.API.Services;
using Microsoft.Extensions.Logging;
using Qoden.Validation;

namespace Avend.API.BL
{
    /// <summary>
    /// Base class for request processors (both reads and writers) that work with certain DTO class.
    /// 
    /// Uses AvendDbContext and provides user and subscription validation.
    /// </summary>
    /// 
    /// <typeparam name="TDto">DTO object to be used with this request processor</typeparam>
    public class BaseRequestProcessor<TDto> 
        where TDto : class, new()
    {
        public ILogger Logger { get; }
        public IValidator Validator { get; }

        public Guid? UserUid { get; private set; }
        public Guid? TenantUid { get; private set; }
        public SubscriptionRecord UserSubscription { get; private set; }

        public AvendDbContext AvendDbContext { get; }

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="avendDbContext">AvendDbContext required to perform subscription validation.</param>
        public BaseRequestProcessor(AvendDbContext avendDbContext)
        {
            Assert.Argument(avendDbContext, nameof(avendDbContext)).NotNull();

            Logger = AvendLog.CreateLogger(GetType().Name);
            AvendDbContext = avendDbContext;

            Validator = new Validator();
        }

        /// <summary>
        /// Validates and assigns user and subscription from UserContext.
        /// 
        /// User UID is validated to be present (check for not null).
        /// User is validated to have a corresponding subscription_member record.
        /// Subscription UID is validated to be not null.
        /// </summary>
        /// 
        /// <param name="userContext">UserContext data to get user and subscription from.</param>
        /// 
        /// <returns>True if validation is passed successfully.</returns>
        public bool SetAndValidateUserAndSubscriptionFromUserContext(UserContext userContext)
        {
            Validator.CheckValue(userContext, nameof(userContext)).NotNull();
            Validator.CheckValue(userContext.Member, nameof(userContext) + "." + nameof(userContext.Member)).NotNull();
            Validator.CheckValue(userContext.Subscription, nameof(userContext) + "." + nameof(userContext.Subscription)).NotNull();

            if (Validator.HasErrors)
                return false;

            UserUid = userContext.Member.Uid;
            TenantUid = userContext.Subscription.Uid;

            //  Validator.CheckValue(UserSubscription).SubscriptionIsValid();

            return Validator.IsValid;
        }

        /// <summary>
        /// Validates and assigns user and subscription from explicit values.
        /// 
        /// User UID is validated to be present (check for not null).
        /// Subscription UID is validated to be not null.
        /// </summary>
        /// 
        /// <param name="userUid">User UID to be used in further requests</param>
        /// <param name="tenantUid">Subscription UID to be used in further requests</param>
        /// 
        /// <returns>True if validation is passed successfully.</returns>
        public bool SetAndValidateExplicitUserAndSubscription(Guid? userUid, Guid? tenantUid)
        {
            Validator.CheckValue(userUid, nameof(userUid)).NotNull();
            Validator.CheckValue(tenantUid, nameof(tenantUid)).NotNull();

            if (Validator.HasErrors)
                return false;

            UserUid = userUid;
            TenantUid = tenantUid;

            return Validator.IsValid;
        }

        /// <summary>
        /// Auxiliary method to convert the camel-cased string into snake-cased.
        /// </summary>
        /// 
        /// <param name="str">String in camel-case to convert</param>
        /// 
        /// <returns>Snake-cased string</returns>
        public static string StringToSnakeCase(string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        /// <summary>
        /// Basic property for getting entity name from DTO class name.
        /// 
        /// DTO class name is converted into snake case, then stripped 
        /// off DTO suffix (both DTO and Dto versions are stripped) and
        /// finally underscores are replaced with spaces.
        /// 
        /// Can be overroden to provide a more meaningful entity name.
        /// </summary>
        private string entityName;
        public virtual string EntityName
        {
            get
            {
                if (entityName == null)
                {
                    entityName = StringToSnakeCase(typeof(TDto).Name);

                    int dtoSuffixPosition;

                    dtoSuffixPosition = entityName.IndexOf("_d_t_o", StringComparison.Ordinal);

                    if (dtoSuffixPosition == entityName.Length - 6)
                        entityName = entityName.Substring(0, entityName.Length - 6);

                    dtoSuffixPosition = entityName.IndexOf("_dto", StringComparison.Ordinal);

                    if (dtoSuffixPosition == entityName.Length - 4)
                        entityName = entityName.Substring(0, entityName.Length - 4);

                    entityName = entityName.Replace('_', ' ');
                }

                return entityName;
            }
        }
    }
}
