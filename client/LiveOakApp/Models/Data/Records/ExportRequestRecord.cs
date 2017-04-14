using System;
using SQLite;

namespace LiveOakApp.Models.Data.Records
{
    [Table(ExportRequestsTableName)]
    public class ExportRequestRecord
    {
        public const string ExportRequestsTableName = "export_requests";
        public const string ExportRequestsColumnLeadIdName = "lead_id";

        [Column(ExportRequestsColumnLeadIdName), Unique]
        public int LeadId { get; set; }

    }
}
