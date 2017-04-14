using System;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Infrastructure.Validation;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Events;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;

namespace Avend.API.Services.Resources
{
    public class ResourcesService
    {
        private readonly DbContextOptions<AvendDbContext> _dbOptions;
        private readonly UserContext _userContext;

        public ResourcesService(DbContextOptions<AvendDbContext> dbOptions, UserContext userContext)
        {
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();
            Assert.Argument(userContext, nameof(userContext)).NotNull();
            _dbOptions = dbOptions;
            _userContext = userContext;
        }

        /// <summary>
        /// Search for resource with given parameters. Result ordered by Name by default.
        /// </summary>
        /// <param name="searchQuery">search query</param>
        /// <param name="user">optional user filter</param>
        /// <param name="tenant">optional tenant filter (for SA only)</param>
        /// <returns></returns>
        public async Task<SearchResult<ResourceDto>> Search(SearchQueryParams searchQuery, Guid? user, Guid? tenant)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var repo = CreateRepo(db);
                
                 if (searchQuery.SortField == "type")
                {
                    searchQuery.SortField = "mime_type";
                }
                
                var search = repo.Search(searchQuery);
                search.Filter(collection =>
                {
                    if (user != null)
                    {
                        collection = collection.Where(x => x.User.Uid == user);
                    }
                    if (tenant != null)
                    {
                        collection = collection.Where(x => x.User.Subscription.Uid == tenant);
                    }
                    return collection;
                });

                return (await search.PaginateAsync(ResourceDto.From))
                    .WithFilter("user", user)
                    .WithFilter("tenant", tenant);
            }
        }

        private ResourcesRepository CreateRepo(AvendDbContext db)
        {
            return new ResourcesRepository(db) {Scope = _userContext.AvailableResources(db)};
        }

        public async Task<ResourceDto> FindByUid(Guid resourceUid)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var repo = CreateRepo(db);
                var resource = await repo.FindByUid(resourceUid);
                Check.Value(resource, "resource", AvendErrors.NotFound).NotNull();
                return ResourceDto.From(resource);
            }
        }

        public async Task<ResourceDto> Create(ResourceDto dto)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var repo = CreateRepo(db);
                var resource = repo.Create();
                resource.UserId = _userContext.UserId;
                resource.UserUid = _userContext.UserUid;
                UpdateResource(db, resource, dto);
                await db.SaveChangesAsync();
                return ResourceDto.From(resource, _userContext.Member, _userContext.Tenant);
            }
        }

        public async Task<ResourceDto> Update(Guid resourceUid, ResourceDto dto)
        {
            Check.Value(dto, "body").NotNull();
            using (var db = new AvendDbContext(_dbOptions))
            {
                var repo = CreateRepo(db);
                var resource = await repo.FindByUid(resourceUid);
                Check.Value(resource, "resource").NotNull(onError: AvendErrors.NotFound);
                UpdateResource(db, resource, dto);
                await db.SaveChangesAsync();
                return ResourceDto.From(resource);
            }
        }


        public async Task Delete(Guid resourceUid)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var repo = CreateRepo(db);
                repo.Scope = _userContext.AvailableResources(db);
                var resource = await repo.FindByUid(resourceUid);
                Check.Value(resource, onError: AvendErrors.NotFound).NotNull("resource not found");
                repo.Delete(resource);
                await db.SaveChangesAsync();
            }
        }

        public async Task MassDelete(Guid[] resources)
        {
            using (var db = new AvendDbContext(_dbOptions))
            {
                var repo = CreateRepo(db);
                repo.Scope = _userContext.AvailableResources(db);
                foreach (var resource in repo.FindByUids(resources))
                {
                    repo.Delete(resource);
                }
                await db.SaveChangesAsync();
            }
        }

        private void UpdateResource(AvendDbContext db, Resource resource, ResourceDto dto)
        {
            if (dto.EventUid.HasValue)
            {
                var eventRepo = new EventsRepository(db) {Scope = _userContext.AvailableEvents()};

                var @event = eventRepo.FindEventByUid(dto.EventUid.Value);
                Check.Value(@event, "event_uid").NotNull(onError: AvendErrors.NotFound);
                resource.Event = @event;
            }

            var validator = new Validator();

            if (dto.Name != null)
                resource.Name = dto.Name;
            validator.CheckColumn(resource, x => x.Name).IsShortText();

            if (dto.Description != null)
                resource.Description = dto.Description;

            if (dto.Url != null)
            {
                resource.Url = dto.Url;
                validator.CheckColumn(resource, x => x.Url)
                    .ConvertTo(x => new Uri(x))
                    .IsAbsoluteUri();
            }

            if (dto.MimeType != null)
                resource.MimeType = dto.MimeType;

            validator.Throw();
        }
    }
}