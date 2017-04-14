using System;
using System.Linq;
using System.Threading.Tasks;

using Avend.ApiTests.Infrastructure;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API.Model;

namespace Avend.ApiTests.DataSamples
{
    public class BaseEntityData<TRecord, TDTO>
        where TRecord : BaseRecord
        where TDTO : class
    {
        public string ControllerPath { get; }

        public TestSystem System { get; }
        public TestUser User { get; }

        /// <summary>
        /// Constructor that sets up required internal fields.
        /// </summary>
        /// 
        /// <param name="user">User that will be sending the requests</param>
        /// <param name="system">Test System to use</param>
        /// <param name="controllerPath">Path to the entity endpoint so that we can send requests in a unified way</param>
        public BaseEntityData(TestUser user, TestSystem system, string controllerPath)
        {
            User = user;
            System = system;

            ControllerPath = controllerPath;
        }

        /// <summary>
        /// Adds the DTO passed over to the system and 
        /// returns an object with all proper fields filled.
        /// </summary>
        /// 
        /// <param name="entity">A DTO to add into the system</param>
        /// 
        /// <returns>A DTO object that has all auxiliary fields populated by server.</returns>
        public virtual async Task<TDTO> AddFromDto(TDTO entity)
        {
            return await Send(entity);
        }

        /// <summary>
        /// Adds the DTO passed over to the system and 
        /// returns an object with all proper fields filled.
        /// </summary>
        /// 
        /// <param name="postProcessor">Postprocessing action that allow to set certain fields to required values</param>
        /// 
        /// <returns>A DTO object that has all auxiliary fields populated by server.</returns>
        public virtual async Task<TDTO> AddFromSample(Action<TDTO> postProcessor = null)
        {
            var entity = MakeSample(postProcessor);

            return await Send(entity);
        }

        /// <summary>
        /// Actually sends request to system's test server.
        /// Expects to get the Guid back and then requests
        /// valid DTO with all fields filled by server.
        /// </summary>
        /// 
        /// <param name="entity">A DTO to add into the system</param>
        /// 
        /// <returns>A DTO object that has all auxiliary fields populated by server.</returns>
        protected virtual async Task<TDTO> Send(TDTO entity)
        {
            using (var http = System.CreateClient(User.Token))
            {
                var leadUid = await http.PostJsonAsync(ControllerPath, entity).AvendResponse<Guid>();

                var leadDto = await http.GetJsonAsync(ControllerPath + "/" + leadUid).AvendResponse<TDTO>();

                return leadDto;
            }
        }

        /// <summary>
        /// Uses DTO with Uid filled in to change the created date and time
        /// for the corresponding record in the database.
        /// </summary>
        /// 
        /// <param name="entityUid">A Uid of the record to update creation time for</param>
        /// <param name="newDateTime">New creation date and time to set for the record</param>
        /// 
        /// <returns>An updated record from database.</returns>
        public TRecord UpdateDbRecordCreationTime(Guid? entityUid, DateTime newDateTime)
        {
            TRecord record;

            using (var services = System.GetServices())
            {
                var db = services.GetService<AvendDbContext>();

                record = db.Set<TRecord>().First(l => l.Uid == entityUid.GetValueOrDefault());

                record.CreatedAt = newDateTime;

                db.SaveChanges();
            }

            return record;
        }

        /// <summary>
        /// Generates a new sample DTO. 
        /// Suggested way to implement is to use sequences 
        /// so that all produced objects are different.
        /// </summary>
        /// 
        /// <param name="postProcessor">Postprocessing action that allow to set certain fields to required values</param>
        /// 
        /// <returns>A freshly generated DTO that is not yet added to system</returns>
        /// 
        /// <exception cref="NotImplementedException">Should be implemented in all descendants.</exception>
        public virtual TDTO MakeSample(Action<TDTO> postProcessor = null)
        {
            throw new NotImplementedException("CreateSample is not implemented for " + nameof(TRecord));
        }
    }
}