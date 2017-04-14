using System.Linq;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.Models.Services
{
    public class CrmService
    {
        readonly ProfileService ProfileService;

        public CrmService(ProfileService profileService)
        {
            ProfileService = profileService;
        }

        public CrmType CurrentCrmType
        {
            get
            {
                var abbr = ProfileService.Profile?.CrmConfiguration?.Type;
                return CrmTypeForAbbreviation(abbr);
            }
        }

        public bool CurrentCrmIsEnabled
        {
            get
            {
                var config = CurrentCrmConfiguration;
                if (config == null) return false;
                return config.Authorized; // TODO: need better test for being enabled
            }
        }

        public UserCrmConfigurationDTO CurrentCrmConfiguration
        {
            get
            {
                return ProfileService.Profile?.CrmConfiguration;
            }
        }

        #region CRM Types

        public enum CrmType
        {
            Salesforce,
            Dynamics365,
            Other
        }

        CrmType CrmTypeForAbbreviation(CrmTypeAbbreviations? abbreviation)
        {
            switch (abbreviation)
            {
                case CrmTypeAbbreviations.Dynamics365:
                    return CrmType.Dynamics365;
                case CrmTypeAbbreviations.Salesforce:
                    return CrmType.Salesforce;
            }
            return CrmType.Other;
        }

        #endregion
    }
}
