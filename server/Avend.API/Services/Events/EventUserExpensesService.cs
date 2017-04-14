using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Events.NetworkDTO;
using Avend.API.Services.Subscriptions;

using Microsoft.EntityFrameworkCore;

using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class EventUserExpensesService
    {
        public readonly DbContextOptions<AvendDbContext> DbOptions;
        public readonly UserContext UserContext;

        public EventUserExpensesService(DbContextOptions<AvendDbContext> dbOptions, UserContext userContext)
        {
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();
            Assert.Argument(userContext, nameof(userContext)).NotNull();

            this.DbOptions = dbOptions;
            this.UserContext = userContext;
        }

        /// <summary>
        /// Returns <see cref="EventUserExpenseDto"/> object for the user who is making the request.
        /// </summary>
        /// 
        /// <param name="eventUidStr">UID of the event as a string. Need to be verified.</param>
        /// 
        /// <returns><see cref="EventUserExpenseDto"/> record with Expenses for the user who is making the request.</returns>
        public List<EventUserExpenseDto> FindUserExpensesForCurrentUserByEventUid(string eventUidStr)
        {
            Check.Value(UserContext, "user_uid", AvendErrors.InvalidUser).NotNull();
            Check.Value(UserContext.UserUid, "user_uid", AvendErrors.InvalidUser).NotNull();
            Check.Value(eventUidStr, "event_uid", AvendErrors.NotFound).NotNull();

            Guid eventUid;
            var eventUidIsValid = Guid.TryParse(eventUidStr, out eventUid);
            Check.Value(eventUidIsValid, "event_uid").IsTrue("Event uid in request route is not a valid GUID value");

            using (var db = new AvendDbContext(DbOptions))
            {
                var expensesRepo = new EventUserExpensesRepository(db);

                var existingUserExpenses = expensesRepo.GetUserExpensesForUserAndEvent(UserContext.UserUid, eventUid);

                Check.Value(existingUserExpenses, "event_user_expense_uid", AvendErrors.NotFound).NotNull();

                return existingUserExpenses.Select(record => EventUserExpenseDto.From(record, eventUid)).ToList();
            }
        }

        /// <summary>
        /// Returns <see cref="EventUserExpenseDto"/> object for the given user.
        /// </summary>
        /// 
        /// <param name="eventUidStr">UID of the event as a string. Need to be verified.</param>
        /// <param name="userUidStr">UID of the event as a string. Need to be verified and to check access rights.</param>
        /// 
        /// <returns><see cref="EventUserExpenseDto"/> record with Expenses for the given user.</returns>
        public List<EventUserExpenseDto> FindUserExpensesByEventUidAndUserUid(string eventUidStr, string userUidStr)
        {
            Check.Value(UserContext, "user_uid", AvendErrors.InvalidUser).NotNull();
            Check.Value(UserContext.UserUid, "user_uid", AvendErrors.InvalidUser).NotNull();
            Check.Value(eventUidStr, "event_uid", AvendErrors.NotFound).NotNull();

            Guid userUid;
            var userUidIsValid = Guid.TryParse(userUidStr, out userUid);
            Check.Value(userUidIsValid, "user_uid").IsTrue("User uid in request route is not a valid GUID value");

            Guid eventUid;
            var eventUidIsValid = Guid.TryParse(eventUidStr, out eventUid);
            Check.Value(eventUidIsValid, "event_uid").IsTrue("Event uid in request route is not a valid GUID value");

            using (var db = new AvendDbContext(DbOptions))
            {
                ValidateUserUidAccess(db, null, userUid);

                var expensesRepo = new EventUserExpensesRepository(db);

                var existingUserExpenses = expensesRepo.GetUserExpensesForUserAndEvent(userUid, eventUid);

                Check.Value(existingUserExpenses, "event_user_expense_uid", AvendErrors.NotFound).NotNull();

                return existingUserExpenses.Select(record => EventUserExpenseDto.From(record, eventUid)).ToList();
            }
        }

        public async Task<EventUserExpenseDto> CreateUserExpense(string eventUidStr, EventUserExpenseDto dto)
        {
            Check.Value(UserContext, "user_uid", AvendErrors.InvalidUser).NotNull();
            Check.Value(UserContext.UserUid, "user_uid", AvendErrors.InvalidUser).NotNull();
            Check.Value(UserContext.Role, "user_uid", AvendErrors.Forbidden).NotEqualsTo(UserRole.SuperAdmin, "Superadmin is not allowed to set user expenses for himself");

            Check.Value(eventUidStr, "event_uid", AvendErrors.NotFound).NotNull();
            Check.Value(dto, "event_user_expenses").NotNull();

            Guid eventUid;
            var eventUidIsValid = Guid.TryParse(eventUidStr, out eventUid);
            Check.Value(eventUidIsValid, "event_uid").IsTrue("Event uid in request route is not a valid GUID value");

            if (!dto.UserUid.HasValue)
                dto.UserUid = UserContext.UserUid;

            using (var db = new AvendDbContext(DbOptions))
            {
                var userExpensesWriter = new EventUserExpensesWriter(db);

                ValidateUserUidAccess(db, userExpensesWriter, dto.UserUid.Value);

                await DoWriteNewExpenseFromDto(userExpensesWriter, dto);

                return EventUserExpenseDto.From(userExpensesWriter.PreparedRecord, eventUid);
            }
        }

        public async Task<EventUserExpenseDto> UpdateUserExpense(string eventUidStr, EventUserExpenseDto dto)
        {
            Check.Value(UserContext, "user_uid", AvendErrors.InvalidUser).NotNull();
            Check.Value(UserContext.UserUid, "user_uid", AvendErrors.InvalidUser).NotNull();

            if (dto.UserUid == null)
            {
                Check.Value(UserContext.Role, "user_uid", AvendErrors.Forbidden).NotEqualsTo(UserRole.SuperAdmin, "Superadmin is not allowed to set user expenses for himself");
                Check.Value(UserContext.Subscription, "user_uid", AvendErrors.InvalidUser).NotNull();
            }

            Check.Value(eventUidStr, "event_uid", AvendErrors.NotFound).NotNull();

            Guid eventUid;
            var eventUidIsValid = Guid.TryParse(eventUidStr, out eventUid);
            Check.Value(eventUidIsValid, "event_uid").IsTrue("Event uid in request route is not a valid GUID value");

            Check.Value(dto, "event_user_expenses").NotNull();
            Check.Value(dto.Uid, "event_user_expense.event_user_expense_uid").NotNull();

            if (!dto.UserUid.HasValue)
                dto.UserUid = UserContext.UserUid;

            using (var db = new AvendDbContext(DbOptions))
            {
                var userExpensesWriter = new EventUserExpensesWriter(db);

                ValidateUserUidAccess(db, userExpensesWriter, dto.UserUid.Value);

                var expensesRepo = new EventUserExpensesRepository(db);

                var existingUserExpense = expensesRepo.GetUserExpensesByUid(dto.UserUid.Value, dto.Uid);

                Check.Value(existingUserExpense, "event_user_expense.uid", AvendErrors.NotFound).NotNull("Cannot find event_user_expense record with this UID");

                if (dto.EventUid.HasValue)
                    Check.Value(dto.EventUid, "event_user_expense.event_uid", AvendErrors.InvalidParameter).EqualsTo(existingUserExpense.EventRecord.Uid, "Cannot change event UID for event_user_expense record");

                await DoWriteUpdatedExpenseFromDto(userExpensesWriter, dto);

                return EventUserExpenseDto.From(userExpensesWriter.PreparedRecord, eventUid);
            }
        }

        private void ValidateUserUidAccess(AvendDbContext db, EventUserExpensesWriter userExpensesWriter, Guid userUid)
        {
            if (userUid == UserContext.UserUid)
            {
                if (userExpensesWriter != null)
                {
                    userExpensesWriter.SetAndValidateUserAndSubscriptionFromUserContext(UserContext);
                    userExpensesWriter.Validator.Throw();
                }

                return;
            }

            var subscriptionsRepo = new SubscriptionRepository(db);
            var userSubscription = subscriptionsRepo.FindSubscriptionForUser(userUid);

            if (UserContext.Role == UserRole.SuperAdmin)
            {
                if (userExpensesWriter != null)
                {
                    userExpensesWriter.SetAndValidateExplicitUserAndSubscription(userUid, userSubscription.Uid);
                    userExpensesWriter.Validator.Throw();
                }

                return;
            }

            Check.Value(UserContext.Role, "event_user_expense.user_uid", AvendErrors.Forbidden).EqualsTo(UserRole.Admin, "event_user_expense.user_uid mismatch - please only set expenses for yourself");

            Check.Value(userSubscription, "event_user_expense.user_uid", AvendErrors.NotFound).NotNull("User subscription is not found");
            Check.Value(userSubscription.Uid, "event_user_expense.user_uid", AvendErrors.NotFound)
                .EqualsTo(UserContext.Subscription.Uid.Value, "User not found in your tenant");

            if (userExpensesWriter != null)
            {
                userExpensesWriter.SetAndValidateExplicitUserAndSubscription(userUid, userSubscription.Uid);
                userExpensesWriter.Validator.Throw();
            }
        }

        private async Task DoWriteNewExpenseFromDto(EventUserExpensesWriter userExpensesWriter, EventUserExpenseDto dto)
        {
            userExpensesWriter.SetRequestBody(dto);
            userExpensesWriter.Validator.Throw();

            await userExpensesWriter.SetEventIdFromRequestBody();
            userExpensesWriter.Validator.Throw();

            userExpensesWriter.MakeNewRecord();
            userExpensesWriter.Validator.Throw();

            await userExpensesWriter.SaveChanges();
            userExpensesWriter.Validator.Throw();
        }

        private async Task DoWriteUpdatedExpenseFromDto(EventUserExpensesWriter userExpensesWriter, EventUserExpenseDto dto)
        {
            userExpensesWriter.SetRequestBody(dto);
            userExpensesWriter.ValidateRequestBody();
            userExpensesWriter.Validator.Throw();

            await userExpensesWriter.SetEventIdFromRequestBody();
            userExpensesWriter.Validator.Throw();

            userExpensesWriter.LoadRecordAndApplyChanges();
            userExpensesWriter.Validator.Throw();

            await userExpensesWriter.SaveChanges();
            userExpensesWriter.Validator.Throw();
        }
    }
}