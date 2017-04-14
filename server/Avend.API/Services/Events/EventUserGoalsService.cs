using System;
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
    public class EventUserGoalsService
    {
        public readonly DbContextOptions<AvendDbContext> DbOptions;
        public readonly UserContext UserContext;

        public EventUserGoalsService(DbContextOptions<AvendDbContext> dbOptions, UserContext userContext)
        {
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();
            Assert.Argument(userContext, nameof(userContext)).NotNull();

            this.DbOptions = dbOptions;
            this.UserContext = userContext;
        }

        /// <summary>
        /// Returns <see cref="EventUserGoalsDto"/> object for the user who is making the request.
        /// </summary>
        /// 
        /// <param name="eventUidStr">UID of the event as a string. Need to be verified.</param>
        /// 
        /// <returns><see cref="EventUserGoalsDto"/> record with goals for the user who is making the request.</returns>
        public EventUserGoalsDto FindUserGoalsByEventUid(string eventUidStr)
        {
            Check.Value(UserContext, "user_uid", AvendErrors.InvalidUser).NotNull();
            Check.Value(UserContext.UserUid, "user_uid", AvendErrors.InvalidUser).NotNull();
            Check.Value(eventUidStr, "event_uid", AvendErrors.NotFound).NotNull();

            Guid eventUid;
            var eventUidIsValid = Guid.TryParse(eventUidStr, out eventUid);
            Check.Value(eventUidIsValid, "event_uid").IsTrue("Event uid in request route is not a valid GUID value");

            using (var db = new AvendDbContext(DbOptions))
            {
                var goalsRepo = new EventUserGoalsRepository(db);

                var existingUserGoals = goalsRepo.GetUserGoalsForUserAndEvent(UserContext.UserUid, eventUid);

                Check.Value(existingUserGoals, "event_user_goals_uid", AvendErrors.NotFound).NotNull();

                return EventUserGoalsDto.From(existingUserGoals, eventUid);
            }
        }

        /// <summary>
        /// Returns <see cref="EventUserGoalsDto"/> object for the given user.
        /// </summary>
        /// 
        /// <param name="eventUidStr">UID of the event as a string. Need to be verified.</param>
        /// <param name="userUidStr">UID of the event as a string. Need to be verified and to check access rights.</param>
        /// 
        /// <returns><see cref="EventUserGoalsDto"/> record with goals for the given user.</returns>
        public EventUserGoalsDto FindUserGoalsByEventUidAndUserUid(string eventUidStr, string userUidStr)
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

                var goalsRepo = new EventUserGoalsRepository(db);

                var existingUserGoals = goalsRepo.GetUserGoalsForUserAndEvent(userUid, eventUid);

                Check.Value(existingUserGoals, "event_user_goals_uid", AvendErrors.NotFound).NotNull();

                return EventUserGoalsDto.From(existingUserGoals, eventUid);
            }
        }

        public async Task<EventUserGoalsDto> CreateOrUpdateUserGoal(string eventUidStr, EventUserGoalsDto dto)
        {
            Check.Value(UserContext, "user_uid", AvendErrors.InvalidUser).NotNull();
            Check.Value(UserContext.UserUid, "user_uid", AvendErrors.InvalidUser).NotNull();
            Check.Value(UserContext.Role, "user_uid", AvendErrors.Forbidden).NotEqualsTo(UserRole.SuperAdmin, "You're not allowed to set user goals for yourself");

            Check.Value(eventUidStr, "event_uid", AvendErrors.NotFound).NotNull();
            Check.Value(dto, "event_user_goals").NotNull();

            Guid eventUid;
            var eventUidIsValid = Guid.TryParse(eventUidStr, out eventUid);
            Check.Value(eventUidIsValid, "event_uid").IsTrue("Event uid in request route is not a valid GUID value");

            if (!dto.UserUid.HasValue)
                dto.UserUid = UserContext.UserUid;

            using (var db = new AvendDbContext(DbOptions))
            {
                var userGoalsWriter = new EventUserGoalsWriter(db);

                ValidateUserUidAccess(db, userGoalsWriter, dto.UserUid.Value);

                var goalsRepo = new EventUserGoalsRepository(db);

                var existingUserGoals = goalsRepo.GetUserGoalsForUserAndEvent(UserContext.UserUid, eventUid);

                if (existingUserGoals == null)
                    await DoWriteNewGoalFromDto(userGoalsWriter, dto);
                else
                {
                    dto.Uid = existingUserGoals.Uid;

                    Check.Value(existingUserGoals.Event.Uid as Guid?, "event_user_goals.event_uid", AvendErrors.InvalidParameter).EqualsTo(dto.EventUid, "Cannot change event UID for event_user_goals record");

                    await DoWriteUpdatedGoalFromDto(userGoalsWriter, dto);
                }

                return EventUserGoalsDto.From(userGoalsWriter.PreparedRecord, eventUid);
            }
        }

        public async Task<EventUserGoalsDto> UpdateUserGoal(string eventUidStr, EventUserGoalsDto dto)
        {
            Check.Value(UserContext, "user_uid", AvendErrors.InvalidUser).NotNull();
            Check.Value(UserContext.UserUid, "user_uid", AvendErrors.InvalidUser).NotNull();
            Check.Value(UserContext.Role, "user_uid", AvendErrors.Forbidden).NotEqualsTo(UserRole.SuperAdmin, "You're not allowed to set user goals for yourself");
            Check.Value(UserContext.Subscription, "user_uid", AvendErrors.InvalidUser).NotNull();

            Check.Value(eventUidStr, "event_uid", AvendErrors.NotFound).NotNull();

            Guid eventUid;
            var eventUidIsValid = Guid.TryParse(eventUidStr, out eventUid);
            Check.Value(eventUidIsValid, "event_uid").IsTrue("Event uid in request route is not a valid GUID value");

            Check.Value(dto, "event_user_goals").NotNull();
            Check.Value(dto.Uid, "event_user_goals.event_user_goals_uid").NotNull();

            if (!dto.UserUid.HasValue)
                dto.UserUid = UserContext.UserUid;

            using (var db = new AvendDbContext(DbOptions))
            {
                var userGoalsWriter = new EventUserGoalsWriter(db);

                ValidateUserUidAccess(db, userGoalsWriter, dto.UserUid.Value);

                var goalsRepo = new EventUserGoalsRepository(db);

                var existingUserGoals = goalsRepo.GetUserGoalsByUid(dto.UserUid.Value, dto.Uid);

                Check.Value(existingUserGoals, "event_user_goals.uid", AvendErrors.NotFound).NotNull("Cannot find event_user_goals record with this UID");
                Check.Value(existingUserGoals.Event.Uid as Guid?, "event_user_goals.event_uid", AvendErrors.InvalidParameter).EqualsTo(dto.EventUid, "Cannot change event UID for event_user_goals record");

                await DoWriteUpdatedGoalFromDto(userGoalsWriter, dto);

                return EventUserGoalsDto.From(userGoalsWriter.PreparedRecord, eventUid, dto.UserUid);
            }
        }

        private void ValidateUserUidAccess(AvendDbContext db, EventUserGoalsWriter userGoalsWriter, Guid userUid)
        {
            if (userUid == UserContext.UserUid)
            {
                if (userGoalsWriter != null)
                {
                    userGoalsWriter.SetAndValidateUserAndSubscriptionFromUserContext(UserContext);
                    userGoalsWriter.Validator.Throw();
                }

                return;
            }

            var subscriptionsRepo = new SubscriptionRepository(db);
            var userSubscription = subscriptionsRepo.FindSubscriptionForUser(userUid);

            if (UserContext.Role == UserRole.SuperAdmin)
            {
                if (userGoalsWriter != null)
                {
                    userGoalsWriter.SetAndValidateExplicitUserAndSubscription(userUid, userSubscription.Uid);
                    userGoalsWriter.Validator.Throw();
                }

                return;
            }

            Check.Value(UserContext.Role, "event_user_goals.user_uid", AvendErrors.Forbidden).EqualsTo(UserRole.Admin);

            Check.Value(userSubscription, "event_user_goals.user_uid", AvendErrors.NotFound).NotNull("User subscription is not found");
            Check.Value(userSubscription.Uid, "event_user_goals.user_uid", AvendErrors.NotFound)
                .EqualsTo(UserContext.Subscription.Uid.Value, "User not found in your tenant");

            if (userGoalsWriter != null)
            {
                userGoalsWriter.SetAndValidateExplicitUserAndSubscription(userUid, userSubscription.Uid);
                userGoalsWriter.Validator.Throw();
            }
        }

        private async Task DoWriteNewGoalFromDto(EventUserGoalsWriter userGoalsWriter, EventUserGoalsDto dto)
        {
            userGoalsWriter.SetRequestBody(dto);
            userGoalsWriter.Validator.Throw();

            await userGoalsWriter.SetEventIdFromRequestBody();
            userGoalsWriter.Validator.Throw();

            userGoalsWriter.MakeNewRecord();
            userGoalsWriter.Validator.Throw();

            await userGoalsWriter.SaveChanges();
            userGoalsWriter.Validator.Throw();
        }

        private async Task DoWriteUpdatedGoalFromDto(EventUserGoalsWriter userGoalsWriter, EventUserGoalsDto dto)
        {
            userGoalsWriter.SetRequestBody(dto);
            userGoalsWriter.ValidateRequestBody();
            userGoalsWriter.Validator.Throw();

            await userGoalsWriter.SetEventIdFromRequestBody();
            userGoalsWriter.Validator.Throw();

            userGoalsWriter.LoadRecordAndApplyChanges();
            userGoalsWriter.Validator.Throw();

            await userGoalsWriter.SaveChanges();
            userGoalsWriter.Validator.Throw();
        }
    }
}