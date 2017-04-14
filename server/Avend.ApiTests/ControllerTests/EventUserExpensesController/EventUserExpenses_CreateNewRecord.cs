using System.Collections.Generic;
using System.Linq;
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
    [TestCategory("EventUserExpensesController.CreateNewRecord()")]
    // ReSharper disable once InconsistentNaming
    public class EventUserExpenses_CreateNewRecord : BaseUserExpensesEndpointTest
    {
        [TestMethod]
        public async Task ShouldCreateUserExpenseWhenTenantAdminAddsExpenseForHimself()
        {
            var userExpenseDto = await BobExpensesDataFixtures.AddFromSample();

            userExpenseDto.Uid.Should()
                .NotBeNull("because we have just added a valid user expense record")
                .And
                .NotBeEmpty("because we have just added a valid user expense record");

            var allUserExpenseDtos = await BobTA.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/expenses").AvendResponse<List<EventUserExpenseDto>>();

            allUserExpenseDtos.Should()
                .HaveCount(1, "because we have just added a single expense for Bob for this event")
                ;

            allUserExpenseDtos[0].Should()
                .NotBeNull("because we have just added a single user expense")
                .And
                .Match<EventUserExpenseDto>(obj => obj.EventUid == userExpenseDto.EventUid, "because we have explicitly set event uid for the new event user expense record")
                .And
                .Match<EventUserExpenseDto>(obj => obj.Expense.Amount == userExpenseDto.Expense.Amount && obj.Expense.Currency == userExpenseDto.Expense.Currency, "because we expect proper expense amount for the added event user expense record")
                ;
        }

        [TestMethod]
        public async Task ShouldCreateUserExpenseWhenTenantAdminAddsExpenseForHisSeatUser()
        {
            var userExpenseDto = await BobExpensesDataFixtures.AddFromSample(dto =>
            {
                dto.UserUid = TestUser.CecileTester.Uid;
            });

            userExpenseDto.Uid.Should()
                .NotBeNull("because we have just added a valid user expense record")
                .And
                .NotBeEmpty("because we have just added a valid user expense record");

            var allUserExpenseDtos = await CecileSU.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/expenses").AvendResponse<List<EventUserExpenseDto>>();

            allUserExpenseDtos.Should()
                .HaveCount(1, "because we have just added a single expense for Cecile for this event")
                ;

            allUserExpenseDtos[0].Should()
                .NotBeNull("because we have just added a single user expense")
                .And
                .Match<EventUserExpenseDto>(obj => obj.EventUid == userExpenseDto.EventUid, "because we have explicitly set event uid for the new event user expense record")
                .And
                .Match<EventUserExpenseDto>(obj => obj.Expense.Amount == userExpenseDto.Expense.Amount && obj.Expense.Currency == userExpenseDto.Expense.Currency, "because we have explicitly set expense value for the new event user expense record")
                ;
        }

        [TestMethod]
        public async Task ShouldCreateUserExpenseWhenSeatUserSetsExpensesForHimself()
        {
            var userExpenseDto = await CecileExpensesDataFixtures.AddFromSample();

            userExpenseDto.Uid.Should()
                .NotBeNull("because we have just added a valid user expense record")
                .And
                .NotBeEmpty("because we have just added a valid user expense record");

            var allUserExpenseDtos = await CecileSU.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/expenses").AvendResponse<List<EventUserExpenseDto>>();

            allUserExpenseDtos.Should()
                .HaveCount(1, "because we have just added a single expense for Cecile for this event")
                ;

            allUserExpenseDtos[0].Should()
                .NotBeNull("because we have just added this user expense")
                .And
                .Match<EventUserExpenseDto>(obj => obj.EventUid == userExpenseDto.EventUid, "because we have explicitly set event uid for the new event user expense record")
                .And
                .Match<EventUserExpenseDto>(obj => obj.Expense.Amount == userExpenseDto.Expense.Amount && obj.Expense.Currency == userExpenseDto.Expense.Currency, "because we have explicitly set expense value for the new event user expense record")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnForbiddenWhenSeatUserAddsExpenseForOtherUser()
        {
            var userExpenseDto = CecileExpensesDataFixtures.MakeSample(BobExpensesDataFixtures.Event.Uid.Value, dto =>
            {
                dto.UserUid = TestUser.MarcTester.Uid;
            });

            var errors = await CecileSU.PostJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/expenses", userExpenseDto).AvendErrorResponse(HttpStatusCode.Forbidden, "because seat user should not be able to add expense for another user");

            errors.Should()
                .HaveCount(1, "because only a single error should be returned");

            errors[0].Should()
                .NotBeNull("because we expect a valid error object at the first position in the errors list")
                .And
                .Match<Error>(obj => obj.Code == ErrorCodes.Forbidden, "because returned code should indicate forbidden access")
                .And
                .Match<Error>(obj => obj.Message == "event_user_expense.user_uid mismatch - please only set expenses for yourself", "because returned message should request to set user expense for himself")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnListWithTwoUserExpensesWhenTenantAdminAddsUserExpenseForHimselfTwoTimes()
        {
            var initialUserExpenseDto = await BobExpensesDataFixtures.AddFromSample();

            initialUserExpenseDto.Uid.Should()
                .NotBeNull("because we have just added a valid user expense record")
                .And
                .NotBeEmpty("because we have just added a valid user expense record");

            var secondUserExpenseDto = await BobExpensesDataFixtures.AddFromSample();

            secondUserExpenseDto.Should()
                .Match<EventUserExpenseDto>(record => record.Uid != initialUserExpenseDto.Uid.Value, "because we expect a new expense record to be created");

            var allUserExpenseDtos = await BobTA.GetJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/expenses").AvendResponse<List<EventUserExpenseDto>>();

            allUserExpenseDtos.Should()
                .HaveCount(2, "because we have just added two expenses for Bob for this event")
                ;

            allUserExpenseDtos.First(record => record.Uid == initialUserExpenseDto.Uid).Should()
                .NotBeNull("because we have initially added this user expense")
                .And
                .Match<EventUserExpenseDto>(obj => obj.EventUid == initialUserExpenseDto.EventUid, "because we have explicitly set event uid for the new event user expense record")
                .And
                .Match<EventUserExpenseDto>(obj => obj.Expense.Amount == initialUserExpenseDto.Expense.Amount && obj.Expense.Currency == initialUserExpenseDto.Expense.Currency, "because we have explicitly set expense value for the new event user expense record")
                ;

            allUserExpenseDtos.First(record => record.Uid == secondUserExpenseDto.Uid).Should()
                .NotBeNull("because we have also added this additional user expense")
                .And
                .Match<EventUserExpenseDto>(obj => obj.EventUid == secondUserExpenseDto.EventUid, "because we have explicitly set event uid for the new event user expense record")
                .And
                .Match<EventUserExpenseDto>(obj => obj.Expense.Amount == secondUserExpenseDto.Expense.Amount && obj.Expense.Currency == secondUserExpenseDto.Expense.Currency, "because we have explicitly set expense value for the new event user expense record")
                ;
        }

        [TestMethod]
        public async Task ShouldReturnForbiddenWhenSuperAdminSetsExpensesForHimself()
        {
            var userExpenseDto = new EventUserExpenseDto()
            {
                EventUid = ConferenceEventData.Event.Uid,
                Expense = new MoneyDto()
                {
                    Amount = 12.50M,
                    Currency = CurrencyCode.USD,
                },
            };

            var errors = await AlexSA.PostJsonAsync(UrlApiV1Events + "/" + ConferenceEventData.Event.Uid + "/expenses", userExpenseDto).AvendErrorResponse(HttpStatusCode.Forbidden, "because super admin have no membership");

            errors.Should()
                .HaveCount(1, "because only a single error should be returned");

            errors[0].Should()
                .NotBeNull("because we expect a valid error object at the first position in the errors list")
                .And
                .Match<Error>(obj => obj.Code == ErrorCodes.Forbidden, "because returned code should indicate forbidden access")
                .And
                .Match<Error>(obj => obj.Message == "Superadmin is not allowed to set user expenses for himself", "because returned message should request to set user expense for himself")
                ;
        }
    }
}