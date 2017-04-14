using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model.NetworkDTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.Events
{
    [TestClass]
    public class EventGetTest : EventsControllerTestBase
    {
        private async Task<string[]> GetEvents(HttpClient user, string url, int total = -1, string message = null,
            HttpStatusCode? code = null)
        {
            code = code ?? HttpStatusCode.OK;
            var events = await user.GetJsonAsync(url)
                .AvendListResponse<EventDto>(total, code.GetValueOrDefault(), message);
            return events.Select(x => x.Name).ToArray();
        }

        [TestMethod]
        public async Task GetEvents_RoleAccess()
        {
            foreach (var url in new[] {"events", "events/uids_and_names"})
            {
                (await GetEvents(CecileSU, url, 2, $"Cecile events at {url}")).Should()
                    .Equal(CecilEvent.Name, AlexEvent.Name);
                (await GetEvents(BobTA, url, 3, $"Bob events at {url}")).Should()
                    .Equal(CecilEvent.Name, AlexEvent.Name, BobEvent.Name);
                (await GetEvents(AlexSA, url, 4, $"Alex events at {url}")).Should()
                    .Equal(CecilEvent.Name, AlexEvent.Name, MarcEvent.Name, BobEvent.Name);
            }
        }

        [TestMethod]
        public async Task Response()
        {
            var cecileEventQuestions = await EventQuestionData.Create(TestUser.CecileTester, System, CecilEvent);
            await cecileEventQuestions.AddQuestion(EventQuestionData.MakeSample());
            await AlexEvents.Addcategory(AlexEvent.Uid, "Category 1", "Value 1", "Value 2");

            var @event = (await CecileSU.GetJsonAsync("events").AvendListResponse<EventDto>())
                .First(x => x.Name == CecilEvent.Name);

            @event.Uid.Should().Be(CecilEvent.Uid);
            @event.Owner.Should().NotBeNull();
            @event.Owner.Uid.Should().Be(TestUser.CecileTester.Uid);
            @event.Tenant.Should().Be("Bob Tester");
            @event.Recurring.Should().Be(true);
            @event.Address.Should().Be(CecileEventRequest.Address);
            @event.Questions.Should().NotBeEmpty();
            @event.Questions[0].Choices.Should().NotBeEmpty();
            @event.AttendeeCategories.Should().BeEmpty();

            @event = (await CecileSU.GetJsonAsync("events").AvendListResponse<EventDto>())
                .First(x => x.Name == AlexEvent.Name);
            @event.Uid.Should().Be(AlexEvent.Uid);
            @event.Owner.Should().NotBeNull();
            @event.Recurring.Should().Be(false);
            @event.Questions.Should().BeEmpty();
            @event.AttendeeCategories.Should().NotBeEmpty();
            @event.AttendeeCategories[0].Options.Should().NotBeEmpty();
        }

        [TestMethod]
        public async Task SortByStartDate()
        {
            var dateOrder = new[] {CecilEvent.Name, AlexEvent.Name, BobEvent.Name};

            var events = await GetEvents(BobTA, "events");
            events.Should()
                .Equal(dateOrder, "Default sort by start date, upcoming first");

            events = await GetEvents(BobTA, "events?sort_order=asc");
            events.Should()
                .Equal(dateOrder.Reverse(), "Order by start_date desc");
        }

        [TestMethod]
        public async Task SortByName()
        {
            var nameOrder = new[] {AlexEvent.Name, BobEvent.Name, CecilEvent.Name};
            var events = await GetEvents(BobTA, "events?sort_field=name");
            events.Should()
                .ContainInOrder(nameOrder, "Order by name default");

            events = await GetEvents(BobTA, "events?sort_field=name&sort_order=desc");
            events.Should()
                .ContainInOrder(nameOrder.Reverse(), "Order by name desc");
        }

        [TestMethod]
        public async Task Pagination()
        {
            var dateOrder = new[] {CecilEvent.Name, AlexEvent.Name, BobEvent.Name};
            var events = await GetEvents(BobTA, "events?per_page=2&page=1", 3, "Only one element on a second page");
            events.Should()
                .ContainInOrder(dateOrder.Skip(2).Take(1));
        }

        [TestMethod]
        public async Task FilterByTenant()
        {
            var dailyEvents = new[] {BobEvent.Name, CecilEvent.Name};

            var events = await GetEvents(AlexSA, $"events?tenant={BobSubscriptionUid}&sort_field=name", 2,
                "super admin can see Bob's events");
            events.Should()
                .ContainInOrder(dailyEvents);

            await GetEvents(BobTA, $"events?tenant={BobSubscriptionUid}&sort_field=name", 2,
                "tenant admin can set filter with itself and it filters out all conference events");

            await GetEvents(BobTA, $"events?tenant={MarcSubscriptionUid}&sort_field=name", 0,
                "tenant admin see empty list when filter by other tenant");
        }

        [TestMethod]
        public async Task FilterByRange()
        {
            var events = await GetEvents(BobTA, "events?range=past", 1);
            events.Should()
                .ContainInOrder(BobEvent.Name);

            events = await GetEvents(BobTA, "events?range=upcoming", 1);

            events.Should()
                .ContainInOrder(CecilEvent.Name);

            events = await GetEvents(BobTA, "events?range=ongoing&sort_field=name", 2);
            events.Should()
                .ContainInOrder(new[] {AlexEvent.Name, CecilEvent.Name},
                    "ongoing events, which is happening now (start_date <= now и end_date >= now or recurring events)");
        }

        [TestMethod]
        public async Task FilterByDate()
        {
            var events = await GetEvents(BobTA, "events?range=ongoing&start_after=2016-10-01&end_before=2017-01-01");
            events.Should().Equal(BobEvent.Name);

            events = await GetEvents(BobTA, "events?range=ongoing&start_after=2016-10-01&sort_field=name");
            events.Should().Equal(AlexEvent.Name, BobEvent.Name, CecilEvent.Name);

            events = await GetEvents(BobTA, "events?range=ongoing&end_before=2017-01-01");
            events.Should().Equal(BobEvent.Name);
        }

        [TestMethod]
        public async Task FilterByIndustry()
        {
            var events = await GetEvents(BobTA, "events?industry=Nuclear%20Power%20Industry&sort_field=name", 2);
            events.Should()
                .ContainInOrder(AlexEvent.Name, BobEvent.Name);
        }

        [TestMethod]
        public async Task FiltersByEventType()
        {
            var conferenceEvents = new[] {AlexEvent.Name};
            var events = await GetEvents(BobTA, "events?event_type=conference&sort_field=name", 1,
                "Only one conference event created in init");
            events.Should()
                .Equal(conferenceEvents);

            var dailyEvents = new[] {BobEvent.Name, CecilEvent.Name};
            events = await GetEvents(BobTA, "events?event_type=personal&sort_field=name&range=all", 2,
                "2 daily events created in init");
            events.Should()
                .Equal(dailyEvents);
        }

        [TestMethod]
        public async Task FiltersByScope()
        {
            var avaialbleEvents = new[] {AlexEvent.Name, BobEvent.Name, CecilEvent.Name};
            var events = await GetEvents(BobTA, "events?scope=available&sort_field=name", avaialbleEvents.Length,
                "SA, own and SU events are visible to Bob");
            events.Should()
                .Equal(avaialbleEvents);

            var selectableEvents = new[] {AlexEvent.Name, BobEvent.Name};
            events = await GetEvents(BobTA, "events?scope=selectable&sort_field=name", selectableEvents.Length,
                "Only conference and own events can be used to create lead");
            events.Should()
                .Equal(selectableEvents);
        }

        [TestMethod]
        public async Task EventsForSubordinate()
        {
            var eventsVisibleToCecile = new[] {AlexEvent.Name, CecilEvent.Name};
            var events = await BobTA.GetJsonAsync($"events?for_user={TestUser.CecileTester.Uid}&sort_field=name")
                .AvendListResponse<EventDto>();
            events.Select(x => x.Name).Should().Equal(eventsVisibleToCecile, 
                "'for_user' query parameter filters request events as if they were requested by given user");

            await BobTA.GetJsonAsync($"events?for_user={TestUser.MarcTester.Uid}&sort_field=name")
                .Response(HttpStatusCode.NotFound, "TA cannot see other tenants events");

            await AlexSA.GetJsonAsync($"events?for_user={TestUser.MarcTester.Uid}&sort_field=name")
                .Response(HttpStatusCode.OK, "SA can see everything");
        }

        [TestMethod]
        public async Task Search()
        {
            var events = await GetEvents(BobTA, "events?q=Bob%20Event", 1);
            events.Should()
                .Equal(BobEvent.Name);
        }

        [TestMethod]
        public async Task GetEventByUid()
        {
            var meetingEvent = await BobTA.GetJsonAsync($"events/{BobEvent.Uid}")
                .AvendResponse<EventDto>();

            meetingEvent.Name.Should()
                .Be(BobEvent.Name, "because returned event's name should be as requested");

            meetingEvent.AttendeeCategories.Count.Should()
                .Be(BobEvent.AttendeeCategories.Count,
                    "because returned event's categories should be as requested");

            var indCat = 0;
            meetingEvent.AttendeeCategories.ForEach(recordCat =>
            {
                recordCat.Options.Count.Should()
                    .Be(BobEvent.AttendeeCategories[indCat].Options.Count,
                        "because returned event categories' options should be as requested");

                indCat++;
            });
        }
    }
}