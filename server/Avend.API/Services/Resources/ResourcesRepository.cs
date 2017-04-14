using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Avend.API.Infrastructure.SearchExtensions;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Model;
using Avend.API.Services.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;

namespace Avend.API.Services.Resources
{
    public class ResourcesRepository
    {
        private readonly AvendDbContext _db;

        public ResourcesRepository(AvendDbContext db)
        {
            Assert.Argument(db, nameof(db)).NotNull();
            _db = db;
        }

        public Expression<Func<Resource, bool>> Scope { get; set; } = x => true;

        
        public DefaultSearch<Resource> Search(SearchQueryParams searchQuery)
        {
            var resources = _db.ResourcesTable
                .Include(x => x.Event)
                .Include(x => x.User)
                .ThenInclude(u => u.Subscription)
                .OrderBy(x => x.Name)
                .NotDeleted()
                .Where(Scope);
            return DefaultSearch.Start(searchQuery, resources);                
        }

        /// <summary>
        /// Fetch one resource with Event and User
        /// </summary>
        /// <param name="resourceUid">resource uid</param>
        /// <returns></returns>
        public Task<Resource> FindByUid(Guid resourceUid)
        {
            var resources = _db.ResourcesTable
                .Include(x => x.Event)
                .Include(x => x.User)
                .ThenInclude(u => u.Subscription)
                .NotDeleted()
                .Where(Scope);
            return resources.FirstOrDefaultAsync(x => x.Uid == resourceUid);
        }

        /// <summary>
        /// Fetch several resources without any associations
        /// </summary>
        /// <param name="resources">resources uids</param>
        /// <returns></returns>
        public IQueryable<Resource> FindByUids(Guid[] resources)
        {
            Assert.Argument(resources, nameof(resources)).NotNull();
            return _db.ResourcesTable
                .Include(x => x.User)
                .ThenInclude(u => u.Subscription)
                .NotDeleted()
                .Where(Scope)
                .Where(x => resources.Contains(x.Uid));
        }

        /// <summary>
        /// Create empty resource for given event
        /// </summary>
        /// <returns></returns>
        public Resource Create()
        {
            var resource = new Resource
                {
                    Uid = Guid.NewGuid(),
                    Status = ResourceStatus.Valid,
                };
            _db.Add(resource);
            return resource;            
        }

        /// <summary>
        /// Delete resource. Method does not check if resources matches Scope.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="soft"></param>
        public void Delete(Resource resource, bool soft = true)
        {
            Assert.Argument(resource, nameof(resource)).NotNull();
            if (soft)
            {
                resource.Deleted = true;
            }
            else
            {
                _db.ResourcesTable.Remove(resource);
            }
        }
    }
}