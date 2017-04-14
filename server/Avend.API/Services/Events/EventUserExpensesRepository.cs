using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class EventUserExpensesRepository
    {
        public AvendDbContext Db { get; }

        public EventUserExpensesRepository(AvendDbContext db)
        {
            Assert.Argument(db, nameof(db)).NotNull();

            Db = db;
        }

        public EventUserExpenseRecord FindByUid(Guid? userExpensesUid)
        {
            Assert.Argument(userExpensesUid, nameof(userExpensesUid)).NotNull();

            var expense = Db.EventUserExpensesTable
                .FirstOrDefault(record => record.Uid == userExpensesUid);

            return expense;
        }

        public async Task<MoneyDto> GetTotalEventExpensesAmountForUserAndEvent(Guid userUid, long eventId)
        {
            Assert.Argument(eventId, nameof(eventId)).Greater(0);

            var totalValue = new MoneyDto();

            var firstExpense = Db.EventUserExpensesTable
                .FirstOrDefault(x => x.UserUid == userUid
                                     && x.EventId == eventId);

            if (firstExpense == null)
                return new MoneyDto()
                {
                    Amount = 0M,
                    Currency = CurrencyCode.USD,
                };

            totalValue.Currency = firstExpense.Currency;

            totalValue.Amount = await Db.EventUserExpensesTable
                .Where(
                    x => x.UserUid == userUid
                         && x.EventId == eventId
                )
                .SumAsync(x => x.Amount);

            return totalValue;
        }

        public IQueryable<EventUserExpenseRecord> GetUserExpensesForUserAndEvent(Guid userUid, Guid eventUid)
        {
            var expense = Db.EventUserExpensesTable.Include(record => record.EventRecord)
                .Where(record => record.UserUid == userUid
                                 && record.EventRecord.Uid == eventUid);

            return expense;
        }

        public EventUserExpenseRecord GetUserExpensesByUid(Guid userUid, Guid? expensesUid)
        {
            var expense = Db.EventUserExpensesTable.Include(record => record.EventRecord)
                .FirstOrDefault(record => record.UserUid == userUid
                                          && record.Uid == expensesUid);

            return expense;
        }
    }
}