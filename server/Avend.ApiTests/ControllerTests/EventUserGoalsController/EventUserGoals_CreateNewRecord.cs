using System;
using System.Net;
using System.Threading.Tasks;
using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Infrastructure.Responses;
using Avend.API.Services.Events.NetworkDTO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventUserGoalsController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("EventUserGoalsController")]
    [TestCategory("EventUserGoalsController.CreateNewRecord()")]
    // ReSharper disable once InconsistentNaming
    public class EventUserGoals_CreateNewRecord : BaseUserGoalsEndpointTest
    {
        [TestMethod]
        public async Task ShouldCreateUserGoalsRecordWhenTenantAdminSetsGoalsForHimself()
        {
            var userGoalsDto = await BobGoalsDataFixtures.AddFromSample();

            userGoalsDto.Uid.Should()
                .NotBeNull("because we have just added a valid user goals record")
                .And
                .NotBeEmpty("because we have just added a valid user goals record");

            var newUserGoalUid = await BobTA.PostJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/goals", userGoalsDto).AvendResponse<Guid>();

            newUserGoalUid.Should()
                .NotBeEmpty("because we have just added a single user goals record");

            var newUserGoalDto = await BobTA.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/goals").AvendResponse<EventUserGoalsDto>();

            newUserGoalDto.Should()
                .NotBeNull("because we have just added a single user goal")
                .And
                .Match<EventUserGoalsDto>(obj => obj.EventUid == userGoalsDto.EventUid, "because we have explicitly set event uid for the new event user goals record")
                .And
                .Match<EventUserGoalsDto>(obj => obj.LeadsGoal == userGoalsDto.LeadsGoal, "because we have explicitly set leads count goal for the new event user goals record")
                ;
        }

        [TestMethod]
        public async Task ShouldCreateUserGoalsRecordWhenTenantAdminSetsGoalsForHisSeatUser()
        {
            var userGoalsDto = await BobGoalsDataFixtures.AddFromSample(dto =>
            {
                dto.UserUid = TestUser.CecileTester.Uid;
            });

            userGoalsDto.Uid.Should()
                .NotBeNull("because we have just added a valid user goals record")
                .And
                .NotBeEmpty("because we have just added a valid user goals record");

            var newUserGoalDto = await BobTA.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/goals/" + TestUser.CecileTester.Uid).AvendResponse<EventUserGoalsDto>();

            newUserGoalDto.Should() 
                .NotBeNull("because we have just added a single user goal")
                .And
                .Match<EventUserGoalsDto>(obj => obj.EventUid == userGoalsDto.EventUid, "because we have explicitly set event uid for the new event user goals record")
                .And
                .Match<EventUserGoalsDto>(obj => obj.LeadsGoal == userGoalsDto.LeadsGoal, "because we have explicitly set leads count goal for the new event user goals record")
                ;
        }

        [TestMethod]
        public async Task ShouldCreateUserGoalsRecordWhenSeatUserSetsGoalsForHimself()
        {
            var userGoalsDto = await CecileGoalsDataFixtures.AddFromSample();

            userGoalsDto.Uid.Should()
                .NotBeNull("because we have just added a valid user goals record")
                .And
                .NotBeEmpty("because we have just added a valid user goals record");

            var newUserGoalDto = await CecileTesterClient.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/goals").AvendResponse<EventUserGoalsDto>();

            newUserGoalDto.Should()
                .NotBeNull("because we have just added a single user goal")
                .And
                .Match<EventUserGoalsDto>(obj => obj.EventUid == userGoalsDto.EventUid, "because we have explicitly set event uid for the new event user goals record")
                .And
                .Match<EventUserGoalsDto>(obj => obj.LeadsGoal == userGoalsDto.LeadsGoal, "because we have explicitly set leads count goal for the new event user goals record")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnForbiddenWhenSeatUserSetsGoalsForOtherUser()
        {
            var userGoalsDto = BobGoalsDataFixtures.MakeSample(BobGoalsDataFixtures.Event.Uid.Value, dto =>
            {
                dto.UserUid = TestUser.MarcTester.Uid;
            });

            var errors = await AlexTesterClient.PostJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/goals", userGoalsDto).AvendErrorResponse(HttpStatusCode.Forbidden, "because seat user should not be able to add goals for another user");

            errors.Should()
                .HaveCount(1, "because only a single error should be returned");

            errors[0].Should()
                .NotBeNull("because we expect a valid error object at the first position in the errors list")
                .And
                .Match<Error>(obj => obj.Code == ErrorCodes.Forbidden, "because returned code should indicate forbidden access")
                .And
                .Match<Error>(obj => obj.Message.Contains("set user goals for yourself"), "because returned message  should request to set user goals for himself")
                ;
        }

        [TestMethod]
        public async Task ShouldChangeUserGoalsRecordWhenTenantAdminSendsUserGoalsForHimselfTwoTimes()
        {
            var userGoalsDto = await BobGoalsDataFixtures.AddFromSample();

            userGoalsDto.Uid.Should()
                .NotBeNull("because we have just added a valid user goals record")
                .And
                .NotBeEmpty("because we have just added a valid user goals record");

            userGoalsDto.LeadsGoal = 20;

            var secondUserGoalUid = await BobTA.PostJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/goals", userGoalsDto).AvendResponse<Guid>();

            secondUserGoalUid.Should()
                .Be(userGoalsDto.Uid.Value, "because we do not replace record but just update it instead.");

            var newUserGoalDto = await BobTA.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/goals").AvendResponse<EventUserGoalsDto>();

            newUserGoalDto.Should()
                .NotBeNull("because we have initially added a single user goal")
                .And
                .Match<EventUserGoalsDto>(obj => obj.EventUid == userGoalsDto.EventUid, "because we have explicitly set event uid for the new event user goals record")
                .And
                .Match<EventUserGoalsDto>(obj => obj.LeadsGoal == userGoalsDto.LeadsGoal, "because we have explicitly set leads count goal for the new event user goals record")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnForbiddenWhenSuperAdminSetsGoalsForHimself()
        {
            var userGoalsDto = new EventUserGoalsDto()
            {
                EventUid = ConferenceEventData.Event.Uid,
                LeadsGoal = 10,
            };

            var errors = await AlexTesterClient.PostJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/goals", userGoalsDto).AvendErrorResponse(HttpStatusCode.Forbidden, "because super admin have no membership");

            errors.Should()
                .HaveCount(1, "because only a single error should be returned");

            errors[0].Should()
                .NotBeNull("because we expect a valid error object at the first position in the errors list")
                .And
                .Match<Error>(obj => obj.Code == ErrorCodes.Forbidden, "because returned code should indicate forbidden access")
                .And
                .Match<Error>(obj => obj.Message.Contains("set user goals for yourself"), "because returned message  should request to set user goals for himself")
                ;
        }
    }
}