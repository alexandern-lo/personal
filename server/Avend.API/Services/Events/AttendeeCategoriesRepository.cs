using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Avend.API.Infrastructure.SearchExtensions;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Model;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class AttendeeCategoriesRepository
    {
        public AvendDbContext Db { get; }

        public AttendeeCategoriesRepository(AvendDbContext db)
        {
            Assert.Argument(db, nameof(db)).NotNull();
            Db = db;
        }

        public Expression<Func<EventRecord, bool>> Scope { get; set; }

        public DefaultSearch<AttendeeCategoryRecord> Search(EventRecord @event, SearchQueryParams searchQuery)
        {
            Assert.Argument(@event, nameof(@event)).NotNull();
            Assert.Argument(searchQuery, nameof(searchQuery)).NotNull();

            var categories = Db.AttendeeCategories
                .Include(x => x.Options)
                .NotDeleted()
                .Where(x => x.EventId == @event.Id);

            return new DefaultSearch<AttendeeCategoryRecord>(searchQuery, categories);
        }

        public Task<AttendeeCategoryRecord> FindByUid(EventRecord @event, Guid categoryUid)
        {
            Assert.Argument(@event, nameof(@event)).NotNull();

            return Db.AttendeeCategories
                .Include(x => x.Options)
                .NotDeleted()
                .FirstOrDefaultAsync(x => x.EventId == @event.Id && x.Uid == categoryUid);
        }


        public AttendeeCategoryRecord CreateCategory(EventRecord @event)
        {
            Assert.Argument(@event, nameof(@event)).NotNull();

            var category = new AttendeeCategoryRecord()
            {
                Uid = Guid.NewGuid(),
                EventRecord = @event,
            };
            Db.AttendeeCategories.Add(category);
            return category;
        }


        public async Task<AttendeeCategoryOption> CreateCategoryOption(AttendeeCategoryRecord category,
            string categoryValueStr)
        {
            Assert.Argument(category, nameof(category)).NotNull();

            await Db.Entry(category).Collection(x => x.Options).LoadAsync();

            var option = new AttendeeCategoryOption()
            {
                AttendeeCategory = category,
                Name = categoryValueStr,
                Uid = Guid.NewGuid()
            };
            Db.AttendeeCategoryOptions.Add(option);
            return option;
        }

        public Task<List<AttendeeCategoryRecord>> FindCategoriesByNames(EventRecord @event,
            IEnumerable<string> categoryNames)
        {
            return Db.AttendeeCategories
                .NotDeleted()
                .Where(x => categoryNames.Contains(x.Name) && x.EventRecord == @event)
                .ToListAsync();
        }

        public Task<Guid[]> DeleteAll(EventRecord @event, Guid[] categoryUid, bool soft = true)
        {
            Assert.Argument(@event, nameof(@event)).NotNull();

            var toDelete = Db.AttendeeCategories
                .NotDeleted()
                .Where(x => categoryUid.Contains(x.Uid));

            if (soft)
            {
                foreach (var categoryRecord in toDelete)
                {
                    categoryRecord.Deleted = true;
                }
            }
            else
            {
                Db.AttendeeCategories.RemoveRange(toDelete);
            }

            return toDelete.Select(x => x.Uid).ToArrayAsync();
        }
    }
}