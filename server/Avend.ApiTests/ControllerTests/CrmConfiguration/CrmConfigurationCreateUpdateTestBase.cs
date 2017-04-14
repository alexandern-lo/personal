using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.CrmConfiguration
{
    [TestClass]
    public abstract class CrmConfigurationCreateUpdateTestBase : CrmConfigurationTestBase
    {
        protected UserCrmDto DynamicsCrm;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            DynamicsCrm = MakeConfigurationDto(Dynamics);
        }

        protected abstract HttpRequestMessage MakeRequest();

        [TestMethod]
        public async Task CreateUpdate()
        {
            var syncFields = new Dictionary<string, bool>()
            {
                {"first_name", true},
                {"last_name", true}
            };
            DynamicsCrm.SyncFields = syncFields;
            DynamicsCrm.Default = true;
            var response = await BobTA.SendJsonAsync(MakeRequest(), DynamicsCrm)
                .AvendResponse<UserCrmDto>();
            response.Name.Should().Be(DynamicsCrm.Name);
            response.Type.Should().Be(DynamicsCrm.Type);
            response.Url.Should().Be(DynamicsCrm.Url);            
            response.Authorized.Should().Be(false);
            response.AuthorizationUrl.Should().NotBeNullOrWhiteSpace();
            foreach (var kv in response.SyncFields)
            {
                kv.Value.Should().Be(syncFields.ContainsKey(kv.Key), "Sync fields enabled/disabled as specified");
            }
        }

        [TestMethod]
        public async Task NameRequired()
        {
            DynamicsCrm.Name = " ";
            await BobTA.SendJsonAsync(MakeRequest(), DynamicsCrm).Response(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task DynamicsUrlRequired()
        {
            DynamicsCrm.Url = new Uri("/invalid/url", UriKind.Relative);
            await BobTA.SendJsonAsync(MakeRequest(), DynamicsCrm).Response(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task SetDefault()
        {
            var defaultCrm = await Create(MakeConfigurationDto(Salesforce, e => e.Default = true));
            defaultCrm.Default.Should().Be(true);

            DynamicsCrm.Default = true;
            DynamicsCrm = await BobTA.SendJsonAsync(MakeRequest(), DynamicsCrm)
                .AvendResponse<UserCrmDto>();
            DynamicsCrm.Default.Should().Be(true, "set new CRM as default");

            defaultCrm = await BobTA.GetJsonAsync($"crm/{defaultCrm.Uid}")
                .AvendResponse<UserCrmDto>();
            defaultCrm.Default.Should().Be(false, "only one CRM can be default at a time");
        }
    }
}