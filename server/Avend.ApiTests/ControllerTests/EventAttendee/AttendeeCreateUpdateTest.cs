using System.Linq;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventAttendee
{
    [TestClass]
    public class AttendeeCreateUpdateTest : AttendeesControllerTestBase
    {
        [TestMethod]
        public async Task Create()
        {
            var attendee = await CreateAttendee();

            attendee.Uid.Should().HaveValue();
            attendee.FirstName.Should().Be("AttendeeRecord");
            attendee.LastName.Should().Be("1");
            attendee.CategoryValues.Select(x => x.OptionUid).Should()
                .ContainInOrder(Prop1.Option("Opt1.2"));
        }

        [TestMethod]
        public async Task Update()
        {
            var attendee = await CreateAttendee();
            attendee.FirstName = "Updated";
            attendee.CategoryValues = null;
            await BobTA.PutJsonAsync($"events/{EventData.Event.Uid}/attendees/{attendee.Uid}", attendee)
                .Response();
            attendee = await BobTA.GetJsonAsync($"events/{EventData.Event.Uid}/attendees/{attendee.Uid}")
                .AvendResponse<AttendeeDto>();
            attendee.FirstName.Should().Be("Updated");
        }

        private async Task<AttendeeDto> CreateAttendee()
        {
            var attendee = EventData.MakeAttendee(Prop1.Value("Opt1.2"), Prop2.Value("Opt2.1"));
            attendee.FirstName = "AttendeeRecord";
            attendee.LastName = "1";

            attendee = await EventData.AddAttendee(EventData.Event.Uid, attendee);
            return attendee;
        }
    }
}