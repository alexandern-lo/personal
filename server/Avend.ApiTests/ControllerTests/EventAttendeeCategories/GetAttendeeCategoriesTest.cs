using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventAttendeeCategories
{
    [TestClass]
    public class GetAttendeeCategoriesTest : AttendeeCategoriesTestBase
    {
        protected TestAttendeeCategory CategoryA;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            CategoryA = await AlexEvents.Addcategory(EventUid, "AAA");
            await AlexEvents.Addcategory(EventUid, "BBB");
            await AlexEvents.Addcategory(EventUid, "CCC");
        }


        [TestMethod]
        public async Task GetAttendees()
        {
            var categories = await AlexSA.GetJsonAsync($"events/{EventUid}/attendee_categories")
                .AvendListResponse<AttendeeCategoryDto>(3);
            categories.Select(x => x.Name).Should().Equal("AAA", "BBB", "CCC");
        }

        [TestMethod]
        public async Task SortByName()
        {
            var categories = await AlexSA.GetJsonAsync($"events/{EventUid}/attendee_categories?sort_field=name&sort_order=desc")
                .AvendListResponse<AttendeeCategoryDto>(3);
            categories.Select(x => x.Name).Should().Equal("CCC", "BBB", "AAA");
        }


        [TestMethod]
        public async Task Pagination()
        {
            var categories = await AlexSA.GetJsonAsync($"events/{EventUid}/attendee_categories?page=1&per_page=2")
                .AvendListResponse<AttendeeCategoryDto>(3);
            categories.Select(x => x.Name).Should().Equal("CCC");
        }

        [TestMethod]
        public async Task RoleAccess()
        {
            await MarcTA.GetJsonAsync($"events/{EventUid}/attendee_categories")
                .AvendListResponse<AttendeeCategoryDto>(3);

            await CecileSU.GetJsonAsync($"events/{EventUid}/attendee_categories")
                .AvendListResponse<AttendeeCategoryDto>(3);
        }

        [TestMethod]
        public async Task GetByUid()
        {
            var category = await MarcTA.GetJsonAsync($"events/{EventUid}/attendee_categories/{CategoryA.Category.Uid}")
                .AvendResponse<AttendeeCategoryDto>();
            category.Uid.Should().Be(CategoryA.Category.Uid);
            category.Name.Should().Be(CategoryA.Category.Name);
            category.Options.Should().HaveCount(CategoryA.Category.Options.Count);
        }
    }
}
