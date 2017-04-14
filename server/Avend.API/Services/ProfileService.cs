using System;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Logging;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Crm;
using Microsoft.Extensions.Logging;
using Qoden.Validation;

namespace Avend.API.Services
{
    /// <summary>
    /// Manage user profile and settings.
    /// </summary>
    public class ProfileService
    {
        public ProfileService(
            UserContext userContext,
            UserCrmDtoBuilder userCrmConfigBuilder,
            UsersManagementService usersService,
            CrmConfigurationService crmService)
        {
            Assert.Argument(userContext, nameof(userContext)).NotNull();
            Assert.Argument(userCrmConfigBuilder, nameof(userCrmConfigBuilder)).NotNull();
            Assert.Argument(usersService, nameof(usersService)).NotNull();
            Assert.Argument(crmService, nameof(crmService)).NotNull();

            UserContext = userContext;
            Logger = AvendLog.CreateLogger(nameof(ProfileService));
            UserCrmConfigBuilder = userCrmConfigBuilder;
            UsersManagementService = usersService;
            CrmService = crmService;
        }

        public CrmConfigurationService CrmService { get; set; }
        public UsersManagementService UsersManagementService { get; }
        public UserCrmDtoBuilder UserCrmConfigBuilder { get; }
        public ILogger Logger { get; }
        public UserContext UserContext { get; }

        public async Task<UserProfileDto> GetProfile()
        {
            var settings = await UsersManagementService.GetSettingsForUser(UserContext.UserUid);
            var defautlCrm = settings?.DefaultCrm != null ? UserCrmConfigBuilder.Build(settings.DefaultCrm) : null;
            DateTime? acceptedAt = null;
            var termsObj = UsersManagementService.GetUserTermsStatus(UserContext.UserUid, ref acceptedAt);
            return GetProfile(defautlCrm, termsObj, acceptedAt);
        }

        public async Task<UserCrmDto> UpdateDefaultCrm(Guid uid)
        {
            return await CrmService.Update(uid, new UserCrmDto()
            {
                Default = true
            });
        }

        private UserProfileDto GetProfile(UserCrmDto defaultCrm, Terms termsObj, DateTime? acceptedAt)
        {
            return new UserProfileDto
            {
                DefaultCrm = defaultCrm,
                CurrentSubscription = UserContext.Subscription,
                User = UserContext.Member,
                Tenant = UserContext.Tenant,
                AcceptedTerms = termsObj == null ? null : TermsDTO.From(termsObj, acceptedAt),
            };
        }
    }
}