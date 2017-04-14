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
    public class DeleteAttendeeCategoryTest : AttendeeCategoriesTestBase
    {
        public TestAttendeeCategory C1, C2, C3;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();
            C1 = await AlexEvents.Addcategory(EventUid, "Category 1", "Option 1");
            C2 = await AlexEvents.Addcategory(EventUid, "Category 2");
            C3 = await AlexEvents.Addcategory(EventUid, "Category 3");
        }

        [TestMethod]
        public async Task MassDelete()
        {
            await AlexSA.PostJsonAsync($"events/{EventUid}/attendee_categories/delete", new[] {C1.Category.Uid, C3.Category.Uid})
                .Response();
            var survivals = await AlexSA.GetJsonAsync($"events/{EventUid}/attendee_categories")
                .AvendListResponse<AttendeeCategoryDto>(1);
            survivals[0].Uid.Should().Be(C2.Category.Uid);
        }

        [TestMethod]
        public async Task CanDeleteCategoriesWithAttendees()
        {
            var attendee = EventData.MakeAttendee(C1.Value("Option 1"));
            await AlexEvents.AddAttendee(EventUid, attendee);
            await AlexSA.PostJsonAsync($"events/{EventUid}/attendee_categories/delete", new[] {C1.Category.Uid})
                .Response();
        }
    }
}
