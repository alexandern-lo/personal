using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.CrmConfiguration
{
    [TestClass]
    public class CrmConfigurationUpdateTest : CrmConfigurationCreateUpdateTestBase
    {
        protected UserCrmDto Crm;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            var syncFields = new Dictionary<string, bool>()
            {
                {"first_name", true},
                {"last_name", true},
                {"zip_code", true}
            };
            DynamicsCrm.SyncFields = syncFields;
            DynamicsCrm.Default = false;
            Crm = await Create(DynamicsCrm);
        }

        [TestMethod]
        public async Task CanChangeCrmSystem()
        {
            Crm.Type = CrmSystemAbbreviation.Salesforce;
            await BobTA.SendJsonAsync(MakeRequest(), Crm).Response();
        }

        [TestMethod]
        public async Task NotFound()
        {
            Crm.Uid = Guid.NewGuid();
            await BobTA.SendJsonAsync(MakeRequest(), Crm).Response(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task CanSetDefaultToFalse()
        {
            DynamicsCrm = await BobTA.SendJsonAsync(MakeRequest(), new {@default = true})
                .AvendResponse<UserCrmDto>();
            DynamicsCrm.Default.Should().Be(true, "put only 'default' flag");
        }

        [TestMethod]
        public async Task SetDefaultViaProfile()
        {
            var profile = await BobTA.GetJsonAsync("profile").AvendResponse<UserProfileDto>();
            profile.DefaultCrm.Should().BeNull();

            await BobTA.PutJsonAsync("profile/crm", Crm.Uid).Response();

            profile = await BobTA.GetJsonAsync("profile").AvendResponse<UserProfileDto>();
            profile.DefaultCrm.Uid.Should().Be(Crm.Uid);
        }

        protected override HttpRequestMessage MakeRequest()
        {
            return new HttpRequestMessage(HttpMethod.Put, "crm/" + Crm.Uid);
        }
    }
}