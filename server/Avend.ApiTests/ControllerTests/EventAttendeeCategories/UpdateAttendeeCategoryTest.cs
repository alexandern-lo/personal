using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventAttendeeCategories
{
    [TestClass]
    public class UpdateAttendeeCategoryTest : AttendeeCategoryCreateUpdateTestBase
    {
        protected AttendeeCategoryDto Category;
        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            Category = await AlexSA.PostJsonAsync($"events/{EventUid}/attendee_categories", AttendeeCategoryDto)
                .AvendResponse<AttendeeCategoryDto>();
        }

        public override HttpRequestMessage MakeRequest(Guid eventUid)
        {
            return new HttpRequestMessage(HttpMethod.Put, $"events/{eventUid}/attendee_categories/{Category.Uid}");
        }


        [TestMethod]
        public async Task UpdateMergesOptions()
        {
            var options = new List<AttendeeCategoryOptionDto>(Category.Options);
            Category.Options[1] = new AttendeeCategoryOptionDto()
            {
                Name = "New Option"
            };
            Category.Options[0].Name = "Updated Option";

            var updated = await AlexSA.SendJsonAsync(MakeRequest(EventUid), Category)
                .AvendResponse<AttendeeCategoryDto>();

            updated.Options[0].Uid.Should().Be(options[0].Uid);
            updated.Options[0].Name.Should().Be("Updated Option");
            updated.Options[1].Uid.Should().NotBe(options[1].Uid.GetValueOrDefault());
            updated.Options[1].Name.Should().Be("New Option");
            updated.Options.Count.Should().Be(2);
        }
    }
}