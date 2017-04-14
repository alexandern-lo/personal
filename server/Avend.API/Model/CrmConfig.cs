using Avend.OAuth;
using System.Collections.Generic;

namespace Avend.API.Model
{
    /// <summary>
    /// A container mapping application settings for all crm configurations.
    /// </summary>
    public class AppSettingsCrm
    {
        /// <summary>
        /// A dictionary for all CrmConfigs basing on its abbreviation.
        /// </summary>
        public Dictionary<CrmSystemAbbreviation, OAuthConfig> CrmConfigs { get; }

        public AppSettingsCrm()
        {
            CrmConfigs = new Dictionary<CrmSystemAbbreviation, OAuthConfig>();
        }
    }
}