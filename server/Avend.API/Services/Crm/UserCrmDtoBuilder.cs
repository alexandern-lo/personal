using System;
using System.Collections.Generic;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Newtonsoft.Json;
using Qoden.Validation;

namespace Avend.API.Services.Crm
{
    public class UserCrmDtoBuilder
    {
        public UserCrmDtoBuilder(CrmConnectorFactory connectorFactory)
        {
            Assert.Argument(connectorFactory, "connectorFactory").NotNull();
            ConnectorFactory = connectorFactory;
        }

        public CrmConnectorFactory ConnectorFactory { get; }

        public UserCrmDto Build(CrmRecord config)
        {
            Uri dynamics365Uri;
            Uri.TryCreate(config.Url, UriKind.Absolute, out dynamics365Uri);
            var connector = ConnectorFactory.GetConnectorForCrmSystem(config.CrmType);
            return new UserCrmDto
            {
                Uid = config.Uid,
                Type = config.CrmType,
                Name = config.Name,
                Authorized = !string.IsNullOrWhiteSpace(config.RefreshToken),
                SyncFields = JsonConvert.DeserializeObject<Dictionary<string, bool>>(config.SyncFields),
                Url = dynamics365Uri,
                AuthorizationUrl = connector.ConstructAuthorizationUrl(config),
                Default = config.Settings?.DefaultCrmId == config.Id
            };
        }
    }
}
