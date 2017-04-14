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
    [TestCategory("EventUserGoalsController.UpdateRecord()")]
    // ReSharper disable once InconsistentNaming
    public class EventUserGoals_UpdateRecord : BaseUserGoalsEndpointTest
    {
        [TestMethod]
        public async Task ShouldUpdateUserGoalsRecordWhenTenantAdminSendsRequestForHimself()
        {
            var userGoalsDto = await BobGoalsDataFixtures.AddFromSample();

            userGoalsDto.Uid.Should()
                .NotBeNull("because we have just added a valid user goals record")
                .And
                .NotBeEmpty("because we have just added a valid user goals record");

            var updatedUserGoalsDto = new EventUserGoalsDto()
            {
                Uid = userGoalsDto.Uid,
                EventUid = ConferenceEventData.Event.Uid,
                LeadsGoal = 20,
            };

            var updatedUserGoalUid = await BobTA.PutJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/goals", updatedUserGoalsDto).AvendResponse<Guid>();

            updatedUserGoalUid.Should()
                .Be(userGoalsDto.Uid.Value, "because we do not replace record but just update it instead.");

            var actualUserGoalDto = await BobTA.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/goals").AvendResponse<EventUserGoalsDto>();

            actualUserGoalDto.Should()
                .NotBeNull("because we have added a single user goal initially")
                .And
                .Match<EventUserGoalsDto>(obj => obj.EventUid == userGoalsDto.EventUid, "because we have explicitly set event uid for the new event user goals record")
                .And
                .Match<EventUserGoalsDto>(obj => obj.LeadsGoal == updatedUserGoalsDto.LeadsGoal, "because we have explicitly set leads count goal for the new event user goals record")
                ;
        }

        [TestMethod]
        public async Task ShouldUpdateUserGoalsRecordWhenTenantAdminSendsRequestForHisSeatUser()
        {
            var userGoalsDto = await CecileGoalsDataFixtures.AddFromSample();

            userGoalsDto.Uid.Should()
                .NotBeNull("because we have just added a valid user goals record")
                .And
                .NotBeEmpty("because we have just added a valid user goals record");

            var updatedUserGoalsDto = new EventUserGoalsDto()
            {
                Uid = userGoalsDto.Uid,
                UserUid = TestUser.CecileTester.Uid,
                EventUid = ConferenceEventData.Event.Uid,
                LeadsGoal = 20,
            };

            var updatedUserGoalUid = await BobTA.PutJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/goals", updatedUserGoalsDto).AvendResponse<Guid>();

            updatedUserGoalUid.Should()
                .Be(userGoalsDto.Uid.Value, "because we do not replace record but just update it instead.");

            var actualUserGoalDto = await BobTA.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/goals/" + TestUser.CecileTester.Uid).AvendResponse<EventUserGoalsDto>();

            actualUserGoalDto.Should()
                .NotBeNull("because we have added a single user goal initially")
                .And
                .Match<EventUserGoalsDto>(obj => obj.EventUid == userGoalsDto.EventUid, "because we have explicitly set event uid for the new event user goals record")
                .And
                .Match<EventUserGoalsDto>(obj => obj.LeadsGoal == updatedUserGoalsDto.LeadsGoal, "because we have explicitly set leads count goal for the new event user goals record")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnBadRequestWhenTryingToChangeEventUid()
        {
            var userGoalsDto = await BobGoalsDataFixtures.AddFromSample();

            userGoalsDto.Uid.Should()
                .NotBeNull("because we have just added a valid user goals record")
                .And
                .NotBeEmpty("because we have just added a valid user goals record");

            userGoalsDto.EventUid = (await BobEventData.AddFromSample()).Uid;

            var errors = await BobTA.PutJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/goals", userGoalsDto)
                .AvendErrorResponse(HttpStatusCode.BadRequest, "because super admin have no membership");

            errors.Should()
                .HaveCount(1, "because only a single error should be returned");

            errors[0].Should()
                .NotBeNull("because we expect a valid error object at the first position in the errors list")
                .And
                .Match<Error>(obj => obj.Code == ErrorCodes.CodeInvalidParameter, "because returned code should indicate forbidden access")
                .And
                .Match<Error>(obj => obj.Message.Contains("Cannot change event"), "because returned message should request to set user goals for himself")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnForbiddenWhenSuperAdminSendsRequestForHimself()
        {
            var userGoalsDto = new EventUserGoalsDto()
            {
                Uid = Guid.Empty,
                EventUid = ConferenceEventData.Event.Uid,
                LeadsGoal = 10,
            };

            var errors = await AlexTesterClient.PutJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/goals", userGoalsDto).AvendErrorResponse(HttpStatusCode.Forbidden, "because super admin have no membership");

            errors.Should()
                .HaveCount(1, "because only a single error should be returned");

            errors[0].Should()
                .NotBeNull("because we expect a valid error object at the first position in the errors list")
                .And
                .Match<Error>(obj => obj.Code == ErrorCodes.Forbidden, "because returned code should indicate forbidden access")
                .And
                .Match<Error>(obj => obj.Message.Contains("set user goals for yourself"), "because returned message should request to set user goals for himself")
                ;
        }
    }
}