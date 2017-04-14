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

namespace Avend.ApiTests.ControllerTests.EventAttendeeCategories
{
    public abstract class AttendeeCategoryCreateUpdateTestBase : AttendeeCategoriesTestBase
    {
        protected AttendeeCategoryDto AttendeeCategoryDto;
        public abstract HttpRequestMessage MakeRequest(Guid eventUid);

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            AttendeeCategoryDto = EventData.MakeSampleCategory();
        }

        [TestMethod]
        public async Task ShortName()
        {
            AttendeeCategoryDto.Name = "a";
            await AlexSA.SendJsonAsync(MakeRequest(EventUid), AttendeeCategoryDto)
                .Response(HttpStatusCode.BadRequest, "Cannot create/update category with short name");
        }

        [TestMethod]
        public async Task ShortOptionName()
        {
            AttendeeCategoryDto.Options[1].Name = "";
            await AlexSA.SendJsonAsync(MakeRequest(EventUid), AttendeeCategoryDto)
                .Response(HttpStatusCode.BadRequest,
                    "Cannot create/update category with option which has short or empty name");
        }

        [TestMethod]
        public async Task Success()
        {
            var result = await AlexSA.SendJsonAsync(MakeRequest(EventUid), AttendeeCategoryDto)
                .AvendResponse<AttendeeCategoryDto>();

            result.Uid.Should().NotBeNull();
            result.Name.Should().Be(AttendeeCategoryDto.Name);
            foreach (var opt in result.Options)
            {
                opt.Uid.Should().NotBeNull();
                AttendeeCategoryDto.Options.Should().Contain(x => x.Name == opt.Name);
            }
        }

        [TestMethod]
        public async Task Permissions()
        {
            await CecileSU.SendJsonAsync(MakeRequest(EventUid), AttendeeCategoryDto)
                .Response(HttpStatusCode.Forbidden);

            await BobTA.SendJsonAsync(MakeRequest(EventUid), AttendeeCategoryDto)
                .Response(HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task EventNotFound()
        {
            var request = MakeRequest(Guid.NewGuid());
            await AlexSA.SendJsonAsync(request, AttendeeCategoryDto)
                .Response(HttpStatusCode.NotFound);
        }
    }
}