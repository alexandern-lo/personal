using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.Resources
{
    public abstract class ResourcesCreateUpdateTestBase : ResourcesTestBase
    {
        protected ResourceDto Resource;
        protected abstract HttpRequestMessage MakeRequest();

        [TestMethod]
        public async Task CreateUpdate()
        {
            var resource = await BobTA.SendJsonAsync(MakeRequest(), Resource)
                .AvendResponse<ResourceDto>();
            resource.User.Uid.Should().Be(TestUser.BobTester.Uid);
            resource.Tenant.Uid.Should().Be(TestUser.BobTester.SubscriptionUid);
            resource.Name.Should().Be(Resource.Name);
            resource.MimeType.Should().Be(Resource.MimeType);
            resource.Description.Should().Be(Resource.Description);
        }

        [TestMethod]
        public async Task EmptyResourceName()
        {
            Resource.Name = " ";
            await BobTA.SendJsonAsync(MakeRequest(), Resource).Response(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task InvalidUri()
        {
            Resource.Url = "this is not url";
            await BobTA.SendJsonAsync(MakeRequest(), Resource).Response(HttpStatusCode.BadRequest);
        }
    }
}