using System.Collections.Generic;
using System.Threading.Tasks;
using SL4N;
using LiveOakApp.Models.Data.Records;

namespace LiveOakApp.Models.Services
{
    public class CacheStorage
    {
        static readonly ILogger LOG = LoggerFactory.GetLogger<CacheStorage>();

        readonly SQLiteDatabase database;
        readonly JsonService Json;

        public CacheStorage(JsonService JsonService)
        {
            database = new SQLiteDatabase("cache");
            Json = JsonService;
            CreateDatabaseIfNeccesarry();
        }

        public async Task<bool> InsertObject<T>(string key, T obj)
        {
            var item = new CacheItemRecord();
            item.Key = key;
            item.Data = Json.Serialize(obj);

            var conn = database.GetConnection();
            var inserted = await conn.InsertOrReplaceAsync(item);
            return inserted != 0;
        }

        public async Task<T> GetObject<T>(string key)
        {
            var conn = database.GetConnection();
            var item = await conn.FindAsync<CacheItemRecord>(key);
            if (item == null) throw new KeyNotFoundException(key);
            return Json.Deserialize<T>(item.Data);
        }

        public Task DropObject(string key)
        {
            var conn = database.GetConnection();
            var query = string.Format("DELETE FROM {0} WHERE {1} = ?",
                                      CacheItemRecord.CacheItemsTableName,
                                      CacheItemRecord.CacheItemsColumnKeyName);
            return conn.ExecuteAsync(query, key);
        }

        public Task DropAllObjects()
        {
            var conn = database.GetConnection();
            var query = string.Format("DELETE FROM {0}",
                                      CacheItemRecord.CacheItemsTableName);
            return conn.ExecuteAsync(query, new object[0]);
        }

        void CreateDatabaseIfNeccesarry()
        {
            var conn = database.GetConnection();
            conn.RunInTransactionAsync((db) =>
            {
                db.CreateTable<CacheItemRecord>();
            }).Wait();
        }

        public async Task DropAllObjectsIfUserDiffers(string userId)
        {
            const string CachedUserIdKey = "CachedUserKey";

            string cachedUserId;
            try
            {
                cachedUserId = await GetObject<string>(CachedUserIdKey);
            }
            catch (KeyNotFoundException)
            {
                LOG.Debug("empty cachedUserId, writing {0}", userId);
                await InsertObject(CachedUserIdKey, userId);
                return;
            }
            if (userId != cachedUserId)
            {
                LOG.Debug("different cachedUserId {0} != userId {1}, cleaning cache", cachedUserId, userId);
                await ServiceLocator.Instance.DropAllCaches();
                await InsertObject(CachedUserIdKey, userId);
            }
        }
    }
}
