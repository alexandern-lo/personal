using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventAttendee
{
    [TestClass]
    [TestCategory("Integrational")]
    public class GetEventAttendeesTest : AttendeesControllerTestBase
    {
        protected AttendeeDto Attendee1, Attendee2, Attendee3;

        [TestInitialize]
        public override async Task Init()
        {
            await base.Init();

            Attendee1 = EventData.MakeAttendee(Prop1.Value("Opt1.2"), Prop2.Value("Opt2.1"));
            Attendee1.FirstName = "AttendeeRecord";
            Attendee1.LastName = "1";

            Attendee2 = EventData.MakeAttendee(Prop1.Value("Opt1.1"), Prop2.Value("Opt2.2"));
            Attendee2.FirstName = "AttendeeRecord";
            Attendee2.LastName = "2";

            Attendee3 = EventData.MakeAttendee(Prop1.Value("Opt1.1"));
            Attendee3.FirstName = "AttendeeRecord";
            Attendee3.LastName = "3";

            Attendee1 = await EventData.AddAttendee(EventData.Event.Uid, Attendee1);
            await EventData.AddAttendee(EventData.Event.Uid, Attendee2);
            await EventData.AddAttendee(EventData.Event.Uid, Attendee3);

            //create a bit of excess data to test search does not find it
            var excessEvent = await EventData.InitWithSampleEvent(TestUser.BobTester, System);
            var excessAttendee = EventData.MakeAttendee();
            excessAttendee.FirstName = "AttendeeRecord";
            excessAttendee.LastName = "4444";
            await excessEvent.AddAttendee(excessEvent.Event.Uid, excessAttendee);
        }

        [TestMethod]
        public async Task GetEventAttendeesList()
        {
            await BobTA.GetJsonAsync($"events/{EventUid}/attendees")
                .AvendListResponse<AttendeeDto>(3, HttpStatusCode.OK, "3 attendees created");

            await BobTA.GetJsonAsync($"events/{EventUid}/attendees?filter=AttendeeRecord")
                .AvendListResponse<AttendeeDto>(3, HttpStatusCode.OK, "3 attendees created, all match filter");

            await BobTA.GetJsonAsync($"events/{EventUid}/attendees?filter=AttendeeRecord%201")
                .AvendListResponse<AttendeeDto>(1, HttpStatusCode.OK, "3 attendees created, only 1 match filter");
        }

        [TestMethod]
        //Filter attendees by category value with two options out out of which only one match
        //Option values inside singe category works as 'OR' filter
        public async Task GetAttendees_ExcessOptions()
        {
            var filter = new AttendeeFilterBuilder();
            filter.AddProperty(Prop1, Prop1.Option("Opt1.1"), Prop1.Option("Opt1.3"));
            var attendees = await BobTA.PostJsonAsync(
                    $"events/{EventData.Event.Uid}/attendees/filter?sort_field=last_name",
                    filter.Dto)
                .AvendListResponse<AttendeeDto>(2, HttpStatusCode.OK, "Attendees 2 and 3 match");

            attendees.Select(x => x.LastName)
                .Should()
                .ContainInOrder(new[] {"2", "3"}, "AttendeeRecord 2 and 3 both have prop Opt1.1");
        }

        [TestMethod]
        //Filter attendees with several categories
        //Categories works as 'AND' filter, means attendee has to have at least one matching option for every 
        //requested category
        public async Task GetAttendees_SeveralCategories()
        {
            var filter = new AttendeeFilterBuilder();
            filter.AddProperty(Prop1, Prop1.Option("Opt1.1"));
            filter.AddProperty(Prop2, Prop2.Option("Opt2.2"));
            var attendees = await BobTA.PostJsonAsync(
                    $"events/{EventData.Event.Uid}/attendees/filter?sort_field=last_name",
                    filter.Dto)
                .AvendListResponse<AttendeeDto>(1, HttpStatusCode.OK, "Only 'AttendeeRecord 2' matched");

            attendees.Select(x => x.LastName)
                .Should()
                .ContainInOrder(new[] {"2"}, "Only AttendeeRecord 2 has props Opt1.1 and Opt2.2 at the same time");
        }

        [TestMethod]
        public async Task Filter()
        {
            var all = await BobTA.PostJsonAsync(
                    $"events/{EventData.Event.Uid}/attendees/filter?sort_order=desc&sort_field=last_name",
                    new AttendeesFilterRequestDTO())
                .AvendListResponse<AttendeeDto>(3, HttpStatusCode.OK, "All attendees match");
            all.Select(x => x.LastName).Should().ContainInOrder("3", "2", "1");
        }

        public class AttendeeFilterBuilder
        {
            public AttendeesFilterRequestDTO Dto { get; }

            public AttendeeFilterBuilder(string filter = null)
            {
                Dto = new AttendeesFilterRequestDTO
                {
                    Query = filter,
                    Categories = new List<AttendeesFilterCategoryValuesDTO>()
                };
            }

            public void AddProperty(TestAttendeeCategory property, params Guid[] values)
            {
                Dto.Categories.Add(new AttendeesFilterCategoryValuesDTO
                {
                    Uid = property.Category.Uid.GetValueOrDefault(),
                    Values = new List<Guid>(values)
                });
            }
        }
    }
}