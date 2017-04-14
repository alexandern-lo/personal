using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Qoden.Validation;

namespace Avend.API.Services.Events
{
    public class AttendeeCategoriesService
    {
        private readonly DbContextOptions<AvendDbContext> _dbOptions;
        private readonly UserContext _userContext;

        public AttendeeCategoriesService(DbContextOptions<AvendDbContext> dbOptions, UserContext userContext)
        {
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();
            Assert.Argument(userContext, nameof(userContext)).NotNull();
            _dbOptions = dbOptions;
            _userContext = userContext;
        }

        public SearchResult<AttendeeCategoryDto> Find(Guid eventUid, SearchQueryParams searchParams)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var @event = FindEvent(eventUid, db);
                var repo = new AttendeeCategoriesRepository(db);
                return repo.Search(@event, searchParams)
                    .Paginate(x => AttendeeCategoryDto.From(x, @event.Uid));
            }
        }

        public async Task<AttendeeCategoryDto> FindByUid(Guid eventUid, Guid categoryUid)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var @event = FindEvent(eventUid, db);
                var repo = new AttendeeCategoriesRepository(db);
                var category = await repo.FindByUid(@event, categoryUid);
                return AttendeeCategoryDto.From(category, eventUid);
            }
        }

        public async Task<AttendeeCategoryDto> Create(Guid eventUid, AttendeeCategoryDto dto)
        {
            AssertSA();
            Check.Value(dto, "body").NotNull();

            using (var db = new AvendDbContext(_dbOptions))
            {
                var @event = FindEvent(eventUid, db);
                var repo = new AttendeeCategoriesRepository(db);
                var attendeeCategory = repo.CreateCategory(@event);
                ApplyChangesToModel(attendeeCategory, dto);
                await db.SaveChangesAsync();
                return AttendeeCategoryDto.From(attendeeCategory, eventUid);
            }
        }

        public async Task<AttendeeCategoryDto> Update(Guid eventUid, Guid categoryUid, AttendeeCategoryDto dto)
        {
            AssertSA();
            Check.Value(dto, "body").NotNull();

            using (var db = new AvendDbContext(_dbOptions))
            {
                var @event = FindEvent(eventUid, db);
                var repo = new AttendeeCategoriesRepository(db);
                var attendeeCategory = await repo.FindByUid(@event, categoryUid);
                ApplyChangesToModel(attendeeCategory, dto);
                await db.SaveChangesAsync();
                return AttendeeCategoryDto.From(attendeeCategory, eventUid);
            }
        }

        public async Task<Guid[]> MassDelete(Guid eventUid, Guid[] categoryUid)
        {
            AssertSA();

            using (var db = new AvendDbContext(_dbOptions))
            {
                var @event = FindEvent(eventUid, db);
                var repo = new AttendeeCategoriesRepository(db);
                var deleted = await repo.DeleteAll(@event, categoryUid);
                await db.SaveChangesAsync();
                return deleted;
            }
        }

        private void Validate(AttendeeCategoryDto dto)
        {
            var validator = new Validator();
            validator.CheckDataMember(dto, x => x.Name).IsShortText();
            for (var i = 0; i < dto.Options.Count; ++i)
            {
                var option = dto.Options[i];
                validator.CheckValue(option.Name, $"option[{i}].name").IsShortText();
            }
            validator.Throw();
        }

        private EventRecord FindEvent(Guid eventUid, AvendDbContext db)
        {
            var eventRepo = new EventsRepository(db) {Scope = _userContext.AvailableEvents()};
            var eventRecord = eventRepo.FindEventByUid(eventUid);
            Check.Value(eventRecord, "event_uid", AvendErrors.NotFound).NotNull("Event not found");
            return eventRecord;
        }

        private void AssertSA()
        {
            Check.Value(_userContext.Role, "role", AvendErrors.Forbidden)
                .EqualsTo(UserRole.SuperAdmin);
        }

        private void ApplyChangesToModel(AttendeeCategoryRecord category, AttendeeCategoryDto dto)
        {
            var validator = new Validator();

            if (dto.Name != null)
            {
                validator.CheckDataMember(dto, x => x.Name).IsShortText();
                category.Name = dto.Name;
            }

            if (dto.Options != null)
            {
                var createOrUpdate = dto.Options.Select((x, i) => new
                {
                    Index = i,
                    DtoOption = x,
                    RecordOption = category.Options.FirstOrDefault(o => o.Uid == x.Uid)
                });
                var remove = category.Options.Where(x => dto.Options.All(o => o.Uid != x.Uid)).ToList();

                foreach (var update in createOrUpdate)
                {
                    var dtoOption = update.DtoOption;
                    var recordOption = update.RecordOption;
                    var i = update.Index;
                    if (recordOption == null)
                    {
                        recordOption = new AttendeeCategoryOption()
                        {
                            Uid = Guid.NewGuid()
                        };
                        category.Options.Add(recordOption);
                    }
                    validator.CheckValue(dtoOption.Name, $"options[{i}].name").IsShortText();
                    recordOption.Name = dtoOption.Name;
                }
                
                category.Options.RemoveAll(x => remove.Contains(x));
            }

            validator.Throw();
        }
    }
}