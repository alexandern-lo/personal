using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avend.API.Infrastructure.Responses;
using Avend.API.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Avend.API.Controllers
{
    /// <summary>
    /// Base controller for application endpoints that utilize database access.
    ///	Database options are passed via DI mechanism provided by AspNetCore.
    /// </summary>
    public class CrudController<TEntity, TEntityDto> : BaseController where TEntity : BaseRecord, new()
        where TEntityDto : new()
    {
        protected string EntityName;
        protected string EntityUidName;

        public CrudController(DbContextOptions<AvendDbContext> options, Dictionary<string, string> sortDictionary) :
            base(options)
        {
            EntityName = typeof(TEntity).Name.ToLower();
            EntityUidName = $"{EntityName}_uid";
        }

        [NonAction]
        protected static IQueryable<TEntity> ConstructRecordsListQuery(
            IQueryable<TEntity> dbTable,
            int? pageNumber,
            int? pageSize,
            string sortPropertyName,
            bool isAscending
            )
        {
            var query = dbTable;

            if (sortPropertyName != null)
            {
                query = query.OrderBy(sortPropertyName, isAscending);
            }

            if (pageNumber != null && pageNumber > 0 && pageSize != null && pageSize > 0)
                query = query.Skip(pageNumber.Value * pageSize.Value);

            if (pageSize != null && pageSize > 0)
                query = query.Take(pageSize.Value);

            return query;
        }

        [NonAction]
        public async Task<IActionResult> GetEntityByUid([FromRoute] Guid? uid)
        {
            if (!UserUid.HasValue)
                return UnauthorizedWithCodeAndBody(401, ErrorResponse.GenerateInvalidUser("user_uid"));

            if (uid == null)
            {
                return NotFound(ErrorResponse.GenerateRequiredParameter(typeof(EventAgendaItem), "attendee_uid"));
            }

            Logger.LogInformation($@"Trying to get {EntityName} for UID '{uid}'");

            using (var db = GetDatabaseService())
            {
                var table = GetEntityTableWithDependents(db);

                var query = from entityRec in table where entityRec.Uid == uid select entityRec;

                var entityItem = await query.FirstOrDefaultAsync();

                if (entityItem == null)
                {
                    return NotFound(ErrorResponse.GenerateNotFound(typeof(TEntity), uid.Value, EntityUidName));
                }

                var entityDto = GetDtoFromEntity(entityItem);

                var responseObj = new OkResponse<TEntityDto>()
                {
                    Data = entityDto,
                };

                return Ok(responseObj);
            }
        }

        [NonAction]
        protected virtual TEntityDto GetDtoFromEntity(TEntity entityItem)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        protected virtual IQueryable<TEntity> GetEntityTableWithDependents(AvendDbContext db)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        protected static async Task<long> GetEventIdFromUid(Guid? eventUid, IQueryable<EventRecord> eventsTable)
        {
            var eventId = await (from events in eventsTable
                                 where events.Uid == eventUid
                                 select events.Id).FirstOrDefaultAsync();

            return eventId;
        }
    }
}