using System;
using System.Linq;
using Avend.API.Model;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class EventUserGoalsRepository
    {
        public AvendDbContext Db { get; }

        public EventUserGoalsRepository(AvendDbContext db)
        {
            Assert.Argument(db, nameof(db)).NotNull();

            Db = db;
        }

        public EventUserGoalsRecord FindByUid(Guid? userGoalsUid)
        {
            Assert.Argument(userGoalsUid, nameof(userGoalsUid)).NotNull();

            var goal = Db.EventUserGoalsTable
                .FirstOrDefault(record => record.Uid == userGoalsUid);

            return goal;
        }

        public EventUserGoalsRecord GetUserGoalsForUserAndEvent(Guid userUid, Guid eventUid)
        {
            var goal = Db.EventUserGoalsTable.Include(record => record.Event)
                .FirstOrDefault(record => record.UserUid == userUid
                                          && record.Event.Uid == eventUid);

            return goal;
        }

        public EventUserGoalsRecord GetUserGoalsByUid(Guid userUid, Guid? goalsUid)
        {
            var goal = Db.EventUserGoalsTable.Include(record => record.Event)
                .FirstOrDefault(record => record.UserUid == userUid
                                          && record.Uid == goalsUid);

            return goal;
        }
    }
}