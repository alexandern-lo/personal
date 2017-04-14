using System;
using SQLite;

namespace LiveOakApp.Models.Data.Records
{
    [Table(CacheItemsTableName)]
    public class CacheItemRecord
    {
        public const string CacheItemsTableName = "cache_items";

        public const string CacheItemsColumnKeyName = "key";
        public const string CacheItemsColumnDataName = "data";
        public const string CacheItemsColumnCreatedDateName = "created_date";

        [Column(CacheItemsColumnKeyName), PrimaryKey]
        public string Key { get; set; }

        [Column(CacheItemsColumnDataName)]
        public string Data { get; set; }

        [Column(CacheItemsColumnCreatedDateName)]
        public DateTimeOffset CreatedDate { get; set; }

        public CacheItemRecord()
        {
            CreatedDate = DateTimeOffset.Now;
        }
    }
}
