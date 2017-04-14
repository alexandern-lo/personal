using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Avend.API.Infrastructure.SearchExtensions;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Model;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;

namespace Avend.API.Services.Crm
{
    public class CrmConfigurationRepository
    {
        private readonly AvendDbContext _db;

        public CrmConfigurationRepository(AvendDbContext db)
        {
            Assert.Argument(db, nameof(db)).NotNull();
            _db = db;
            Scope = x => true;
        }

        public DefaultSearch<CrmRecord> Search(SearchQueryParams search)
        {
            var configurations = _db.Crms
                .Include(x => x.Settings)
                .Where(Scope);
            return new DefaultSearch<CrmRecord>(search, configurations);
        }

        public Expression<Func<CrmRecord, bool>> Scope { get; set; }

        public CrmRecord NewCrm(Guid userUid)
        {
            var crmConfiguration = new CrmRecord
            {
                Uid = Guid.NewGuid(),
                UserUid = userUid,
            };
            _db.Crms.Add(crmConfiguration);
            return crmConfiguration;
        }

        public Task <CrmRecord> FindByUid(Guid crmConfigUid)
        {
            return _db.Crms
                .Include(x => x.Settings)
                .Where(Scope)
                .FirstOrDefaultAsync(x => x.Uid == crmConfigUid);
        }

        public void Delete(CrmRecord config)
        {
            _db.Remove(config);
        }
    }
}