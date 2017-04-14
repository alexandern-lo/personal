using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Avend.ApiTests.DataSamples;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Infrastructure.Responses;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Events.NetworkDTO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avend.ApiTests.ControllerTests.EventUserExpensesController
{
    [TestClass]
    [TestCategory("Integrational")]
    [TestCategory("EventUserExpensesController")]
    [TestCategory("EventUserExpensesController.UpdateRecord()")]
    // ReSharper disable once InconsistentNaming
    public class EventUserExpenses_UpdateRecord : BaseUserExpensesEndpointTest
    {
        [TestMethod]
        public async Task ShouldUpdateUserExpenseWhenTenantAdminUpdatesValidRecordForHimself()
        {
            var userExpenseDto = await BobExpensesDataFixtures.AddFromSample();

            userExpenseDto.Uid.Should()
                .NotBeNull("because we have just added a valid user expense record")
                .And
                .NotBeEmpty("because we have just added a valid user expense record");

            var userExpenseChangesDto = new EventUserExpenseDto()
            {
                Uid = userExpenseDto.Uid,
                EventUid = ConferenceEventData.Event.Uid,

                Expense = new MoneyDto()
                {
                    Amount = 12.50M,
                    Currency = CurrencyCode.USD,
                },
            };

            var updatedUserExpenseDto = await BobTA.PutJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/expenses/" + userExpenseDto.Uid, userExpenseChangesDto)
                .AvendResponse<EventUserExpenseDto>();

            updatedUserExpenseDto.Should()
                .Match<EventUserExpenseDto>(record => record.Uid.Value == userExpenseDto.Uid.Value, "because we do not replace record but just update it instead.");

            updatedUserExpenseDto.Should()
                .NotBeNull("because we expect update request to return a valid reply")
                .And
                .Match<EventUserExpenseDto>(obj => obj.EventUid == userExpenseDto.EventUid,
                    "because we expect event to be the same as initially set for the new event user expense record")
                .And
                .Match<EventUserExpenseDto>(
                    obj =>
                        obj.Expense.Amount == userExpenseChangesDto.Expense.Amount &&
                        obj.Expense.Currency == userExpenseChangesDto.Expense.Currency,
                    "because we have explicitly set expense value for the changed event user expense record")
                ;

            var allUserExpenseDtos = await BobTA.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/expenses")
                .AvendResponse<List<EventUserExpenseDto>>();

            allUserExpenseDtos.Should()
                .HaveCount(1, "because we have just added a single expense for Bob for this event")
                ;

            allUserExpenseDtos[0].Should()
                .NotBeNull("because we have added a single user expense initially")
                .And
                .Match<EventUserExpenseDto>(obj => obj.EventUid == userExpenseDto.EventUid,
                    "because we expect event to be the same as initially set for the new event user expense record")
                .And
                .Match<EventUserExpenseDto>(
                    obj =>
                        obj.Expense.Amount == userExpenseChangesDto.Expense.Amount &&
                        obj.Expense.Currency == userExpenseChangesDto.Expense.Currency,
                    "because we have explicitly set expense value for the changed event user expense record")                
                ;
        }

        [TestMethod]
        public async Task ShouldUpdateUserExpenseWhenTenantAdminUpdatesValidRecordForHisSeatUser()
        {
            var userExpenseDto = await CecileExpensesDataFixtures.AddFromSample();

            userExpenseDto.Uid.Should()
                .NotBeNull("because we have just added a valid user expense record")
                .And
                .NotBeEmpty("because we have just added a valid user expense record");

            var userExpenseChangesDto = new EventUserExpenseDto()
            {
                Uid = userExpenseDto.Uid,
                UserUid = TestUser.CecileTester.Uid,
                EventUid = userExpenseDto.EventUid,

                Expense = new MoneyDto()
                {
                    Amount = 12.50M,
                    Currency = CurrencyCode.USD,
                },
            };

            var updatedUserExpenseDto = await BobTA.PutJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/expenses/" + userExpenseDto.Uid, userExpenseChangesDto)
                .AvendResponse<EventUserExpenseDto>();

            updatedUserExpenseDto.Should()
                .Match<EventUserExpenseDto>(dto => dto.Uid == userExpenseDto.Uid.Value, "because we do not replace record but just update it instead.");

            updatedUserExpenseDto.Should()
                .NotBeNull("because we expect update request to return a valid reply")
                .And
                .Match<EventUserExpenseDto>(obj => obj.EventUid == userExpenseDto.EventUid,
                    "because we expect event to be the same as initially set for the new event user expense record")
                .And
                .Match<EventUserExpenseDto>(
                    obj =>
                        obj.Expense.Amount == userExpenseChangesDto.Expense.Amount &&
                        obj.Expense.Currency == userExpenseChangesDto.Expense.Currency,
                    "because we have explicitly set expense value for the changed event user expense record")
                ;

            var allUserExpenseDtos = await CecileSU.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/expenses")
                .AvendResponse<List<EventUserExpenseDto>>();

            allUserExpenseDtos.Should()
                .HaveCount(1, "because we have just added a single expense for Cecile for this event")
                ;

            allUserExpenseDtos[0].Should()
                .NotBeNull("because we have added a single user expense initially")
                .And
                .Match<EventUserExpenseDto>(obj => obj.EventUid == userExpenseChangesDto.EventUid,
                    "because we have explicitly set event uid for the new event user expense record")
                .And
                .Match<EventUserExpenseDto>(
                    obj =>
                        obj.Expense.Amount == userExpenseChangesDto.Expense.Amount &&
                        obj.Expense.Currency == userExpenseChangesDto.Expense.Currency,
                    "because we have explicitly set expense value for the new event user expense record")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnBadRequestWhenTenantAdminChangesEventUidForHisRecord()
        {
            var userExpenseDto = await BobExpensesDataFixtures.AddFromSample();

            userExpenseDto.Uid.Should()
                .NotBeNull("because we have just added a valid user expense record")
                .And
                .NotBeEmpty("because we have just added a valid user expense record");

            var updatedUserExpenseDto = new EventUserExpenseDto()
            {
                Uid = userExpenseDto.Uid,
                EventUid = (await BobEventData.AddFromSample()).Uid.Value,
                Expense = userExpenseDto.Expense,
            };

            var errors = await BobTA.PutJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/expenses/" + userExpenseDto.Uid, updatedUserExpenseDto)
                .AvendErrorResponse(HttpStatusCode.BadRequest, "because tenant admin is not allowed to change the event for expense record for himself");

            errors.Should()
                .HaveCount(1, "because only a single error should be returned");

            errors[0].Should()
                .NotBeNull("because we expect a valid error object at the first position in the errors list")
                .And
                .Match<Error>(obj => obj.Code == ErrorCodes.CodeInvalidParameter, "because returned code should indicate forbidden access")
                .And
                .Match<Error>(obj => obj.Message.Contains("Cannot change event"), "because returned message should request to set user expense for himself")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnForbiddenWhenSuperAdminUpdatesExpenseForHimself()
        {
            var userExpenseDto = new EventUserExpenseDto()
            {
                Uid = Guid.Empty,
                EventUid = ConferenceEventData.Event.Uid,
                Expense = new MoneyDto()
                {
                    Amount = 12.50M,
                    Currency = CurrencyCode.USD,
                },
            };

            var errors = await AlexSA.PutJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/expenses/" + userExpenseDto.Uid, userExpenseDto)
                .AvendErrorResponse(HttpStatusCode.Forbidden, "because super admin have no membership and is not allowed to edit his expenses");

            errors.Should()
                .HaveCount(1, "because only a single error should be returned");

            errors[0].Should()
                .NotBeNull("because we expect a valid error object at the first position in the errors list")
                .And
                .Match<Error>(obj => obj.Code == ErrorCodes.Forbidden, "because returned code should indicate forbidden access")
                .And
                .Match<Error>(obj => obj.Message == "Superadmin is not allowed to set user expenses for himself", "because returned message should indicate that superadmin is not allowed to perform this action")
                ;
        }
    }
}