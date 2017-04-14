using System;
using System.Collections.Generic;
using Avend.API.Model;
using Avend.OAuth;

namespace Avend.API.Services.Crm
{
    /// <summary>
    /// Factory class to get proper CRM connector by condition.
    /// </summary>
    public class CrmConnectorFactory
    {
        public Dictionary<CrmSystemAbbreviation, OAuthConfig> Configs { get; }

        /// <summary>
        /// Global factory method 
        /// </summary>
        /// 
        /// <param name="settings"></param>
        public CrmConnectorFactory(AppSettingsCrm settings)
        {
            Configs = settings.CrmConfigs;
        }

        /// <summary>
        /// Factory method to get connector for a given CRM system.
        /// </summary>
        /// 
        /// <param name="system">CrmSystem to instantiate connector for</param>
        /// 
        /// <returns>Properly typed and initialized connector for given CRM system</returns>
        public BaseCrmConnector GetConnectorForCrmSystem(CrmSystemAbbreviation system)
        {
            switch (system)
            {
                case CrmSystemAbbreviation.Salesforce:
                    return new SalesForceConnector(Configs[system]);

                case CrmSystemAbbreviation.Dynamics365:
                    return new Dynamics365Connector(Configs[system]);

                default:
                    throw new ArgumentOutOfRangeException($"Connector for CRM with abbreviation {system} is not implemented yet");
            }
        }
    }
}