using System;
using System.Collections.Generic;
using System.Linq;
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
    [TestClass]
    public class ResourceDeleteTest : ResourcesTestBase
    {
        protected ResourceDto R1, R2, R3;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            R1 = await BobResources.Add(r => r.Name = "AAA");
            R2 = await BobResources.Add(r => r.Name = "BBB");
            R3 = await BobResources.Add(r => r.Name = "CCC");
        }

        [TestMethod]
        public async Task Delete()
        {
            await BobTA.DeleteJsonAsync($"{Resources}/{R1.Uid}").Response();
            var resources = await UserResources(BobTA);
            resources.Select(x => x.Name).Should().Equal("BBB", "CCC");
        }

        [TestMethod]
        public async Task OthersCannotDeleteMyResources()
        {
            await MarcTA.DeleteJsonAsync($"{Resources}/{R1.Uid}")
                .Response(HttpStatusCode.NotFound, "Others does not see TA resources");

            await CecileSU.DeleteJsonAsync($"{Resources}/{R1.Uid}")
                .Response(HttpStatusCode.NotFound, "SU dos not see TA resources");
        }

        [TestMethod]
        public async Task SuperAdminCanDeleteResource()
        {
            await AlexSA.DeleteJsonAsync($"{Resources}/{R1.Uid}").Response();
            var resources = await UserResources(BobTA);
            resources.Select(x => x.Name).Should().Equal("BBB", "CCC");
        }

        [TestMethod]
        public async Task MassDelete()
        {
            await BobTA.PostJsonAsync($"{Resources}/delete", new [] {R1.Uid, R3.Uid}).Response();
            var resources = await UserResources(BobTA);
            resources.Select(x => x.Name).Should().Equal("BBB");
        }
    }
}
